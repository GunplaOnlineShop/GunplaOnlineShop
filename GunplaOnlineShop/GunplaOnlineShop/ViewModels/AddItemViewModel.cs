using GunplaOnlineShop.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class AddItemViewModel : IValidatableObject
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            var item = context.Items.Find(ItemId);
            if (Quantity > item.Quantity) yield return new ValidationResult("Invalid quantity");
        }
    }
}
