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
using static GunplaOnlineShop.ViewModels.CollectionViewModel;

namespace GunplaOnlineShop.Controllers
{
    public class CollectionController : BaseController
    {

        public CollectionController(AppDbContext context) : base(context)
        {
        }

        // GET: /<controller>/
        public async Task<IActionResult> SearchAsync(string keyword, SortOrder selectedOrder,int?pageNumber)
        {
            int pageSize = 12;

            var items= new List<Item>().AsQueryable();

            if (string.IsNullOrEmpty(keyword))
            {


                items = _context.Items
                .AsNoTracking();

            }
            else
            {
                if (!keyword.Contains(" ")) {
                    var nameMatchItems = _context.Items
                        .AsNoTracking()
                        .Where(i => (i.Name.ToLower().Contains(keyword.ToLower())));
                    var graedMatchItems = _context.Items
                        .AsNoTracking()
                        .Include(i => i.ItemCategories)
                        .ThenInclude(ic => ic.Category)
                        .Where(i => i.ItemCategories.Any(ic => ic.Category.Name.Contains(keyword)));
                    var descrMatchItems = _context.Items
                        .AsNoTracking()
                        .Where(i => (i.Description.ToLower().Contains(keyword.ToLower())));

                    var list1 = nameMatchItems.Concat(graedMatchItems);
                    items = list1.Concat(descrMatchItems);
                    items = items.Distinct();
                }
                else
                {
                    string[] keyList = keyword.Split(" ");
                    foreach (var key in keyList)
                    {
                        var nameMatchItems = _context.Items
                            .AsNoTracking()
                            .Where(i => (i.Name.ToLower().Contains(key.ToLower())));
                        var graedMatchItems = _context.Items
                            .AsNoTracking()
                            .Include(i => i.ItemCategories)
                            .ThenInclude(ic => ic.Category)
                            .Where(i => i.ItemCategories.Any(ic => ic.Category.Name.Contains(key)));
                        var descrMatchItems = _context.Items
                            .AsNoTracking()
                            .Where(i => (i.Description.ToLower().Contains(key.ToLower())));
                        var list1 = nameMatchItems.Concat(graedMatchItems);
                        items = list1.Concat(descrMatchItems);

                    }
                    items = items.Distinct();
                }
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
                Items = await Pagination<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize),
            };
            return View(model);
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
