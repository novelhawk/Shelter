using UnityEngine;

namespace Mod.Interface
{
    public class Background : Gui
    {
        private Texture2D background;

        protected override void OnShow()
        {
            background = GetImage("Background");
        }

        protected override void Render()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);
        }

        protected override void OnHide()
        {
            Destroy(background);
        }
    }
}
