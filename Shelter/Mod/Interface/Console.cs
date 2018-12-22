using UnityEngine;

namespace Mod.Interface
{
    public class Console : Gui
    {
        private GUIStyle _textStyle;
        
        protected override void OnShow()
        {
            _textStyle = new GUIStyle
            {
                alignment = TextAnchor.LowerRight,
                normal = {textColor = UnityEngine.Color.white},
                fontSize = 12
            };
        }

        protected override void Render()
        {
            float y = Screen.height;
            foreach (string log in Shelter.ConsoleLogger.Logs)
                GUI.Label(new Rect(Screen.width * 0.75f, y -= 12, Screen.width * 0.25f, 12), log, _textStyle);
        }
    }
}