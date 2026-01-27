using Microsoft.Maui.Controls;

#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

namespace Catsy;

public sealed class KeyboardListenerBehavior : Behavior<ContentPage>
{
#if WINDOWS
    FrameworkElement? _nativeView;
#endif

    protected override void OnAttachedTo(ContentPage bindable)
    {
        base.OnAttachedTo(bindable);

#if WINDOWS
        bindable.HandlerChanged += OnHandlerChanged;
#endif
    }

#if WINDOWS
    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is not ContentPage page)
            return;

        _nativeView = page.Handler?.PlatformView as FrameworkElement;

        if (_nativeView == null)
            return;

        _nativeView.KeyDown += OnKeyDown;
        _nativeView.IsTabStop = true;
        _nativeView.Focus(FocusState.Programmatic);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        SnakeInputRouter.HandleKey(e.Key);
    }
#endif

    protected override void OnDetachingFrom(ContentPage bindable)
    {
#if WINDOWS
        if (_nativeView != null)
            _nativeView.KeyDown -= OnKeyDown;
#endif
        base.OnDetachingFrom(bindable);
    }
}
