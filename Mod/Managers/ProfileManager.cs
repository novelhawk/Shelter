using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
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
            _file = new ConfigManager<ProfileFile>(ProfileFile.Name);
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