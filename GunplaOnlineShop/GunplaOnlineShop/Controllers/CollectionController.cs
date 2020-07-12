using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Dasync.Collections;
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
        public async Task<IActionResult> Category(string grade, string series, [Bind("PageNumber,SelectedOrder")] CollectionViewModel model)
        {
            var allCategories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();
            var gradeCategory = allCategories.Where(c => c.Name.NameEncode() == grade).FirstOrDefault();
            if (gradeCategory == null) return NotFound();
            model.GradeName = gradeCategory.Name;
            var items = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                 .ThenInclude(ic => ic.Category)
                .Where(i => i.ItemCategories.Any(ic => ic.CategoryId == gradeCategory.Id));

            if (!string.IsNullOrEmpty(series))
            {
                var seriesCategory = allCategories.Where(c => c.Name.NameEncode() == series).FirstOrDefault();
                int seriesCategoryId = -1;
                if (seriesCategory != null)
                {
                    model.SeriesName = seriesCategory.Name;
                    seriesCategoryId = seriesCategory.Id;
                }
                items = items.Where(b => b.ItemCategories.Any(ic => ic.CategoryId == seriesCategoryId));
            }

            switch (model.SelectedOrder)
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

            await model.PaginateItems(items);
            model.SeriesCategories = await _context.Categories
                .AsNoTracking()
                .Where(b => b.ParentCategory.Id == gradeCategory.Id)
                .ToListAsync();

            return View(model);
        }

        [Route("{controller}/{grade}/{action}/{name}")]
        public async Task<IActionResult> Products(string grade, string name)
        {
            var items = await _context.Items
                .AsNoTracking()
                .Select(i => new { Id = i.Id, Name = i.Name  })
                .ToListAsync();
            var itemInfo = items.Where(i => i.Name.NameEncode() == name).FirstOrDefault();
            if (itemInfo == null) return NotFound();
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
