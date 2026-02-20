namespace Catsy;

public partial class HomeView : ContentView
{
    public HomeView()
    {
        InitializeComponent();
        BindingContext = new HomeViewModel();
    }
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if WINDOWS
    if (Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement fe)
    {
        fe.PointerWheelChanged += (s, e) =>
        {
            e.Handled = true; // letiltjuk a wheel scrollt
        };
    }
#endif
    }

}
