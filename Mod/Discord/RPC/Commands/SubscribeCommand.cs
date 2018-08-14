using Mod.Discord.RPC.Payload;

namespace Mod.Discord.RPC.Commands
{
    class SubscribeCommand : ICommand
    {
        public ServerEvent Event { get; set; }
        public bool IsUnsubscribe { get; set; }
		
        public IPayload PreparePayload(long nonce)
        {
            return new EventPayload(nonce)
            {
                Command = IsUnsubscribe ? Payload.Command.Unsubscribe : Payload.Command.Subscribe,
                Event = Event
            };
        }
    }
}
