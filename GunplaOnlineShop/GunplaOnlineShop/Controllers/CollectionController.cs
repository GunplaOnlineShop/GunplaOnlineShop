using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        [Route("{controller}/{action}/{grade}/{series?}")]
        public async Task<IActionResult> CategoryAsync(string grade, string series, SortOrder selectedOrder, int? pageNumber)
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

            int pageSize = 12;

            var model = new CollectionViewModel()
            {
                SelectedOrder = selectedOrder,
                Items = await Pagination<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize),
                Categories = await itemSeries.ToListAsync(),
                Grade = grade,
                Series = series
            };

            return View(model);
        }

    }
}
