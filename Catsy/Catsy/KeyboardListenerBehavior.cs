#if WINDOWS
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

namespace Catsy;

public sealed class KeyboardListenerBehavior : Behavior<ContentPage>
{
#if WINDOWS
    FrameworkElement? _root;
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

        var window = page.Handler?.MauiContext?
            .Services
            .GetService(typeof(Microsoft.UI.Xaml.Window))
            as Microsoft.UI.Xaml.Window;

        if (window?.Content is not FrameworkElement root)
            return;

        _root = root;

        _root.KeyDown += OnKeyDown;
        _root.IsTabStop = true;
        _root.Focus(FocusState.Programmatic);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine(e.Key);
        SnakeInputRouter.HandleKey(e.Key);
    }
#endif

    protected override void OnDetachingFrom(ContentPage bindable)
    {
#if WINDOWS
        if (_root != null)
            _root.KeyDown -= OnKeyDown;
#endif
        base.OnDetachingFrom(bindable);
    }
}
