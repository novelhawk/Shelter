using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mod.Interface.Components;

namespace Mod
{
    public static class Utility
    {
        public static bool IsValidTag(string tagName)
        {
            tagName = tagName.ToLower();
            switch (tagName)
            {
//                case "size": // It doesnt get buggy even if you don't insert any number
                case "color": // Couldn't find any instance of it bugging out with all sorts of args
                case "i":
                case "b":
                    return true;
                default:
                    return false;
            }
        }

        public static string CheckHTMLTags(string text)
        {
            StringBuilder builder = new StringBuilder(text);

            Stack<Tag> stack = new Stack<Tag>();
            foreach (Match match in Regex.Matches(text, @"<([\\\/]?)([^\/]+?)(?:=(.+?))?>"))
            {
                if (!IsValidTag(match.Groups[2].Value))
                {
                    builder.Replace(match.Value, string.Empty);
                    continue;
                }

                Tag tag = new Tag(
                    match.Value,
                    match.Groups[2].Value,
                    text.IndexOf(match.Value, StringComparison.Ordinal),
                    match.Value.Length,
                    string.IsNullOrEmpty(match.Groups[1].Value));


                if (tag.IsOpeningTag)
                    stack.Push(tag);
                else if (stack.Peek().TagName == tag.TagName)
                    stack.Pop();
                else
                    builder.Replace(tag.TagFull, stack.Pop().ClosingTag, tag.Index, tag.Length);
            }
            while (stack.Count > 0)
                builder.Append(stack.Pop().ClosingTag);

            return builder.ToString();
        }

        private class Tag
        {
            public bool IsOpeningTag { get; }
            public string ClosingTag => $"</{TagName}>";
            public string TagFull { get; }
            public string TagName { get; }
            public int Index { get; }
            public int Length { get; }

            public Tag(string tagFull, string tagName, int index, int length, bool isOpeningTag)
            {
                TagFull = tagFull;
                TagName = tagName;
                Index = index;
                Length = length;
                IsOpeningTag = isOpeningTag;
            }
        }
    }
}
