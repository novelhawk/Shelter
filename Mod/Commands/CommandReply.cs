using System.Text.RegularExpressions;
using Mod.Exceptions;

namespace Mod.Commands
{
    public class CommandReply : Command
    {
        public override string CommandName => "reply";
        public override string[] Aliases => new[] {"r"};

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/reply [message]");
//            if (FengGameManagerMKII.instance.reply == null) TODO: Add reply
//                throw new PlayerNotFoundException();
//            FengGameManagerMKII.instance.reply.SendPrivateMessage(Regex.Match(GUIChat.Message, @"[\\\/]\w*\s(.*)").Groups[1].Value);
        }
    }
}
