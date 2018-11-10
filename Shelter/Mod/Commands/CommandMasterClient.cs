using Mod.Interface;
using Photon;

namespace Mod.Commands
{
    public class CommandMasterClient : Command
    {
        public override string CommandName => "mc";
        public override string[] Aliases => new[] {"setmc", "givemc", "stealmc"};

        public override void Execute(string[] args)
        {
            if (!Player.TryParse(args[0], out Player player))
                player = Player.Self;
            PhotonNetwork.SetMasterClient(player);
            Shelter.Chat.System("{0} is now MasterClient.", player);
        }
    }
}
