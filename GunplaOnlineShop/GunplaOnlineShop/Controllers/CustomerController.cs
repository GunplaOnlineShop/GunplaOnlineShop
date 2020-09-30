using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GunplaOnlineShop.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment) : base(context, userManager)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult OrderHistory()
        {
            return View();
        }

        public IActionResult Security()
        {
            return View();
        }

        public IActionResult Addresses()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Login","Account");
            }
            else 
            {
                ApplicationUser user = _userManager.FindByIdAsync(userid).Result;
                List<MailingAddress> mailingAddresses = new List<MailingAddress>();
                var model = new AddressViewModel();
                return View(model);
            }
        }
    }
}