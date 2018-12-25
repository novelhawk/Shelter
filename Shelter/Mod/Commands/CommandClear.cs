using Photon.Enums;

namespace Mod.Commands
{
    public class CommandClear : Command
    {
        public override string CommandName => "clear";
        public override string[] Aliases => new[] {"cc"};

        public override void Execute(string[] args)
        {
            if (args.Length > 0)
                Shelter.Chat.Clear();
            for (int i = 0; i < 30; i++)
                Shelter.Chat.SendMessage(string.Empty, string.Empty, PhotonTargets.Others);
            
            Shelter.Chat.System("Chat has been cleaned up by {0}.", Player.Self.Properties.HexName);
        }
    }
}
