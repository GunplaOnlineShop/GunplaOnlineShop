using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class ItemCategory
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int CateogoryId { get; set; }
        public Category Category { get; set; }
    }
}
