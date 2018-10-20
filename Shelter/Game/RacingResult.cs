using System;

namespace Game
{
    public class RacingResult : IComparable
    {
        public string name;
        public float time;
        
        public int CompareTo(object obj)
        {
            if (obj is RacingResult other)
            {
                if (time > other.time)
                    return 1;
                if (time < other.time)
                    return -1;
                
                return 0;
            }

            return 1;
        }
    }
}

