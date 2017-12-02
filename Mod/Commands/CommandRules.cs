using Mod.Interface;

namespace Mod.Commands
{
    public class CommandRules : Command
    {
        public override string CommandName => "rules";

        public override void Execute(string[] args)
        {
            Chat.System("Currently activated gamemodes:");
            if (RCSettings.bombMode > 0)
                Chat.System("Bomb mode is on.");
            if (RCSettings.teamMode > 0)
                Chat.System($"Team mode is on {(RCSettings.teamMode == 1 ? "no sort" : RCSettings.teamMode == 2 ? "sort by size" : "sort by skill")}.");
            if (RCSettings.pointMode > 0)
                Chat.System($"Point mode is on {RCSettings.pointMode}.");
            if (RCSettings.disableRock > 0)
                Chat.System("Punk Rock-Throwing is disabled.");
            if (RCSettings.spawnMode > 0)
                Chat.System($"Custom spawn rate is on {RCSettings.nRate:F2}% Normal, {RCSettings.aRate:F2}% Abnormal, {RCSettings.jRate:F2}% Jumper, {RCSettings.cRate:F2}% Crawler, {RCSettings.pRate:F2}% Punk ");
            if (RCSettings.explodeMode > 0)
                Chat.System($"Titan explode mode is on {RCSettings.explodeMode}.");
            if (RCSettings.healthMode > 0)
                Chat.System($"Titan health mode is on {RCSettings.healthLower}-{RCSettings.healthUpper}.");
            if (RCSettings.infectionMode > 0)
                Chat.System($"Infection mode is on {RCSettings.infectionMode}.");
            if (RCSettings.damageMode > 0)
                Chat.System($"Minimum nape damage is on {RCSettings.damageMode}.");
            if (RCSettings.moreTitans > 0)
                Chat.System($"Custom titan # is on {RCSettings.moreTitans}.");
            if (RCSettings.sizeMode > 0)
                Chat.System($"Custom titan size is on {RCSettings.sizeLower:F2},{RCSettings.sizeUpper:F2}.");
            if (RCSettings.banEren > 0)
                Chat.System("Anti-Eren is on. Using Titan eren will get you kicked.");
            if (RCSettings.waveModeOn == 1)
                Chat.System($"Custom wave mode is on {RCSettings.waveModeNum}.");
            if (RCSettings.friendlyMode > 0)
                Chat.System("Friendly-Fire disabled. PVP is prohibited.");
            if (RCSettings.pvpMode > 0)
                Chat.System($"AHSS/Blade PVP is on {(RCSettings.pvpMode == 1 ? "team-based" : "FFA")}.");
            if (RCSettings.maxWave > 0)
                Chat.System($"Max Wave set to {RCSettings.maxWave}");
            if (RCSettings.horseMode > 0)
                Chat.System("Horses are enabled.");
            if (RCSettings.ahssReload > 0)
                Chat.System("AHSS Air-Reload disabled.");
            if (RCSettings.punkWaves > 0)
                Chat.System("Punk override every 5 waves enabled.");
            if (RCSettings.endlessMode > 0)
                Chat.System($"Endless Respawn is enabled {RCSettings.endlessMode} seconds.");
            if (RCSettings.globalDisableMinimap > 0)
                Chat.System("Minimaps are disabled.");
            if (RCSettings.motd != string.Empty)
                Chat.System($"Motd <color=#FF0000>{RCSettings.motd}</color>");
            if (RCSettings.deadlyCannons > 0)
                Chat.System("Cannons kill humans.");
        }
    }
}
