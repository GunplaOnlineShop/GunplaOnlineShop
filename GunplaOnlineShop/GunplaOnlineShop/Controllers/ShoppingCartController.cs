using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GunplaOnlineShop.Controllers
{
    [Route("/cart/{action}")]
    public class ShoppingCartController : BaseController
    {
        public ShoppingCartController(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentLoggedInUser();
            if (currentUser == null)
            {
                // not logged in
                // take out the shopping cart data from cookie 

                return View();
            }
            else
            {
                // logged in
                var shoppingCartForThisUser = await _context.ShoppingCartLineItems.Where(li => li.CustomerId == currentUser.Id).ToListAsync();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemToShoppingCart([Bind("ItemId,Quantity")]AddItemViewModel lineItem)
        {
            var item = await _context.Items.FindAsync(lineItem.ItemId);
            if (item == null) return BadRequest();
            List<AddItemViewModel> ShoppingCartLineItems = new List<AddItemViewModel>();
            if (HttpContext.Request.Cookies["GunplaShopShoppingCart"] != null)
            {
                // extract items from cookie if exists
                string shoppingCartCookie = HttpContext.Request.Cookies["GunplaShopShoppingCart"];
                // deserialize the cookie
                ShoppingCartLineItems = JsonSerializer.Deserialize<List<AddItemViewModel>>(shoppingCartCookie);
            }

            if (ModelState.IsValid)
            {
                // if user inputs are valid
                // add the new line item to the shopping cart
                var existingLineItemInCookie = ShoppingCartLineItems.Where(li => li.ItemId == lineItem.ItemId).FirstOrDefault();
                if (existingLineItemInCookie != null)
                {
                    existingLineItemInCookie.Quantity += lineItem.Quantity;
                }
                else
                {
                    ShoppingCartLineItems.Add(lineItem);
                }
            }
            else
            {
                var model = new ProductViewModel();
                model.Item = item;
                model.lineItem = lineItem;
                return View("/Views/Collection/Products.cshtml",model);
            }
            var customer = await GetCurrentLoggedInUser();
            if (customer == null)
            {
                // not logged in
                // save the shopping cart to the cookie
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(3);
                HttpContext.Response.Cookies.Append("GunplaShopShoppingCart", JsonSerializer.Serialize(ShoppingCartLineItems), option);
            }
            else
            {
                // logged in
                foreach (var shoppingCartLineItem in ShoppingCartLineItems)
                {
                    var scli = _context.ShoppingCartLineItems.Where(li => li.CustomerId == customer.Id && li.ItemId == shoppingCartLineItem.ItemId).FirstOrDefault();
                    if (scli != null)
                    {
                        scli.Quantity += shoppingCartLineItem.Quantity;
                    }
                    else
                    {
                        _context.ShoppingCartLineItems.Add(new ShoppingCartLineItem
                        {
                            ItemId = shoppingCartLineItem.ItemId,
                            Quantity = shoppingCartLineItem.Quantity,
                            CustomerId = customer.Id
                        });
                    }    
                }
                await _context.SaveChangesAsync();
                HttpContext.Response.Cookies.Delete("GunplaShopShoppingCart");
            }

            return RedirectToAction("Index");
        }
    }
}
