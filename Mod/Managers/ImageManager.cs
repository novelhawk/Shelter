using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod
{
    public class ImageManager
    {
        private readonly Dictionary<string, string> _images = new Dictionary<string, string>(2);

        public ImageManager()
        {
            if (Shelter.Assembly.GetManifestResourceInfo(@"Mod.Resources.Base64Images") == null) return;
            using (var stream = new StreamReader(Shelter.Assembly.GetManifestResourceStream(@"Mod.Resources.Base64Images")))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var split = line.Split(':');
                    _images.Add(split[0], split[1]);
                }
            }
        }

        public Texture2D GetImage(string image)
        {
            if (_images.ContainsKey(image))
                return FromBase64(_images[image]);
            return null;
        }

        private static Texture2D FromBase64(string base64String)
        {
            byte[] image = Convert.FromBase64String(base64String);
            var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            texture.LoadImage(image);
            texture.Apply();
            return texture;
        }
    }
}
