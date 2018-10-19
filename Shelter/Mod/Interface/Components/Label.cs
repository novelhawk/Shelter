using System;
using Mod.Events.EventArgs;
using UnityEngine;

namespace Mod.Interface.Components
{
    public class Label : IComponent
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public Rect Position { get; set; }
        
        public GUIStyle GUIStyle { get; set; }

        public int FontSize
        {
            get => GUIStyle.fontSize;
            set => GUIStyle.fontSize = value;
        }

        public Color Color
        {
            get => GUIStyle.normal.textColor;
            set => GUIStyle.normal.textColor = value;
        }

        public TextAnchor Alignment
        {
            get => GUIStyle.alignment;
            set => GUIStyle.alignment = value;
        }

        public Label()
        {
            GUIStyle = new GUIStyle
            {
                normal = {textColor = Color.white},
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
        }

        public Label(GUIStyle style)
        {
            GUIStyle = style;
        }
        
        public void Draw()
        {
            GUI.Label(Position, Text, GUIStyle);
        }

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
    }
}