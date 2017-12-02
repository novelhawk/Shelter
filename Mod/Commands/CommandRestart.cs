using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRestart : Command
    {
        public override string CommandName => "restart";

        public override void Execute(string[] args)
        {
            FengGameManagerMKII.instance.RestartGameRC();
            Chat.System("You restarted the room.");
        }
    }
}
