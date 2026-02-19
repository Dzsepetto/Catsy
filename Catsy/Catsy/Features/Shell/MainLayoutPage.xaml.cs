using Catsy.Features.Shell;

namespace Catsy;

public partial class MainLayoutPage : ContentPage
{
    const double PhoneW = 420;
    const double PhoneH = 900;

    public MainLayoutPage(MainLayoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0 || height <= 0)
            return;

        var availableW = width - 24;
        var availableH = height - 24;

        var scaleX = availableW / PhoneW;
        var scaleY = availableH / PhoneH;
        var scale = Math.Min(1.0, Math.Min(scaleX, scaleY));

        PhoneContainer.Scale = scale;
    }
}