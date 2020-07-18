using GunplaOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.QueryObjects
{
    public static class ItemSort
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

        public static IQueryable<Item> OrderItemsBy(this IQueryable<Item> items, SortOrder orderByOptions)
        {
            switch (orderByOptions)
            {
                case SortOrder.NameAscending:
                    return items.OrderBy(i => i.Name);
                case SortOrder.NameDescending:
                    return items.OrderByDescending(i => i.Name);
                case SortOrder.PriceAscending:
                    return items.OrderBy(i => i.Price);
                case SortOrder.PriceDescending:
                    return items.OrderByDescending(i => i.Price);
                case SortOrder.Rating:
                    return items.OrderByDescending(i => i.AverageRating);
                case SortOrder.BestSelling:
                    return items.OrderByDescending(i => i.TotalSales);
                case SortOrder.ReleaseDateAscending:
                    return items.OrderBy(i => i.ReleaseDate);
                case SortOrder.ReleaseDateDescending:
                    return items.OrderByDescending(i => i.ReleaseDate);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orderByOptions), orderByOptions, null);
            }
        }
    }
}
