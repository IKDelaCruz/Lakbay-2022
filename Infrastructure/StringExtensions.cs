using System;
using System.Text;

namespace Infrastructure
{
    public static class StringExtensions
    {
        public static string ToReadableText(this string s)
        {
            if (string.IsNullOrEmpty(s) || 2 > s.Length)
            {
                return s;
            }

            var sb = new StringBuilder();
            var ca = s.ToCharArray();
            sb.Append(ca[0]);
            for (int i = 1; i < ca.Length - 1; i++)
            {
                char c = ca[i];
                if (char.IsUpper(c) && (char.IsLower(ca[i + 1]) || char.IsLower(ca[i - 1])))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            sb.Append(ca[ca.Length - 1]);
            return sb.ToString();
        }
        public static string FixCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }
        public static string FixMobile(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var convertedMobile = "";
            str = str.Replace("/", ",");

            if (str.IndexOf(',') > 0)
            {
                var numbers = str.Split(',');
                foreach(string s in numbers)
                {
                    convertedMobile += (Beautify(s) + ",");
                }
            }
            else
            {
                convertedMobile += Beautify(str);
            }
            return convertedMobile;


        }
        private static string Beautify(string str)
        {
            var number = ParsePhoneNumber(str);

            try
            {
                var con = Convert.ToInt64(number);
                if (con < 9999999)
                {
                    return "0" + string.Format("{0:###-####}", con);
                }
                else
                    return "0" + string.Format("{0:####-###-####}", con);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return str;
        }
        private static string ParsePhoneNumber(string origPhoneNumber)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in origPhoneNumber.ToCharArray())
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static bool IsEmail(this string text)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(text);
                return addr.Address == text;
            }
            catch
            {
                return false;
            }
        }
    }
}
