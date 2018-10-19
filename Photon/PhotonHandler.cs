using ExitGames.Client.Photon;
using System;
using System.Collections;
using Mod;
using Mod.Exceptions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using LogType = Mod.Logging.LogType;
using MonoBehaviour = Photon.MonoBehaviour;

public class PhotonHandler : MonoBehaviour, IPhotonPeerListener
{
    private static PhotonHandler _instance;
    
    public static bool AppQuits;
    public int nextSendTickCount;
    public int nextSendTickCountOnSerialize;
    public static Type PingImplementation;
    private static bool sendThreadShouldRun;
    public int updateInterval;
    public int updateIntervalOnSerialize;

    protected void Awake()
    {
        if (_instance != null && _instance != this)
            DestroyImmediate(_instance.gameObject);
        _instance = this;
        DontDestroyOnLoad(gameObject);
        this.updateInterval = 1000 / PhotonNetwork.sendRate;
        this.updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
        StartFallbackSendAckThread();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        switch (level)
        {
            case DebugLevel.ERROR:
                Shelter.Log(message, LogType.Error);
                break;
            case DebugLevel.WARNING:
                Shelter.Log(message, LogType.Warning);
                break;
            case DebugLevel.INFO when PhotonNetwork.LogLevel >= PhotonLogLevel.Informational:
            case DebugLevel.ALL when PhotonNetwork.LogLevel == PhotonLogLevel.Full:
                Shelter.Log(message);
                break;
        }
    }

    private static bool FallbackSendAckThread()
    {
        if (sendThreadShouldRun && PhotonNetwork.networkingPeer != null)
        {
            PhotonNetwork.networkingPeer.SendAcksOnly();
        }
        return sendThreadShouldRun;
    }

    protected void OnApplicationQuit()
    {
        AppQuits = true;
        StopFallbackSendAckThread();
        PhotonNetwork.Disconnect();
    }

    protected void OnCreatedRoom()
    {
        PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(Application.loadedLevelName);
    }

    public void OnEvent(EventData photonEvent)
    {
    }

    protected void OnJoinedRoom()
    {
        PhotonNetwork.networkingPeer.LoadLevelIfSynced();
    }

    protected void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.networkingPeer.NewSceneLoaded();
        PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(Application.loadedLevelName);
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
    }

    protected internal static void PingAvailableRegionsAndConnectToBest()
    {
        _instance.StartCoroutine(PingAvailableRegionsCoroutine(true));
    }

    public static void StartFallbackSendAckThread()
    {
        if (!sendThreadShouldRun)
        {
            sendThreadShouldRun = true;
            SupportClass.CallInBackground(FallbackSendAckThread);
        }
    }

    private static void StopFallbackSendAckThread()
    {
        sendThreadShouldRun = false;
    }

    protected void Update()
    {
        if (PhotonNetwork.networkingPeer == null)
        {
            Shelter.LogBoth("NetworkingPeer broke while PhotonHandler is still active.", LogType.Error);
            return;
        }
        
        if (PhotonNetwork.connectionStatesDetailed != PeerStates.PeerCreated && PhotonNetwork.connectionStatesDetailed != PeerStates.Disconnected && !PhotonNetwork.offlineMode && PhotonNetwork.isMessageQueueRunning)
        {
            bool flag = true;
            while (PhotonNetwork.isMessageQueueRunning && flag)
            {
                try
                {
                    flag = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
                }
                catch (NotAllowedException)
                {}
                catch (Exception e)
                {
                    Shelter.LogConsole("A {0} has been thrown in Photon3Unity3D.dll", LogType.Error, e.GetType().Name);
                    Shelter.Log("{0}: {1}\n{2}", LogType.Error, e.GetType().FullName, e.Message, e.StackTrace);
                    flag = false; 
                }
            }
            int num = (int)(Time.realtimeSinceStartup * 1000f);
            if (PhotonNetwork.isMessageQueueRunning && num > this.nextSendTickCountOnSerialize)
            {
                PhotonNetwork.networkingPeer.RunViewUpdate();
                this.nextSendTickCountOnSerialize = num + this.updateIntervalOnSerialize;
                this.nextSendTickCount = 0;
            }
            num = (int)(Time.realtimeSinceStartup * 1000f);
            if (num > this.nextSendTickCount)
            {
                while (PhotonNetwork.isMessageQueueRunning && PhotonNetwork.networkingPeer.SendOutgoingCommands())
                {
                }
                this.nextSendTickCount = num + this.updateInterval;
            }
        }
    }

    internal static CloudRegionCode BestRegionCodeInPreferences
    {
        get
        {
            string str = PlayerPrefs.GetString("PUNCloudBestRegion", string.Empty);
            if (!string.IsNullOrEmpty(str))
            {
                return Region.Parse(str);
            }
            return CloudRegionCode.none;
        }
        set
        {
            if (value == CloudRegionCode.none)
            {
                PlayerPrefs.DeleteKey("PUNCloudBestRegion");
            }
            else
            {
                PlayerPrefs.SetString("PUNCloudBestRegion", value.ToString());
            }
        }
    }

    private static IEnumerator PingAvailableRegionsCoroutine(bool connectToBest)
    {
        while (PhotonNetwork.networkingPeer.AvailableRegions == null)
        {
            if (PhotonNetwork.connectionStatesDetailed != PeerStates.ConnectingToNameServer && PhotonNetwork.connectionStatesDetailed != PeerStates.ConnectedToNameServer)
            {
                Debug.LogError("Call ConnectToNameServer to ping available regions.");
                yield break; // break if we don't connect to the nameserver at all
            }

            Shelter.Log("Waiting for AvailableRegions. State: {0} Server: {1} PhotonNetwork.networkingPeer.AvailableRegions {2}",
                LogType.Info, PhotonNetwork.connectionStatesDetailed, PhotonNetwork.Server, PhotonNetwork.networkingPeer.AvailableRegions != null);
            yield return new WaitForSeconds(0.25f); // wait until pinging finished (offline mode won't ping)
        }

        if (PhotonNetwork.networkingPeer.AvailableRegions == null || PhotonNetwork.networkingPeer.AvailableRegions.Count == 0)
        {
            Debug.LogError("No regions available. Are you sure your appid is valid and setup?");
            yield break; // break if we don't get regions at all
        }

        PhotonPingManager pingManager = new PhotonPingManager();
        foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
        {
            _instance.StartCoroutine(pingManager.PingSocket(region));
        }

        while (!pingManager.Done)
        {
            yield return new WaitForSeconds(0.1f); // wait until pinging finished (offline mode won't ping)
        }


        Region best = PhotonPingManager.BestRegion;
        BestRegionCodeInPreferences = best.Code;

        Debug.Log("Found best region: " + best.Code + " ping: " + best.Ping + ". Calling ConnectToRegionMaster() is: " + connectToBest);


        if (connectToBest)
        {
            PhotonNetwork.networkingPeer.ConnectToRegionMaster(best.Code);
        }
    }
}