using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mod.Animation
{
    public struct AnimationFile
    {
        public const string Name = "animations.json";
        
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        
        [JsonProperty("selected")]
        public int Selected { get; set; }
        
        [JsonProperty("animations")]
        public List<AnimationInfo> Animations { get; set; }
    }
}