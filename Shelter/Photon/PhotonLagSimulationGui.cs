using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class PhotonLagSimulationGui : MonoBehaviour
{
    public bool Visible = true;
    public int WindowId = 101;
    public Rect WindowRect = new Rect(0f, 100f, 120f, 100f);

    private void NetSimHasNoPeerWindow(int windowId)
    {
        GUILayout.Label("No peer to communicate with. ");
    }

    private void NetSimWindow(int windowId)
    {
        bool flag;
        bool flag2;
        GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", this.Peer.RoundTripTime, this.Peer.RoundTripTimeVariance));
        if ((flag2 = GUILayout.Toggle(flag = this.Peer.IsSimulationEnabled, "Simulate")) != flag)
        {
            this.Peer.IsSimulationEnabled = flag2;
        }
        float incomingLag = this.Peer.NetworkSimulationSettings.IncomingLag;
        GUILayout.Label("Lag " + incomingLag);
        incomingLag = GUILayout.HorizontalSlider(incomingLag, 0f, 500f);
        this.Peer.NetworkSimulationSettings.IncomingLag = (int) incomingLag;
        this.Peer.NetworkSimulationSettings.OutgoingLag = (int) incomingLag;
        float incomingJitter = this.Peer.NetworkSimulationSettings.IncomingJitter;
        GUILayout.Label("Jit " + incomingJitter);
        incomingJitter = GUILayout.HorizontalSlider(incomingJitter, 0f, 100f);
        this.Peer.NetworkSimulationSettings.IncomingJitter = (int) incomingJitter;
        this.Peer.NetworkSimulationSettings.OutgoingJitter = (int) incomingJitter;
        float incomingLossPercentage = this.Peer.NetworkSimulationSettings.IncomingLossPercentage;
        GUILayout.Label("Loss " + incomingLossPercentage);
        incomingLossPercentage = GUILayout.HorizontalSlider(incomingLossPercentage, 0f, 10f);
        this.Peer.NetworkSimulationSettings.IncomingLossPercentage = (int) incomingLossPercentage;
        this.Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int) incomingLossPercentage;
        if (GUI.changed)
        {
            this.WindowRect.height = 100f;
        }
        GUI.DragWindow();
    }

    public void OnGUI()
    {
        if (this.Visible)
        {
            if (this.Peer == null)
            {
                this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimHasNoPeerWindow), "Netw. Sim.");
            }
            else
            {
                this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimWindow), "Netw. Sim.");
            }
        }
    }

    public void Start()
    {
        this.Peer = PhotonNetwork.networkingPeer;
    }

    public PhotonPeer Peer { get; set; }
}

