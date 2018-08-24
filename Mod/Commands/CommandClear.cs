﻿using Mod.Interface;

namespace Mod.Commands
{
    public class CommandClear : Command
    {
        public override string CommandName => "clear";
        public override string[] Aliases => new[] {"cc"};

        public override void Execute(string[] args)
        {
            if (args.Length > 0)
                Chat.Clear();
            for (int i = 0; i < 30; i++)
                Chat.SendMessage(string.Empty, PhotonTargets.Others);
            
            Chat.System($"Chat has been cleaned up by {Player.Self.HexName}");
        }
    }
}
