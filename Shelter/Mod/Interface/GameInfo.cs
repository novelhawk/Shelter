using System.Text;
using Photon;
using Photon.Enums;
using UnityEngine;

namespace Mod.Interface
{
    public class GameInfo : Gui
    {
        private GUIStyle _text;
        private float _maxSpeed;
        private float _fps;

        protected override void OnShow()
        {
            _text = new GUIStyle
            {
                alignment = TextAnchor.UpperRight,
                fontSize = 12
            };
        }

        protected override void Render()
        {
            if (!PhotonNetwork.inRoom)
                return;
            
            StringBuilder output = new StringBuilder();

            output.Append("<color=#03FF5A>");
            
            output.Append(IN_GAME_MAIN_CAMERA.GameType);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                output.AppendFormat(" Ping: {0}", PhotonNetwork.GetPing());
            output.Append("\n");

            output.AppendFormat("Map: {0}\n", FengGameManagerMKII.Level);
            
            switch (IN_GAME_MAIN_CAMERA.GameType)
            {
                case GameType.Singleplayer:
                    var speed = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity.magnitude; //TODO: Show in new gui
                    _maxSpeed = Mathf.Max(_maxSpeed, speed);
                    output.AppendFormat("Speed: Current {0} Max {1}\n", speed, _maxSpeed);
                    break;
                case GameType.Multiplayer:
                    var room = PhotonNetwork.Room;
                    output.AppendFormat("{0} ({1}/{2})\n", room.Name.HexColor(), Room.CurrentPlayers, room.MaxPlayers);
                    break;
            }
            
            output.AppendFormat("Titans: {0}\n", GameObject.FindGameObjectsWithTag("titan")?.Length ?? 0);
            
            output.AppendFormat("FPS: {0:0}", _fps);
            
            output.Append("</color>");
            
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), output.ToString(), _text); // TODO: Change rect to `windowRect` and update it in a OnResize event
        }

        private float _lastUpdate;
        private void Update()
        {
            if (Time.time - _lastUpdate < .4f)
                return;
            
            _fps = (int) (1f / Time.unscaledDeltaTime);
            _lastUpdate = Time.time;
        }
    }
}
