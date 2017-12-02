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
            PhotonPlayer player;
            switch (args[0].ToLower())
            {
                case "list":
                    if (FengGameManagerMKII.ignoreList.Count > 0)
                    {
                        Chat.System("Ignored players:");
                        foreach (var id in FengGameManagerMKII.ignoreList)
                        {
                            if ((player = PhotonPlayer.Find(id)) != null)
                                Chat.System(player);
                            else
                                Chat.System(id);
                        }
                    }
                    else 
                        Chat.System("Your ignore list is empty");
                    break;

                case "new":
                case "add":
                    if (args.Length < 2)
                        throw new CommandArgumentException(CommandName, "/ignore [list/add/rem] [id]");
                    if (!FengGameManagerMKII.ignoreList.Contains(args[1].ToInt()))
                        FengGameManagerMKII.ignoreList.Add(args[1].ToInt());

                    player = PhotonPlayer.Find(args[1].ToInt());
                    Chat.System($"Hai ignorato {player?.ToString() ?? "#" + args[1]}.");
                    break;

                case "remove":
                case "rem":
                    if (args.Length < 2)
                        throw new ArgumentException("/ignore [list/add/rem] [id]");

                    player = PhotonPlayer.Find(args[1].ToInt());

                    if (FengGameManagerMKII.ignoreList.Contains(args[1].ToInt()))
                        FengGameManagerMKII.ignoreList.Remove(args[1].ToInt());
                    Chat.System($"Hai un-ignorato {player?.ToString() ?? "#" + args[1]}.");
                    break;

                default:
                    throw new ArgumentException("/ignore [list/add/remove] [id]");
            }
        }
    }
}
