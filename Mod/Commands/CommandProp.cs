using System.Linq;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandProp : Command
    {
        public override string CommandName => "prop";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/prop [id]");
            if (!PhotonPlayer.TryParse(args[0], out PhotonPlayer player))
                throw new PlayerNotFoundException();
            var list = player.CustomProperties.Keys.Where(prop => !PhotonPlayer.Self.CustomProperties.Keys.Contains(prop)).Select(prop => prop.ToString()).ToList();
            foreach (var str in list)
                if (str != "sender")
                    Chat.SendMessage(str);
        }
    }
}
