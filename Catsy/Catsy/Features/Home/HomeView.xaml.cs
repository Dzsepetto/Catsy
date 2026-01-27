using Microsoft.Maui.Controls;
using System;

#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endif

namespace Catsy;

public partial class HomeView : ContentView
{
    private bool _wheelCooldown;

    public HomeView()
    {
        InitializeComponent();
        BindingContext = new HomeViewModel();

#if WINDOWS
        Carousel.HandlerChanged += (_, __) =>
        {
            if (Carousel.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.ListView list)
            {
                list.PointerWheelChanged += OnPointerWheelChanged;
            }
        };
#endif
    }

#if WINDOWS
    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (_wheelCooldown)
        {
            e.Handled = true;
            return;
        }

        if (BindingContext is not HomeViewModel vm)
            return;

        int delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;

        if (delta < 0 && vm.CanSwipeRight)
        {
            vm.SelectedIndex++;
            StartCooldown();
        }
        else if (delta > 0 && vm.CanSwipeLeft)
        {
            vm.SelectedIndex--;
            StartCooldown();
        }

        // 🔒 SOHA ne engedjük tovább
        e.Handled = true;
    }

    private void StartCooldown()
    {
        _wheelCooldown = true;

        // megakadályozza az ugrálást / gyors scrollt
        Dispatcher.DispatchDelayed(
            TimeSpan.FromMilliseconds(250),
            () => _wheelCooldown = false);
    }
#endif
}
