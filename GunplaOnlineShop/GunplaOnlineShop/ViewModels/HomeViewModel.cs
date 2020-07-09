using GunplaOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class HomeViewModel
    {
        public List<Item> HgItems { get; set; }
        public List<Item> MgItems { get; set; }
        public List<Item> PgItems { get; set; }
        public List<Item> BestSellingItems { get; set; }
        public List<Item> NewItems { get; set; }

    }
}
