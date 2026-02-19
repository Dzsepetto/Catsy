namespace Catsy;

#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
#endif

public partial class App : Application
{
    private readonly MainLayoutPage _mainLayoutPage;

    public App(MainLayoutPage mainLayoutPage)
    {
        InitializeComponent();
        _mainLayoutPage = mainLayoutPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(_mainLayoutPage);

#if WINDOWS
window.HandlerChanged += (_, __) =>
{
    if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
    {
        var hWnd = WindowNative.GetWindowHandle(nativeWindow);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        // induló méret (csak induláskor!)
        appWindow.Resize(new SizeInt32(500, 900));

        nativeWindow.Activate();
    }
};
#endif


        return window;
    }
}
