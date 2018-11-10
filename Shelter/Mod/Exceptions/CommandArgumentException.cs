using Mod.Interface;

namespace Mod.Exceptions
{
    public class CommandArgumentException : CustomException
    {
        public CommandArgumentException(string command, string usage)
        {
            Shelter.Chat.System("Error in '{0}' args", command);
            Shelter.Chat.System("Usage: {0}", usage);
        }
    }
}
