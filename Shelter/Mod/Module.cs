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
        
        protected string PlayerPref => $"MOD|{ID}";
        private bool _visible;
        private bool _isEnabled;

        public bool Enabled => _isEnabled;
        
        protected virtual void OnModuleEnable() {}
        protected virtual void OnModuleDisable() {}
        protected virtual void OnModuleUpdate() {}
        protected virtual void OnModuleStatusChange(bool status) {}

        protected virtual void OnGuiOpen() {}
        public virtual void Render(Rect windowRect) {}
        protected virtual void OnGuiClose() {}

        private void Start()
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

        private void OnStatusUpdate()
        {
            if (_isEnabled)
                OnModuleEnable();
            else
                OnModuleDisable();

            OnModuleStatusChange(_isEnabled);
        }
        
        #region Public methods

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
            if (_visible)
                return;
            
            _visible = true;
            OnGuiOpen();
        }

        public void Close()
        {
            if (!_visible)
                return;
            
            _visible = false;
            OnGuiClose();
        }

        #endregion
    }
}