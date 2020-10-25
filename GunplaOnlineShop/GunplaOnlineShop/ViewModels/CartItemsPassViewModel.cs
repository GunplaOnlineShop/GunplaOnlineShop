using GunplaOnlineShop.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class CartItemsPassViewModel
    {
        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
        public decimal Total { get; set; }
    }
    public class ShoppingCartItem
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
