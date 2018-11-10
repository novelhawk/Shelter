using System;
using Mod.Exceptions;
using Mod.Interface;
using Photon;

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
                    Shelter.Chat.System("Ignored players:");
                    foreach (var p in PhotonNetwork.PlayerList)
                    {
                        if (p.IsIgnored)
                            Shelter.Chat.System(p);
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
                        Shelter.Chat.System("Player {0} is now ignored.", player);
                    else
                        Shelter.Chat.System("Player {0} was already ignored", player);
                    
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
                        Shelter.Chat.System("Player {0} is no longer ignored.", player);
                    else
                        Shelter.Chat.System("Player {0} is not ignored.", player);

                    player.IsIgnored = false;
                    break;
                }

                default:
                    throw new ArgumentException("/ignore [list/add/remove] [id]");
            }
        }
    }
}
