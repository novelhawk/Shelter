using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRevive : Command
    {
        public override string CommandName => "revive";

        public override void Execute(string[] args)
        {
            Chat.System("Ti sei curato");
            FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", PhotonPlayer.Self); //TODO: Add /revive {id/all}
        }
    }
}
