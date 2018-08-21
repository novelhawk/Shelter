using System;
using Mod.Keybinds;
using Mod.Managers;
using UnityEngine;

namespace Mod.Interface
{
    public class Navigator : Gui
    {
        private Action<Rect> _currentGUI;
        private GUIStyle _textStyle;
        private GUIStyle _buttonText;
        private GUIStyle _buttonMenu;
        private Texture2D _background;
        private Texture2D _verticalLine;
        private Texture2D _enabledTexture;
        private Texture2D _disabledTexture;
        private string _searchQuery;

        protected override void OnShow()
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
            
            _background = Texture(0, 0, 0, 85);
            _enabledTexture = Texture(100, 220, 255);
            _disabledTexture = Texture(0, 20, 0, 80);
            _verticalLine = Texture(127, 127, 127);
            
            _textStyle = new GUIStyle
            {
                normal = {textColor = Color(255, 255, 255)},
                fontSize = 30
            };
            _buttonText = new GUIStyle
            {
                normal = { textColor = UnityEngine.Color.grey },
                alignment = TextAnchor.MiddleLeft,
                padding = { left = 10 },
                fontSize = 14
            };
            _buttonMenu = new GUIStyle(_buttonText)
            {
                normal = { textColor = Color(0, 149, 255) },
                fontSize = 18
            };
            _searchQuery = string.Empty;
            _currentGUI = null;
        }

        protected override void Render()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _background);
            int width = Screen.width / 100 * 58;
            int height = Screen.height / 100 * 90;
            Rect rect = new Rect(Screen.width / 2f - width / 2f, Screen.height / 2f - height / 2f, width, height);
            if (_currentGUI != null)
            {
                _currentGUI.Invoke(new Rect(Screen.width / 2f - 640, Screen.height / 2f - 360, 1280, 720));
                return;
            }
            
            GUI.Label(new Rect(rect.x, rect.y + 10, rect.width, rect.height - 10), "Search:", _textStyle);
            GUI.Label(new Rect(rect.x + 120, rect.y + 10, rect.width - 240, rect.height - 10), _searchQuery, _textStyle);

            CreateButtons(new Rect(rect.x, rect.y + 50, rect.width - 50, rect.height));
        }

        private void CreateButtons(Rect box)
        {
            int times = Mathf.FloorToInt(box.width/200);
            SmartRect rect = new SmartRect(box.x, box.y, 200, 40);
            foreach (Module module in ModuleManager.Modules)
            {
                if (!string.IsNullOrEmpty(_searchQuery) && !module.Name.ContainsIgnoreCase(_searchQuery))
                    continue;

                if (rect.Y >= box.height)
                    continue;
                
                GUI.DrawTexture(rect, module.Enabled ? _enabledTexture : _disabledTexture);
                GUI.Label(new Rect(rect.X, rect.Y, rect.Width - 30, rect.Height), module.Name, _buttonText);
                if (module.HasGUI)
                {
                    GUI.DrawTexture(new Rect(rect.X + rect.Width - 30, rect.Y + 5, 1, rect.Height - 10), _verticalLine);
                    GUI.Label(new Rect(rect.X + rect.Width - 25 - 10, rect.Y, 25, rect.Height), "▼", _buttonMenu);
                    if (GUI.Button(new Rect(rect.X + rect.Width - 30, rect.Y, 30, rect.Height), string.Empty, GUIStyle.none))
                        _currentGUI = module.GetGUI();
                }
                
                if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width - 30, rect.Height), string.Empty, GUIStyle.none))
                    module.Toggle();

                if (rect.OX((box.width - 200 * times) / times + 200).X - 200 > box.width)
                    rect = new SmartRect(box.x, rect.Y + 50, rect.Width, rect.Height);
            }
        }

        private void Update()
        {
            if (Visible)
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (_searchQuery.Length > 0)
                        _searchQuery = _searchQuery.Substring(0, _searchQuery.Length - 1);
                }
                else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.Return)) { }
                else if (Input.anyKeyDown)
                    _searchQuery += Input.inputString;
            }

            if (Shelter.InputManager.IsDown(InputAction.OpenNavigator))
                Toggle();
        }

        protected override void OnHide()
        {
            Screen.showCursor = false;
            
            Destroy(_background);
            Destroy(_enabledTexture);
            Destroy(_disabledTexture);
        }
    }
}