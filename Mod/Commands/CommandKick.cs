using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandKick : Command
    {
        public override string CommandName => "kick";
        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/kick [list/id]");
            if (!PhotonPlayer.TryParse(args[0], out PhotonPlayer player))
                    throw new PlayerNotFoundException();
            FengGameManagerMKII.instance.KickPlayerRC(player, false, string.Empty);
            Chat.System("Il player è stato kickato");
        }
    }
}
