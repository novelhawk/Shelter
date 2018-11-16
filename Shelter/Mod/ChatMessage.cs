using System.Net.Configuration;
using System.Text;

namespace Mod.Interface.Components
{
    public struct ChatMessage
    {
        private const long MessageTimeout = 10_000;

        private readonly int _id;
        private readonly Player _sender;
        private readonly long _time;

        public int ID => _id;
        public string Content { private get; set; }
        public bool IsVisible => Shelter.Stopwatch.ElapsedMilliseconds - _time < MessageTimeout;
        public bool IsForemost { get; set; }

        public ChatMessage(int id, Player sender, string content, bool isForemost)
        {
            _id = id;
            
            _sender = sender;
            Content = content;
            IsForemost = isForemost;
            
            _time = Shelter.Stopwatch.ElapsedMilliseconds;
        }

        public ChatMessage(int id)
        {
            _id = id;

            _sender = null;
            Content = null;
            IsForemost = false;

            _time = Shelter.Stopwatch.ElapsedMilliseconds;
        }


        public override string ToString()
        {
            var capacity = 0;
            
            if (_sender != null)
                capacity += Utility.IDLength(_sender.ID) + 3;
            
            if (Content == null)
                capacity += 12; // "Null message"
            else
                capacity = Content.Length;
            
            StringBuilder builder = new StringBuilder(capacity);
            if (_sender != null)
                builder.AppendFormat("[{0}] ", _sender.ID);
            builder.Append(Content ?? "Null message");
            return builder.ToString();
        }
    }
}