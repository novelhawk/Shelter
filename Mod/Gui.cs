using UnityEngine;

namespace Mod
{
    public abstract class Gui : MonoBehaviour
    {
        protected Rect windowRect;
        public string Name;
        public bool Visible;

        protected Gui()
        {
            Name = GetType().Name;
        }

        public void Enable()
        {
            OnShow();
            Visible = true;
        }

        public void Disable()
        {
            OnHide();
            Visible = false;
        }

        protected void Switch(string gui)
        {
            Disable();
            Shelter.InterfaceManager.Enable(gui);
        }

        private void OnGUI()
        {
            if (Visible)
                Render();
        }

        protected virtual void Update()
        {

        }


        protected virtual void OnShow()
        {
        }

        protected virtual void Render()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected Texture2D Texture(byte r, byte g, byte b, byte a = 255)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(1, 1, Color(r, g, b, a));
            texture.Apply();
            return texture;
        }

        protected Color Color(byte r, byte g, int b, byte a = 255) => new Color(r/255f, g/255f, b/255f, a/255f);
        protected Texture2D GetImage(string image) => Shelter.GetImage(image);
        protected void Enable(string gui) => Shelter.InterfaceManager.Enable(gui);
        protected void Disable(string gui) => Shelter.InterfaceManager.Disable(gui);
        protected bool IsVisible(string gui) => Shelter.InterfaceManager.IsVisible(gui);

    }
}
