using Catsy.Features.Shell;

namespace Catsy;

public partial class MainLayoutPage : ContentPage
{
    public MainLayoutPage(MainLayoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}



            