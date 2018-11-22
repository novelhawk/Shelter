using System;
using Mod.Keybinds;
using UnityEngine;

namespace Mod.Interface
{
    public class InGameMenu : Gui
    {
        private Rect _windowRect;
        private Texture2D _background;
        private GUIStyle _menuItem;
        private GUIStyle _menuValue;
        private GUIStyle _menuCategory;
        private GUIStyle _menuEntry;
        private GUIStyle _selectedEntry;
        private float _lastAnimationUpdate;
        private SubMenu _currentMenu;
        
        protected override void OnShow()
        {
            _background = Texture(255, 255, 255);
            
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
            _menuValue = new GUIStyle(_menuItem)
            {
                alignment = TextAnchor.MiddleRight
            };
        }

        private void Update()
        {
            if (_isListening)
            {
                try
                {
                    var input = Input.inputString;
                    for (int i = 0; i < 6; i++)
                        if (Input.GetMouseButtonDown(i))
                            input = $"Mouse{i}";
                    
                    if (string.IsNullOrEmpty(input))
                        return;
                    
                    KeyCode code = (KeyCode) Enum.Parse(typeof(KeyCode), input, true);
                    Shelter.InputManager[_action] = code;
                    Shelter.InputManager.Save(); //TODO: Save after Save is pressed

                    _isListening = false;
                }
                catch (ArgumentException)
                {
                    // ignored
                }
                return;
            }
            
            if (Shelter.InputManager.IsDown(InputAction.OpenMenu))
                Toggle();
            if (Visible && Input.GetKeyDown(KeyCode.Escape))
                Disable();
        }

        private bool EnableDisableButton(Rect rect, string text, bool value)
        {
            GUI.Label(rect, text, _menuItem);
            GUI.Label(rect, value ? "Enabled" : "Disabled", _menuValue);
            if (GUI.Button(rect, string.Empty, GUIStyle.none))
                return !value;
            return value;
        }

