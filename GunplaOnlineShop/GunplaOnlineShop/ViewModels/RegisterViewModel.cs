using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Your Password")]
        [Compare("Password",
            ErrorMessage = "Please enter the same password.")]
        public string ConfirmPassword { get; set; }

    }
}
