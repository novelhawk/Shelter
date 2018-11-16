using System.Collections.Generic;
using UnityEngine;

namespace Mod.Interface
{
    public class Notify : Gui
    {
        private static readonly Queue<Notification> Notifications = new Queue<Notification>();
        private const float Width = 300;
        
        private Notification _current;
        private float _width;
        
        private GUIStyle _title;
        private GUIStyle _message;
        private Texture2D _black;
        private Texture2D _white;

        protected override void OnShow()
        {
            _white = Texture(255, 255, 255, 140);
            _black = Texture(137, 137, 137, 140);
            
            _message = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 15,
                normal = { textColor = UnityEngine.Color.white },
            };
            _title = new GUIStyle(_message)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
            
            _width = 0f;
        }

        private void Update()
        {
            
        }

        protected override void Render()
        {
            if (!_current.Done && _width < Width) // != null
                _width = Mathf.Clamp(_width + 50 * Time.deltaTime + _current.ElapsedTime / 500f, 0, Width);
            else if (_current.Active && _width > 0) // != null
                _width = Mathf.Clamp(_width - 50 * Time.deltaTime - (_current.ElapsedTime - _current.Duration) / 1000f, 0, Width);
            else if (_current.Done) // == null
            {
                if (Notifications.Count > 0)
                {
                    _current = Notifications.Dequeue();
                    _current.Start();
                    _width = 0f;
                }
                else
                {
                    Disable();
                    return;
                }
            }
            
            Rect rect = new Rect(Screen.width - _width, 50, _width, _current.Height);
            
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), _black);
            GUI.DrawTexture(new Rect(rect.x - 2, rect.y, 2, rect.height), _white);
            
            GUI.Label(new Rect(rect.x, rect.y, Width, 30), _current.Title, _title);
            GUI.Label(new Rect(rect.x, rect.y + 30, Width, rect.height - 30), _current.Message, _message);
        }

        protected override void OnHide()
        {
            Destroy(_white);
            Destroy(_black);
        }

        public static void New(string message, long duration, float height = 30F)
        {
            New(message, string.Empty, duration, height);
        }

        public static void New(string title, string message, long duration, float height = 100)
        {
            Notifications.Enqueue(new Notification(title, message, duration, height));
            Shelter.InterfaceManager.Enable(nameof(Notify));
        }

        private struct Notification
        {
            private readonly string _title;
            private readonly string _message;
            private readonly long _duration;
            private readonly float _height;
            
            private bool _active;
            private long _startTime;
            
            public Notification(string title, string message, long duration, float height)
            {
                _title = title;
                _message = message;
                _duration = duration;
                _height = height;

                _active = false;
                _startTime = 0;
            }

            public void Start()
            {
                _startTime = Shelter.Stopwatch.ElapsedMilliseconds;
                _active = true;
            }

            public string Title => _title;
            public string Message => _message;
            public long Duration => _duration;
            public float Height => _height;

            public bool Active => _active;
            public long ElapsedTime => Shelter.Stopwatch.ElapsedMilliseconds - _startTime;
            public bool Done => _active && Shelter.Stopwatch.ElapsedMilliseconds - _startTime > Duration;
        }
    }
}
