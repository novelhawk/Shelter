using Newtonsoft.Json;
using UnityEngine;

namespace Mod.GameSettings
{
    public struct Settings
    {
        [JsonProperty("randomizeSkinParts")]
        public bool Randomize; // TODO: Move away

        [JsonProperty("humanSkin")]
        public HumanSkin HumanSkin; // TODO: Move away
        [JsonProperty("titanSkin")]
        public TitanSkin TitanSkin; // TODO: Move away
        [JsonProperty("forestSkin")]
        public ForestSkin ForestSkin; // TODO: Move away
        [JsonProperty("citySkin")]
        public CitySkin CitySkin; // TODO: Move away
        [JsonProperty("customMapSkin")]
        public CustomMapSkin CustomSkin; // TODO: Move away
        
        [JsonProperty("teamSort")]
        public TeamSort TeamSort;

        [JsonProperty("titans")]
        public int Titans;
        [JsonProperty("respawnTimer")]
        public int RespawnTimer;
        [JsonProperty("spawnTimer")]
        public int SpawnTimer;
        [JsonProperty("customMaxTitans")]
        public int CustomMaxTitans;
        
        [JsonProperty("snapshotMinDamage")]
        public int SnapshotDamage; // TODO: Move away
        
        [JsonProperty("enableVSync")]
        public bool EnableVSync; // TODO: Move away
        [JsonProperty("fpsCap")]
        public int FPSCap; // TODO: Move away
        
        [JsonProperty("isBombMode")]
        public bool IsBombMode;
        
        [JsonProperty("enablePunkRockThrow")]
        public bool EnableRockThrow;

        public bool IsExplodeMode => ExplodeRadius > 0;
        [JsonProperty("explodeRadius")]
        public int ExplodeRadius;

        [JsonProperty("healthMode")]
        public HealthMode HealthMode;
        [JsonProperty("titanHealth")]
        public Range TitanHealth;

        public bool IsInfectionMode => InfectionTitanNumber > 0;
        [JsonProperty("infectionTitanNumber")]
        public int InfectionTitanNumber;

        [JsonProperty("allowErenTitan")]
        public bool AllowErenTitan; // 0: true 1: false

        public bool SpawnMoreTitans => MoreTitansNumber > 0;
        [JsonProperty("additionalTitans")]
        public int MoreTitansNumber;

        public bool IsDamageMode => MinimumDamage > 0;
        [JsonProperty("minimumDamage")]
        public int MinimumDamage;

        public bool EnableCustomSize => TitanSize != Range.Zero;
        [JsonProperty("titanSize")]
        public Range TitanSize;

        public bool UseCustomSpawnRates => SpawnRates != SpawnRates.Zero;
        [JsonProperty("spawnRates")]
        public SpawnRates SpawnRates;

        [JsonProperty("enableHorse")]
        public bool EnableHorse;

        public bool IsWaveMode => WaveNumber > 0;
        [JsonProperty("waveNumber")]
        public int WaveNumber;

        public bool IsMaxWaveMode => MaxWaveNumber > 0;
        [JsonProperty("maxWaveNumber")]
        public int MaxWaveNumber;

        [JsonProperty("friendlyMode")]
        public bool IsFriendlyMode;
        
        [JsonProperty("gameType")]
        public int GameType; // Not assigned. Dunno what's that for

        [JsonProperty("pvpMode")]
        public PVPMode PVPMode;

        public bool IsEndless => EndlessTime >= 0;
        [JsonProperty("endlessTime")]
        public int EndlessTime;

        public bool HasMotd => Motd != string.Empty;
        [JsonProperty("motd")]
        public string Motd;

        public bool IsPointMode => PointModeWin > 0;
        [JsonProperty("pointModePoints")]
        public int PointModeWin;

        [JsonProperty("ahssAirReload")]
        public bool AllowAirAHSSReload;

        [JsonProperty("allowPunks")]
        public bool AllowPunks;

        [JsonProperty("asoPreserveKDA")]
        public bool AsoPreserveKDA;
        [JsonProperty("enableASORacing")]
        public bool IsASORacing;

        [JsonProperty("enableMap")]
        public bool IsMapAllowed;

        [JsonProperty("enableChatFeed")]
        public bool EnableChatFeed; // TODO: Move away

        public bool InSpectatorMode; // TODO: Move away

        [JsonProperty("bombColor")]
        public Color BombColor; // TODO: Move away or Remove
        [JsonProperty("bombRadius")]
        public int BombRadius; // TODO: Move away or Remove
        [JsonProperty("bombRange")]
        public int BombRange; // TODO: Move away or Remove
        [JsonProperty("bombSpeed")]
        public int BombSpeed; // TODO: Move away or Remove
        [JsonProperty("bombCountdown")]
        public int BombCountdown; // TODO: Move away or Remove

        [JsonProperty("allowCannonPKs")]
        public bool AllowCannonHumanKills;

        [JsonProperty("masterTextureLimit")]
        public int MasterTextureLimit; // TODO: Move away
        [JsonProperty("useMipmap")]
        public bool UseMipmap; // TODO: Move away
        
        [JsonProperty("volume")]
        public float Volume; // TODO: Move away
        [JsonProperty("cameraDistance")]
        public float CameraDistance; // TODO: Move away
        [JsonProperty("mouseSensitivity")]
        public float MouseSensitivity; // TODO: Move away
    }
}