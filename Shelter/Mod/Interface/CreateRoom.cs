using System.Collections;
using Game;
using Photon;
using Photon.Enums;
using UnityEngine;

namespace Mod.Interface
{
    public class CreateRoom : Gui
    {
        private readonly SimpleAES _aes;
        
        private Texture2D _background;
        private Texture2D _selectedNormal;
        private Texture2D _selectedHover;
        private Texture2D _selectedActive;
        private Texture2D _normal;
        private Texture2D _hover;
        private Texture2D _active;
        
        private GUIStyle _textField;
        private GUIStyle _label;
        private GUIStyle _button;
        private GUIStyle _buttonSelected;

        private const float Width = 1280;
        private const float Height = 720;
        
        private float _width;
        private float _height;
        
        private bool _isSingleplayer;
        private bool _animationDone;

        public CreateRoom()
        {
            _aes = new SimpleAES();
        }

        protected override void OnShow()
        {
            _background = Texture(255, 255, 255, 63);
            
            _selectedNormal = Texture(230, 230, 230, 160);
            _selectedHover = Texture(230, 230, 230, 200);
            _selectedActive = Texture(230, 230, 230);
            
            _normal = Texture(200, 200, 200, 69);
            _hover = Texture(200, 200, 200, 120);
            _active = Texture(200, 200, 200);
            
            _label = new GUIStyle
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 22,
            };
            _button = new GUIStyle
            {
                normal = {background = _normal},
                active = {background = _active},
                hover = {background = _hover},
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24
            };
            _buttonSelected = new GUIStyle(_button)
            {
                normal = {background = _selectedNormal},
                hover = {background = _selectedHover},
                active = {background = _selectedActive}
            };
            _textField = new GUIStyle
            {
                normal = { textColor = UnityEngine.Color.white },
                alignment = TextAnchor.MiddleLeft,
                fontSize = 22,
            };
            _animationDone = false;
            _width = 0f;
            _height = 0f;
        }

        private void Animation()
        {
            if (_width < Width || _height < Height)
            {
                const float changeInValue = 1.5f;
                const float slowdown = 0.5f;
                
                _width += (Width * changeInValue - _width * slowdown) * Time.deltaTime;
                _height += (Height * changeInValue - _height * slowdown) * Time.deltaTime;
                
                if (_width < Width && _height < Height)
                    return;
                _animationDone = true;
                
                _width = Width;
                _height = Height;
            }
        }

        private static int CustomButton(Rect rect, string txt, GUIStyle style)
        {
            GUI.DrawTexture(rect, style.normal.background);
            GUI.Label(rect, txt, style);
            
            Vector3 pos = Input.mousePosition;
            pos.y = -(pos.y - Screen.height + 1);
            if (!rect.Contains(new Vector2(pos.x, pos.y))) 
                return 0;
            
            if (Event.current.type == EventType.MouseUp)
            {
                switch (Event.current.button)
                {
                    case 0:
                        GUI.DrawTexture(rect, style.active.background);
                        GUI.Label(rect, txt, style);
                        return 1;
                    case 1:
                        GUI.DrawTexture(rect, style.active.background);
                        GUI.Label(rect, txt, style);
                        return -1;
                }
            }
            GUI.DrawTexture(rect, style.hover.background);
            GUI.Label(rect, txt, style);
            return 0;
        }

