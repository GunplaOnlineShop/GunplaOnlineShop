﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class Item
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
        public string Photo { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Qantity { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}