using Game;
using ExitGames.Client.Photon;
using Mod.Exceptions;
using Photon;

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
                Shelter.Chat.System("You resetted your kd.");
                return;
            }

            if (args[0].EqualsIgnoreCase("all"))
            {
#if !ABUSIVE
                if (!Player.Self.IsMasterClient)
                {
                    Chat.System("You need to be MC to reset everyone's KDs");
                    return;
                }
#endif
                foreach (Player p in PhotonNetwork.PlayerList)
                    p.SetCustomProperties(hashtable);
                Shelter.Chat.System("You resetted the kds of everyone.");
                return;
            }

            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);

            player.SetCustomProperties(hashtable);
            Shelter.Chat.System("You reset {0}'s kda", player);
        }
    }
}
