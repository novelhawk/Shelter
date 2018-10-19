namespace Mod.Events.EventArgs
{
    public class ChatEventArgs : System.EventArgs
    {
        public bool Sender { get; set; }
        public string Message { get; set; }
        
        public bool Allowed { get; set; }
    }
}