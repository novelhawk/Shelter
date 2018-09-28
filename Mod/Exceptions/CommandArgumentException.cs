using System.CodeDom;
using Mod.Interface;

namespace Mod.Exceptions
{
    public class CommandArgumentException : CustomException
    {
        public CommandArgumentException(string command, string usage)
        {
            Chat.System("Error in '{0}' args", command);
            Chat.System("Usage: {0}", usage);
        }
    }
}
