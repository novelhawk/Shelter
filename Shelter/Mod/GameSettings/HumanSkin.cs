using Newtonsoft.Json;

namespace Mod.GameSettings
{
    public struct HumanSkin
    {
        public string[] Set => new[]
        {
            Horse, Hair, Eye, Glass, 
            Face, Body, Costume, Cape, 
            LeftBlade, RightBlade, Hoodie, Trail
        };
        
        [JsonProperty("horse")]
        public string Horse { get; set; }
        
        [JsonProperty("hair")]
        public string Hair { get; set; }
        
        [JsonProperty("eye")]
        public string Eye { get; set; }
        
        [JsonProperty("glass")]
        public string Glass { get; set; }
        
        [JsonProperty("face")]
        public string Face { get; set; }
        
        [JsonProperty("body")]
        public string Body { get; set; }
        
        [JsonProperty("costume")]
        public string Costume { get; set; }
        
        [JsonProperty("cape")]
        public string Cape { get; set; }
        
        [JsonProperty("leftblade")]
        public string LeftBlade { get; set; }
        
        [JsonProperty("rightblade")]
        public string RightBlade { get; set; }
        
        [JsonProperty("hoodie")]
        public string Hoodie { get; set; }
        
        [JsonProperty("weapontrail")]
        public string Trail { get; set; }
    }
}