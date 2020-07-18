using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
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
using static GunplaOnlineShop.QueryObjects.ItemSort;
using static GunplaOnlineShop.ViewModels.CollectionViewModel;

namespace GunplaOnlineShop.Controllers
{
    public class CollectionController : BaseController
    {

        public CollectionController(AppDbContext context) : base(context)
        {
        }

        // GET: /<controller>/
        [Route("/{action}")]
        public async Task<IActionResult> Search([Bind("q,PageNumber,SelectedOrder")] CollectionViewModel model)
        {
            model.PageSize = 50;
            var items = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemCategories)
                 .ThenInclude(i => i.Category)
                .AsQueryable();
            if (!string.IsNullOrEmpty(model.q))
            {
                string[] keywords = model.q.Trim().ToLower().Split(" ");
                foreach (string keyword in keywords)
                {
                    items = items.Where(i => i.Name.ToLower().Contains(keyword)
                                          || i.ItemCategories.Any(ic => ic.Category.Name.ToLower().Contains(keyword))
                                          || i.Description.ToLower().Contains(keyword));
                }
            }

            items = items.OrderItemsBy(model.SelectedOrder);

            await model.PaginateItems(items);
            return View("Category", model);
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

            items = items.OrderItemsBy(model.SelectedOrder);

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
            if (Regex.IsMatch("[^a-zA-Z0-9-]+", name)) return NotFound();
            var item = await _context.Items
                .FromSqlRaw($"SELECT * FROM Items i WHERE REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(LOWER(i.Name), '[^a-zA-Z0-9]+', '-'), '[^a-zA-Z0-9]+$', ''), '^[^a-zA-Z0-9]+', '') = '{name}'")
                .Include(i => i.ItemCategories)
                 .ThenInclude(ic => ic.Category)
                .Include(i => i.Photos)
                .Include(i => i.Reviews)
                .FirstOrDefaultAsync();
            if (item == null) return NotFound();
            return View(item);
        }

    }
}
