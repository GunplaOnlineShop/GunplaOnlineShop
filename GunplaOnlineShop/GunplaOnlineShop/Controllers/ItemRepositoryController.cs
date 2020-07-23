using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GunplaOnlineShop.QueryObjects.ItemSort;


namespace GunplaOnlineShop.Controllers
{
    public class ItemRepositoryController:BaseController
    {
        public ItemRepositoryController(AppDbContext context): base(context) { }

        public async Task<IActionResult> ItemRepositoryCollection([Bind("PageNumber,SelectedOrder")] CollectionViewModel model)
        {
            model.PageSize = 20;

            var allItems = _context.Items
                .AsNoTracking();
           
            var items = allItems.OrderItemsBy(model.SelectedOrder);
            await model.PaginateItems(items);

            return View(model);            
        }

        [HttpGet]
        public IActionResult ItemCreate(ItemViewModel model)
        {
            var categories = _context.Categories
                .AsTracking()
                .ToList();

            List<CategoryCheck> categoryList = new List<CategoryCheck>();

            foreach (var cate in categories)
            {
                categoryList.Add(new CategoryCheck() { category = cate});
            }

            model.CategoryList = categoryList;

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Items.Any(c => c.Name == model.Name)) return View("ItemCreate",model);
                Item newItem = new Item();
                newItem.Name = model.Name;
                newItem.Price = model.Price;
                newItem.Description = model.Description;
                newItem.Qantity = model.Qantity;
                newItem.ReleaseDate = model.ReleaseDate;
                newItem.IsAvailable = model.IsAvailable;
                List<ItemCategory> cateList = new List<ItemCategory>();
                foreach (var Cate in model.CategoryList)
                {
                    ItemCategory itemCate = new ItemCategory
                    {
                        CategoryId = Cate.category.Id,
                        ItemId = newItem.Id
                    };
                    cateList.Add(itemCate);
            
                }
                newItem.ItemCategories = cateList;

                _context.Items.Add(newItem);
                _context.SaveChanges();
                return RedirectToAction("Admin", "Index");

            }
            return View("ItemCreate",model);
        }

    }
}
