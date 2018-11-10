using System.Linq;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandProperties : Command
    {
        public override string CommandName => "prop";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/prop [id]");
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);
            
            var list = player.Properties/*.Where(prop => !Player.Self.Properties.Contains(prop))*/.ToArray();
            if (list.Length > 0)
            {
                Shelter.Chat.System(player.Properties.HexName + " properties:");
                foreach (var str in list)
                    Shelter.Chat.System("{0} : {1}", str.Key, str.Value);
//                if (str != "sender") ??
                return;
            }
            
            Shelter.Chat.System(player.Properties.HexName + " has no unregular properties.");
        }
    }
}
