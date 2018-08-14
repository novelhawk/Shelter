using System;
using System.Runtime.Serialization;
using Mod.Interface;

namespace Mod.Exceptions
{
    [Serializable]
    public class NotAllowedException : CustomException
    {
        public NotAllowedException(byte eventId, Player sender)
        {
            Chat.System($"NotAllowedExeption from {sender} on calling Event({eventId:X})");
            if (!FengGameManagerMKII.ignoreList.Contains(sender.ID))
                FengGameManagerMKII.ignoreList.Add(sender.ID);
        }

        public NotAllowedException(string rpc, PhotonMessageInfo info, bool ignore = true)
        {
            Chat.System($"NotAllowedExeption from {info.sender} on calling RPC({rpc})");
            if (ignore && !FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
                FengGameManagerMKII.ignoreList.Add(info.sender.ID);
        }
    }
}