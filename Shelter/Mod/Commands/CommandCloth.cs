using Game;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandCloth : Command
    {
        public override string CommandName => "cloth";

        public override void Execute(string[] args)
        {
            Shelter.Chat.System("Active clothes: {0}", ClothFactory.GetDebugInfo());
        }
    }
}
