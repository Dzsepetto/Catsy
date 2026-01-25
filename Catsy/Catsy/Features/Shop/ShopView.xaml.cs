using Microsoft.Maui.Controls;

namespace Catsy
{
    public partial class ShopView : ContentView
    {
        public ShopView(ShopViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}