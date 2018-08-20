using System;
using System.CodeDom;
using UnityEngine;
using Animator = Mod.Animation.Animator;

namespace Mod.Interface
{
    public class InGameMenu : Gui
    {
        private Animator _animator;
        private Texture2D _background;
        private Texture2D _topBar;
        private GUIStyle _menuItem;
        private GUIStyle _menuCategory;
        private GUIStyle _menuEntry;
        private GUIStyle _selectedEntry;
        private float _lastAnimationUpdate;
        private SubMenu _currentMenu;
        
        protected override void OnShow()
        {
            _background = Texture(255, 255, 255);
            _topBar = Texture(255, 0, 0);
            _animator = new Animator(Shelter.Animation, 20);
            _animator.ComputeNext();
            
            _menuEntry = new GUIStyle
            {
                normal = {textColor = Color(112, 112, 112)},
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            _selectedEntry = new GUIStyle(_menuEntry);
            _menuCategory = new GUIStyle
            {
                normal = {textColor = Color(255, 111, 0)},
                fontSize = 16,
                alignment = TextAnchor.UpperCenter
            };
            _menuItem = new GUIStyle
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };
        }

        protected override void Render()
        {
            _selectedEntry.normal.textColor = _animator.LastColor;
            
            windowRect = new Rect(Screen.width / 2f - 400, Screen.height / 2f - 250, 800, 500);
            GUI.DrawTexture(windowRect, _background);
            
            _topBar = Texture(_animator.LastColor.R, _animator.LastColor.G, _animator.LastColor.B, _animator.LastColor.A);
            GUI.DrawTexture(new Rect(windowRect.x, windowRect.y, windowRect.width, 4), _topBar);
            Destroy(_topBar);

            SmartRect rect = new SmartRect(windowRect.x, windowRect.y + 9, windowRect.width / 5f, 27);
            if (GUI.Button(rect, "General", _currentMenu == SubMenu.General ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.General;
            if (GUI.Button(rect.OX(rect.Width), "Keyboard", _currentMenu == SubMenu.Keyboard ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.Keyboard;
            if (GUI.Button(rect.OX(rect.Width), "Game Settings",
                _currentMenu == SubMenu.GameSettings ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.GameSettings;
            if (GUI.Button(rect.OX(rect.Width), "Skins", _currentMenu == SubMenu.Skins ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.Skins;
            if (GUI.Button(rect.OX(rect.Width), "Custom Map", _currentMenu == SubMenu.CustomMap ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.CustomMap;

            switch (_currentMenu)
            {
                case SubMenu.General:
                    DoGeneral();
                    break;
                case SubMenu.Keyboard:
                    break;
                case SubMenu.GameSettings:
                    break;
                case SubMenu.Skins:
                    break;
                case SubMenu.CustomMap:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (Time.time - _lastAnimationUpdate < 0.05f)
                return;

            _animator.ComputeNext();
            _lastAnimationUpdate = Time.time;
        }
        
        

        private void DoGeneral()
        {
            const float columns = 3;
            const float betweenSpace = 60;
            
            float width = (windowRect.width - 50) / columns - (betweenSpace * columns - 1) / columns;
            
            SmartRect category = new SmartRect(windowRect.x + 15, windowRect.y + 60, width, windowRect.height - 75);
            GUI.Label(category, "Graphics", _menuCategory);
            
            SmartRect item = new SmartRect(category.X, category.Y + 20, category.Width, 17);
            GUI.Label(item, "Skin gas", _menuItem);
            GUI.Label(item.OY(item.Height + 5), "Skin gas", _menuItem);
            GUI.Label(item.OY(item.Height + 5), "Wind effect", _menuItem);
            GUI.Label(item.OY(item.Height + 10), "VSync", _menuItem);
            GUI.Label(item.OY(item.Height + 5), "FPS Cap", _menuItem);
            GUI.Label(item.OY(item.Height + 10), "Texture Quality", _menuItem);
            GUI.Label(item.OY(item.Height + 5), "Mipmapping", _menuItem);
            
            
            GUI.Label(category.OX(width + betweenSpace), "None", _menuCategory);
            GUI.Label(category.OX(width + betweenSpace), "Other", _menuCategory);

        }
        

        protected override void OnHide()
        {
            _animator = null;
            if (_topBar != null)
                Destroy(_topBar);
            Destroy(_background);
        }

        private enum SubMenu
        {
            General,
            Keyboard,
            GameSettings,
            Skins,
            CustomMap,
        }
    }
}
