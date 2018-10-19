using System;

namespace Mod.GameSettings
{
    public struct SpawnRates : IEquatable<SpawnRates>
    {
        public static SpawnRates Zero = new SpawnRates();
        
        public readonly float Normal;
        public readonly float Aberrant;
        public readonly float Jumper;
        public readonly float Crawler;
        public readonly float Punk;

        public SpawnRates(string normal, string aberrant, string jumper, string crawler, string punk)
        {
            if (!float.TryParse(normal, out Normal))
                Normal = 20;

            if (!float.TryParse(aberrant, out Aberrant))
                Aberrant = 20;

            if (!float.TryParse(jumper, out Jumper))
                Jumper = 20;

            if (!float.TryParse(crawler, out Crawler))
                Crawler = 20;

            if (!float.TryParse(punk, out Punk))
                Punk = 20;
            
            var sum = Normal + Aberrant + Jumper + Crawler + Punk; 
            if (sum == 100)
                return;

            Normal /= sum;
            Normal *= 100;
            
            Aberrant /= sum;
            Aberrant *= 100;
            
            Jumper /= sum;
            Jumper *= 100;
            
            Crawler /= sum;
            Crawler *= 100;
            
            Punk /= sum;
            Punk *= 100;
        }
        
        public SpawnRates(float normal, float aberrant, float jumper, float crawler, float punk)
        {
            Normal = normal;
            Aberrant = aberrant;
            Jumper = jumper;
            Crawler = crawler;
            Punk = punk;
            
            var sum = normal + aberrant + jumper + crawler + punk; 
            if (sum == 100)
                return;

            Normal /= sum;
            Normal *= 100;
            
            Aberrant /= sum;
            Aberrant *= 100;
            
            Jumper /= sum;
            Jumper *= 100;
            
            Crawler /= sum;
            Crawler *= 100;
            
            Punk /= sum;
            Punk *= 100;
        }

        public static bool operator ==(SpawnRates rates, SpawnRates other)
        {
            return rates.Equals(other);
        }

        public static bool operator !=(SpawnRates rates, SpawnRates other)
        {
            return !rates.Equals(other);
        }

        public bool Equals(SpawnRates other)
        {
            return Normal.Equals(other.Normal) && Aberrant.Equals(other.Aberrant) && Jumper.Equals(other.Jumper) && Crawler.Equals(other.Crawler) && Punk.Equals(other.Punk);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SpawnRates rates && Equals(rates);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Aberrant.GetHashCode();
                hashCode = (hashCode * 397) ^ Jumper.GetHashCode();
                hashCode = (hashCode * 397) ^ Crawler.GetHashCode();
                hashCode = (hashCode * 397) ^ Punk.GetHashCode();
                return hashCode;
            }
        }
    }
}