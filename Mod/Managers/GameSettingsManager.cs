using ExitGames.Client.Photon;
using Mod.GameSettings;
using Photon;
using UnityEngine;

namespace Mod.Managers
{
    public static class GameSettingsManager
    {
        public static void ApplySettings(Settings settings)
        {
//            if (settings.IsInfectionMode)
//            {
//                _infectionTitans.Clear();
//                int length = PhotonNetwork.PlayerList.Length;
//                for (var i = 0; i < length; i++)
//                {
//                    Player player = PhotonNetwork.PlayerList[i];
//                    Hashtable hash = new Hashtable { {PlayerProperty.IsTitan, 1} };
//                
//                    if (length > 0 && UnityEngine.Random.Range(0, 1) <= i / (float) length)
//                    {
//                        hash[PlayerProperty.IsTitan] = 2;
//                        _infectionTitans.Add(i, 2);
//                    }
//    
//                    length--;
//                    player.SetCustomProperties(hash);
//                }
//            }
//
//            if (FengGameManagerMKII.settings.TeamSort > TeamSort.Off) // Might not work
//                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
//                    photonView.RPC(Rpc.setTeamRPC, PhotonNetwork.PlayerList[i], i % 2 + 1);
            Application.targetFrameRate = settings.FPSCap;
            QualitySettings.vSyncCount = settings.EnableVSync ? 1 : 0;
            AudioListener.volume = settings.Volume;
            QualitySettings.masterTextureLimit = settings.MasterTextureLimit;
        }
        
        public static Settings ImportRCSettings()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            Settings settings = new Settings();

            settings.HumanSkin = new HumanSkin
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

            settings.TitanSkin = new TitanSkin
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

            settings.CitySkin = new CitySkin
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

            settings.ForestSkin = new ForestSkin
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

            settings.CustomSkin = new CustomMapSkin
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

            settings.Randomize = Utility.GetBoolean("titanR") || Utility.GetBoolean("forestR");
            
            settings.MasterTextureLimit = PlayerPrefs.GetInt("skinQ", 0); 
            settings.UseMipmap = !Utility.GetBoolean("skinQL");
            if (!int.TryParse(PlayerPrefs.GetString("cnumber", "1"), out settings.Titans))
                settings.Titans = 1;
            settings.RespawnTimer = 30;
            settings.SpawnTimer = 0;
            if (!int.TryParse(PlayerPrefs.GetString("cmax", "20"), out settings.CustomMaxTitans))
                settings.CustomMaxTitans = 20;
            if (!int.TryParse(PlayerPrefs.GetString("snapshot", "0"), out settings.SnapshotDamage))
                settings.SnapshotDamage = 0;
            settings.EnableVSync = Utility.GetBoolean("vsync");
            if (!int.TryParse(PlayerPrefs.GetString("fpscap", "-1"), out settings.FPSCap))
                settings.FPSCap = -1;
            settings.IsBombMode = Utility.GetBoolean("bombMode");
            settings.TeamSort = (TeamSort) PlayerPrefs.GetInt("teamMode", 0);
            settings.EnableRockThrow = Utility.GetBoolean("rockThrow");
            if (!int.TryParse(PlayerPrefs.GetString("explodeModeNum", "30"), out settings.ExplodeRadius))
                settings.ExplodeRadius = 30;
            
            settings.HealthMode = (HealthMode) PlayerPrefs.GetInt("healthMode", 0);
            if (!int.TryParse(PlayerPrefs.GetString("healthLower", "100"), out var min))
                min = 100;
            if (!int.TryParse(PlayerPrefs.GetString("healthUpper", "200"), out var max))
                max = 200;
            settings.TitanHealth = new Range(min, max);
            
            if (Utility.GetBoolean("infectionModeOn"))
                if (!int.TryParse(PlayerPrefs.GetString("infectionModeNum", "1"), out settings.InfectionTitanNumber))
                    settings.InfectionTitanNumber = 1;
            
