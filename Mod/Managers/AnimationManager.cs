using System.Collections.Generic;
using Mod.Animation;

namespace Mod.Managers
{
    public class AnimationManager
    {
        private readonly ConfigManager<AnimationFile> _file;
        private AnimationFile _animationFile;
        
        //BUG: File doesn't get created if not present
        public AnimationManager()
        {       
            _file = new ConfigManager<AnimationFile>(AnimationFile.Name);
            Load();
        }

        public void Load()
        {
            _animationFile = _file.Deserialize();

            if (_animationFile.Selected > _animationFile.Animations.Count - 1)
                _animationFile.Selected = 0;
        }

        public bool Save()
        {
            return _file.WriteFile(_file.Serialize(_animationFile));
        }

        public AnimationFile AnimationFile => _animationFile;

        public bool Enabled
        {
            get => _animationFile.Enabled;
            set => _animationFile.Enabled = value;
        }

        public int Selected
        {
            get => _animationFile.Selected;
            set => _animationFile.Selected = value;
        }
        public AnimationInfo Animation => _animationFile.Animations[_animationFile.Selected];
        public List<AnimationInfo> Animations => _animationFile.Animations;
    }
}