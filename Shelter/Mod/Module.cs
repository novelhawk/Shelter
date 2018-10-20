using UnityEngine;

namespace Mod
{
    public abstract class Module : MonoBehaviour
    {
        public string ID => this.GetType().Name;
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract bool IsAbusive { get; }
        public abstract bool HasGUI { get; }
        
        public bool Enabled => _isEnabled;
        public bool Visible;
        
        protected string PlayerPref => $"MOD|{ID}";
        private bool _isEnabled;
        
        protected virtual void OnModuleEnable() {}
        protected virtual void OnModuleDisable() {}
        protected virtual void OnModuleUpdate() {}
        protected virtual void OnModuleStatusChange(bool status) {}

        protected virtual void OnGuiOpen() {}
        public virtual void Render(Rect windowRect) {}
        protected virtual void OnGuiClose() {}

        public void Start()
        {
            _isEnabled = PlayerPrefs.GetString(PlayerPref, "False") == "True";
            if (_isEnabled)
                OnModuleEnable();
        }

        private void Update()
        {
            if (_isEnabled)
                OnModuleUpdate();
        }

        public void Enable()
        {
            if (_isEnabled)
                return;

            _isEnabled = true;
            PlayerPrefs.SetString(PlayerPref, _isEnabled.ToString());
            OnStatusUpdate();
        }

        public void Disable()
        {
            if (!_isEnabled)
                return;

            _isEnabled = false;
            PlayerPrefs.SetString(PlayerPref, _isEnabled.ToString());
            OnStatusUpdate();
        }

        public void Toggle()
        {
            _isEnabled = !_isEnabled;
            PlayerPrefs.SetString(PlayerPref, _isEnabled.ToString());
            OnStatusUpdate();
        }

        public void Open()
        {
            if (Visible)
                return;
            
            Visible = true;
            OnGuiOpen();
        }

        public void Close()
        {
            if (!Visible)
                return;
            
            Visible = false;
            OnGuiClose();
        }

        private void OnStatusUpdate()
        {
            if (_isEnabled)
                OnModuleEnable();
            else
                OnModuleDisable();

            OnModuleStatusChange(_isEnabled);
        }
    }
}