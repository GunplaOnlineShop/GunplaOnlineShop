using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [StringLength(25)]
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }  // nullable foreign key --> not all category has parent category
        [ForeignKey("ParentCategoryId")]
        public Category ParentCategory { get; set; }
    }
}
