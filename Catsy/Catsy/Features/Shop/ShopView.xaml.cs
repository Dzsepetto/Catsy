using Microsoft.Maui.Controls;

namespace Catsy
{
    public partial class ShopView : ContentView
    {
        private readonly ShopViewModel _vm;

        public ShopView(ShopViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = vm;
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            _vm.ShowMenu();
        }

        private void OnBackgroundClicked(object sender, EventArgs e)
        {
            _vm.ShowCategory(ShopCategory.Background);
        }

        private void OnAccessoriesClicked(object sender, EventArgs e)
        {
            _vm.ShowCategory(ShopCategory.Accessories);
        }

        private void OnBoostsClicked(object sender, EventArgs e)
        {
            _vm.ShowCategory(ShopCategory.Boosts);
        }
    }
}