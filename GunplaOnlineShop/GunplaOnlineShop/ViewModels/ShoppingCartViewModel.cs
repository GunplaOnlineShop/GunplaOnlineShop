using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class ShoppingCartViewModel
    {
        private AppDbContext _context;
        public List<ShoppingCartLineItemViewModel> ShoppingCartItems { get; set; } = new List<ShoppingCartLineItemViewModel>();
        public decimal Total { get; set; }

        public void PopulateShoppingCartItemsByLineItems(List<AddItemViewModel> lineItems)
        {
            foreach (var lineItem in lineItems)
            {
                var item = _context.Items.Find(lineItem.ItemId);
                var lineItemTotal = item.Price * lineItem.Quantity;
                ShoppingCartItems.Add(new ShoppingCartLineItemViewModel
                {
                    ItemId = lineItem.ItemId,
                    Item = item,
                    Quantity = lineItem.Quantity,
                    Total = lineItemTotal
                });
                Total += lineItemTotal;
            }
        }

        public void PopulateShoppingCartItemsByCustomerId(string customerId)
        {
            var lineItems = _context.ShoppingCartLineItems.Include(li => li.Item).Where(li => li.CustomerId == customerId).ToList();
            foreach (var lineItem in lineItems)
            {
                var lineItemTotal = lineItem.Item.Price * lineItem.Quantity;
                ShoppingCartItems.Add(new ShoppingCartLineItemViewModel
                {
                    ItemId = lineItem.ItemId,
                    Item = lineItem.Item,
                    Quantity = lineItem.Quantity,
                    Total = lineItemTotal
                });
                Total += lineItemTotal;
            }
        }

        public ShoppingCartViewModel(AppDbContext context, List<AddItemViewModel> lineItems)
        {
            _context = context;
            PopulateShoppingCartItemsByLineItems(lineItems);
        }

        public ShoppingCartViewModel(AppDbContext context, string customerId)
        {
            _context = context;
            PopulateShoppingCartItemsByCustomerId(customerId);
        }
    }

    public class ShoppingCartLineItemViewModel
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
