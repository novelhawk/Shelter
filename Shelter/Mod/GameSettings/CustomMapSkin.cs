using Newtonsoft.Json;

namespace Mod.GameSettings
{
    public struct CustomMapSkin
    {
        [JsonProperty("skybox")]
        public string[] Skybox { get; set; }
        
        [JsonProperty("ground")]
        public string Ground { get; set; }
    }
}