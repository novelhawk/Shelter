using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using UnityEngine;
using LogType = Mod.Logging.LogType;

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

        /// <summary>
        /// Used to compute the length of a number
        /// </summary>
        /// <param name="num">The number to count the number of digits from</param>
        /// <returns>Number of digits of <see cref="num"/></returns>
        public static int Count(this int num)
        {
            if (num < 0)
                num *= -1;
            
            // O(1) algorithm
            if (num < 10) return 1;
            if (num < 100) return 2;
            if (num < 1000) return 3;
            if (num < 10000) return 4;
            if (num < 100000) return 5;
            if (num < 1000000) return 6;
            if (num < 10000000) return 7;
            if (num < 100000000) return 8;
            if (num < 1000000000) return 9;

            // O(2n) algorithm
            num /= 1000000000;
            int n = 9;
            do
            {
                num /= 10;
                n++;
            } while (num > 0);
            return n;
        }

        public static bool EqualsIgnoreCase(this string str, string str1)
        {
            return string.Equals(str, str1, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string str, string str1)
        {
            if (str == null || str1 == null)
                return false;
            
            return str.IndexOf(str1, StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static bool AnyEqualsIgnoreCase(this string[] arr, string str)
        {
            if (arr == null)
                return false;
            
            return arr.Any(x => x.EqualsIgnoreCase(str));
        }

        public static bool AnyContainsIgnoreCase(this string[] arr, string str)
        {
            if (arr == null) 
                return false;
            
            return arr.Any(x => x.ContainsIgnoreCase(str));
        }
    }
}
