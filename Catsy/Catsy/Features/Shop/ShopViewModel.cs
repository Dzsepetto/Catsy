using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Catsy.Features.Models;

namespace Catsy
{
    public enum ShopCategory
    {
        None,
        Background,
        Accessories,
        Boosts
    }

    public sealed class ShopViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ShopItem> Items { get; } = new();

        private readonly List<ShopItem> _allItems = new();

        private bool _isCategoryMenu = true;
        public bool IsCategoryMenu
        {
            get => _isCategoryMenu;
            private set { _isCategoryMenu = value; OnPropertyChanged(); }
        }

        private bool _isInCategory;
        public bool IsInCategory
        {
            get => _isInCategory;
            private set { _isInCategory = value; OnPropertyChanged(); }
        }

        private string _headerTitle = "Shop";
        public string HeaderTitle
        {
            get => _headerTitle;
            private set { _headerTitle = value; OnPropertyChanged(); }
        }

        private ShopCategory _currentCategory = ShopCategory.None;
        public ShopCategory CurrentCategory
        {
            get => _currentCategory;
            private set { _currentCategory = value; OnPropertyChanged(); }
        }

        public ShopViewModel()
        {
            // ide jön az összes item - kategóriával együtt
            _allItems.Add(new ShopItem
            {
                Id = "bg_default",
                Name = "Classic Background",
                Price = 0,
                Icon = "Shop/bg_default.png",
                Category = "Background"
            });

            _allItems.Add(new ShopItem
            {
                Id = "acc_hat",
                Name = "Cute Hat",
                Price = 500,
                Icon = "Shop/acc_hat.png",
                Category = "Accessories"
            });

            _allItems.Add(new ShopItem
            {
                Id = "boost_x2",
                Name = "Coins x2 (5 min)",
                Price = 1000,
                Icon = "Shop/boost_x2.png",
                Category = "Boosts"
            });

            // induláskor: menü
            ShowMenu();
        }

        public void ShowMenu()
        {
            CurrentCategory = ShopCategory.None;
            HeaderTitle = "Shop";
            Items.Clear();

            IsCategoryMenu = true;
            IsInCategory = false;
        }

        public void ShowCategory(ShopCategory category)
        {
            CurrentCategory = category;

            HeaderTitle = category switch
            {
                ShopCategory.Background => "Background",
                ShopCategory.Accessories => "Accessories",
                ShopCategory.Boosts => "Boosts",
                _ => "Shop"
            };

            Items.Clear();

            string cat = HeaderTitle; // "Background" / "Accessories" / "Boosts"
            foreach (var item in _allItems.Where(x => x.Category == cat))
                Items.Add(item);

            IsCategoryMenu = false;
            IsInCategory = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}