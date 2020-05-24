using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // This constructor is how the ASP.NET creates an instance of ApplicationDbContext
            // options object containing details such as the connection string 
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; } 
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MailingAddress> MailingAddresses { get; set; }


        // define relationships in OnModelCreating() using fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //If you are overriding `OnModelCreating()` in your DbContext class, 
            // need to call `base.OnModelCreating(modelBuilder)`
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ItemId });
            // use Flutent API to configure relationships
            //modelBuilder.Entity<User>()    // One-To-Many
            //    .HasMany(u => u.Reservations)
            //    .WithOne(r => r.User);
            //modelBuilder.Entity<Reservation>()   // Many-To-One
            //.HasOne(r => r.Table)
            //.WithMany(t => t.Reservations);
        }

    }
}
