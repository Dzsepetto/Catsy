using Microsoft.Maui;
using Microsoft.Maui.Controls;

#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
#endif

namespace Catsy;

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

                // induló méret (még mindig portrait jellegű, de utána szabadon méretezhető)
                appWindow.Resize(new SizeInt32(600, 1000));

                appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsResizable = true;     // <-- EZ kell neked
                    presenter.IsMaximizable = true;   // opcionális, de általában jó
                    presenter.IsMinimizable = true;
                }

                nativeWindow.Activate();
            }
        };
#endif

        return window;
    }
}