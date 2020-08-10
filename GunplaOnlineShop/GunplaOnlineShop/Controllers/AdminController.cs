using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Controllers
{
    public class AdminController:BaseController
    {
        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        { }

        public IActionResult Index()
        {
            return View();
        }
    }
}
