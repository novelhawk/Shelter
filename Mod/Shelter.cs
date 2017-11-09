using Mod.Managers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Mod
{
    public class Shelter : MonoBehaviour
    {
        public static Assembly Assembly => Assembly.GetExecutingAssembly();
        public static Stopwatch Stopwatch = Stopwatch.StartNew();
        private static InterfaceManager _interfaceManager;

        public void Start()
        {
            InitComponents();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) && Input.GetKey(KeyCode.LeftControl))
                File.WriteAllLines($"GameObjects{UnityEngine.Random.Range(0, 0xFF)}.txt", FindObjectsOfType(typeof(GameObject)).OrderBy(x => x.GetInstanceID()).Select(x => $"{x.GetInstanceID()} - {x.name}").ToArray());
        }

        private void InitComponents()
        {
            _interfaceManager = new InterfaceManager();
        }
        
        #region Static methods

        public static Texture2D CreateTexture(int r, int g, int b, int a)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(1, 1, new Color(r/255f, g/255f, b/255f, a/255f));
            texture.Apply();
            return texture;
        }

        public static void Log(params object[] messages)
        {
            foreach (var obj in messages)
                Log(obj);
        }

        public static void Log(object msg)
        {

        }

        public static Texture2D GetImage(string image)
        {
            if (Assembly.GetManifestResourceInfo($@"Mod.Resources.{image}.png") == null) return null;
            using (var stream = Assembly.GetManifestResourceStream($@"Mod.Resources.{image}.png"))
            {
                var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                texture.LoadImage(stream.ToBytes());
                texture.Apply();
                return texture;
            }
        }

        #endregion

        public static InterfaceManager InterfaceManager => _interfaceManager;
    }
}
