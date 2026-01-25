using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Devices;

#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Maui.Platform;
#endif

namespace Catsy.Features.Minigames.Games;

public partial class LaneRunnerView : ContentView
{
    private readonly GameWorld _world = new();
    private readonly GameDrawable _drawable;

    private IDispatcherTimer? _timer;
    private DateTime _lastTick;

    private double _panAccumX;
    private bool _swipeConsumed;

    public LaneRunnerView(LaneRunnerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        _drawable = new GameDrawable(_world);
        GameView.Drawable = _drawable;

        // Swipe (mobil)
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        GameView.GestureRecognizers.Add(pan);

        // Desktopon gombok elrejtése
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            ControlsGrid.IsVisible = false;

#if WINDOWS
        AttachKeyboardWindows();
#endif

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
        _timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
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

    // ====== MOBIL GOMBOK ======
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

    private void OnRestartClicked(object sender, EventArgs e)
        => StartGame();

    // ====== SWIPE (MOBIL) ======
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

#if WINDOWS
private void AttachKeyboardWindows()
{
    var mauiWindow = Microsoft.Maui.Controls.Application.Current?
        .Windows.FirstOrDefault();

    if (mauiWindow?.Handler?.PlatformView is not MauiWinUIWindow winuiWindow)
        return;

    if (winuiWindow.Content is Microsoft.UI.Xaml.FrameworkElement root)
    {
        root.KeyDown += OnWindowsKeyDown;
        root.Focus(FocusState.Programmatic); // 👈 NAGYON FONTOS
    }
}

private void OnWindowsKeyDown(object sender, KeyRoutedEventArgs e)
{
    if (_world.IsGameOver) return;

    switch (e.Key)
    {
        case Windows.System.VirtualKey.Left:
        case Windows.System.VirtualKey.A:
            _world.MoveLeft();
            break;

        case Windows.System.VirtualKey.Right:
        case Windows.System.VirtualKey.D:
            _world.MoveRight();
            break;

        case Windows.System.VirtualKey.R:
            StartGame();
            break;
    }
}
#endif

}
