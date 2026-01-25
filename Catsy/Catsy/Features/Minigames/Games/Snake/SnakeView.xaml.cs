using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Devices;
using System;

#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Maui.Platform;
#endif

namespace Catsy.Features.Minigames.Games.Snake;

public partial class SnakeView : ContentView
{
    private readonly GameWorld _world = new();
    private readonly GameDrawable _drawable;

    private IDispatcherTimer? _timer;
    private DateTime _lastTick;

    private double _panAccumX;
    private double _panAccumY;
    private bool _swipeConsumed;

    public SnakeView(SnakeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        _drawable = new GameDrawable(_world);
        GameView.Drawable = _drawable;

        // Swipe (mobile)
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        GameView.GestureRecognizers.Add(pan);

        // Keep on-screen controls visible by default; hide if you prefer:
        // if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        //     ControlsGrid.IsVisible = false;

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

            ScoreLabel.Text = $"Score: {_world.Score}";
            StateLabel.Text = _world.IsGameOver ? "GAME OVER" : "";

            GameView.Invalidate();
        };
        _timer.Start();
    }

    // ====== MOBILE BUTTONS ======
    private void OnLeftClicked(object sender, EventArgs e)
    {
        if (_world.IsGameOver) return;
        _world.SetDirectionLeft();
    }

    private void OnRightClicked(object sender, EventArgs e)
    {
        if (_world.IsGameOver) return;
        _world.SetDirectionRight();
    }

    private void OnUpClicked(object sender, EventArgs e)
    {
        if (_world.IsGameOver) return;
        _world.SetDirectionUp();
    }

    private void OnDownClicked(object sender, EventArgs e)
    {
        if (_world.IsGameOver) return;
        _world.SetDirectionDown();
    }

    private void OnRestartClicked(object sender, EventArgs e)
        => StartGame();

    // ====== SWIPE (MOBILE) ======
    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (_world.IsGameOver) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _panAccumX = 0;
                _panAccumY = 0;
                _swipeConsumed = false;
                break;

            case GestureStatus.Running:
                if (_swipeConsumed) return;

                _panAccumX = e.TotalX;
                _panAccumY = e.TotalY;

                if (Math.Abs(_panAccumX) > Math.Abs(_panAccumY))
                {
                    if (_panAccumX <= -35)
                    {
                        _world.SetDirectionLeft();
                        _swipeConsumed = true;
                    }
                    else if (_panAccumX >= 35)
                    {
                        _world.SetDirectionRight();
                        _swipeConsumed = true;
                    }
                }
                else
                {
                    if (_panAccumY <= -35)
                    {
                        _world.SetDirectionUp();
                        _swipeConsumed = true;
                    }
                    else if (_panAccumY >= 35)
                    {
                        _world.SetDirectionDown();
                        _swipeConsumed = true;
                    }
                }
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _panAccumX = 0;
                _panAccumY = 0;
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
            root.Focus(FocusState.Programmatic); // important
        }
    }

    private void OnWindowsKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (_world.IsGameOver)
        {
            if (e.Key == Windows.System.VirtualKey.R)
                StartGame();
            return;
        }

        switch (e.Key)
        {
            case Windows.System.VirtualKey.Left:
            case Windows.System.VirtualKey.A:
                _world.SetDirectionLeft();
                break;

            case Windows.System.VirtualKey.Right:
            case Windows.System.VirtualKey.D:
                _world.SetDirectionRight();
                break;

            case Windows.System.VirtualKey.Up:
            case Windows.System.VirtualKey.W:
                _world.SetDirectionUp();
                break;

            case Windows.System.VirtualKey.Down:
            case Windows.System.VirtualKey.S:
                _world.SetDirectionDown();
                break;

            case Windows.System.VirtualKey.R:
                StartGame();
                break;
        }
    }
#endif
}