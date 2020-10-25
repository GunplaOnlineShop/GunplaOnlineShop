using GunplaOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class AddressViewModel
    {
        public List<MailingAddress> Addresses { get; set; }

    }
}
