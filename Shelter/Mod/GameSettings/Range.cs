using System;

namespace Mod.GameSettings
{
    public struct Range : IEquatable<Range>
    {
        public static Range Zero = new Range();
        
        private readonly float _min;
        private readonly float _max;
        
        public Range(float min, float max)
        {
            _min = min < max ? min : max; 
            _max = min < max ? max : min; 
        }
        
        public float Min => _min;
        public float Max => _max;

        public float Random => UnityEngine.Random.Range(_min, _max);

        public float Clamp(float num)
        {
            if (num < _min)
                return _min;
            if (num > _max)
                return _max;
            return num;
        }
        
        public bool Contains(float num)
        {
            return _min < num && num < _max;
        }

        public override string ToString()
        {
            return $"{_min:F2}-{_max:F2}";
        }

        public static bool operator ==(Range range, Range other)
        {
            return range.Equals(other);
        }

        public static bool operator !=(Range range, Range other)
        {
            return !range.Equals(other);
        }
        
        public bool Equals(Range other)
        {
            return _min.Equals(other._min) && _max.Equals(other._max);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Range range && Equals(range);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_min.GetHashCode() * 397) ^ _max.GetHashCode();
            }
        }
    }
}