        protected override void Render()
        {
            var color = Shelter.Animation.ToColor();
            _selectedEntry.normal.textColor = color;
            
            _windowRect = new Rect(Screen.width / 2f - 400, Screen.height / 2f - 250, 800, 500);
            GUI.DrawTexture(_windowRect, _background);
            
            //PERFORMANCE: Use custom shader instead
            var bar = Texture(color);
            GUI.DrawTexture(new Rect(_windowRect.x, _windowRect.y, _windowRect.width, 4), bar);
            Destroy(bar);

            SmartRect rect = new SmartRect(_windowRect.x, _windowRect.y + 9, _windowRect.width / 5f, 27);
            if (GUI.Button(rect, "General", _currentMenu == SubMenu.General ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.General;
            if (GUI.Button(rect.OX(rect.Width), "Keyboard", _currentMenu == SubMenu.Keyboard ? _selectedEntry : _menuEntry))
                _currentMenu = SubMenu.Keyboard;
            if (GUI.Button(rect.OX(rect.Width), "Game Settings", _currentMenu == SubMenu.GameSettings ? _selectedEntry : _menuEntry))
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
                    DoKeyboard();
                    break;
                case SubMenu.GameSettings:
                    DoGameSettings();
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

            _lastAnimationUpdate = Time.time;
        }
        
        private void DoGeneral()
        {
            const float columns = 2;
            const float betweenSpace = 50;
            const float borderDistance = 30;

            const float categoryDistance = 40;
            
            float width = (_windowRect.width - betweenSpace * (columns - 1) - borderDistance * 2) / columns;
            
            SmartRect category = new SmartRect(_windowRect.x + borderDistance, _windowRect.y + 55, width, 30);
            GUI.Label(category, "Graphics", _menuCategory);
            
            SmartRect item = new SmartRect(category.X, category.Y + categoryDistance, category.Width, 17);
            EnableDisableButton(item, "Skin gas", true);
            EnableDisableButton(item.OY(item.Height + 5), "Weapon trail", true);
            EnableDisableButton(item.OY(item.Height + 5), "Wind effect", true);
            EnableDisableButton(item.OY(item.Height + 10), "VSync", true);
            EnableDisableButton(item.OY(item.Height + 5), "FPS Cap", true);
            EnableDisableButton(item.OY(item.Height + 10), "Texture Quality", true);
            EnableDisableButton(item.OY(item.Height + 5), "Mipmapping", true);
            
            GUI.Label(category.OX(width + betweenSpace), "Other", _menuCategory);
            
            item = new SmartRect(category.X, category.Y + categoryDistance, category.Width, 17);
            EnableDisableButton(item, "Volume", true);
            EnableDisableButton(item.OY(item.Height + 5), "Mouse Speed", true);
            EnableDisableButton(item.OY(item.Height + 5), "Camera Distance", true);
            EnableDisableButton(item.OY(item.Height + 5), "Camera Tilt", true);
            EnableDisableButton(item.OY(item.Height + 5), "Invert Mouse", true);
            EnableDisableButton(item.OY(item.Height + 5), "Speedmeter", true);
            EnableDisableButton(item.OY(item.Height + 5), "Minimap", true);
            EnableDisableButton(item.OY(item.Height + 5), "Game feed", true);
        }

        private InputAction _action;
        private bool _isListening;
        private void KeyboardButton(Rect rect, string text, InputAction action)
        {
            GUI.Label(rect, text, _menuItem);

            var key = Shelter.InputManager[action];
            GUI.Label(rect, key == KeyCode.None ? "<b>+</b>" : key.ToString(), _menuValue);
            if (GUI.Button(rect, string.Empty, GUIStyle.none))
            {
                _action = action;
                _isListening = true;
            }
        }
        
        private void DoKeyboard()
        {
            const float columns = 3;
            const float betweenSpace = 50;
            const float borderDistance = 30;

            const float categoryDistance = 40;

            float width = (_windowRect.width - betweenSpace * (columns - 1) - borderDistance * 2) / columns;
            
            SmartRect category = new SmartRect(_windowRect.x + borderDistance, _windowRect.y + 55, width, 30);
            GUI.Label(category, "Human", _menuCategory);
            
            SmartRect item = new SmartRect(category.X, category.Y + categoryDistance, category.Width, 17);
            
            KeyboardButton(item                    , "Attack", InputAction.Attack);
            KeyboardButton(item.OY(item.Height + 3), "Special", InputAction.Special);
            KeyboardButton(item.OY(item.Height + 3), "Gas", InputAction.Gas);
            
            KeyboardButton(item.OY(item.Height + 10), "Both hooks", InputAction.BothHooks);
            KeyboardButton(item.OY(item.Height + 3), "Left hook", InputAction.LeftHook);
            KeyboardButton(item.OY(item.Height + 3), "Right hook", InputAction.RightHook);
            
            KeyboardButton(item.OY(item.Height + 10), "Forward", InputAction.Forward);
            KeyboardButton(item.OY(item.Height + 3), "Back", InputAction.Back);
            KeyboardButton(item.OY(item.Height + 3), "Right", InputAction.Right);
            KeyboardButton(item.OY(item.Height + 3), "Left", InputAction.Left);
            
            KeyboardButton(item.OY(item.Height + 10), "Reel in", InputAction.ReelIn);
            KeyboardButton(item.OY(item.Height + 3), "Reel out", InputAction.ReelOut);
            
            KeyboardButton(item.OY(item.Height + 10), "Suicide", InputAction.Suicide);
            KeyboardButton(item.OY(item.Height + 3), "Dodge", InputAction.Dodge);
            KeyboardButton(item.OY(item.Height + 3), "Reload", InputAction.Reload);
            
            GUI.Label(category.OX(width + betweenSpace), "Titan", _menuCategory);

            item = new SmartRect(category.X, category.Y + categoryDistance, category.Width, 17);
            KeyboardButton(item                    , "Jump", InputAction.TitanJump);
            KeyboardButton(item.OY(item.Height + 3), "Slam", InputAction.TitanSlam);
            KeyboardButton(item.OY(item.Height + 3), "Double Punch", InputAction.TitanDoublePunch);
            KeyboardButton(item.OY(item.Height + 3), "Cover", InputAction.TitanCover);
            KeyboardButton(item.OY(item.Height + 3), "Grab front", InputAction.TitanGrabFront);
            KeyboardButton(item.OY(item.Height + 3), "Grab back", InputAction.TitanGrabBack);
            KeyboardButton(item.OY(item.Height + 3), "Grab nape", InputAction.TitanGrabNape);
            KeyboardButton(item.OY(item.Height + 3), "AntiAE", InputAction.TitanAntiAE);
            KeyboardButton(item.OY(item.Height + 3), "Bite", InputAction.TitanBite);
            
            GUI.Label(category.OX(width + betweenSpace), "Other", _menuCategory);
            
            item = new SmartRect(category.X, category.Y + categoryDistance, category.Width, 17);
            KeyboardButton(item                    , "Lock titan", InputAction.LockTitan);
            KeyboardButton(item.OY(item.Height + 3), "Slowdown", InputAction.SlowMovement);
            KeyboardButton(item.OY(item.Height + 3), "Horse Jump", InputAction.HorseJump);
            
            KeyboardButton(item.OY(item.Height + 10), "Enter cannon", InputAction.EnterCannon);
            KeyboardButton(item.OY(item.Height + 3), "Change camera mode", InputAction.ChangeCamera);
            
            KeyboardButton(item.OY(item.Height + 10), "Salute", InputAction.Salute);
            KeyboardButton(item.OY(item.Height + 3), "Red flare", InputAction.RedFlare);
            KeyboardButton(item.OY(item.Height + 3), "Green flare", InputAction.GreenFlare);
            KeyboardButton(item.OY(item.Height + 3), "Black flare", InputAction.BlackFlare);
            
            KeyboardButton(item.OY(item.Height + 10), "Toggle fullscreen", InputAction.ToggleFullscreen);
            KeyboardButton(item.OY(item.Height + 3), "Open menu", InputAction.OpenMenu);
            KeyboardButton(item.OY(item.Height + 3), "Open navigator", InputAction.OpenNavigator);
        }

        private void DoGameSettings()
        {
            const float columns = 3;
            const float betweenSpace = 50;
            const float borderDistance = 30;

            float width = (_windowRect.width - betweenSpace * (columns - 1) - borderDistance * 2) / columns;
        }
        

        protected override void OnHide()
        {
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
