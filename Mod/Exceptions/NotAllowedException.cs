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
            Shelter.LogBoth("{0} from {1} on calling Event({2})", LogType.Warning, nameof(NotAllowedException), sender, (PhotonEvent) eventId);
            if (!FengGameManagerMKII.ignoreList.Contains(sender.ID))
                FengGameManagerMKII.ignoreList.Add(sender.ID);
        }

        public NotAllowedException(string rpc, PhotonMessageInfo info, bool ignore = true)
        {
            Shelter.LogBoth("{0} from {1} on calling RPC({2})", LogType.Warning, nameof(NotAllowedException), info.sender, rpc);
            if (ignore && !FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
                FengGameManagerMKII.ignoreList.Add(info.sender.ID);
        }
    }
}