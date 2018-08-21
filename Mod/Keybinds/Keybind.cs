using Newtonsoft.Json;
using UnityEngine;

namespace Mod.Keybinds
{
    public struct Keybind
    {
        [JsonProperty("action")]
        public InputAction Action { get; set; }
        
        [JsonProperty("key")]
        public KeyCode Key { get; set; }
    }
}