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
                throw new PlayerNotFoundException();
            
            var list = player.Properties.Where(prop => !Player.Self.Properties.Contains(prop)).ToArray();
            if (list.Length > 0)
            {
                Chat.System(player.HexName + " properties:");
                foreach (var str in list)
                    Chat.System($"{str.Key} : {str.Value}");
//                if (str != "sender") ??
                return;
            }
            
            Chat.System(player.HexName + " has no unregular properties.");
        }
    }
}
