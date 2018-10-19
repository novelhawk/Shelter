using ExitGames.Client.Photon;
using Mod;

namespace Photon
{
    internal static class ScoreExtensions
    {
        public static void AddScore(this Player player, int scoreToAddToCurrent)
        {
            int num = player.GetScore() + scoreToAddToCurrent;
            player.SetCustomProperties(new Hashtable
            {
                ["score"] = num
            });
        }

        public static int GetScore(this Player player)
        {
            object obj2;
            if (player.Properties.TryGetValue("score", out obj2))
            {
                return (int) obj2;
            }
            return 0;
        }

        public static void SetScore(this Player player, int newScore)
        {
            player.SetCustomProperties(new Hashtable
            {
                ["score"] = newScore
            });
        }
    }
}

