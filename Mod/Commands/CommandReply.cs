using System.Text.RegularExpressions;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandReply : Command
    {
        public static int replyTo;
        
        public override string CommandName => "reply";
        public override string[] Aliases => new[] {"r"};

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/reply [message]");
            if (!Player.TryParse(replyTo, out Player player))
                throw new PlayerNotFoundException(replyTo);
            
            player.SendPrivateMessage(Regex.Match(Chat.Message, @"[\\\/]\w+\s(.*)").Groups[1].Value);
        }
    }
}
