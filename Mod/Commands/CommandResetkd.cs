using ExitGames.Client.Photon;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandResetkd : Command
    {
        public override string CommandName => "resetkd";
        public override string[] Aliases => new[] {"resetkda"};

        public override void Execute(string[] args)
        {
            Hashtable hashtable = new Hashtable
            {
                {PlayerProperty.Kills, 0},
                {PlayerProperty.Deaths, 0},
                {PlayerProperty.MaxDamage, 0},
                {PlayerProperty.TotalDamage, 0}
            };

            if (args.Length < 1)
            {
                Player.Self.SetCustomProperties(hashtable);
                Chat.System("You resetted your kd.");
                return;
            }

            if (args[0].EqualsIgnoreCase("all"))
            {
                foreach (Player p in PhotonNetwork.playerList)
                    p.SetCustomProperties(hashtable);
                Chat.System("You resetted the kds of everyone.");
                return;
            }

            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);

            player.SetCustomProperties(hashtable);
            Chat.System($"You resetted {player}'s kd");
        }
    }
}