            settings.AllowErenTitan = !Utility.GetBoolean("banEren");
            if (Utility.GetBoolean("moreTitanOn"))
                if (!int.TryParse(PlayerPrefs.GetString("moreTitanNum", "1"), out settings.MoreTitansNumber))
                    settings.MoreTitansNumber = 1;
            
            if (Utility.GetBoolean("damageModeOn"))
                if (!int.TryParse(PlayerPrefs.GetString("damageModeNum", "1000"), out settings.MinimumDamage))
                    settings.MinimumDamage = 1000;

            if (Utility.GetBoolean("sizeMode"))
            {
                if (!int.TryParse(PlayerPrefs.GetString("sizeLower", "1.0"), out min))
                    min = 3;
                if (!int.TryParse(PlayerPrefs.GetString("sizeUpper", "3.0"), out max))
                    max = 3;
                settings.TitanSize = new Range(min, max);
            }

            if (Utility.GetBoolean("spawnModeOn"))
            {
                settings.SpawnRates = new SpawnRates(
                    PlayerPrefs.GetString("nRate", "20.0"),
                    PlayerPrefs.GetString("aRate", "20.0"),
                    PlayerPrefs.GetString("jRate", "20.0"),
                    PlayerPrefs.GetString("cRate", "20.0"),
                    PlayerPrefs.GetString("pRate", "20.0"));
            }

            settings.EnableHorse = Utility.GetBoolean("horseMode");
            if (Utility.GetBoolean("waveModeOn"))
                if (!int.TryParse(PlayerPrefs.GetString("waveModeNum", "1"), out settings.WaveNumber))
                    settings.WaveNumber = 1;
            settings.IsFriendlyMode = Utility.GetBoolean("friendlyMode");
            settings.PVPMode = (PVPMode) PlayerPrefs.GetInt("pvpMode", 0);
            if (Utility.GetBoolean("maxWaveOn"))
                if (!int.TryParse(PlayerPrefs.GetString("maxWaveNum", "20"), out settings.MaxWaveNumber))
                    settings.MaxWaveNumber = 20;
            if (Utility.GetBoolean("endlessModeOn"))
                if (!int.TryParse(PlayerPrefs.GetString("endlessModeNum", "1"), out settings.EndlessTime))
                    settings.EndlessTime = 1;
            settings.Motd = PlayerPrefs.GetString("motd", string.Empty);
            if (Utility.GetBoolean("pointModeOn"))
                if (!int.TryParse(PlayerPrefs.GetString("pointModeNum", "50"), out settings.PointModeWin))
                    settings.PointModeWin = 50;

            settings.IsASORacing = Utility.GetBoolean("asoracing");
            settings.AllowAirAHSSReload = Utility.GetBoolean("ahssReload");
            settings.AllowPunks = Utility.GetBoolean("punkWaves");
            settings.IsMapAllowed = !Utility.GetBoolean("globalDisableMinimap");
            settings.EnableChatFeed = Utility.GetBoolean("chatfeed");
            settings.InSpectatorMode = false;
            settings.BombColor = new Color(
                PlayerPrefs.GetFloat("bombR", 1f),
                PlayerPrefs.GetFloat("bombG", 1f),
                PlayerPrefs.GetFloat("bombB", 1f),
                PlayerPrefs.GetFloat("bombA", 1f));
            settings.BombRadius = PlayerPrefs.GetInt("bombRadius", 5);
            settings.BombRange = PlayerPrefs.GetInt("bombRange", 5);
            settings.BombSpeed = PlayerPrefs.GetInt("bombSpeed", 5);
            settings.BombCountdown = PlayerPrefs.GetInt("bombCD", 5);
            settings.AllowCannonHumanKills = Utility.GetBoolean("deadlyCannon");
            settings.Volume = PlayerPrefs.GetFloat("vol", 1f);
            settings.CameraDistance = PlayerPrefs.GetFloat("cameraDistance", 1f);
            settings.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
            settings.GameQuality = PlayerPrefs.GetFloat("GameQuality", 0f);
            return settings;
        }
        
