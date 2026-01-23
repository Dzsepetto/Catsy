using Microsoft.Maui.Graphics;

namespace Catsy.Features.Minigames.Games;

public sealed class GameWorld
{
    private readonly Random _rng = new();
    public const int LaneCount = 3;

    public int PlayerLane { get; private set; } = 1;
    public float PlayerYRatio { get; private set; } = 0.82f;

    private readonly List<Obstacle> _obstacles = new();
    private float _spawnTimer;
    private float _spawnInterval = 0.70f;
    private float _speed = 420f;
    private float _difficultyTimer;

    public float DifficultyStepSeconds = 3.0f;
    public float SpeedMultiplierPerStep = 1.05f;
    public float SpawnIntervalMultiplierPerStep = 0.95f;
    public float MinSpawnInterval = 0.32f;

    public int Score { get; private set; }
    private float _scoreFloat;

    private const float ScorePerSecond = 20f;
    private const int ScorePerPassedObstacle = 50;

    public bool IsGameOver { get; private set; }
    public IReadOnlyList<Obstacle> Obstacles => _obstacles;

    public void Reset()
    {
        PlayerLane = 1;
        _obstacles.Clear();
        _spawnTimer = 0;
        _spawnInterval = 0.70f;
        _speed = 420f;
        _difficultyTimer = 0;
        IsGameOver = false;

        Score = 0;
        _scoreFloat = 0;
    }

    public void MoveLeft() => PlayerLane = Math.Max(0, PlayerLane - 1);
    public void MoveRight() => PlayerLane = Math.Min(LaneCount - 1, PlayerLane + 1);

    public void Update(float dt, float width, float height)
    {
        if (width <= 0 || height <= 0) return;
        if (IsGameOver) return;

        _scoreFloat += dt * ScorePerSecond;
        Score = (int)_scoreFloat;

        _difficultyTimer += dt;
        if (_difficultyTimer >= DifficultyStepSeconds)
        {
            _difficultyTimer -= DifficultyStepSeconds;

            _speed *= SpeedMultiplierPerStep;
            _spawnInterval = Math.Max(
                MinSpawnInterval,
                _spawnInterval * SpawnIntervalMultiplierPerStep
            );
        }

        _spawnTimer += dt;
        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0;
            SpawnObstacle();
        }

        float bottom = height + 200;

        for (int i = _obstacles.Count - 1; i >= 0; i--)
        {
            _obstacles[i].Y += _speed * dt;

            if (_obstacles[i].Y > bottom)
            {
                _scoreFloat += ScorePerPassedObstacle;
                Score = (int)_scoreFloat;

                _obstacles.RemoveAt(i);
            }
        }

        var playerRect = GetPlayerRect(width, height);
        foreach (var o in _obstacles)
        {
            var r = GetObstacleRect(o, width, height);
            if (playerRect.IntersectsWith(r))
            {
                IsGameOver = true;
                break;
            }
        }
    }
    private void SpawnObstacle()
    {
        int lane = _rng.Next(0, LaneCount);
        if (_obstacles.Count > 0 && _obstacles[^1].Lane == lane && _rng.NextDouble() < 0.6)
            lane = (lane + _rng.Next(1, LaneCount)) % LaneCount;

        _obstacles.Add(new Obstacle { Lane = lane, Y = -120f });
    }

    public RectF GetPlayerRect(float width, float height)
    {
        float laneW = width / LaneCount;
        float blockW = laneW * 0.55f;
        float blockH = Math.Min(height * 0.10f, 90f);

        float cx = (PlayerLane + 0.5f) * laneW;
        float y = height * PlayerYRatio;

        return new RectF(cx - blockW / 2f, y - blockH / 2f, blockW, blockH);
    }

    public RectF GetObstacleRect(Obstacle o, float width, float height)
    {
        float laneW = width / LaneCount;
        float blockW = laneW * 0.60f;
        float blockH = Math.Min(height * 0.11f, 100f);

        float cx = (o.Lane + 0.5f) * laneW;
        float y = o.Y;

        return new RectF(cx - blockW / 2f, y - blockH / 2f, blockW, blockH);
    }
}

public sealed class Obstacle
{
    public int Lane { get; set; }
    public float Y { get; set; }
}

public sealed class GameDrawable : IDrawable
{
    private readonly GameWorld _world;
    public GameDrawable(GameWorld world) => _world = world;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float w = dirtyRect.Width;
        float h = dirtyRect.Height;

        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(dirtyRect);

        canvas.StrokeColor = Colors.DimGray;
        canvas.StrokeSize = 4;

        float laneW = w / GameWorld.LaneCount;
        for (int i = 1; i < GameWorld.LaneCount; i++)
        {
            float x = i * laneW;
            canvas.DrawLine(x, 0, x, h);
        }

        var p = _world.GetPlayerRect(w, h);
        canvas.FillColor = _world.IsGameOver ? Colors.DarkRed : Colors.DeepSkyBlue;
        canvas.FillRoundedRectangle(p, 12);

        canvas.FillColor = Colors.Orange;
        foreach (var o in _world.Obstacles)
        {
            var r = _world.GetObstacleRect(o, w, h);
            canvas.FillRoundedRectangle(r, 12);
        }
    }
}
