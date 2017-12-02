using Mod.Interface;

namespace Mod.Commands
{
    public class CommandSpectateMode : Command
    {
        public override string CommandName => "specmode";
        public override string[] Aliases => new[] {"s", "spectatemode"};

        public override void Execute(string[] args)
        {
            var condition = (int)FengGameManagerMKII.settings[245] == 0;
            FengGameManagerMKII.settings[245] = condition ? 1 : 0;
            FengGameManagerMKII.instance.EnterSpecMode(condition);
            if (condition)
                Chat.System("You are now in spectate mode.");
            else
                Chat.System("You aren't in spectate mode anymore.");
        }
    }
}
