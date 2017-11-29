using System;
using System.IO.Pipes;

namespace Mod.Discord
{
    public class DiscordAPI
    {
        private readonly NamedPipeClientStream connection;

        public DiscordAPI()
        {
            connection = new NamedPipeClientStream(".", "discord-ipc-0");
        }

        public NamedPipeClientStream Get() => connection;
    }
}
