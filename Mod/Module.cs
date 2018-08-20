using System;
using Mod.Interface;
using UnityEngine;

namespace Mod
{
    public abstract class Module : MonoBehaviour
    {
        public abstract string ID { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract bool IsAbusive { get; }
        public abstract bool HasGUI { get; }
        public bool Enabled => _isEnabled;
        
        private string PlayerPref => $"MOD|{ID}";
        private bool _isEnabled;
        
        protected virtual void OnModuleEnable()
        {
        }

        protected virtual void OnModuleUpdate()
        {
        }

        protected virtual void OnModuleDisable()
        {
        }

        public virtual Action<Rect> GetGUI()
        {
            return null;
        }

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

        private void OnStatusUpdate()
        {
            if (_isEnabled)
                OnModuleEnable();
            else
                OnModuleDisable();
        }
    }
}