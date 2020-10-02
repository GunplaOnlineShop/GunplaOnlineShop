using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Emit;
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
        public DbSet<Category> Categories { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; } 
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MailingAddress> MailingAddresses { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<ShoppingCartLineItem> ShoppingCartLineItems { get; set; }

        // define relationships in OnModelCreating() using fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //If you are overriding `OnModelCreating()` in your DbContext class, 
            // need to call `base.OnModelCreating(modelBuilder)`
            base.OnModelCreating(modelBuilder);

            // composite keys can only configure here using Fluent API
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ItemId });

            modelBuilder.Entity<ItemCategory>()
                .HasKey(ic => new { ic.ItemId, ic.CategoryId });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.MailingAddresses)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId);

            modelBuilder.Entity<MailingAddress>()
                .HasOne(c => c.Customer)
                .WithMany(a => a.MailingAddresses)
                .HasForeignKey(c => c.CustomerId);
                
            // use Flutent API to configure relationships
            //modelBuilder.Entity<User>()    // One-To-Many
            //    .HasMany(u => u.Reservations)
            //    .WithOne(r => r.User);
            //modelBuilder.Entity<Reservation>()   // Many-To-One
            //.HasOne(r => r.Table)
            //.WithMany(t => t.Reservations);
            /*
            modelBuilder.Entity<ApplicationUser>(
                TypeBuilder =>
                {
                    TypeBuilder.HasMany(appUser => appUser.MailingAddresses)
                        .WithOne(mailAddress => mailAddress.Customer)
                        .HasForeignKey(mailAddress => mailAddress.CustomerId)
                        .IsRequired();
                });
            modelBuilder.Entity<MailingAddress>(
                TypeBuilder =>
                {
                    TypeBuilder.HasOne(mailAddress => mailAddress.Customer)
                        .WithMany(appUser => appUser.MailingAddresses)
                        .HasForeignKey(mailAddress => mailAddress.CustomerId)
                        .IsRequired();
                }); */

        }

    }
}
