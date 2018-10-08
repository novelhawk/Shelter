using Newtonsoft.Json;
using UnityEngine;

namespace Mod.GameSettings
{
    public struct Settings
    {
        [JsonProperty("enableHumanSkins")]
        public bool EnableHumanSkins;
        [JsonProperty("enableTitanSkins")]
        public bool EnableTitanSkins;
        [JsonProperty("enableLevelSkins")]
        public bool EnableLevelSkins;
        
        [JsonProperty("enableGasSkin")]
        public bool EnableGasSkin;
        [JsonProperty("enableWeaponTrail")]
        public bool EnableWeaponTrail;
        [JsonProperty("enableWind")]
        public bool EnableWind;
        [JsonProperty("enableReel")]
        public bool EnableReel;

        [JsonProperty("randomizeSkinParts")]
        public bool Randomize;

        [JsonProperty("humanSkin")]
        public HumanSkin HumanSkin;
        [JsonProperty("titanSkin")]
        public TitanSkin TitanSkin;
        [JsonProperty("forestSkin")]
        public ForestSkin ForestSkin;
        [JsonProperty("citySkin")]
        public CitySkin CitySkin;
        [JsonProperty("customMapSkin")]
        public CustomMapSkin CustomSkin;
        
        [JsonProperty("teamSort")]
        public TeamSort TeamSort; // Unused

        [JsonProperty("titans")]
        public int Titans; // Unused
        [JsonProperty("respawnTimer")]
        public int RespawnTimer; // Unused
        [JsonProperty("spawnTimer")]
        public int SpawnTimer; // Unused
        [JsonProperty("customMaxTitans")]
        public int CustomMaxTitans;
        
        [JsonProperty("snapshotMinDamage")]
        public int SnapshotDamage;
        
        [JsonProperty("enableVSync")]
        public bool EnableVSync;
        [JsonProperty("fpsCap")]
        public int FPSCap;
        
        [JsonProperty("speedmeterType")]
        public Speedmeter SpeedmeterType;
        
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
        public int MoreTitansNumber; // Unused

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
        public bool EnableChatFeed;

        public bool InSpectatorMode;

        [JsonProperty("bombColor")]
        public Color BombColor;
        [JsonProperty("bombRadius")]
        public int BombRadius;
        [JsonProperty("bombRange")]
        public int BombRange;
        [JsonProperty("bombSpeed")]
        public int BombSpeed;
        [JsonProperty("bombCountdown")]
        public int BombCountdown;

        [JsonProperty("allowCannonPKs")]
        public bool AllowCannonHumanKills;

        [JsonProperty("masterTextureLimit")]
        public int MasterTextureLimit;
        [JsonProperty("useMipmap")]
        public bool UseMipmap;
        
        [JsonProperty("volume")]
        public float Volume;
        [JsonProperty("cameraDistance")]
        public float CameraDistance;
        [JsonProperty("mouseSensitivity")]
        public float MouseSensitivity;
        [JsonProperty("gameQuality")]
        public float GameQuality;
    }
}