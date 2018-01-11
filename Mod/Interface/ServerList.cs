using UnityEngine;

namespace Mod.Interface
{
    public class ServerList : Gui
    {
        private GUIStyle filterStyle;
        private GUIStyle searchStyle;
        private GUIStyle buttonStyle;
        private Texture2D buttonHover;
        private Texture2D buttonActive;
        private Texture2D background;
        private Vector2 _scrollPosition;
        private string _filter = string.Empty;
        private float width;
        private float height;
        public static float Alpha;

        protected override void OnShow()
        {
            if (PhotonNetwork.connectionStatesDetailed != PeerStates.JoinedLobby)
                Loading.Start("ConnectingToLobby");
            buttonHover = Texture(255, 255, 255, 70);
            buttonActive = Texture(255, 255, 255, 140);
            filterStyle = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 22,
                normal = { textColor = Color(255, 255, 255), background = null },
                active = { background = buttonActive },
                hover = { background = buttonHover, textColor = Color(250, 250, 250) },
                border = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.UpperCenter
            };
            searchStyle = new GUIStyle
            {
                fontSize = 22,
                normal = { textColor = Color(110, 110, 110) },
                alignment = TextAnchor.MiddleCenter
            };
            buttonStyle = new GUIStyle(GUIStyle.none)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                active = {background = buttonActive},
                hover = {background = buttonHover}
            };
            background = Texture(255, 255, 255, 63);
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
            }
            else if (PhotonNetwork.connectionStatesDetailed == PeerStates.JoinedLobby)
            {
                Alpha += Time.deltaTime * 100;
                if (Alpha < 255) return;
                Alpha = Mathf.Clamp(Alpha, 0, 255);
            }
        }
        protected override void Render()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F6)
                GUI.FocusControl("Search");

            GUI.DrawTexture(windowRect = new Rect(Screen.width / 2f - width/2, Screen.height / 2f - height/2, width, height), background);
            if (Alpha < 255f)
                Animation();

            if (GUI.GetNameOfFocusedControl() != "Search" && string.IsNullOrEmpty(_filter))
                GUI.Label(new Rect(windowRect.x, windowRect.y, windowRect.width, 33), "Search...", searchStyle);
            GUILayout.BeginArea(windowRect);
            GUI.SetNextControlName("Search");
            _filter = GUILayout.TextField(_filter.Replace("\n", ""), filterStyle);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false, GUIStyle.none, GUIStyle.none);
            foreach (Room room in Room.GetOrdinatedList(Room.List))
            {
                if (_filter != string.Empty && !room.RoomName.RemoveColors().ContainsIgnoreCase(_filter) && !room.Map.ContainsIgnoreCase(_filter))
                    continue;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(room.ToString(Alpha.ToInt()), buttonStyle) && room.IsJoinable)
                    room.Join();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        protected override void OnHide()
        {
            Alpha = 0f;
            width = 0f;
            height = 0f;
            Destroy(background);
            Destroy(buttonActive);
            Destroy(buttonHover);
        }
    }
}
