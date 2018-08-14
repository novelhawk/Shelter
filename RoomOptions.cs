using ExitGames.Client.Photon;

public class RoomOptions
{
    public Hashtable RoomProperties { get; }
    
    public bool RemovedFromList { get; set; }
    public int MaxPlayers { get; set; } //TODO: Setter is ignored. Maybe need to apply it to Room
    public int CurrentPlayers { get; set; } 
    public bool IsVisible { get; set; }
    public bool IsOpen { get; set; }
    public bool DoAutoCleanup { get; set; }
}

