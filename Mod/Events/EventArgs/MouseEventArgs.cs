using UnityEngine;

namespace Mod.Events.EventArgs
{
    public class MouseDownEventArgs : System.EventArgs
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Location => new Vector2(X, Y);
        public MouseButton Button { get; set; }
    }

    public enum MouseButton
    {
        None,
        Left,
        Right,
        Middle,
        XButton1,
        XButton2
    }
}