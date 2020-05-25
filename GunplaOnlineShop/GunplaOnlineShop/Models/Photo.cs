using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [StringLength(25)]
        public string Name { get; set; }
        [Url]
        [MaxLength(100)]
        public string Url { get; set; }
    }
}
