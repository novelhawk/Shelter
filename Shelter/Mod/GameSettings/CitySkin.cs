using Newtonsoft.Json;

namespace Mod.GameSettings
{
    public struct CitySkin
    {
        [JsonProperty("houses")]
        public string[] Houses { get; set; }
        
        [JsonProperty("skybox")]
        public string[] Skybox { get; set; }
        
        [JsonProperty("ground")]
        public string Ground { get; set; }
        
        [JsonProperty("walls")]
        public string Walls { get; set; }
        
        [JsonProperty("gate")]
        public string Gate { get; set; }
    }
}