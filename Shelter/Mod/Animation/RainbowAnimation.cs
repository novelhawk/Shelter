using System;
using UnityEngine;

namespace Mod.Animation
{
    public class RainbowAnimation
    {
        private readonly bool _autoIncrement;
        private readonly float _increment;
        
        private float _hue;

        public RainbowAnimation()
        {
            _autoIncrement = false;
            _increment = 0;
        }
        
        public RainbowAnimation(bool autoIncrement, float increment)
        {
            _autoIncrement = autoIncrement;
            _increment = increment;
        }
        
        public RainbowAnimation(bool autoIncrement, int degrees)
        {
            _autoIncrement = autoIncrement;
            _increment = degrees / 360f;
        }

        public void Increment(int degrees) => Increment(degrees / 360f);
        public void Increment(float incrementBy)
        {
            _hue += incrementBy;
            if (_hue > 1f)
                _hue -= (int) _hue;
        }

        public Color ToColor()
        {
            if (_autoIncrement)
                Increment(_increment);
            
            var hue = _hue * 360;
            var X = 1 - Math.Abs(hue / 60 % 2 - 1);

            if (hue <= 60)
                return new Color(1, X, 0);
            if (hue <= 120)
                return new Color(X, 1, 0);
            if (hue <= 180)
                return new Color(0, 1, X);
            if (hue <= 240)
                return new Color(0, X, 1);
            if (hue <= 300)
                return new Color(X, 0, 1);
            return new Color(1, 0, X);
        }

        public string ToHex()
        {
            if (_autoIncrement)
                Increment(_increment);
            
            var hue = _hue * 360;
            int X = (int)((1 - Math.Abs(hue / 60 % 2 - 1)) * 255);

            if (hue <= 60)
                return $"FF{X:X2}00";
            if (hue <= 120)
                return $"{X:X2}FF00";
            if (hue <= 180)
                return $"00FF{X:X2}";
            if (hue <= 240)
                return $"00{X:X2}FF";
            if (hue <= 300)
                return $"{X:X2}00FF";
            return $"FF00{X:X2}";
        }
    }
}