using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Data.Migrations;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.Utilities;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static GunplaOnlineShop.ViewModels.CollectionViewModel;

namespace GunplaOnlineShop.Controllers
{
    public class CollectionController : BaseController
    {

        public CollectionController(AppDbContext context) : base(context)
        {
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            // show all collection list
            return View();
        }

        [Route("{controller}/{grade}/{series?}")]
        public async Task<IActionResult> Category(string grade, string series, int? pageNumber, SortOrder selectedOrder = SortOrder.BestSelling)
        {
            var items = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                 .ThenInclude(ic => ic.Category)
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name.Trim().ToLower().Replace(" ", "-") == grade));

            var itemSeries = _context.Categories
                .AsNoTracking()
                .Where(b => b.ParentCategory.Name.Trim().ToLower().Replace(" ", "-") == grade);

            if (!string.IsNullOrEmpty(series))
            {
                items = items.Where(b => b.ItemCategories.Any(c => c.Category.Name.Trim().ToLower().Replace(" ", "-") == series));
            }

            switch (selectedOrder)
            {
                case SortOrder.NameAscending:
                    items = items.OrderBy(i => i.Name);
                    break;
                case SortOrder.NameDescending:
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case SortOrder.PriceAscending:
                    items = items.OrderBy(i => i.Price);
                    break;
                case SortOrder.PriceDescending:
                    items = items.OrderByDescending(i => i.Price);
                    break;
                case SortOrder.Rating:
                    items = items.OrderByDescending(i => i.AverageRating);
                    break;
                case SortOrder.BestSelling:
                    items = items.OrderByDescending(i => i.TotalSales);
                    break;
                case SortOrder.ReleaseDateAscending:
                    items = items.OrderBy(i => i.ReleaseDate);
                    break;
                case SortOrder.ReleaseDateDescending:
                    items = items.OrderByDescending(i => i.ReleaseDate);
                    break;
            }

            int pageSize = 2;

            var model = new CollectionViewModel()
            {
                SelectedOrder = selectedOrder,
                Items = await Pagination<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize),
                Categories = await itemSeries.ToListAsync(),
            };

            return View(model);
        }

        [Route("{controller}/{grade}/{action}/{name}")]
        public async Task<IActionResult> Products(string grade, string name)
        {
            var items = await _context.Items
                .AsNoTracking()
                .Select(i => new { Id = i.Id, Name = i.Name  })
                .ToListAsync();
            var itemInfo = items.Where(i => i.Name.Encode("[^a-zA-Z0-9]+", "-") == name).FirstOrDefault();
            var item = await _context.Items
                .Include(i => i.ItemCategories)
                 .ThenInclude(ic => ic.Category)
                .Include(i => i.Photos)
                .Include(i => i.Reviews)
                .Where(i => i.Id == itemInfo.Id)
                .FirstOrDefaultAsync();
            return View(item);
        }

    }
}
