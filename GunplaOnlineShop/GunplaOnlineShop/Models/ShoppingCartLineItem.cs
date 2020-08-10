using GunplaOnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class ShoppingCartLineItem : TimestampedModel, IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }
        [Required]
        public int Quantity { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            var item = context.Items.Find(ItemId);
            if (item == null) yield return new ValidationResult("Invalid input.");
            if (Quantity > item.Qantity) yield return new ValidationResult("Invalid quantity");
        }
    }
}
