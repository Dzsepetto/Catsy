using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Catsy.Features.Minigames.Games.Snake
{
    public readonly record struct PointI(int X, int Y);

    public sealed class GameWorld
    {
        private readonly Random _rng = new();
        public const int Columns = 10;
        public const int Rows = 10;

        private readonly List<PointI> _snake = new();
        public IReadOnlyList<PointI> Snake => _snake;

        public PointI Food { get; private set; }

        public bool IsGameOver { get; private set; }

        private float _moveTimer;
        private const float DefaultMoveInterval = 0.5f; // seconds per step (slow)
        public float MoveInterval { get; set; } = DefaultMoveInterval;

        public int Score { get; private set; }

        private Direction _direction = Direction.Right;
        private Direction _nextDirection = Direction.Right;

        private enum Direction { Up, Down, Left, Right }

        public void Reset()
        {
            _snake.Clear();
            int startX = 2;
            int startY = Rows / 2;

            // starting snake length 2
            for (int i = 0; i < 2; i++)
                _snake.Add(new PointI(startX - i, startY));

            _direction = Direction.Right;
            _nextDirection = Direction.Right;
            IsGameOver = false;
            Score = 0;
            _moveTimer = 0;
            SpawnFood();

            // restore default speed on reset
            MoveInterval = DefaultMoveInterval;
        }

        public void SetDirectionUp() => TrySetDirection(Direction.Up);
        public void SetDirectionDown() => TrySetDirection(Direction.Down);
        public void SetDirectionLeft() => TrySetDirection(Direction.Left);
        public void SetDirectionRight() => TrySetDirection(Direction.Right);

        private void TrySetDirection(Direction dir)
        {
            // Prevent reversing directly
            if ((_direction == Direction.Left && dir == Direction.Right) ||
                (_direction == Direction.Right && dir == Direction.Left) ||
                (_direction == Direction.Up && dir == Direction.Down) ||
                (_direction == Direction.Down && dir == Direction.Up))
                return;

            _nextDirection = dir;
        }

        public void Update(float dt, float width, float height)
        {
            if (IsGameOver) return;
            if (width <= 0 || height <= 0) return;

            _moveTimer += dt;
            if (_moveTimer < MoveInterval) return;

            _moveTimer -= MoveInterval;
            Step();
        }

        private void Step()
        {
            _direction = _nextDirection;

            var head = _snake[0];
            PointI delta = _direction switch
            {
                Direction.Up => new PointI(0, -1),
                Direction.Down => new PointI(0, 1),
                Direction.Left => new PointI(-1, 0),
                Direction.Right => new PointI(1, 0),
                _ => new PointI(1, 0)
            };

            var newHead = new PointI(head.X + delta.X, head.Y + delta.Y);

            // wall collision
            if (newHead.X < 0 || newHead.X >= Columns || newHead.Y < 0 || newHead.Y >= Rows)
            {
                IsGameOver = true;
                return;
            }

            // self collision
            for (int i = 0; i < _snake.Count; i++)
            {
                if (_snake[i].X == newHead.X && _snake[i].Y == newHead.Y)
                {
                    IsGameOver = true;
                    return;
                }
            }

            _snake.Insert(0, newHead);

            // food?
            if (newHead.X == Food.X && newHead.Y == Food.Y)
            {
                Score += 10;
                // optional speedup
                MoveInterval = Math.Max(0.04f, MoveInterval * 0.985f);
                SpawnFood();
            }
            else
            {
                // normal move: remove tail
                _snake.RemoveAt(_snake.Count - 1);
            }
        }

        private void SpawnFood()
        {
            // find random free cell
            if (_snake.Count >= Columns * Rows)
            {
                // full board — win (treat as game over)
                IsGameOver = true;
                return;
            }

            while (true)
            {
                var p = new PointI(_rng.Next(0, Columns), _rng.Next(0, Rows));
                bool occupied = false;
                foreach (var s in _snake)
                {
                    if (s.X == p.X && s.Y == p.Y) { occupied = true; break; }
                }
                if (!occupied)
                {
                    Food = p;
                    return;
                }
            }
        }

        public RectF GetCellRect(int x, int y, float width, float height)
        {
            float cellW = width / Columns;
            float cellH = height / Rows;
            return new RectF(x * cellW, y * cellH, cellW, cellH);
        }
    }

    public sealed class GameDrawable : IDrawable
    {
        private readonly GameWorld _world;

        // use fully-qualified graphics IImage to avoid ambiguity
        private readonly Microsoft.Maui.Graphics.IImage? _imgHead;
        private readonly Microsoft.Maui.Graphics.IImage? _imgBody;
        private readonly Microsoft.Maui.Graphics.IImage? _imgTail;

        public GameDrawable(GameWorld world)
        {
            _world = world;

            // Try a few candidate paths (most robust across platforms/build setups)
            _imgHead = TryLoadImage("Games/Snake/cat01_head.png");
            _imgBody = TryLoadImage("Games/Snake/cat01_body.png");
            _imgTail = TryLoadImage("Games/Snake/cat01_tail.png");
        }

        private static Microsoft.Maui.Graphics.IImage? TryLoadImage(string relativePath)
        {
            // Try packaged path first (may succeed on some platforms)
            try
            {
                using Stream s = FileSystem.OpenAppPackageFileAsync(relativePath).GetAwaiter().GetResult();
                if (s != null)
                {
                    Debug.WriteLine($"[Snake] Loaded '{relativePath}' from app package.");
                    return PlatformImage.FromStream(s);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Snake] OpenAppPackageFile failed for '{relativePath}': {ex.Message}");
            }

            // If not found, search the build output for any file that starts with the base name
            try
            {
                var outDir = AppContext.BaseDirectory ?? Environment.CurrentDirectory;
                var baseName = Path.GetFileNameWithoutExtension(relativePath); // e.g. "cat01_tail"
                Debug.WriteLine($"[Snake] Searching output '{outDir}' for files starting with '{baseName}'");

                // find files like cat01_tail*.png (will match cat01_tail.scale-100.png etc.)
                foreach (var f in Directory.EnumerateFiles(outDir, $"{baseName}*.png", SearchOption.AllDirectories))
                {
                    Debug.WriteLine($"[Snake] Found candidate in output: {f}");
                    try
                    {
                        using var fs = File.OpenRead(f);
                        Debug.WriteLine($"[Snake] Loaded '{f}' from output folder.");
                        return PlatformImage.FromStream(fs);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[Snake] Failed to load candidate '{f}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Snake] Output search failed: {ex.Message}");
            }

            Debug.WriteLine($"[Snake] All candidates failed for '{relativePath}'");
            return null;
        }

        private static float DirectionToAngleDeg(int dx, int dy)
        {
            // dx,dy expected to be -1,0,1
            if (dx == 1 && dy == 0) return 0f;      // right
            if (dx == -1 && dy == 0) return 180f;   // left
            if (dx == 0 && dy == 1) return 90f;     // down
            if (dx == 0 && dy == -1) return 270f;   // up
            return 0f;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float w = dirtyRect.Width;
            float h = dirtyRect.Height;

            // background
            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(dirtyRect);

            // faint grid
            canvas.StrokeColor = Colors.DimGray;
            canvas.StrokeSize = 1;
            float cellW = w / GameWorld.Columns;
            float cellH = h / GameWorld.Rows;
            for (int i = 1; i < GameWorld.Columns; i++)
                canvas.DrawLine(i * cellW, 0, i * cellW, h);
            for (int j = 1; j < GameWorld.Rows; j++)
                canvas.DrawLine(0, j * cellH, w, j * cellH);

            // draw food (keep as colored rect if no image)
            var fRect = _world.GetCellRect(_world.Food.X, _world.Food.Y, w, h);
            canvas.FillColor = Colors.OrangeRed;
            canvas.FillRoundedRectangle(fRect, Math.Min(cellW, cellH) * 0.2f);

            // draw snake using images when available, otherwise fallback to colored rects
            for (int i = 0; i < _world.Snake.Count; i++)
            {
                var s = _world.Snake[i];
                var r = _world.GetCellRect(s.X, s.Y, w, h);

                // choose image: head, body, tail
                var img = (i == 0) ? _imgHead : (i == _world.Snake.Count - 1) ? _imgTail : _imgBody;

                if (img is not null)
                {
                    // compute orientation for this segment
                    int dx = 1, dy = 0; // default right

                    if (i == 0) // head: forward = head - next
                    {
                        if (_world.Snake.Count > 1)
                        {
                            var next = _world.Snake[1];
                            dx = s.X - next.X;
                            dy = s.Y - next.Y;
                        }
                    }
                    else if (i == _world.Snake.Count - 1) // tail: forward = tail - prev
                    {
                        var prev = _world.Snake[i - 1];
                        dx = s.X - prev.X;
                        dy = s.Y - prev.Y;
                    }
                    else // body: use direction from prev -> current
                    {
                        var prev = _world.Snake[i - 1];
                        dx = s.X - prev.X;
                        dy = s.Y - prev.Y;
                    }

                    var angle = DirectionToAngleDeg(Math.Sign(dx), Math.Sign(dy));

                    // draw rotated around cell center
                    canvas.SaveState();
                    canvas.Translate(r.X + r.Width / 2f, r.Y + r.Height / 2f);
                    // rotate expects radians
                    canvas.Rotate((float)(angle * Math.PI / 180.0));
                    canvas.DrawImage(img, -r.Width / 2f, -r.Height / 2f, r.Width, r.Height);
                    canvas.RestoreState();
                }
                else
                {
                    // fallback colored rects
                    canvas.FillColor = i == 0 ? Colors.LimeGreen : Colors.ForestGreen;
                    canvas.FillRoundedRectangle(r, Math.Min(cellW, cellH) * 0.2f);
                }
            }
        }
    }
}