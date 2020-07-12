using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class CollectionViewModel
    {
        public enum SortOrder
        {
            [Display(Name = "Alphabetically, A-Z")]
            NameAscending,
            [Display(Name = "Alphabetically, Z-A")]
            NameDescending,
            [Display(Name = "Price, low to high")]
            PriceAscending,
            [Display(Name = "Price, high to low")]
            PriceDescending,
            [Display(Name = "Best selling")]
            BestSelling,
            Rating,
            [Display(Name = "Release Date, old to new")]
            ReleaseDateAscending,
            [Display(Name = "Release Date, new to old")]
            ReleaseDateDescending
        }
        public string GradeName { get; set; }
        public string SeriesName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;

        public Pagination<Item> Items { get; set; }
        public List<Category> SeriesCategories { get; set; }
        public SortOrder SelectedOrder { get; set; } = SortOrder.BestSelling;

        public async Task PaginateItems(IQueryable<Item> items)
        {
            Items = await Pagination<Item>.CreateAsync(items, PageNumber, PageSize);
        }
    }
}
