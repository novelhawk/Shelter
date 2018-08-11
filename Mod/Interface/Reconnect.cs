using UnityEngine;

namespace Mod.Interface
{
    public class Reconnect : Gui
    {
        private GUIStyle _text;
//        private GUIStyle _button;
        private Texture2D _backgroundTexture;

        protected override void OnShow()
        {
            _text = new GUIStyle
            {
                normal = {textColor = UnityEngine.Color.white},
                fontSize = 20
            };
            _backgroundTexture = Texture(255, 255, 255, 63);
        }

        protected override void Render()
        {
            const float WIDTH = 500;
            const float HEIGHT = 150;
            GUI.DrawTexture(windowRect = new Rect(Screen.width / 2f - WIDTH / 2f, Screen.height / 2f - HEIGHT / 2f, WIDTH, HEIGHT), _backgroundTexture);
            GUILayout.BeginArea(windowRect);
            GUILayout.Label("asd", _text);
            GUILayout.EndArea();
//            GUI.Label(, "We are trying to reconnect you to " + LastRoom, _text);
//            GUI.Button(new Rect(windowRect.x + 100f, windowRect.y + windowRect.height - 125f, windowRect.width / 2f - 125f, 100), "re", _button);
        }

        protected override void OnHide()
        {
            Destroy(_backgroundTexture);
        }

        public static global::Room LastRoom { get; set; }
    }
}
