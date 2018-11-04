namespace Photon.Enums
{
    /// <summary>
    /// Class for constants. These (byte) values are for "well known" room/game properties used in Photon Loadbalancing.
    /// </summary>
    /// <remarks>
    /// Pun uses these constants internally.
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public static class GamePropertyKey
    {
        /// <summary>(255) Max number of players that "fit" into this room. 0 is for "unlimited".</summary>
        public const byte MaxPlayers = 255;

        /// <summary>(254) Makes this room listed or not in the lobby on master.</summary>
        public const byte IsVisible = 254;

        /// <summary>(253) Allows more players to join a room (or not).</summary>
        public const byte IsOpen = 253;

        /// <summary>(252) Current count of players in the room. Used only in the lobby on master.</summary>
        public const byte PlayerCount = 252;

        /// <summary>(251) True if the room is to be removed from room listing (used in update to room list in lobby on master)</summary>
        public const byte Removed = 251;

        /// <summary>(250) A list of the room properties to pass to the RoomInfo list in a lobby. This is used in CreateRoom, which defines this list once per room.</summary>
        public const byte PropsListedInLobby = 250;

        /// <summary>(249) Equivalent of Operation Join parameter CleanupCacheOnLeave.</summary>
        public const byte CleanupCacheOnLeave = 249;

        /// <summary>(248) Code for MasterClientId, which is synced by server. When sent as op-parameter this is (byte)203. As room property this is (byte)248.</summary>
        /// <remarks>Tightly related to ParameterCode.MasterClientId.</remarks>
        public const byte MasterClientId = 248;

        /// <summary>(247) Code for ExpectedUsers in a room. Matchmaking keeps a slot open for the players with these userIDs.</summary>
        public const byte ExpectedUsers = 247;

        /// <summary>(246) Player Time To Live. How long any player can be inactive (due to disconnect or leave) before the user gets removed from the playerlist (freeing a slot).</summary>
        public const byte PlayerTTL = 246;

        /// <summary>(245) Room Time To Live. How long a room stays available (and in server-memory), after the last player becomes inactive. After this time, the room gets persisted or destroyed.</summary>
        public const byte EmptyRoomTTL = 245;
    }
}