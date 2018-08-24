using System;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRevive : Command
    {
        public override string CommandName => "revive";
        public override string[] Aliases => new[] {"respawn"};

        public override void Execute(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].EqualsIgnoreCase("all"))
                {
                    foreach (Player player in PhotonNetwork.playerList)
                        FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", player);
                    Notify.New("Successfully respawned all players!", 1300, 35F);
                }
                else
                {
                    if (!Player.TryParse(args[0], out Player player))
                        throw new PlayerNotFoundException(args[0]);
                    FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", player);
                    Notify.New($"{player.HexName} respawned!", 1300, 35F);
                }
            }
            else
            {
                FengGameManagerMKII.instance.RespawnHeroInNewRound();
                Notify.New("Respawn forced!", 1300, 35F);
            }
        }
    }
}
