using System;
using System.Drawing.Design;
using System.Globalization;
using ExitGames.Client.Photon;
using Mod.GameSettings;
using UnityEngine;
using UnityEngine.UI;

namespace Mod.GameSettings
{
    public class GameSettings
    {
        public bool EnableHumanSkins;
        public bool EnableTitanSkins;
        public bool EnableLevelSkins;
        public bool EnableGasSkin;

        public HumanSkin HumanSkin;
        public TitanSkin TitanSkin;
        public ForestSkin ForestSkin;
        public CitySkin CitySkin;
        public CustomMapSkin CustomSkin;

        public bool Randomize;
        
        public bool IsTeam;
        public TeamSort TeamSort;

        public int Titans;
        public int RespawnTimer;
        public int SpawnTimer;
        public int CustomMaxTitans;
        
        public bool EnableWeaponTrail;
        public bool EnableWind;
        public bool EnableReel;
        public int SnapshotDamage;
        
        public bool EnableVSync;
        public int FPSCap;
        
        public Speedmeter SpeedmeterType;
        
        public bool IsBomb;
        
        public bool IsRockThrowEnabled;
        
        public bool IsExplodeMode;
        public int ExplodeRadius;

        public bool IsHealthMode;
        public Range TitanHealth;

        public bool IsInfectionMode;
        public int InfectionStartTitan;

        public bool IsErenAllowed; // 0: true 1: false

        public bool DoSpawnMoreTitans;
        public int MoreTitansNumber;

        public bool IsDamageMode;
        public int MinimumDamage;

        public bool IsSizeMode;
        public Range TitanSize;

        public bool UseCustomSpawnRates;
        public SpawnRates SpawnRates;

        public bool IsHorseAllowed;

        public bool IsWaveMode; // Aren't those 2 the same
        public int WaveNumber;

        public bool IsMaxWaveMode; // Aren't those 2 the same
        public int MaxWaveNumber;

        public bool IsFriendlyMode;

        public PVPMode PVPMode;

        public bool IsEndless;
        public int EndlessTime;

        public string Motd;

        public bool IsPointMode;
        public int PointModeWin;

        public bool AllowAirAHSSReload;

        public bool AllowPunks;

        public bool EnableMap;
        public bool IsMapAllowed;

        public bool EnableChatFeed;

        public bool InSpectatorMode;

        public Color BombColor;
        public int BombRadius;
        public int BombRange;
        public int BombSpeed;
        public int BombCountdown;

        public bool AllowCannonHumanKills;

        public int MasterTextureLimit;
        public bool UseMipmap;
        
        public float Volume;
        public float CameraDistance;
        public float MouseSensitivity;
        public float GameQuality;

