using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mod.Interface;

namespace Mod.Animation
{
    public class Animator
    {
        private readonly AnimationType _type;
        private readonly AnimationColor[] _colors;
        private readonly string _text;

        private string _lastCompute;
        private AnimationColor _lastColor;

        public Animator(AnimationInfo animation, int shades) : this(null, animation, shades) { }

        public Animator(string text, AnimationInfo animation, int shades)
        {
            _text = text;
            _type = animation.Type;
            if (_type != AnimationType.Fader)
                _colors = CreateFade(animation.Colors, shades);
            else
                _colors = GetColors(Shelter.Profile.Name);
        }

        public bool IsValidAnimation()
        {
            throw new NotImplementedException();
        }

        public AnimationColor LastColor => _lastColor;
        public string Current
        {
            get
            {
                if (_lastCompute == null)
                    ComputeNext();

                return _lastCompute;
            }
        }

        private int _index;
        public void ComputeNext()
        {
            switch (_type)
            {
                case AnimationType.Cycle:
                    _lastCompute = $"[{GetColor(0)}]{_text ?? Shelter.Profile.Name}";
                    break;
                case AnimationType.Fader:
                case AnimationType.LeftToRight:
                    _lastCompute = LeftToRight(_text ?? Shelter.Profile.Name);
                    break;
                case AnimationType.RightToLeft:
                    _lastCompute = RightToLeft(_text ?? Shelter.Profile.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _index++;
        }

        private string LeftToRight(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length + name.Length * 8);
            for (int i = name.Length - 1; i >= 0; i--)
                builder.AppendFormat("[{0}]{1}", GetColor(i), name[name.Length - i - 1]);
            return builder.ToString();
        }

        private string RightToLeft(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length + name.Length * 8);
            for (var i = 0; i < name.Length; i++)
                builder.AppendFormat("[{0}]{1}", GetColor(i), name[i]);
            return builder.ToString();
        }
        
        private string GetColor(int offset)
        {
            offset += _index;
            if (offset > _colors.Length - 1)
                offset %= _colors.Length;

            return (_lastColor = _colors[offset]).ToHex();
        }

        private static AnimationColor[] CreateFade(IList<AnimationColor> colors, int shades)
        {
            var list = new List<AnimationColor>(colors.Count * shades);
            for (var i = 0; i < colors.Count; i++)
            {
                AnimationColor color = colors[0];
                if (i <= colors.Count - 2)
                    color = colors[i + 1];

                for (var j = 0; j < shades; j++)
                    list.Add(AnimationColor.Lerp(colors[i], color, 1f / shades * j));
            }
            return list.ToArray();
        }
        
        private static AnimationColor[] GetColors(string name)
        {
            MatchCollection matches = Regex.Matches(name, @"\[([A-Fa-f0-9]{6})\]");
            return matches.Cast<Match>().Select(match => new AnimationColor(match.Groups[1].Value)).ToArray();
        }
    }
}