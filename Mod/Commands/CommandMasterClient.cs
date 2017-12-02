using Mod.Interface;

namespace Mod.Commands
{
    public class CommandMasterClient : Command
    {
        public override string CommandName => "mc";
        public override string[] Aliases => new[] {"setmc", "givemc", "stealmc"};

        public override void Execute(string[] args)
        {
            if (!PhotonPlayer.TryParse(args[0], out PhotonPlayer player))
                player = PhotonPlayer.Self;
            PhotonNetwork.SetMasterClient(player);
            Chat.System($"{player} is now MasterClient.");
        }
    }
}
