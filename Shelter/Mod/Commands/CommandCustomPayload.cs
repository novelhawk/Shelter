#define CUSTOMPHOTONASSEMBLY // Remove this if you don't have a custom assembly

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;
using Mod.Exceptions;
using Photon;

namespace Mod.Commands
{
    public class CommandCustomPayload : Command
    {
        public override string CommandName => "payload";

        public override void Execute(string[] args)
        {
#if !CUSTOMPHOTONASSEMBLY
            Chat.System("You need a custom photon assembly in order to use this command!");
            return;
#endif
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/payload [payload] (playerId) (times)");
            
            var hex = args[0];
            if (hex.Length > byte.MaxValue * 2)
            {
                Shelter.Chat.System("Cannot deserialize payload: Longer than 256 bytes.");
                return;
            }
            
            Player player = null;
            if (hex.Contains("ID"))
            {
                if (args.Length < 2 || !Player.TryParse(args[1], out player))
                    throw new CommandArgumentException(CommandName, "/payload [payload] [playerId] (times)");
                
                hex = hex.Replace("ID", player.ID.ToString("X8"));
            }
            
            var bytes = StringToBytes(hex);
            DecryptBytes(bytes);

            if (args.Length < 2 || !int.TryParse(args[2], out var times))
                times = 1;
            
            if (!(PhotonNetwork.networkingPeer.peerBase is EnetPeer peer))
            {
                Shelter.Chat.System("Peer is not available.");
                return;
            }
            
            for (int i = 0; i < times; i++)
                peer.CreateAndEnqueueCommand(0x6, bytes, 0x0);
            
            Shelter.Chat.System("Sent payload ({0} bytes) to {1} {2} times", bytes.Length, player?.ToString() ?? "everyone", times);
        }

        [Conditional("RELEASE")] // Development version allows unencrypted data.
        private static void DecryptBytes(IList<byte> bytes)
        {
            for (byte i = 0; i < bytes.Count; i++)
            {
                unchecked
                {
                    bytes[i] -= (byte) (i + 1);
                }
            }
        }

        private static byte[] StringToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
    }
}
