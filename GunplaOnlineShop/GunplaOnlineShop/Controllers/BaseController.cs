using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GunplaOnlineShop.Controllers
{
    public class BaseController : Controller
    {
        protected readonly AppDbContext _context;
        public BaseController(AppDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var allCategories = _context.Categories.ToList();
            String navigationLinksHtml = "";
            foreach (var rootCategory in allCategories.Where(c => c.ParentCategory == null))
            {
                if (allCategories.Any(c => c.ParentCategory == rootCategory))
                {
                    navigationLinksHtml += $"<li class=\"nav-item dropdown\"><a class=\"nav-link dropdown-toggle\" href=\"{ Url.Action("ByCategory", "Collection", new { grade = rootCategory.Name }) } id=\"nav{rootCategory.Name.Replace(" ", "")}\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">{rootCategory.Name.ToUpper()}</a><div class=\"dropdown-menu dropdown-menu-right\" aria-labelledby=\"{rootCategory.Name.Replace(" ", "")}\">";
                    foreach (var subCategory in allCategories.Where(c => c.ParentCategory == rootCategory))
                    {
                        navigationLinksHtml += $"<a class=\"dropdown-item\" href=\"{ Url.Action("ByCategory", "Collection", new { grade=subCategory.Name }) }\">{subCategory.Name.ToUpper()}</a>";
                    }
                    navigationLinksHtml += "</div></li>";
                }
                else
                {
                    navigationLinksHtml += $"<li class=\"nav - item\">< a class=\"nav-link\" href=\"{ Url.Action("ByCategory", "Collection", new { grade = rootCategory.Name }) }>{rootCategory.Name.ToUpper()}</a></li>";
                }
            }
            ViewData["NavigationLinksHtml"] = navigationLinksHtml;
            base.OnActionExecuting(actionExecutingContext);
        }

    }
}