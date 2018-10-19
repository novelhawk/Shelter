using Mod.GameSettings;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRules : Command
    {
        public override string CommandName => "rules";

        public override void Execute(string[] args)
        {
            Chat.System("Currently activated gamemodes:");
            if (FengGameManagerMKII.settings.IsBombMode)
                Chat.System("Bomb mode is on.");
            if (FengGameManagerMKII.settings.TeamSort > 0)
                Chat.System("Team mode is on {0}.",
                    FengGameManagerMKII.settings.TeamSort == TeamSort.NoSort ? 
                        "no sort" : 
                        FengGameManagerMKII.settings.TeamSort == TeamSort.SizeSort ? 
                            "sort by size" : 
                            "sort by skill");
            if (FengGameManagerMKII.settings.IsPointMode)
                Chat.System("Point mode is on {0}.", FengGameManagerMKII.settings.PointModeWin);
            if (!FengGameManagerMKII.settings.EnableRockThrow)
                Chat.System("Punk Rock-Throwing is disabled.");
            if (FengGameManagerMKII.settings.UseCustomSpawnRates)
                Chat.System("Spawnrates: {0:F2}% Normal; {1:F2}% Abnormal; {2:F2}% Jumper; {3:F2}% Crawler; {4:F2}% Punk.", 
                    FengGameManagerMKII.settings.SpawnRates.Normal,
                    FengGameManagerMKII.settings.SpawnRates.Aberrant,
                    FengGameManagerMKII.settings.SpawnRates.Jumper,
                    FengGameManagerMKII.settings.SpawnRates.Crawler,
                    FengGameManagerMKII.settings.SpawnRates.Punk);
            if (FengGameManagerMKII.settings.IsExplodeMode)
                Chat.System("Titan explode mode is on {0}.", FengGameManagerMKII.settings.ExplodeRadius);
            if (FengGameManagerMKII.settings.HealthMode > HealthMode.Off)
                Chat.System("Titan health mode is on {0}.", FengGameManagerMKII.settings.TitanHealth);
            if (FengGameManagerMKII.settings.IsInfectionMode)
                Chat.System("Infection mode is on {0}.", FengGameManagerMKII.settings.InfectionTitanNumber);
            if (FengGameManagerMKII.settings.IsDamageMode)
                Chat.System("Minimum nape damage is on {0}.", FengGameManagerMKII.settings.MinimumDamage);
            if (FengGameManagerMKII.settings.SpawnMoreTitans)
                Chat.System("Custom titan # is on {0}.", FengGameManagerMKII.settings.MoreTitansNumber);
            if (FengGameManagerMKII.settings.EnableCustomSize)
                Chat.System("Custom titan size is on {0}.", FengGameManagerMKII.settings.TitanSize);
            if (!FengGameManagerMKII.settings.AllowErenTitan)
                Chat.System("Anti-Eren is on. Using Titan eren will get you kicked.");
            if (FengGameManagerMKII.settings.IsMaxWaveMode)
                Chat.System("Custom wave mode is on {0}.", FengGameManagerMKII.settings.MaxWaveNumber);
            if (FengGameManagerMKII.settings.IsFriendlyMode)
                Chat.System("Friendly-Fire disabled. PVP is prohibited.");
            if (FengGameManagerMKII.settings.PVPMode > PVPMode.Off)
                Chat.System("AHSS/Blade PVP is on {0}.", FengGameManagerMKII.settings.PVPMode == PVPMode.Teams ? "team-based" : "FFA");
            if (FengGameManagerMKII.settings.IsMaxWaveMode)
                Chat.System("Max Wave set to {0}.", FengGameManagerMKII.settings.MaxWaveNumber);
            if (FengGameManagerMKII.settings.EnableHorse)
                Chat.System("Horses are enabled.");
            if (FengGameManagerMKII.settings.AllowAirAHSSReload)
                Chat.System("AHSS Air-Reload disabled.");
            if (FengGameManagerMKII.settings.AllowPunks)
                Chat.System("Punk override every 5 waves enabled.");
            if (FengGameManagerMKII.settings.IsEndless)
                Chat.System("Endless Respawn is enabled {0} seconds.", FengGameManagerMKII.settings.EndlessTime);
            if (!FengGameManagerMKII.settings.IsMapAllowed)
                Chat.System("Minimaps are disabled.");
            if (FengGameManagerMKII.settings.Motd != string.Empty)
                Chat.System("Motd <color=#FF0000>{0}</color>", FengGameManagerMKII.settings.Motd);
            if (FengGameManagerMKII.settings.AllowCannonHumanKills)
                Chat.System("Cannons kill humans.");
        }
    }
}
