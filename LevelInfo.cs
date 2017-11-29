public class LevelInfo
{
    public string Description { get; set; }
    public int EnemyNumber { get; set; }
    public bool Hint { get; set; }
    public bool Horse { get; set; }
    public bool IsLava { get; set; }
    public string Map { get; set; }
    public Minimap.Preset MinimapPreset { get; set; }
    public string Name { get; set; }
    public bool NoCrawler { get; set; }
    public bool HasPunk { get; set; } = true;
    public bool IsPvP { get; set; }
    public RespawnMode RespawnMode { get; set; }
    public bool Supply { get; set; } = true;
    public bool PlayerTitansAllowed { get; set; }
    public GAMEMODE Gamemode { get; set; }

    public static LevelInfo getInfo(string name)
    {
        return null;
    }
}

