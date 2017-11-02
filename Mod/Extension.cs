using System;
using System.IO;
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

        public static bool ContainsIgnoreCase(this string str, string str1)
        {
            return str.ToLower().Contains(str1.ToLower());
        }

        public static bool ToBool(this object obj, bool @default = false)
        {
            if (obj != null)
                return (bool) obj;
            return @default;
        }
    }
}
