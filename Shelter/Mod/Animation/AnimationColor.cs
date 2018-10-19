using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace Mod.Animation
{
    [JsonConverter(typeof(ColorConverter))]
    public struct AnimationColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public AnimationColor(string hex)
        {
            R = G = B = 0;
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            
            if (byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r))
                R = r;
            if (byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
                G = b;
            if (byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g))
                B = g;
            
            A = 255;
            if (hex.Length > 6 && byte.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte a))
                A = a;
        }
        
        public AnimationColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        private AnimationColor(float r, float g, float b, float a)
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

        public override string ToString()
        {
            return ToHex();
        }

        public string ToHex()
        {
            return $"{R:X2}{G:X2}{B:X2}";
        }

        public string ToHexFull()
        {
            if (A != 255)
                return $"{R:X2}{G:X2}{B:X2}{A:X2}";
            return ToHex();
        }

        public static AnimationColor Random => new AnimationColor(UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), 255);

        public static AnimationColor Lerp(AnimationColor a, AnimationColor b, float t)
        {
            return new AnimationColor(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
        }
        
        public static implicit operator Color(AnimationColor color)
        {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
        
        public static List<AnimationColor> Rainbow => new List<AnimationColor>
        {
            new AnimationColor(255, 0, 0, 255), 
            new AnimationColor(255, 127, 0, 255), 
            new AnimationColor(255, 255, 0, 255), 
            new AnimationColor(0, 255, 0, 255),
            new AnimationColor(0, 255, 255, 255),
            new AnimationColor(0, 0, 255, 255),
            new AnimationColor(139, 0, 255, 255)
        };
    }
}