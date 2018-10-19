using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using Mod.Interface;
using Mod.Logging;
using Photon;

namespace Mod.Exceptions
{
    [Serializable]
    public class NotAllowedException : CustomException
    {
        public NotAllowedException(byte eventId, Player sender)
        {
            Shelter.LogBoth("{0} from {1} on calling Event({2})", LogType.Warning, nameof(NotAllowedException), sender, eventId);
            sender.Ignore();
        }

        public NotAllowedException(string rpc, PhotonMessageInfo info, bool ignore = true)
        {
            Shelter.LogBoth("{0} from {1} on calling RPC({2})", LogType.Warning, nameof(NotAllowedException), info.sender, rpc);
            if (ignore)
                info.sender.Ignore();
        }
    }
}