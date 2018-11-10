using System.Text.RegularExpressions;
using Mod.Exceptions;
using Mod.Interface;
using Photon;

namespace Mod.Commands
{
    public class CommandRoom : Command
    {
        public override string CommandName => "room";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/room [max/time/visible/roomttl/playerttl]");

            int num;
            var room = PhotonNetwork.Room;
            if (room == null)
            {
                Shelter.Chat.System("You can call this command only in a multiplayer lobby.");
                return;
            }
                
            switch (args[0].ToLower())
            {
                case "players":
                case "max":
                    if (args.Length < 2 || !int.TryParse(args[1], out num))
                        throw new CommandArgumentException(CommandName, "/room max [number]");
                    
                    room.MaxPlayers = num;
                    Shelter.Chat.System("Max player changed to " + num + ".");
                    break;

                case "add":
                case "time":
                    Match match = Regex.Match(args[1], @"([0-9]+)(\w*)");
                    if (!match.Success)
                        throw new CommandArgumentException(CommandName, "/room time [number][s/m/h]");
                    
                    num = match.Groups[1].Value.ToInt();

                    switch (match.Groups[1].Value.ToLower())
                    {
                        default:
                            GameManager.instance.AddTime(num * 60);
                            Shelter.Chat.System("You added {0} minutes to the clock.", num);
                            break;
                        
                        case "s":
                        case "second":
                        case "seconds":
                            GameManager.instance.AddTime(num);
                            Shelter.Chat.System("You added {0} seconds to the clock.", num);
                            break;

                        case "h":
                        case "hour":
                        case "hours":
                            GameManager.instance.AddTime(num * 3600);
                            Shelter.Chat.System("You added {0} hours to the clock.", num);
                            break;
                    }

                    break;
                
                case "open":
                case "close":
                case "closed":
                    room.IsOpen = !room.IsOpen;
                    Shelter.Chat.System("The room is now {0}.", room.IsOpen ? "open" : "closed");
                    break;

                case "hide":
                case "hidden":
                case "visible":
                    room.IsVisible = !room.IsVisible;
                    Shelter.Chat.System("The room is now {0}.", room.IsVisible ? "visible" : "invisible");
                    break;
                
                case "ttl":
                case "roomttl":
                    if (Player.Self.ID != 1)
                    {
                        Shelter.Chat.System("You need to be the <b>owner</b> of the room to change the TTL");
                        return;
                    }
                    
                    if (args.Length < 2 || !int.TryParse(args[1], out num))
                        throw new CommandArgumentException(CommandName, "/room ttl [number in s]");

                    room.RoomTTL = num;
                    Shelter.Chat.System("Room will decay {0} seconds after all player left.", num);
                    break;
                
                case "playerttl":
                    if (Player.Self.ID != 1)
                    {
                        Shelter.Chat.System("You need to be the <b>owner</b> of the room to change the TTL");
                        return;
                    }
                    
                    if (args.Length < 2 || !int.TryParse(args[1], out num))
                        throw new CommandArgumentException(CommandName, "/room playerttl [number in s]");
                    
                    room.PlayerTTL = num;
                    Shelter.Chat.System("Player instance will decay {0} seconds after the player left.", num);
                    break;
            }
        }
    }
}
