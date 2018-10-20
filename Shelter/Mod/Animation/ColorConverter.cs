using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mod.Animation
{
    public class ColorConverter : JsonConverter<AnimationColor>
    {
        private const string Pattern = @"(?:rgb\()?.*?(\d+)[,\s]+(\d+)[,\s]+(\d+)(?:[,\s]+(\d+))?.*?\)?|#?([A-Fa-f0-9]{6,8})";
        
        public override void WriteJson(JsonWriter writer, AnimationColor value, JsonSerializer serializer)
        {
            writer.WriteValue($"#{value.ToHexFull()}");
        }

        public override AnimationColor ReadJson(JsonReader reader, Type objectType, AnimationColor existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is string str)
            {
                Match match = Regex.Match(str, Pattern, RegexOptions.IgnoreCase);
                if (!match.Success)
                    throw new Exception("Cannot parse AnimationColor");

                if (!string.IsNullOrEmpty(match.Groups[5].Value))
                    return new AnimationColor(match.Groups[5].Value);
                
                if (byte.TryParse(match.Groups[1].Value, out byte r) &&
                    byte.TryParse(match.Groups[2].Value, out byte g) &&
                    byte.TryParse(match.Groups[3].Value, out byte b))
                {
                    if (!byte.TryParse(match.Groups[4].Value, out byte a))
                        a = 255;
                    
                    return new AnimationColor(r, g, b, a);
                }
            }
            return new AnimationColor(0, 0, 0, 0);
        }
    }
}