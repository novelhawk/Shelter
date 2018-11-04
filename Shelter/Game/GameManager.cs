using Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Game;
using Mod.GameSettings;
using Mod.Keybinds;
using Mod.Managers;
using Mod.Modules;
using NGUI.Internal;
using Photon;
using Photon.Enums;
using RC;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// ReSharper disable once CheckNamespace
public partial class FengGameManagerMKII : Photon.MonoBehaviour
{
    public const string ApplicationID = "f1f6195c-df4a-40f9-bae5-4744c32901ef";
    private const string PlayerRespawnTag = "playerRespawn";
    public static Hashtable banHash;
    public static Hashtable boolVariables;
    public static string currentLevel;
    public static string currentScript;
    public static string currentScriptLogic;
    private float currentSpeed;
    private float _timeSincePause;
    public static bool customLevelLoaded;
    private bool endRacing;
    public static Hashtable floatVariables;
    private float gameEndCD;
    private float gameEndTotalCDtime = 9f;
    private bool gameTimesUp;
    public static Hashtable globalVariables;
    public static Hashtable heroHash;
    private int highestwave = 1;
    private int humanScore;
    public static Hashtable _infectionTitans;
    public static FengGameManagerMKII instance;
    public static Hashtable intVariables;
    private bool isLosing;
    private bool isWinning;
    private ArrayList killInfoGO = new ArrayList();
    public static bool LAN;
    public static string Level = string.Empty;
    public static Hashtable[] linkHash;
    private string localRacingResult;
    public static bool logicLoaded;
    public static bool masterRC;
    private float maxSpeed;
    private string myLastHero;
    public static bool noRestart;
    public static string oldScript;
    public static string oldScriptLogic;
    public const bool OnPrivateServer = false;
    public static Hashtable playerVariables;
    public static string PrivateServerAuthPass;    
    public static string privateServerField;
    private int PVPhumanScoreMax = 200;
    private int PVPtitanScoreMax = 200;
    public static AssetBundle RCassets;
    public static Hashtable RCEvents;
    public static Hashtable RCRegions;
    public static Hashtable RCRegionTriggers;
    public static Hashtable RCVariableNames;
    public static Settings settings; //TODO: Replace with a class which contains named settings
    public static Material skyMaterial;
    private bool startRacing;
    public static Hashtable stringVariables;
    private int[] teamScores;
    private int teamWinner;
    private float timeElapse;
    private float timeTotalServer;
    private int titanScore;
    public static Hashtable titanVariables;
    private GameObject ui;
    private List<RacingResult> racingResult;

    public readonly ArrayList Heroes = new ArrayList();
    public readonly ArrayList Hooks = new ArrayList();
    public readonly ArrayList FemaleTitans = new ArrayList();
    public readonly ArrayList ColossalTitans = new ArrayList();
    public readonly ArrayList Titans = new ArrayList();
    public readonly ArrayList ErenTitans = new ArrayList();

    public void AddTime(float t)
    {
        timeTotalServer -= t;
    }

