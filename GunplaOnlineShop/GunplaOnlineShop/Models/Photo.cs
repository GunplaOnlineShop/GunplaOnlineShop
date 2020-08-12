using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required]
        [Column("Name",TypeName ="LONGTEXT")]
        public string Name { get; set; }
        [Url]
        [Column("URL", TypeName = "LONGTEXT")]
        public string Url { get; set; }

        [Url]
        [Column("FilePath", TypeName = "LONGTEXT")]
        public string FilePath { get; set; }

    }
}
