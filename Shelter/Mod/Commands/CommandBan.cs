using Mod.Exceptions;

namespace Mod.Commands
{
    public class CommandBan : Command
    {
        public override string CommandName => "ban";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/ban [list/id]");
            
            if (args[0].EqualsIgnoreCase("list"))
            {
                if (GameManager.banHash.Count > 0)
                {
                    Shelter.Chat.System("Banned players: ");
                    foreach (var entry in GameManager.banHash)
                        Shelter.Chat.System("[{0}] {1}", entry.Key, entry.Value);
                }
                else
                {
                    Shelter.Chat.System("The banlist is empty");
                }

                return;
            }
            
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);
            
            GameManager.instance.KickPlayerRC(player, true, string.Empty);
            Shelter.Chat.System("{0} has been banned", player);
        }
    }
}
