using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Loading : Gui
    {
        private static readonly Queue<string> loadings = new Queue<string>();
        private float rotAngle;
        private Texture2D texture;

        // ReSharper disable once Unity.InvalidStaticModifier Unity.InvalidParameters
        public static void Start(string id)
        {
            if (!loadings.Contains(id))
                loadings.Enqueue(id);
            Shelter.InterfaceManager.Enable(nameof(Loading));
        }

        public static void Stop(string id)
        {
            if (loadings.Contains(id))
                loadings.Dequeue();
            if (loadings.Count <= 0)
                Shelter.InterfaceManager.Disable(nameof(Loading));
        }

        private void Update()
        {
            rotAngle += Time.deltaTime * 250;
        }

        protected override void OnShow()
        {
            texture = GetImage("Loading");
        }

        protected override void Render()
        {
            Rect rect = new Rect(Screen.width - 79, Screen.height - 79, 69, 69);

            GUIUtility.RotateAroundPivot(rotAngle, new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
            GUI.DrawTexture(rect, texture);
        }

        protected override void OnHide()
        {
            Destroy(texture);
        }

    }
}
