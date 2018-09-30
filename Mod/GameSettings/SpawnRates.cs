namespace Mod.GameSettings
{
    public struct SpawnRates
    {
        public double Normal;
        public double Aberrant;
        public double Jumper;
        public double Crawler;
        public double Punk;

        public SpawnRates(string normal, string aberrant, string jumper, string crawler, string punk)
        {
            if (!double.TryParse(normal, out Normal))
                Normal = 20;

            if (!double.TryParse(aberrant, out Aberrant))
                Aberrant = 20;

            if (!double.TryParse(jumper, out Jumper))
                Jumper = 20;

            if (!double.TryParse(crawler, out Crawler))
                Crawler = 20;

            if (!double.TryParse(punk, out Punk))
                Punk = 20;
            
            VerifySums();
        }

        private void VerifySums()
        {
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
    }
}