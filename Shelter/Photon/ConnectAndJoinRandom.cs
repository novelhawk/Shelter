using Photon;
using Photon.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class ConnectAndJoinRandom : Photon.MonoBehaviour
{
    public bool AutoConnect = true;
    private bool ConnectInUpdate = true;

    public void OnConnectedToMaster()
    {
        if (PhotonNetwork.networkingPeer.AvailableRegions != null)
        {
            Debug.LogWarning(string.Concat("List of available regions counts ", PhotonNetwork.networkingPeer.AvailableRegions.Count, ". First: ", PhotonNetwork.networkingPeer.AvailableRegions[0], " \t Current Region: ", PhotonNetwork.networkingPeer.CloudRegion));
        }
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedLobby()
    {
    }

    public void OnJoinedRoom()
    {
    }

    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        RoomOptions roomOptions = new RoomOptions {
            MaxPlayers = 4
        };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    public void Start()
    {
        PhotonNetwork.autoJoinLobby = false;
    }

    public void Update()
    {
        if (this.ConnectInUpdate && this.AutoConnect && !PhotonNetwork.connected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
            this.ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings("2." + Application.loadedLevel);
        }
    }
}

