using Boo.Lang;
using ExitGames.Client.Photon;

public class RoomOptions : Hashtable
{
    public bool? RemovedFromList => (bool?) this[251];
    public bool? DoAutoCleanup => (bool?) this[249];
    public int? CurrentPlayers => (byte?) this[252];
    public int? PlayerTTL => (int?) this[235];
    public int? RoomTTL => (int?) this[236];
    
    public int? MaxPlayers
    {
        get => (byte?) this[255];
        set => this[255] = value;
    }
    
    public bool? IsVisible
    {
        get => (bool?) this[254];
        set => this[254] = value;
    }
    
    public bool? IsOpen
    {
        get => (bool?) this[253];
        set => this[253] = value;
    }

    private object this[int key]
    {
        get => base[(byte) key];
        set => base[(byte) key] = value;
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

