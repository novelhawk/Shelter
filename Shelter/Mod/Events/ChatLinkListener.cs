using Mod.Events.EventArgs;

namespace Mod.Events
{
    public class ChatLinkListener : EventListener
    {
        [Event(nameof(GameManager.Chat))]
        private void OnChat(object sender, ChatEventArgs e)
        {
            
        }
    }
}