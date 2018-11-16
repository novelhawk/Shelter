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
            SmartRect rect = new SmartRect(Screen.width * 0.65f, Screen.height, Screen.width / 2f, 12);
            foreach (string log in Shelter.ConsoleLogger.Logs)
                GUI.Label(rect.OY(-12), log, _textStyle);
        }
    }
}