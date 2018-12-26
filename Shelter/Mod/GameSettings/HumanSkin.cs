using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Mod.GameSettings
{
    public struct HumanSkin
    {
        private const int HumanSkins = 13;
        
        public string[] Set => new string[HumanSkins]
        {
            Horse, Hair, Eye, Glass, 
            Face, Body, Costume, Cape, 
            LeftBlade, RightBlade, Gas, Hoodie, Trail
        };
        
        [CanBeNull]
        [JsonProperty("horse")]
        public string Horse { get; set; }
        
        [CanBeNull]
        [JsonProperty("hair")]
        public string Hair { get; set; }
        
        [CanBeNull]
        [JsonProperty("eye")]
        public string Eye { get; set; }
        
        [CanBeNull]
        [JsonProperty("glass")]
        public string Glass { get; set; }
        
        [CanBeNull]
        [JsonProperty("face")]
        public string Face { get; set; }
        
        [CanBeNull]
        [JsonProperty("body")]
        public string Body { get; set; }
        
        [CanBeNull]
        [JsonProperty("hoodie")]
        public string Hoodie { get; set; }
        
        [CanBeNull]
        [JsonProperty("costume")]
        public string Costume { get; set; }
        
        [CanBeNull]
        [JsonProperty("cape")]
        public string Cape { get; set; }
        
        [CanBeNull]
        [JsonProperty("leftBlade")]
        public string LeftBlade { get; set; }
        
        [CanBeNull]
        [JsonProperty("rightBlade")]
        public string RightBlade { get; set; }
        
        [CanBeNull]
        [JsonProperty("gas")]
        public string Gas { get; set; }
        
        [CanBeNull]
        [JsonProperty("weapontrail")]
        public string Trail { get; set; }

        public HumanSkin(IList<string> skins)
        {
            Horse = skins[0];
            Hair = skins[1];
            Eye = skins[2];
            Glass = skins[3];
            Face = skins[4];
            Body = skins[5];
            Costume = skins[6];
            Cape = skins[7];
            LeftBlade = skins[8];
            RightBlade = skins[9];
            Gas = skins[10];
            Hoodie = skins[11];
            Trail = skins[12];
        }

        public static bool TryParse(string str, out HumanSkin skin)
        {
            if (str == null)
            {
                skin = default;
                return false;
            }
            
            string[] urls = str.Split(',');
            if (urls.Length != HumanSkins)
            {
                skin = default;
                return false;
            }

            for (var i = 0; i < HumanSkins; i++)
                if (!Utility.IsValidImageUrl(urls[i]))
                    urls[i] = null;

            skin = new HumanSkin(urls);
            return true;
        }
    }
}