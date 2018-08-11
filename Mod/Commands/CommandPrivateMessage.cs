using System.Text.RegularExpressions;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandPrivateMessage : Command
    {
        public override string CommandName => "msg";
        public override string[] Aliases => new[] {"pm", "privatemessage", "w", "whisper"};

        public override void Execute(string[] args)
        {
            if (args.Length < 2)
                throw new CommandArgumentException(CommandName, "/pm [id] [msg]");
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);
            
            player.SendPrivateMessage(Regex.Match(Chat.Message, @"[\\\/]\w+\s\d+\s(.*)").Groups[1].Value); //TODO: Add private message and reply
            CommandReply.replyTo = player.ID;
        }
    }
}