        private string roomName = "Room name";
        private string roomCharacter = "LEVI"; //TODO: Character customization
        private string roomPassword = string.Empty;
        private int roomMapIndex = 1;
        private string roomMaxPlayers = "10";
        private string roomTime = "99999";
        private int roomDifficultySingle;
        private string roomDifficulty;
        private Daylight _roomDaylight = Daylight.Day;
        private bool roomOpen = true;
        private bool roomVisible = true;
        protected override void Render()
        {
            Rect rect;
            GUI.DrawTexture(rect = new Rect(Screen.width / 2f - _width/2, Screen.height / 2f - _height/2, _width, _height), _background);
            Animation();
            if (!_animationDone) return;

            if (GUI.Button(new Rect(rect.x + rect.width / 2f - 150, rect.y + 50, 100, 40), "Online", _isSingleplayer ? _button : _buttonSelected))
                _isSingleplayer = false;
            if (GUI.Button(new Rect(rect.x + rect.width / 2f + 50, rect.y + 50, 100, 40), "Offline", !_isSingleplayer ? _button : _buttonSelected))
                _isSingleplayer = true;

            if (_isSingleplayer)
                SingleplayerUI(new Rect(rect.x + rect.width / 100f * 10, rect.y + 120f, rect.width - rect.width / 100f * 20, rect.height - 200f));
            else
                MultiplayerUI(new Rect(rect.x + rect.width / 100f * 10, rect.y + 120f, rect.width - rect.width / 100f * 20, rect.height - 200f));
        }

        private void SingleplayerUI(Rect areaRect)
        {
            GUI.DrawTexture(new Rect(areaRect.x + areaRect.width / 2f - 1, areaRect.y, 2, 160), _hover);

            SmartRect rect = new SmartRect(areaRect.x, areaRect.y, areaRect.width / 2f - 20, 30);
            GUI.Label(rect                    , "Character", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Map", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Time", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Difficulty", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Daylight", _label);

            rect = new SmartRect(areaRect.x + areaRect.width / 2f + 20, areaRect.y, areaRect.width / 2f - 20, 30);
            roomCharacter = GUI.TextField(rect, roomCharacter, _textField);
            switch (CustomButton(rect.OY(rect.Height + 3), LevelInfoManager.Levels[roomMapIndex].Name, _button))
            {
                case 1:
                    roomMapIndex = LevelInfoManager.Levels.Length - 1 == roomMapIndex ? 0 : roomMapIndex + 1;
                    break;
                case -1:
                    roomMapIndex = roomMapIndex == 0 ? LevelInfoManager.Levels.Length - 1 : roomMapIndex - 1;
                    break;
            }
            roomTime = GUI.TextField(rect.OY(rect.Height + 3), roomTime, _textField);
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Easy", roomDifficultySingle == 0 ? _buttonSelected : _button))
                roomDifficultySingle = 0;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Normal", roomDifficultySingle == 1 ? _buttonSelected : _button))
                roomDifficultySingle = 1;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Hard", roomDifficultySingle == 2 ? _buttonSelected : _button))
                roomDifficultySingle = 2;
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Day", _roomDaylight == Daylight.Day ? _buttonSelected : _button))
                _roomDaylight = Daylight.Day;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Dawn", _roomDaylight == Daylight.Dawn ? _buttonSelected : _button))
                _roomDaylight = Daylight.Dawn;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Night", _roomDaylight == Daylight.Night ? _buttonSelected : _button))
                _roomDaylight = Daylight.Night;

            if (GUI.Button(new Rect(areaRect.x + areaRect.width / 2f - 100f, areaRect.y + areaRect.height - 90f, 200f, 70f), "Play", _button))
            {                    
                PhotonNetwork.Disconnect();
                IN_GAME_MAIN_CAMERA.GameType = GameType.Singleplayer;
                IN_GAME_MAIN_CAMERA.singleCharacter = roomCharacter.ToUpper();
                IN_GAME_MAIN_CAMERA.difficulty = roomDifficultySingle;
                if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
                    Screen.lockCursor = true;
                Screen.showCursor = false;
//                if (LevelInfoManager.Levels[roomMapIndex].Map == "trainning_0") Does not exist in LevelInfoManager TODO: Check why
//                    IN_GAME_MAIN_CAMERA.difficulty = -1;
                GameManager.Level = LevelInfoManager.Levels[roomMapIndex].Name;
                Application.LoadLevel(LevelInfoManager.Levels[roomMapIndex].LevelName);
                Shelter.OnJoinedGame();
            }
        }

