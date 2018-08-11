using System;
using Mod.Interface;
using Mod.Exceptions;

namespace Mod.Commands
{
    public class CommandBan : Command
    {
        public override string CommandName => "ban";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException();
            if (args[0].EqualsIgnoreCase("list"))
            {
                if (FengGameManagerMKII.banHash.Count > 0)
                {
                    Chat.System("Banned players: ");
                    foreach (var entry in FengGameManagerMKII.banHash)
                        Chat.System($"[{entry.Key}] {entry.Value}");
                }
                else
                {
                    Chat.System("The banlist is empty");
                }
            }
            else
            {
                if (!Player.TryParse(args[0], out Player player))
                    throw new PlayerNotFoundException(args[0]);
                FengGameManagerMKII.instance.KickPlayerRC(player, true, string.Empty);
                Chat.System($"{player} has been banned");
            }
        }
    }
}
