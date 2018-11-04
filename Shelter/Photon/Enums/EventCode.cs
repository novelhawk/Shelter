using System;

namespace Photon.Enums
{
    /// <summary>
    /// Class for constants. These values are for events defined by Photon Loadbalancing.
    /// Pun uses these constants internally.
    /// </summary>
    /// <remarks>They start at 255 and go DOWN. Your own in-game events can start at 0.</remarks>
    public static class EventCode
    {
        /// <summary>(230) Initial list of RoomInfos (in lobby on Master)</summary>
        public const byte GameList = 230;

        /// <summary>(229) Update of RoomInfos to be merged into "initial" list (in lobby on Master)</summary>
        public const byte GameListUpdate = 229;

        /// <summary>(228) Currently not used. State of queueing in case of server-full</summary>
        public const byte QueueState = 228;

        /// <summary>(227) Currently not used. Event for matchmaking</summary>
        public const byte Match = 227;

        /// <summary>(226) Event with stats about this application (players, rooms, etc)</summary>
        public const byte AppStats = 226;

        /// <summary>(224) This event provides a list of lobbies with their player and game counts.</summary>
        public const byte LobbyStats = 224;

        /// <summary>(210) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureNodeInfo = 210;

        /// <summary>(255) Event Join: someone joined the game. The new actorNumber is provided as well as the properties of that actor (if set in OpJoin).</summary>
        public const byte Join = 255;

        /// <summary>(254) Event Leave: The player who left the game can be identified by the actorNumber.</summary>
        public const byte Leave = 254;

        /// <summary>(253) When you call OpSetProperties with the broadcast option "on", this event is fired. It contains the properties being set.</summary>
        public const byte PropertiesChanged = 253;

        /// <summary>(253) When you call OpSetProperties with the broadcast option "on", this event is fired. It contains the properties being set.</summary>
        [Obsolete("Use PropertiesChanged now.")]
        public const byte SetProperties = 253;

        /// <summary>(252) When player left game unexpected and the room has a playerTtl > 0, this event is fired to let everyone know about the timeout.</summary>
        /// Obsolete. Replaced by Leave. public const byte Disconnect = LiteEventCode.Disconnect;

        /// <summary>(251) Sent by Photon Cloud when a plugin-call or webhook-call failed. Usually, the execution on the server continues, despite the issue. Contains: ParameterCode.Info.</summary>
        /// <seealso cref="https://doc.photonengine.com/en/realtime/current/reference/webhooks#options"/>
        public const byte ErrorInfo = 251;

        /// <summary>(250) Sent by Photon whent he event cache slice was changed. Done by OpRaiseEvent.</summary>
        public const byte CacheSliceChanged = 250;

        /// <summary>(223) Sent by Photon to update a token before it times out.</summary>
        public const byte AuthEvent = 223;
    }
}