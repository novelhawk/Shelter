using System;
using Mod.Logging;
using Photon;

namespace Mod.Exceptions
{
    [Serializable]
    public class NotAllowedException : CustomException
    {
        public NotAllowedException(byte eventId, Player sender) : base(string.Format("Blocked Event({1}) from {0}", sender.Properties.HexName, (PhotonEvent) eventId))
        {
            Shelter.LogBoth(nameof(NotAllowedException) + " from {0} on calling Event({1})", LogType.Warning, sender, (PhotonEvent) eventId);
            sender.Ignore();
        }

        public NotAllowedException(string rpc, PhotonMessageInfo info, bool ignore = true) : base(string.Format("Blocked RPC({1}) from {0}", info.sender.Properties.HexName, rpc))
        {
            Shelter.LogBoth(nameof(NotAllowedException) + " from {0} on calling RPC({1})", LogType.Warning, info.sender, rpc);
            if (ignore)
                info.sender.Ignore();
        }
    }
}