using Newtonsoft.Json;
using UnityEngine;

namespace Mod.Animation
{
    public struct AnimationColor
    {
        [JsonProperty("r")]
        public byte R { get; set; }
        
        [JsonProperty("g")]
        public byte G { get; set; }
        
        [JsonProperty("b")]
        public byte B { get; set; }
        
        [JsonProperty("a", NullValueHandling = NullValueHandling.Ignore)]
        public byte A { get; set; }

        public AnimationColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public AnimationColor(float r, float g, float b, float a)
        {
            R = (byte) r;
            G = (byte) g;
            B = (byte) b;
            A = (byte) a;
        }

        public AnimationColor(Color color)
        {
            R = (byte) (color.r * 255f);
            G = (byte) (color.g * 255f);
            B = (byte) (color.b * 255f);
            A = (byte) (color.a * 255f);
        }

        public string ToHex()
        {
            return $"{R:X2}{G:X2}{B:X2}";
        }

        public string ToHexFull()
        {
            return $"{R:X2}{G:X2}{B:X2}{A:X2}";
        }

        public static AnimationColor Lerp(AnimationColor a, AnimationColor b, float t)
        {
            return new AnimationColor(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
        }
        
        public static implicit operator Color(AnimationColor color)
        {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
    }
}