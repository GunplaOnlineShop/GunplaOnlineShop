using GunplaOnlineShop.Models;
using GunplaOnlineShop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class HomeViewModel
    {
        public int[] RootCategoryIds { get; set; }
        public List<Item> HgItems { get; set; }
        public List<Item> MgItems { get; set; }
        public List<Item> PgItems { get; set; }
        public List<Item> BestSellingItems { get; set; }
        public List<Item> NewItems { get; set; }

        public string getItemCategoryName(Item item)
        {
            return item.ItemCategories.Where(ic => RootCategoryIds.Contains(ic.Category.ParentCategoryId??=1)).Select(ic => ic.Category.Name).FirstOrDefault().NameEncode();
        }
    }
}
