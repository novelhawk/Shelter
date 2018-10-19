using Newtonsoft.Json;

namespace Mod.Profiles
{
    public class Profile
    {
//        public string HexName { get; }
//        public Hero Character { get; set; }

        public static Profile DefaultProfile => new Profile
        {
            ProfileName = "Default Profile",
            Name = "Name",
            Guild = "Guild",
            ChatName = "ChatName",
            FriendName = "FriendName",
            ChatFormat = "$(chatName): $(message)"
        };

        private string _name;
        private string _hexName;
        
        [JsonProperty("profileName")]
        public string ProfileName { get; set; }

        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _hexName = value.HexColor();
            }
        }

        public string HexName => _hexName;
        
        [JsonProperty("guild")]
        public string Guild { get; set; }
        
        [JsonProperty("chatName")]
        public string ChatName { get; set; }
        
        [JsonProperty("friendName")]
        public string FriendName { get; set; }
        
        [JsonProperty("chatFormat")]
        public string ChatFormat { get; set; }
    }
}
