using Mod.Interface;

namespace Mod.Commands
{
    public class CommandSpectateMode : Command
    {
        public override string CommandName => "specmode";
        public override string[] Aliases => new[] {"s", "spectatemode"};

        public override void Execute(string[] args)
        {
            var enter = GameManager.settings.InSpectatorMode == false;
            GameManager.settings.InSpectatorMode = enter;
            GameManager.instance.EnterSpecMode(enter);
            if (enter)
                Shelter.Chat.System("You are now in spectate mode.");
            else
                Shelter.Chat.System("You aren't in spectate mode anymore.");
        }
    }
}
