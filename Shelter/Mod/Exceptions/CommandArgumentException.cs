using System;

namespace Mod.Exceptions
{
    [Serializable]
    public class CommandArgumentException : CustomException
    {
        public CommandArgumentException(string command, string usage)
        {
            Shelter.Chat.System("Error in '{0}' args", command);
            Shelter.Chat.System("Usage: {0}", usage);
        }
    }
}
