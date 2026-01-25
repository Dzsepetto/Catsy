using System.Collections.ObjectModel;
using Catsy.Features.Models;

namespace Catsy
{
    public sealed class ShopViewModel
    {
        public ObservableCollection<ShopItem> Items { get; } = new();

        public ShopViewModel()
        {
            // sample items — only presentation data, no buy/owned logic
            Items.Add(new ShopItem
            {
                Id = "cat_default",
                Name = "Classic Cat",
                Price = 0,
                Icon = "btnpink.png"
            });

            Items.Add(new ShopItem
            {
                Id = "cat_purple",
                Name = "Purple Cat",
                Price = 500,
                Icon = "Shop/cat_purple.png"
            });

            Items.Add(new ShopItem
            {
                Id = "cat_gold",
                Name = "Golden Cat",
                Price = 1000,
                Icon = "Shop/cat_gold.png"
            });
        }
    }
}