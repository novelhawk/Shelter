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
            
            Player player;
            switch (args[0].ToLower())
            {
                case "list":
                {
                    if (FengGameManagerMKII.ignoreList.Count > 0)
                    {
                        Chat.System("Ignored players:");
                        foreach (var id in FengGameManagerMKII.ignoreList)
                        {
                            if ((player = Player.Find(id)) != null)
                                Chat.System(player);
                            else
                                Chat.System(id);
                        }
                    }
                    else
                        Chat.System("Your ignore list is empty");

                    break;
                }

                case "new":
                case "add":
                {
                    if (args.Length < 2 || !int.TryParse(args[1], out int id))
                        throw new CommandArgumentException(CommandName, "/ignore [list/add/rem] [id]");

                    if (!Player.TryParse(id, out player))
                        throw new PlayerNotFoundException(id);
                    
                    if (!FengGameManagerMKII.ignoreList.Contains(id))
                        FengGameManagerMKII.ignoreList.Add(id);
                    
                    Chat.System($"Hai ignorato {player}.");
                    break;
                }

                case "remove":
                case "rem":
                {
                    if (args.Length < 2)
                        throw new ArgumentException("/ignore [list/add/rem] [id]");

                    if (!Player.TryParse(args[1], out player))
                        throw new PlayerNotFoundException(args[1]);

                    if (FengGameManagerMKII.ignoreList.Contains(player.ID))
                        FengGameManagerMKII.ignoreList.Remove(player.ID);
                    
                    Chat.System($"Hai un-ignorato {player}.");
                    break;
                }

                default:
                    throw new ArgumentException("/ignore [list/add/remove] [id]");
            }
        }
    }
}
