using ExitGames.Client.Photon;
using Mod;

namespace Photon
{
    public class RoomOptions : Hashtable
    {
        public bool? RemovedFromList => (bool?) this[GamePropertyKey.Removed];
        public bool? DoAutoCleanup => (bool?) this[GamePropertyKey.CleanupCacheOnLeave];
        public int? CurrentPlayers => (byte?) this[GamePropertyKey.PlayerCount];
        public int? PlayerTTL => (int?) this[GamePropertyKey.PlayerTTL];
        public int? RoomTTL => (int?) this[GamePropertyKey.EmptyRoomTTL];
    
        public int? MaxPlayers
        {
            get => this[GamePropertyKey.MaxPlayers] as byte?;
            set => this[GamePropertyKey.MaxPlayers] = value;
        }

        public bool? IsVisible
        {
            get => this[GamePropertyKey.IsVisible] as bool?;
            set => this[GamePropertyKey.IsVisible] = value;
        }
    
        public bool? IsOpen
        {
            get => this[GamePropertyKey.IsOpen] as bool?;
            set => this[GamePropertyKey.IsOpen] = value;
        }
    }
}

