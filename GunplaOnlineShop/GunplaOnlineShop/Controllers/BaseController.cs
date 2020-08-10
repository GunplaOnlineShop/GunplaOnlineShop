using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GunplaOnlineShop.Controllers
{
    public class BaseController : Controller
    {
        protected AppDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected ApplicationUser _currentLoggedInUser;
        public BaseController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetCurrentLoggedInUser()
        {
            return _currentLoggedInUser ??= await _userManager.GetUserAsync(User);
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var allCategories = _context.Categories.ToList();
            String navigationLinksHtml = "";
            foreach (var rootCategory in allCategories.Where(c => c.ParentCategory == null))
            {
                if (allCategories.Any(c => c.ParentCategory == rootCategory))
                {
                    navigationLinksHtml += "<li class=\"nav-item dropdown\">" +
                                             $"<a class=\"nav-link dropdown-toggle\" href=\"{ Url.Action("Category", "Collection", new { grade = rootCategory.Name.NameEncode(), series="" }) }\" id=\"nav{rootCategory.Name.NameEncode()}\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">{rootCategory.Name.ToUpper()}</a>" +
                                             $"<div class=\"dropdown-menu dropdown-menu-right\" aria-labelledby=\"{rootCategory.Name.NameEncode()}\">";
                    foreach (var subCategory in allCategories.Where(c => c.ParentCategory == rootCategory))
                    {
                        navigationLinksHtml += $"<a class=\"dropdown-item\" href=\"{ Url.Action("Category", "Collection", new { grade=subCategory.Name.NameEncode(), series="" }) }\">{subCategory.Name.ToUpper()}</a>";
                    }
                    navigationLinksHtml += "</div></li>";
                }
                else
                {
                    navigationLinksHtml += "<li class=\"nav - item\">" +
                                             $"< a class=\"nav-link\" href=\"{ Url.Action("Category", "Collection", new { grade = rootCategory.Name.NameEncode() }) }>{rootCategory.Name.ToUpper()}</a>" +
                                           "</li>";
                }
            }
            ViewData["NavigationLinksHtml"] = navigationLinksHtml;
            base.OnActionExecuting(actionExecutingContext);
        }

    }
}