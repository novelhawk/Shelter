using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
    public static readonly Dictionary<Team, List<Player>> PlayersPerTeam = new Dictionary<Team, List<Player>>();

    public void OnJoinedRoom()
    {
        this.UpdateTeams();
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        this.UpdateTeams();
    }

    public void Start()
    {
        PlayersPerTeam.Clear(); //Just in case of multiple times called Start() (there was the initializer here)
        IEnumerator enumerator = Enum.GetValues(typeof(Team)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
                if (enumerator.Current != null)
                    PlayersPerTeam[(Team)enumerator.Current] = new List<Player>();
        }
        finally
        {
            if (enumerator is IDisposable disposable)
            {
            	disposable.Dispose();
            }
        }
    }

    public void UpdateTeams()
    {
        IEnumerator enumerator = Enum.GetValues(typeof(Team)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
                if (enumerator.Current != null)
                    PlayersPerTeam[(Team)enumerator.Current].Clear();
        }
        finally
        {
            if (enumerator is IDisposable disposable)
            	disposable.Dispose();
        }
        foreach (Player player in PhotonNetwork.playerList)
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

