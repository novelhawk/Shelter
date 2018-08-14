using System;

namespace Mod.Discord.Exceptions
{
    class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(string message) : base(message) { }
    }
}
