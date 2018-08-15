namespace Mod.Interface.Components
{
    public struct ChatMessage
    {
        public string Message { get; }
        public Player Sender { get; }
        public long Time { get; }
        public bool LocalOnly => Sender == null;

        public ChatMessage(object message, Player sender = null)
        {
            Message = message.ToString();
            Sender = sender;
            Time = Shelter.Stopwatch.ElapsedMilliseconds;
        }
    }
}