using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catsy.Features.Models
{
    public class ShopItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Icon { get; set; } // pl. sprite png
        public bool IsOwned { get; set; }
    }
}
