using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelInfoManager
{
    public static readonly List<LevelInfo> Levels = new List<LevelInfo>();

    public static void Initialize()
    {
        Levels.AddRange(new[] {
            new LevelInfo
            {
                Name = "The City II",
                Map = "The City I",
                Description = "Fight the titans with your friends.(RESPAWN AFTER 10 SECONDS/SUPPLY/TEAM TITAN)",
                EnemyNumber = 10,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.DEATHMATCH,
                Supply = true,
                PlayerTitansAllowed = true,
                IsPvP = true
            },
            new LevelInfo
            {
                Name = "The Forest III",
                Map = "The Forest",
                Description = "Survive for 20 waves.player will respawn in every new wave",
                EnemyNumber = 3,
                Gamemode = GAMEMODE.SURVIVE_MODE,
                RespawnMode = RespawnMode.NEWROUND,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Custom",
                Map = "The Forest",
                Description = "Custom Map.",
                EnemyNumber = 1,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                PlayerTitansAllowed = true,
                IsPvP = true,
                HasPunk = true
            },
            new LevelInfo
            {
                Name = "Custom (No PT)",
                Map = "The Forest",
                Description = "Custom Map (No Player Titans).",
                EnemyNumber = 1,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                IsPvP = true,
                HasPunk = true,
                Supply = true,
                PlayerTitansAllowed = false
            },
            new LevelInfo
            {
                Name = "The City",
                Map = "The City I",
                Description = "Kill all the titans. No respawn, play as titan.",
                EnemyNumber = 10,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                PlayerTitansAllowed = true,
                IsPvP = true,
                MinimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 731.9738f)
            },
            new LevelInfo
            {
                Name = "The City III",
                Map = "The City I",
                Description = "Capture Checkpoint mode.",
                EnemyNumber = 0,
                Gamemode = GAMEMODE.PVP_CAPTURE,
                RespawnMode = RespawnMode.DEATHMATCH,
                Supply = true,
                Horse = false,
                PlayerTitansAllowed = true
            },
            new LevelInfo
            {
                Name = "Cage Fighting",
                Map = "Cage Fighting",
                Description =
                    "2 players in different cages. when you kill a titan,  one or more titan will spawn to your opponent's cage.",
                EnemyNumber = 1,
                Gamemode = GAMEMODE.CAGE_FIGHT,
                RespawnMode = RespawnMode.NEVER
            },
            new LevelInfo
            {
                Name = "The Forest",
                Map = "The Forest",
                Description = "The Forest Of Giant Trees.(No RESPAWN/SUPPLY/PLAY AS TITAN)",
                EnemyNumber = 5,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                PlayerTitansAllowed = true,
                IsPvP = true
            },
            new LevelInfo
            {
                Name = "The Forest II",
                Map = "The Forest",
                Description = "Survive for 20 waves.",
                EnemyNumber = 3,
                Gamemode = GAMEMODE.SURVIVE_MODE,
                RespawnMode = RespawnMode.NEVER,
                Supply = true
            },
            new LevelInfo
            {
                Name = "Annie",
                Map = "The Forest",
                Description =
                    "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:2500/100\nAbnormal:4000/200\nYou only have 1 life.Don't do this alone.",
                EnemyNumber = 15,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                HasPunk = false,
                IsPvP = true
            },
            new LevelInfo
            {
                Name = "Annie II",
                Map = "The Forest",
                Description =
                    "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:3000/200\nAbnormal:6000/1000\n(RESPAWN AFTER 10 SECONDS)",
                EnemyNumber = 15,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.DEATHMATCH,
                HasPunk = false,
                IsPvP = true
            },
            new LevelInfo
            {
                Name = "Colossal Titan",
                Map = "Colossal Titan",
                Description =
                    "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:2000\nHard:3500\nAbnormal:5000\n",
                EnemyNumber = 2,
                Gamemode = GAMEMODE.BOSS_FIGHT_CT,
                RespawnMode = RespawnMode.NEVER,
                MinimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f)
            },
            new LevelInfo
            {
                Name = "Colossal Titan II",
                Map = "Colossal Titan",
                Description =
                    "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:5000\nHard:8000\nAbnormal:12000\n(RESPAWN AFTER 10 SECONDS)",
                EnemyNumber = 2,
                Gamemode = GAMEMODE.BOSS_FIGHT_CT,
                RespawnMode = RespawnMode.DEATHMATCH,
                MinimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f)
            },
            new LevelInfo
            {
                Name = "Trost",
                Map = "Colossal Titan",
                Description = "Escort Titan Eren",
                EnemyNumber = 2,
                Gamemode = GAMEMODE.TROST,
                RespawnMode = RespawnMode.NEVER,
                HasPunk = false
            },
            new LevelInfo
            {
                Name = "Trost II",
                Map = "Colossal Titan",
                Description = "Escort Titan Eren(RESPAWN AFTER 10 SECONDS)",
                EnemyNumber = 2,
                Gamemode = GAMEMODE.TROST,
                RespawnMode = RespawnMode.DEATHMATCH,
                HasPunk = false
            },
            new LevelInfo
            {
                Name = "[S]City",
                Map = "The City I",
                Description = "Kill all 15 Titans",
                EnemyNumber = 15,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true
            },
            new LevelInfo
            {
                Name = "[S]Forest",
                Map = "The Forest",
                Description = string.Empty,
                EnemyNumber = 15,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true
            },
            new LevelInfo
            {
                Name = "[S]Forest Survive(no crawler)",
                Map = "The Forest",
                Description = string.Empty,
                EnemyNumber = 3,
                Gamemode = GAMEMODE.SURVIVE_MODE,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                NoCrawler = true,
                HasPunk = true
            },
            new LevelInfo
            {
                Name = "[S]Tutorial",
                Map = "tutorial",
                Description = string.Empty,
                EnemyNumber = 1,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                Hint = true,
                HasPunk = false
            },
            new LevelInfo
            {
                Name = "[S]Battle training",
                Map = "tutorial 1",
                Description = string.Empty,
                EnemyNumber = 7,
                Gamemode = GAMEMODE.KILL_TITAN,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                HasPunk = false
            },
            new LevelInfo
            {
                Name = "The Forest IV  - LAVA",
                Map = "The Forest",
                Description =
                    "Survive for 20 waves.player will respawn in every new wave.\nNO CRAWLERS\n***YOU CAN'T TOUCH THE GROUND!***",
                EnemyNumber = 3,
                Gamemode = GAMEMODE.SURVIVE_MODE,
                RespawnMode = RespawnMode.NEWROUND,
                Supply = true,
                NoCrawler = true,
                IsLava = true
            },
            new LevelInfo
            {
                Name = "[S]Racing - Akina",
                Map = "track - akina",
                Description = string.Empty,
                EnemyNumber = 0,
                Gamemode = GAMEMODE.RACING,
                RespawnMode = RespawnMode.NEVER,
                Supply = false,
                MinimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f)
            },
            new LevelInfo
            {
                Name = "Racing - Akina",
                Map = "track - akina",
                Description = string.Empty,
                EnemyNumber = 0,
                Gamemode = GAMEMODE.RACING,
                RespawnMode = RespawnMode.NEVER,
                Supply = false,
                IsPvP = true,
                MinimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f)
            },
            new LevelInfo
            {
                Name = "Outside The Walls",
                Map = "OutSide",
                Description = "Capture Checkpoint mode.",
                EnemyNumber = 0,
                Gamemode = GAMEMODE.PVP_CAPTURE,
                RespawnMode = RespawnMode.DEATHMATCH,
                Supply = true,
                Horse = true,
                PlayerTitansAllowed = true,
                MinimapPreset = new Minimap.Preset(new Vector3(2549.4f, 0f, 3042.4f), 3697.16f)
            },
            new LevelInfo
            {
                Name = "Cave Fight",
                Map = "CaveFight",
                Description = "***Spoiler Alarm!***",
                EnemyNumber = -1,
                Gamemode = GAMEMODE.PVP_AHSS,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                Horse = false,
                PlayerTitansAllowed = true,
                IsPvP = true,
                MinimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 734.9738f)
            },
            new LevelInfo
            {
                Name = "House Fight",
                Map = "HouseFight",
                Description = "***Spoiler Alarm!***",
                EnemyNumber = -1,
                Gamemode = GAMEMODE.PVP_AHSS,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                Horse = false,
                PlayerTitansAllowed = true,
                IsPvP = true
            },
            new LevelInfo
            {
                Name = "[S]Forest Survive(no crawler no punk)",
                Map = "The Forest",
                Description = string.Empty,
                EnemyNumber = 3,
                Gamemode = GAMEMODE.SURVIVE_MODE,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                NoCrawler = true,
                HasPunk = false
            }
        });
    }

    public static LevelInfo GetInfo(string name) => Levels.FirstOrDefault(info => info.Name == name);
}
