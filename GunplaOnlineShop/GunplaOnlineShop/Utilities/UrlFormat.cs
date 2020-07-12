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
        public static string Encode(this string s, string pattern, string replace)
        {
            return Regex.Replace(s.Trim().ToLower(), pattern, replace);
        }
    }
}
