using GunplaOnlineShop.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Controllers
{
    public class AdminController:BaseController
    {
        public AdminController(AppDbContext context) : base(context)
        { }

        public IActionResult Index()
        {
            return View();
        }
    }
}
