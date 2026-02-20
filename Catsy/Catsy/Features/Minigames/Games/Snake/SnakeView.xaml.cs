using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if WINDOWS
using Microsoft.Maui.Platform;
#endif

namespace Catsy.Features.Minigames.Games.Snake;

public partial class SnakeView : ContentView
{
    private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
    {
        { GridValue.Empty, Images.Empty },
        { GridValue.Snake, Images.SnakeBody },
        { GridValue.Food, Images.Food },
    };

    private readonly Dictionary<Direction, int> dirToRotation = new()
    {
        { Direction.Up, 0 },
        { Direction.Right, 90 },
        { Direction.Down, 180 },
        { Direction.Left, 270 }
    };

    private readonly int rows = 12;
    private readonly int cols = 12;
    private readonly Image[,] gridImages;

    private GameState gameState = null!;
    private readonly InputState input = new();

    private bool isRunning;
    private Task? gameLoopTask;

    public SnakeView(SnakeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        gridImages = SetupGrid();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        ResetGame();
        ShowOverlay("Press any key to start");
    }

    // =========================
    // INPUT ENTRY POINT
    // =========================
    public void HandleInput(Direction direction)
    {
        if (!isRunning)
        {
            ResetGame();
            gameLoopTask = GameLoop();
            return;
        }

        input.Press(direction);
    }

    // =========================
    // GAME LOOP
    // =========================
    private async Task GameLoop()
    {
        isRunning = true;
        HideOverlay();

        while (!gameState.GameOver)
        {
            await Task.Delay(120);

            gameState.ApplyInput(input);
            gameState.Move();
            Draw();
        }

        isRunning = false;
        ShowOverlay("Game Over\nPress any key to restart");
    }

    // =========================
    // GAME RESET
    // =========================
    private void ResetGame()
    {
        gameState = new GameState(rows, cols);
        input.Clear();
        Draw();
    }

    // =========================
    // GRID SETUP
    // =========================
    private Image[,] SetupGrid()
    {
        Image[,] images = new Image[rows, cols];

        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();
        GameGrid.Children.Clear();

        for (int r = 0; r < rows; r++)
            GameGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

        for (int c = 0; c < cols; c++)
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Image img = new Image
                {
                    Source = Images.Empty,
                    Aspect = Aspect.Fill,
                    AnchorX = 0.5,
                    AnchorY = 0.5
                };

                images[r, c] = img;
                GameGrid.Children.Add(img);
                Grid.SetRow(img, r);
                Grid.SetColumn(img, c);
            }
        }

        return images;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if WINDOWS
        SnakeInputRouter.CurrentSnakeView = this;
#endif
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
#if WINDOWS
        if (SnakeInputRouter.CurrentSnakeView == this)
            SnakeInputRouter.CurrentSnakeView = null;
#endif
    }

    // =========================
    // DRAW
    // =========================
    private void Draw()
    {
        DrawGrid();
        DrawSnake();
        ScoreText.Text = $"Score: {gameState.Score}";
    }


    private void DrawGrid()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GridValue val = gameState.Grid[r, c];
                Image img = gridImages[r, c];

                img.Source = gridValToImage[val];
                img.Rotation = 0;
            }
        }
    }

    private void DrawSnake()
    {
        var positions = gameState.SnakePositions().ToList();

        if (positions.Count == 0)
            return;

        // ===== HEAD =====
        var head = positions[0];
        var headImg = gridImages[head.Row, head.Col];
        headImg.Source = Images.SnakeHead;
        headImg.Rotation = dirToRotation[gameState.Dir];

        if (positions.Count == 1)
            return;

        // ===== TAIL =====
        var tail = positions[^1];
        var beforeTail = positions[^2];

        var tailImg = gridImages[tail.Row, tail.Col];
        tailImg.Source = Images.SnakeTail;

        Direction tailDir = GetDirection(tail, beforeTail);
        tailImg.Rotation = dirToRotation[tailDir];

        // ===== BODY =====
        // ===== BODY =====
        for (int i = 1; i < positions.Count - 1; i++)
        {
            var prev = positions[i - 1];
            var current = positions[i];
            var next = positions[i + 1];

            var bodyImg = gridImages[current.Row, current.Col];
            bodyImg.Source = Images.SnakeBody;

            var dirFromPrev = GetDirection(current, prev);
            var dirToNext = GetDirection(current, next);

            // ===== Egyenes =====
            if ((dirFromPrev == Direction.Left && dirToNext == Direction.Right) ||
                (dirFromPrev == Direction.Right && dirToNext == Direction.Left))
            {
                bodyImg.Rotation = 90; // vízszintes
            }
            else if ((dirFromPrev == Direction.Up && dirToNext == Direction.Down) ||
                     (dirFromPrev == Direction.Down && dirToNext == Direction.Up))
            {
                bodyImg.Rotation = 0; // függõleges
            }
            else
            {
                // ===== KANYAR =====
                bodyImg.Rotation = GetCornerRotation(dirFromPrev, dirToNext);
            }
        }

    }
    private int GetCornerRotation(Direction from, Direction to)
    {
        if ((from == Direction.Up && to == Direction.Left) ||
            (from == Direction.Left && to == Direction.Up))
            return 0;

        if ((from == Direction.Up && to == Direction.Right) ||
            (from == Direction.Right && to == Direction.Up))
            return 90;

        if ((from == Direction.Down && to == Direction.Right) ||
            (from == Direction.Right && to == Direction.Down))
            return 180;

        return 270;
    }

    private Direction GetDirection(Position from, Position to)
    {
        return Direction.FromOffset(
            to.Row - from.Row,
            to.Col - from.Col);
    }



    // =========================
    // OVERLAY
    // =========================
    private void ShowOverlay(string text)
    {
        Overlay.IsVisible = true;
        OverlayText.Text = text;
    }

    private void HideOverlay()
    {
        Overlay.IsVisible = false;
    }

    // =========================
    // MOBILE BUTTONS
    // =========================
    private void UpClicked(object sender, EventArgs e)
        => HandleInput(Direction.Up);

    private void DownClicked(object sender, EventArgs e)
        => HandleInput(Direction.Down);

    private void LeftClicked(object sender, EventArgs e)
        => HandleInput(Direction.Left);

    private void RightClicked(object sender, EventArgs e)
        => HandleInput(Direction.Right);
}
