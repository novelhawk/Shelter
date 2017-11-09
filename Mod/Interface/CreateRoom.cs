using System.Linq;
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
            normal = Shelter.CreateTexture(200, 200, 200, 69);
            selectedNormal = Shelter.CreateTexture(230, 230, 230, 160);
            selectedHover = Shelter.CreateTexture(230, 230, 230, 200);
            selectedActive = Shelter.CreateTexture(230, 230, 230, 255);
            hover = Shelter.CreateTexture(200, 200, 200, 120);
            _active = Shelter.CreateTexture(200, 200, 200, 255);
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
            background = Shelter.CreateTexture(255, 255, 255, 63);
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
                if (GUI.Button(rect, string.Empty, GUIStyle.none))
                    // Make it runs once at frame (otherwise it increments by 4)
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

        #region x
        private string roomName = "Room name";
        private string roomCharacter = "LEVI";
        private string roomPassword = string.Empty;
        private readonly string[] roomMaps = LevelInfoManager.Levels.Select(x => x.Map).ToArray();
        private int roomMapIndex = 1;
        private string roomMaxPlayers = "10";
        private string roomTime = "99999";
        private int roomDifficultySingle;
        private string roomDifficulty;
        private DayLight roomDayLight = DayLight.Day;
        private bool roomOpen = true;
        private bool roomVisible = true;
        #endregion
        protected override void Render()
        {
            Rect rect;
            GUI.DrawTexture(rect = new Rect(Screen.width / 2f - width/2, Screen.height / 2f - height/2, width, height), background);
            Animation();
            if (!animDone) return;

            if (GUI.Button(new Rect(rect.x + rect.width / 2f - 150, rect.y + 50, 100, 40), "Online", singleplayer ? button : buttonSelected))
                singleplayer = false;
            if (GUI.Button(new Rect(rect.x + rect.width / 2f + 50, rect.y + 50, 100, 40), "Offline", !singleplayer ? button : buttonSelected))
                singleplayer = true;

            if (singleplayer)
                SingleplayerUI(rect);
            else
                MultiplayerUI(rect);
        }

        private void SingleplayerUI(Rect rect)
        {
            rect = new Rect(rect.x + rect.width / 100f * 10, rect.y + 120f, rect.width - rect.width / 100f * 20, rect.height - 200f);
            GUI.DrawTexture(new Rect(rect.x + rect.width / 2f - 1, rect.y, 2, 160), hover);
            Rect r;
            GUI.Label(r = new Rect(rect.x, rect.y, rect.width / 2f - 20, 30), "Character", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Map", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Time", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Difficulty", label);
            GUI.Label(new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Daylight", label);
            roomCharacter = GUI.TextField(r = new Rect(rect.x + rect.width / 2f + 20, rect.y, rect.width / 2f - 20, 30), roomCharacter, textField);
            var btn = CustomButton(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomMaps[roomMapIndex], button);
            if (btn == 1)
                roomMapIndex = roomMaps.Length - 1 == roomMapIndex ? 0 : roomMapIndex + 1;
            else if (btn == -1)
                roomMapIndex = roomMapIndex == 0 ? roomMaps.Length - 1 : roomMapIndex - 1;
            roomTime = GUI.TextField(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomTime, textField);
            r = new Rect(r.x, r.y + r.height + 3, r.width, r.height);
            if (GUI.Button(new Rect(r.x, r.y, r.width / 3 - 20, r.height), "Easy", roomDifficultySingle == 0 ? buttonSelected : button))
                roomDifficultySingle = 0;
            if (GUI.Button(new Rect(r.x + r.width / 3 + 10, r.y, r.width / 3 - 20, r.height), "Normal", roomDifficultySingle == 1 ? buttonSelected : button))
                roomDifficultySingle = 1;
            if (GUI.Button(new Rect(r.x + r.width / 3 * 2 + 20, r.y, r.width / 3 - 20, r.height), "Hard", roomDifficultySingle == 2 ? buttonSelected : button))
                roomDifficultySingle = 2;
            r = new Rect(r.x, r.y + r.height + 3, r.width, r.height);
            if (GUI.Button(new Rect(r.x, r.y, r.width / 3 - 20, r.height), "Day", roomDayLight == DayLight.Day ? buttonSelected : button))
                roomDayLight = DayLight.Day;
            if (GUI.Button(new Rect(r.x + r.width / 3 + 10, r.y, r.width / 3 - 20, r.height), "Dawn", roomDayLight == DayLight.Dawn ? buttonSelected : button))
                roomDayLight = DayLight.Dawn;
            if (GUI.Button(new Rect(r.x + r.width / 3 * 2 + 20, r.y, r.width / 3 - 20, r.height), "Night", roomDayLight == DayLight.Night ? buttonSelected : button))
                roomDayLight = DayLight.Night;
            if (GUI.Button(new Rect(rect.x + rect.width / 2f - 100f, rect.y + rect.height - 90f, 200f, 70f), "Play", button))
            {
                IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.SINGLE;
                IN_GAME_MAIN_CAMERA.singleCharacter = roomCharacter.ToUpper();
                IN_GAME_MAIN_CAMERA.difficulty = roomDifficultySingle;
                if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
                    Screen.lockCursor = true;
                Screen.showCursor = false;
                if (roomMaps[roomMapIndex] == "trainning_0")
                    IN_GAME_MAIN_CAMERA.difficulty = -1;
                FengGameManagerMKII.level = roomMaps[roomMapIndex];
                Application.LoadLevel(LevelInfoManager.GetInfo(roomMaps[roomMapIndex]).Map);
                Shelter.InterfaceManager.OnJoinedGame();
            }
        }

        private void MultiplayerUI(Rect rect)
        {
            rect = new Rect(rect.x + rect.width / 100f * 10, rect.y + 120f, rect.width - rect.width / 100f * 20, rect.height - 200f);
            GUI.DrawTexture(new Rect(rect.x + rect.width / 2f - 1, rect.y, 2, 300), hover);
            Rect r;
            GUI.Label(r = new Rect(rect.x, rect.y, rect.width / 2f - 20, 30), "Name", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Password", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Map", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Players", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Time", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Difficulty", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Daylight", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Open", label);
            GUI.Label(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), "Visible", label);
            roomName = GUI.TextField(r = new Rect(rect.x + rect.width / 2f + 20,  rect.y, rect.width / 2f - 20, 30), roomName, textField);
            roomPassword = GUI.TextField(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomPassword, textField);
            var btn = CustomButton(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomMaps[roomMapIndex], button);
            if (btn == 1)
                roomMapIndex = roomMaps.Length - 1 == roomMapIndex ? 0 : roomMapIndex + 1;
            else if (btn == -1)
                roomMapIndex = roomMapIndex == 0 ? roomMaps.Length - 1 : roomMapIndex - 1;
            roomMaxPlayers = GUI.TextField(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomMaxPlayers, textField);
            roomTime = GUI.TextField(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomTime, textField);
            r = new Rect(r.x, r.y + r.height + 3, r.width, r.height);
            if (GUI.Button(new Rect(r.x, r.y, r.width / 3 - 20, r.height), "Easy", roomDifficulty == "easy" ? buttonSelected : button))
                roomDifficulty = "easy";
            if (GUI.Button(new Rect(r.x + r.width / 3 + 10, r.y, r.width / 3 - 20, r.height), "Normal", roomDifficulty == "normal" ? buttonSelected : button))
                roomDifficulty = "normal";
            if (GUI.Button(new Rect(r.x + r.width / 3 * 2 + 20, r.y, r.width / 3 - 20, r.height), "Hard", roomDifficulty == "hard" ? buttonSelected : button))
                roomDifficulty = "hard";
            r = new Rect(r.x, r.y + r.height + 3, r.width, r.height);
            if (GUI.Button(new Rect(r.x, r.y, r.width / 3 - 20, r.height), "Day", roomDayLight == DayLight.Day ? buttonSelected : button))
                roomDayLight = DayLight.Day;
            if (GUI.Button(new Rect(r.x + r.width / 3 + 10, r.y, r.width / 3 - 20, r.height), "Dawn", roomDayLight == DayLight.Dawn ? buttonSelected : button))
                roomDayLight = DayLight.Dawn;
            if (GUI.Button(new Rect(r.x + r.width / 3 * 2 + 20, r.y, r.width / 3 - 20, r.height), "Night", roomDayLight == DayLight.Night ? buttonSelected : button))
                roomDayLight = DayLight.Night;
            if (GUI.Button(r = new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomOpen.ToString(), button))
                roomOpen = !roomOpen;
            if (GUI.Button(new Rect(r.x, r.y + r.height + 3, r.width, r.height), roomVisible.ToString(), button))
                roomVisible = !roomVisible;
            if (GUI.Button(new Rect(rect.x + rect.width / 2f - 100f, rect.y + rect.height - 90f, 200f, 70f), "Play", button))
            {
                string roomFullName = $"{roomName}`{roomMaps[roomMapIndex]}`{roomDifficulty}`{roomMaxPlayers}`{roomDayLight}`{(roomPassword != string.Empty ? aes.Encrypt(roomPassword) : roomPassword)}`{roomTime}";
                PhotonNetwork.CreateRoom(roomFullName, roomVisible, roomOpen, roomMaxPlayers.ToInt());
            }
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
