using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class SortingViewModel
    {
        public List<SelectListItem> SortOrder { get; } = new List<SelectListItem>
        {
            new SelectListItem {Value = "name",Text = "Name,A-Z" },
            new SelectListItem {Value = "name_dec",Text = "Name,Z-A" },
            new SelectListItem {Value = "price",Text = "Price,Low to High" },
            new SelectListItem {Value = "price_dec",Text = "Price,High to Low" },
            new SelectListItem {Value = "name_dec",Text = "Name,Z-A" },
        };
    }
}
