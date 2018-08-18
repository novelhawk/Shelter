using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mod
{
    public static class Extension
    {
        public static int ToInt(this object obj) => Convert.ToInt32(obj);

        public static string HexColor(this object str)
        {
            StringBuilder builder = new StringBuilder(str as string);
            foreach (Match match in Regex.Matches(builder.ToString(), @"\[([0-9a-fA-F]{6})\]"))
            {
                builder.Replace(match.Value, $"<color=#{match.Groups[1].Value}>");
                builder.Append("</color>");
            }
            return builder.Replace("[-]", string.Empty).ToString();
        }

        public static string MaxChars(this string str, int chars)
        {
            if (str.Length - 4 > chars)
                return str.Substring(0, chars) + "...";
            return str;
        }

        public static byte[] ToBytes(this Stream input)
        {
            byte[] buffer = new byte[8 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        public static string RemoveColors(this object str)
        {
            return Regex.Replace(str as string, @"\[\w{3,8}\]|\[-\]|\<\/color\>|\<color=#\w+\>", "", RegexOptions.IgnoreCase);
        }

        public static bool EqualsIgnoreCase(this string str, string str1)
        {
            if (str == null && str1 == null) 
                return true;
            if (str != null && str1 != null)
                return str.Equals(str1, StringComparison.CurrentCultureIgnoreCase);
            return false;
        }

        public static bool ContainsIgnoreCase(this string str, string str1)
        {
            if (str == null && str1 == null) 
                return true;
            if (str != null && str1 != null)
                return str.IndexOf(str1, StringComparison.CurrentCultureIgnoreCase) > -1;
            return false;
        }

        public static bool AnyEqualsIgnoreCase(this string[] arr, string str)
        {
            if (arr != null)
                return arr.Any(x => x.EqualsIgnoreCase(str));
            return false;
        }
        public static bool AnyContainsIgnoreCase(this string[] arr, string str)
        {
            if (arr != null)
                return arr.Any(x => x.ContainsIgnoreCase(str));
            return false;
        }

        public static bool ToBool(this object obj, bool @default = false)
        {
            if (obj != null)
                return (bool) obj;
            return @default;
        }
    }
}
