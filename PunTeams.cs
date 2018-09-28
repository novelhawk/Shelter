using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
    private static readonly Dictionary<Team, List<Player>> PlayersPerTeam = new Dictionary<Team, List<Player>>();

    public void OnJoinedRoom()
    {
        UpdateTeams();
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        UpdateTeams();
    }

    public void Start()
    {
        PlayersPerTeam.Clear(); //Just in case of multiple times called Start() (there was the initializer here)
        foreach (Team current in Enum.GetValues(typeof(Team)))
            PlayersPerTeam[current] = new List<Player>();
    }

    public static void UpdateTeams()
    {
        foreach (Team team in Enum.GetValues(typeof(Team)))
            PlayersPerTeam[team].Clear();
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Team team = player.GetTeam();
            PlayersPerTeam[team].Add(player);
        }
    }

    public enum Team : byte
    {
        None,
        Red,
        Blue
    }
}