        public static Hashtable EncodeToHashtable(Settings settings)
        {
            Hashtable hashtable = new Hashtable();
            if (settings.IsInfectionMode)
            {
                settings.IsBombMode = false;
                settings.PointModeWin = 0;
                settings.TeamSort = TeamSort.Off;
                settings.PVPMode = PVPMode.Off;
                
                if (settings.InfectionTitanNumber < 0 || settings.InfectionTitanNumber > PhotonNetwork.countOfPlayers)
                    settings.InfectionTitanNumber = 1;
                
                hashtable.Add("infection", settings.InfectionTitanNumber);
            }
            if (settings.IsBombMode)
                hashtable.Add("bomb", 1);
            
            if (!settings.IsMapAllowed)
                hashtable.Add("globalDisableMinimap", 1);
                    
            if (settings.TeamSort != TeamSort.Off)
                hashtable.Add("team", (int) settings.TeamSort);
            
//                if (RCSettings.teamMode != (int) settings[193])
//                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
//                        photonView.RPC(Rpc.setTeamRPC, PhotonNetwork.PlayerList[i], i % 2 + 1);
            if (settings.IsPointMode)
                hashtable.Add("point", settings.PointModeWin);
            
            if (settings.EnableRockThrow)
                hashtable.Add("rock", 1);
            
            if (settings.IsExplodeMode)
                hashtable.Add("explode", settings.ExplodeRadius);
            
            if (settings.HealthMode > HealthMode.Off)
            {
                hashtable.Add("healthMode", (int) settings.HealthMode);
                hashtable.Add("healthLower", (int) settings.TitanHealth.Min);
                hashtable.Add("healthUpper", (int) settings.TitanHealth.Max);
            }
            
            if (!settings.AllowErenTitan)
                hashtable.Add("eren", 1);
            
            if (settings.SpawnMoreTitans)
                hashtable.Add("titanc", settings.Titans);
            
            if (settings.IsDamageMode)
                hashtable.Add("damage", settings.MinimumDamage);
            
            if (settings.EnableCustomSize)
            {
                hashtable.Add("sizeMode", 1);
                hashtable.Add("sizeLower", settings.TitanSize.Min);
                hashtable.Add("sizeUpper", settings.TitanSize.Max);
            }
            
            if (settings.UseCustomSpawnRates)
            {
                hashtable.Add("spawnMode", 1);
                hashtable.Add("nRate", settings.SpawnRates.Normal);
                hashtable.Add("aRate", settings.SpawnRates.Aberrant);
                hashtable.Add("jRate", settings.SpawnRates.Jumper);
                hashtable.Add("cRate", settings.SpawnRates.Crawler);
                hashtable.Add("pRate", settings.SpawnRates.Punk);
            }
            
            if (settings.EnableHorse)
                hashtable.Add("horse", 1);
            
            if (settings.IsWaveMode)
            {
                hashtable.Add("waveModeOn", 1);
                hashtable.Add("waveModeNum", settings.WaveNumber);
            }
            
            if (settings.IsFriendlyMode)
                hashtable.Add("friendly", 1);
            
            if (settings.PVPMode != PVPMode.Off)
                hashtable.Add("pvp", (int) settings.PVPMode);
            
            if (settings.IsMaxWaveMode)
                hashtable.Add("maxwave", settings.MaxWaveNumber);
            
            if (settings.IsEndless)
                hashtable.Add("endless", settings.EndlessTime);
            
            if (!string.IsNullOrEmpty(settings.Motd))
                hashtable.Add("motd", settings.Motd);
            
            if (settings.AllowAirAHSSReload)
                hashtable.Add("ahssReload", 1);
            
            if (settings.AllowPunks)
                hashtable.Add("punkWaves", 1);
            
            if (settings.AllowCannonHumanKills)
                hashtable.Add("deadlycannons", 1);
            
            if (settings.IsASORacing)
                hashtable.Add("asoracing", 1);
            
            return hashtable;
        }

