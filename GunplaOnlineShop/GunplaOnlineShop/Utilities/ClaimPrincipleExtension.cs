using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Utilities
{
    public static class ClaimPrincipleExtension
    {
        public static string IsAdminCheck(this ClaimsPrincipal principal)
        {
            var isAdmin = principal.Claims.FirstOrDefault(c => c.Type == "IsAdmin");
            return isAdmin?.Value;
        }
    }
}
