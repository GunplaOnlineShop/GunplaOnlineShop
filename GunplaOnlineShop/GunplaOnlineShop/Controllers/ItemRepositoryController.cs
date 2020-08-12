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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using GunplaOnlineShop.Utilities;
using Microsoft.AspNetCore.Identity;

namespace GunplaOnlineShop.Controllers
{

    [Authorize(Policy = "AdminOnly")]
    public class ItemRepositoryController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ItemRepositoryController(AppDbContext context, UserManager<ApplicationUser> userManager,IWebHostEnvironment webHostEnvironment) : base(context, userManager) 
        {
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index([Bind("PageNumber,SelectedOrder")] CollectionViewModel model)
        {
            String coverPath = Path.Combine(_webHostEnvironment.WebRootPath, "images\\cover");
            String galleryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images\\gallery");
            if (!Directory.Exists(coverPath)) Directory.CreateDirectory(coverPath);
            if (!Directory.Exists(galleryPath)) Directory.CreateDirectory(galleryPath);

            model.PageSize = 20;
            var allItems = _context.Items
                .Include(i=>i.Photos)
                .AsNoTracking();

            var items = allItems.OrderItemsBy(model.SelectedOrder);
            await model.PaginateItems(items);

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ItemViewModel();
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Item newItem = new Item();
                if (_context.Items.Any(c => c.Name == model.Name))
                {
                    ViewBag.nameExist = "Item Name Exist";
                    return View(model);
                }



                if (model.CoverPhoto != null || model.GalleryPhotos != null)
                {
                    List<Photo> photos = new List<Photo>();
                    if (model.CoverPhoto != null)
                    {
                        String uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cover");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverPhoto.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            model.CoverPhoto.CopyTo(fs);
                        }
                        photos.Add(new Photo() { Name = uniqueFileName, Url = "~/images/cover/" + uniqueFileName, FilePath = filePath });
                    }
                    if (model.GalleryPhotos != null && model.GalleryPhotos.Count > 0)
                    {
                        foreach (IFormFile photo in model.GalleryPhotos)
                        {
                            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath,"images/gallery");
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                photo.CopyTo(fs);
                            }
                            photos.Add(new Photo() { Name = uniqueFileName, Url = "~/images/gallery/" + uniqueFileName, FilePath = filePath });
                        }
                    }
                    newItem.Photos = photos;

                }            
                newItem.Name = model.Name;
                newItem.Price = model.Price;
                newItem.Description = model.Description;
                newItem.Qantity = model.Qantity;
                newItem.ReleaseDate = model.ReleaseDate;
                newItem.IsAvailable = model.IsAvailable;
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
                return RedirectToAction("Index", "ItemRepository");

            }
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "ItemRepository");
            }
            var item = _context.Items
                .Include(i => i.ItemCategories)
                .Include(i => i.Photos)
                .Include(i => i.Reviews)
                .Where(i => i.Id == id)
                .FirstOrDefault();
            if (item == null)
            {
                return RedirectToAction("Index", "ItemRepository");
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

            return RedirectToAction("Index", "ItemRepository");
        }

        [HttpGet]
        public IActionResult Edit(int id)
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
        [ValidateAntiForgeryToken]
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
                updateItem.State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index", "ItemRepository");

            }
            return View(model);
        }

        public IActionResult PhotoEdit(int id)
        {
            var item = _context.Items
                .Include(i => i.Photos)
                .Where(i => i.Id == id).FirstOrDefault();
            var model = new PhotoEditViewModel();
            model.ItemId = id;
            List<Photo> gallery = new List<Photo>();
            if (item.Photos.Any()) 
            {
                foreach (var photo in item.Photos)
                {
                    if (photo.Url.Contains("cover"))
                    {
                        model.Cover = photo;
                    }
                    else
                    {
                        gallery.Add(photo);
                    }
                }
                model.Gallery = gallery;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PhotoDelete(PhotoEditViewModel model, int id)
        {
            var photo = _context.Photos
                .Where(i => i.Id == id)
                .FirstOrDefault();
            if (System.IO.File.Exists(photo.FilePath))
            {
                System.IO.File.Delete(photo.FilePath);
            }
            _context.Remove(photo);
            _context.SaveChanges();

            return RedirectToAction("PhotoEdit", new {id = model.ItemId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PhotoAdd(PhotoEditViewModel model, int id)
        {
            var item = _context.Items
                .Include(i => i.Photos)
                .Where(i => i.Id == id).FirstOrDefault();
            List<Photo> photos = new List<Photo>();
            if (item.Photos.Any()) { photos = item.Photos.ToList(); }
            if (model.CoverUpdate != null)
            {
                String uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cover");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverUpdate.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    model.CoverUpdate.CopyTo(fs);
                }
                photos.Add(new Photo() { Name = uniqueFileName, Url = "~/images/cover/" + uniqueFileName, FilePath = filePath });
            }
            if (model.GalleryUpdate != null && model.GalleryUpdate.Count > 0)
            {
                foreach (IFormFile photo in model.GalleryUpdate)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/gallery");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fs);
                    }
                    photos.Add(new Photo() { Name = uniqueFileName, Url = "~/images/gallery/" + uniqueFileName, FilePath = filePath });
                }
            }
            item.Photos = photos;
            var updateItem = _context.Items.Attach(item);
            updateItem.State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("PhotoEdit", new { id = id });
        }

    }
}
