using ExitGames.Client.Photon;
using Mod;

namespace Photon
{
    public class RoomOptions : Hashtable
    {
        public bool? RemovedFromList => (bool?) this[GamePropertyKey.Removed];
        public bool? DoAutoCleanup => (bool?) this[GamePropertyKey.CleanupCacheOnLeave];
        public int? CurrentPlayers => (byte?) this[GamePropertyKey.PlayerCount];
        public int? PlayerTTL => (int?) this[ParameterCode.PlayerTTL];
        public int? RoomTTL => (int?) this[ParameterCode.EmptyRoomTTL];
    
        public int? MaxPlayers
        {
            get => (byte?) this[GamePropertyKey.MaxPlayers];
            set => this[GamePropertyKey.MaxPlayers] = value;
        }
    
        public bool? IsVisible
        {
            get => (bool?) this[GamePropertyKey.IsVisible];
            set => this[GamePropertyKey.IsVisible] = value;
        }
    
        public bool? IsOpen
        {
            get => (bool?) this[GamePropertyKey.IsOpen];
            set => this[GamePropertyKey.IsOpen] = value;
        }

        public RoomOptions()
        {}

        public RoomOptions(Hashtable hash)
        {
            if (hash == null)
                return;
        
            foreach (var entry in hash)
                this[entry.Key] = entry.Value;
        }
    }
}

