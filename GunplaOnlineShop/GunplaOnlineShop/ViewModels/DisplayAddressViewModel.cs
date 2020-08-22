using GunplaOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class DisplayAddressViewModel
    {
        //    [Required]
        //    [MaxLength(35)]
        //    [StringLength(25)]
        //    public string FirstName { get; set; }
        //    [Required]
        //    [MaxLength(35)]
        //    [StringLength(25)]
        //    public string LastName { get; set; }
        //    [MaxLength(45)]
        //    [StringLength(35)]
        //    public string Company { get; set; }
        //    [Required]
        //    [MaxLength(70)]
        //    [StringLength(60)]
        //    public string Address1 { get; set; }
        //    [MaxLength(70)]
        //    [StringLength(60)]
        //    public string Address2 { get; set; }
        //    [Required]
        //    [MaxLength(50)]
        //    [StringLength(40)]
        //    public string City { get; set; }
        //    [Required]
        //    [MaxLength(50)]
        //    [StringLength(40)]
        //    public string Province { get; set; }
        //    [Required]
        //    [MaxLength(50)]
        //    [StringLength(40)]
        //    public string Country { get; set; }
        //    [Required]
        //    [MaxLength(10)]
        //    [StringLength(6, ErrorMessage = "Postal code has to be 6 digits long.", MinimumLength = 6)]
        //    public string PostalCode { get; set; }
        //    [Required]
        //    [Phone]
        //    public string PhoneNumber { get; set; }
        //    [Display(Name = "Is Default Address")]
        //    public bool IsDefaultAddress { get; set; }

        public List<MailingAddress> Addresses { get; set; }
    }

}