        public static Settings DecodeFromHashtable(Hashtable hash)
        {
            Settings settings = new Settings();
            if (hash.ContainsKey("bomb"))
                settings.IsBombMode = true;

            settings.IsMapAllowed = true;
            if (hash.ContainsKey("globalDisableMinimap"))
                settings.IsMapAllowed = !hash.ToBool("globalDisableMinimap");
            if (hash.ContainsKey("horse"))
                settings.EnableHorse = hash.ToBool("horse");
            if (hash.ContainsKey("punkWaves"))
                settings.AllowPunks = hash.ToBool("punkWaves");
            if (hash.ContainsKey("ahssReload"))
                settings.AllowAirAHSSReload = hash.ToBool("ahssReload");
            if (hash.ContainsKey("team"))
                settings.TeamSort = (TeamSort) (int) hash["team"];
            if (hash.ContainsKey("point"))
                settings.PointModeWin = (int) hash["point"];
            if (hash.ContainsKey("rock"))
                settings.EnableRockThrow = hash.ToBool("rock");
            if (hash.ContainsKey("explode"))
                settings.ExplodeRadius = (int) hash["explode"];
            if (hash.ContainsKey("healthMode") && 
                hash.ContainsKey("healthLower") && 
                hash.ContainsKey("healthUpper"))
            {
                if ((settings.HealthMode = (HealthMode) (int) hash["healthMode"]) > HealthMode.Off)
                {
                    settings.TitanHealth = new Range(
                        (int) hash["healthLower"],
                        (int) hash["healthUpper"]);
                }
            }
            if (hash.ContainsKey("infection"))
                settings.InfectionTitanNumber = (int) hash["infection"];

            settings.AllowErenTitan = true;
            if (hash.ContainsKey("eren"))
                settings.AllowErenTitan = !hash.ToBool("eren");

            if (hash.ContainsKey("titanc"))
                settings.MoreTitansNumber = (int) hash["titanc"];

            if (hash.ContainsKey("damage"))
                settings.MinimumDamage = (int) hash["damage"];
            
            if (hash.ContainsKey("sizeMode") && 
                hash.ContainsKey("sizeLower") && 
                hash.ContainsKey("sizeUpper"))
            {
                settings.TitanSize = new Range(
                    (float) hash["sizeLower"],
                    (float) hash["sizeUpper"]);
            }

            if (hash.ContainsKey("spawnMode") && hash.ContainsKey("nRate") && hash.ContainsKey("aRate") &&
                hash.ContainsKey("jRate") && hash.ContainsKey("cRate") && hash.ContainsKey("pRate"))
            {
                settings.SpawnRates = new SpawnRates
                (
                    (float) hash["nRate"],
                    (float) hash["aRate"],
                    (float) hash["jRate"],
                    (float) hash["cRate"],
                    (float) hash["pRate"]
                );
            }

            if (hash.ContainsKey("waveModeOn") && hash.ContainsKey("waveModeNum"))
                settings.WaveNumber = (int) hash["waveModeNum"];

            if (hash.ContainsKey("friendly"))
                settings.IsFriendlyMode = hash.ToBool("friendly");

            if (hash.ContainsKey("pvp"))
                settings.PVPMode = (PVPMode) (int) hash["pvp"];

            if (hash.ContainsKey("maxwave"))
                settings.MaxWaveNumber = (int) hash["maxwave"];

            if (hash.ContainsKey("endless"))
                settings.EndlessTime = (int) hash["endless"];

            if (hash.ContainsKey("motd"))
                settings.Motd = (string) hash["motd"];
            
            if (hash.ContainsKey("deadlycannons"))
                settings.AllowCannonHumanKills = hash.ToBool("deadlycannons");

            if (hash.ContainsKey("asoracing"))
                settings.IsASORacing = true;

            return settings;
        }
    }
}