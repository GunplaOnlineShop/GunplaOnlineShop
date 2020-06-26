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
                    Name = "HG RX-78-2 Gundam G40 (Industrial Design Ver.)",
                    Price = 42.00m,
                    Description = "This HG RX-78-2 is released for 40th Anniversary of Gundam Animation, designed by famous designer Ken Okuyama.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("High Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "HGGTO RX-78-02 Gundam",
                    Price = 32.00m,
                    Description = "The RX-78-02 Gundam is the titular mobile suit of the Mobile Suit Gundam: The Origin manga. It was piloted by Amuro Ray during the One Year War.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("High Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "HGCE ZGMF-X42S Destiny Gundam Revive Version",
                    Price = 30.50m,
                    Description = "The ZGMF-X42S Destiny Gundam is the titular mobile suit of Mobile Suit Gundam SEED Destiny and was piloted by Shinn Asuka. A straight-built Gunpla version is piloted by Shimon Izuna in Gundam Build Fighters Try.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("High Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "HGUC RX-104FF Penelope",
                    Price = 94.00m,
                    Description = "The RX-104FF Penelope is a prototype transformable Newtype-use mobile suit. It was featured in the novel Mobile Suit Gundam: Hathaway's Flash. Without its flight unit it is referred to as the RX-104 Odysseus Gundam. The unit is piloted by Lane Aime.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("High Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "MG Freedom Gundam Ver.2.0",
                    Price = 64.50m,
                    Description = "Stolen from Z.A.F.T or gifted by Lacus Clyne? Here comes a new and updated re-design of the ZGMF-X10A Freedom Gundam.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Master Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "MG FA-010A FAZZ Ver.Ka",
                    Price = 145.00m,
                    Description = "The FA-010A FAZZ is a variant of the MSZ-010 ΖΖ Gundam. It was featured in Gundam Sentinel.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Master Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "MG RX-78-02 Gundam The Origin",
                    Price = 75.00m,
                    Description = "The RX-78-02 Gundam is the titular mobile suit of the Mobile Suit Gundam: The Origin manga. It was piloted by Amuro Ray during the One Year War.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Master Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "MG MSN-04 Sazabi Ver.Ka",
                    Price = 129.00m,
                    Description = "The MSN-04 Sazabi is a mobile suit that appears in Mobile Suit Gundam: Char's Counterattack. It is piloted by Char Aznable.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Master Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "MG MBF-P02Kai Gundam Astray Red Frame",
                    Price = 70.00m,
                    Description = "The Astray Red Frame Kai is the upgraded version of the MBF-P02 Gundam Astray Red Frame developed and piloted by Lowe Guele. It first appeared in the photo series Mobile Suit Gundam SEED VS Astray.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Master Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "PG RX-0 Unicorn Gundam",
                    Price = 276.50m,
                    Description = "The RX-0 Unicorn Gundam is the titular prototype mobile suit of the Mobile Suit Gundam Unicorn novel, its OVA adaptation and the television re-cut. Developed by Anaheim Electronics for the Earth Federation, it concealed a secret that could shake the future of all humanity,[1] for it was the key to opening Laplace's Box. It is piloted by Banagher Links after the head of the Vist Foundation and his father, Cardeas Vist, entrusted it to him at the dawn of the Third Neo Zeon War in U.C. 0096.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Perfect Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "PG GAT-X105+AQM E-YM1 Perfect Strike Gundam",
                    Price = 345.50m,
                    Description = "The GAT-X105+AQM/E-YM1 Perfect Strike Gundam is a mobile suit first featured in the eyecatches of the Mobile Suit Gundam SEED HD Remaster, and later made its debut in episode 36 of the HD Remaster. It is the heavy armed variant of the GAT-X105 Strike Gundam, and is piloted by Mu La Flaga.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Perfect Grade")).ToList(),
                    Qantity = 10,
                },
                new Item
                {
                    Name = "PG GN-001 Gundam Exia",
                    Price = 249.00m,
                    Description = "The GN-001 Gundam Exia (aka Exia, Gundam Seven Swords) is a mobile suit featured in season one of Mobile Suit Gundam 00 and is piloted by Setsuna F. Seiei.",
                    Categories = _dbContext.Categories.Where(c => c.Name.Equals("Perfect Grade")).ToList(),
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
