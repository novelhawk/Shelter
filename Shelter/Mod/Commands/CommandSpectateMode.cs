using Mod.Interface;

namespace Mod.Commands
{
    public class CommandSpectateMode : Command
    {
        public override string CommandName => "specmode";
        public override string[] Aliases => new[] {"s", "spectatemode"};

        public override void Execute(string[] args)
        {
            var enter = FengGameManagerMKII.settings.InSpectatorMode == false;
            FengGameManagerMKII.settings.InSpectatorMode = enter;
            FengGameManagerMKII.instance.EnterSpecMode(enter);
            if (enter)
                Chat.System("You are now in spectate mode.");
            else
                Chat.System("You aren't in spectate mode anymore.");
        }
    }
}
