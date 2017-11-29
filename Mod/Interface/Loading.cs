using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Loading : Gui
    {
        private static readonly List<string> loadings = new List<string>();
        private float rotAngle;
        private Vector2 pivotPoint;
        private Texture2D texture;

        public Loading()
        {
//            Enable();
        }

        public static void Start(string id)
        {
            if (!loadings.Contains(id))
                loadings.Add(id);
            Shelter.InterfaceManager.Enable("Loading");
        }

        public static void Stop(string id)
        {
            if (loadings.Contains(id))
                loadings.Remove(id);
            if (loadings.Count <= 0)
                Shelter.InterfaceManager.Disable("Loading");
        }

        protected override void OnShow()
        {
            texture = GetImage("Loading");
        }

        protected override void Render()
        {
            Rect rect = new Rect(Screen.width - 79, Screen.height - 79, 69, 69);

            pivotPoint = new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
            GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.width), texture);
        }

        private void Update()
        {
            rotAngle += Time.deltaTime * 250;
        }

        protected override void OnHide()
        {
            Destroy(texture);
        }
    }
}