        private void MultiplayerUI(Rect areaRect)
        {
            GUI.DrawTexture(new Rect(areaRect.x + areaRect.width / 2f - 1, areaRect.y, 2, 300), _hover);

            SmartRect rect = new SmartRect(areaRect.x, areaRect.y, areaRect.width / 2f - 20, 30);
            GUI.Label(rect                    , "Name", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Password", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Map", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Players", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Time", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Difficulty", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Daylight", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Open", _label);
            GUI.Label(rect.OY(rect.Height + 3), "Visible", _label);

            rect = new SmartRect(areaRect.x + areaRect.width / 2f + 20, areaRect.y, areaRect.width / 2f - 20, 30);
            roomName = GUI.TextField(rect, roomName, _textField);
            roomPassword = GUI.TextField(rect.OY(rect.Height + 3), roomPassword, _textField);
            switch (CustomButton(rect.OY(rect.Height + 3), LevelInfoManager.Levels[roomMapIndex].Name + " " + roomMapIndex, _button))
            {
                case 1:
                    roomMapIndex = LevelInfoManager.Levels.Length - 1 == roomMapIndex ? 0 : roomMapIndex + 1;
                    break;
                case -1:
                    roomMapIndex = roomMapIndex == 0 ? LevelInfoManager.Levels.Length - 1 : roomMapIndex - 1;
                    break;
            }
            roomMaxPlayers = GUI.TextField(rect = rect.OY(rect.Height + 3), roomMaxPlayers, _textField);
            roomTime = GUI.TextField(rect.OY(rect.Height + 3), roomTime, _textField);
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Easy", roomDifficulty == "easy" ? _buttonSelected : _button))
                roomDifficulty = "easy";
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Normal", roomDifficulty == "normal" ? _buttonSelected : _button))
                roomDifficulty = "normal";
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Hard", roomDifficulty == "hard" ? _buttonSelected : _button))
                roomDifficulty = "hard";
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Day", _roomDaylight == Daylight.Day ? _buttonSelected : _button))
                _roomDaylight = Daylight.Day;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Dawn", _roomDaylight == Daylight.Dawn ? _buttonSelected : _button))
                _roomDaylight = Daylight.Dawn;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Night", _roomDaylight == Daylight.Night ? _buttonSelected : _button))
                _roomDaylight = Daylight.Night;
            if (GUI.Button(rect = rect.OY(rect.Height + 3), roomOpen.ToString(), _button))
                roomOpen = !roomOpen;
            if (GUI.Button(new Rect(rect.X, rect.Y + rect.Height + 3, rect.Width, rect.Height), roomVisible.ToString(), _button))
                roomVisible = !roomVisible;
            if (GUI.Button(new Rect(areaRect.x + areaRect.width / 2f - 100f, areaRect.y + areaRect.height - 90f, 200f, 70f), !isRunning ? "Play" : "Connecting", _button) && !isRunning) //FIXME: Possible to click while joining the game.
                StartCoroutine(AwaitConnect());
        }

        private bool isRunning;
        private IEnumerator AwaitConnect()
        {
            isRunning = true;
            while (PhotonNetwork.connectionStatesDetailed != ClientState.JoinedLobby && PhotonNetwork.connectionStatesDetailed != ClientState.Joined)
                yield return null;
            string roomFullName = $"{roomName}`{LevelInfoManager.Levels[roomMapIndex].Name}`{roomDifficulty}`{roomMaxPlayers}`{_roomDaylight}`{(roomPassword != string.Empty ? _aes.Encrypt(roomPassword) : roomPassword)}`{roomTime}";
            PhotonNetwork.CreateRoom(roomFullName, roomVisible, roomOpen, roomMaxPlayers.ToInt());
            isRunning = false;
        }


        protected override void OnHide()
        {
//            aes.Dispose();
            Destroy(_background);
            Destroy(_normal);
            Destroy(_hover);
            Destroy(_active);
            Destroy(_selectedNormal);
            Destroy(_selectedActive);
            Destroy(_selectedHover);
        }
    }
}
