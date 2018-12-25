using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class PhotonStatsGui : MonoBehaviour
{
    public bool buttonsOn;
    public bool healthStatsVisible;
    public bool statsOn = true;
    public Rect statsRect = new Rect(0f, 100f, 200f, 50f);
    public bool statsWindowOn = true;
    public bool trafficStatsOn;
    public int WindowId = 100;

    public void OnGUI()
    {
        if (PhotonNetwork.networkingPeer.TrafficStatsEnabled != this.statsOn)
        {
            PhotonNetwork.networkingPeer.TrafficStatsEnabled = this.statsOn;
        }
        if (this.statsWindowOn)
        {
            this.statsRect = GUILayout.Window(this.WindowId, this.statsRect, new GUI.WindowFunction(this.TrafficStatsWindow), "Messages (shift+tab)");
        }
    }

    public void Start()
    {
        this.statsRect.x = Screen.width - this.statsRect.width;
    }

    public void TrafficStatsWindow(int windowID)
    {
        bool flag = false;
        TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.networkingPeer.TrafficStatsGameLevel;
        long num = PhotonNetwork.networkingPeer.TrafficStatsElapsedMs / 1000L;
        if (num == 0)
        {
            num = 1L;
        }
        GUILayout.BeginHorizontal();
        this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons");
        this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health");
        this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic");
        GUILayout.EndHorizontal();
        string text = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", trafficStatsGameLevel.TotalOutgoingMessageCount, trafficStatsGameLevel.TotalIncomingMessageCount, trafficStatsGameLevel.TotalMessageCount);
        string str2 = string.Format("{0}sec average:", num);
        string str3 = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", trafficStatsGameLevel.TotalOutgoingMessageCount / num, trafficStatsGameLevel.TotalIncomingMessageCount / num, trafficStatsGameLevel.TotalMessageCount / num);
        GUILayout.Label(text);
        GUILayout.Label(str2);
        GUILayout.Label(str3);
        if (this.buttonsOn)
        {
            GUILayout.BeginHorizontal();
            this.statsOn = GUILayout.Toggle(this.statsOn, "stats on");
            if (GUILayout.Button("Reset"))
            {
                PhotonNetwork.networkingPeer.TrafficStatsReset();
                PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
            }
            flag = GUILayout.Button("To Log");
            GUILayout.EndHorizontal();
        }
        string str4 = string.Empty;
        string str5 = string.Empty;
        if (this.trafficStatsOn)
        {
            str4 = "Incoming: " + PhotonNetwork.networkingPeer.TrafficStatsIncoming;
            str5 = "Outgoing: " + PhotonNetwork.networkingPeer.TrafficStatsOutgoing;
            GUILayout.Label(str4);
            GUILayout.Label(str5);
        }
        string str6 = string.Empty;
        if (this.healthStatsVisible)
        {
            object[] args = new object[] { trafficStatsGameLevel.LongestDeltaBetweenSending, trafficStatsGameLevel.LongestDeltaBetweenDispatching, trafficStatsGameLevel.LongestEventCallback, trafficStatsGameLevel.LongestEventCallbackCode, trafficStatsGameLevel.LongestOpResponseCallback, trafficStatsGameLevel.LongestOpResponseCallbackOpCode, PhotonNetwork.networkingPeer.RoundTripTime, PhotonNetwork.networkingPeer.RoundTripTimeVariance };
            str6 = string.Format("ping: {6}[+/-{7}]ms\nlongest delta between\nsend: {0,4}ms disp: {1,4}ms\nlongest time for:\nev({3}):{2,3}ms op({5}):{4,3}ms", args);
            GUILayout.Label(str6);
        }
        if (flag)
        {
            object[] objArray2 = new object[] { text, str2, str3, str4, str5, str6 };
            Debug.Log(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", objArray2));
        }
        if (GUI.changed)
        {
            this.statsRect.height = 100f;
        }
        GUI.DragWindow();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            this.statsWindowOn = !this.statsWindowOn;
            this.statsOn = true;
        }
    }
}

