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

        public async Task<IActionResult> Index(HomeViewModel homeViewModel)
        {
            var hgItems = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                .ThenInclude(ic => ic.Category)
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == "High Grade"))
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var mgItems = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                .ThenInclude(ic => ic.Category)
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == "Master Grade"))
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var pgItems = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                .ThenInclude(ic => ic.Category)
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == "Perfect Grade"))
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var bestSellingItems = _context.Items
                .AsNoTracking()
                .OrderByDescending(i => i.TotalSales)
                .Take(4);
            var newItems = _context.Items
                .AsNoTracking()
                .OrderByDescending(i => i.ReleaseDate)
                .Take(4);

            var model = new HomeViewModel()
            {
                HgItems = await hgItems.ToListAsync(),
                MgItems = await mgItems.ToListAsync(),
                PgItems = await pgItems.ToListAsync(),
                BestSellingItems = await bestSellingItems.ToListAsync(),
                NewItems =await newItems.ToListAsync(),
            };
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
