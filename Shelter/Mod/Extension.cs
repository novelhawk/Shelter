using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using Mod.Logging;

namespace Mod
{
    public static class Extension
    {
        public static int ToInt(this object obj) => Convert.ToInt32(obj);

        public static bool ToBool(this Hashtable hash, string key)
        {
            if (!hash.ContainsKey(key))
                throw new KeyNotFoundException($"Hashtable has no key {key}.");

            object obj = hash[key];
            switch (obj)
            {
                case int i:
                    return i != 0;
                case bool ret:
                    return ret;
                default:
                    Shelter.LogBoth("Couldn't parse {0} to bool. (of type {1})", LogType.Warning, key, obj.GetType().Name);
                    return false;
            }
        }
        
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
            if (str.Length > chars)
                return str.Substring(0, chars - 3) + "...";
            return str;
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
