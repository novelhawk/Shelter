namespace Mod
{
    public struct ChatMessage
    {
        public string Message { get; }
        public PhotonPlayer Sender { get; }
        public long Time { get; }
        public bool LocalOnly { get; }

        public ChatMessage(object message, PhotonPlayer sender = null)
        {
            Message = message.ToString();
            Sender = sender;
            Time = Shelter.Stopwatch.ElapsedMilliseconds;
            LocalOnly = sender == null;
        }
    }
}