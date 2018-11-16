using System.Net;
using Mod.Profiles;
using UnityEngine;

namespace Mod.Interface
{
    public class ProfileChanger : Gui
    {
        private Texture2D _background;
        
        private Texture2D _normal;
        private Texture2D _hover;
        private Texture2D _active;
        private Texture2D _transparent;
        
        private GUIStyle _textField;
        private GUIStyle _label;
        private GUIStyle _button;
        private GUIStyle _deleteButton;

        private const float Width = 1280;
        private const float Height = 720;
        
        private float _width;
        private float _height;
        
        private Profile _profile;
        private int _selectedProfile;

        protected override void OnShow()
        {
            _background = Texture(255, 255, 255, 63);

            _transparent = Texture(0, 0, 0, 0);
            _normal = Texture(200, 200, 200, 69);
            _hover = Texture(200, 200, 200, 120);
            _active = Texture(200, 200, 200);

            var deleteColor = Color(255, 0, 0, 125);
            
            _label = new GUIStyle
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 22,
            };
            _button = new GUIStyle
            {
                normal = {background = _normal},
                hover = {background = _hover},
                active = {background = _active},
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24
            };
            _deleteButton = new GUIStyle(_button)
            {
                normal = {textColor = deleteColor}
            };
            _textField = new GUIStyle(GUI.skin.textField)
            {
                normal = {textColor = Color(255, 255, 255), background = _transparent},
                active = {background = _transparent},
                focused = {background = _transparent},
                hover = {background = _transparent},
                border = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 22,
            };

            _selectedProfile = Shelter.ProfileManager.ProfileFile.Selected;
            _profile = Shelter.Profile;
        }
        
