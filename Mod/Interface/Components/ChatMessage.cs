namespace Mod.Interface.Components
{
    public struct ChatMessage
    {
        public string Message { get; }
        public Player Sender { get; }
        public long Time { get; }
        public bool IsForemost { get; set; }
        public bool LocalOnly => Sender == null;

        public ChatMessage(object message, Player sender = null)
        {
            Message = Utility.CheckHTMLTags(message.ToString());
            Sender = sender;
            IsForemost = false;
            Time = Shelter.Stopwatch.ElapsedMilliseconds;
        }

        public ChatMessage(object message, Player sender, long time)
        {
            Message = Utility.CheckHTMLTags(message.ToString());
            Sender = sender;
            IsForemost = false;
            Time = time;
        }
    }
}