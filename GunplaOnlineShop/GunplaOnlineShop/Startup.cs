using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GunplaOnlineShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure and add mysql
            var dbHost = Configuration.GetValue<string>("Database:Hostname");
            var dbSchema = Configuration.GetValue<string>("Database:Schema");
            var dbUser = Configuration.GetValue<string>("Database:Username");
            var dbPassword = Configuration.GetValue<string>("Database:Password");
            var port = Configuration.GetValue<string>("Database:Port");
            // AppDbContext is the DbContext you need to define and place it in Model folder
            services.AddDbContext<AppDbContext>(options => options.UseMySql($"server={dbHost};port={port};database={dbSchema};user={dbUser};password={dbPassword}"));
            // Register identity service 
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders(); ;
            //config password complexity
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 10;
            });
            //Register Admin Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin"));
            });
            services.AddRouting(options => options.LowercaseUrls = true); // enforce lowercase routing
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();
            // register the authorization in the request pipeline
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
