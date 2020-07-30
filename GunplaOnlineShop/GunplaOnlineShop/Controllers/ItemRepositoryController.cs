using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static GunplaOnlineShop.QueryObjects.ItemSort;
using System.IO;

namespace GunplaOnlineShop.Controllers
{
    public class ItemRepositoryController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ItemRepositoryController(AppDbContext context,IWebHostEnvironment webHostEnvironment) : base(context) 
        {
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> ItemRepositoryCollection([Bind("PageNumber,SelectedOrder")] CollectionViewModel model)
        {
            model.PageSize = 20;

            var allItems = _context.Items
                .Include(i=>i.Photos)
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
                categoryList.Add(new CategoryCheck() { cateName = cate.Name, cateId = cate.Id });
            }

            model.CategoryList = categoryList;

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Items.Any(c => c.Name == model.Name)) return View("ItemCreate", model);

                Photo coverPhoto = new Photo();
                if (model.CoverPhoto != null)
                {
                    String uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cover");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverPhoto.FileName;
                    var filePath = Path.Combine(uploadsFolder,uniqueFileName);
                    model.CoverPhoto.CopyTo(new FileStream(filePath, FileMode.Create));
                    coverPhoto.FilePath = filePath;
                    coverPhoto.Name = uniqueFileName;
                    coverPhoto.Url = "~/images/cover/"+uniqueFileName;
                }
                Item newItem = new Item();
                newItem.Name = model.Name;
                newItem.Price = model.Price;
                newItem.Description = model.Description;
                newItem.Qantity = model.Qantity;
                newItem.ReleaseDate = model.ReleaseDate;
                newItem.IsAvailable = model.IsAvailable;
                if (coverPhoto != null) 
                {
                    List<Photo> photos = new List<Photo>
                    {
                        coverPhoto,
                    };
                    newItem.Photos = photos;
                }

                List<ItemCategory> cateList = new List<ItemCategory>();
                foreach (var Cate in model.CategoryList)
                {
                    if (Cate.IsCheck == true)
                    {
                        ItemCategory itemCate = new ItemCategory
                        {
                            CategoryId = Cate.cateId,
                            ItemId = newItem.Id
                        };
                        cateList.Add(itemCate);
                    }
                }
                newItem.ItemCategories = cateList;

                _context.Items.Add(newItem);
                _context.SaveChanges();
                return RedirectToAction("ItemRepositoryCollection", "ItemRepository");

            }
            return View("ItemCreate", model);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ItemRepositoryCollection", "ItemRepository");
            }
            var item = _context.Items
                .Include(i => i.ItemCategories)
                .Include(i => i.Photos)
                .Include(i => i.Reviews)
                .Where(i => i.Id == id)
                .FirstOrDefault();
            if (item == null)
            {
                return RedirectToAction("ItemRepositoryCollection", "ItemRepository");
            }
            var photoTemp = item.Photos;
            _context.Items.Remove(item);
            _context.SaveChanges();
            if (photoTemp.Any())
            {
                foreach (var photo in photoTemp)
                {
                    if (System.IO.File.Exists(photo.FilePath))
                    {
                        System.IO.File.Delete(photo.FilePath);
                    }
                    _context.Remove(photo);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("ItemRepositoryCollection", "ItemRepository");
        }

        [HttpGet]
        public IActionResult ItemEdit(int id)
        {
            var item = _context.Items.Find(id);

            var itemCategories = _context.ItemCategories
                .Where(i => i.ItemId == id)
                .ToList();

            var categories = _context.Categories
                .AsNoTracking()
                .ToList();

            List<CategoryCheck> categoryList = new List<CategoryCheck>();

            foreach (var cate in categories)
            { 
                {categoryList.Add(new CategoryCheck() { cateName = cate.Name, cateId = cate.Id }); }    
            }

            foreach (var itemCate in itemCategories)
            {
                for (var i = 0; i < categoryList.Count; i++)
                {
                    if (categoryList[i].cateId == itemCate.CategoryId)
                    {
                        categoryList[i].IsCheck = true;
                        break;
                    }
                }
            }

            ItemViewModel model = new ItemViewModel
            {
                Id=id,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description,
                Qantity = item.Qantity,
                ReleaseDate = item.ReleaseDate,
                IsAvailable = item.IsAvailable,
                CategoryList = categoryList
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = _context.Items
                    .Include(x => x.ItemCategories)
                    .FirstOrDefault(x => x.Id ==model.Id);
                item.Name = model.Name;
                item.Price = model.Price;
                item.Description = model.Description;
                item.Description = model.Description;
                item.Qantity = model.Qantity;
                item.ReleaseDate = model.ReleaseDate;
                item.IsAvailable = model.IsAvailable;
                List<ItemCategory> cateList = new List<ItemCategory>();
                foreach (var Cate in model.CategoryList)
                {
                    if (Cate.IsCheck == true)
                    {
                        ItemCategory itemCate = new ItemCategory
                        {
                            CategoryId = Cate.cateId,
                            ItemId = item.Id
                        };
                        cateList.Add(itemCate);
                    }
                }
                var oldItemCategories = _context.ItemCategories
                    .Where(x => x.ItemId == model.Id);
                foreach (var oldItemCate in oldItemCategories)
                {
                    _context.ItemCategories.Remove(oldItemCate);
                }
                _context.SaveChanges();
                item.ItemCategories = cateList;
                var updateItem = _context.Items.Attach(item);
                updateItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("ItemRepositoryCollection", "ItemRepository");

            }

            return View("ItemEdit",model);
        }
    }
}
