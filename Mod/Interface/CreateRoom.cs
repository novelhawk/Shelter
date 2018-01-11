using System.Collections;
using UnityEngine;

namespace Mod.Interface
{
    public class CreateRoom : Gui
    {
        private bool singleplayer;
        private bool animDone;
        private readonly SimpleAES aes;
        private GUIStyle textField;
        private GUIStyle label;
        private Texture2D normal;
        private Texture2D selectedNormal;
        private Texture2D selectedHover;
        private Texture2D selectedActive;
        private Texture2D _active;
        private Texture2D hover;
        private GUIStyle button;
        private GUIStyle buttonSelected;
        private Texture2D background;
        private float width;
        private float height;

        public CreateRoom()
        {
            aes = new SimpleAES();
        }

        protected override void OnShow()
        {
            normal = Texture(200, 200, 200, 69);
            selectedNormal = Texture(230, 230, 230, 160);
            selectedHover = Texture(230, 230, 230, 200);
            selectedActive = Texture(230, 230, 230, 255);
            hover = Texture(200, 200, 200, 120);
            _active = Texture(200, 200, 200, 255);
            label = new GUIStyle
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 22,
            };
            button = new GUIStyle
            {
                normal = {background = normal},
                active = {background = _active},
                hover = {background = hover},
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24
            };
            buttonSelected = new GUIStyle(button)
            {
                normal = {background = selectedNormal},
                hover = {background = selectedHover},
                active = {background = selectedActive}
            };
            textField = new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 22,
            };
            animDone = false;
            background = Texture(255, 255, 255, 63);
            width = 0f;
            height = 0f;
        }

        private void Animation()
        {
            if (width < 1280f || height < 720f)
            {
                if (width < 1280f)
                    width += 1280f / 100f * 0.1f + width / 100 * .5f * Time.deltaTime * 500;
                if (height < 720f)
                    height += 720f / 100f * 0.1f + height / 100 * .5f * Time.deltaTime * 500;
                if (width < 1280f || height < 720f)
                    return;
                width = 1280f;
                height = 720f;
                animDone = true;
            }
        }

        private static int CustomButton(Rect rect, string txt, GUIStyle style)
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
                return 0;
            }
            GUI.DrawTexture(rect, style.normal.background);
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
        private DayLight roomDayLight = DayLight.Day;
        private bool roomOpen = true;
        private bool roomVisible = true;
        protected override void Render()
        {
            GUI.DrawTexture(windowRect = new Rect(Screen.width / 2f - width/2, Screen.height / 2f - height/2, width, height), background);
            Animation();
            if (!animDone) return;

            if (GUI.Button(new Rect(windowRect.x + windowRect.width / 2f - 150, windowRect.y + 50, 100, 40), "Online", singleplayer ? button : buttonSelected))
                singleplayer = false;
            if (GUI.Button(new Rect(windowRect.x + windowRect.width / 2f + 50, windowRect.y + 50, 100, 40), "Offline", !singleplayer ? button : buttonSelected))
                singleplayer = true;

            if (singleplayer)
                SingleplayerUI(new Rect(windowRect.x + windowRect.width / 100f * 10, windowRect.y + 120f, windowRect.width - windowRect.width / 100f * 20, windowRect.height - 200f));
            else
                MultiplayerUI(new Rect(windowRect.x + windowRect.width / 100f * 10, windowRect.y + 120f, windowRect.width - windowRect.width / 100f * 20, windowRect.height - 200f));
        }

        private void SingleplayerUI(Rect areaRect)
        {
            GUI.DrawTexture(new Rect(areaRect.x + areaRect.width / 2f - 1, areaRect.y, 2, 160), hover);

            SmartRect rect = new SmartRect(areaRect.x, areaRect.y, areaRect.width / 2f - 20, 30);
            GUI.Label(rect                    , "Character", label);
            GUI.Label(rect.OY(rect.Height + 3), "Map", label);
            GUI.Label(rect.OY(rect.Height + 3), "Time", label);
            GUI.Label(rect.OY(rect.Height + 3), "Difficulty", label);
            GUI.Label(rect.OY(rect.Height + 3), "Daylight", label);

            rect = new SmartRect(areaRect.x + areaRect.width / 2f + 20, areaRect.y, areaRect.width / 2f - 20, 30);
            roomCharacter = GUI.TextField(rect, roomCharacter, textField);
            switch (CustomButton(rect.OY(rect.Height + 3), LevelInfoManager.Levels[roomMapIndex].Name, button))
            {
                case 1:
                    roomMapIndex = LevelInfoManager.Levels.Count - 1 == roomMapIndex ? 0 : roomMapIndex + 1;
                    break;
                case -1:
                    roomMapIndex = roomMapIndex == 0 ? LevelInfoManager.Levels.Count - 1 : roomMapIndex - 1;
                    break;
            }
            roomTime = GUI.TextField(rect.OY(rect.Height + 3), roomTime, textField);
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Easy", roomDifficultySingle == 0 ? buttonSelected : button))
                roomDifficultySingle = 0;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Normal", roomDifficultySingle == 1 ? buttonSelected : button))
                roomDifficultySingle = 1;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Hard", roomDifficultySingle == 2 ? buttonSelected : button))
                roomDifficultySingle = 2;
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Day", roomDayLight == DayLight.Day ? buttonSelected : button))
                roomDayLight = DayLight.Day;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Dawn", roomDayLight == DayLight.Dawn ? buttonSelected : button))
                roomDayLight = DayLight.Dawn;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Night", roomDayLight == DayLight.Night ? buttonSelected : button))
                roomDayLight = DayLight.Night;

            if (GUI.Button(new Rect(areaRect.x + areaRect.width / 2f - 100f, areaRect.y + areaRect.height - 90f, 200f, 70f), "Play", button))
            {
                IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.SINGLE;
                IN_GAME_MAIN_CAMERA.singleCharacter = roomCharacter.ToUpper();
                IN_GAME_MAIN_CAMERA.difficulty = roomDifficultySingle;
                if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
                    Screen.lockCursor = true;
                Screen.showCursor = false;
//                if (LevelInfoManager.Levels[roomMapIndex].Map == "trainning_0") Does not exist in LevelInfoManager TODO: Check why
//                    IN_GAME_MAIN_CAMERA.difficulty = -1;
                FengGameManagerMKII.level = LevelInfoManager.Levels[roomMapIndex].Name;
                Application.LoadLevel(LevelInfoManager.Levels[roomMapIndex].Name);
                Shelter.OnJoinedGame();
            }
        }

        private void MultiplayerUI(Rect areaRect)
        {
            GUI.DrawTexture(new Rect(areaRect.x + areaRect.width / 2f - 1, areaRect.y, 2, 300), hover);

            SmartRect rect = new SmartRect(areaRect.x, areaRect.y, areaRect.width / 2f - 20, 30);
            GUI.Label(rect                    , "Name", label);
            GUI.Label(rect.OY(rect.Height + 3), "Password", label);
            GUI.Label(rect.OY(rect.Height + 3), "Map", label);
            GUI.Label(rect.OY(rect.Height + 3), "Players", label);
            GUI.Label(rect.OY(rect.Height + 3), "Time", label);
            GUI.Label(rect.OY(rect.Height + 3), "Difficulty", label);
            GUI.Label(rect.OY(rect.Height + 3), "Daylight", label);
            GUI.Label(rect.OY(rect.Height + 3), "Open", label);
            GUI.Label(rect.OY(rect.Height + 3), "Visible", label);

            rect = new SmartRect(areaRect.x + areaRect.width / 2f + 20, areaRect.y, areaRect.width / 2f - 20, 30);
            roomName = GUI.TextField(rect, roomName, textField);
            roomPassword = GUI.TextField(rect.OY(rect.Height + 3), roomPassword, textField);
            switch (CustomButton(rect.OY(rect.Height + 3), LevelInfoManager.Levels[roomMapIndex].Name, button))
            {
                case 1:
                    roomMapIndex = LevelInfoManager.Levels.Count - 1 == roomMapIndex ? 0 : roomMapIndex + 1;
                    break;
                case -1:
                    roomMapIndex = roomMapIndex == 0 ? LevelInfoManager.Levels.Count - 1 : roomMapIndex - 1;
                    break;
            }
            roomMaxPlayers = GUI.TextField(rect = rect.OY(rect.Height + 3), roomMaxPlayers, textField);
            roomTime = GUI.TextField(rect.OY(rect.Height + 3), roomTime, textField);
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Easy", roomDifficulty == "easy" ? buttonSelected : button))
                roomDifficulty = "easy";
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Normal", roomDifficulty == "normal" ? buttonSelected : button))
                roomDifficulty = "normal";
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Hard", roomDifficulty == "hard" ? buttonSelected : button))
                roomDifficulty = "hard";
            rect.OY(rect.Height + 3);
            if (GUI.Button(new Rect(rect.X, rect.Y, rect.Width / 3 - 20, rect.Height), "Day", roomDayLight == DayLight.Day ? buttonSelected : button))
                roomDayLight = DayLight.Day;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 + 10, rect.Y, rect.Width / 3 - 20, rect.Height), "Dawn", roomDayLight == DayLight.Dawn ? buttonSelected : button))
                roomDayLight = DayLight.Dawn;
            if (GUI.Button(new Rect(rect.X + rect.Width / 3 * 2 + 20, rect.Y, rect.Width / 3 - 20, rect.Height), "Night", roomDayLight == DayLight.Night ? buttonSelected : button))
                roomDayLight = DayLight.Night;
            if (GUI.Button(rect = rect.OY(rect.Height + 3), roomOpen.ToString(), button))
                roomOpen = !roomOpen;
            if (GUI.Button(new Rect(rect.X, rect.Y + rect.Height + 3, rect.Width, rect.Height), roomVisible.ToString(), button))
                roomVisible = !roomVisible;
            if (GUI.Button(new Rect(areaRect.x + areaRect.width / 2f - 100f, areaRect.y + areaRect.height - 90f, 200f, 70f), !isRunning ? "Play" : "Connecting", button) && !isRunning) //FIXME: Possible to click while joining the game.
                StartCoroutine(AwaitConnect());
        }

        private bool isRunning;
        private IEnumerator AwaitConnect()
        {
            isRunning = true;
            while (PhotonNetwork.connectionStatesDetailed != PeerStates.JoinedLobby && PhotonNetwork.connectionStatesDetailed != PeerStates.Joined)
                yield return null;
            string roomFullName = $"{roomName}`{LevelInfoManager.Levels[roomMapIndex].Name}`{roomDifficulty}`{roomMaxPlayers}`{roomDayLight}`{(roomPassword != string.Empty ? aes.Encrypt(roomPassword) : roomPassword)}`{roomTime}";
            PhotonNetwork.CreateRoom(roomFullName, roomVisible, roomOpen, roomMaxPlayers.ToInt());
            isRunning = false;
        }


        protected override void OnHide()
        {
//            aes.Dispose();
            Destroy(background);
            Destroy(normal);
            Destroy(hover);
            Destroy(_active);
            Destroy(selectedNormal);
            Destroy(selectedActive);
            Destroy(selectedHover);
        }
    }
}
