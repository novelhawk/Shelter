using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Managers
{
    public class ImageManager
    {
//        private readonly Dictionary<string, string> _images = new Dictionary<string, string>(2);

        public ImageManager()
        {
//
//
//            if (Shelter.Assembly.GetManifestResourceInfo(@"Mod.Resources.Base64Images") == null) return;
//            using (var stream = new StreamReader(Shelter.Assembly.GetManifestResourceStream(@"Mod.Resources.Base64Images")))
//            {
//                string line;
//                while ((line = stream.ReadLine()) != null)
//                {
//                    var split = line.Split(':'); // TODO: use IndexOf
//                    _images.Add(split[0], split[1]);
//                }
//            }
        }

        public Texture2D GetImage(string image)
        {
            if (Shelter.Assembly.GetManifestResourceInfo($@"Mod.Resources.{image}.png") == null) return null;
            using (var stream = Shelter.Assembly.GetManifestResourceStream($@"Mod.Resources.{image}.png"))
            {
                var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                texture.LoadImage(ReadFully(stream));
                texture.Apply();
                return texture;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[8 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

//        public Texture2D GetImage(string image)
//        {
//            if (_images.ContainsKey(image))
//                return FromBase64(_images[image]);
//            return null;
//        }
//
//        private static Texture2D FromBase64(string base64String)
//        {
//            byte[] image = Convert.FromBase64String(base64String);
//            var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
//            texture.LoadImage(image);
//            texture.Apply();
//            return texture;
//        }
    }
}
