using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Notify : Gui
    {
        private static readonly Queue<Notification> Notifications = new Queue<Notification>();
        private GUIStyle title;
        private GUIStyle message;
        private Texture2D black;
        private Texture2D white;
        
        public static void New(string title, string message, long duration, float height = 100)
        {
            Notifications.Enqueue(new Notification(title, message, duration, height));
            Shelter.InterfaceManager.Enable("Notify");
        }

        protected override void OnShow()
        {
            black = Texture(137, 137, 137, 140);
            white = Texture(255, 255, 255, 140);
            message = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 15,
                normal = { textColor = UnityEngine.Color.white },
            };
            title = new GUIStyle(message)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
        }

        private float _width;
        private const float FinalWidth = 300;
        private Notification current;
        protected override void Render()
        {
            if (current != null && !current.Done && _width < FinalWidth)
                _width = Mathf.Clamp(_width + 50 * Time.deltaTime + current.ElapsedTime / 500f, 0, FinalWidth);
            else if (current != null && current.Done && _width > 0)
                _width = Mathf.Clamp(_width - 50 * Time.deltaTime - (current.ElapsedTime - current.Duration) / 1000f, 0, FinalWidth);
            else if ((current == null || current.Done) && Notifications.Count > 0)
            {
                current = Notifications.Dequeue();
                current.Start();
                _width = 0f;
            }
            else if (current == null || current.Done)
            {
                Disable();
                return;
            }

            Rect rect = new Rect(Screen.width - _width, 50, _width, current.Height);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), black);
            GUI.DrawTexture(new Rect(rect.x - 2, rect.y, 2, rect.height), white);
            GUI.Label(new Rect(rect.x, rect.y, FinalWidth, 30), current.Title, title);
            GUI.Label(new Rect(rect.x, rect.y + 30, FinalWidth, rect.height - 30), current.Message, message);
        }

        protected override void OnHide()
        {
            _width = 0f;
            Destroy(white);
            Destroy(black);
        }

        private class Notification
        {
            public Notification(string title, string message, long duration, float height)
            {
                Title = title;
                Message = message;
                Duration = duration;
                Height = height;
            }

            public void Start()
            {
                _startTime = Shelter.Stopwatch.ElapsedMilliseconds;
                _active = true;
            }

            private bool _active;
            private long _startTime;
            public float Height { get; }
            public string Title { get; }
            public string Message { get; }
            public long Duration { get; }
            public long ElapsedTime => Shelter.Stopwatch.ElapsedMilliseconds - _startTime;
            public bool Done => _active && Shelter.Stopwatch.ElapsedMilliseconds - _startTime > Duration;
        }
    }
}
