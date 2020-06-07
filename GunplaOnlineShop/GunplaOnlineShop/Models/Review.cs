using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class Review
    {
        public int Id { get; set; }
        [MaxLength(500)]
        [StringLength(500)]
        public string Comment { get; set; }
        [Required]
        [Range(0,5)]
        public int Rating { get; set; }

        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}
