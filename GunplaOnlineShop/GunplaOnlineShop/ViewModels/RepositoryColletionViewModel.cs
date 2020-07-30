using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using static GunplaOnlineShop.QueryObjects.ItemSort;

namespace GunplaOnlineShop.ViewModels
{
    public class RepositoryCollectionViewModel
    {
        public string GradeName { get; set; }
        public string SeriesName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public string q { get; set; }

        public Pagination<Item> Items { get; set; }
        public List<Category> SeriesCategories { get; set; } = new List<Category>();
        public SortOrder SelectedOrder { get; set; } = SortOrder.BestSelling;

        public async Task PaginateItems(IQueryable<Item> items)
        {
            Items = await Pagination<Item>.CreateAsync(items, PageNumber, PageSize);
        }
    }
}
