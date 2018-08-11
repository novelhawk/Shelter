using System.CodeDom;
using Mod.Interface;

namespace Mod.Exceptions
{
    public class CommandArgumentException : CustomException
    {
        public CommandArgumentException(string command, string usage)
        {
            Chat.System($"Error in {command}'s args");
            Chat.System($"Usage: {usage}");
        }
    }
}
