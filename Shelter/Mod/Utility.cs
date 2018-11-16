using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using LogType = Mod.Logging.LogType;

namespace Mod
{
    public static class Utility
    {
        public static bool GetBoolean(string key)
        {
            return PlayerPrefs.GetInt(key, 0) != 0;
        }
        
        public static bool IsValidImageUrl(string url)
        {
            var regex = Regex.Match(url, @"https?:\/\/(?:www\.)?.*?(\w+)\.\w+\/[^\?]+\.(?:png|jpg|gif|jpeg)(\?.*)?");
            if (!regex.Success) return false;
            switch (regex.Groups[1].Value.ToUpperInvariant())
            {
                case "IMGUR":
                case "TINYPIC":
                case "DISCORDAPP":
                case "POSTIMG":
                case "STATICFLICKR":
                    return true;
                default:
                    Shelter.LogConsole("{0} is not an allowed domain.", LogType.Warning, regex.Groups[1].Value);
                    Shelter.Log("{0} is now an allowed domain.", LogType.Warning, url);
                    return false;
            }
        }

        
        public static string ValidateUnityTags(string text)
        {
            StringBuilder builder = new StringBuilder(text);

            Stack<Tag> tags = new Stack<Tag>();
            var matches = Regex.Matches(text, @"<(\/?)(\w+)(?:=.+?)?>");
            foreach (Match match in matches)
            {
                var tag = new Tag(
                    match.Groups[2].Value,
                    string.IsNullOrEmpty(match.Groups[1].Value));

                if (tag.IsOpeningTag)
                {
                    tags.Push(tag);
                }
                else
                {
                    if (tags.Count > 0 && tags.Peek().TagName == tag.TagName)
                        tags.Pop();
                    else
                        builder.Remove(match.Index, match.Length);
                }
            }
            while (tags.Count > 0)
                builder.Append(tags.Pop().ClosingTag);

            return builder.ToString();
        }

        private struct Tag
        {
            private readonly string _name;
            private readonly bool _isOpening;

            public Tag(string name, bool isOpening)
            {
                _name = name;
                _isOpening = isOpening;
            }

            public string TagName => _name;
            public bool IsOpeningTag => _isOpening;
            
            public string ClosingTag => $"</{TagName}>";
        }
    }
}
