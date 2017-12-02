using System.Text.RegularExpressions;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRoom : Command
    {
        public override string CommandName => "room";

        public override void Execute(string[] args)
        {
            if (args.Length < 2)
                throw new CommandArgumentException(CommandName, "/room [max/time/visible] [arg]");
            int num;
            switch (args[0].ToLower())
            {
                case "players":
                case "max":
                    num = args[1].ToInt();
                    PhotonNetwork.room.maxPlayers = num;
                    Chat.System("Max player changed to " + num + ".");
                    break;

                case "add":
                case "time":
                    Match match = Regex.Match(args[1], @"([0-9]+)(\w*)");
                    num = match.Groups[1].Value.ToInt();
                    bool minutes = match.Groups[1].Value.EqualsIgnoreCase("m");

                    if (minutes)
                        FengGameManagerMKII.instance.AddTime(num * 60);
                    else
                        FengGameManagerMKII.instance.AddTime(num);

                    Chat.System($"You added {num} {(minutes ? "minutes" : "seconds")} to the clock.");
                    break;

                case "visible":
                    PhotonNetwork.room.visible = args[1].ToBool();
                    PhotonNetwork.room.open = args[1].ToBool();

                    Chat.System($"The room is now {(!args[1].ToBool() ? "in" : "")}visible.");
                    break;
            }
        }
    }
}
