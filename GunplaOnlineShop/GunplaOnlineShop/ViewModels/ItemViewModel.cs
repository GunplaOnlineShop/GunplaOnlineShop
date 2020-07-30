using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class CategoryCheck 
    {
        public string cateName { get; set; }
        public int cateId { get; set; }
        public bool IsCheck { get; set; } = false;
    }
    public class ItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [MaxLength(510)]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Qantity { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
        
        [Required]
        public List<CategoryCheck> CategoryList { get; set; }

        [Required]
        public IFormFile CoverPhoto { get; set; }

    }
}
