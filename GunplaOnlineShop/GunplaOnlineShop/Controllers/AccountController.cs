using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GunplaOnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _dbContext;

        public AccountController(AppDbContext applicationDbContext, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = applicationDbContext;
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

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Invalid Login Attempt");
                }
            }
            return View(model);
        }

        // GET: /Account/OrderHistory
        [HttpGet]
        public IActionResult OrderHistory()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Security()
        {
            var user = await _userManager.GetUserAsync(User); //not sure if it can get the user
            var model = new SecurityViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                Password = user.PasswordHash
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Security(SecurityViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.PasswordHash = model.Password;
                user.PhoneNumber = model.Phone;
            };
            var result = await _userManager.UpdateAsync(user);
            return View(model);
        }

        [HttpGet]
        public IActionResult NewAddress()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewAddress(NewAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                MailingAddress address = new MailingAddress();
                address.FirstName = model.FirstName;
                address.LastName = model.LastName;
                address.PhoneNumber = model.PhoneNumber;
                address.PostalCode = model.PostalCode;
                address.Address1 = model.Address1;
                address.Address2 = model.Address2;
                address.Country = model.Country;
                address.City = model.City;
                address.Province = model.Province;
                address.IsDefaultAddress = model.IsDefaultAddress;
                if (!_dbContext.MailingAddresses.Any(a => a.PostalCode == model.PostalCode))
                {
                    _dbContext.MailingAddresses.Add(address);
                }
                _dbContext.SaveChanges();
                return RedirectToAction("Addresses");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fail to create address");
            }
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int? id, UpdateAddressViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _dbContext.MailingAddresses.FindAsync(id);
            model.FirstName = address.FirstName;
            model.LastName = address.LastName;
            model.Company = address.Company;
            model.Address1 = address.Address1;
            model.Address2 = address.Address2;
            model.Country = address.Country;
            model.Province = address.Province;
            model.City = address.City;
            model.PostalCode = address.PostalCode;
            model.PhoneNumber = address.PhoneNumber;
            model.IsDefaultAddress = address.IsDefaultAddress;

            if (address == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, UpdateAddressViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var address = _dbContext.MailingAddresses.Find(id);
                    address.FirstName = model.FirstName;
                    address.LastName = model.LastName;
                    address.PhoneNumber = model.PhoneNumber;
                    address.PostalCode = model.PostalCode;
                    address.Address1 = model.Address1;
                    address.Address2 = model.Address2;
                    address.Country = model.Country;
                    address.City = model.City;
                    address.Province = model.Province;
                    address.IsDefaultAddress = model.IsDefaultAddress;
                    _dbContext.Update(address);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Addresses", "Account");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return RedirectToAction("Addresses", "Account");
        }

        //[HttpGet]
        //public async Task<IActionResult> DeleteAddress(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var address = await _dbContext.MailingAddresses
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (address == null)
        //    {
        //        return NotFound();
        //    }

        //    return RedirectToAction("Addresses", "Account");
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var to_be_deleted_address = await _dbContext.MailingAddresses.FindAsync(id);
            _dbContext.MailingAddresses.Remove(to_be_deleted_address);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Addresses", "Account");
        }


        [HttpGet]
        public async Task<IActionResult> Addresses(DisplayAddressViewModel model)
        {
            var allAddresses = _dbContext.MailingAddresses;
            model.Addresses = await allAddresses.ToListAsync();
            return View(model);
        }



    }
}
