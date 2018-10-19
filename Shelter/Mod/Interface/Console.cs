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
            windowRect = new Rect(Screen.width / 2f, Screen.height / 2f, Screen.width / 2f, Screen.height / 2f);
            
            SmartRect rect = new SmartRect(windowRect.x, Screen.height, windowRect.width, 12);
            foreach (string log in Shelter.ConsoleLogger.Logs)
                GUI.Label(rect.OY(-12), log, _textStyle);
        }
    }
}