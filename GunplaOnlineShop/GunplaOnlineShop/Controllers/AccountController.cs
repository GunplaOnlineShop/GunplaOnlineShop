using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GunplaOnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Register
        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, IsAdmin = false};
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddClaimAsync(user, new Claim("IsAdmin",user.IsAdmin.ToString(),ClaimValueTypes.Boolean));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // sync user shopping cart cookie data if there are any
                    List<AddItemViewModel> ShoppingCartLineItems = new List<AddItemViewModel>();
                    if (HttpContext.Request.Cookies["GunplaShopShoppingCart"] != null)
                    {
                        // extract items from cookie if exists
                        string shoppingCartCookie = HttpContext.Request.Cookies["GunplaShopShoppingCart"];
                        // deserialize the cookie
                        ShoppingCartLineItems = JsonSerializer.Deserialize<List<AddItemViewModel>>(shoppingCartCookie);
                    }
                    var customer = await _userManager.FindByNameAsync(model.Email);
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


                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }
    }
}
