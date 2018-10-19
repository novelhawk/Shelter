using Mod.Events.EventArgs;

namespace Mod.Events
{
    public class ChatLinkListener : EventListener
    {
        [Event(nameof(FengGameManagerMKII.Chat))]
        private void OnChat(object sender, ChatEventArgs e)
        {
            
        }
    }
}