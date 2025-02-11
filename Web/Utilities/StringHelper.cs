using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Web.Utilities
{
    public static class StringHelper
    {
        public static string TruncateLongString(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;

            if (str == null || str.Length < maxLength || str.IndexOf(" ", maxLength) == -1)
                return str;

            return str.Substring(0, str.IndexOf(" ", maxLength)) + "..";
        }
        public static string TruncateEmail(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;

            if (str == null || str.Length < maxLength)
                return str;

            return str.Substring(0, maxLength) + "..";
        }
       
       




    }
}