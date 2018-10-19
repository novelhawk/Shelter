using System;
using Photon;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class ServerTime : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, 0f, 200f, 30f));
        GUILayout.Label(string.Format("Time Offset: {0}", PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds - Environment.TickCount), new GUILayoutOption[0]);
        if (GUILayout.Button("fetch", new GUILayoutOption[0]))
        {
            PhotonNetwork.FetchServerTimestamp();
        }
        GUILayout.EndArea();
    }
}

