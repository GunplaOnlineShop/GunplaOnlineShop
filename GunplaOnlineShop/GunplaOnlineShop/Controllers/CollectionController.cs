using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Data.Migrations;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GunplaOnlineShop.Controllers
{
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;

        public CollectionController(AppDbContext context)
        {
            _context = context;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sorting(CollectionViewModel collectionViewModel)
        {
   
            switch (collectionViewModel.SelectedOrder)
            {
                case "name":
                    var a = collectionViewModel.Items.OrderBy(s => s.Name).ToList();
                    collectionViewModel.Items = a;
                    break;
                case "name_dec":
                    var b = collectionViewModel.Items.OrderByDescending(s => s.Name).ToList();
                    collectionViewModel.Items = b;
                    break;
                case "price":
                    var c = collectionViewModel.Items.OrderBy(s => s.Price).ToList();
                    collectionViewModel.Items = c;
                    break;
                case "price_dec":
                    var d = collectionViewModel.Items.OrderByDescending(s => s.Price).ToList();
                    collectionViewModel.Items = d;
                    break;

            }
            return View(collectionViewModel);
        }
        


        [HttpGet]
        [Route("{controller}/{action}/{grade}")]
        public IActionResult ByCategory(string grade)
        {
            ViewBag.Grade = grade;
            var items = _context.Items
                .AsNoTracking()
                .Include(b => b.ItemCategories)
                .ThenInclude(c => c.Category)
                .Where(b => b.ItemCategories.Any(c => c.Category.Name == grade))
                .OrderBy(x => x.Name)
                .ToList();

            var model = new CollectionViewModel();
            model.Items = items;
            return View(model);
        }

    }
}
