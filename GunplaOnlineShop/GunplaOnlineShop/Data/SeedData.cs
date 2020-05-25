using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public SeedData(AppDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _dbContext = applicationDbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task Run()
        {
            await SeedAdmin();
            SeedCategories();
            SeedItems();
        }

        private void SeedCategories()
        {
            if (_dbContext.Categories.Any()) return;

            Category[] categories =
            {
                new Category
                {
                    Id = 1,
                    Name = "Gundam Model Kits"
                },
                new Category
                {
                    Name = "High Grade",
                    ParentCategoryId = 1
                },
                new Category
                {
                    Name = "Master Grade",
                    ParentCategoryId = 1
                },
                new Category
                {
                    Name = "Perfect Grade",
                    ParentCategoryId = 1
                },
                new Category
                {
                    Name = "HIRM Hi-Resolution",
                    ParentCategoryId = 1
                },
                new Category
                {
                    Name = "SD Super Deformed",
                    ParentCategoryId = 1
                },
            };

            foreach (Category category in categories)
            {
                _dbContext.Categories.Add(category);
            }

            _dbContext.SaveChanges();
        }

        private void SeedItems()
        {
            if (_dbContext.Items.Any()) return;

            Item[] items =
            {
                new Item
                {
                    Name = "MG 1/100 Freedom Gundam Ver.2.0",
                    Price = 64.50m,
                    Description = "Stolen from Z.A.F.T or gifted by Lacus Clyne? Here comes a new and updated re-design of the ZGMF-X10A Freedom Gundam.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Master Grade")).ToList(),
                    Qantity = 10,
                },
            };

            foreach(Item item in items)
            {
                _dbContext.Items.Add(item);
            }

            _dbContext.SaveChanges();
        }

        private async Task SeedAdmin()
        {
            if (_dbContext.Users.Any()) return;

            var adminAccount = new ApplicationUser
            {
                UserName = _configuration["AdminAccount:Email"],
                Email = _configuration["AdminAccount:Email"],
                IsAdmin = true
            };

            await _userManager.CreateAsync(adminAccount, _configuration["AdminAccount:Password"]);
        }



    }
}
