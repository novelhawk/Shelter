using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mod.Animation
{
    public struct AnimationInfo
    {
        [JsonProperty("animationName")]
        public string Name { get; set; }
        
        [JsonProperty("animationType"), JsonConverter(typeof(StringEnumConverter))]
        public AnimationType Type { get; set; }
        
        [JsonProperty("colors", NullValueHandling = NullValueHandling.Ignore)]
        public List<AnimationColor> Colors { get; set; }

        public AnimationInfo(AnimationType type, List<AnimationColor> colors)
        {
            Name = "Game Animation";
            Type = type;
            Colors = colors;
        }
    }
}