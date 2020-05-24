using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        public ICollection<Item> ShoppingCartItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<MailingAddress> MailingAddresses { get; set; }
    }
}
