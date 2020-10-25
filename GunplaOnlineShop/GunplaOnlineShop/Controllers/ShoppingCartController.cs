using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace GunplaOnlineShop.Controllers
{
    [Route("/cart/{action}")]
    public class ShoppingCartController : BaseController
    {
        public ShoppingCartController(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }

        [Route("/cart")]
        [Route("/cart/{action}")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentLoggedInUser();
            if (currentUser == null)
            {
                // not logged in
                // take out the shopping cart data from cookie 
                List<AddItemViewModel> ShoppingCartLineItems = new List<AddItemViewModel>();
                if (HttpContext.Request.Cookies["GunplaShopShoppingCart"] != null)
                {
                    // extract items from cookie if exists
                    string shoppingCartCookie = HttpContext.Request.Cookies["GunplaShopShoppingCart"];
                    // deserialize the cookie
                    ShoppingCartLineItems = JsonSerializer.Deserialize<List<AddItemViewModel>>(shoppingCartCookie);
                }
                var upgrade = new ShoppingCartViewModel(_context, ShoppingCartLineItems);
                var model = new CartItemsPassViewModel();
                foreach (var item in upgrade.ShoppingCartItems)
                {
                    model.ShoppingCartItems.Add(new ShoppingCartItem
                    {
                        ItemId = item.ItemId,
                        Item = item.Item,
                        Quantity = item.Quantity,
                        Total = item.Total
                    });
                }
                model.Total = upgrade.Total;
                return View(model);
            }
            else
            {
                // logged in
                
                var upgrade = new ShoppingCartViewModel(_context, currentUser.Id);
                var model = new CartItemsPassViewModel();
                foreach (var item in upgrade.ShoppingCartItems)
                {
                    model.ShoppingCartItems.Add(new ShoppingCartItem
                    {
                        ItemId = item.ItemId,
                        Item = item.Item,
                        Quantity = item.Quantity,
                        Total = item.Total
                    });
                }
                model.Total = upgrade.Total;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCart(CartItemsPassViewModel model)
        {

            List<AddItemViewModel> ShoppingCartLineItems = new List<AddItemViewModel>();

            if (ModelState.IsValid) {
                var customer = await GetCurrentLoggedInUser();
                if (customer == null)
                {
                    foreach (var ShoppingCartItem in model.ShoppingCartItems)
                    {
                        if (ShoppingCartItem.Quantity > 0)
                        {
                            ShoppingCartLineItems.Add(new AddItemViewModel
                            {
                                ItemId = ShoppingCartItem.ItemId,
                                Quantity = ShoppingCartItem.Quantity,
                            });
                        }

                    }
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(3);
                    option.HttpOnly = true;
                    HttpContext.Response.Cookies.Append("GunplaShopShoppingCart", JsonSerializer.Serialize(ShoppingCartLineItems), option);
                }
                else
                {
                    foreach (var shoppingCartItem in model.ShoppingCartItems) 
                    {
                        if (await _context.Items.FindAsync(shoppingCartItem.ItemId) == null)
                        {
                            continue;
                        }
                        var scli = _context.ShoppingCartLineItems
                            .Where(li => li.CustomerId == customer.Id
                                    && li.ItemId == shoppingCartItem.ItemId)
                            .FirstOrDefault();
                        if (scli != null)
                        {
                            if (shoppingCartItem.Quantity > 0)
                            {
                                scli.Quantity = shoppingCartItem.Quantity;
                            }
                            else // delete CartItem from db since quantity is <=0
                            {
                                _context.ShoppingCartLineItems.Remove(scli);
                            }
                        }
                        else
                        {
                            _context.ShoppingCartLineItems.Add(new ShoppingCartLineItem
                            {
                                ItemId = shoppingCartItem.ItemId,
                                Quantity = shoppingCartItem.Quantity,
                                CustomerId = customer.Id
                            });
                        }

                    }
                    await _context.SaveChangesAsync();
                    HttpContext.Response.Cookies.Delete("GunplaShopShoppingCart");
                }
                

            }
            return RedirectToAction("Index");            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemToShoppingCart([Bind("ItemId,Quantity")]AddItemViewModel lineItem)
        {
            //find item
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
                return View("~/Views/Collection/Products.cshtml",model);
            }
            var customer = await GetCurrentLoggedInUser();
            if (customer == null)
            {
                // not logged in
                // save the shopping cart to the cookie
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(3);
                option.HttpOnly = true;
                HttpContext.Response.Cookies.Append("GunplaShopShoppingCart", JsonSerializer.Serialize(ShoppingCartLineItems), option);
            }
            else
            {
                // logged in
                foreach (var shoppingCartLineItem in ShoppingCartLineItems)
                {
                    if (await _context.Items.FindAsync(shoppingCartLineItem.ItemId) == null)
                    {
                        continue;
                    }
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
