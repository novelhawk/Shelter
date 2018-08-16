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
        private readonly string[] _colors;
        private readonly AnimationInfo _animation;

        private string _lastCompute;

        public Animator(AnimationInfo animation, int shades)
        {
            _animation = animation;
            if (animation.Type != AnimationType.Fader)
                _colors = CreateFade(animation.Colors, shades);
            else
                _colors = GetColors(Shelter.Profile.Name);
        }

        public bool IsValidAnimation()
        {
            throw new NotImplementedException();
        }

        public string Name
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
            switch (_animation.Type)
            {
                case AnimationType.Cycle:
                    _lastCompute = $"[{GetColor(0)}]{Shelter.Profile.Name}";
                    break;
                case AnimationType.Fader:
                    _lastCompute = LeftToRight(Shelter.Profile.Name);
                    break;
                case AnimationType.LeftToRight:
                    _lastCompute = LeftToRight(Shelter.Profile.Name);
                    break;
                case AnimationType.RightToLeft:
                    _lastCompute = RightToLeft(Shelter.Profile.Name);
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

            return _colors[offset];
        }

        private static string[] CreateFade(IList<AnimationColor> colors, int shades)
        {
            var list = new List<string>(colors.Count * shades);
            for (var i = 0; i < colors.Count; i++)
            {
                AnimationColor color = colors[0];
                if (i <= colors.Count - 2)
                    color = colors[i + 1];

                for (var j = 0; j < shades; j++)
                    list.Add(AnimationColor.Lerp(colors[i], color, 1f / shades * j).ToHex());
            }
            return list.ToArray();
        }
        
        private static string[] GetColors(string name)
        {
            MatchCollection matches = Regex.Matches(name, @"\[([A-Fa-f0-9]{6})\]");
            return matches.Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
        }
    }
}