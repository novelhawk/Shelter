using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mod
{
    public class SmartRect
    {
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public SmartRect()
        {
        }

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

        public SmartRect Offset(float offsetX, float offsetY, float offsetWidth, float offsetHeight)
        {
            _x += offsetX;
            _y += offsetY;
            _width += offsetWidth;
            _height += offsetHeight;
            return this;
        }

        public SmartRect Offset(float offsetWidth, float offsetHeight)
        {
            return Offset(offsetWidth / 2, offsetHeight / 2, offsetWidth, offsetHeight);
        }

        public SmartRect OX(float offset)
        {
            return Offset(offset, 0, 0, 0);
        }

        public SmartRect OY(float offset)
        {
            return Offset(0, offset, 0, 0);
        }

        public SmartRect Center(float width, float height)
        {
            return Offset(_width / 2 - width / 2, _height / 2 - height / 2, width, height); // maybe we want to create a new instance
        }

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;
        public float Bottom => _height + _y;
        public float Right => _width + _x;

        public static implicit operator Rect(SmartRect smartRect)
        {
            return new Rect(smartRect.X, smartRect.Y, smartRect.Width, smartRect.Height);
        }
    }
}