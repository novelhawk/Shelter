using System;
using System.Runtime.Serialization;
using Mod.Interface;
using Mod.Logging;

namespace Mod.Exceptions
{
    [Serializable]
    public class NotAllowedException : CustomException
    {
        public NotAllowedException(byte eventId, Player sender)
        {
            Shelter.LogConsole("NotAllowedExeption from {0} on calling Event({1})", LogLevel.Warning, sender, (PhotonEvent) eventId);
            if (!FengGameManagerMKII.ignoreList.Contains(sender.ID))
                FengGameManagerMKII.ignoreList.Add(sender.ID);
        }

        public NotAllowedException(string rpc, PhotonMessageInfo info, bool ignore = true)
        {
            Shelter.LogConsole("NotAllowedExeption from {0} on calling RPC({1})", LogLevel.Warning, info.sender, rpc);
            if (ignore && !FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
                FengGameManagerMKII.ignoreList.Add(info.sender.ID);
        }
    }
}