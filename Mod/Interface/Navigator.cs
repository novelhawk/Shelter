using System;
using Mod.Keybinds;
using Mod.Managers;
using UnityEngine;

namespace Mod.Interface
{
    public class Navigator : Gui
    {
        private Module _module;
        private GUIStyle _textStyle;
        private GUIStyle _buttonText;
        private GUIStyle _buttonMenu;
        private GUIStyle _moduleName;
        private GUIStyle _moduleDescription;
        private GUIStyle _moduleButton;
        private Texture2D _background;
        private Texture2D _moduleBackground;
        private Texture2D _verticalLine;
        private Texture2D _enabledTexture;
        private Texture2D _disabledTexture;
        private string _searchQuery;

        protected override void OnShow()
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
            
            _background = Texture(0, 0, 0, 85);
            _moduleBackground = Texture(255, 255, 255, 120);
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
            _moduleButton = new GUIStyle(_buttonMenu)
            {
                alignment = TextAnchor.MiddleCenter
            };
            _moduleName = new GUIStyle
            {
                fontSize = 20,
                normal = {textColor = Color(255, 82, 241)},
                alignment = TextAnchor.MiddleCenter
            };
            _moduleDescription = new GUIStyle
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            _searchQuery = string.Empty;
        }

        protected override void Render()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _background);
            int width = Screen.width / 100 * 58;
            int height = Screen.height / 100 * 90;
            Rect rect = new Rect(Screen.width / 2f - width / 2f, Screen.height / 2f - height / 2f, width, height);
            if (_module != null && _module.Visible)
            {
                GUI.DrawTexture(rect = new Rect(Screen.width / 2f - 640, Screen.height / 2f - 360, 1280, 720), _moduleBackground);
                GUI.Label(new Rect(rect.x + 30, rect.y + 10, rect.width - 60, 30), _module.Name, _moduleName);
                GUI.Label(new Rect(rect.x + 30, rect.y + 30, rect.width - 60, 30), _module.Description, _moduleDescription);
                _module.Render(new Rect(rect.x + 30, rect.y + 80, rect.width - 60, rect.height - 140));
                GUI.DrawTexture(new Rect(rect.x + 30, rect.yMax - 40, rect.width / 2f - 10 - 30, 30), _background);
                if (GUI.Button(new Rect(rect.x + 30, rect.yMax - 40, rect.width / 2f - 10 - 30, 30), "Close", _moduleButton))
                    _module.Close();
                GUI.DrawTexture(new Rect(rect.x + rect.width / 2f + 10, rect.yMax - 40, rect.width / 2f - 10 - 30, 30), _module.Enabled ? _enabledTexture : _disabledTexture);
                if (GUI.Button(new Rect(rect.x + rect.width / 2f + 10, rect.yMax - 40, rect.width / 2f - 10 - 30, 30), _module.Enabled ? "Enabled" : "Disabled", _moduleButton))
                    _module.Toggle();
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
                
#if !ABUSIVE
                if (module.IsAbusive)
                    continue;
#endif

                if (rect.Y >= box.height)
                    continue;
                
                GUI.DrawTexture(rect, module.Enabled ? _enabledTexture : _disabledTexture);
                GUI.Label(new Rect(rect.X, rect.Y, rect.Width - 30, rect.Height), module.Name, _buttonText);
                if (module.HasGUI)
                {
                    GUI.DrawTexture(new Rect(rect.X + rect.Width - 30, rect.Y + 5, 1, rect.Height - 10), _verticalLine);
                    GUI.Label(new Rect(rect.X + rect.Width - 25 - 10, rect.Y, 25, rect.Height), "▼", _buttonMenu);
                    if (GUI.Button(new Rect(rect.X + rect.Width - 30, rect.Y, 30, rect.Height), string.Empty, GUIStyle.none))
                    {
                        module.Open();
                        _module = module;
                    }
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
            {
                Toggle();
                if (!Visible && _module.Visible)
                    _module.Close();
            }
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