using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Notify : Gui
    {
        private static readonly Queue<Notification> Notifications = new Queue<Notification>();
        private GUIStyle title;
        private GUIStyle message;
        private Texture2D titleBG;
        private Texture2D messageBG;
        
        public static void New(string title, string message, long duration)
        {
            Notifications.Enqueue(new Notification(title, message, duration));
            Shelter.InterfaceManager.Enable("Notify");
        }

        public override void OnShow()
        {
            titleBG = Shelter.CreateTexture(100, 100, 100, 180);
            messageBG = Shelter.CreateTexture(137, 137, 137, 140);
            message = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                normal = { textColor = UnityEngine.Color.white },
            };
            title = new GUIStyle(message)
            {
                fontSize = 16
            };
        }

        private Notification current;
        public override void Render()
        {
            if ((current == null || current.Elapsed) && Notifications.Count > 0)
            {
                current = Notifications.Dequeue();
                current.Start();
            }
            else if (current == null || current.Elapsed)
            {
                Disable();
                return;
            }

            Rect rect = new Rect(Screen.width - 300 - 50, 50, 300, 100);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 30), titleBG);
            GUI.DrawTexture(new Rect(rect.x, rect.y + 30, rect.width, rect.height - 30), messageBG);
            GUI.Label(new Rect(rect.x, rect.y, rect.width, 30), current.Title, title);
            GUI.Label(new Rect(rect.x, rect.y + 30, rect.width, rect.height - 30), current.Message, message);
        }

        public override void OnHide()
        {
            Destroy(titleBG);
            Destroy(messageBG);
        }

        private class Notification
        {
            public Notification(string title, string message, long duration)
            {
                Title = title;
                Message = message;
                Duration = duration;
            }

            public void Start()
            {
                _startTime = Shelter.Stopwatch.ElapsedMilliseconds;
                _active = true;
            }

            private bool _active;
            private long _startTime;
            public string Title { get; }
            public string Message { get; }
            private long Duration { get; }
            public bool Elapsed => _active && Shelter.Stopwatch.ElapsedMilliseconds - _startTime > Duration;
        }
    }
}
