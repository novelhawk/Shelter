using System.Linq;
using UnityEngine;

public static class LevelInfoManager
{
    public static readonly LevelInfo[] Levels;

    static LevelInfoManager()
    {
        Levels = new[]
        {
            new LevelInfo
            {
                Name = "The City II",
                LevelName = "The City I",
                Description = "Fight the titans with your friends.(RESPAWN AFTER 10 SECONDS/SUPPLY/TEAM TITAN)",
                EnemyNumber = 10,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.DEATHMATCH,
                Supply = true,
                PlayerTitansNotAllowed = true,
                IsPvP = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "The Forest III",
                LevelName = "The Forest",
                Description = "Survive for 20 waves.player will respawn in every new wave",
                EnemyNumber = 3,
                Gamemode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEWROUND,
                Supply = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Custom",
                LevelName = "The Forest",
                Description = "Custom Map.",
                EnemyNumber = 1,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                PlayerTitansNotAllowed = true,
                IsPvP = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Custom (No PT)",
                LevelName = "The Forest",
                Description = "Custom Map (No Player Titans).",
                EnemyNumber = 1,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                IsPvP = true,
                HasPunk = true,
                Supply = true,
                PlayerTitansNotAllowed = false,
            },
            new LevelInfo
            {
                Name = "The City",
                LevelName = "The City I",
                Description = "Kill all the titans. No respawn, play as titan.",
                EnemyNumber = 10,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                PlayerTitansNotAllowed = true,
                IsPvP = true,
                MinimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 731.9738f),
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "The City III",
                LevelName = "The City I",
                Description = "Capture Checkpoint mode.",
                EnemyNumber = 0,
                Gamemode = GameMode.PvpCapture,
                RespawnMode = RespawnMode.DEATHMATCH,
                Supply = true,
                Horse = false,
                PlayerTitansNotAllowed = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Cage Fighting",
                LevelName = "Cage Fighting",
                Description =
                    "2 players in different cages. when you kill a titan,  one or more titan will spawn to your opponent's cage.",
                EnemyNumber = 1,
                Gamemode = GameMode.CaveFight,
                RespawnMode = RespawnMode.NEVER,
                HasPunk = true,
                Supply = true
            },
            new LevelInfo
            {
                Name = "The Forest",
                LevelName = "The Forest",
                Description = "The Forest Of Giant Trees.(No RESPAWN/SUPPLY/PLAY AS TITAN)",
                EnemyNumber = 5,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                PlayerTitansNotAllowed = true,
                IsPvP = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "The Forest II",
                LevelName = "The Forest",
                Description = "Survive for 20 waves.",
                EnemyNumber = 3,
                Gamemode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Annie",
                LevelName = "The Forest",
                Description =
                    "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:2500/100\nAbnormal:4000/200\nYou only have 1 life.Don't do this alone.",
                EnemyNumber = 15,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                HasPunk = false,
                IsPvP = true,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Annie II",
                LevelName = "The Forest",
                Description =
                    "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:3000/200\nAbnormal:6000/1000\n(RESPAWN AFTER 10 SECONDS)",
                EnemyNumber = 15,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.DEATHMATCH,
                HasPunk = false,
                IsPvP = true,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Colossal Titan",
                LevelName = "Colossal Titan",
                Description =
                    "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:2000\nHard:3500\nAbnormal:5000\n",
                EnemyNumber = 2,
                Gamemode = GameMode.BossFight,
                RespawnMode = RespawnMode.NEVER,
                MinimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f),
                HasPunk = true,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Colossal Titan II",
                LevelName = "Colossal Titan",
                Description =
                    "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:5000\nHard:8000\nAbnormal:12000\n(RESPAWN AFTER 10 SECONDS)",
                EnemyNumber = 2,
                Gamemode = GameMode.BossFight,
                RespawnMode = RespawnMode.DEATHMATCH,
                MinimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f),
                HasPunk = true,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Trost",
                LevelName = "Colossal Titan",
                Description = "Escort Titan Eren",
                EnemyNumber = 2,
                Gamemode = GameMode.Trost,
                RespawnMode = RespawnMode.NEVER,
                HasPunk = false,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Trost II",
                LevelName = "Colossal Titan",
                Description = "Escort Titan Eren(RESPAWN AFTER 10 SECONDS)",
                EnemyNumber = 2,
                Gamemode = GameMode.Trost,
                RespawnMode = RespawnMode.DEATHMATCH,
                HasPunk = false,
                Supply = true
            },
            new LevelInfo
            {
                Name = "[S]City",
                LevelName = "The City I",
                Description = "Kill all 15 Titans",
                EnemyNumber = 15,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "[S]Forest",
                LevelName = "The Forest",
                Description = string.Empty,
                EnemyNumber = 15,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "[S]Forest Survive(no crawler)",
                LevelName = "The Forest",
                Description = string.Empty,
                EnemyNumber = 3,
                Gamemode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                NoCrawler = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "[S]Tutorial",
                LevelName = "tutorial",
                Description = string.Empty,
                EnemyNumber = 1,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                Hint = true,
                HasPunk = false,
            },
            new LevelInfo
            {
                Name = "[S]Battle training",
                LevelName = "tutorial 1",
                Description = string.Empty,
                EnemyNumber = 7,
                Gamemode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                HasPunk = false,
            },
            new LevelInfo
            {
                Name = "The Forest IV  - LAVA",
                LevelName = "The Forest",
                Description =
                    "Survive for 20 waves.player will respawn in every new wave.\nNO CRAWLERS\n***YOU CAN'T TOUCH THE GROUND!***",
                EnemyNumber = 3,
                Gamemode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEWROUND,
                Supply = true,
                NoCrawler = true,
                IsLava = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "[S]Racing - Akina",
                LevelName = "track - akina",
                Description = string.Empty,
                EnemyNumber = 0,
                Gamemode = GameMode.Racing,
                RespawnMode = RespawnMode.NEVER,
                Supply = false,
                MinimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f),
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Racing - Akina",
                LevelName = "track - akina",
                Description = string.Empty,
                EnemyNumber = 0,
                Gamemode = GameMode.Racing,
                RespawnMode = RespawnMode.NEVER,
                Supply = false,
                IsPvP = true,
                MinimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f),
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Outside The Walls",
                LevelName = "OutSide",
                Description = "Capture Checkpoint mode.",
                EnemyNumber = 0,
                Gamemode = GameMode.PvpCapture,
                RespawnMode = RespawnMode.DEATHMATCH,
                Supply = true,
                Horse = true,
                PlayerTitansNotAllowed = true,
                MinimapPreset = new Minimap.Preset(new Vector3(2549.4f, 0f, 3042.4f), 3697.16f),
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "Cave Fight",
                LevelName = "CaveFight",
                Description = "***Spoiler Alarm!***",
                EnemyNumber = -1,
                Gamemode = GameMode.PvpAHSS,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                Horse = false,
                PlayerTitansNotAllowed = true,
                IsPvP = true,
                MinimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 734.9738f),
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "House Fight",
                LevelName = "HouseFight",
                Description = "***Spoiler Alarm!***",
                EnemyNumber = -1,
                Gamemode = GameMode.PvpAHSS,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                Horse = false,
                PlayerTitansNotAllowed = true,
                IsPvP = true,
                HasPunk = true,
            },
            new LevelInfo
            {
                Name = "[S]Forest Survive(no crawler no punk)",
                LevelName = "The Forest",
                Description = string.Empty,
                EnemyNumber = 3,
                Gamemode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                NoCrawler = true,
                HasPunk = false,
            }
        };
    }

    public static LevelInfo Get(string name) => Levels.FirstOrDefault(info => info.Name == name);

    public static bool TryGet(string name, out LevelInfo map)
    {
        foreach (var info in Levels)
        {
            if (info.Name == name)
            {
                map = info;
                return true;
            }
        }
        
        map = new LevelInfo();
        return false;
    }
}