        private static int CustomButton(Rect rect, string txt, GUIStyle style) // Same as CreateRoom.CustomButton. Move to custom gui framework
        {
            Vector3 pos = Input.mousePosition;
            var y1 = -(Input.mousePosition.y - Screen.height + 1);
            bool x = rect.x <= pos.x && pos.x <= rect.x + rect.width;
            bool y = rect.y <= y1 && y1 <= rect.y + rect.height;
            if (x && y)
            {
                if (GUI.Button(rect, string.Empty, GUIStyle.none)) // Make sure it register only once click per frame. TODO: Find alternative
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        GUI.DrawTexture(rect, style.active.background);
                        GUI.Label(rect, txt, style);
                        return 1;
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        GUI.DrawTexture(rect, style.active.background);
                        GUI.Label(rect, txt, style);
                        return -1;
                    }
                }
                GUI.DrawTexture(rect, style.hover.background);
                GUI.Label(rect, txt, style);
                return 3;
            }
            GUI.DrawTexture(rect, style.normal.background);
            GUI.Label(rect, txt, style);
            return 0;
        }

        protected override void Render()
        {
            Rect windowRect = new Rect(Screen.width / 2f - _width / 2, Screen.height / 2f - _height / 2, _width, _height);
            GUI.DrawTexture(windowRect, _background);
            if (!Animation())
                return;

            Rect areaRect = new Rect(windowRect.x + windowRect.width / 100f * 10, windowRect.y + 120f, windowRect.width - windowRect.width / 100f * 20, windowRect.height - 200f);

            SmartRect rect = new SmartRect(areaRect.x + areaRect.width / 2f - 150, areaRect.y - 60, 275, 30);
            switch (CustomButton(rect, _profile.ProfileName, _button))
            {
                case 1:
                    _selectedProfile++;
                    if (_selectedProfile >= Shelter.Profiles.Count)
                        _selectedProfile = 0;

                    _profile = Shelter.Profiles[_selectedProfile];
                    Shelter.ProfileManager.ProfileFile.Selected = _selectedProfile;
                    break;
                
                case 2:
                    _selectedProfile--;
                    if (_selectedProfile < 0)
                        _selectedProfile = Shelter.Profiles.Count - 1;
                    
                    _profile = Shelter.Profiles[_selectedProfile];
                    Shelter.ProfileManager.ProfileFile.Selected = _selectedProfile;
                    break;
            }
            if (GUI.Button(new Rect(rect.X + rect.Width, rect.Y, 25, rect.Height), "+", _button))
            {
                _profile = Profile.DefaultProfile;
                _selectedProfile = Shelter.Profiles.Count;
                _profile.ProfileName = "Profile " + (_selectedProfile + 1);

                Shelter.Profiles.Add(_profile);
            }
            
            rect = new SmartRect(areaRect.x, areaRect.y, areaRect.width / 2f - 20, 30);
            GUI.Label(rect                    , "Profile"   , _label);
            GUI.Label(rect.OY(rect.Height + 3), "Name"      , _label);
            GUI.Label(rect.OY(rect.Height + 3), "Guild"     , _label);
            GUI.Label(rect.OY(rect.Height + 3), "FriendName", _label);
            GUI.Label(rect.OY(rect.Height + 3), "ChatName"  , _label);
            GUI.Label(rect.OY(rect.Height + 3), "ChatFormat", _label);

            rect = new SmartRect(areaRect.x + areaRect.width / 2f + 20, areaRect.y, areaRect.width / 2f - 20, 30);
            _profile.ProfileName = GUI.TextField(rect, _profile.ProfileName ?? "Profile name", _textField);
            _profile.Name = GUI.TextField(rect.OY(rect.Height + 3), _profile.Name ?? "InGame Name", _textField);
            _profile.Guild = GUI.TextField(rect.OY(rect.Height + 3), _profile.Guild ?? string.Empty, _textField);
            _profile.FriendName = GUI.TextField(rect.OY(rect.Height + 3), _profile.FriendName ?? string.Empty, _textField);
            _profile.ChatName = GUI.TextField(rect.OY(rect.Height + 3), _profile.ChatName ?? "ChatName", _textField);
            _profile.ChatFormat = GUI.TextField(rect.OY(rect.Height + 3), _profile.ChatFormat ?? "$(chatName): $(message)", _textField);

            if (Shelter.Profiles.Count > 1 && GUI.Button(new Rect(areaRect.x + areaRect.width / 2f - 150, rect.Y + 100, 300f, 50f), "Delete profile", _deleteButton))
            {
                Shelter.Profiles.RemoveAt(_selectedProfile);
                _selectedProfile = Shelter.ProfileManager.ProfileFile.Selected = Shelter.Profiles.Count - 1;
                _profile = Shelter.Profile;
            }
            
            if (GUI.Button(new Rect(areaRect.x + areaRect.width / 2f - 175, areaRect.y + areaRect.height - 100, 150f, 70f), "Apply", _button))
            {
                Shelter.Profiles[_selectedProfile] = _profile;
                Notify.New("Changes applied", 3000);
            }
            if (GUI.Button(new Rect(areaRect.x + areaRect.width / 2f + 25, areaRect.y + areaRect.height - 100, 150f, 70f), "Save", _button))
            {
                Shelter.Profiles[_selectedProfile] = _profile;
                if (Shelter.ProfileManager.Save())
                    Notify.New("Successfully saved to file", 3000);
                else
                    Notify.New("Failed to save to file", 3000);
            }
        }

        private bool Animation()
        {
            if (_width < Width || _height < Height) // Cache animation done?
            {
                const float changeInValue = 1.5f;
                const float slowdown = 0.5f;
                
                _width += (Width * changeInValue - _width * slowdown) * Time.deltaTime;
                _height += (Height * changeInValue - _height * slowdown) * Time.deltaTime;
                
                if (_width < Width && _height < Height)
                    return false;
                _width = Width;
                _height = Height;
            }
            return true;
        }

        protected override void OnHide()
        {
            _width = 0f;
            _height = 0f;
            Destroy(_background);
        }
    }
}
