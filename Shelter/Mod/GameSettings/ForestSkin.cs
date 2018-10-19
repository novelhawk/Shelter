using Newtonsoft.Json;

namespace Mod.GameSettings
{
    public class ForestSkin
    {
        [JsonProperty("trees")]
        public string[] Trees { get; set; }
        
        [JsonProperty("leaves")]
        public string[] Leaves { get; set; }
        
        [JsonProperty("skybox")]
        public string[] Skybox { get; set; }
        
        [JsonProperty("ground")]
        public string Ground { get; set; }
    }
}