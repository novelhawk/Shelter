namespace Photon.Enums
{
    /// <summary>
    /// Class for constants. These (byte) values define "well known" properties for an Actor / Player.
    /// Pun uses these constants internally.
    /// </summary>
    /// <remarks>
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public static class ActorProperties
    {
        /// <summary>(255) Name of a player/actor.</summary>
        public const byte PlayerName = 255; // was: 1

        /// <summary>(254) Tells you if the player is currently in this game (getting events live).</summary>
        /// <remarks>A server-set value for async games, where players can leave the game and return later.</remarks>
        public const byte IsInactive = 254;

        /// <summary>(253) UserId of the player. Sent when room gets created with RoomOptions.PublishUserId = true.</summary>
        public const byte UserId = 253;
    }
}