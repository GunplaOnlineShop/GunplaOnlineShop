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
        public async Task<IActionResult> ByCategoryAsync(string grade, string series, SortOrder selectedOrder= SortOrder.BestSelling)
        {
            var items = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                 .ThenInclude(ic => ic.Category)
                .Where(i => i.ItemCategories.Any(ic => ic.Category.Name == grade));

            var itemSeries = _context.Categories
                .AsNoTracking()
                .Where(b => b.ParentCategory.Name == grade);

            if (!string.IsNullOrEmpty(series))
            {
                items = items.Where(b => b.ItemCategories.Any(c => c.Category.Name == series));
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
                    break;
                case SortOrder.ReleaseDateAscending:
                    items = items.OrderBy(i => i.ReleaseDate);
                    break;
                case SortOrder.ReleaseDateDescending:
                    items = items.OrderByDescending(i => i.ReleaseDate);
                    break;
            }

            var model = new CollectionViewModel()
            {
                SelectedOrder = selectedOrder,
                Items = await items.ToListAsync(),
                Categories = await itemSeries.ToListAsync(),
                Grade = grade,
                Series = series
            };
            return View(model);
        }

        [Route("{controller}/{grade}/action/{name}")]
        public IActionResult Products(string name)
        {
            
            return View();
        }

    }
}
