using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using ExitGames.Client.Photon;
using Mod.GameSettings;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Mod.GameSettings
{
    public class GameSettings
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

        [JsonProperty("showMap")]
        public bool EnableMap;
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

        public void ImportFromRC()
        {
            EnableHumanSkins = Utility.GetBoolean("human");
            EnableTitanSkins = Utility.GetBoolean("titan");
            EnableLevelSkins = Utility.GetBoolean("level");

            HumanSkin = new HumanSkin
            {
                Horse = PlayerPrefs.GetString("horse", string.Empty), 
                Hair = PlayerPrefs.GetString("hair", string.Empty),
                Eye = PlayerPrefs.GetString("eye", string.Empty),
                Glass = PlayerPrefs.GetString("glass", string.Empty),
                Face = PlayerPrefs.GetString("face", string.Empty),
                Body = PlayerPrefs.GetString("skin", string.Empty),
                Costume = PlayerPrefs.GetString("costume", string.Empty),
                Cape = PlayerPrefs.GetString("logo", string.Empty),
                LeftBlade = PlayerPrefs.GetString("bladel", string.Empty),
                RightBlade = PlayerPrefs.GetString("blader", string.Empty),
                Hoodie = PlayerPrefs.GetString("hoodie", string.Empty),
                Trail = PlayerPrefs.GetString("trailskin", string.Empty)
            };

            EnableGasSkin = Utility.GetBoolean("gasenable");

            TitanSkin = new TitanSkin
            {
                Body = new[]
                {
                    PlayerPrefs.GetString("titanbody1", string.Empty),
                    PlayerPrefs.GetString("titanbody2", string.Empty),
                    PlayerPrefs.GetString("titanbody3", string.Empty),
                    PlayerPrefs.GetString("titanbody4", string.Empty),
                    PlayerPrefs.GetString("titanbody5", string.Empty)
                },
                Hairs = new[]
                {
                    PlayerPrefs.GetString("titanhair1", string.Empty),
                    PlayerPrefs.GetString("titanhair2", string.Empty),
                    PlayerPrefs.GetString("titanhair3", string.Empty),
                    PlayerPrefs.GetString("titanhair4", string.Empty),
                    PlayerPrefs.GetString("titanhair5", string.Empty)
                },
                Type = new[]
                {
                    PlayerPrefs.GetInt("titantype1", -1),
                    PlayerPrefs.GetInt("titantype2", -1),
                    PlayerPrefs.GetInt("titantype3", -1),
                    PlayerPrefs.GetInt("titantype4", -1),
                    PlayerPrefs.GetInt("titantype5", -1)
                },
                Eyes = new[]
                {
                    PlayerPrefs.GetString("titaneye1", string.Empty),
                    PlayerPrefs.GetString("titaneye2", string.Empty),
                    PlayerPrefs.GetString("titaneye3", string.Empty),
                    PlayerPrefs.GetString("titaneye4", string.Empty),
                    PlayerPrefs.GetString("titaneye5", string.Empty)
                },
                Annie = PlayerPrefs.GetString("annie", string.Empty), 
                Eren = PlayerPrefs.GetString("eren", string.Empty), 
                Colossal = PlayerPrefs.GetString("colossal", string.Empty)
            };

            CitySkin = new CitySkin
            {
                Houses = new[]
                {
                    PlayerPrefs.GetString("house1", string.Empty),
                    PlayerPrefs.GetString("house2", string.Empty),
                    PlayerPrefs.GetString("house3", string.Empty),
                    PlayerPrefs.GetString("house4", string.Empty),
                    PlayerPrefs.GetString("house5", string.Empty),
                    PlayerPrefs.GetString("house6", string.Empty),
                    PlayerPrefs.GetString("house7", string.Empty),
                    PlayerPrefs.GetString("house8", string.Empty)
                },
                Skybox = new[]
                {
                    PlayerPrefs.GetString("cityskyfront", string.Empty),
                    PlayerPrefs.GetString("cityskyback", string.Empty),
                    PlayerPrefs.GetString("cityskyleft", string.Empty),
                    PlayerPrefs.GetString("cityskyright", string.Empty),
                    PlayerPrefs.GetString("cityskyup", string.Empty),
                    PlayerPrefs.GetString("cityskydown", string.Empty)
                },
                Ground = PlayerPrefs.GetString("cityG", string.Empty),
                Walls = PlayerPrefs.GetString("cityW", string.Empty),
                Gate = PlayerPrefs.GetString("cityH", string.Empty)
            };

            ForestSkin = new ForestSkin
            {
                Skybox = new[]
                {
                    PlayerPrefs.GetString("forestskyfront", string.Empty),
                    PlayerPrefs.GetString("forestskyback", string.Empty),
                    PlayerPrefs.GetString("forestskyleft", string.Empty),
                    PlayerPrefs.GetString("forestskyright", string.Empty),
                    PlayerPrefs.GetString("forestskyup", string.Empty),
                    PlayerPrefs.GetString("forestskydown", string.Empty)
                },
                Trees = new[]
                {
                    PlayerPrefs.GetString("tree1", string.Empty),
                    PlayerPrefs.GetString("tree2", string.Empty),
                    PlayerPrefs.GetString("tree3", string.Empty),
                    PlayerPrefs.GetString("tree4", string.Empty),
                    PlayerPrefs.GetString("tree5", string.Empty),
                    PlayerPrefs.GetString("tree6", string.Empty),
                    PlayerPrefs.GetString("tree7", string.Empty),
                    PlayerPrefs.GetString("tree8", string.Empty)
                },
                Leaves = new[]
                {
                    PlayerPrefs.GetString("leaf1", string.Empty),
                    PlayerPrefs.GetString("leaf2", string.Empty),
                    PlayerPrefs.GetString("leaf3", string.Empty),
                    PlayerPrefs.GetString("leaf4", string.Empty),
                    PlayerPrefs.GetString("leaf5", string.Empty),
                    PlayerPrefs.GetString("leaf6", string.Empty),
                    PlayerPrefs.GetString("leaf7", string.Empty),
                    PlayerPrefs.GetString("leaf8", string.Empty)
                },
                Ground = PlayerPrefs.GetString("forestG", string.Empty)
            };

            CustomSkin = new CustomMapSkin
            {
                Skybox = new[]
                {
                    PlayerPrefs.GetString("customskyfront", string.Empty),
                    PlayerPrefs.GetString("customskyback", string.Empty),
                    PlayerPrefs.GetString("customskyleft", string.Empty),
                    PlayerPrefs.GetString("customskyright", string.Empty),
                    PlayerPrefs.GetString("customskyup", string.Empty),
                    PlayerPrefs.GetString("customskydown", string.Empty)
                },
                Ground = PlayerPrefs.GetString("customGround", string.Empty)
            };

            Randomize = Utility.GetBoolean("titanR") || Utility.GetBoolean("forestR");
            
            MasterTextureLimit = PlayerPrefs.GetInt("skinQ", 0); 
            UseMipmap = !Utility.GetBoolean("skinQL");
            if (!int.TryParse(PlayerPrefs.GetString("cnumber", "1"), out Titans))
                Titans = 1;
            RespawnTimer = 30;
            SpawnTimer = 0;
            if (!int.TryParse(PlayerPrefs.GetString("cmax", "20"), out CustomMaxTitans))
                CustomMaxTitans = 20;
            EnableWeaponTrail = !Utility.GetBoolean("traildisable");
            EnableWind = !Utility.GetBoolean("wind"); // Might be 1: true 0: false
            if (!int.TryParse(PlayerPrefs.GetString("snapshot", "0"), out SnapshotDamage))
                SnapshotDamage = 0;
            EnableReel = Utility.GetBoolean("reel");
            EnableVSync = Utility.GetBoolean("vsync");
            if (!int.TryParse(PlayerPrefs.GetString("fpscap", "-1"), out FPSCap))
                FPSCap = -1;
            SpeedmeterType = (Speedmeter) PlayerPrefs.GetInt("speedometer", 0);
            IsBombMode = Utility.GetBoolean("bombMode");
            TeamSort = (TeamSort) PlayerPrefs.GetInt("teamMode", 0);
            EnableRockThrow = Utility.GetBoolean("rockThrow");
            if (!int.TryParse(PlayerPrefs.GetString("explodeModeNum", "30"), out ExplodeRadius))
                ExplodeRadius = 30;
            
            HealthMode = (HealthMode) PlayerPrefs.GetInt("healthMode", 0);
            if (!int.TryParse(PlayerPrefs.GetString("healthLower", "100"), out var min))
                min = 100;
            if (!int.TryParse(PlayerPrefs.GetString("healthUpper", "200"), out var max))
                max = 200;
            TitanHealth = new Range(min, max);
            
            IsInfectionMode = Utility.GetBoolean("infectionModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("infectionModeNum", "1"), out InfectionTitanNumber))
                InfectionTitanNumber = 1;
            
            AllowErenTitan = !Utility.GetBoolean("banEren");
            SpawnMoreTitans = Utility.GetBoolean("moreTitanOn");
            if (!int.TryParse(PlayerPrefs.GetString("moreTitanNum", "1"), out MoreTitansNumber))
                MoreTitansNumber = 1;
            
            IsDamageMode = Utility.GetBoolean("damageModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("damageModeNum", "1000"), out MinimumDamage))
                MinimumDamage = 1000;
            
            EnableCustomSize = Utility.GetBoolean("sizeMode");
            if (!int.TryParse(PlayerPrefs.GetString("sizeLower", "1.0"), out min))
                min = 3;
            if (!int.TryParse(PlayerPrefs.GetString("sizeUpper", "3.0"), out max))
                max = 3;
            TitanSize = new Range(min, max);
            
            UseCustomSpawnRates = Utility.GetBoolean("spawnModeOn");
            SpawnRates = new SpawnRates(
                PlayerPrefs.GetString("nRate", "20.0"),
                PlayerPrefs.GetString("aRate", "20.0"),
                PlayerPrefs.GetString("jRate", "20.0"),
                PlayerPrefs.GetString("cRate", "20.0"),
                PlayerPrefs.GetString("pRate", "20.0"));
            EnableHorse = Utility.GetBoolean("horseMode");
            IsWaveMode = Utility.GetBoolean("waveModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("waveModeNum", "1"), out WaveNumber))
                WaveNumber = 1;
            IsFriendlyMode = Utility.GetBoolean("friendlyMode");
            PVPMode = (PVPMode) PlayerPrefs.GetInt("pvpMode", 0);
            IsMaxWaveMode = Utility.GetBoolean("maxWaveOn");
            if (!int.TryParse(PlayerPrefs.GetString("maxWaveNum", "20"), out MaxWaveNumber))
                MaxWaveNumber = 20;
            IsEndless = Utility.GetBoolean("endlessModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("endlessModeNum", "5"), out EndlessTime))
                EndlessTime = 5;
            Motd = PlayerPrefs.GetString("motd", string.Empty);
            IsPointMode = Utility.GetBoolean("pointModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("pointModeNum", "50"), out PointModeWin))
                PointModeWin = 50;

            IsASORacing = Utility.GetBoolean("asoracing");
            AllowAirAHSSReload = Utility.GetBoolean("ahssReload");
            AllowPunks = Utility.GetBoolean("punkWaves");
            EnableMap = Utility.GetBoolean("mapOn");
            IsMapAllowed = !Utility.GetBoolean("globalDisableMinimap");
            EnableChatFeed = Utility.GetBoolean("chatfeed");
            InSpectatorMode = false;
            BombColor = new Color(
                PlayerPrefs.GetFloat("bombR", 1f),
                PlayerPrefs.GetFloat("bombG", 1f),
                PlayerPrefs.GetFloat("bombB", 1f),
                PlayerPrefs.GetFloat("bombA", 1f));
            BombRadius = PlayerPrefs.GetInt("bombRadius", 5);
            BombRange = PlayerPrefs.GetInt("bombRange", 5);
            BombSpeed = PlayerPrefs.GetInt("bombSpeed", 5);
            BombCountdown = PlayerPrefs.GetInt("bombCD", 5);
            AllowCannonHumanKills = Utility.GetBoolean("deadlyCannon");
            Volume = PlayerPrefs.GetFloat("vol", 1f);
            CameraDistance = PlayerPrefs.GetFloat("cameraDistance", 1f);
            MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
            GameQuality = PlayerPrefs.GetFloat("GameQuality", 0f);
            
            Application.targetFrameRate = FPSCap;
            QualitySettings.vSyncCount = EnableVSync ? 1 : 0;
            AudioListener.volume = Volume;
            QualitySettings.masterTextureLimit = MasterTextureLimit;
        }

        public Hashtable SerializeRCSettings()
        {
            Hashtable hashtable = new Hashtable();
            if (IsInfectionMode)
            {
                IsBombMode = false;
                IsPointMode = false;
                TeamSort = TeamSort.Off;
                PVPMode = PVPMode.Off;
                
                if (InfectionTitanNumber < 0 || InfectionTitanNumber > PhotonNetwork.countOfPlayers)
                    InfectionTitanNumber = 1;
                
                hashtable.Add("infection", InfectionTitanNumber);
            }
            if (IsBombMode)
                hashtable.Add("bomb", 1);
            
            if (!EnableMap)
                hashtable.Add("globalDisableMinimap", 1);
                    
            if (TeamSort != TeamSort.Off)
                hashtable.Add("team", (int) TeamSort);
            
//                if (RCSettings.teamMode != (int) settings[193])
//                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
//                        photonView.RPC("setTeamRPC", PhotonNetwork.PlayerList[i], i % 2 + 1);
            if (IsPointMode)
                hashtable.Add("point", PointModeWin);
            
            if (EnableRockThrow)
                hashtable.Add("rock", 1);
            
            if (IsExplodeMode)
                hashtable.Add("explode", ExplodeRadius);
            
            if (HealthMode > HealthMode.Off)
            {
                hashtable.Add("healthMode", HealthMode);
                hashtable.Add("healthLower", (int) TitanHealth.Min);
                hashtable.Add("healthUpper", (int) TitanHealth.Max);
            }
            
            if (!AllowErenTitan)
                hashtable.Add("eren", 1);
            
            if (SpawnMoreTitans)
                hashtable.Add("titanc", Titans);
            
            if (IsDamageMode)
                hashtable.Add("damage", MinimumDamage);
            
            if (EnableCustomSize)
            {
                hashtable.Add("sizeMode", 1);
                hashtable.Add("sizeLower", TitanSize.Min);
                hashtable.Add("sizeUpper", TitanSize.Max);
            }
            
            if (UseCustomSpawnRates)
            {
                hashtable.Add("spawnMode", 1);
                hashtable.Add("nRate", SpawnRates.Normal);
                hashtable.Add("aRate", SpawnRates.Aberrant);
                hashtable.Add("jRate", SpawnRates.Jumper);
                hashtable.Add("cRate", SpawnRates.Crawler);
                hashtable.Add("pRate", SpawnRates.Punk);
            }
            
            if (EnableHorse)
                hashtable.Add("horse", 1);
            
            if (IsWaveMode)
            {
                hashtable.Add("waveModeOn", 1);
                hashtable.Add("waveModeNum", WaveNumber);
            }
            
            if (IsFriendlyMode)
                hashtable.Add("friendly", 1);
            
            if (PVPMode != PVPMode.Off)
                hashtable.Add("pvp", (int) PVPMode);
            
            if (IsMaxWaveMode)
                hashtable.Add("maxwave", MaxWaveNumber);
            
            if (IsEndless)
                hashtable.Add("endless", EndlessTime);
            
            if (!string.IsNullOrEmpty(Motd))
                hashtable.Add("motd", Motd);
            
            if (AllowAirAHSSReload)
                hashtable.Add("ahssReload", 1);
            
            if (AllowPunks)
                hashtable.Add("punkWaves", 1);
            
            if (AllowCannonHumanKills)
                hashtable.Add("deadlycannons", 1);
            
            if (IsASORacing)
                hashtable.Add("asoracing", 1);
            
            return hashtable;
        }

        public void LoadFromHashtable(Hashtable hash)
        {
            if (hash.ContainsKey("bomb"))
                IsBombMode = true;
            
            if (hash.ContainsKey("globalDisableMinimap"))
                IsMapAllowed = !hash.ToBool("globalDisableMinimap");
            if (hash.ContainsKey("horse"))
                EnableHorse = hash.ToBool("horse");
            if (hash.ContainsKey("punkWaves"))
                AllowPunks = hash.ToBool("punkWaves");
            if (hash.ContainsKey("ahssReload"))
                AllowAirAHSSReload = hash.ToBool("ahssReload");
            if (hash.ContainsKey("team"))
                TeamSort = (TeamSort) (int) hash["team"];
            if (hash.ContainsKey("point"))
                IsPointMode = hash.ToBool("point");
            if (hash.ContainsKey("rock"))
                EnableRockThrow = hash.ToBool("rock");
            if (hash.ContainsKey("explode"))
                ExplodeRadius = (int) hash["explode"];
            if (hash.ContainsKey("healthMode") && 
                hash.ContainsKey("healthLower") && 
                hash.ContainsKey("healthUpper"))
            {
                if ((HealthMode = (HealthMode) (int) hash["healthMode"]) > HealthMode.Off)
                {
                    TitanHealth = new Range(
                        (int) hash["healthLower"],
                        (int) hash["healthUpper"]);
                }
            }
            if (hash.ContainsKey("infection"))
                IsInfectionMode = hash.ToBool("infection");

            if (hash.ContainsKey("eren"))
                AllowErenTitan = !hash.ToBool("eren");

            if (hash.ContainsKey("titanc"))
                MoreTitansNumber = (int) hash["titanc"];

            if (hash.ContainsKey("damage"))
            {
                IsDamageMode = true;
                MinimumDamage = (int) hash["damage"];
            }
            if (hash.ContainsKey("sizeMode") && 
                hash.ContainsKey("sizeLower") && 
                hash.ContainsKey("sizeUpper"))
            {
                TitanSize = new Range(
                    (float) hash["sizeLower"],
                    (float) hash["sizeUpper"]);
            }

            if (hash.ContainsKey("spawnMode") && hash.ContainsKey("nRate") && hash.ContainsKey("aRate") &&
                hash.ContainsKey("jRate") && hash.ContainsKey("cRate") && hash.ContainsKey("pRate"))
            {
                UseCustomSpawnRates = hash.ToBool("spawnMode");
                SpawnRates = new SpawnRates
                {
                    Normal = (float) hash["nRate"],
                    Aberrant = (float) hash["aRate"],
                    Jumper = (float) hash["jRate"],
                    Crawler = (float) hash["cRate"],
                    Punk = (float) hash["pRate"],
                };
            }

            if (hash.ContainsKey("waveModeOn") && hash.ContainsKey("waveModeNum"))
            {
                IsWaveMode = hash.ToBool("waveModeOn");
                WaveNumber = (int) hash["waveModeNum"];
            }

            if (hash.ContainsKey("friendly"))
                IsFriendlyMode = hash.ToBool("friendly");

            if (hash.ContainsKey("pvp"))
                PVPMode = (PVPMode) (int) hash["pvp"];

            if (hash.ContainsKey("maxwave"))
                MaxWaveNumber = (int) hash["maxwave"];

            if (hash.ContainsKey("endless"))
            {
                IsEndless = true;
                EndlessTime = (int) hash["endless"];
            }

            if (hash.ContainsKey("motd"))
                Motd = (string) hash["motd"];
            
            if (hash.ContainsKey("deadlycannons"))
                AllowCannonHumanKills = hash.ToBool("deadlycannons");

            if (hash.ContainsKey("asoracing"))
            {
                IsASORacing = true;
            }
        }

        public void ReloadFile()
        {
            
        }
    }
}