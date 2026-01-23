using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;

namespace Catsy.Features.Minigames.Games;

public partial class LaneRunnerView : ContentView
{
    private readonly GameWorld _world = new();
    private readonly GameDrawable _drawable;

    private IDispatcherTimer? _timer;
    private DateTime _lastTick;

    public LaneRunnerView(LaneRunnerViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;

        _drawable = new GameDrawable(_world);
        GameView.Drawable = _drawable;

        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        GameView.GestureRecognizers.Add(pan);

        StartGame();
    }

    private void StartGame()
    {
        _world.Reset();
        StateLabel.Text = "";
        ScoreLabel.Text = "Score: 0";

        _lastTick = DateTime.UtcNow;

        _timer?.Stop();
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(16);
        _timer.Tick += (_, __) =>
        {
            var now = DateTime.UtcNow;
            var dt = (float)(now - _lastTick).TotalSeconds;
            _lastTick = now;

            _world.Update(dt, (float)GameView.Width, (float)GameView.Height);

            ScoreLabel.Text = $"Score: {(int)_world.Score}";
            StateLabel.Text = _world.IsGameOver ? "GAME OVER" : "";

            GameView.Invalidate();
        };
        _timer.Start();
    }

    private void OnLeftClicked(object sender, EventArgs e)
    {
        if (_world.IsGameOver) return;
        _world.MoveLeft();
    }

    private void OnRightClicked(object sender, EventArgs e)
    {
        if (_world.IsGameOver) return;
        _world.MoveRight();
    }

    private void OnRestartClicked(object sender, EventArgs e) => StartGame();

    private double _panAccumX;
    private bool _swipeConsumed;

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (_world.IsGameOver) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _panAccumX = 0;
                _swipeConsumed = false;
                break;

            case GestureStatus.Running:
                if (_swipeConsumed) return;

                _panAccumX = e.TotalX;

                if (_panAccumX <= -35)
                {
                    _world.MoveLeft();
                    _swipeConsumed = true;
                }
                else if (_panAccumX >= 35)
                {
                    _world.MoveRight();
                    _swipeConsumed = true;
                }
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _panAccumX = 0;
                _swipeConsumed = false;
                break;
        }
    }
}
