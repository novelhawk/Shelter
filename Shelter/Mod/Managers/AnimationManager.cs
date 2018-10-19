using System.Collections.Generic;
using UnityEngine;
using AnimationInfo = Mod.Animation.AnimationInfo;

namespace Mod.Managers
{
    public class AnimationManager
    {
        private readonly ConfigManager<List<AnimationInfo>> _file;
        private List<AnimationInfo> _animations;
        private int _selected;

        public AnimationManager()
        {
            _selected = PlayerPrefs.GetInt("MOD|ModuleNameAnimation.selected", 0);
            _file = new ConfigManager<List<AnimationInfo>>("animations.json");
            Load();
        }

        public void Load()
        {
            _animations = _file.Deserialize();

            if (_selected > _animations.Count - 1)
                _selected = 0;
        }

        public bool Save()
        {
            return _file.WriteFile(_file.Serialize(_animations));
        }

        public int Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;
                
                _selected = value;
                PlayerPrefs.SetInt("MOD|ModuleNameAnimation.selected", value);
            }
        }

        public AnimationInfo Animation => _animations[_selected];
        public List<AnimationInfo> Animations => _animations;
    }
}