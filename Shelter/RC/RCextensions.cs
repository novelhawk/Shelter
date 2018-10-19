using System;
using UnityEngine;

namespace RC
{
    public static class RCextensions
    {
        public static void Add<T>(ref T[] source, T value)
        {
            T[] localArray = new T[source.Length + 1];
            for (int i = 0; i < source.Length; i++)
            {
                localArray[i] = source[i];
            }
            localArray[localArray.Length - 1] = value;
            source = localArray;
        }

        public static Texture2D LoadImageRC(WWW link, bool mipmap, int size)
        {
            Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, mipmap);
            if (link.size >= size)
            {
                return tex;
            }
            Texture2D texture = link.texture;
            int width = texture.width;
            int height = texture.height;
            int num3 = 0;
            if (width < 4 || (width & (width - 1)) != 0)
            {
                num3 = 4;
                width = Math.Min(width, 1023);
                while (num3 < width)
                {
                    num3 *= 2;
                }
            }
            else if (height < 4 || (height & (height - 1)) != 0)
            {
                num3 = 4;
                height = Math.Min(height, 1023);
                while (num3 < height)
                {
                    num3 *= 2;
                }
            }
            if (num3 == 0)
            {
                if (mipmap)
                {
                    try
                    {
                        link.LoadImageIntoTexture(tex);
                    }
                    catch
                    {
                        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
                        link.LoadImageIntoTexture(tex);
                    }
                    return tex;
                }
                link.LoadImageIntoTexture(tex);
                return tex;
            }
            if (num3 < 4)
            {
                return tex;
            }
            Texture2D textured3 = new Texture2D(4, 4, TextureFormat.DXT1, false);
            link.LoadImageIntoTexture(textured3);
            if (mipmap)
            {
                try
                {
                    textured3.Resize(num3, num3, TextureFormat.DXT1, true);
                }
                catch
                {
                    textured3.Resize(num3, num3, TextureFormat.DXT1, false);
                }
            }
            else
            {
                textured3.Resize(num3, num3, TextureFormat.DXT1, false);
            }
            textured3.Apply();
            return textured3;
        }

        public static void RemoveAt<T>(ref T[] source, int index)
        {
            if (source.Length == 1)
            {
                source = new T[0];
            }
            else if (source.Length > 1)
            {
                T[] localArray = new T[source.Length - 1];
                int num = 0;
                int num2 = 0;
                while (num < source.Length)
                {
                    if (num != index)
                    {
                        localArray[num2] = source[num];
                        num2++;
                    }
                    num++;
                }
                source = localArray;
            }
        }
    }
}