    private void Cache()
    {
        ClothFactory.ClearClothCache();
        chatRoom = GameObject.Find("Chatroom").GetComponent<InRoomChat>();
        playersRPC.Clear();
        titanSpawners.Clear();
        groundList.Clear();
        PreservedPlayerKDR = new Dictionary<string, int[]>();
        noRestart = false;
        skyMaterial = null;
        isSpawning = false;
        retryTime = 0f;
        logicLoaded = false;
        customLevelLoaded = true;
        isUnloading = false;
        isRecompiling = false;
        Time.timeScale = 1f;
        UnityEngine.Camera.main.farClipPlane = 1500f;
        pauseWaitTime = 0f;
        spectateSprites = new List<GameObject>();
        isRestarting = false;
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(WaitAndResetRestarts());
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            roundTime = 0f;
            if (Level.StartsWith("Custom"))
            {
                customLevelLoaded = false;
            }
            if (PhotonNetwork.isMasterClient)
            {
                if (isFirstLoad)
                {
                    SetGameSettings(GetGameSettings());
                }
                if (FengGameManagerMKII.settings.IsEndless)
                {
//                    StartCoroutine(RespawnEnumerator(RCSettings.endlessMode)); TODO: Check what the actual fuck is this and make it better
                }
            }
            if (settings.EnableChatFeed)
                Mod.Interface.Chat.System("<color=#FFC000>({0:F2})</color> Round Start.", roundTime);
        }
        isFirstLoad = false;
    }

    private Hashtable GetGameSettings()
    {
        var rcsettings = GameSettingsManager.ImportRCSettings();
        return GameSettingsManager.EncodeToHashtable(rcsettings);
    }

    private static bool IsAnyTitanAlive => 
        GameObject.FindGameObjectsWithTag("titan")
            .Any(x => (x.GetComponent<TITAN>() != null && !x.GetComponent<TITAN>().hasDie) ||
                      (x.GetComponent<FEMALE_TITAN>() != null && !x.GetComponent<FEMALE_TITAN>().hasDie) ||
                      (x.GetComponent<COLOSSAL_TITAN>() != null && !x.GetComponent<COLOSSAL_TITAN>().hasDie));

    public void CheckPVPPoints()
    {
        if (PVPtitanScore >= PVPtitanScoreMax)
        {
            PVPtitanScore = PVPtitanScoreMax;
            GameLose();
        }
        else if (PVPhumanScore >= PVPhumanScoreMax)
        {
            PVPhumanScore = PVPhumanScoreMax;
            GameWin();
        }
    }

    private IEnumerator ClearLevelEnumerator(string[] skybox)
    {
        bool mipmap = settings.UseMipmap;
        bool unloadAssets = false;
        
        if (skybox.Length >= 6)
        {
            string iteratorVariable3 = string.Join(",", skybox);
            if (!linkHash[1].ContainsKey(iteratorVariable3))
            {
                unloadAssets = true;
                Material material = Camera.main.GetComponent<Skybox>().material;

                string[] skyboxSide = {"_FrontTex", "_BackTex", "_LeftTex", "_RightTex", "_UpTex", "_DownTex"};
                for (int i = 0; i < 6; i++)
                {
                    var url = skybox[i];
                    if (!Utility.IsValidImageUrl(url))
                        continue;
                    using (WWW www = new WWW(url))
                    {
                        yield return www;
                        if (www.error != null)
                            continue;
                        material.SetTexture(skyboxSide[i], RCextensions.LoadImageRC(www, mipmap, 500000));
                    }
                }
                
                Camera.main.GetComponent<Skybox>().material = material;
                linkHash[1].Add(iteratorVariable3, material);
                skyMaterial = material;
            }
            else
            {
                Camera.main.GetComponent<Skybox>().material = (Material)linkHash[1][iteratorVariable3];
                skyMaterial = (Material)linkHash[1][iteratorVariable3];
            }
        }
        string grassUrl = skybox[6];
        if (Utility.IsValidImageUrl(grassUrl))
        {
            foreach (GameObject ground in groundList)
            {
                if (ground != null && ground.renderer != null)
                {
                    foreach (Renderer groundRenderer in ground.GetComponentsInChildren<Renderer>())
                    {
                        if (!linkHash[0].ContainsKey(grassUrl))
                        {
                            using (WWW www = new WWW(grassUrl))
                            {
                                yield return www;
                                if (www.error != null)
                                    continue;
                                groundRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                            }
                            unloadAssets = true;
                            linkHash[0].Add(grassUrl, groundRenderer.material);
                        }
                        else
                        {
                            groundRenderer.material = (Material)linkHash[0][grassUrl];
                        }
                    }
                }
            }
        }
        else if (grassUrl.EqualsIgnoreCase("transparent"))
        {
            foreach (GameObject ground in groundList)
            {
                if (ground != null && ground.renderer != null)
                {
                    foreach (Renderer renderer1 in ground.GetComponentsInChildren<Renderer>())
                    {
                        renderer1.enabled = false;
                    }
                }
            }
        }
        
        if (unloadAssets)
            UnloadAssets();
    }

    public void CompileScript(string str)
    {
        int num3;
        string[] strArray2 = str.Replace(" ", string.Empty).Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        Hashtable hashtable = new Hashtable();
        int num = 0;
        int num2 = 0;
        bool flag = false;
        for (num3 = 0; num3 < strArray2.Length; num3++)
        {
            if (strArray2[num3] == "{")
            {
                num++;
            }
            else if (strArray2[num3] == "}")
            {
                num2++;
            }
            else
            {
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                foreach (char ch in strArray2[num3])
                {
                    switch (ch)
                    {
                        case '(':
                            num4++;
                            break;

                        case ')':
                            num5++;
                            break;

                        case '"':
                            num6++;
                            break;
                    }
                }
                if (num4 != num5)
                {
                    int num8 = num3 + 1;
                    Mod.Interface.Chat.System("Script Error: Parentheses not equal! (line " + num8 + ")");
                    flag = true;
                }
                if (num6 % 2 != 0)
                {
                    Mod.Interface.Chat.System("Script Error: Quotations not equal! (line " + (num3 + 1) + ")");
                    flag = true;
                }
            }
        }
        if (num != num2)
        {
            Mod.Interface.Chat.System("Script Error: Bracket count not equivalent!");
            flag = true;
        }
        if (!flag)
        {
            try
            {
                int num10;
                num3 = 0;
                while (num3 < strArray2.Length)
                {
                    if (strArray2[num3].StartsWith("On") && strArray2[num3 + 1] == "{")
                    {
                        int key = num3;
                        num10 = num3 + 2;
                        int num11 = 0;
                        for (int i = num3 + 2; i < strArray2.Length; i++)
                        {
                            if (strArray2[i] == "{")
                            {
                                num11++;
                            }
                            if (strArray2[i] == "}")
                            {
                                if (num11 > 0)
                                {
                                    num11--;
                                }
                                else
                                {
                                    num10 = i - 1;
                                    i = strArray2.Length;
                                }
                            }
                        }
                        hashtable.Add(key, num10);
                        num3 = num10;
                    }
                    num3++;
                }
                foreach (int num9 in hashtable.Keys)
                {
                    int num14;
                    int num15;
                    string str4;
                    string str5;
                    RegionTrigger trigger;
                    string str3 = strArray2[num9];
                    num10 = (int) hashtable[num9];
                    string[] stringArray = new string[num10 - num9 + 1];
                    int index = 0;
                    for (num3 = num9; num3 <= num10; num3++)
                    {
                        stringArray[index] = strArray2[num3];
                        index++;
                    }
                    RCEvent event2 = ParseBlock(stringArray, 0, 0, null);
                    if (str3.StartsWith("OnPlayerEnterRegion"))
                    {
                        num14 = str3.IndexOf('[');
                        num15 = str3.IndexOf(']');
                        str4 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        num14 = str3.IndexOf('(');
                        num15 = str3.IndexOf(')');
                        str5 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        if (RCRegionTriggers.ContainsKey(str4))
                        {
                            trigger = (RegionTrigger) RCRegionTriggers[str4];
                            trigger.playerEventEnter = event2;
                            trigger.myName = str4;
                            RCRegionTriggers[str4] = trigger;
                        }
                        else
                        {
                            trigger = new RegionTrigger {
                                playerEventEnter = event2,
                                myName = str4
                            };
                            RCRegionTriggers.Add(str4, trigger);
                        }
                        RCVariableNames.Add("OnPlayerEnterRegion[" + str4 + "]", str5);
                    }
                    else if (str3.StartsWith("OnPlayerLeaveRegion"))
                    {
                        num14 = str3.IndexOf('[');
                        num15 = str3.IndexOf(']');
                        str4 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        num14 = str3.IndexOf('(');
                        num15 = str3.IndexOf(')');
                        str5 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        if (RCRegionTriggers.ContainsKey(str4))
                        {
                            trigger = (RegionTrigger) RCRegionTriggers[str4];
                            trigger.playerEventExit = event2;
                            trigger.myName = str4;
                            RCRegionTriggers[str4] = trigger;
                        }
                        else
                        {
                            trigger = new RegionTrigger {
                                playerEventExit = event2,
                                myName = str4
                            };
                            RCRegionTriggers.Add(str4, trigger);
                        }
                        RCVariableNames.Add("OnPlayerExitRegion[" + str4 + "]", str5);
                    }
                    else if (str3.StartsWith("OnTitanEnterRegion"))
                    {
                        num14 = str3.IndexOf('[');
                        num15 = str3.IndexOf(']');
                        str4 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        num14 = str3.IndexOf('(');
                        num15 = str3.IndexOf(')');
                        str5 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        if (RCRegionTriggers.ContainsKey(str4))
                        {
                            trigger = (RegionTrigger) RCRegionTriggers[str4];
                            trigger.titanEventEnter = event2;
                            trigger.myName = str4;
                            RCRegionTriggers[str4] = trigger;
                        }
                        else
                        {
                            trigger = new RegionTrigger {
                                titanEventEnter = event2,
                                myName = str4
                            };
                            RCRegionTriggers.Add(str4, trigger);
                        }
                        RCVariableNames.Add("OnTitanEnterRegion[" + str4 + "]", str5);
                    }
                    else if (str3.StartsWith("OnTitanLeaveRegion"))
                    {
                        num14 = str3.IndexOf('[');
                        num15 = str3.IndexOf(']');
                        str4 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        num14 = str3.IndexOf('(');
                        num15 = str3.IndexOf(')');
                        str5 = str3.Substring(num14 + 2, num15 - num14 - 3);
                        if (RCRegionTriggers.ContainsKey(str4))
                        {
                            trigger = (RegionTrigger) RCRegionTriggers[str4];
                            trigger.titanEventExit = event2;
                            trigger.myName = str4;
                            RCRegionTriggers[str4] = trigger;
                        }
                        else
                        {
                            trigger = new RegionTrigger {
                                titanEventExit = event2,
                                myName = str4
                            };
                            RCRegionTriggers.Add(str4, trigger);
                        }
                        RCVariableNames.Add("OnTitanExitRegion[" + str4 + "]", str5);
                    }
                    else if (str3.StartsWith("OnFirstLoad()"))
                    {
                        RCEvents.Add("OnFirstLoad", event2);
                    }
                    else if (str3.StartsWith("OnRoundStart()"))
                    {
                        RCEvents.Add("OnRoundStart", event2);
                    }
                    else if (str3.StartsWith("OnUpdate()"))
                    {
                        RCEvents.Add("OnUpdate", event2);
                    }
                    else
                    {
                        string[] strArray4;
                        if (str3.StartsWith("OnTitanDie"))
                        {
                            num14 = str3.IndexOf('(');
                            num15 = str3.LastIndexOf(')');
                            strArray4 = str3.Substring(num14 + 1, num15 - num14 - 1).Split(',');
                            strArray4[0] = strArray4[0].Substring(1, strArray4[0].Length - 2);
                            strArray4[1] = strArray4[1].Substring(1, strArray4[1].Length - 2);
                            RCVariableNames.Add("OnTitanDie", strArray4);
                            RCEvents.Add("OnTitanDie", event2);
                        }
                        else if (str3.StartsWith("OnPlayerDieByTitan"))
                        {
                            RCEvents.Add("OnPlayerDieByTitan", event2);
                            num14 = str3.IndexOf('(');
                            num15 = str3.LastIndexOf(')');
                            strArray4 = str3.Substring(num14 + 1, num15 - num14 - 1).Split(',');
                            strArray4[0] = strArray4[0].Substring(1, strArray4[0].Length - 2);
                            strArray4[1] = strArray4[1].Substring(1, strArray4[1].Length - 2);
                            RCVariableNames.Add("OnPlayerDieByTitan", strArray4);
                        }
                        else if (str3.StartsWith("OnPlayerDieByPlayer"))
                        {
                            RCEvents.Add("OnPlayerDieByPlayer", event2);
                            num14 = str3.IndexOf('(');
                            num15 = str3.LastIndexOf(')');
                            strArray4 = str3.Substring(num14 + 1, num15 - num14 - 1).Split(',');
                            strArray4[0] = strArray4[0].Substring(1, strArray4[0].Length - 2);
                            strArray4[1] = strArray4[1].Substring(1, strArray4[1].Length - 2);
                            RCVariableNames.Add("OnPlayerDieByPlayer", strArray4);
                        }
                        else if (str3.StartsWith("OnChatInput"))
                        {
                            RCEvents.Add("OnChatInput", event2);
                            num14 = str3.IndexOf('(');
                            num15 = str3.LastIndexOf(')');
                            str5 = str3.Substring(num14 + 1, num15 - num14 - 1);
                            RCVariableNames.Add("OnChatInput", str5.Substring(1, str5.Length - 2));
                        }
                    }
                }
            }
            catch (UnityException exception)
            {
                Mod.Interface.Chat.System(exception.Message);
            }
        }
    }

    public int StringToType(string str)
    {
        if (!str.StartsWith("Int"))
        {
            if (str.StartsWith("Bool"))
            {
                return 1;
            }
            if (str.StartsWith("String"))
            {
                return 2;
            }
            if (str.StartsWith("Float"))
            {
                return 3;
            }
            if (str.StartsWith("Titan"))
            {
                return 5;
            }
            if (str.StartsWith("Player"))
            {
                return 4;
            }
        }
        return 0;
    }

    private int? _racingMessageId;
    private int? _endingMessageId;
    private int? _pauseMessageId;
    private void Core()
    {
        if (false) //TODO: Probably broken || was `(int) settings[64] >= 100`
        {
            CoreEditor();
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && needChooseSide)
            {
                if (Shelter.InputManager.IsDown(Mod.Keybinds.InputAction.RedFlare)) //TODO: Automatically show on room join
                {
                    if (NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3])) //TODO: Remove UI_IN_GAME
                    {
                        Screen.lockCursor = true;
                        Screen.showCursor = true;
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], true);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
                        UnityEngine.Camera.main.GetComponent<SpectatorMovement>().disable = false;
                        UnityEngine.Camera.main.GetComponent<MouseLook>().disable = false;
                    }
                    else
                    {
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], true);
                        UnityEngine.Camera.main.GetComponent<SpectatorMovement>().disable = true;
                        UnityEngine.Camera.main.GetComponent<MouseLook>().disable = true;
                    }
                }
                if (Shelter.InputManager.IsDown(InputAction.MenuKey) && !_isVisible)
                {
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                    UnityEngine.Camera.main.GetComponent<SpectatorMovement>().disable = true;
                    UnityEngine.Camera.main.GetComponent<MouseLook>().disable = true;
                    _isVisible = true;
                }
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                int length;
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    case GameType.Multiplayer:
                        CoreAdd();
                        if (Camera.main != null && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing && UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver && !needChooseSide && !settings.InSpectatorMode)
                        {
                            if (LevelInfoManager.Get(Level).RespawnMode == RespawnMode.DEATHMATCH || 
                                settings.IsEndless || (settings.IsBombMode || settings.PVPMode > PVPMode.Off) && 
                                !settings.IsPointMode)
                            {
                                myRespawnTime += Time.deltaTime;
                                int endlessMode = 5;
                                if (Player.Self.Properties.PlayerType == PlayerType.Titan)
                                {
                                    endlessMode = 10;
                                }
                                if (settings.IsEndless)
                                {
                                    endlessMode = 1;
                                }
                                myRespawnTime = 0f; //TODO: Add option to disable insta respawn
                                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                                if (Player.Self.Properties.PlayerType == PlayerType.Titan)
                                {
                                    SpawnPlayerTitan(myLastHero);
                                }
                                else
                                {
                                    StartCoroutine(WaitAndRespawn1(0.1f));
                                }
                                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                            }
                        }
                        break;
                    case GameType.Singleplayer:
                        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.Racing)
                        {
                            if (!isLosing)
                            {
                                currentSpeed = UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity.magnitude; //TODO: Show in new gui
                                maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
                            }
                        }
                        break;
                }

                if (isWinning || (isLosing && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing))
                {
                    if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && _endingMessageId.HasValue)
                    {
                        if (gameEndCD <= 0f)
                        {
                            gameEndCD = 0f;
                            Mod.Interface.Chat.EditMessage(_endingMessageId, "Game is now restarting.", false);
                            _endingMessageId = null;
                            if (PhotonNetwork.isMasterClient)
                                RestartGameRC();
                        }
                        else
                        {
                            gameEndCD -= Time.deltaTime;
                            Mod.Interface.Chat.EditMessage(_endingMessageId, $"Game restarting in {gameEndCD:0.00} seconds.", true);
                        }
                    }
                } 
                else if (_endingMessageId.HasValue)
                {
                    Mod.Interface.Chat.EditMessage(_endingMessageId, "Restart failed.", false);
                    _endingMessageId = null;
                }
                timeElapse += Time.deltaTime;
                roundTime += Time.deltaTime;
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    case GameType.Singleplayer when IN_GAME_MAIN_CAMERA.GameMode == GameMode.Racing:
                        if (!isWinning)
                        {
                            timeTotalServer += Time.deltaTime;
                        }

                        break;
                    case GameType.Singleplayer:
                        if (!(isLosing || isWinning))
                        {
                            timeTotalServer += Time.deltaTime;
                        }

                        break;
                    default:
                        timeTotalServer += Time.deltaTime;
                        break;
                }
                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.Racing)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    {
                        if (timeTotalServer < 5f)
                        {
                            if (!_racingMessageId.HasValue)
                                _racingMessageId = Mod.Interface.Chat.System("Race is starting soon");
                            Mod.Interface.Chat.EditMessage(_racingMessageId, $"Race is starting in {5 - timeTotalServer:0.00}", true);
                        }
                        else if (!startRacing && isWinning)
                        {
                            Mod.Interface.Chat.EditMessage(_racingMessageId, "Race started.", false);
                            _racingMessageId = null;
                            startRacing = true;
                            endRacing = false;
                            if (Shelter.TryFind("door", out GameObject door))
                                door.SetActive(false);
                        }
                    }
                    else
                    {
                        if (roundTime < 20f)
                        {
                            if (!_racingMessageId.HasValue)
                                _racingMessageId = Mod.Interface.Chat.System("Race is starting soon");
                            Mod.Interface.Chat.EditMessage(_racingMessageId, $"Race is starting in {20 - roundTime:0.00}", true);
                        }
                        else if (!startRacing)
                        {
                            Mod.Interface.Chat.EditMessage(_racingMessageId, "Race started.", false);
                            _racingMessageId = null;

                            startRacing = true;
                            endRacing = false;
                            if (Shelter.TryFind("door", out GameObject obj))
                                obj.SetActive(false);

                            if (racingDoors != null && customLevelLoaded)
                            {
                                foreach (GameObject door in racingDoors)
                                    door.SetActive(false);

                                racingDoors = null;
                            }
                        }
                        else if (racingDoors != null && customLevelLoaded)
                        {
                            foreach (GameObject door in racingDoors)
                                door.SetActive(false);

                            racingDoors = null;
                        }
                    }
                    if (Camera.main != null && Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver && !needChooseSide && customLevelLoaded)
                    {
                        myRespawnTime += Time.deltaTime;
                        if (myRespawnTime > 1.5f)
                        {
                            myRespawnTime = 0f;
                            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                            if (checkpoint != null)
                            {
                                StartCoroutine(WaitAndRespawn2(0.1f, checkpoint));
                            }
                            else
                            {
                                StartCoroutine(WaitAndRespawn1(0.1f));
                            }
                            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                        }
                    }
                }
                if (timeElapse > 1f)
                    timeElapse--;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && killInfoGO.Count > 0 && killInfoGO[0] == null)
                {
                    killInfoGO.RemoveAt(0);
                }
            }
        }
    }

    private void CoreAdd()
    {
        if (PhotonNetwork.isMasterClient)
        {
            OnUpdate();
            if (customLevelLoaded)
            {
                for (int i = 0; i < titanSpawners.Count; i++)
                {
                    TitanSpawner item = titanSpawners[i];
                    item.time -= Time.deltaTime;
                    if (item.time <= 0f && Titans.Count + FemaleTitans.Count < Math.Min(settings.CustomMaxTitans, 80))
                    {
                        string name1 = item.name;
                        if (name1 == "spawnAnnie")
                        {
                            PhotonNetwork.Instantiate("FEMALE_TITAN", item.location, new Quaternion(0f, 0f, 0f, 1f), 0);
                        }
                        else
                        {
                            GameObject obj2 = PhotonNetwork.Instantiate("TITAN_VER3.1", item.location, new Quaternion(0f, 0f, 0f, 1f), 0);
                            switch (name1)
                            {
                                case "spawnAbnormal":
                                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Abnormal, false);
                                    break;
                                case "spawnJumper":
                                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                                    break;
                                case "spawnCrawler":
                                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, true);
                                    break;
                                case "spawnPunk":
                                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Punk, false);
                                    break;
                            }
                        }
                        if (item.endless)
                        {
                            item.time = item.delay;
                        }
                        else
                        {
                            titanSpawners.Remove(item);
                        }
                    }
                }
            }
        }

        if (_pauseMessageId.HasValue)
        {
            pauseWaitTime -= Time.deltaTime * 1000000f;
            _timeSincePause += Time.deltaTime * 1000000f;
            if (pauseWaitTime <= 3f)
                Mod.Interface.Chat.EditMessage(_pauseMessageId, $"Pause will end in {pauseWaitTime} seconds.", true);
            else 
                Mod.Interface.Chat.EditMessage(_pauseMessageId, $"Game paused for {_timeSincePause}.", true);
            
            if (pauseWaitTime <= 1f)
                Camera.main.farClipPlane = 1500f;
            
            if (pauseWaitTime <= 0f)
            {
                Mod.Interface.Chat.EditMessage(_pauseMessageId, "Pause ended.", false);
                _pauseMessageId = null;
                pauseWaitTime = 0f;
                Time.timeScale = 1f;
            }
        }
    }

    private void CoreEditor()
    {
        Debug.Log("Editor is currently WIP");
        Application.Quit();
//        if (Input.GetKey(KeyCode.Tab))
//        {
//            GUI.FocusControl(null);
//        }
//        if (selectedObj != null)
//        {
//            float num = 0.2f;
//            if (inputRC.isInputLevel(InputCodeRC.levelSlow))
//            {
//                num = 0.04f;
//            }
//            else if (inputRC.isInputLevel(InputCodeRC.levelFast))
//            {
//                num = 0.6f;
//            }
//            if (inputRC.isInputLevel(InputCodeRC.levelForward))
//            {
//                Transform transform1 = selectedObj.transform;
//                transform1.position += num * new Vector3(UnityEngine.Camera.main.transform.forward.x, 0f, UnityEngine.Camera.main.transform.forward.z);
//            }
//            else if (inputRC.isInputLevel(InputCodeRC.levelBack))
//            {
//                Transform transform9 = selectedObj.transform;
//                transform9.position -= num * new Vector3(UnityEngine.Camera.main.transform.forward.x, 0f, UnityEngine.Camera.main.transform.forward.z);
//            }
//            if (inputRC.isInputLevel(InputCodeRC.levelLeft))
//            {
//                Transform transform10 = selectedObj.transform;
//                transform10.position -= num * new Vector3(UnityEngine.Camera.main.transform.right.x, 0f, UnityEngine.Camera.main.transform.right.z);
//            }
//            else if (inputRC.isInputLevel(InputCodeRC.levelRight))
//            {
//                Transform transform11 = selectedObj.transform;
//                transform11.position += num * new Vector3(UnityEngine.Camera.main.transform.right.x, 0f, UnityEngine.Camera.main.transform.right.z);
//            }
//            if (inputRC.isInputLevel(InputCodeRC.levelDown))
//            {
//                Transform transform12 = selectedObj.transform;
//                transform12.position -= Vector3.up * num;
//            }
//            else if (inputRC.isInputLevel(InputCodeRC.levelUp))
//            {
//                Transform transform13 = selectedObj.transform;
//                transform13.position += Vector3.up * num;
//            }
//            if (!selectedObj.name.StartsWith("misc,region"))
//            {
//                if (inputRC.isInputLevel(InputCodeRC.levelRRight))
//                {
//                    selectedObj.transform.Rotate(Vector3.up * num);
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelRLeft))
//                {
//                    selectedObj.transform.Rotate(Vector3.down * num);
//                }
//                if (inputRC.isInputLevel(InputCodeRC.levelRCCW))
//                {
//                    selectedObj.transform.Rotate(Vector3.forward * num);
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelRCW))
//                {
//                    selectedObj.transform.Rotate(Vector3.back * num);
//                }
//                if (inputRC.isInputLevel(InputCodeRC.levelRBack))
//                {
//                    selectedObj.transform.Rotate(Vector3.left * num);
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelRForward))
//                {
//                    selectedObj.transform.Rotate(Vector3.right * num);
//                }
//            }
//            if (inputRC.isInputLevel(InputCodeRC.levelPlace))
//            {
//                linkHash[3].Add(
//                    selectedObj.GetInstanceID(),             //key
//                    selectedObj.name + "," +                 // Name
//                    selectedObj.transform.position.x + "," + // x 1
//                    selectedObj.transform.position.y + "," + // y 1
//                    selectedObj.transform.position.z + "," + // z 1
//                    selectedObj.transform.rotation.x + "," + // x 2
//                    selectedObj.transform.rotation.y + "," + // y 2
//                    selectedObj.transform.rotation.z + "," + // z 2
//                    selectedObj.transform.rotation.w);
//                selectedObj = null;
//                UnityEngine.Camera.main.GetComponent<MouseLook>().enabled = true;
//                Screen.lockCursor = true;
//            }
//            if (inputRC.isInputLevel(InputCodeRC.levelDelete))
//            {
//                linkHash[3].Remove(selectedObj?.GetInstanceID());
//                Destroy(selectedObj);
//                selectedObj = null;
//                UnityEngine.Camera.main.GetComponent<MouseLook>().enabled = true;
//                Screen.lockCursor = true;
//            }
//        }
//        else
//        {
//            if (Screen.lockCursor)
//            {
//                float num2 = 100f;
//                if (inputRC.isInputLevel(InputCodeRC.levelSlow))
//                {
//                    num2 = 20f;
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelFast))
//                {
//                    num2 = 400f;
//                }
//                Transform transform7 = UnityEngine.Camera.main.transform;
//                if (inputRC.isInputLevel(InputCodeRC.levelForward))
//                {
//                    transform7.position += transform7.forward * num2 * Time.deltaTime;
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelBack))
//                {
//                    transform7.position -= transform7.forward * num2 * Time.deltaTime;
//                }
//                if (inputRC.isInputLevel(InputCodeRC.levelLeft))
//                {
//                    transform7.position -= transform7.right * num2 * Time.deltaTime;
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelRight))
//                {
//                    transform7.position += transform7.right * num2 * Time.deltaTime;
//                }
//                if (inputRC.isInputLevel(InputCodeRC.levelUp))
//                {
//                    transform7.position += transform7.up * num2 * Time.deltaTime;
//                }
//                else if (inputRC.isInputLevel(InputCodeRC.levelDown))
//                {
//                    transform7.position -= transform7.up * num2 * Time.deltaTime;
//                }
//            }
//            if (inputRC.isInputLevelDown(InputCodeRC.levelCursor))
//            {
//                if (Screen.lockCursor)
//                {
//                    UnityEngine.Camera.main.GetComponent<MouseLook>().enabled = false;
//                    Screen.lockCursor = false;
//                }
//                else
//                {
//                    UnityEngine.Camera.main.GetComponent<MouseLook>().enabled = true;
//                    Screen.lockCursor = true;
//                }
//            }
//            if (Input.GetKeyDown(KeyCode.Mouse0) && !Screen.lockCursor && GUIUtility.hotControl == 0 && !((Input.mousePosition.x <= 300f || Input.mousePosition.x >= Screen.width - 300f) && Screen.height - Input.mousePosition.y <= 600f))
//            {
//                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
//                {
//                    Transform transform8 = hitInfo.transform;
//                    if (transform8.gameObject.name.StartsWith("custom") || transform8.gameObject.name.StartsWith("base") || transform8.gameObject.name.StartsWith("racing") || transform8.gameObject.name.StartsWith("photon") || transform8.gameObject.name.StartsWith("spawnpoint") || transform8.gameObject.name.StartsWith("misc"))
//                    {
//                        selectedObj = transform8.gameObject;
//                        UnityEngine.Camera.main.GetComponent<MouseLook>().enabled = false;
//                        Screen.lockCursor = true;
//                        linkHash[3].Remove(selectedObj.GetInstanceID());
//                    }
//                    else if (transform8.parent.gameObject.name.StartsWith("custom") || transform8.parent.gameObject.name.StartsWith("base") || transform8.parent.gameObject.name.StartsWith("racing") || transform8.parent.gameObject.name.StartsWith("photon"))
//                    {
//                        selectedObj = transform8.parent.gameObject;
//                        UnityEngine.Camera.main.GetComponent<MouseLook>().enabled = false;
//                        Screen.lockCursor = true;
//                        linkHash[3].Remove(selectedObj.GetInstanceID());
//                    }
//                }
//            }
//        }
    }

    private IEnumerator CustomLevelCache()
    {
        int iteratorVariable0 = 0;
        while (true)
        {
            if (iteratorVariable0 >= levelCache.Count)
            {
                yield break;
            }
            CustomLevelClient(levelCache[iteratorVariable0], false);
            yield return new WaitForEndOfFrame();
            iteratorVariable0++;
        }
    }

    private void CustomLevelClient(string[] content, bool renewHash)
    {
        int num;
        string[] strArray;
        bool flag = false;
        bool flag2 = false;
        if (content[content.Length - 1].StartsWith("a"))
        {
            flag = true;
        }
        else if (content[content.Length - 1].StartsWith("z"))
        {
            flag2 = true;
            customLevelLoaded = true;
            SpawnPlayerCustomMap();
            Minimap.TryRecaptureInstance();
            UnloadAssets();
            UnityEngine.Camera.main.GetComponent<TiltShift>().enabled = false;
        }
        if (renewHash)
        {
            if (flag)
            {
                currentLevel = string.Empty;
                levelCache.Clear();
                titanSpawns.Clear();
                playerSpawnsC.Clear();
                playerSpawnsM.Clear();
                for (num = 0; num < content.Length; num++)
                {
                    strArray = content[num].Split(',');
                    switch (strArray[0])
                    {
                        case "titan":
                            titanSpawns.Add(new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3])));
                            break;
                        case "playerC":
                            playerSpawnsC.Add(new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3])));
                            break;
                        case "playerM":
                            playerSpawnsM.Add(new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3])));
                            break;
                    }
                }
                SpawnPlayerCustomMap();
            }
            currentLevel = currentLevel + content[content.Length - 1];
            levelCache.Add(content);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.CurrentLevel, currentLevel }
            });
        }
        if (!flag && !flag2)
        {
            for (num = 0; num < content.Length; num++)
            {
                float num2;
                GameObject obj2;
                float num3;
                float num5;
                float num6;
                float num7;
                Color color;
                Mesh mesh;
                Color[] colorArray;
                int num8;
                strArray = content[num].Split(',');
                if (strArray[0].StartsWith("custom"))
                {
                    num2 = 1f;
                    obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[16]), Convert.ToSingle(strArray[17]), Convert.ToSingle(strArray[18])));
                    if (strArray[2] != "default")
                    {
                        if (strArray[2].EqualsIgnoreCase("transparent"))
                        {
                            if (float.TryParse(strArray[2].Substring(11), out num3))
                            {
                                num2 = num3;
                            }
                            foreach (Renderer renderer1 in obj2.GetComponentsInChildren<Renderer>())
                            {
                                renderer1.material = (Material) RCassets.Load("transparent");
                                if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                {
                                    renderer1.material.mainTextureScale = new Vector2(renderer1.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer1.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                }
                            }
                        }
                        else
                        {
                            foreach (Renderer renderer1 in obj2.GetComponentsInChildren<Renderer>())
                            {
                                renderer1.material = (Material) RCassets.Load(strArray[2]);
                                if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                {
                                    renderer1.material.mainTextureScale = new Vector2(renderer1.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer1.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                }
                            }
                        }
                    }
                    num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[3]);
                    num5 -= 0.001f;
                    num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[4]);
                    num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[5]);
                    obj2.transform.localScale = new Vector3(num5, num6, num7);
                    if (strArray[6] != "0")
                    {
                        color = new Color(Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), num2);
                        foreach (MeshFilter filter in obj2.GetComponentsInChildren<MeshFilter>())
                        {
                            mesh = filter.mesh;
                            colorArray = new Color[mesh.vertexCount];
                            num8 = 0;
                            while (num8 < mesh.vertexCount)
                            {
                                colorArray[num8] = color;
                                num8++;
                            }
                            mesh.colors = colorArray;
                        }
                    }
                }
                else if (strArray[0].StartsWith("base"))
                {
                    if (strArray.Length < 15)
                    {
                        Instantiate(Resources.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4])), new Quaternion(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8])));
                    }
                    else
                    {
                        num2 = 1f;
                        obj2 = (GameObject)Instantiate((GameObject) Resources.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[16]), Convert.ToSingle(strArray[17]), Convert.ToSingle(strArray[18])));
                        if (strArray[2] != "default")
                        {
                            if (strArray[2].EqualsIgnoreCase("transparent"))
                            {
                                if (float.TryParse(strArray[2].Substring(11), out num3))
                                {
                                    num2 = num3;
                                }
                                foreach (Renderer renderer1 in obj2.GetComponentsInChildren<Renderer>())
                                {
                                    renderer1.material = (Material) RCassets.Load("transparent");
                                    if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                    {
                                        renderer1.material.mainTextureScale = new Vector2(renderer1.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer1.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                    }
                                }
                            }
                            else
                            {
                                foreach (Renderer renderer1 in obj2.GetComponentsInChildren<Renderer>())
                                {
                                    if (!(renderer1.name.Contains("Particle System") && obj2.name.Contains("aot_supply")))
                                    {
                                        renderer1.material = (Material) RCassets.Load(strArray[2]);
                                        if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                        {
                                            renderer1.material.mainTextureScale = new Vector2(renderer1.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer1.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                        }
                                    }
                                }
                            }
                        }
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[3]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[4]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[5]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        if (strArray[6] != "0")
                        {
                            color = new Color(Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), num2);
                            foreach (MeshFilter filter in obj2.GetComponentsInChildren<MeshFilter>())
                            {
                                mesh = filter.mesh;
                                colorArray = new Color[mesh.vertexCount];
                                for (num8 = 0; num8 < mesh.vertexCount; num8++)
                                {
                                    colorArray[num8] = color;
                                }
                                mesh.colors = colorArray;
                            }
                        }
                    }
                }
                else if (strArray[0].StartsWith("misc"))
                {
                    if (strArray[1].StartsWith("barrier"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                    }
                    else if (strArray[1].StartsWith("racingStart"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        racingDoors?.Add(obj2);
                    }
                    else if (strArray[1].StartsWith("racingEnd"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        obj2.AddComponent<LevelTriggerRacingEnd>();
                    }
                    else if (strArray[1].StartsWith("region") && PhotonNetwork.isMasterClient)
                    {
                        Vector3 loc = new Vector3(Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]));
                        RCRegion region = new RCRegion(loc, Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4]), Convert.ToSingle(strArray[5]));
                        string key = strArray[2];
                        if (RCRegionTriggers.ContainsKey(key))
                        {
                            GameObject obj3 = (GameObject)Instantiate((GameObject) RCassets.Load("region"));
                            obj3.transform.position = loc;
                            obj3.AddComponent<RegionTrigger>();
                            obj3.GetComponent<RegionTrigger>().CopyTrigger((RegionTrigger) RCRegionTriggers[key]);
                            num5 = obj3.transform.localScale.x * Convert.ToSingle(strArray[3]);
                            num5 -= 0.001f;
                            num6 = obj3.transform.localScale.y * Convert.ToSingle(strArray[4]);
                            num7 = obj3.transform.localScale.z * Convert.ToSingle(strArray[5]);
                            obj3.transform.localScale = new Vector3(num5, num6, num7);
                            region.myBox = obj3;
                        }
                        RCRegions.Add(key, region);
                    }
                }
                else if (strArray[0].StartsWith("racing"))
                {
                    if (strArray[1].StartsWith("start"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        racingDoors?.Add(obj2);
                    }
                    else if (strArray[1].StartsWith("end"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        obj2.GetComponentInChildren<Collider>().gameObject.AddComponent<LevelTriggerRacingEnd>();
                    }
                    else if (strArray[1].StartsWith("kill"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        obj2.GetComponentInChildren<Collider>().gameObject.AddComponent<RacingKillTrigger>();
                    }
                    else if (strArray[1].StartsWith("checkpoint"))
                    {
                        obj2 = (GameObject)Instantiate((GameObject) RCassets.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                        num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                        num5 -= 0.001f;
                        num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                        num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        obj2.GetComponentInChildren<Collider>().gameObject.AddComponent<RacingCheckpointTrigger>();
                    }
                }
                else if (strArray[0].StartsWith("map"))
                {
                    if (strArray[1].StartsWith("disablebounds"))
                    {
                        Destroy(GameObject.Find("gameobjectOutSide"));
                        Instantiate(RCassets.Load("outside"));
                    }
                }
                else if (PhotonNetwork.isMasterClient && strArray[0].StartsWith("photon"))
                {
                    if (strArray[1].StartsWith("Cannon"))
                    {
                        if (strArray.Length > 15)
                        {
                            GameObject go = PhotonNetwork.Instantiate("RCAsset/" + strArray[1] + "Prop", new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[16]), Convert.ToSingle(strArray[17]), Convert.ToSingle(strArray[18])), 0);
                            go.GetComponent<CannonPropRegion>().settings = content[num];
                            go.GetPhotonView().RPC(Rpc.SetSize, PhotonTargets.AllBuffered, content[num]);
                        }
                        else
                        {
                            PhotonNetwork.Instantiate("RCAsset/" + strArray[1] + "Prop", new Vector3(Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4])), new Quaternion(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8])), 0).GetComponent<CannonPropRegion>().settings = content[num];
                        }
                    }
                    else
                    {
                        TitanSpawner item = new TitanSpawner();
                        num5 = 30f;
                        if (float.TryParse(strArray[2], out num3))
                        {
                            num5 = Mathf.Max(Convert.ToSingle(strArray[2]), 1f);
                        }
                        item.time = num5;
                        item.delay = num5;
                        item.name = strArray[1];
                        if (strArray[3] == "1")
                        {
                            item.endless = true;
                        }
                        else
                        {
                            item.endless = false;
                        }
                        item.location = new Vector3(Convert.ToSingle(strArray[4]), Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]));
                        titanSpawners.Add(item);
                    }
                }
            }
        }
    }

    private IEnumerator CustomLevelEnumerator(List<Player> players)
    {
        string[] strArray;
        if (currentLevel != string.Empty)
        {
            for (int i = 0; i < levelCache.Count; i++)
            {
                foreach (Player player in players)
                {
                    if (player.Properties.CurrentLevel != null && currentLevel != string.Empty && player.Properties.CurrentLevel == currentLevel)
                    {
                        if (i == 0)
                        {
                            strArray = new[] { "loadcached" };
                            photonView.RPC(Rpc.CustomLevel, player, new object[] { strArray });
                        }
                    }
                    else
                    {
                        photonView.RPC(Rpc.CustomLevel, player, new object[] { levelCache[i] });
                    }
                }
                if (i > 0)
                {
                    yield return new WaitForSeconds(0.75f);
                }
                else
                {
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }
        else
        {
            strArray = new[] { "loadempty" };
            foreach (Player player in players)
            {
                photonView.RPC(Rpc.CustomLevel, player, new object[] { strArray });
            }
            customLevelLoaded = true;
        }
    }

    public void DestroyAllExistingCloths()
    {
        Cloth[] clothArray = FindObjectsOfType<Cloth>();
        if (clothArray.Length > 0)
            foreach (Cloth cloth in clothArray)
                ClothFactory.DisposeObject(cloth.gameObject);
    }

    private void GameInfectionEnd()
    {
        int num;
        _infectionTitans.Clear();
        for (num = 0; num < PhotonNetwork.PlayerList.Length; num++)
        {
            Player player = PhotonNetwork.PlayerList[num];
            player.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.IsTitan, 1 }
            });
        }
        int length = PhotonNetwork.PlayerList.Length;
        int infectionMode = settings.InfectionTitanNumber;
        for (num = 0; num < PhotonNetwork.PlayerList.Length; num++)
        {
            Player player2 = PhotonNetwork.PlayerList[num];
            if (length > 0 && UnityEngine.Random.Range(0f, 1f) <= infectionMode / (float)length)
            {
                Hashtable hashtable2 = new Hashtable
                {
                    { PlayerProperty.IsTitan, 2 }
                };
                player2.SetCustomProperties(hashtable2);
                _infectionTitans.Add(player2.ID, 2);
                infectionMode--;
            }
            length--;
        }
        gameEndCD = 0f;
        RestartGame();
    }

    private void GameEndRC()
    {
        if (settings.IsPointMode)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.SetCustomProperties(new Hashtable
                {
                    { PlayerProperty.Kills, 0 },
                    { PlayerProperty.Deaths, 0 },
                    { PlayerProperty.MaxDamage, 0 },
                    { PlayerProperty.TotalDamage, 0 }
                });
            }
        }
        gameEndCD = 0f;
        RestartGame();
    }

    public void EnterSpecMode(bool enter)
    {
        if (enter)
        {
            spectateSprites = new List<GameObject>();
            foreach (UnityEngine.Object obj in FindObjectsOfType(typeof(GameObject)))
            {
                GameObject obj2 = (GameObject) obj;
                if (obj2.GetComponent<UISprite>() != null && obj2.activeInHierarchy)
                {
                    string name1 = obj2.name;
                    if (name1.Contains("blade") || name1.Contains("bullet") || name1.Contains("gas") || name1.Contains("flare") || name1.Contains("skill_cd"))
                    {
                        if (!spectateSprites.Contains(obj2))
                        {
                            spectateSprites.Add(obj2);
                        }
                        obj2.SetActive(false);
                    }
                }
            }
            string[] strArray2 = { "Flare", "LabelInfoBottomRight" };
            foreach (string str2 in strArray2)
            {
                GameObject item = GameObject.Find(str2);
                if (item != null)
                {
                    if (!spectateSprites.Contains(item))
                    {
                        spectateSprites.Add(item);
                    }
                    item.SetActive(false);
                }
            }
            foreach (HERO hero in instance.GetPlayers())
            {
                if (hero.photonView.isMine)
                {
                    PhotonNetwork.Destroy(hero.photonView);
                }
            }
            if (Player.Self.Properties.Alive != null && Player.Self.Properties.PlayerType == PlayerType.Titan && Player.Self.Properties.Alive.Value)
            {
                foreach (TITAN titan in instance.GetTitans())
                {
                    if (titan.photonView.isMine)
                    {
                        PhotonNetwork.Destroy(titan.photonView);
                    }
                }
            }
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
            instance.needChooseSide = false;
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.Original)
            {
                Screen.lockCursor = false;
                Screen.showCursor = false;
            }
            GameObject obj4 = GameObject.FindGameObjectWithTag("Player");
            if (obj4 != null && obj4.GetComponent<HERO>() != null)
            {
                UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj4);
            }
            else
            {
                UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            }
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            StartCoroutine(ReloadSkyEnumerator());
        }
        else
        {
            if (GameObject.Find("cross1") != null)
            {
                GameObject.Find("cross1").transform.localPosition = Vector3.up * 5000f;
            }
            if (spectateSprites != null)
            {
                foreach (GameObject obj2 in spectateSprites)
                {
                    if (obj2 != null)
                    {
                        obj2.SetActive(true);
                    }
                }
            }
            spectateSprites = new List<GameObject>();
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
            instance.needChooseSide = true;
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
        }
    }

    public void GameLose()
    {
        if (!(isWinning || isLosing))
        {
            isLosing = true;
            titanScore++;
            gameEndCD = gameEndTotalCDtime;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                photonView.RPC(Rpc.OnGameLose, PhotonTargets.Others, titanScore);
                if (settings.EnableChatFeed)
                    Mod.Interface.Chat.System("<color=#FFC000>({0:F2})</color> Round ended (game lose).", roundTime);
            }
        }
    }

    public void GameWin()
    {
        if (!isLosing && !isWinning)
        {
            isWinning = true;
            humanScore++;
            switch (IN_GAME_MAIN_CAMERA.GameMode)
            {
                case GameMode.Racing:
                    if (FengGameManagerMKII.settings.IsASORacing)
                        gameEndCD = 1000f;
                    else
                        gameEndCD = 20f;
                    
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                    {
                        photonView.RPC(Rpc.OnGameWin, PhotonTargets.Others, 0);
                        if (settings.EnableChatFeed)
                            Mod.Interface.Chat.System("<color=#FFC000>({0:F2})</color> Round ended (game lose).", roundTime);
                    }

                    break;
                case GameMode.PvpAHSS:
                    gameEndCD = gameEndTotalCDtime;
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                    {
                        object[] objArray3 = { teamWinner };
                        photonView.RPC(Rpc.OnGameWin, PhotonTargets.Others, objArray3);
                        if (settings.EnableChatFeed)
                            Mod.Interface.Chat.System("<color=#FFC000>({0:F2})</color> Round ended (game lose).", roundTime);
                    }
                    teamScores[teamWinner - 1]++;
                    break;
                default:
                    gameEndCD = gameEndTotalCDtime;
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                    {
                        object[] objArray4 = { humanScore };
                        photonView.RPC(Rpc.OnGameWin, PhotonTargets.Others, objArray4);
                        if (settings.EnableChatFeed)
                            Mod.Interface.Chat.System("<color=#FFC000>({0:F2})</color> Round ended (game lose).", roundTime);
                    }

                    break;
            }
        }
    }

    public ArrayList GetPlayers()
    {
        return Heroes;
    }

    public ArrayList GetTitans()
    {
        return Titans;
    }

    private static string GetHairType(int hairType)
    {
        if (hairType < 0)
            return "Random";
        
        return "Male " + hairType;
    }

    public static GameObject InstantiateCustomAsset(string key)
    {
        key = key.Substring(8);
        return (GameObject) RCassets.Load(key);
    }

    public bool IsAnyPlayerAlive() =>
        PhotonNetwork.PlayerList.Any(player => player.Properties.PlayerType == PlayerType.Human && player.Properties.Alive == true);

    public bool IsAnyTeamMemberAlive(int team) =>
        PhotonNetwork.PlayerList.Any(player =>
            player.Properties.PlayerType == PlayerType.Human &&
            player.Properties.Team == team &&
            player.Properties.Alive == true);

    public void KickPlayerRC(Player player, bool ban, string reason)
    {
        string str;
        if (OnPrivateServer)
        {
            str = player.Properties.Name;
            ServerCloseConnection(player, ban, str);
        }
        else
        {
            PhotonNetwork.DestroyPlayerObjects(player);
            PhotonNetwork.CloseConnection(player);
            photonView.RPC(Rpc.Ignore, PhotonTargets.Others, player.ID);
            if (!player.IsIgnored)
                player.Ignore();
            
            if (!(!ban || banHash.ContainsKey(player.ID)))
            {
                str = player.Properties.Name;
                banHash.Add(player.ID, str);
            }
            
            if (reason != string.Empty)
                Mod.Interface.Chat.System("Player " + player.ID + " was autobanned. Reason:" + reason);
        }
    }

    private void LoadConfig()
    {
        settings = GameSettingsManager.ImportRCSettings();
        GameSettingsManager.ApplySettings(settings);
        
//        distanceSlider = settings.CameraDistance;
//        mouseSlider = settings.MouseSensitivity;
//        qualitySlider = settings.GameQuality;
        
        linkHash = new[] { new Hashtable(), new Hashtable(), new Hashtable(), new Hashtable(), new Hashtable() };
        scroll = Vector2.zero;
        scroll2 = Vector2.zero;
        transparencySlider = 1f;
    }

    private void LoadMapCustom()
    {
        int num2;
        if (false) // If in LevelEditor //TODO: Probably broken || was `(int) settings[64] >= 100`
        {
            foreach (var o in FindObjectsOfType(typeof(GameObject)))
                if (o is GameObject obj)
                    if (obj.name.Contains("TREE") || obj.name.Contains("aot_supply") || obj.name.Contains("gameobjectOutSide"))
                        Destroy(obj);

            if (Shelter.TryFind("Cube_001", out GameObject go))
                go.renderer.material.mainTexture = ((Material) RCassets.Load("grass")).mainTexture;
            
            Instantiate(RCassets.Load("spawnPlayer"), new Vector3(-10f, 1f, -10f), new Quaternion(0f, 0f, 0f, 1f));
            
            if (Shelter.TryFind("skill_cd_bottom", out go))
                Destroy(go);
            if (Shelter.TryFind("GasUI", out go))
                Destroy(go);
            
            Camera.main.GetComponent<SpectatorMovement>().disable = true;
        }
        else
        {
            string[] strArray3;
            InstantiateTracker.instance?.Dispose();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
            {
                updateTime = 1f;
                if (oldScriptLogic != currentScriptLogic)
                {
                    intVariables.Clear();
                    boolVariables.Clear();
                    stringVariables.Clear();
                    floatVariables.Clear();
                    globalVariables.Clear();
                    RCEvents.Clear();
                    RCVariableNames.Clear();
                    playerVariables.Clear();
                    titanVariables.Clear();
                    RCRegionTriggers.Clear();
                    oldScriptLogic = currentScriptLogic;
                    CompileScript(currentScriptLogic);
                    if (RCEvents.ContainsKey("OnFirstLoad"))
                        ((RCEvent) RCEvents["OnFirstLoad"]).checkEvent();
                }
                if (RCEvents.ContainsKey("OnRoundStart"))
                    ((RCEvent) RCEvents["OnRoundStart"]).checkEvent();
                photonView.RPC(Rpc.SetMasterclient, PhotonTargets.All);
            }
            logicLoaded = true;
            racingSpawnPoint = new Vector3(0f, 0f, 0f);
            racingSpawnPointSet = false;
            racingDoors = new List<GameObject>();
            allowedToCannon = new Dictionary<int, CannonValues>();
            if (!Level.StartsWith("Custom") && ModuleManager.Enabled(nameof(ModuleEnableSkins)) && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || PhotonNetwork.isMasterClient))
            {
                var obj4 = GameObject.Find("aot_supply");
                if (obj4 != null && Minimap.instance != null)
                    Minimap.instance.TrackGameObjectOnMinimap(obj4, Color.white, false, true, Minimap.IconStyle.SUPPLY);

                StringBuilder url = new StringBuilder();
                StringBuilder str3 = new StringBuilder();
                StringBuilder randomDigits = new StringBuilder();
                strArray3 = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
                if (LevelInfoManager.Get(Level).LevelName.Contains("City"))
                {
                    var citySkin = settings.CitySkin;
                    foreach (var house in citySkin.Houses)
                        url.AppendFormat("{0},", house);
                    url.Remove(url.Length - 2, 1);
                    
                    for (int i = 0; i < 250; i++)
                        randomDigits.Append(Convert.ToString((int) UnityEngine.Random.Range(0f, 8f)));

                    str3.AppendFormat("{0},{1},{2}", 
                        citySkin.Ground, 
                        citySkin.Walls,
                        citySkin.Gate);
                    
                    for (int i = 0; i < citySkin.Skybox.Length; i++)
                        strArray3[i] = citySkin.Skybox[i];
                }
                else if (LevelInfoManager.Get(Level).LevelName.Contains("Forest"))
                {
                    var forestSkin = settings.ForestSkin;
                    foreach (var tree in forestSkin.Trees)
                        url.AppendFormat("{0},", tree);
                    url.Remove(url.Length - 2, 1);
                    
                    foreach (var leaf in forestSkin.Leaves)
                        str3.AppendFormat("{0},", leaf);
                    str3.Append(forestSkin.Ground);
                    
                    for (int i = 0; i < 150; i++)
                    {
                        string str5 = Convert.ToString((int) UnityEngine.Random.Range(0f, 8f));
                        randomDigits.Append(str5);
                        if (settings.Randomize)
                            randomDigits.Append(str5);
                        else
                            randomDigits.Append(Convert.ToString((int) UnityEngine.Random.Range(0f, 8f)));
                    }

                    forestSkin.Skybox.CopyTo(strArray3, 0);
                }
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    StartCoroutine(LoadSkinEnumerator(randomDigits.ToString(), url.ToString(), str3.ToString(), strArray3));
                }
                else if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC(Rpc.LoadSkin, PhotonTargets.AllBuffered, randomDigits.ToString(), url.ToString(), str3.ToString(), strArray3);
                }
            }
            else if (Level.StartsWith("Custom") && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
            {
                foreach (GameObject spawn in GameObject.FindGameObjectsWithTag(PlayerRespawnTag))
                    if (spawn != null)
                        spawn.transform.position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));

                
                foreach (UnityEngine.Object o in FindObjectsOfType(typeof(GameObject)))
                {
                    if (o is GameObject obj)
                    {
                        if (obj.name.Contains("TREE") || obj.name.Contains("aot_supply"))
                            Destroy(obj);
                        if (obj.name == "Cube_001" && !obj.transform.parent.gameObject.CompareTag("Player") && obj.renderer != null)
                        {
                            groundList.Add(obj);
                            obj.renderer.material.mainTexture = ((Material) RCassets.Load("grass")).mainTexture;
                        }
                    }
                    
                }
                if (PhotonNetwork.isMasterClient)
                {
                    var customSkin = settings.CustomSkin;
                    strArray3 = new string[6];
                    customSkin.Skybox.CopyTo(strArray3, 0);
                    
                    strArray3[6] = customSkin.Ground;
                    photonView.RPC(Rpc.ClearLevel, PhotonTargets.AllBuffered, strArray3, settings.GameType);
                    RCRegions.Clear();
                    if (oldScript != currentScript)
                    {
                        Hashtable hashtable;
                        levelCache.Clear();
                        titanSpawns.Clear();
                        playerSpawnsC.Clear();
                        playerSpawnsM.Clear();
                        titanSpawners.Clear();
                        currentLevel = string.Empty;
                        if (currentScript == string.Empty)
                        {
                            hashtable = new Hashtable
                            {
                                { PlayerProperty.CurrentLevel, currentLevel }
                            };
                            Player.Self.SetCustomProperties(hashtable);
                            oldScript = currentScript;
                        }
                        else
                        {
                            string[] strArray4 = Regex.Replace(currentScript, @"\s+|\r\n|\n|\r", string.Empty).Split(';');
                            for (int i = 0; i < Mathf.FloorToInt(((strArray4.Length - 1) / 100f)) + 1; i++)
                            {
                                string[] strArray5;
                                int num7;
                                string[] strArray6;
                                string str6;
                                if (i < strArray4.Length / 100)
                                {
                                    strArray5 = new string[101];
                                    num7 = 0;
                                    num2 = 100 * i;
                                    while (num2 < 100 * i + 100)
                                    {
                                        if (strArray4[num2].StartsWith("spawnpoint"))
                                        {
                                            strArray6 = strArray4[num2].Split(',');
                                            switch (strArray6[1])
                                            {
                                                case "titan":
                                                    titanSpawns.Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                                    break;
                                                case "playerC":
                                                    playerSpawnsC.Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                                    break;
                                                case "playerM":
                                                    playerSpawnsM.Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                                    break;
                                            }
                                        }
                                        strArray5[num7] = strArray4[num2];
                                        num7++;
                                        num2++;
                                    }
                                    str6 = UnityEngine.Random.Range(10000, 99999).ToString();
                                    strArray5[100] = str6;
                                    currentLevel = currentLevel + str6;
                                    levelCache.Add(strArray5);
                                }
                                else
                                {
                                    strArray5 = new string[strArray4.Length % 100 + 1];
                                    num7 = 0;
                                    for (num2 = 100 * i; num2 < 100 * i + strArray4.Length % 100; num2++)
                                    {
                                        if (strArray4[num2].StartsWith("spawnpoint"))
                                        {
                                            strArray6 = strArray4[num2].Split(',');
                                            if (strArray6[1] == "titan")
                                            {
                                                titanSpawns.Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                            }
                                            else if (strArray6[1] == "playerC")
                                            {
                                                playerSpawnsC.Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                            }
                                            else if (strArray6[1] == "playerM")
                                            {
                                                playerSpawnsM.Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                            }
                                        }
                                        strArray5[num7] = strArray4[num2];
                                        num7++;
                                    }
                                    str6 = UnityEngine.Random.Range(10000, 99999).ToString();
                                    strArray5[strArray4.Length % 100] = str6;
                                    currentLevel = currentLevel + str6;
                                    levelCache.Add(strArray5);
                                }
                            }
                            List<string> list = new List<string>();
                            foreach (Vector3 vector in titanSpawns)
                            {
                                list.Add("titan," + vector.x + "," + vector.y + "," + vector.z);
                            }
                            foreach (Vector3 vector in playerSpawnsC)
                            {
                                list.Add("playerC," + vector.x + "," + vector.y + "," + vector.z);
                            }
                            foreach (Vector3 vector in playerSpawnsM)
                            {
                                list.Add("playerM," + vector.x + "," + vector.y + "," + vector.z);
                            }
                            string item = "a" + UnityEngine.Random.Range(10000, 99999);
                            list.Add(item);
                            currentLevel = item + currentLevel;
                            levelCache.Insert(0, list.ToArray());
                            string str8 = "z" + UnityEngine.Random.Range(10000, 99999);
                            levelCache.Add(new[] { str8 });
                            currentLevel = currentLevel + str8;
                            hashtable = new Hashtable
                            {
                                { PlayerProperty.CurrentLevel, currentLevel }
                            };
                            Player.Self.SetCustomProperties(hashtable);
                            oldScript = currentScript;
                        }
                    }
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        Player player = PhotonNetwork.PlayerList[i];
                        if (!player.IsMasterClient)
                        {
                            playersRPC.Add(player);
                        }
                    }
                    StartCoroutine(CustomLevelEnumerator(playersRPC));
                    StartCoroutine(CustomLevelCache());
                }
            }
        }
    }

    private IEnumerator LoadSkinEnumerator(string digits, string treeSkins, string planeSkins, string[] skybox)
    {
        bool mipmap = settings.UseMipmap;
        bool unloadAssets = false;
        
        if (skybox.Length >= 6)
        {
            string iteratorVariable3 = string.Join(",", skybox);
            if (!linkHash[1].ContainsKey(iteratorVariable3))
            {
                unloadAssets = true;
                Material material = Camera.main.GetComponent<Skybox>().material;

                string[] skyboxSide = {"_FrontTex", "_BackTex", "_LeftTex", "_RightTex", "_UpTex", "_DownTex"};
                for (int i = 0; i < 6; i++)
                {
                    var url = skybox[i];
                    if (!Utility.IsValidImageUrl(url))
                        continue;
                    using (WWW www = new WWW(url))
                    {
                        yield return www;
                        if (www.error != null)
                            continue;
                        material.SetTexture(skyboxSide[i], RCextensions.LoadImageRC(www, mipmap, 500000));
                    }
                }
                
                Camera.main.GetComponent<Skybox>().material = material;
                linkHash[1].Add(iteratorVariable3, material);
                skyMaterial = material;
            }
            else
            {
                Camera.main.GetComponent<Skybox>().material = (Material)linkHash[1][iteratorVariable3];
                skyMaterial = (Material)linkHash[1][iteratorVariable3];
            }
        }
        if (LevelInfoManager.Get(Level).LevelName.Contains("Forest"))
        {
            string[] url = treeSkins.Split(',');
            string[] url2 = planeSkins.Split(',');
            
            foreach (UnityEngine.Object o in FindObjectsOfType(typeof(GameObject)))
            {
                if (o is GameObject currentGameObject)
                {
                    int startIndex = 0;
                    if (currentGameObject.name.Contains("TREE") && digits.Length > startIndex + 1)
                    {
                        string number = digits.Substring(startIndex, 1);
                        string number2 = digits.Substring(startIndex + 1, 1);
                        if (int.TryParse(number, out var i) && int.TryParse(number2, out var x) && i >= 0 && i < 8 && x >= 0 && x < 8 && url.Length >= 8 && url2.Length >= 8 && url[i] != null && url2[x] != null)
                        {
                            string currentUrl = url[i];
                            string planes = url2[x];
                            foreach (Renderer myRenderer in currentGameObject.GetComponentsInChildren<Renderer>())
                            {
                                if (myRenderer.name.Contains("Cube"))
                                {
                                    if (Utility.IsValidImageUrl(currentUrl))
                                    {
                                        if (!linkHash[2].ContainsKey(currentUrl))
                                        {
                                            unloadAssets = true;
                                            using (WWW www = new WWW(currentUrl))
                                            {
                                                yield return www;
                                                if (www.error != null)
                                                    continue;
                                                myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 1000000);
                                            }
                                            linkHash[2].Add(currentUrl, myRenderer.material);
                                            myRenderer.material = (Material)linkHash[2][currentUrl];
                                        }
                                        else
                                        {
                                            myRenderer.material = (Material)linkHash[2][currentUrl];
                                        }
                                    }
                                }
                                else if (myRenderer.name.Contains("Plane_031"))
                                {
                                    if (Utility.IsValidImageUrl(planes))
                                    {
                                        if (!linkHash[0].ContainsKey(planes))
                                        {
                                            unloadAssets = true;
                                            using (WWW www = new WWW(planes))
                                            {
                                                yield return www;
                                                if (www.error != null)
                                                    continue;
                                                myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                                            }
                                            linkHash[0].Add(planes, myRenderer.material);
                                            myRenderer.material = (Material)linkHash[0][planes];
                                        }
                                        else
                                        {
                                            myRenderer.material = (Material)linkHash[0][planes];
                                        }
                                    }
                                    else if (planes.EqualsIgnoreCase("transparent"))
                                    {
                                        myRenderer.enabled = false;
                                    }
                                }
                            }
                        }
                        startIndex += 2;
                    }
                    else if (currentGameObject.name.Contains("Cube_001") && !currentGameObject.transform.parent.gameObject.CompareTag("Player") && url2.Length > 8 && url2[8] != null)
                    {
                        string currentUrl = url2[8];
                        if (Utility.IsValidImageUrl(currentUrl))
                        {
                            foreach (Renderer iteratorVariable39 in currentGameObject.GetComponentsInChildren<Renderer>())
                            {
                                if (!linkHash[0].ContainsKey(currentUrl))
                                {
                                    unloadAssets = true;
                                    using (WWW www = new WWW(currentUrl))
                                    {
                                        yield return www;
                                        if (www.error != null)
                                            continue;
                                        iteratorVariable39.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                                    }
                                    linkHash[0].Add(currentUrl, iteratorVariable39.material);
                                    iteratorVariable39.material = (Material)linkHash[0][currentUrl];
                                }
                                else
                                {
                                    iteratorVariable39.material = (Material)linkHash[0][currentUrl];
                                }
                            }
                        }
                        else if (currentUrl.EqualsIgnoreCase("transparent"))
                        {
                            foreach (Renderer renderer1 in currentGameObject.GetComponentsInChildren<Renderer>())
                            {
                                renderer1.enabled = false;
                            }
                        }
                    }
                }
            }
        }
        else if (LevelInfoManager.Get(Level).LevelName.Contains("City"))
        {
            string[] trees = treeSkins.Split(',');
            string[] planes = planeSkins.Split(',');
            int index = 0;
            var gameObjects = FindObjectsOfType(typeof(GameObject));
            foreach (UnityEngine.Object o in gameObjects)
            {
                if (o is GameObject go && go.name.Contains("Cube_") && !go.transform.parent.gameObject.CompareTag("Player"))
                {
                    if (go.name.EndsWith("001"))
                    {
                        if (planes.Length > 0 && planes[0] != null)
                        {
                            string url = planes[0];
                            if (Utility.IsValidImageUrl(url))
                            {
                                foreach (Renderer myRenderer in go.GetComponentsInChildren<Renderer>())
                                {
                                    if (!linkHash[0].ContainsKey(url))
                                    {
                                        unloadAssets = true;
                                        using (WWW www = new WWW(url))
                                        {
                                            yield return www;
                                            if (www.error != null)
                                                continue;
                                            myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                                        }
                                        linkHash[0].Add(url, myRenderer.material);
                                        myRenderer.material = (Material)linkHash[0][url];
                                    }
                                    else
                                    {
                                        myRenderer.material = (Material)linkHash[0][url];
                                    }
                                }
                            }
                            else if (url.EqualsIgnoreCase("transparent"))
                            {
                                foreach (Renderer renderer1 in go.GetComponentsInChildren<Renderer>())
                                {
                                    renderer1.enabled = false;
                                }
                            }
                        }
                    }
                    else if (go.name.EndsWith("006") || go.name.EndsWith("007") || go.name.EndsWith("015") || go.name.EndsWith("000") || go.name.EndsWith("002") && go.transform.position.x == 0f && go.transform.position.y == 0f && go.transform.position.z == 0f)
                    {
                        if (planes.Length > 0 && planes[1] != null)
                        {
                            string url = planes[1];
                            if (Utility.IsValidImageUrl(url))
                            {
                                foreach (Renderer myRenderer in go.GetComponentsInChildren<Renderer>())
                                {
                                    if (!linkHash[0].ContainsKey(url))
                                    {
                                        unloadAssets = true;
                                        using (WWW www = new WWW(url))
                                        {
                                            yield return www;
                                            if (www.error != null)
                                                continue;
                                            myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                                        }
                                        linkHash[0].Add(url, myRenderer.material);
                                        myRenderer.material = (Material)linkHash[0][url];
                                    }
                                    else
                                    {
                                        myRenderer.material = (Material)linkHash[0][url];
                                    }
                                }
                            }
                        }
                    }
                    else if (go.name.EndsWith("005") || go.name.EndsWith("003") || go.name.EndsWith("002") && (go.transform.position.x != 0f || go.transform.position.y != 0f || go.transform.position.z != 0f) && digits.Length > index)
                    {
                        string iteratorVariable57 = digits.Substring(index, 1);
                        if (int.TryParse(iteratorVariable57, out var iteratorVariable56) && iteratorVariable56 >= 0 && iteratorVariable56 < 8 && trees.Length >= 8 && trees[iteratorVariable56] != null)
                        {
                            string url = trees[iteratorVariable56];
                            if (Utility.IsValidImageUrl(url))
                            {
                                foreach (Renderer iteratorVariable59 in go.GetComponentsInChildren<Renderer>())
                                {
                                    if (!linkHash[2].ContainsKey(url))
                                    {
                                        unloadAssets = true;
                                        using (WWW www = new WWW(url))
                                        {
                                            yield return www;
                                            if (www.error != null)
                                                continue;
                                            iteratorVariable59.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 1000000);
                                        }
                                        linkHash[2].Add(url, iteratorVariable59.material);
                                        iteratorVariable59.material = (Material)linkHash[2][url];
                                    }
                                    else
                                    {
                                        iteratorVariable59.material = (Material)linkHash[2][url];
                                    }
                                }
                            }
                        }
                        index++;
                    }
                    else if ((go.name.EndsWith("019") || go.name.EndsWith("020")) && planes.Length > 2 && planes[2] != null)
                    {
                        string url = planes[2];
                        if (Utility.IsValidImageUrl(url))
                        {
                            foreach (Renderer myRenderer in go.GetComponentsInChildren<Renderer>())
                            {
                                if (!linkHash[2].ContainsKey(url))
                                {
                                    unloadAssets = true;
                                    using (WWW www = new WWW(url))
                                    {
                                        yield return www;
                                        if (www.error != null)
                                            continue;
                                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 1000000);
                                    }
                                    linkHash[2].Add(url, myRenderer.material);
                                    myRenderer.material = (Material)linkHash[2][url];
                                }
                                else
                                {
                                    myRenderer.material = (Material)linkHash[2][url];
                                }
                            }
                        }
                    }
                }
            }
        }
        Minimap.TryRecaptureInstance();
        if (unloadAssets)
        {
            UnloadAssets();
        }
    }

    public void MultiplayerRacingFinish()
    {
        float time1 = roundTime - 20f;
        if (PhotonNetwork.isMasterClient)
        {
            GetRacingResult(Shelter.Profile.Name, time1);
        }
        else
        {
            photonView.RPC(Rpc.GetRacingResults, PhotonTargets.MasterClient, Shelter.Profile.Name, time1);
        }
        GameWin();
    }

    public void SpawnPlayerTitanAfterGameEnd(string id) // Called after choose of titan
    {
        myLastHero = id.ToUpper();
        Player.Self.SetCustomProperties(new Hashtable
        {
            { "dead", true },
            { PlayerProperty.IsTitan, 2 }
        });
        if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public void SpawnPlayerTitanAfterGameEndRC(string id)
    {
        myLastHero = id.ToUpper();
        Player.Self.SetCustomProperties(new Hashtable
        {
            { "dead", true },
            { PlayerProperty.IsTitan, 2 }
        });
        if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public void SpawnPlayerAfterGameEnd(string id)
    {
        myLastHero = id.ToUpper();
        Player.Self.SetCustomProperties(new Hashtable
        {
            { "dead", true },
            { PlayerProperty.IsTitan, 1 }
        });
        if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = false;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public void SpawnPlayerAfterGameEndRC(string id)
    {
        myLastHero = id.ToUpper();
        Player.Self.SetCustomProperties(new Hashtable
        {
            { "dead", true },
            { PlayerProperty.IsTitan, 1 }
        });
        if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = false;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public int OperatorType(string str, int condition)
    {
        switch (condition)
        {
            case 0:
            case 3:
                if (!str.StartsWith("Equals"))
                {
                    if (str.StartsWith("NotEquals"))
                    {
                        return 5;
                    }
                    if (!str.StartsWith("LessThan"))
                    {
                        if (str.StartsWith("LessThanOrEquals"))
                        {
                            return 1;
                        }
                        if (str.StartsWith("GreaterThanOrEquals"))
                        {
                            return 3;
                        }
                        if (str.StartsWith("GreaterThan"))
                        {
                            return 4;
                        }
                    }
                    return 0;
                }
                return 2;

            case 1:
            case 4:
            case 5:
                if (!str.StartsWith("Equals"))
                {
                    if (str.StartsWith("NotEquals"))
                    {
                        return 5;
                    }
                    return 0;
                }
                return 2;

            case 2:
                if (!str.StartsWith("Equals"))
                {
                    if (str.StartsWith("NotEquals"))
                    {
                        return 1;
                    }
                    if (str.StartsWith("Contains"))
                    {
                        return 2;
                    }
                    if (str.StartsWith("NotContains"))
                    {
                        return 3;
                    }
                    if (str.StartsWith("StartsWith"))
                    {
                        return 4;
                    }
                    if (str.StartsWith("NotStartsWith"))
                    {
                        return 5;
                    }
                    if (str.StartsWith("EndsWith"))
                    {
                        return 6;
                    }
                    if (str.StartsWith("NotEndsWith"))
                    {
                        return 7;
                    }
                    return 0;
                }
                return 0;
        }
        return 0;
    }

    public RCEvent ParseBlock(string[] stringArray, int eventClass, int eventType, RCCondition condition)
    {
        List<RCAction> sentTrueActions = new List<RCAction>();
        RCEvent event2 = new RCEvent(null, null, 0, 0);
        for (int i = 0; i < stringArray.Length; i++)
        {
            int num2;
            int num3;
            int num4;
            int length;
            string[] strArray;
            int num6;
            int num7;
            int index;
            int num9;
            string str;
            int num10;
            int num11;
            int num12;
            string[] strArray2;
            RCCondition condition2;
            RCEvent event3;
            RCAction action;
            if (stringArray[i].StartsWith("If") && stringArray[i + 1] == "{")
            {
                num2 = i + 2;
                num3 = i + 2;
                num4 = 0;
                length = i + 2;
                while (length < stringArray.Length)
                {
                    if (stringArray[length] == "{")
                    {
                        num4++;
                    }
                    if (stringArray[length] == "}")
                    {
                        if (num4 > 0)
                        {
                            num4--;
                        }
                        else
                        {
                            num3 = length - 1;
                            length = stringArray.Length;
                        }
                    }
                    length++;
                }
                strArray = new string[num3 - num2 + 1];
                num6 = 0;
                num7 = num2;
                while (num7 <= num3)
                {
                    strArray[num6] = stringArray[num7];
                    num6++;
                    num7++;
                }
                index = stringArray[i].IndexOf('(');
                num9 = stringArray[i].LastIndexOf(')');
                str = stringArray[i].Substring(index + 1, num9 - index - 1);
                num10 = StringToType(str);
                num11 = str.IndexOf('.');
                str = str.Substring(num11 + 1);
                num12 = OperatorType(str, num10);
                index = str.IndexOf('(');
                num9 = str.LastIndexOf(')');
                strArray2 = str.Substring(index + 1, num9 - index - 1).Split(',');
                condition2 = new RCCondition(num12, num10, ReturnHelper(strArray2[0]), ReturnHelper(strArray2[1]));
                event3 = ParseBlock(strArray, 1, 0, condition2);
                action = new RCAction(0, 0, event3, null);
                event2 = event3;
                sentTrueActions.Add(action);
                i = num3;
            }
            else if (stringArray[i].StartsWith("While") && stringArray[i + 1] == "{")
            {
                num2 = i + 2;
                num3 = i + 2;
                num4 = 0;
                length = i + 2;
                while (length < stringArray.Length)
                {
                    if (stringArray[length] == "{")
                    {
                        num4++;
                    }
                    if (stringArray[length] == "}")
                    {
                        if (num4 > 0)
                        {
                            num4--;
                        }
                        else
                        {
                            num3 = length - 1;
                            length = stringArray.Length;
                        }
                    }
                    length++;
                }
                strArray = new string[num3 - num2 + 1];
                num6 = 0;
                num7 = num2;
                while (num7 <= num3)
                {
                    strArray[num6] = stringArray[num7];
                    num6++;
                    num7++;
                }
                index = stringArray[i].IndexOf('(');
                num9 = stringArray[i].LastIndexOf(')');
                str = stringArray[i].Substring(index + 1, num9 - index - 1);
                num10 = StringToType(str);
                num11 = str.IndexOf('.');
                str = str.Substring(num11 + 1);
                num12 = OperatorType(str, num10);
                index = str.IndexOf('(');
                num9 = str.LastIndexOf(')');
                strArray2 = str.Substring(index + 1, num9 - index - 1).Split(',');
                condition2 = new RCCondition(num12, num10, ReturnHelper(strArray2[0]), ReturnHelper(strArray2[1]));
                event3 = ParseBlock(strArray, 3, 0, condition2);
                action = new RCAction(0, 0, event3, null);
                sentTrueActions.Add(action);
                i = num3;
            }
            else if (stringArray[i].StartsWith("ForeachTitan") && stringArray[i + 1] == "{")
            {
                num2 = i + 2;
                num3 = i + 2;
                num4 = 0;
                length = i + 2;
                while (length < stringArray.Length)
                {
                    if (stringArray[length] == "{")
                    {
                        num4++;
                    }
                    if (stringArray[length] == "}")
                    {
                        if (num4 > 0)
                        {
                            num4--;
                        }
                        else
                        {
                            num3 = length - 1;
                            length = stringArray.Length;
                        }
                    }
                    length++;
                }
                strArray = new string[num3 - num2 + 1];
                num6 = 0;
                num7 = num2;
                while (num7 <= num3)
                {
                    strArray[num6] = stringArray[num7];
                    num6++;
                    num7++;
                }
                index = stringArray[i].IndexOf('(');
                num9 = stringArray[i].LastIndexOf(')');
                str = stringArray[i].Substring(index + 2, num9 - index - 3);
                event3 = ParseBlock(strArray, 2, 0, null);
                event3.foreachVariableName = str;
                action = new RCAction(0, 0, event3, null);
                sentTrueActions.Add(action);
                i = num3;
            }
            else if (stringArray[i].StartsWith("ForeachPlayer") && stringArray[i + 1] == "{")
            {
                num2 = i + 2;
                num3 = i + 2;
                num4 = 0;
                length = i + 2;
                while (length < stringArray.Length)
                {
                    if (stringArray[length] == "{")
                    {
                        num4++;
                    }
                    if (stringArray[length] == "}")
                    {
                        if (num4 > 0)
                        {
                            num4--;
                        }
                        else
                        {
                            num3 = length - 1;
                            length = stringArray.Length;
                        }
                    }
                    length++;
                }
                strArray = new string[num3 - num2 + 1];
                num6 = 0;
                num7 = num2;
                while (num7 <= num3)
                {
                    strArray[num6] = stringArray[num7];
                    num6++;
                    num7++;
                }
                index = stringArray[i].IndexOf('(');
                num9 = stringArray[i].LastIndexOf(')');
                str = stringArray[i].Substring(index + 2, num9 - index - 3);
                event3 = ParseBlock(strArray, 2, 1, null);
                event3.foreachVariableName = str;
                action = new RCAction(0, 0, event3, null);
                sentTrueActions.Add(action);
                i = num3;
            }
            else if (stringArray[i].StartsWith("Else") && stringArray[i + 1] == "{")
            {
                num2 = i + 2;
                num3 = i + 2;
                num4 = 0;
                length = i + 2;
                while (length < stringArray.Length)
                {
                    if (stringArray[length] == "{")
                    {
                        num4++;
                    }
                    if (stringArray[length] == "}")
                    {
                        if (num4 > 0)
                        {
                            num4--;
                        }
                        else
                        {
                            num3 = length - 1;
                            length = stringArray.Length;
                        }
                    }
                    length++;
                }
                strArray = new string[num3 - num2 + 1];
                num6 = 0;
                for (num7 = num2; num7 <= num3; num7++)
                {
                    strArray[num6] = stringArray[num7];
                    num6++;
                }
                if (stringArray[i] == "Else")
                {
                    event3 = ParseBlock(strArray, 0, 0, null);
                    action = new RCAction(0, 0, event3, null);
                    event2.setElse(action);
                    i = num3;
                }
                else if (stringArray[i].StartsWith("Else If"))
                {
                    index = stringArray[i].IndexOf('(');
                    num9 = stringArray[i].LastIndexOf(')');
                    str = stringArray[i].Substring(index + 1, num9 - index - 1);
                    num10 = StringToType(str);
                    num11 = str.IndexOf('.');
                    str = str.Substring(num11 + 1);
                    num12 = OperatorType(str, num10);
                    index = str.IndexOf('(');
                    num9 = str.LastIndexOf(')');
                    strArray2 = str.Substring(index + 1, num9 - index - 1).Split(',');
                    condition2 = new RCCondition(num12, num10, ReturnHelper(strArray2[0]), ReturnHelper(strArray2[1]));
                    event3 = ParseBlock(strArray, 1, 0, condition2);
                    action = new RCAction(0, 0, event3, null);
                    event2.setElse(action);
                    i = num3;
                }
            }
            else
            {
                int num13;
                int num14;
                int num15;
                int num16;
                string str2;
                string[] strArray3;
                RCActionHelper helper;
                RCActionHelper helper2;
                RCActionHelper helper3;
                if (stringArray[i].StartsWith("VariableInt"))
                {
                    num13 = 1;
                    num14 = stringArray[i].IndexOf('.');
                    num15 = stringArray[i].IndexOf('(');
                    num16 = stringArray[i].LastIndexOf(')');
                    str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                    strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                    if (str2.StartsWith("SetRandom"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        helper3 = ReturnHelper(strArray3[2]);
                        action = new RCAction(num13, 12, null, new[] { helper, helper2, helper3 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Set"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Add"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 1, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Subtract"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 2, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Multiply"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 3, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Divide"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 4, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Modulo"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 5, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Power"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 6, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableBool"))
                {
                    num13 = 2;
                    num14 = stringArray[i].IndexOf('.');
                    num15 = stringArray[i].IndexOf('(');
                    num16 = stringArray[i].LastIndexOf(')');
                    str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                    strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                    if (str2.StartsWith("SetToOpposite"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        action = new RCAction(num13, 11, null, new[] { helper });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetRandom"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        action = new RCAction(num13, 12, null, new[] { helper });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Set"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableString"))
                {
                    num13 = 3;
                    num14 = stringArray[i].IndexOf('.');
                    num15 = stringArray[i].IndexOf('(');
                    num16 = stringArray[i].LastIndexOf(')');
                    str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                    strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                    if (str2.StartsWith("Set"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Concat"))
                    {
                        RCActionHelper[] helpers = new RCActionHelper[strArray3.Length];
                        for (length = 0; length < strArray3.Length; length++)
                        {
                            helpers[length] = ReturnHelper(strArray3[length]);
                        }
                        action = new RCAction(num13, 7, null, helpers);
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Append"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 8, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Replace"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        helper3 = ReturnHelper(strArray3[2]);
                        action = new RCAction(num13, 10, null, new[] { helper, helper2, helper3 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Remove"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 9, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableFloat"))
                {
                    num13 = 4;
                    num14 = stringArray[i].IndexOf('.');
                    num15 = stringArray[i].IndexOf('(');
                    num16 = stringArray[i].LastIndexOf(')');
                    str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                    strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                    if (str2.StartsWith("SetRandom"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        helper3 = ReturnHelper(strArray3[2]);
                        action = new RCAction(num13, 12, null, new[] { helper, helper2, helper3 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Set"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Add"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 1, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Subtract"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 2, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Multiply"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 3, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Divide"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 4, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Modulo"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 5, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Power"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 6, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariablePlayer"))
                {
                    num13 = 5;
                    num14 = stringArray[i].IndexOf('.');
                    num15 = stringArray[i].IndexOf('(');
                    num16 = stringArray[i].LastIndexOf(')');
                    str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                    strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                    if (str2.StartsWith("Set"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableTitan"))
                {
                    num13 = 6;
                    num14 = stringArray[i].IndexOf('.');
                    num15 = stringArray[i].IndexOf('(');
                    num16 = stringArray[i].LastIndexOf(')');
                    str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                    strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                    if (str2.StartsWith("Set"))
                    {
                        helper = ReturnHelper(strArray3[0]);
                        helper2 = ReturnHelper(strArray3[1]);
                        action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                        sentTrueActions.Add(action);
                    }
                }
                else
                {
                    RCActionHelper helper4;
                    if (stringArray[i].StartsWith("Player"))
                    {
                        num13 = 7;
                        num14 = stringArray[i].IndexOf('.');
                        num15 = stringArray[i].IndexOf('(');
                        num16 = stringArray[i].LastIndexOf(')');
                        str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                        strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                        if (str2.StartsWith("KillPlayer"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 0, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SpawnPlayerAt"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            helper3 = ReturnHelper(strArray3[2]);
                            helper4 = ReturnHelper(strArray3[3]);
                            action = new RCAction(num13, 2, null, new[] { helper, helper2, helper3, helper4 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SpawnPlayer"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            action = new RCAction(num13, 1, null, new[] { helper });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("MovePlayer"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            helper3 = ReturnHelper(strArray3[2]);
                            helper4 = ReturnHelper(strArray3[3]);
                            action = new RCAction(num13, 3, null, new[] { helper, helper2, helper3, helper4 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetKills"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 4, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetDeaths"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 5, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetMaxDmg"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 6, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetTotalDmg"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 7, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetName"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 8, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetGuildName"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 9, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetTeam"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 10, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetCustomInt"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 11, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetCustomBool"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 12, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetCustomString"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 13, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetCustomFloat"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 14, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                    }
                    else if (stringArray[i].StartsWith("Titan"))
                    {
                        num13 = 8;
                        num14 = stringArray[i].IndexOf('.');
                        num15 = stringArray[i].IndexOf('(');
                        num16 = stringArray[i].LastIndexOf(')');
                        str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                        strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                        if (str2.StartsWith("KillTitan"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            helper3 = ReturnHelper(strArray3[2]);
                            action = new RCAction(num13, 0, null, new[] { helper, helper2, helper3 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SpawnTitanAt"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            helper3 = ReturnHelper(strArray3[2]);
                            helper4 = ReturnHelper(strArray3[3]);
                            RCActionHelper helper5 = ReturnHelper(strArray3[4]);
                            RCActionHelper helper6 = ReturnHelper(strArray3[5]);
                            RCActionHelper helper7 = ReturnHelper(strArray3[6]);
                            action = new RCAction(num13, 2, null, new[] { helper, helper2, helper3, helper4, helper5, helper6, helper7 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SpawnTitan"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            helper3 = ReturnHelper(strArray3[2]);
                            helper4 = ReturnHelper(strArray3[3]);
                            action = new RCAction(num13, 1, null, new[] { helper, helper2, helper3, helper4 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("SetHealth"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            action = new RCAction(num13, 3, null, new[] { helper, helper2 });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("MoveTitan"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            helper2 = ReturnHelper(strArray3[1]);
                            helper3 = ReturnHelper(strArray3[2]);
                            helper4 = ReturnHelper(strArray3[3]);
                            action = new RCAction(num13, 4, null, new[] { helper, helper2, helper3, helper4 });
                            sentTrueActions.Add(action);
                        }
                    }
                    else if (stringArray[i].StartsWith("Game"))
                    {
                        num13 = 9;
                        num14 = stringArray[i].IndexOf('.');
                        num15 = stringArray[i].IndexOf('(');
                        num16 = stringArray[i].LastIndexOf(')');
                        str2 = stringArray[i].Substring(num14 + 1, num15 - num14 - 1);
                        strArray3 = stringArray[i].Substring(num15 + 1, num16 - num15 - 1).Split(',');
                        if (str2.StartsWith("PrintMessage"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            action = new RCAction(num13, 0, null, new[] { helper });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("LoseGame"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            action = new RCAction(num13, 2, null, new[] { helper });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("WinGame"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            action = new RCAction(num13, 1, null, new[] { helper });
                            sentTrueActions.Add(action);
                        }
                        else if (str2.StartsWith("Restart"))
                        {
                            helper = ReturnHelper(strArray3[0]);
                            action = new RCAction(num13, 3, null, new[] { helper });
                            sentTrueActions.Add(action);
                        }
                    }
                }
            }
        }
        return new RCEvent(condition, sentTrueActions, eventClass, eventType);
    }

    public void PlayerKillInfoSingleplayerUpdate(int dmg)
    {
    }

    public void PlayerKillInfoUpdate(Player player, int dmg)
    {
        player.SetCustomProperties(new Hashtable
        {
            { PlayerProperty.Kills, player.Properties.Kills + 1 },
            { PlayerProperty.MaxDamage, Mathf.Max(dmg, player.Properties.MaxDamage) },
            { PlayerProperty.TotalDamage, player.Properties.TotalDamage + dmg }
        });
    }

    public GameObject RandomSpawnOneTitan(string place, int rate)
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag(place);
        int index = UnityEngine.Random.Range(0, objArray.Length);
        GameObject obj2 = objArray[index];
        while (objArray[index] == null)
        {
            index = UnityEngine.Random.Range(0, objArray.Length);
            obj2 = objArray[index];
        }
        objArray[index] = null;
        return SpawnTitan(rate, obj2.transform.position, obj2.transform.rotation);
    }

    public void RandomSpawnTitan(string place, int rate, int num, bool punk = false)
    {
        if (num == -1)
        {
            num = 1;
        }
        GameObject[] objArray = GameObject.FindGameObjectsWithTag(place);
        if (objArray.Length > 0)
        {
            for (int i = 0; i < num; i++)
            {
                int index = UnityEngine.Random.Range(0, objArray.Length);
                GameObject obj2 = objArray[index];
                while (objArray[index] == null)
                {
                    index = UnityEngine.Random.Range(0, objArray.Length);
                    obj2 = objArray[index];
                }
                objArray[index] = null;
                SpawnTitan(rate, obj2.transform.position, obj2.transform.rotation, punk);
            }
        }
    }

    public Texture2D RCLoadTexture(string tex)
    {
        if (assetCacheTextures == null)
        {
            assetCacheTextures = new Dictionary<string, Texture2D>();
        }
        if (assetCacheTextures.ContainsKey(tex))
        {
            return assetCacheTextures[tex];
        }
        Texture2D textured2 = (Texture2D) RCassets.Load(tex);
        assetCacheTextures.Add(tex, textured2);
        return textured2;
    }
    
    private void RefreshRacingResult()
    {
        this.localRacingResult = "Result\n";
        racingResult.Sort();
        int num = Mathf.Min(racingResult.Count, 10);
        for (int i = 0; i < num; i++)
        {
            string localRacingResult1 = this.localRacingResult;
            object[] objArray2 = { localRacingResult1, "Rank ", i + 1, " : " };
            this.localRacingResult = string.Concat(objArray2);
            this.localRacingResult = this.localRacingResult + racingResult[i].name;
            this.localRacingResult = this.localRacingResult + "   " + (int) (racingResult[i].time * 100f) * 0.01f + "s";
            this.localRacingResult = this.localRacingResult + "\n";
        }
        photonView.RPC(Rpc.RefreshRacingResult, PhotonTargets.All, localRacingResult);
    }

    public IEnumerator ReloadSkyEnumerator()
    {
        yield return new WaitForSeconds(0.5f);
        if (skyMaterial != null && UnityEngine.Camera.main.GetComponent<Skybox>().material != skyMaterial)
            UnityEngine.Camera.main.GetComponent<Skybox>().material = skyMaterial;
        Screen.lockCursor = !Screen.lockCursor; //tf? Probably feng's try to fix something lul
        Screen.lockCursor = !Screen.lockCursor;
    }

    private void ResetGameSettings()
    {
        settings = GameSettingsManager.ImportRCSettings();
    }

    private void ResetSettings(bool leaving)
    {
        masterRC = false;
        Hashtable propertiesToSet = new Hashtable(6);
        if (leaving)
        {
            currentLevel = string.Empty;
            propertiesToSet.Add(PlayerProperty.CurrentLevel, string.Empty);
            levelCache = new List<string[]>();
            titanSpawns.Clear();
            playerSpawnsC.Clear();
            playerSpawnsM.Clear();
            titanSpawners.Clear();
            intVariables.Clear();
            boolVariables.Clear();
            stringVariables.Clear();
            floatVariables.Clear();
            globalVariables.Clear();
            RCRegions.Clear();
            RCEvents.Clear();
            RCVariableNames.Clear();
            playerVariables.Clear();
            titanVariables.Clear();
            RCRegionTriggers.Clear();
            currentScriptLogic = string.Empty;
            propertiesToSet.Add(PlayerProperty.Acceleration, 100);
            propertiesToSet.Add(PlayerProperty.Blade, 100);
            propertiesToSet.Add(PlayerProperty.Gas, 100);
            propertiesToSet.Add(PlayerProperty.Speed, 100);
            restartingTitan = false;
            restartingMC = false;
            restartingHorse = false;
            restartingEren = false;
            restartingBomb = false;
        }
        propertiesToSet.Add(PlayerProperty.RCTeam, 0);
        Player.Self.SetCustomProperties(propertiesToSet);
        ResetGameSettings();
        banHash = new Hashtable();
        _infectionTitans = new Hashtable();
        oldScript = string.Empty;
        restartCount = new List<float>();
        heroHash = new Hashtable();
    }

//    private IEnumerator RespawnEnumerator(float seconds)
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(seconds);
//            if (!isLosing && !isWinning)
//            {
//                foreach (PhotonPlayer targetPlayer in PhotonNetwork.playerList)
//                {
//                    if (targetPlayer.CustomProperties[PhotonPlayerProperty.RCteam] == null && RCextensions.returnBoolFromObject(targetPlayer.CustomProperties[PhotonPlayerProperty.dead]) && RCextensions.returnIntFromObject(targetPlayer.CustomProperties[PhotonPlayerProperty.isTitan]) != 2)
//                    {
//                        photonView.RPC(Rpc.respawnHeroInNewRound, targetPlayer);
//                    }
//                }
//            }
//        }
//    }

    public IEnumerator RestartEnumerator(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        RestartGame();
    }
  
    public void RestartGame(bool masterclientSwitched = false)
    {
        Mod.Interface.Chat.EditMessage(_endingMessageId, "Restart aborted: Room restarted", false);
        _endingMessageId = null;
        Mod.Interface.Chat.EditMessage(_racingMessageId, "Racing aborted: Room restarted", false);
        _racingMessageId = null;
        Mod.Interface.Chat.EditMessage(_pauseMessageId, "Pause aborted: Room restarted", false);
        _pauseMessageId = null;

        PVPtitanScore = 0;
        PVPhumanScore = 0;
        startRacing = false;
        endRacing = false;
        checkpoint = null;
        timeElapse = 0f;
        roundTime = 0f;
        isWinning = false;
        isLosing = false;
        isPlayer1Winning = false;
        isPlayer2Winning = false;
        wave = 1;
        myRespawnTime = 0f;
        killInfoGO = new ArrayList();
        racingResult = new List<RacingResult>();
        isRestarting = true;
        DestroyAllExistingCloths();
        PhotonNetwork.DestroyAll();
        Hashtable hash = GetGameSettings();
        photonView.RPC(Rpc.Settings, PhotonTargets.Others, hash);
        photonView.RPC(Rpc.LoadLevel, PhotonTargets.All);
        SetGameSettings(hash);
        
        if (masterclientSwitched)
            SendChatContentInfo("<color=#A8FF24>MasterClient has switched to</color> " + Shelter.Profile.HexName);
    }
    
    public void RestartSingleplayer()
    {
        startRacing = false;
        endRacing = false;
        checkpoint = null;
        timeElapse = 0f;
        roundTime = 0f;
        timeTotalServer = 0f;
        isWinning = false;
        isLosing = false;
        isPlayer1Winning = false;
        isPlayer2Winning = false;
        wave = 1;
        myRespawnTime = 0f;
        DestroyAllExistingCloths();
        Application.LoadLevel(Application.loadedLevel);
    }

    public void RestartGameRC()
    {
        intVariables.Clear();
        boolVariables.Clear();
        stringVariables.Clear();
        floatVariables.Clear();
        playerVariables.Clear();
        titanVariables.Clear();
        if (settings.IsInfectionMode)
        {
            GameInfectionEnd();
        }
        else
        {
            GameEndRC();
        }
    }

    public RCActionHelper ReturnHelper(string str)
    {
        int num3;
        string[] strArray = str.Split('.');
        if (float.TryParse(str, out float _))
        {
            strArray = new[] { str };
        }
        List<RCActionHelper> list = new List<RCActionHelper>();
        int sentType = 0;
        for (num3 = 0; num3 < strArray.Length; num3++)
        {
            string str2;
            RCActionHelper helper;
            if (list.Count == 0)
            {
                str2 = strArray[num3];
                if (str2.StartsWith("\"") && str2.EndsWith("\""))
                {
                    helper = new RCActionHelper(0, 0, str2.Substring(1, str2.Length - 2));
                    list.Add(helper);
                    sentType = 2;
                }
                else
                {
                    if (int.TryParse(str2, out var num4))
                    {
                        helper = new RCActionHelper(0, 0, num4);
                        list.Add(helper);
                        sentType = 0;
                    }
                    else
                    {
                        if (float.TryParse(str2, out var num5))
                        {
                            helper = new RCActionHelper(0, 0, num5);
                            list.Add(helper);
                            sentType = 3;
                        }
                        else if (str2.ToLower() == "true" || str2.ToLower() == "false")
                        {
                            helper = new RCActionHelper(0, 0, Convert.ToBoolean(str2.ToLower()));
                            list.Add(helper);
                            sentType = 1;
                        }
                        else
                        {
                            int index;
                            int num7;
                            if (str2.StartsWith("Variable"))
                            {
                                index = str2.IndexOf('(');
                                num7 = str2.LastIndexOf(')');
                                if (str2.StartsWith("VariableInt"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(1, 0, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("VariableBool"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(1, 1, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 1;
                                }
                                else if (str2.StartsWith("VariableString"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(1, 2, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 2;
                                }
                                else if (str2.StartsWith("VariableFloat"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(1, 3, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("VariablePlayer"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(1, 4, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 4;
                                }
                                else if (str2.StartsWith("VariableTitan"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(1, 5, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 5;
                                }
                            }
                            else if (str2.StartsWith("Region"))
                            {
                                index = str2.IndexOf('(');
                                num7 = str2.LastIndexOf(')');
                                if (str2.StartsWith("RegionRandomX"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(4, 0, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("RegionRandomY"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(4, 1, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("RegionRandomZ"))
                                {
                                    str2 = str2.Substring(index + 1, num7 - index - 1);
                                    helper = new RCActionHelper(4, 2, ReturnHelper(str2));
                                    list.Add(helper);
                                    sentType = 3;
                                }
                            }
                        }
                    }
                }
            }
            else if (list.Count > 0)
            {
                str2 = strArray[num3];
                if (list[list.Count - 1].helperClass != 1)
                {
                    if (str2.StartsWith("ConvertToInt()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 0;
                    }
                    else if (str2.StartsWith("ConvertToBool()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 1;
                    }
                    else if (str2.StartsWith("ConvertToString()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 2;
                    }
                    else if (str2.StartsWith("ConvertToFloat()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 3;
                    }
                }
                else
                {
                    switch (list[list.Count - 1].helperType)
                    {
                        case 4:
                        {
                            if (!str2.StartsWith("GetTeam()"))
                            {
                                goto Label_063B;
                            }
                            helper = new RCActionHelper(2, 1, null);
                            list.Add(helper);
                            sentType = 0;
                            continue;
                        }
                        case 5:
                        {
                            if (!str2.StartsWith("GetType()"))
                            {
                                goto Label_095F;
                            }
                            helper = new RCActionHelper(3, 0, null);
                            list.Add(helper);
                            sentType = 0;
                            continue;
                        }
                    }
                    if (str2.StartsWith("ConvertToInt()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 0;
                    }
                    else if (str2.StartsWith("ConvertToBool()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 1;
                    }
                    else if (str2.StartsWith("ConvertToString()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 2;
                    }
                    else if (str2.StartsWith("ConvertToFloat()"))
                    {
                        helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 3;
                    }
                }
            }
            continue;
        Label_063B:
            if (str2.StartsWith("GetType()"))
            {
                helper = new RCActionHelper(2, 0, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetIsAlive()"))
            {
                helper = new RCActionHelper(2, 2, null);
                list.Add(helper);
                sentType = 1;
            }
            else if (str2.StartsWith("GetTitan()"))
            {
                helper = new RCActionHelper(2, 3, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetKills()"))
            {
                helper = new RCActionHelper(2, 4, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetDeaths()"))
            {
                helper = new RCActionHelper(2, 5, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetMaxDmg()"))
            {
                helper = new RCActionHelper(2, 6, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetTotalDmg()"))
            {
                helper = new RCActionHelper(2, 7, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetCustomInt()"))
            {
                helper = new RCActionHelper(2, 8, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetCustomBool()"))
            {
                helper = new RCActionHelper(2, 9, null);
                list.Add(helper);
                sentType = 1;
            }
            else if (str2.StartsWith("GetCustomString()"))
            {
                helper = new RCActionHelper(2, 10, null);
                list.Add(helper);
                sentType = 2;
            }
            else if (str2.StartsWith("GetCustomFloat()"))
            {
                helper = new RCActionHelper(2, 11, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetPositionX()"))
            {
                helper = new RCActionHelper(2, 14, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetPositionY()"))
            {
                helper = new RCActionHelper(2, 15, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetPositionZ()"))
            {
                helper = new RCActionHelper(2, 16, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetName()"))
            {
                helper = new RCActionHelper(2, 12, null);
                list.Add(helper);
                sentType = 2;
            }
            else if (str2.StartsWith("GetGuildName()"))
            {
                helper = new RCActionHelper(2, 13, null);
                list.Add(helper);
                sentType = 2;
            }
            else if (str2.StartsWith("GetSpeed()"))
            {
                helper = new RCActionHelper(2, 17, null);
                list.Add(helper);
                sentType = 3;
            }
            continue;
        Label_095F:
            if (str2.StartsWith("GetSize()"))
            {
                helper = new RCActionHelper(3, 1, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetHealth()"))
            {
                helper = new RCActionHelper(3, 2, null);
                list.Add(helper);
                sentType = 0;
            }
            else if (str2.StartsWith("GetPositionX()"))
            {
                helper = new RCActionHelper(3, 3, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetPositionY()"))
            {
                helper = new RCActionHelper(3, 4, null);
                list.Add(helper);
                sentType = 3;
            }
            else if (str2.StartsWith("GetPositionZ()"))
            {
                helper = new RCActionHelper(3, 5, null);
                list.Add(helper);
                sentType = 3;
            }
        }
        for (num3 = list.Count - 1; num3 > 0; num3--)
        {
            list[num3 - 1].setNextHelper(list[num3]);
        }
        return list[0];
    }

    public static ClientState returnPeerState(int peerstate)
    {
        switch (peerstate)
        {
            case 0:
                return ClientState.Authenticated;

            case 1:
                return ClientState.ConnectedToMaster;

            case 2:
                return ClientState.DisconnectingFromMasterserver;

            case 3:
                return ClientState.DisconnectingFromGameserver;

            case 4:
                return ClientState.DisconnectingFromNameServer;
        }
        return ClientState.ConnectingToMasterserver;
    }

    public void SendChatContentInfo(string content)
    {
        photonView.RPC(Rpc.Chat, PhotonTargets.All, content, string.Empty);
    }

    public void SendKillInfo(bool t1, string killer, bool t2, string victim, int dmg = 0)
    {
        photonView.RPC(Rpc.UpdateKillInfo, PhotonTargets.All, t1, killer, t2, victim, dmg);
    }

    public static void ServerCloseConnection(Player targetPlayer, bool requestIpBan, string inGameName = null)
    {
        RaiseEventOptions options = new RaiseEventOptions {
            TargetActors = new[] { targetPlayer.ID }
        };
        if (requestIpBan) // I have litterally no idea on what this is.
        {
            Hashtable eventContent = new Hashtable
            {
                [(byte)0] = true
            };
            if (!string.IsNullOrEmpty(inGameName))
            {
                eventContent[(byte) 1] = inGameName;
            }
            PhotonNetwork.RaiseEvent(203, eventContent, true, options); // TODO: Search on what 203 is and what does it do with ipban
        }
        else
        {
            PhotonNetwork.RaiseEvent(203, null, true, options);
        }
    }

    public static void ServerRequestAuthentication(string authPassword) //TODO: No idea of what this is -- Probably LAN stuff?
    {
        if (!string.IsNullOrEmpty(authPassword))
        {
            Hashtable eventContent = new Hashtable
            {
                [(byte)0] = authPassword
            };
            PhotonNetwork.RaiseEvent(198, eventContent, true, new RaiseEventOptions());
        }
    }

    public static void ServerRequestUnban(string bannedAddress) //TODO: First time seeing this shit after years
    {
        if (!string.IsNullOrEmpty(bannedAddress))
        {
            Hashtable eventContent = new Hashtable
            {
                [(byte)0] = bannedAddress
            };
            PhotonNetwork.RaiseEvent(199, eventContent, true, new RaiseEventOptions());
        }
    }

    private void SetGameSettings(Hashtable hash)
    {
        restartingEren = false;
        restartingBomb = false;
        restartingHorse = false;
        restartingTitan = false;

        settings = GameSettingsManager.DecodeFromHashtable(hash);
    }

    private void SetTeam(int setting)
    {
        switch (setting)
        {
            case 0:
                Player.Self.SetCustomProperties(new Hashtable
                {
                    { PlayerProperty.RCTeam, 0 },
                    { PlayerProperty.Name, Shelter.Profile.Name }
                });
                break;
            case 1:
                Player.Self.SetCustomProperties(new Hashtable
                {
                    { PlayerProperty.RCTeam, 1 },
                    { PlayerProperty.Name, "[00FFFF][Cyan][-] " + Shelter.Profile.Name }
                });
                break;
            case 2:
                Player.Self.SetCustomProperties(new Hashtable
                {
                    { PlayerProperty.RCTeam, 2 },
                    { PlayerProperty.Name, "[FF00FF][Magenta][-] " + Shelter.Profile.Name }
                });
                break;
            case 3:
                int cyanTeam = 0;
                int magentaTeam = 0;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    switch (player.Properties.RCTeam)
                    {
                        case 1:
                            cyanTeam++;
                            break;
                        case 2:
                            magentaTeam++;
                            break;
                    }
                }
                SetTeam(cyanTeam < magentaTeam ? 1 : 2);
                break;
        }

        if (setting == 0 || setting == 1 || setting == 2)
        {
            foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (obj2.GetPhotonView().isMine)
                {
                    photonView.RPC(Rpc.UpdateHealthLabel, PhotonTargets.All, obj2.GetPhotonView().viewID);
                }
            }
        }
    }
    
    public void SpawnPlayerTitan(string id, string tag1 = "titanRespawn")
    {
        if (logicLoaded && customLevelLoaded)
        {
            GameObject obj3;
            GameObject[] objArray = GameObject.FindGameObjectsWithTag(tag1);
            GameObject obj2 = objArray[UnityEngine.Random.Range(0, objArray.Length)];
            Vector3 position = obj2.transform.position;
            if (Level.StartsWith("Custom") && titanSpawns.Count > 0)
            {
                position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
            }
            myLastHero = id.ToUpper();
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
            {
                obj3 = PhotonNetwork.Instantiate("TITAN_VER3.1", checkpoint.transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20)), checkpoint.transform.rotation, 0);
            }
            else
            {
                obj3 = PhotonNetwork.Instantiate("TITAN_VER3.1", position, obj2.transform.rotation, 0);
            }
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObjectASTITAN(obj3);
            obj3.GetComponent<TITAN>().nonAI = true;
            obj3.GetComponent<TITAN>().speed = 30f;
            obj3.GetComponent<TITAN_CONTROLLER>().enabled = true;
            if (id == "RANDOM" && UnityEngine.Random.Range(0, 100) < 7)
            {
                obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, true);
            }
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
            GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
            GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            Player.Self.SetCustomProperties( new Hashtable
            {
                { "dead", false },
                { PlayerProperty.IsTitan, 2 }
            });
            
            if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
                Screen.lockCursor = true;
            else
                Screen.lockCursor = false;
            Screen.showCursor = true;
        }
        else
        {
            SpawnPlayerTitanAfterGameEndRC(id);
        }
    }

    public void SpawnPlayer(string id)
    {
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
        {
            SpawnPlayerAt(id, checkpoint);
        }
        else
        {
            GameObject[] objArray = GameObject.FindGameObjectsWithTag(PlayerRespawnTag);
            GameObject pos = objArray[UnityEngine.Random.Range(0, objArray.Length)];
            SpawnPlayerAt(id, pos);
        }
    }

    public void SpawnPlayerAt(string id, GameObject pos)
    {
        if (!logicLoaded || !customLevelLoaded)
        {
            SpawnPlayerAfterGameEndRC(id);
        }
        else
        {
            Vector3 position = pos.transform.position;
            if (racingSpawnPointSet)
            {
                position = racingSpawnPoint;
            }
            else if (Level.StartsWith("Custom"))
            {
                if (Player.Self.Properties.RCTeam == 0)
                {
                    List<Vector3> list = new List<Vector3>();
                    foreach (Vector3 vector2 in playerSpawnsC)
                    {
                        list.Add(vector2);
                    }
                    foreach (Vector3 vector2 in playerSpawnsM)
                    {
                        list.Add(vector2);
                    }
                    if (list.Count > 0)
                    {
                        position = list[UnityEngine.Random.Range(0, list.Count)];
                    }
                }
                else if (Player.Self.Properties.RCTeam == 1)
                {
                    if (playerSpawnsC.Count > 0)
                    {
                        position = playerSpawnsC[UnityEngine.Random.Range(0, playerSpawnsC.Count)];
                    }
                }
                else if (Player.Self.Properties.RCTeam == 2 && playerSpawnsM.Count > 0)
                {
                    position = playerSpawnsM[UnityEngine.Random.Range(0, playerSpawnsM.Count)];
                }
            }
            IN_GAME_MAIN_CAMERA component = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>();
            myLastHero = id.ToUpper();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                if (IN_GAME_MAIN_CAMERA.singleCharacter == "TITAN_EREN")
                {
                    component.setMainObject((GameObject)Instantiate(Resources.Load("TITAN_EREN"), pos.transform.position, pos.transform.rotation));
                }
                else
                {
                    component.setMainObject((GameObject)Instantiate(Resources.Load("AOTTG_HERO 1"), pos.transform.position, pos.transform.rotation));
                    if (IN_GAME_MAIN_CAMERA.singleCharacter == "SET 1" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 2" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 3")
                    {
                        HeroCostume costume = CostumeConverter.LocalDataToHeroCostume(IN_GAME_MAIN_CAMERA.singleCharacter);
                        costume?.ValidateHeroStats();
                        CostumeConverter.HeroCostumeToLocalData(costume, IN_GAME_MAIN_CAMERA.singleCharacter);
                        if (costume != null)
                        {
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume;
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = costume.stat;
                        }
                        else
                        {
                            costume = HeroCostume.costumeOption[3];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume;
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(costume.name);
                        }
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                        component.main_object.GetComponent<HERO>().setStat2();
                        component.main_object.GetComponent<HERO>().SetSkillHUDPosition();
                    }
                    else
                    {
                        foreach (HeroCostume hero in HeroCostume.costume)
                        {
                            if (hero.name.EqualsIgnoreCase(IN_GAME_MAIN_CAMERA.singleCharacter))
                            {
                                int index = hero.id + CheckBoxCostume.costumeSet - 1;
                                if (HeroCostume.costume[index].name != hero.name)
                                {
                                    index = hero.id + 1;
                                }
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[index];
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(HeroCostume.costume[index].name);
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                                component.main_object.GetComponent<HERO>().setStat2();
                                component.main_object.GetComponent<HERO>().SetSkillHUDPosition();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                component.setMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", position, pos.transform.rotation, 0));
                id = id.ToUpper();
                if (id == "SET 1" || id == "SET 2" || id == "SET 3") //TODO: Rename/Remove and use Profilesystem
                {
                    HeroCostume costume2 = CostumeConverter.LocalDataToHeroCostume(id);
                    costume2?.ValidateHeroStats();
                    CostumeConverter.HeroCostumeToLocalData(costume2, id);
                    if (costume2 != null)
                    {
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume2;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = costume2.stat;
                    }
                    else
                    {
                        costume2 = HeroCostume.costumeOption[3];
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume2;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(costume2.name);
                    }
                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                    component.main_object.GetComponent<HERO>().setStat2();
                    component.main_object.GetComponent<HERO>().SetSkillHUDPosition();
                }
                else
                {
                    foreach (HeroCostume hero in HeroCostume.costume)
                    {
                        if (hero.name.EqualsIgnoreCase(id))
                        {
                            int num4 = hero.id;
                            if (id.ToUpper() != "AHSS")
                            {
                                num4 += CheckBoxCostume.costumeSet - 1;
                            }
                            if (HeroCostume.costume[num4].name != hero.name)
                            {
                                num4 = hero.id + 1;
                            }
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[num4];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(HeroCostume.costume[num4].name);
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                            component.main_object.GetComponent<HERO>().setStat2();
                            component.main_object.GetComponent<HERO>().SetSkillHUDPosition();
                            break;
                        }
                    }
                }
                CostumeConverter.HeroCostumeToPhotonData(component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, Player.Self);
                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
                {
                    Transform transform1 = component.main_object.transform;
                    transform1.position += new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20));
                }

                Player.Self.SetCustomProperties(new Hashtable
                {
                    {"dead", false},
                    {PlayerProperty.IsTitan, 1}
                });
            }
            component.enabled = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
            GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
            component.gameOver = false;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            Screen.showCursor = false;
            isLosing = false;
        }
    }

    private void SpawnPlayerCustomMap()
    {
        if (!needChooseSide && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            if (Player.Self.Properties.PlayerType == PlayerType.Titan)
            {
                SpawnPlayerTitan(myLastHero);
            }
            else
            {
                SpawnPlayer(myLastHero);
            }
        }
    }

    public GameObject SpawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
    {
        GameObject obj3;
        GameObject obj2 = SpawnTitanRaw(position, rotation);
        if (punk)
        {
            obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Punk, false);
        }
        else if (UnityEngine.Random.Range(0, 100) < rate)
        {
            if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                if (UnityEngine.Random.Range(0f, 1f) >= 0.7f && !LevelInfoManager.Get(Level).NoCrawler)
                {
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, false);
                }
                else
                {
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
        {
            if (UnityEngine.Random.Range(0f, 1f) >= 0.7f && !LevelInfoManager.Get(Level).NoCrawler)
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, false);
            }
            else
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
            }
        }
        else if (UnityEngine.Random.Range(0, 100) < rate)
        {
            if (UnityEngine.Random.Range(0f, 1f) >= 0.8f && !LevelInfoManager.Get(Level).NoCrawler)
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, false);
            }
            else
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Abnormal, false);
            }
        }
        else if (UnityEngine.Random.Range(0f, 1f) >= 0.8f && !LevelInfoManager.Get(Level).NoCrawler)
        {
            obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, false);
        }
        else
        {
            obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            obj3 = (GameObject)Instantiate(Resources.Load("FX/FXtitanSpawn"), obj2.transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        else
        {
            obj3 = PhotonNetwork.Instantiate("FX/FXtitanSpawn", obj2.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
        }
        obj3.transform.localScale = obj2.transform.localScale;
        return obj2;
    }

    public void SpawnTitanAction(int type, float size, int health, int number)
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-400f, 400f), 0f, UnityEngine.Random.Range(-400f, 400f));
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
        if (titanSpawns.Count > 0)
        {
            position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
        }
        else
        {
            GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
            if (objArray.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, objArray.Length);
                GameObject obj2 = objArray[index];
                while (objArray[index] == null)
                {
                    index = UnityEngine.Random.Range(0, objArray.Length);
                    obj2 = objArray[index];
                }
                objArray[index] = null;
                position = obj2.transform.position;
                rotation = obj2.transform.rotation;
            }
        }
        for (int i = 0; i < number; i++)
        {
            GameObject obj3 = SpawnTitanRaw(position, rotation);
            obj3.GetComponent<TITAN>().resetLevel(size);
            obj3.GetComponent<TITAN>().hasSetLevel = true;
            if (health > 0f)
            {
                obj3.GetComponent<TITAN>().currentHealth = health;
                obj3.GetComponent<TITAN>().maxHealth = health;
            }
            switch (type)
            {
                case 0:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Normal, false);
                    break;

                case 1:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Abnormal, false);
                    break;

                case 2:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                    break;

                case 3:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, true);
                    break;

                case 4:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Punk, false);
                    break;
            }
        }
    }

    public void SpawnTitanAtAction(int type, float size, int health, int number, float posX, float posY, float posZ)
    {
        Vector3 position = new Vector3(posX, posY, posZ);
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
        for (int i = 0; i < number; i++)
        {
            GameObject obj2 = SpawnTitanRaw(position, rotation);
            obj2.GetComponent<TITAN>().resetLevel(size);
            obj2.GetComponent<TITAN>().hasSetLevel = true;
            if (health > 0f)
            {
                obj2.GetComponent<TITAN>().currentHealth = health;
                obj2.GetComponent<TITAN>().maxHealth = health;
            }
            switch (type)
            {
                case 0:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Normal, false);
                    break;

                case 1:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Abnormal, false);
                    break;

                case 2:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                    break;

                case 3:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, true);
                    break;

                case 4:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Punk, false);
                    break;
            }
        }
    }

    public void SpawnTitanCustom(string type, int abnormal, int rate, bool punk)
    {
        int num8;
        Vector3 position;
        Quaternion rotation;
        GameObject[] objArray;
        int num9;
        GameObject obj2;
        int moreTitans = rate;
        if (Level.StartsWith("Custom"))
        {
            moreTitans = 5;
            if (settings.GameType == 1)
            {
                moreTitans = 3;
            }
            else if (settings.GameType == 2 || settings.GameType == 3)
            {
                moreTitans = 0;
            }
        }
        if (settings.MoreTitansNumber > 0 || settings.MoreTitansNumber == 0 && Level.StartsWith("Custom") && settings.GameType >= 2)
        {
            moreTitans = settings.MoreTitansNumber;
        }
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.SurviveMode)
        {
            if (punk)
            {
                moreTitans = rate;
            }
            else
            {
                int waveModeNum;
                if (settings.MoreTitansNumber == 0)
                {
                    waveModeNum = 1;
                    if (settings.IsMaxWaveMode)
                    {
                        waveModeNum = settings.MaxWaveNumber;
                    }
                    moreTitans += (wave - 1) * (waveModeNum - 1);
                }
                else if (settings.MoreTitansNumber > 0)
                {
                    waveModeNum = 1;
                    if (settings.IsWaveMode)
                    {
                        waveModeNum = settings.WaveNumber;
                    }
                    moreTitans += (wave - 1) * waveModeNum;
                }
            }
        }
        moreTitans = Math.Min(50, moreTitans);
        if (settings.UseCustomSpawnRates)
        {
            if (punk && settings.AllowPunks)
            {
                moreTitans = rate;
            }
            for (num8 = 0; num8 < moreTitans; num8++)
            {
                position = new Vector3(UnityEngine.Random.Range(-400f, 400f), 0f, UnityEngine.Random.Range(-400f, 400f));
                rotation = new Quaternion(0f, 0f, 0f, 1f);
                if (titanSpawns.Count > 0)
                {
                    position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
                }
                else
                {
                    objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
                    if (objArray.Length > 0)
                    {
                        num9 = UnityEngine.Random.Range(0, objArray.Length);
                        obj2 = objArray[num9];
                        while (objArray[num9] == null)
                        {
                            num9 = UnityEngine.Random.Range(0, objArray.Length);
                            obj2 = objArray[num9];
                        }
                        objArray[num9] = null;
                        position = obj2.transform.position;
                        rotation = obj2.transform.rotation;
                    }
                }

                var rates = settings.SpawnRates;
                float num10 = UnityEngine.Random.Range(0f, 100f);
                if (num10 <= rates.Normal + rates.Aberrant + rates.Jumper + rates.Crawler + rates.Punk)
                {
                    GameObject obj3 = SpawnTitanRaw(position, rotation);
                    if (num10 < rates.Normal)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Normal, false);
                    }
                    else if (num10 >= rates.Normal && num10 < rates.Normal + rates.Aberrant)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Abnormal, false);
                    }
                    else if (num10 >= rates.Normal + rates.Aberrant && num10 < rates.Normal + rates.Aberrant + rates.Jumper)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                    }
                    else if (num10 >= rates.Normal + rates.Aberrant + rates.Jumper && num10 < rates.Normal + rates.Aberrant + rates.Jumper + rates.Crawler)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, true);
                    }
                    else if (num10 >= rates.Normal + rates.Aberrant + rates.Jumper + rates.Crawler && num10 < rates.Normal + rates.Aberrant + rates.Jumper + rates.Crawler + rates.Punk)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Punk, false);
                    }
                    else
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Normal, false);
                    }
                }
                else
                {
                    SpawnTitan(abnormal, position, rotation, punk);
                }
            }
        }
        else if (Level.StartsWith("Custom"))
        {
            for (num8 = 0; num8 < moreTitans; num8++)
            {
                position = new Vector3(UnityEngine.Random.Range(-400f, 400f), 0f, UnityEngine.Random.Range(-400f, 400f));
                rotation = new Quaternion(0f, 0f, 0f, 1f);
                if (titanSpawns.Count > 0)
                {
                    position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
                }
                else
                {
                    objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
                    if (objArray.Length > 0)
                    {
                        num9 = UnityEngine.Random.Range(0, objArray.Length);
                        obj2 = objArray[num9];
                        while (objArray[num9] == null)
                        {
                            num9 = UnityEngine.Random.Range(0, objArray.Length);
                            obj2 = objArray[num9];
                        }
                        objArray[num9] = null;
                        position = obj2.transform.position;
                        rotation = obj2.transform.rotation;
                    }
                }
                SpawnTitan(abnormal, position, rotation, punk);
            }
        }
        else
        {
            RandomSpawnTitan("titanRespawn", abnormal, moreTitans, punk);
        }
    }

    private GameObject SpawnTitanRaw(Vector3 position, Quaternion rotation)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            return (GameObject)Instantiate(Resources.Load("TITAN_VER3.1"), position, rotation);
        }
        return PhotonNetwork.Instantiate("TITAN_VER3.1", position, rotation, 0);
    }

    private void DestroyOldMenu()
    {
        var whitelist = new[]
        {
            "Shelter",
            "Interface",
            "Modules",
            //"MainCamera_Mono",
            "PhotonMono",
            //"EventSystem",
            "UIRefer",
            "MultiplayerManager",
            //"InputManagerController", Custom input manager
            //"UI Root",
            //"Camera",
            //"MenuBackGround", // Without this you can't join rooms. || EDIT: Seems like now you can.
        };

        foreach (var obj2 in FindObjectsOfType(typeof(GameObject)))
        {
            if (whitelist.All(x => !x.ContainsIgnoreCase(obj2.name)))
                Destroy(obj2);
        }
    }

    public void UnloadAssets()
    {
        if (!isUnloading)
        {
            isUnloading = true;
            StartCoroutine(UnloadAssetsEnumerator(10f));
        }
    }

    public IEnumerator UnloadAssetsEnumerator(float time1)
    {
        yield return new WaitForSeconds(time1);
        Resources.UnloadUnusedAssets();
        isUnloading = false;
    }

    public void UnloadAssetsEditor()
    {
        if (!isUnloading)
        {
            isUnloading = true;
            StartCoroutine(UnloadAssetsEnumerator(30f));
        }
    }
    
    public IEnumerator WaitAndReloadKDR(Player player)
    {
        yield return new WaitForSeconds(5f);
        string key = player.Properties.Name;
        if (PreservedPlayerKDR.ContainsKey(key))
        {
            int[] numArray = PreservedPlayerKDR[key];
            PreservedPlayerKDR.Remove(key);
            player.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Kills, numArray[0] },
                { PlayerProperty.Deaths, numArray[1] },
                { PlayerProperty.MaxDamage, numArray[2] },
                { PlayerProperty.TotalDamage, numArray[3] }
            });
        }
    }

    public IEnumerator WaitAndResetRestarts()
    {
        yield return new WaitForSeconds(10f);
        restartingBomb = false;
        restartingEren = false;
        restartingHorse = false;
        restartingMC = false;
        restartingTitan = false;
    }

    public IEnumerator WaitAndRespawn1(float time1)
    {
        yield return new WaitForSeconds(time1);
        SpawnPlayer(myLastHero);
    }

    public IEnumerator WaitAndRespawn2(float waitTime, GameObject pos)
    {
        yield return new WaitForSeconds(waitTime);
        SpawnPlayerAt(myLastHero, pos);
    }
}