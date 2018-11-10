using System.Collections.Generic;

namespace Mod.Profiles
{
    public class Profile
    {
        public Dictionary<string, string> Properties = new Dictionary<string, string>(7);

        public string ProfileName 
        {
            get => Properties["profileName"];
            set => Properties["profileName"] = value;
        }
        
        public string Name 
        {
            get => Properties["name"];
            set
            {
                Properties["name"] = value;
                _hexName = value.HexColor();
            }
        }
        
        private string _hexName;
        public string HexName => _hexName;
        
        public string Guild 
        {
            get => Properties["guild"];
            set => Properties["guild"] = value;
        }
        
        public string ChatName 
        {
            get => Properties["chatName"];
            set => Properties["chatName"] = value;
        }
        
        public string FriendName 
        {
            get => Properties["friendName"];
            set => Properties["friendName"] = value;
        }
        
        public string ChatFormat 
        {
            get => Properties["chatFormat"];
            set => Properties["chatFormat"] = value;
        }
        
        public static Profile DefaultProfile => new Profile
        {
            ProfileName = "Default Profile",
            Name = "Name",
            Guild = "Guild",
            ChatName = "ChatName",
            FriendName = "FriendName",
            ChatFormat = "$(chatName): $(content)"
        };
    }
}
