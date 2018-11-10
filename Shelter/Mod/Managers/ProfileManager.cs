using System.Collections.Generic;
using Mod.Profiles;
using Newtonsoft.Json;

namespace Mod.Managers
{
    public class ProfileManager
    {
        private readonly ConfigManager<ProfileFile> _file;
        private ProfileFile _profileFile;

        public ProfileManager()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> {new ProfileConverter()}
            };
            
            _file = new ConfigManager<ProfileFile>(ProfileFile.Name, settings);
            Load();
        }

        private void Load()
        {
            _profileFile = _file.Deserialize();
            
            if (_profileFile.Profiles.Count < 1)
                _profileFile = _file.Deserialize(_file.InvalidateCurrentFile());

            if (_profileFile.Selected > _profileFile.Profiles.Count - 1)
                _profileFile.Selected = 0;
        }

        public bool Save()
        {
            return _file.WriteFile(_file.Serialize(_profileFile));
        }

        public ProfileFile ProfileFile => _profileFile;
    }
}