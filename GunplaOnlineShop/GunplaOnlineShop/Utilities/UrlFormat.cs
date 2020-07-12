using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GunplaOnlineShop.Utilities
{
    public static class UrlFormat
    {
        // replace all white space with dash
        public static string NameEncode(this string s)
        {
            return Regex.Replace(s.Trim().ToLower(), "[^a-zA-Z0-9]+", "-").TrimStartEncodedDash().TrimEndEncodedDash();
        }

        private static string TrimStartEncodedDash(this string s)
        {
            return Regex.Replace(s, "^[-]+", "");
        }

        private static string TrimEndEncodedDash(this string s)
        {
            return Regex.Replace(s, "[-]+$", "");
        }
    }
}
