using System;
using System.Linq;
using System.Text;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandIgnore : Command
    {
        public override string CommandName => "ignore";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/ignore [list/add/rem] [id]");
            
            switch (args[0].ToLower())
            {
                case "list":
                {
                    Chat.System("Ignored players:");
                    foreach (var p in PhotonNetwork.PlayerList)
                    {
                        if (p.IsIgnored)
                            Chat.System(p);
                    }
                    break;
                }

                case "new":
                case "add":
                {
                    if (args.Length < 2 || !int.TryParse(args[1], out int id))
                        throw new CommandArgumentException(CommandName, "/ignore [list/add/rem] [id]");

                    if (!Player.TryParse(id, out Player player))
                        throw new PlayerNotFoundException(id);
                    
                    if (!player.IsIgnored)
                        Chat.System($"Player {player} is now ignored.");
                    else
                        Chat.System($"Player {player} was already ignored");
                    
                    player.Ignore();
                    break;
                }

                case "remove":
                case "rem":
                {
                    if (args.Length < 2)
                        throw new ArgumentException("/ignore [list/add/rem] [id]");

                    if (!Player.TryParse(args[1], out Player player))
                        throw new PlayerNotFoundException(args[1]);

                    if (player.IsIgnored)
                        Chat.System($"Player {player} is no longer ignored.");
                    else
                        Chat.System($"Player {player} is not ignored.");

                    player.IsIgnored = false;
                    break;
                }

                default:
                    throw new ArgumentException("/ignore [list/add/remove] [id]");
            }
        }
    }
}
