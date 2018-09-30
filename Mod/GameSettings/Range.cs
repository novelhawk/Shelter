namespace Mod.GameSettings
{
    public struct Range
    {
        private readonly int _min;
        private readonly int _max;
        
        public Range(int min, int max)
        {
            _min = min < max ? min : max;
            _max = min < max ? max : min;
        }

        public int Min => _min;
        public int Max => _max;
        
        public override string ToString()
        {
            return $"{_min}-{_max}";
        }
    }
}