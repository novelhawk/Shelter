using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRestart : Command
    {
        public override string CommandName => "restart";

        public override void Execute(string[] args)
        {
            GameManager.instance.RestartGameRC();
            Shelter.Chat.System("You restarted the room.");
        }
    }
}
