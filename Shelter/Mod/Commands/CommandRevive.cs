using Mod.Exceptions;
using Mod.Interface;
using Photon;

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
                    foreach (Player player in PhotonNetwork.PlayerList)
                        GameManager.instance.photonView.RPC(Rpc.Respawn, player);
                    Notify.New("Successfully respawned all players!", 1300, 35F);
                }
                else
                {
                    if (!Player.TryParse(args[0], out Player player))
                        throw new PlayerNotFoundException(args[0]);
                    GameManager.instance.photonView.RPC(Rpc.Respawn, player);
                    Notify.New($"{player.Properties.HexName} respawned!", 1300, 35F);
                }
            }
            else
            {
                GameManager.instance.RespawnHeroInNewRound();
                Notify.New("Respawn forced!", 1300, 35F);
            }
        }
    }
}
