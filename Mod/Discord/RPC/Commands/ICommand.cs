namespace Mod.Discord.RPC.Commands
{
    public interface ICommand
    {
        IPayload PreparePayload(long nonce);
    }
}