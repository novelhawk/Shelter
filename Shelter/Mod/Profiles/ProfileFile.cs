using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mod.Profiles
{
    public class ProfileFile
    {
        public const string Name = "profiles.json";
        private List<Profile> _profiles;
        
        [JsonProperty("selected")]
        public int Selected { get; set; }

        [JsonProperty("profiles")]
        public List<Profile> Profiles
        {
            get => _profiles;
            set
            {
//                for (int i = value.Count - 1; i >= 0; i--)
//                {
//                    if (!IsValid(value[i]))
//                    {
//                        value.RemoveAt(i);
//                        if (i < Selected)
//                            Selected--;
//                    }
//                }
                
                
                _profiles = value;
            }
        }

        public Profile SelectedProfile => Profiles[Selected];

        private static bool IsValid(Profile profile) //TODO: Make a validator
        {
            return true;
        }
    }
}