using UnityEngine;

namespace Mod
{
    public abstract class Gui : MonoBehaviour
    {
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

        public void Switch(string gui)
        {
            Disable();
            Shelter.InterfaceManager.Enable(gui);
        }

        public void OnGUI()
        {
            if (Visible)
                Render();
        }

        public static Color Color(int r, int g, int b, int a = 255)
        {
            return new Color(r/255f, g/255f, b/255f, a/255f);
        }

        public abstract void OnShow();
        public abstract void Render();
        public abstract void OnHide();

        public Texture2D GetImage(string image) => Shelter.ImageManager.GetImage(image);
        public void Enable(string gui) => Shelter.InterfaceManager.Enable(gui);
        public void Disable(string gui) => Shelter.InterfaceManager.Disable(gui);
        public bool IsVisible(string gui) => Shelter.InterfaceManager.IsVisible(gui);

    }
}