        public void ImportFromRC()
        {
            EnableHumanSkins = Utility.GetBoolean("human");
            EnableTitanSkins = Utility.GetBoolean("titan");
            EnableLevelSkins = Utility.GetBoolean("level");

            HumanSkin = new HumanSkin
            {
                Set = new[] 
                {
                    PlayerPrefs.GetString("horse", string.Empty),
                    PlayerPrefs.GetString("hair", string.Empty),
                    PlayerPrefs.GetString("eye", string.Empty),
                    PlayerPrefs.GetString("glass", string.Empty),
                    PlayerPrefs.GetString("face", string.Empty),
                    PlayerPrefs.GetString("skin", string.Empty),
                    PlayerPrefs.GetString("costume", string.Empty),
                    PlayerPrefs.GetString("logo", string.Empty),
                    PlayerPrefs.GetString("bladel", string.Empty),
                    PlayerPrefs.GetString("blader", string.Empty),
                    PlayerPrefs.GetString("hoodie", string.Empty),
                    PlayerPrefs.GetString("trailskin", string.Empty)
                }
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
            
            HumanSkinSelection = PlayerPrefs.GetInt("humangui", 0);

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
            IsBomb = Utility.GetBoolean("bombMode");
            IsTeam = Utility.GetBoolean("teamMode");
            IsRockThrowEnabled = Utility.GetBoolean("rockThrow");
            IsExplodeMode = Utility.GetBoolean("explodeModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("explodeModeNum", "30"), out ExplodeRadius))
                ExplodeRadius = 30;
            
            IsHealthMode = Utility.GetBoolean("healthMode");
            if (!int.TryParse(PlayerPrefs.GetString("healthLower", "100"), out var min))
                min = 100;
            if (!int.TryParse(PlayerPrefs.GetString("healthUpper", "200"), out var max))
                max = 200;
            TitanHealth = new Range(min, max);
            
            IsInfectionMode = Utility.GetBoolean("infectionModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("infectionModeNum", "1"), out InfectionStartTitan))
                InfectionStartTitan = 1;
            
            IsErenAllowed = !Utility.GetBoolean("banEren");
            DoSpawnMoreTitans = Utility.GetBoolean("moreTitanOn");
            if (!int.TryParse(PlayerPrefs.GetString("moreTitanNum", "1"), out MoreTitansNumber))
                MoreTitansNumber = 1;
            
            IsDamageMode = Utility.GetBoolean("damageModeOn");
            if (!int.TryParse(PlayerPrefs.GetString("damageModeNum", "1000"), out MinimumDamage))
                MinimumDamage = 1000;
            
            IsSizeMode = Utility.GetBoolean("sizeMode");
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
            IsHorseAllowed = Utility.GetBoolean("horseMode");
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
                IsBomb = false;
                IsTeam = false;
                IsPointMode = false;
                PVPMode = PVPMode.Off;
                
                if (InfectionStartTitan < 0 || InfectionStartTitan > PhotonNetwork.countOfPlayers)
                    InfectionStartTitan = 1;
                
                hashtable.Add("infection", InfectionStartTitan);
            }
            if (IsBomb)
                hashtable.Add("bomb", 1);
            
            if (!EnableMap)
                hashtable.Add("globalDisableMinimap", 1);
                    
            if (IsTeam)
                hashtable.Add("team", 1);
            
//                if (RCSettings.teamMode != (int) settings[193])
//                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
//                        photonView.RPC("setTeamRPC", PhotonNetwork.PlayerList[i], i % 2 + 1);
            if (IsPointMode)
                hashtable.Add("point", PointModeWin);
            
            if (IsRockThrowEnabled)
                hashtable.Add("rock", 1);
            
            if (IsExplodeMode)
                hashtable.Add("explode", ExplodeRadius);
            
            if (IsHealthMode)
            {
                hashtable.Add("healthMode", 1);
                hashtable.Add("healthLower", TitanHealth.Min);
                hashtable.Add("healthUpper", TitanHealth.Max);
            }
            
            if (!IsErenAllowed)
                hashtable.Add("eren", 1);
            
            if (DoSpawnMoreTitans)
                hashtable.Add("titanc", Titans);
            
            if (IsDamageMode)
                hashtable.Add("damage", MinimumDamage);
            
            if (IsSizeMode)
            {
                hashtable.Add("sizeMode", 1);
                hashtable.Add("sizeLower", TitanSize.Min);
                hashtable.Add("sizeUpper", TitanSize.Max);
            }
            
            if (UseCustomSpawnRates)
            {
                hashtable.Add("spawnMode", 1);
                hashtable.Add("nRate", SpawnRates.Normal.ToString(CultureInfo.InvariantCulture));
                hashtable.Add("aRate", SpawnRates.Aberrant.ToString(CultureInfo.InvariantCulture));
                hashtable.Add("jRate", SpawnRates.Jumper.ToString(CultureInfo.InvariantCulture));
                hashtable.Add("cRate", SpawnRates.Crawler.ToString(CultureInfo.InvariantCulture));
                hashtable.Add("pRate", SpawnRates.Punk.ToString(CultureInfo.InvariantCulture));
            }
            
            if (IsHorseAllowed)
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
            
            if (RCSettings.racingStatic > 0)
                hashtable.Add("asoracing", 1);
            
            return hashtable;
        }
    }
}