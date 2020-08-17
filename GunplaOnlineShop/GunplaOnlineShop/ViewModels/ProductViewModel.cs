using GunplaOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class ProductViewModel
    {
        public Item Item { get; set; }
        public AddItemViewModel lineItem { get; set; } = new AddItemViewModel();
    }
}
