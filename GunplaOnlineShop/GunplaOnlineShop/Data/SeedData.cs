using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Data
{
    public class SeedData
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedData(AppDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task Run()
        {
            await SeedAdmin();
        }

        private async Task SeedAdmin()
        {
            if (_dbContext.Users.Any()) return;

            var adminAccount = new ApplicationUser
            {
                UserName = "root@store.com",
                Email = "root@store.com",
                IsAdmin = true
            };

            await _userManager.CreateAsync(adminAccount, "Password123!");
        }

    }
}
