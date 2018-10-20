using UnityEngine;

namespace Game
{
    public static class FengColor
    {
        public static Color NightAmbientLight => new Color(0.05f, 0.05f, 0.05f);
        public static Color NightLight => new Color(0.08f, 0.08f, 0.1f);
        
        public static Color DawnAmbientLight => new Color(0.345f, 0.305f, 0.271f);
        public static Color DawnLight => new Color(0.729f, 0.643f, 0.458f);
        
        public static Color DayAmbientLight => new Color(0.494f, 0.478f, 0.447f);
        public static Color DayLight => new Color(1f, 1f, 1f);
        
        public static Color PunkHair1 => new Color(0.45f, 1f, 0f);
        public static Color PunkHair2 => new Color(1f, 0.95f, 0f);
        public static Color PunkHair3 => new Color(1f, 0.1f, 0.05f);
        
        public static Color TitanSkin1 => new Color(0.97f, 0.94f, 0.92f);
        public static Color TitanSkin2 => new Color(0.91f, 0.89f, 0.82f);
        public static Color TitanSkin3 => new Color(0.89f, 0.81f, 0.76f);
        
        public const string RateA = "FAAC58";
        public const string RateB = "F4FA58";
        public const string RateC = "ACFA58";
        public const string RateD = "FFFFFF";
        public const string RateS = "FA8258";
        public const string RateSS = "BE81F7";
        public const string RateSSS = "FF0000";
        public const string RateX = "000000";
    }
}
