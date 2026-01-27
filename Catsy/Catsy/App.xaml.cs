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

                // 📱 Telefon arány
                var size = new SizeInt32(420, 900);
                appWindow.Resize(size);

                // 🔒 Fixálás (nincs SetMinSize → workaround)
                appWindow.Changed += (_, args) =>
                {
                    if (args.DidSizeChange)
                    {
                        if (appWindow.Size.Width != size.Width ||
                            appWindow.Size.Height != size.Height)
                        {
                            appWindow.Resize(size);
                        }
                    }
                };

                nativeWindow.Activate(); // 🔑 fókusz → keyboard listener működik
            }
        };
#endif

        return window;
    }
}
