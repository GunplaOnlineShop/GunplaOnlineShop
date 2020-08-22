using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class SecurityViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }
    }
}
