using ExitGames.Client.Photon;

internal static class ScoreExtensions
{
    public static void AddScore(this Player player, int scoreToAddToCurrent)
    {
        int num = player.GetScore() + scoreToAddToCurrent;
        Hashtable propertiesToSet = new Hashtable
        {
            ["score"] = num
        };
        player.SetCustomProperties(propertiesToSet);
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
        Hashtable propertiesToSet = new Hashtable
        {
            ["score"] = newScore
        };
        player.SetCustomProperties(propertiesToSet);
    }
}

