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
        DrawSnakeHead();
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

    private void DrawSnakeHead()
    {
        Position headPos = gameState.HeadPosition();
        Image image = gridImages[headPos.Row, headPos.Col];

        image.Source = Images.SnakeHead;
        image.Rotation = dirToRotation[gameState.Dir];
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
