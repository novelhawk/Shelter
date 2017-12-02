using System;
using System.Runtime.Serialization;
using Mod.Interface;

namespace Mod.Exceptions
{
    [Serializable]
    public class NotAllowedException : CustomException
    {
        public NotAllowedException(string eventId, PhotonPlayer sender)
        {
            Chat.SendMessage($"NotAllowedExeption from {sender} on calling Event({eventId})"); // Temp global
            Chat.SendMessage("Ignore is temporaly disabled");
        }

        public NotAllowedException(string rpc, PhotonMessageInfo info)
        {
            Chat.SendMessage($"NotAllowedExeption from {info.sender} on calling RPC({rpc})"); // Temp global
            Chat.SendMessage("Ignore is temporaly disabled");
        }
    }
}