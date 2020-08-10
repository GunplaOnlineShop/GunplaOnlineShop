using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GunplaOnlineShop.Controllers
{
    [Route("/cart/{action}")]
    public class ShoppingCartController : BaseController
    {
        public ShoppingCartController(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemToShoppingCart([Bind("ItemId,Quantity")] ShoppingCartLineItem lineItem)
        {
            var item = _context.Items.FindAsync(lineItem.ItemId);
            if (item == null) return BadRequest();

            // extract items from cookie


            if (await GetCurrentLoggedInUser() == null)
            {
                // not logged in
            }
            else
            {
                // logged in
            }

            return Ok();
        }
    }
}
