using Mod.GameSettings;

namespace Mod.Commands
{
    public class CommandRules : Command
    {
        public override string CommandName => "rules";

        public override void Execute(string[] args)
        {
            Shelter.Chat.System("Currently activated gamemodes:");
            if (GameManager.settings.IsBombMode)
                Shelter.Chat.System("Bomb mode is on.");
            if (GameManager.settings.TeamSort > 0)
                Shelter.Chat.System("Team mode is on {0}.",
                    GameManager.settings.TeamSort == TeamSort.NoSort ? 
                        "no sort" : 
                        GameManager.settings.TeamSort == TeamSort.SizeSort ? 
                            "sort by size" : 
                            "sort by skill");
            if (GameManager.settings.IsPointMode)
                Shelter.Chat.System("Point mode is on {0}.", GameManager.settings.PointModeWin);
            if (!GameManager.settings.EnableRockThrow)
                Shelter.Chat.System("Punk Rock-Throwing is disabled.");
            if (GameManager.settings.UseCustomSpawnRates)
                Shelter.Chat.System("Spawnrates: {0:F2}% Normal; {1:F2}% Abnormal; {2:F2}% Jumper; {3:F2}% Crawler; {4:F2}% Punk.", 
                    GameManager.settings.SpawnRates.Normal,
                    GameManager.settings.SpawnRates.Aberrant,
                    GameManager.settings.SpawnRates.Jumper,
                    GameManager.settings.SpawnRates.Crawler,
                    GameManager.settings.SpawnRates.Punk);
            if (GameManager.settings.IsExplodeMode)
                Shelter.Chat.System("Titan explode mode is on {0}.", GameManager.settings.ExplodeRadius);
            if (GameManager.settings.HealthMode > HealthMode.Off)
                Shelter.Chat.System("Titan health mode is on {0}.", GameManager.settings.TitanHealth);
            if (GameManager.settings.IsInfectionMode)
                Shelter.Chat.System("Infection mode is on {0}.", GameManager.settings.InfectionTitanNumber);
            if (GameManager.settings.IsDamageMode)
                Shelter.Chat.System("Minimum nape damage is on {0}.", GameManager.settings.MinimumDamage);
            if (GameManager.settings.SpawnMoreTitans)
                Shelter.Chat.System("Custom titan # is on {0}.", GameManager.settings.MoreTitansNumber);
            if (GameManager.settings.EnableCustomSize)
                Shelter.Chat.System("Custom titan size is on {0}.", GameManager.settings.TitanSize);
            if (!GameManager.settings.AllowErenTitan)
                Shelter.Chat.System("Anti-Eren is on. Using Titan eren will get you kicked.");
            if (GameManager.settings.IsMaxWaveMode)
                Shelter.Chat.System("Custom wave mode is on {0}.", GameManager.settings.MaxWaveNumber);
            if (GameManager.settings.IsFriendlyMode)
                Shelter.Chat.System("Friendly-Fire disabled. PVP is prohibited.");
            if (GameManager.settings.PVPMode > PVPMode.Off)
                Shelter.Chat.System("AHSS/Blade PVP is on {0}.", GameManager.settings.PVPMode == PVPMode.Teams ? "team-based" : "FFA");
            if (GameManager.settings.IsMaxWaveMode)
                Shelter.Chat.System("Max Wave set to {0}.", GameManager.settings.MaxWaveNumber);
            if (GameManager.settings.EnableHorse)
                Shelter.Chat.System("Horses are enabled.");
            if (GameManager.settings.AllowAirAHSSReload)
                Shelter.Chat.System("AHSS Air-Reload disabled.");
            if (GameManager.settings.AllowPunks)
                Shelter.Chat.System("Punk override every 5 waves enabled.");
            if (GameManager.settings.IsEndless)
                Shelter.Chat.System("Endless Respawn is enabled {0} seconds.", GameManager.settings.EndlessTime);
            if (!GameManager.settings.IsMapAllowed)
                Shelter.Chat.System("Minimaps are disabled.");
            if (GameManager.settings.Motd != string.Empty)
                Shelter.Chat.System("Motd <color=#FF0000>{0}</color>", GameManager.settings.Motd);
            if (GameManager.settings.AllowCannonHumanKills)
                Shelter.Chat.System("Cannons kill humans.");
        }
    }
}
