using Mod.Exceptions;

namespace Mod.Commands
{
    public class CommandKick : Command
    {
        public override string CommandName => "kick";
        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/kick [id]");
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException();
            
            GameManager.instance.KickPlayerRC(player, false, string.Empty);
            Shelter.Chat.System("Il player è stato kickato");
        }
    }
}
