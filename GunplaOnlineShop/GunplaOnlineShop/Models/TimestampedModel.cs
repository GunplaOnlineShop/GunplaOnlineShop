using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class TimestampedModel
    {
        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Display(Name = "Last Modified Date")]
        public DateTime LastModifiedDate { get; set; }

        protected TimestampedModel()
        {
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
        }
    }
}
