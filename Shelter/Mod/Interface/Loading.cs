using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Loading : Gui
    {
        private Texture2D _texture;
        private float _rotation;

        private void Update()
        {
            _rotation += 250 * Time.deltaTime;
        }
        
        protected override void OnShow()
        {
            _texture = GetImage("Loading");
        }

        protected override void Render()
        {
            Rect rect = new Rect(Screen.width - 79, Screen.height - 79, 69, 69);

            GUIUtility.RotateAroundPivot(_rotation, new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
            GUI.DrawTexture(rect, _texture);
        }

        protected override void OnHide()
        {
            Destroy(_texture);
        }

        public static void Show()
        {
            Shelter.InterfaceManager.Enable(nameof(Loading));
        }

        public static void Stop()
        {
            Shelter.InterfaceManager.Disable(nameof(Loading));
        }
    }
}
