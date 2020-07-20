using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.Data;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using GunplaOnlineShop.ViewModels;

namespace GunplaOnlineShop.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(AppDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index(HomeViewModel model)
        {

            var allItems = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                .ThenInclude(ic => ic.Category);
            
            var hgItems = allItems
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == "High Grade"))
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var mgItems = allItems
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == "Master Grade"))
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var pgItems = allItems
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == "Perfect Grade"))
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var bestSellingItems = allItems
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var newItems = allItems
                .OrderByDescending(i => i.ReleaseDate)
                .Take(4);



            model.RootCategoryIds = await _context.Categories.AsNoTracking().Where(c => c.ParentCategoryId == null).Select(c => c.Id).ToArrayAsync();
            model.HgItems = await hgItems.ToListAsync();
            model.MgItems = await mgItems.ToListAsync();
            model.PgItems = await pgItems.ToListAsync();
            model.BestSellingItems = await bestSellingItems.ToListAsync();
            model.NewItems = await newItems.ToListAsync();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
