using ExitGames.Client.Photon;
using UnityEngine;

internal static class TeamExtensions
{
    public static PunTeams.Team GetTeam(this Player player)
    {
        object obj2;
        if (player.Properties.TryGetValue("team", out obj2))
        {
            return (PunTeams.Team) (byte) obj2;
        }
        return PunTeams.Team.None;
    }

    public static void SetTeam(this Player player, PunTeams.Team team)
    {
        if (!PhotonNetwork.connectedAndReady)
        {
            Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.connectionStatesDetailed + ". Not connectedAndReady.");
        }
        if (Player.Self.GetTeam() != team)
        {
            Player.Self.SetCustomProperties(new Hashtable
            {
                { "team", (byte)team }
            });
        }
    }
}

