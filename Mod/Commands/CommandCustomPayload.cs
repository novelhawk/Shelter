using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandCustomPayload : Command
    {
        public override string CommandName => "payload";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/payload [payload] (playerId) (times)");
            
            var hex = args[0];
            if (hex.Length > byte.MaxValue * 2)
            {
                Chat.System("Cannot deserialize payload: Longer than 256 bytes.");
                return;
            }

            if (hex.Contains("ID"))
            {
                if (args.Length < 2 || !Player.TryParse(args[1], out Player player))
                    throw new CommandArgumentException(CommandName, "/payload [payload] [playerId] (times)");
                
                hex = hex.Replace("ID", player.ID.ToString("X2"));
            }
            
            var bytes = StringToBytes(hex);
            DecryptBytes(bytes);

            if (args.Length < 2 || !int.TryParse(args[2], out var times))
                times = 1;
            
            #if CUSTOMPHOTONASSEMBLY
            if (!(PhotonNetwork.networkingPeer.peerBase is EnetPeer peer))
            {
                Chat.System("Peer is not available.");
                return;
            }
            
            for (int i = 0; i < times; i++)
                peer.CreateAndEnqueueCommand(0x6, bytes, 0x0);
            #endif
            
            Chat.System("Sent payload ({0} bytes) to {1} {2} times", bytes.Length, args.Length >= 2 ? args[2] : "everyone", times);
        }

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
