using UnityEngine;

namespace Mod.Interface
{
    public class Compass : Gui
    {
        private Texture2D playerCircle;
        private Texture2D titanCircle;
        private Texture2D compassTexture;

        protected override void OnShow()
        {
            playerCircle = GetImage("Player");
            titanCircle = GetImage("Titan");
            compassTexture = GetImage("Compass");
        }
        protected override void Render()
        {
            //TODO: Re-enable it
//            float cameraRotation = IN_GAME_MAIN_CAMERA.instance.camera.transform.rotation.eulerAngles.y;

//            GUIUtility.RotateAroundPivot(cameraRotation, new Vector2(Screen.width - 125 - 40, 255));
//            GUI.DrawTexture(new Rect(Screen.width - 250 - 40, 130, 250, 250), compassTexture);
        }

        protected override void OnHide()
        {
            Destroy(playerCircle);
            Destroy(titanCircle);
            Destroy(compassTexture);
        }
    }
}
