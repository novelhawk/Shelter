using UnityEngine;

namespace Mod
{
    public class SmartRect
    {
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public SmartRect(Rect rect)
        {
            _x = rect.x;
            _y = rect.y;
            _width = rect.width;
            _height = rect.height;
        }

        public SmartRect(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }


        public SmartRect Set(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            return this;
        }

        /// <summary>
        /// Translates the SmartRect on the X axis
        /// </summary>
        /// <param name="offset">The X translation offset</param>
        /// <returns>The instance of the SmartRect</returns>
        public SmartRect TranslateX(float offset) => Translate(offset, 0);

        /// <summary>
        /// Translates the SmartRect on the Y axis
        /// </summary>
        /// <param name="offset">The Y translation offset</param>
        /// <returns>The instance of the SmartRect</returns>
        public SmartRect TranslateY(float offset) => Translate(0, offset);

        /// <summary>
        /// Translates the SmartRect
        /// </summary>
        /// <param name="x">The X translation offset</param>
        /// <param name="y">The Y translation offset</param>
        /// <returns>The instance of the SmartRect</returns>
        public SmartRect Translate(float x, float y)
        {
            _x += x;
            _y += y;
            return this;
        }

        /// <summary>
        /// Creates a new concentric SmartRect of specified size
        /// </summary>
        /// <param name="width">The width of the new SmartRect</param>
        /// <param name="height">The height of the new SmartRect</param>
        /// <returns>A concentric SmartRect of size (<paramref name="width"/>, <paramref name="height"/>)</returns>
        public SmartRect Center(float width, float height)
        {
            return new SmartRect(_width / 2 - width / 2, _height / 2 - height / 2, width, height);
        }

        public static implicit operator Rect(SmartRect smartRect)
        {
            return new Rect(smartRect.X, smartRect.Y, smartRect.Width, smartRect.Height);
        }

        public static implicit operator SmartRect(Rect rect)
        {
            return new SmartRect(rect);
        }

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;
        public float Bottom => _height + _y;
        public float Right => _width + _x;
    }
}