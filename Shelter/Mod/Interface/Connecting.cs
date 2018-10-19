using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mod.Interface
{
    public class Connecting : Gui
    {
        private static Room _room;
        private float _rotation;
        private GUIStyle _title;
        private Texture2D _loading;
        private Texture2D _background;

        public static void ConnectTo(Room room)
        {
            _room = room;
            Shelter.InterfaceManager.Enable(nameof(Connecting));
        }

        private void Update()
        {
            if (!Shelter.InterfaceManager.IsVisible(nameof(Connecting)))
                return;

            if (!Room.List.Contains(_room))
                _room = Room.List.FirstOrDefault(x => x.FullName == _room.FullName);
                    
            _rotation += Time.deltaTime * 250;
            if (_room != null && _room.IsJoinable)
            {
                _room.Join();
                Disable();
            }
        }

        protected override void OnShow()
        {
            _title = new GUIStyle
            {
                normal = {textColor = UnityEngine.Color.white},
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
            _loading = GetImage("Loading");
            _background = Texture(0, 0, 0, 63);
        }

        protected override void Render()
        {
            if (_room == null)
                return;
            
            windowRect = new Rect(Screen.width / 2f - 200, Screen.height - 60, 400, 60);
            GUI.DrawTexture(windowRect, _background);

            Rect rect = new Rect(windowRect.x + 65, windowRect.y + 8, windowRect.width - 65, windowRect.height - 16);
            if (_room.IsJoinable)
                GUI.Label(rect, $"Connecting to {_room.Name.HexColor()}", _title);
            else
                GUI.Label(rect, $"Awaiting empty slot {_room.Players}/{_room.MaxPlayers}\n{_room.Name.HexColor()}", _title);
            
            rect = new Rect(windowRect.x + 5, windowRect.y + 5, 50, 50);
            GUIUtility.RotateAroundPivot(_rotation, new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
            GUI.DrawTexture(rect, _loading);
        }

        protected override void OnHide()
        {
            Destroy(_loading);
            Destroy(_background);
        }
    }
}
