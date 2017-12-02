using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandPause : Command
    {
        public override string CommandName => "pause";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/pause [true/false]");
            FengGameManagerMKII.instance.photonView.RPC("pauseRPC", PhotonTargets.All, args[0].ToBool());
            Chat.System($"You {(args[0].ToBool() ? "un" : string.Empty)}paused the game.");
        }
    }
}
