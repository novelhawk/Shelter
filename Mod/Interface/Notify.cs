using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Notify : Gui
    {
        private Notification current;
        private static readonly Queue<Notification> Notifications = new Queue<Notification>();
        private GUIStyle title;
        private GUIStyle message;
        private Texture2D black;
        private Texture2D white;

        public static void New(string message, long duration, float height = 30F)
        {
            New(message, string.Empty, duration, height);
        }

        public static void New(string title, string message, long duration, float height = 100)
        {
            Notifications.Enqueue(new Notification(title, message, duration, height));
            Shelter.InterfaceManager.Enable(typeof(Notify));
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

        protected override void Render()
        {
            const float FinalWidth = 300;
            if (current != null && !current.Done && windowRect.width < FinalWidth)
                windowRect.width = Mathf.Clamp(windowRect.width + 50 * Time.deltaTime + current.ElapsedTime / 500f, 0, FinalWidth);
            else if (current != null && current.Done && windowRect.width > 0)
                windowRect.width = Mathf.Clamp(windowRect.width - 50 * Time.deltaTime - (current.ElapsedTime - current.Duration) / 1000f, 0, FinalWidth);
            else if ((current == null || current.Done) && Notifications.Count > 0)
            {
                current = Notifications.Dequeue();
                current.Start();
                windowRect.width = 0f;
            }
            else if (current == null || current.Done)
            {
                Disable();
                return;
            }

            windowRect = new Rect(Screen.width - windowRect.width, 50, windowRect.width, current.Height);
            GUI.DrawTexture(new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height), black);
            GUI.DrawTexture(new Rect(windowRect.x - 2, windowRect.y, 2, windowRect.height), white);
            GUI.Label(new Rect(windowRect.x, windowRect.y, FinalWidth, 30), current.Title, title);
            GUI.Label(new Rect(windowRect.x, windowRect.y + 30, FinalWidth, windowRect.height - 30), current.Message, message);
        }

        protected override void OnHide()
        {
            windowRect.width = 0f;
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
