using Mod;
using Mod.Exceptions;
using Mod.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class FengGameManagerMKII : Photon.MonoBehaviour
{
    public const string ApplicationID = "f1f6195c-df4a-40f9-bae5-4744c32901ef";
    private const string PlayerRespawnTag = "playerRespawn";
    public Dictionary<int, CannonValues> allowedToCannon;
    public Dictionary<string, Texture2D> assetCacheTextures;
    public static ExitGames.Client.Photon.Hashtable banHash;
    public static ExitGames.Client.Photon.Hashtable boolVariables;
    public static Dictionary<string, GameObject> CachedPrefabs;
    private ArrayList chatContent;
    public InRoomChat chatRoom;
    public GameObject checkpoint;
    private ArrayList cT;
    public static string currentLevel;
    public static string currentScript;
    public static string currentScriptLogic;
    private float currentSpeed;
    public static bool customLevelLoaded;
    public int cyanKills;
    public int difficulty;
    public float distanceSlider;
    private bool endRacing;
    private ArrayList eT;
    public static ExitGames.Client.Photon.Hashtable floatVariables;
    private ArrayList fT;
    private float gameEndCD;
    private float gameEndTotalCDtime = 9f;
    public bool gameStart;
    private bool gameTimesUp;
    public static ExitGames.Client.Photon.Hashtable globalVariables;
    public List<GameObject> groundList;
    public static bool hasLogged;
    private ArrayList heroes;
    public static ExitGames.Client.Photon.Hashtable heroHash;
    private int highestwave = 1;
    private ArrayList hooks;
    private int humanScore;
    public static List<int> ignoreList;
    public static ExitGames.Client.Photon.Hashtable imatitan;
    public FengCustomInputs inputManager;
    public static InputManagerRC inputRC;
    public static FengGameManagerMKII instance;
    public static ExitGames.Client.Photon.Hashtable intVariables;
    public static bool isAssetLoaded;
    public bool isFirstLoad;
    private bool isLosing;
    public bool isPlayer1Winning;
    public bool isPlayer2Winning;
    public bool isRecompiling;
    public bool isRestarting;
    public bool isSpawning;
    public bool isUnloading;
    private bool isWinning;
    public bool justSuicide;
//    private ArrayList kicklist;
    private ArrayList killInfoGO = new ArrayList();
    public static bool LAN;
    public static string Level = string.Empty;
    public List<string[]> levelCache;
    public static ExitGames.Client.Photon.Hashtable[] linkHash;
    private string localRacingResult;
    public static bool logicLoaded;
    public static int loginstate;
    public int magentaKills;
    private IN_GAME_MAIN_CAMERA mainCamera;
    public static bool masterRC;
    public int maxPlayers;
    private float maxSpeed;
    public float mouseSlider;
    private string myLastHero;
    public float myRespawnTime;
    public static string nameField;
    public bool needChooseSide;
    public static bool noRestart;
    public static string oldScript;
    public static string oldScriptLogic;
    public static bool OnPrivateServer;
    public static string passwordField;
    public float pauseWaitTime;
    public string playerList;
    public List<Vector3> playerSpawnsC;
    public List<Vector3> playerSpawnsM;
    public List<Player> playersRPC;
    public static ExitGames.Client.Photon.Hashtable playerVariables;
    public Dictionary<string, int[]> PreservedPlayerKDR;
    public static string PrivateServerAuthPass;
    public static string privateServerField;
    public int PVPhumanScore;
    private int PVPhumanScoreMax = 200;
    public int PVPtitanScore;
    private int PVPtitanScoreMax = 200;
    public float qualitySlider;
    public List<GameObject> racingDoors;
    private ArrayList racingResult;
    public Vector3 racingSpawnPoint;
    public bool racingSpawnPointSet;
    public static AssetBundle RCassets;
    public static ExitGames.Client.Photon.Hashtable RCEvents;
    public static ExitGames.Client.Photon.Hashtable RCRegions;
    public static ExitGames.Client.Photon.Hashtable RCRegionTriggers;
    public static ExitGames.Client.Photon.Hashtable RCVariableNames;
    public List<float> restartCount;
    public bool restartingBomb;
    public bool restartingEren;
    public bool restartingHorse;
    public bool restartingMC;
    public bool restartingTitan;
    public float retryTime;
    public float roundTime;
    public Vector2 scroll;
    public Vector2 scroll2;
    public GameObject selectedObj;
    public static object[] settings; //TODO: Replace with a class which contains named settings
    private int single_kills;
    private int single_maxDamage;
    private int single_totalDamage;
    public static Material skyMaterial;
    public List<GameObject> spectateSprites;
    private bool startRacing;
    public static ExitGames.Client.Photon.Hashtable stringVariables;
    private int[] teamScores;
    private int teamWinner;
    public Texture2D textureBackgroundBlack;
    public Texture2D textureBackgroundBlue;
    public int time = 600;
    private float timeElapse;
    private float timeTotalServer;
    private ArrayList titans;
    private int titanScore;
    public List<TitanSpawner> titanSpawners;
    public List<Vector3> titanSpawns;
    public static ExitGames.Client.Photon.Hashtable titanVariables;
    public float transparencySlider;
    private GameObject ui;
    public float updateTime;
    public static string usernameField;
    public int wave = 1;

    public new string name
    {
        get => Shelter.Profile.Name;
        set {  }
    } //TODO: Remove (non mi ricordo per cosa viene usato)

    // ReSharper disable UnusedMember.Local

    #region RPCs

    [RPC]
    private void RequireStatus()
    {
        object[] parameters = { humanScore, titanScore, wave, highestwave, roundTime, timeTotalServer, startRacing, endRacing };
        photonView.RPC("refreshStatus", PhotonTargets.Others, parameters);
        object[] objArray2 = { PVPhumanScore, PVPtitanScore };
        photonView.RPC("refreshPVPStatus", PhotonTargets.Others, objArray2);
        object[] objArray3 = { teamScores };
        photonView.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, objArray3);
    }

    [RPC]
    private void RefreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin)
    {
        humanScore = score1;
        titanScore = score2;
        wave = wav;
        highestwave = highestWav;
        roundTime = time1;
        timeTotalServer = time2;
        startRacing = startRacin;
        endRacing = endRacin;
        if (startRacing && GameObject.Find("door") != null)
        {
            GameObject.Find("door").SetActive(false);
        }
    }

    [RPC]
    private void RefreshPVPStatus(int score1, int score2)
    {
        PVPhumanScore = score1;
        PVPtitanScore = score2;
    }

    [RPC]
    private void RefreshPVPStatus_AHSS(int[] score1)
    {
        print(score1);
        teamScores = score1;
    }

    [RPC]
    public void PauseRPC(bool pause, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            if (pause)
            {
                pauseWaitTime = 100000f;
                Time.timeScale = 1E-06f;
            }
            else
            {
                pauseWaitTime = 3f;
            }
        }
    }

    [RPC]
    private void LabelRPC(int viewId, PhotonMessageInfo info)
    {
        if (PhotonView.TryParse(viewId, out PhotonView view))
        {
            if (info.sender != view.owner)
                throw new NotAllowedException(nameof(LabelRPC), info); 
            
            HERO hero = PhotonView.Find(viewId).gameObject.GetComponent<HERO>();
            if (hero != null)
            {
                if (info.sender.Properties.Guild != string.Empty)
                    hero.myNetWorkName.GetComponent<UILabel>().text = "[FFFF00]" + info.sender.Properties.Guild + "\n[FFFFFF]" + info.sender.Properties.Name;
                else
                    hero.myNetWorkName.GetComponent<UILabel>().text = info.sender.Properties.Name;
            }
        }
    }

    [RPC]
    public void OneTitanDown(string name1, bool onPlayerLeave)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || PhotonNetwork.isMasterClient)
        {
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                if (name1 != string.Empty)
                {
                    if (name1 == "Titan")
                    {
                        PVPhumanScore++;
                    }
                    else if (name1 == "Aberrant")
                    {
                        PVPhumanScore += 2;
                    }
                    else if (name1 == "Jumper")
                    {
                        PVPhumanScore += 3;
                    }
                    else if (name1 == "Crawler")
                    {
                        PVPhumanScore += 4;
                    }
                    else if (name1 == "Female Titan")
                    {
                        PVPhumanScore += 10;
                    }
                    else
                    {
                        PVPhumanScore += 3;
                    }
                }
                CheckPVPPoints();
                object[] parameters = { PVPhumanScore, PVPtitanScore };
                photonView.RPC("refreshPVPStatus", PhotonTargets.Others, parameters);
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.CAGE_FIGHT)
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
                {
                    if (IsAnyTitanAlive())
                    {
                        GameWin();
                        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    if (IsAnyTitanAlive())
                    {
                        wave++;
                        if (!(LevelInfoManager.GetInfo(Level).RespawnMode != RespawnMode.NEWROUND && (!Level.StartsWith("Custom") || RCSettings.gameType != 1) || IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER))
                        {
                            foreach (Player player in PhotonNetwork.playerList)
                            {
                                if (player.Properties.PlayerType != PlayerType.Titan)
                                {
                                    photonView.RPC("respawnHeroInNewRound", player);
                                }
                            }
                        }
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                        {
                            SendChatContentInfo("<color=#A8FF24>Wave : " + wave + "</color>");
                        }
                        if (wave > highestwave)
                        {
                            highestwave = wave;
                        }
                        if (PhotonNetwork.isMasterClient)
                        {
                            RequireStatus();
                        }
                        if (!((RCSettings.maxWave != 0 || wave <= 20) && (RCSettings.maxWave <= 0 || wave <= RCSettings.maxWave)))
                        {
                            GameWin();
                        }
                        else
                        {
                            int abnormal = 90;
                            if (difficulty == 1)
                            {
                                abnormal = 70;
                            }
                            if (!LevelInfoManager.GetInfo(Level).HasPunk)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, wave + 2, false);
                            }
                            else if (wave == 5)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 1, true);
                            }
                            else if (wave == 10)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 2, true);
                            }
                            else if (wave == 15)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 3, true);
                            }
                            else if (wave == 20)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 4, true);
                            }
                            else
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, wave + 2, false);
                            }
                        }
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
                {
                    if (!onPlayerLeave)
                    {
                        humanScore++;
                        int num2 = 90;
                        if (difficulty == 1)
                        {
                            num2 = 70;
                        }
                        SpawnTitanCustom("titanRespawn", num2, 1, false);
                    }
                }
                else if (LevelInfoManager.GetInfo(Level).EnemyNumber != -1)
                {
                }
            }
        }
    }

    [RPC]
    private void SetMasterRC(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            masterRC = true;
        }
    }

    [RPC]
    public void RespawnHeroInNewRound()
    {
        if (!needChooseSide && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            SpawnPlayer(myLastHero);
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    private void NetGameLose(int score, PhotonMessageInfo info)
    {
        isLosing = true;
        titanScore = score;
        gameEndCD = gameEndTotalCDtime;
        if ((int)settings[244] == 1)
        {
            Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round ended (game lose).");
        }
        if (!(info.sender == PhotonNetwork.masterClient || info.sender.isLocal) && PhotonNetwork.isMasterClient)
        {
            Mod.Interface.Chat.System("Round end sent from Player " + info.sender.ID);
        }
    }

    [RPC]
    private void RestartGameByClient(PhotonMessageInfo info)
    {
        throw new NotAllowedException(nameof(RestartGameByClient), info);
    }

    [RPC]
    private void NetGameWin(int score, PhotonMessageInfo info)
    {
        humanScore = score;
        isWinning = true;
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
        {
            teamWinner = score;
            teamScores[teamWinner - 1]++;
            gameEndCD = gameEndTotalCDtime;
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
        {
            if (RCSettings.racingStatic == 1)
            {
                gameEndCD = 1000f;
            }
            else
            {
                gameEndCD = 20f;
            }
        }
        else
        {
            gameEndCD = gameEndTotalCDtime;
        }
        if ((int)settings[244] == 1)
        {
            Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round ended (game win).");
        }
        if (!(Equals(info.sender, PhotonNetwork.masterClient) || info.sender.isLocal))
        {
            Mod.Interface.Chat.System("Round end sent from Player " + info.sender.ID);
        }
    }

    [RPC]
    public void SomeOneIsDead(int id = -1)
    {
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            if (id != 0)
            {
                PVPtitanScore += 2;
            }
            CheckPVPPoints();
            object[] parameters = { PVPhumanScore, PVPtitanScore };
            photonView.RPC("refreshPVPStatus", PhotonTargets.Others, parameters);
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
        {
            titanScore++;
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.TROST)
        {
            if (IsAnyPlayerAlive())
            {
                GameLose();
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS && RCSettings.pvpMode == 0 && RCSettings.bombMode == 0)
        {
            if (IsAnyPlayerAlive())
            {
                GameLose();
                teamWinner = 0;
            }
            if (IsAnyTeamMemberAlive(1))
            {
                teamWinner = 2;
                GameWin();
            }
            if (IsAnyTeamMemberAlive(2))
            {
                teamWinner = 1;
                GameWin();
            }
        }
    }

    [RPC]
    private void NetRefreshRacingResult(string tmp)
    {
        localRacingResult = tmp;
    }

    [RPC]
    public void NetShowDamage(int speed)
    {
        GameObject t;
        if ((t = GameObject.Find("Stylish")) != null)
            t.GetComponent<StylishComponent>().Style(speed);
        GameObject target = GameObject.Find("LabelScore");
        Debug.Log("Target null?:" + target == null);
        if (target != null)
        {
            target.GetComponent<UILabel>().text = speed.ToString();
            target.transform.localScale = Vector3.zero;
            speed = (int) (speed * 0.1f);
            speed = Mathf.Max(40, speed);
            speed = Mathf.Min(150, speed);
            iTween.Stop(target);
            object[] args = {"x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f};
            iTween.ScaleTo(target, iTween.Hash(args));
            object[] objArray2 = {"x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f};
            iTween.ScaleTo(target, iTween.Hash(objArray2));
        }
    }

    [RPC]
    private void LoadskinRPC(string n, string url, string url2, string[] skybox, PhotonMessageInfo info)
    {
        if ((int)settings[2] == 1 && info.sender.IsMasterClient)
        {
            StartCoroutine(LoadSkinEnumerator(n, url, url2, skybox));
        }
    }

    [RPC]
    private void RPCLoadLevel(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            DestroyAllExistingCloths();
            PhotonNetwork.LoadLevel(LevelInfoManager.GetInfo(Level).LevelName);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            KickPlayerRC(info.sender, true, "false restart.");
        }
        else if (!masterRC)
        {
            restartCount.Add(Time.time);
            foreach (float num in restartCount)
            {
                if (Time.time - num > 60f)
                {
                    restartCount.Remove(num);
                }
            }
            if (restartCount.Count < 6)
            {
                DestroyAllExistingCloths();
                PhotonNetwork.LoadLevel(LevelInfoManager.GetInfo(Level).LevelName);
            }
        }
    }

    [RPC]
    private void GetRacingResult(string player, float time1)
    {
        RacingResult result = new RacingResult
        {
            name = player,
            time = time1
        };
        racingResult.Add(result);
        RefreshRacingResult();
    }

    [RPC]
    private void IgnorePlayer(int id, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(IgnorePlayer), info);

        if (Player.TryParse(id, out Player player) && !ignoreList.Contains(id))
        {
            ignoreList.Add(player.ID);
            PhotonNetwork.RaiseEvent(254, null, true, new RaiseEventOptions
            {
                TargetActors = new[] { id }
            });
        }
    }

    [RPC]
    private void IgnorePlayerArray(IEnumerable<int> ids, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(IgnorePlayerArray), info);

        foreach (int id in ids)
        {
            if (!Player.TryParse(id, out Player _))
                continue;
            if (!ignoreList.Contains(id))
            {
                ignoreList.Add(id);
                PhotonNetwork.RaiseEvent(254, null, true, new RaiseEventOptions
                {
                    TargetActors = new[] { id }
                });
            }
        }
    }

    [RPC]
    private void CustomlevelRPC(string[] content, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            if (content.Length == 1 && content[0] == "loadcached")
            {
                StartCoroutine(CustomLevelCache());
            }
            else if (content.Length == 1 && content[0] == "loadempty")
            {
                currentLevel = string.Empty;
                levelCache.Clear();
                titanSpawns.Clear();
                playerSpawnsC.Clear();
                playerSpawnsM.Clear();
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.CurrentLevel, currentLevel }
                };
                Player.Self.SetCustomProperties(propertiesToSet);
                customLevelLoaded = true;
                SpawnPlayerCustomMap();
            }
            else
            {
                CustomLevelClient(content, true);
            }
        }
    }

    [RPC]
    private void Clearlevel(string[] link, int gametype, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            if (gametype == 0)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.KILL_TITAN;
            }
            else if (gametype == 1)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.SURVIVE_MODE;
            }
            else if (gametype == 2)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.PVP_AHSS;
            }
            else if (gametype == 3)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.RACING;
            }
            else if (gametype == 4)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.None;
            }
            if (info.sender.IsMasterClient && link.Length > 6)
            {
                StartCoroutine(ClearLevelEnumerator(link));
            }
        }
    }

    [RPC]
    private void ShowResult(string text0, string text1, string text2, string text3, string text4, string text6, PhotonMessageInfo t)
    {
        if (!(gameTimesUp || !t.sender.IsMasterClient))
        {
            gameTimesUp = true;
            GameObject obj2 = GameObject.Find("UI_IN_GAME");
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[0], false);
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[2], true);
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[3], false);
            GameObject.Find("LabelName").GetComponent<UILabel>().text = text0;
            GameObject.Find("LabelKill").GetComponent<UILabel>().text = text1;
            GameObject.Find("LabelDead").GetComponent<UILabel>().text = text2;
            GameObject.Find("LabelMaxDmg").GetComponent<UILabel>().text = text3;
            GameObject.Find("LabelTotalDmg").GetComponent<UILabel>().text = text4;
            GameObject.Find("LabelResultTitle").GetComponent<UILabel>().text = text6;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
            gameStart = false;
        }
        else if (!(t.sender.IsMasterClient || !Player.Self.IsMasterClient))
        {
            KickPlayerRC(t.sender, true, "false game end.");
        }
    }

    [RPC]
    private void SpawnTitanRPC(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            foreach (TITAN titan in titans)
            {
                if (titan.photonView.isMine && !(PhotonNetwork.isMasterClient && !titan.nonAI))
                {
                    PhotonNetwork.Destroy(titan.gameObject);
                }
            }
            SpawnPlayerTitan(myLastHero);
        }
    }

    [RPC]
    private void Chat(string content, string sender, PhotonMessageInfo info)
    {
        if (sender == string.Empty)
            Mod.Interface.Chat.ReceiveMessage(info.sender, content);
        else
            Mod.Interface.Chat.ReceiveMessageFromPlayer(info.sender, content);
    }

    [RPC]
    private void SetTeamRPC(int setting, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient || info.sender.isLocal)
        {
            SetTeam(setting);
        }
    }

    [RPC]
    private void SettingRPC(ExitGames.Client.Photon.Hashtable hash, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            SetGameSettings(hash);
        }
    }

    [RPC]
    private void ShowChatContent(string content) // I don't know what this is
    {
        chatContent.Add(content);
        if (chatContent.Count > 10)
        {
            chatContent.RemoveAt(0);
        }
        GameObject.Find("LabelChatContent").GetComponent<UILabel>().text = string.Empty;
        foreach (object chat in chatContent)
        {
            UILabel component = GameObject.Find("LabelChatContent").GetComponent<UILabel>();
            component.text = component.text + chat;
        }
    }

    [RPC]
    private void UpdateKillInfo(bool t1, string killer, bool t2, string victim, int dmg)
    {
        GameObject obj2 = GameObject.Find("UI_IN_GAME");
        GameObject obj3 = (GameObject)Instantiate(Resources.Load("UI/KillInfo"));
        foreach (GameObject t in killInfoGO)
        {
            if (t != null)
                t.GetComponent<KillInfoComponent>().MoveOn();
        }
        if (killInfoGO.Count > 4)
        {
            GameObject obj4 = (GameObject)killInfoGO[0];
            if (obj4 != null)
            {
                obj4.GetComponent<KillInfoComponent>().Destroy();
            }
            killInfoGO.RemoveAt(0);
        }
        obj3.transform.parent = obj2.GetComponent<UIReferArray>().panels[0].transform;
        obj3.GetComponent<KillInfoComponent>().Show(t1, killer, t2, victim, dmg);
        killInfoGO.Add(obj3);
        if ((int)settings[244] == 1)
        {
            string str2 = "<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> " + killer.hexColor() + " killed ";
            string newLine = str2 + victim.hexColor() + " for " + dmg + " damage.";
            Mod.Interface.Chat.System(newLine);
        }
    }

    [RPC]
    public void VerifyPlayerHasLeft(int ID, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient && Player.Find(ID) != null)
        {
            Player player = Player.Find(ID);
            banHash.Add(ID, player.Properties.Name);
        }
    }


    [RPC]
    public void SpawnPlayerAtRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient && logicLoaded && customLevelLoaded && !needChooseSide && Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            Vector3 position = new Vector3(posX, posY, posZ);
            IN_GAME_MAIN_CAMERA component = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>();
            component.setMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", position, new Quaternion(0f, 0f, 0f, 1f), 0));
            string slot = myLastHero.ToUpper();
            switch (slot)
            {
                case "SET 1": //TODO: Remove and use ProfileSystem
                case "SET 2":
                case "SET 3":
                    {
                        HeroCostume costume = CostumeConverter.LocalDataToHeroCostume(slot);
                        costume?.checkstat();
                        CostumeConverter.HeroCostumeToLocalData(costume, slot);
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
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
                        component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
                        break;
                    }
                default:
                    foreach (HeroCostume hero in HeroCostume.costume)
                    {
                        if (hero.name.EqualsIgnoreCase(slot))
                        {
                            int id = hero.id;
                            if (slot.ToUpper() != "AHSS")
                                id += CheckBoxCostume.costumeSet - 1;
                            if (HeroCostume.costume[id].name != hero.name)
                                id = hero.id + 1;

                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[id];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(HeroCostume.costume[id].name);
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                            component.main_object.GetComponent<HERO>().setStat2();
                            component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
                            break;
                        }
                    }
                    break;
            }
            CostumeConverter.HeroCostumeToPhotonData(component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, Player.Self);
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                Transform transform1 = component.main_object.transform;
                transform1.position += new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20));
            }

            Player.Self.SetCustomProperties(new Hashtable
            {
                { "dead", false },
                { PlayerProperty.IsTitan, 1 }
            });
            component.enabled = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
            GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
            component.gameOver = false;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            Screen.showCursor = false;
            isLosing = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    public void TitanGetKill(Player player, int Damage, string name1, PhotonMessageInfo info)
    {
        if (info != null && !info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(TitanGetKill), info);

        Damage = Mathf.Max(10, Damage);
        object[] parameters = { Damage };
        photonView.RPC("netShowDamage", player, parameters);
        object[] objArray2 = { name1, false };
        photonView.RPC("oneTitanDown", PhotonTargets.MasterClient, objArray2);
        SendKillInfo(false, player.Properties.Name, true, name1, Damage);
        PlayerKillInfoUpdate(player, Damage);
    }

    [RPC]
    private void ChatPM(string sender, string content, PhotonMessageInfo info) //TODO: Customize PMs message
    {
        Mod.Interface.Chat.ReceivePrivateMessage(info.sender, $"<color=#1068D4>PM</color><color=#108CD4>></color> <color=#{Mod.Interface.Chat.SystemColor}>{info.sender.HexName}: {content}</color>");
    }

    #endregion

    #region Unity & Photon methods

    public void OnConnectedToMaster()
    {
    }

    public void OnConnectedToPhoton()
    {
    }

    public void OnConnectionFail(DisconnectCause cause)
    {
        print("OnConnectionFail : " + cause);
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
        gameStart = false;
        //        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], false);
        //        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
        //        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
        //        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
        //        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[4], true);
        GameObject.Find("LabelDisconnectInfo").GetComponent<UILabel>().text = "OnConnectionFail : " + cause;
    }

    public void OnCreatedRoom()
    {
        racingResult = new ArrayList();
        teamScores = new int[2];
        print("OnCreatedRoom");
    }

    public void OnCustomAuthenticationFailed()
    {
        print("OnCustomAuthenticationFailed");
    }

    public void OnDisconnectedFromPhoton()
    {
        print("OnDisconnectedFromPhoton");
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    public void OnFailedToConnectToPhoton()
    {
        print("OnFailedToConnectToPhoton");
    }

    public void OnGUI()
    {
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.STOP)
        {
            bool flag2;
            int num13;
            int num18;
            TextEditor editor;
            int num23;
            Event current;
            bool flag4;
            string str4;
            bool flag5;
            Texture2D textured;
            bool flag6;
            int num30;
            bool flag10;
            if ((int)settings[64] >= 100)
            {
                GameObject obj4;
                float num14;
                Color color;
                Mesh mesh;
                Color[] colorArray;
                float num20;
                float num21;
                float num27;
                int num28;
                int num29;
                float num11 = Screen.width - 300f;
                GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                GUI.DrawTexture(new Rect(7f, 7f, 291f, 586f), textureBackgroundBlue);
                GUI.DrawTexture(new Rect(num11 + 2f, 7f, 291f, 586f), textureBackgroundBlue);
                flag2 = false;
                bool flag3 = false;
                GUI.Box(new Rect(5f, 5f, 295f, 590f), string.Empty);
                GUI.Box(new Rect(num11, 5f, 295f, 590f), string.Empty);
                if (GUI.Button(new Rect(10f, 10f, 60f, 25f), "Script", "box"))
                {
                    settings[68] = 100;
                }
                if (GUI.Button(new Rect(75f, 10f, 65f, 25f), "Controls", "box"))
                {
                    settings[68] = 101;
                }
                if (GUI.Button(new Rect(210f, 10f, 80f, 25f), "Full Screen", "box"))
                {
                    Screen.fullScreen = !Screen.fullScreen;
                    if (Screen.fullScreen)
                    {
                        Screen.SetResolution(960, 600, false);
                    }
                    else
                    {
                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    }
                }
                if ((int)settings[68] == 100 || (int)settings[68] == 102)
                {
                    string str2;
                    int num19;
                    GUI.Label(new Rect(115f, 40f, 100f, 20f), "Level Script:", "Label");
                    GUI.Label(new Rect(115f, 115f, 100f, 20f), "Import Data", "Label");
                    GUI.Label(new Rect(12f, 535f, 280f, 60f), "Warning: your current level will be lost if you quit or import data. Make sure to save the level to a text document.", "Label");
                    settings[77] = GUI.TextField(new Rect(10f, 140f, 285f, 350f), (string)settings[77]);
                    if (GUI.Button(new Rect(35f, 500f, 60f, 30f), "Apply"))
                    {
                        foreach (var o in FindObjectsOfType(typeof(GameObject)))
                        {
                            var obj2 = (GameObject)o;
                            if (obj2.name.StartsWith("custom") || obj2.name.StartsWith("base") || obj2.name.StartsWith("photon") || obj2.name.StartsWith("spawnpoint") || obj2.name.StartsWith("misc") || obj2.name.StartsWith("racing"))
                            {
                                Destroy(obj2);
                            }
                        }
                        linkHash[3].Clear();
                        settings[186] = 0;
                        string[] strArray = Regex.Replace((string)settings[77], @"\s+", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Split(';');
                        for (num13 = 0; num13 < strArray.Length; num13++)
                        {
                            string[] strArray2 = strArray[num13].Split(',');
                            if (strArray2[0].StartsWith("custom") || strArray2[0].StartsWith("base") || strArray2[0].StartsWith("photon") || strArray2[0].StartsWith("spawnpoint") || strArray2[0].StartsWith("misc") || strArray2[0].StartsWith("racing"))
                            {
                                float num15;
                                float num16;
                                float num17;
                                GameObject obj3 = null;
                                if (strArray2[0].StartsWith("custom"))
                                {
                                    obj3 = (GameObject)Instantiate((GameObject)RCassets.Load(strArray2[1]), new Vector3(Convert.ToSingle(strArray2[12]), Convert.ToSingle(strArray2[13]), Convert.ToSingle(strArray2[14])), new Quaternion(Convert.ToSingle(strArray2[15]), Convert.ToSingle(strArray2[16]), Convert.ToSingle(strArray2[17]), Convert.ToSingle(strArray2[18])));
                                }
                                else if (strArray2[0].StartsWith("photon"))
                                {
                                    if (strArray2[1].StartsWith("Cannon"))
                                    {
                                        if (strArray2.Length < 15)
                                        {
                                            obj3 = (GameObject)Instantiate((GameObject)RCassets.Load(strArray2[1] + "Prop"), new Vector3(Convert.ToSingle(strArray2[2]), Convert.ToSingle(strArray2[3]), Convert.ToSingle(strArray2[4])), new Quaternion(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8])));
                                        }
                                        else
                                        {
                                            obj3 = (GameObject)Instantiate((GameObject)RCassets.Load(strArray2[1] + "Prop"), new Vector3(Convert.ToSingle(strArray2[12]), Convert.ToSingle(strArray2[13]), Convert.ToSingle(strArray2[14])), new Quaternion(Convert.ToSingle(strArray2[15]), Convert.ToSingle(strArray2[16]), Convert.ToSingle(strArray2[17]), Convert.ToSingle(strArray2[18])));
                                        }
                                    }
                                    else
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)RCassets.Load(strArray2[1]), new Vector3(Convert.ToSingle(strArray2[4]), Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6])), new Quaternion(Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10])));
                                    }
                                }
                                else if (strArray2[0].StartsWith("spawnpoint"))
                                {
                                    obj3 = (GameObject)Instantiate((GameObject)RCassets.Load(strArray2[1]), new Vector3(Convert.ToSingle(strArray2[2]), Convert.ToSingle(strArray2[3]), Convert.ToSingle(strArray2[4])), new Quaternion(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8])));
                                }
                                else if (strArray2[0].StartsWith("base"))
                                {
                                    if (strArray2.Length < 15)
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)Resources.Load(strArray2[1]), new Vector3(Convert.ToSingle(strArray2[2]), Convert.ToSingle(strArray2[3]), Convert.ToSingle(strArray2[4])), new Quaternion(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8])));
                                    }
                                    else
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)Resources.Load(strArray2[1]), new Vector3(Convert.ToSingle(strArray2[12]), Convert.ToSingle(strArray2[13]), Convert.ToSingle(strArray2[14])), new Quaternion(Convert.ToSingle(strArray2[15]), Convert.ToSingle(strArray2[16]), Convert.ToSingle(strArray2[17]), Convert.ToSingle(strArray2[18])));
                                    }
                                }
                                else if (strArray2[0].StartsWith("misc"))
                                {
                                    if (strArray2[1].StartsWith("barrier"))
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)RCassets.Load("barrierEditor"), new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7])), new Quaternion(Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]), Convert.ToSingle(strArray2[11])));
                                    }
                                    else if (strArray2[1].StartsWith("region"))
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)RCassets.Load("regionEditor"));
                                        obj3.transform.position = new Vector3(Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8]));
                                        obj4 = (GameObject)Instantiate(Resources.Load("UI/LabelNameOverHead"));
                                        obj4.name = "RegionLabel";
                                        obj4.transform.parent = obj3.transform;
                                        num14 = 1f;
                                        if (Convert.ToSingle(strArray2[4]) > 100f)
                                        {
                                            num14 = 0.8f;
                                        }
                                        else if (Convert.ToSingle(strArray2[4]) > 1000f)
                                        {
                                            num14 = 0.5f;
                                        }
                                        obj4.transform.localPosition = new Vector3(0f, num14, 0f);
                                        obj4.transform.localScale = new Vector3(5f / Convert.ToSingle(strArray2[3]), 5f / Convert.ToSingle(strArray2[4]), 5f / Convert.ToSingle(strArray2[5]));
                                        obj4.GetComponent<UILabel>().text = strArray2[2];
                                        obj3.AddComponent<RCRegionLabel>();
                                        obj3.GetComponent<RCRegionLabel>().myLabel = obj4;
                                    }
                                    else if (strArray2[1].StartsWith("racingStart"))
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)RCassets.Load("racingStart"), new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7])), new Quaternion(Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]), Convert.ToSingle(strArray2[11])));
                                    }
                                    else if (strArray2[1].StartsWith("racingEnd"))
                                    {
                                        obj3 = (GameObject)Instantiate((GameObject)RCassets.Load("racingEnd"), new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7])), new Quaternion(Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]), Convert.ToSingle(strArray2[11])));
                                    }
                                }
                                else if (strArray2[0].StartsWith("racing"))
                                {
                                    obj3 = (GameObject)Instantiate((GameObject)RCassets.Load(strArray2[1]), new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7])), new Quaternion(Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]), Convert.ToSingle(strArray2[11])));
                                }
                                if (strArray2[2] != "default" && (strArray2[0].StartsWith("custom") || strArray2[0].StartsWith("base") && strArray2.Length > 15 || strArray2[0].StartsWith("photon") && strArray2.Length > 15))
                                {
                                    foreach (Renderer renderer1 in obj3.GetComponentsInChildren<Renderer>())
                                    {
                                        if (!(renderer1.name.Contains("Particle System") && obj3.name.Contains("aot_supply")))
                                        {
                                            renderer1.material = (Material)RCassets.Load(strArray2[2]);
                                            renderer1.material.mainTextureScale = new Vector2(renderer1.material.mainTextureScale.x * Convert.ToSingle(strArray2[10]), renderer1.material.mainTextureScale.y * Convert.ToSingle(strArray2[11]));
                                        }
                                    }
                                }
                                if (strArray2[0].StartsWith("custom") || strArray2[0].StartsWith("base") && strArray2.Length > 15 || strArray2[0].StartsWith("photon") && strArray2.Length > 15)
                                {
                                    num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[3]);
                                    num15 -= 0.001f;
                                    num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[4]);
                                    num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[5]);
                                    obj3.transform.localScale = new Vector3(num15, num16, num17);
                                    if (strArray2[6] != "0")
                                    {
                                        color = new Color(Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), 1f);
                                        foreach (MeshFilter filter in obj3.GetComponentsInChildren<MeshFilter>())
                                        {
                                            mesh = filter.mesh;
                                            colorArray = new Color[mesh.vertexCount];
                                            num18 = 0;
                                            while (num18 < mesh.vertexCount)
                                            {
                                                colorArray[num18] = color;
                                                num18++;
                                            }
                                            mesh.colors = colorArray;
                                        }
                                    }
                                    obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," + strArray2[3] + "," + strArray2[4] + "," + strArray2[5] + "," + strArray2[6] + "," + strArray2[7] + "," + strArray2[8] + "," + strArray2[9] + "," + strArray2[10] + "," + strArray2[11];
                                }
                                else if (strArray2[0].StartsWith("misc"))
                                {
                                    if (strArray2[1].StartsWith("barrier") || strArray2[1].StartsWith("racing"))
                                    {
                                        num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[2]);
                                        num15 -= 0.001f;
                                        num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[3]);
                                        num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[4]);
                                        obj3.transform.localScale = new Vector3(num15, num16, num17);
                                        obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," + strArray2[3] + "," + strArray2[4];
                                    }
                                    else if (strArray2[1].StartsWith("region"))
                                    {
                                        num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[3]);
                                        num15 -= 0.001f;
                                        num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[4]);
                                        num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[5]);
                                        obj3.transform.localScale = new Vector3(num15, num16, num17);
                                        obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," + strArray2[3] + "," + strArray2[4] + "," + strArray2[5];
                                    }
                                }
                                else if (strArray2[0].StartsWith("racing"))
                                {
                                    num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[2]);
                                    num15 -= 0.001f;
                                    num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[3]);
                                    num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[4]);
                                    obj3.transform.localScale = new Vector3(num15, num16, num17);
                                    obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," + strArray2[3] + "," + strArray2[4];
                                }
                                else if (!(!strArray2[0].StartsWith("photon") || strArray2[1].StartsWith("Cannon")))
                                {
                                    obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," + strArray2[3];
                                }
                                else
                                {
                                    obj3.name = strArray2[0] + "," + strArray2[1];
                                }
                                linkHash[3].Add(obj3.GetInstanceID(), strArray[num13]);
                            }
                            else if (strArray2[0].StartsWith("map") && strArray2[1].StartsWith("disablebounds"))
                            {
                                settings[186] = 1;
                                if (!linkHash[3].ContainsKey("mapbounds"))
                                {
                                    linkHash[3].Add("mapbounds", "map,disablebounds");
                                }
                            }
                        }
                        UnloadAssets();
                        settings[77] = string.Empty;
                    }
                    else if (GUI.Button(new Rect(205f, 500f, 60f, 30f), "Exit"))
                    {
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
                        inputManager.menuOn = false;
                        Destroy(GameObject.Find("MultiplayerManager"));
                        Application.LoadLevel("menu");
                    }
                    else if (GUI.Button(new Rect(15f, 70f, 115f, 30f), "Copy to Clipboard"))
                    {
                        str2 = string.Empty;
                        num19 = 0;
                        foreach (string str3 in linkHash[3].Values)
                        {
                            num19++;
                            str2 = str2 + str3 + ";\n";
                        }
                        editor = new TextEditor
                        {
                            content = new GUIContent(str2)
                        };
                        editor.SelectAll();
                        editor.Copy();
                    }
                    else if (GUI.Button(new Rect(175f, 70f, 115f, 30f), "View Script"))
                    {
                        settings[68] = 102;
                    }
                    if ((int)settings[68] == 102)
                    {
                        str2 = string.Empty;
                        num19 = 0;
                        foreach (string str3 in linkHash[3].Values)
                        {
                            num19++;
                            str2 = str2 + str3 + ";\n";
                        }
                        num20 = Screen.width / 2f - 110.5f;
                        num21 = Screen.height / 2f - 250f;
                        GUI.DrawTexture(new Rect(num20 + 2f, num21 + 2f, 217f, 496f), textureBackgroundBlue);
                        GUI.Box(new Rect(num20, num21, 221f, 500f), string.Empty);
                        if (GUI.Button(new Rect(num20 + 10f, num21 + 460f, 60f, 30f), "Copy"))
                        {
                            editor = new TextEditor
                            {
                                content = new GUIContent(str2)
                            };
                            editor.SelectAll();
                            editor.Copy();
                        }
                        else if (GUI.Button(new Rect(num20 + 151f, num21 + 460f, 60f, 30f), "Done"))
                        {
                            settings[68] = 100;
                        }
                        GUI.TextArea(new Rect(num20 + 5f, num21 + 5f, 211f, 415f), str2);
                        GUI.Label(new Rect(num20 + 10f, num21 + 430f, 150f, 20f), "Object Count: " + Convert.ToString(num19), "Label");
                    }
                }
                else if ((int)settings[68] == 101)
                {
                    GUI.Label(new Rect(92f, 50f, 180f, 20f), "Level Editor Rebinds:", "Label");
                    GUI.Label(new Rect(12f, 80f, 145f, 20f), "Forward:", "Label");
                    GUI.Label(new Rect(12f, 105f, 145f, 20f), "Back:", "Label");
                    GUI.Label(new Rect(12f, 130f, 145f, 20f), "Left:", "Label");
                    GUI.Label(new Rect(12f, 155f, 145f, 20f), "Right:", "Label");
                    GUI.Label(new Rect(12f, 180f, 145f, 20f), "Up:", "Label");
                    GUI.Label(new Rect(12f, 205f, 145f, 20f), "Down:", "Label");
                    GUI.Label(new Rect(12f, 230f, 145f, 20f), "Toggle Cursor:", "Label");
                    GUI.Label(new Rect(12f, 255f, 145f, 20f), "Place Object:", "Label");
                    GUI.Label(new Rect(12f, 280f, 145f, 20f), "Delete Object:", "Label");
                    GUI.Label(new Rect(12f, 305f, 145f, 20f), "Movement-Slow:", "Label");
                    GUI.Label(new Rect(12f, 330f, 145f, 20f), "Rotate Forward:", "Label");
                    GUI.Label(new Rect(12f, 355f, 145f, 20f), "Rotate Backward:", "Label");
                    GUI.Label(new Rect(12f, 380f, 145f, 20f), "Rotate Left:", "Label");
                    GUI.Label(new Rect(12f, 405f, 145f, 20f), "Rotate Right:", "Label");
                    GUI.Label(new Rect(12f, 430f, 145f, 20f), "Rotate CCW:", "Label");
                    GUI.Label(new Rect(12f, 455f, 145f, 20f), "Rotate CW:", "Label");
                    GUI.Label(new Rect(12f, 480f, 145f, 20f), "Movement-Speedup:", "Label");
                    for (num13 = 0; num13 < 17; num13++)
                    {
                        float num22 = 80f + 25f * num13;
                        num23 = 117 + num13;
                        if (num13 == 16)
                        {
                            num23 = 161;
                        }
                        if (GUI.Button(new Rect(135f, num22, 60f, 20f), (string)settings[num23], "box"))
                        {
                            settings[num23] = "waiting...";
                            settings[100] = num23;
                        }
                    }
                    if ((int)settings[100] != 0)
                    {
                        current = Event.current;
                        flag4 = false;
                        str4 = "waiting...";
                        if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
                        {
                            flag4 = true;
                            str4 = current.keyCode.ToString();
                        }
                        else if (Input.GetKey(KeyCode.LeftShift))
                        {
                            flag4 = true;
                            str4 = KeyCode.LeftShift.ToString();
                        }
                        else if (Input.GetKey(KeyCode.RightShift))
                        {
                            flag4 = true;
                            str4 = KeyCode.RightShift.ToString();
                        }
                        else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                        {
                            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                            {
                                flag4 = true;
                                str4 = "Scroll Up";
                            }
                            else
                            {
                                flag4 = true;
                                str4 = "Scroll Down";
                            }
                        }
                        else
                        {
                            num13 = 0;
                            while (num13 < 7)
                            {
                                if (Input.GetKeyDown((KeyCode)(323 + num13)))
                                {
                                    flag4 = true;
                                    str4 = "Mouse" + Convert.ToString(num13);
                                }
                                num13++;
                            }
                        }
                        if (flag4)
                        {
                            for (num13 = 0; num13 < 17; num13++)
                            {
                                num23 = 117 + num13;
                                if (num13 == 16)
                                {
                                    num23 = 161;
                                }
                                if ((int)settings[100] == num23)
                                {
                                    settings[num23] = str4;
                                    settings[100] = 0;
                                    inputRC.setInputLevel(num13, str4);
                                }
                            }
                        }
                    }
                    if (GUI.Button(new Rect(100f, 515f, 110f, 30f), "Save Controls"))
                    {
                        PlayerPrefs.SetString("lforward", (string)settings[117]);
                        PlayerPrefs.SetString("lback", (string)settings[118]);
                        PlayerPrefs.SetString("lleft", (string)settings[119]);
                        PlayerPrefs.SetString("lright", (string)settings[120]);
                        PlayerPrefs.SetString("lup", (string)settings[121]);
                        PlayerPrefs.SetString("ldown", (string)settings[122]);
                        PlayerPrefs.SetString("lcursor", (string)settings[123]);
                        PlayerPrefs.SetString("lplace", (string)settings[124]);
                        PlayerPrefs.SetString("ldel", (string)settings[125]);
                        PlayerPrefs.SetString("lslow", (string)settings[126]);
                        PlayerPrefs.SetString("lrforward", (string)settings[127]);
                        PlayerPrefs.SetString("lrback", (string)settings[128]);
                        PlayerPrefs.SetString("lrleft", (string)settings[129]);
                        PlayerPrefs.SetString("lrright", (string)settings[130]);
                        PlayerPrefs.SetString("lrccw", (string)settings[131]);
                        PlayerPrefs.SetString("lrcw", (string)settings[132]);
                        PlayerPrefs.SetString("lfast", (string)settings[161]);
                    }
                }
                if ((int)settings[64] != 105 && (int)settings[64] != 106)
                {
                    GUI.Label(new Rect(num11 + 13f, 445f, 125f, 20f), "Scale Multipliers:", "Label");
                    GUI.Label(new Rect(num11 + 13f, 470f, 50f, 22f), "Length:", "Label");
                    settings[72] = GUI.TextField(new Rect(num11 + 58f, 470f, 40f, 20f), (string)settings[72]);
                    GUI.Label(new Rect(num11 + 13f, 495f, 50f, 20f), "Width:", "Label");
                    settings[70] = GUI.TextField(new Rect(num11 + 58f, 495f, 40f, 20f), (string)settings[70]);
                    GUI.Label(new Rect(num11 + 13f, 520f, 50f, 22f), "Height:", "Label");
                    settings[71] = GUI.TextField(new Rect(num11 + 58f, 520f, 40f, 20f), (string)settings[71]);
                    if ((int)settings[64] <= 106)
                    {
                        GUI.Label(new Rect(num11 + 155f, 554f, 50f, 22f), "Tiling:", "Label");
                        settings[79] = GUI.TextField(new Rect(num11 + 200f, 554f, 40f, 20f), (string)settings[79]);
                        settings[80] = GUI.TextField(new Rect(num11 + 245f, 554f, 40f, 20f), (string)settings[80]);
                        GUI.Label(new Rect(num11 + 219f, 570f, 10f, 22f), "x:", "Label");
                        GUI.Label(new Rect(num11 + 264f, 570f, 10f, 22f), "y:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 445f, 50f, 20f), "Color:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 470f, 10f, 20f), "R:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 495f, 10f, 20f), "G:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 520f, 10f, 20f), "B:", "Label");
                        settings[73] = GUI.HorizontalSlider(new Rect(num11 + 170f, 475f, 100f, 20f), (float)settings[73], 0f, 1f);
                        settings[74] = GUI.HorizontalSlider(new Rect(num11 + 170f, 500f, 100f, 20f), (float)settings[74], 0f, 1f);
                        settings[75] = GUI.HorizontalSlider(new Rect(num11 + 170f, 525f, 100f, 20f), (float)settings[75], 0f, 1f);
                        GUI.Label(new Rect(num11 + 13f, 554f, 57f, 22f), "Material:", "Label");
                        if (GUI.Button(new Rect(num11 + 66f, 554f, 60f, 20f), (string)settings[69]))
                        {
                            settings[78] = 1;
                        }
                        if ((int)settings[78] == 1)
                        {
                            string[] strArray4 = { "bark", "bark2", "bark3", "bark4" };
                            string[] strArray5 = { "wood1", "wood2", "wood3", "wood4" };
                            string[] strArray6 = { "grass", "grass2", "grass3", "grass4" };
                            string[] strArray7 = { "brick1", "brick2", "brick3", "brick4" };
                            string[] strArray8 = { "metal1", "metal2", "metal3", "metal4" };
                            string[] strArray9 = { "rock1", "rock2", "rock3" };
                            string[] strArray10 = { "stone1", "stone2", "stone3", "stone4", "stone5", "stone6", "stone7", "stone8", "stone9", "stone10" };
                            string[] strArray11 = { "earth1", "earth2", "ice1", "lava1", "crystal1", "crystal2", "empty" };
                            List<string[]> list2 = new List<string[]> {
                                strArray4,
                                strArray5,
                                strArray6,
                                strArray7,
                                strArray8,
                                strArray9,
                                strArray10,
                                strArray11
                            };
                            string[] strArray13 = { "bark", "wood", "grass", "brick", "metal", "rock", "stone", "misc", "transparent" };
                            int index = 78;
                            int num25 = 69;
                            num20 = Screen.width / 2f - 110.5f;
                            num21 = Screen.height / 2f - 220f;
                            int num26 = (int)settings[185];
                            num27 = 10f + 104f * (list2[num26].Length / 3 + 1);
                            num27 = Math.Max(num27, 280f);
                            GUI.DrawTexture(new Rect(num20 + 2f, num21 + 2f, 208f, 446f), textureBackgroundBlue);
                            GUI.Box(new Rect(num20, num21, 212f, 450f), string.Empty);
                            for (num13 = 0; num13 < list2.Count; num13++)
                            {
                                num28 = num13 / 3;
                                num29 = num13 % 3;
                                if (GUI.Button(new Rect(num20 + 5f + 69f * num29, num21 + 5f + 30 * num28, 64f, 25f), strArray13[num13], "box"))
                                {
                                    settings[185] = num13;
                                }
                            }
                            scroll2 = GUI.BeginScrollView(new Rect(num20, num21 + 110f, 225f, 290f), scroll2, new Rect(num20, num21 + 110f, 212f, num27), true, true);
                            if (num26 != 8)
                            {
                                for (num13 = 0; num13 < list2[num26].Length; num13++)
                                {
                                    num28 = num13 / 3;
                                    num29 = num13 % 3;
                                    GUI.DrawTexture(new Rect(num20 + 5f + 69f * num29, num21 + 115f + 104f * num28, 64f, 64f), RCLoadTexture("p" + list2[num26][num13]));
                                    if (GUI.Button(new Rect(num20 + 5f + 69f * num29, num21 + 184f + 104f * num28, 64f, 30f), list2[num26][num13]))
                                    {
                                        settings[num25] = list2[num26][num13];
                                        settings[index] = 0;
                                    }
                                }
                            }
                            GUI.EndScrollView();
                            if (GUI.Button(new Rect(num20 + 24f, num21 + 410f, 70f, 30f), "Default"))
                            {
                                settings[num25] = "default";
                                settings[index] = 0;
                            }
                            else if (GUI.Button(new Rect(num20 + 118f, num21 + 410f, 70f, 30f), "Done"))
                            {
                                settings[index] = 0;
                            }
                        }
                        flag5 = false;
                        if ((int)settings[76] == 1)
                        {
                            flag5 = true;
                            textured = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                            textured.SetPixel(0, 0, new Color((float)settings[73], (float)settings[74], (float)settings[75], 1f));
                            textured.Apply();
                            GUI.DrawTexture(new Rect(num11 + 235f, 445f, 30f, 20f), textured, ScaleMode.StretchToFill);
                            Destroy(textured);
                        }
                        flag6 = GUI.Toggle(new Rect(num11 + 193f, 445f, 40f, 20f), flag5, "On");
                        if (flag5 != flag6)
                        {
                            if (flag6)
                            {
                                settings[76] = 1;
                            }
                            else
                            {
                                settings[76] = 0;
                            }
                        }
                    }
                }
                if (GUI.Button(new Rect(num11 + 5f, 10f, 60f, 25f), "General", "box"))
                {
                    settings[64] = 101;
                }
                else if (GUI.Button(new Rect(num11 + 70f, 10f, 70f, 25f), "Geometry", "box"))
                {
                    settings[64] = 102;
                }
                else if (GUI.Button(new Rect(num11 + 145f, 10f, 65f, 25f), "Buildings", "box"))
                {
                    settings[64] = 103;
                }
                else if (GUI.Button(new Rect(num11 + 215f, 10f, 50f, 25f), "Nature", "box"))
                {
                    settings[64] = 104;
                }
                else if (GUI.Button(new Rect(num11 + 5f, 45f, 70f, 25f), "Spawners", "box"))
                {
                    settings[64] = 105;
                }
                else if (GUI.Button(new Rect(num11 + 80f, 45f, 70f, 25f), "Racing", "box"))
                {
                    settings[64] = 108;
                }
                else if (GUI.Button(new Rect(num11 + 155f, 45f, 40f, 25f), "Misc", "box"))
                {
                    settings[64] = 107;
                }
                else if (GUI.Button(new Rect(num11 + 200f, 45f, 70f, 25f), "Credits", "box"))
                {
                    settings[64] = 106;
                }
                if ((int)settings[64] == 101)
                {
                    GameObject obj5;
                    scroll = GUI.BeginScrollView(new Rect(num11, 80f, 305f, 350f), scroll, new Rect(num11, 80f, 300f, 470f), true, true);
                    GUI.Label(new Rect(num11 + 100f, 80f, 120f, 20f), "General Objects:", "Label");
                    GUI.Label(new Rect(num11 + 108f, 245f, 120f, 20f), "Spawn Points:", "Label");
                    GUI.Label(new Rect(num11 + 7f, 415f, 290f, 60f), "* The above titan spawn points apply only to randomly spawned titans specified by the Random Titan #.", "Label");
                    GUI.Label(new Rect(num11 + 7f, 470f, 290f, 60f), "* If team mode is disabled both cyan and magenta spawn points will be randomly chosen for players.", "Label");
                    GUI.DrawTexture(new Rect(num11 + 27f, 110f, 64f, 64f), RCLoadTexture("psupply"));
                    GUI.DrawTexture(new Rect(num11 + 118f, 110f, 64f, 64f), RCLoadTexture("pcannonwall"));
                    GUI.DrawTexture(new Rect(num11 + 209f, 110f, 64f, 64f), RCLoadTexture("pcannonground"));
                    GUI.DrawTexture(new Rect(num11 + 27f, 275f, 64f, 64f), RCLoadTexture("pspawnt"));
                    GUI.DrawTexture(new Rect(num11 + 118f, 275f, 64f, 64f), RCLoadTexture("pspawnplayerC"));
                    GUI.DrawTexture(new Rect(num11 + 209f, 275f, 64f, 64f), RCLoadTexture("pspawnplayerM"));
                    if (GUI.Button(new Rect(num11 + 27f, 179f, 64f, 60f), "Supply"))
                    {
                        flag2 = true;
                        obj5 = (GameObject)Resources.Load("aot_supply");
                        selectedObj = (GameObject)Instantiate(obj5);
                        selectedObj.name = "base,aot_supply";
                    }
                    else if (GUI.Button(new Rect(num11 + 118f, 179f, 64f, 60f), "Cannon \nWall"))
                    {
                        flag2 = true;
                        obj5 = (GameObject)RCassets.Load("CannonWallProp");
                        selectedObj = (GameObject)Instantiate(obj5);
                        selectedObj.name = "photon,CannonWall";
                    }
                    else if (GUI.Button(new Rect(num11 + 209f, 179f, 64f, 60f), "Cannon\n Ground"))
                    {
                        flag2 = true;
                        obj5 = (GameObject)RCassets.Load("CannonGroundProp");
                        selectedObj = (GameObject)Instantiate(obj5);
                        selectedObj.name = "photon,CannonGround";
                    }
                    else if (GUI.Button(new Rect(num11 + 27f, 344f, 64f, 60f), "Titan"))
                    {
                        flag2 = true;
                        flag3 = true;
                        obj5 = (GameObject)RCassets.Load("titan");
                        selectedObj = (GameObject)Instantiate(obj5);
                        selectedObj.name = "spawnpoint,titan";
                    }
                    else if (GUI.Button(new Rect(num11 + 118f, 344f, 64f, 60f), "Player \nCyan"))
                    {
                        flag2 = true;
                        flag3 = true;
                        obj5 = (GameObject)RCassets.Load("playerC");
                        selectedObj = (GameObject)Instantiate(obj5);
                        selectedObj.name = "spawnpoint,playerC";
                    }
                    else if (GUI.Button(new Rect(num11 + 209f, 344f, 64f, 60f), "Player \nMagenta"))
                    {
                        flag2 = true;
                        flag3 = true;
                        obj5 = (GameObject)RCassets.Load("playerM");
                        selectedObj = (GameObject)Instantiate(obj5);
                        selectedObj.name = "spawnpoint,playerM";
                    }
                    GUI.EndScrollView();
                }
                else
                {
                    GameObject obj6;
                    if ((int)settings[64] == 107)
                    {
                        bool flag8;
                        GUI.DrawTexture(new Rect(num11 + 30f, 90f, 64f, 64f), RCLoadTexture("pbarrier"));
                        GUI.DrawTexture(new Rect(num11 + 30f, 199f, 64f, 64f), RCLoadTexture("pregion"));
                        GUI.Label(new Rect(num11 + 110f, 243f, 200f, 22f), "Region Name:", "Label");
                        GUI.Label(new Rect(num11 + 110f, 179f, 200f, 22f), "Disable Map Bounds:", "Label");
                        bool flag7 = false;
                        if ((int)settings[186] == 1)
                        {
                            flag7 = true;
                            if (!linkHash[3].ContainsKey("mapbounds"))
                            {
                                linkHash[3].Add("mapbounds", "map,disablebounds");
                            }
                        }
                        else if (linkHash[3].ContainsKey("mapbounds"))
                        {
                            linkHash[3].Remove("mapbounds");
                        }
                        if (GUI.Button(new Rect(num11 + 30f, 159f, 64f, 30f), "Barrier"))
                        {
                            flag2 = true;
                            flag3 = true;
                            obj6 = (GameObject)RCassets.Load("barrierEditor");
                            selectedObj = (GameObject)Instantiate(obj6);
                            selectedObj.name = "misc,barrier";
                        }
                        else if (GUI.Button(new Rect(num11 + 30f, 268f, 64f, 30f), "Region"))
                        {
                            if ((string)settings[191] == string.Empty)
                            {
                                settings[191] = "Region" + UnityEngine.Random.Range(10000, 99999);
                            }
                            flag2 = true;
                            flag3 = true;
                            obj6 = (GameObject)RCassets.Load("regionEditor");
                            selectedObj = (GameObject)Instantiate(obj6);
                            obj4 = (GameObject)Instantiate(Resources.Load("UI/LabelNameOverHead"));
                            obj4.name = "RegionLabel";
                            if (!float.TryParse((string)settings[71], out _))
                            {
                                settings[71] = "1";
                            }
                            if (!float.TryParse((string)settings[70], out _))
                            {
                                settings[70] = "1";
                            }
                            if (!float.TryParse((string)settings[72], out _))
                            {
                                settings[72] = "1";
                            }
                            obj4.transform.parent = selectedObj.transform;
                            num14 = 1f;
                            if (Convert.ToSingle((string)settings[71]) > 100f)
                            {
                                num14 = 0.8f;
                            }
                            else if (Convert.ToSingle((string)settings[71]) > 1000f)
                            {
                                num14 = 0.5f;
                            }
                            obj4.transform.localPosition = new Vector3(0f, num14, 0f);
                            obj4.transform.localScale = new Vector3(5f / Convert.ToSingle((string)settings[70]), 5f / Convert.ToSingle((string)settings[71]), 5f / Convert.ToSingle((string)settings[72]));
                            obj4.GetComponent<UILabel>().text = (string)settings[191];
                            selectedObj.AddComponent<RCRegionLabel>();
                            selectedObj.GetComponent<RCRegionLabel>().myLabel = obj4;
                            selectedObj.name = "misc,region," + (string)settings[191];
                        }
                        settings[191] = GUI.TextField(new Rect(num11 + 200f, 243f, 75f, 20f), (string)settings[191]);
                        if ((flag8 = GUI.Toggle(new Rect(num11 + 240f, 179f, 40f, 20f), flag7, "On")) != flag7)
                        {
                            if (flag8)
                            {
                                settings[186] = 1;
                            }
                            else
                            {
                                settings[186] = 0;
                            }
                        }
                    }
                    else if ((int)settings[64] == 105)
                    {
                        GameObject obj7;
                        GUI.Label(new Rect(num11 + 95f, 85f, 130f, 20f), "Custom Spawners:", "Label");
                        GUI.DrawTexture(new Rect(num11 + 7.8f, 110f, 64f, 64f), RCLoadTexture("ptitan"));
                        GUI.DrawTexture(new Rect(num11 + 79.6f, 110f, 64f, 64f), RCLoadTexture("pabnormal"));
                        GUI.DrawTexture(new Rect(num11 + 151.4f, 110f, 64f, 64f), RCLoadTexture("pjumper"));
                        GUI.DrawTexture(new Rect(num11 + 223.2f, 110f, 64f, 64f), RCLoadTexture("pcrawler"));
                        GUI.DrawTexture(new Rect(num11 + 7.8f, 224f, 64f, 64f), RCLoadTexture("ppunk"));
                        GUI.DrawTexture(new Rect(num11 + 79.6f, 224f, 64f, 64f), RCLoadTexture("pannie"));
                        if (GUI.Button(new Rect(num11 + 7.8f, 179f, 64f, 30f), "Titan"))
                        {
                            if (!float.TryParse((string)settings[83], out float _))
                            {
                                settings[83] = "30";
                            }
                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject)RCassets.Load("spawnTitan");
                            selectedObj = (GameObject)Instantiate(obj7);
                            num30 = (int)settings[84];
                            selectedObj.name = "photon,spawnTitan," + (string)settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 79.6f, 179f, 64f, 30f), "Aberrant"))
                        {
                            if (!float.TryParse((string)settings[83], out float _))
                            {
                                settings[83] = "30";
                            }
                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject)RCassets.Load("spawnAbnormal");
                            selectedObj = (GameObject)Instantiate(obj7);
                            num30 = (int)settings[84];
                            selectedObj.name = "photon,spawnAbnormal," + (string)settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 151.4f, 179f, 64f, 30f), "Jumper"))
                        {
                            if (!float.TryParse((string)settings[83], out float _))
                            {
                                settings[83] = "30";
                            }
                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject)RCassets.Load("spawnJumper");
                            selectedObj = (GameObject)Instantiate(obj7);
                            num30 = (int)settings[84];
                            selectedObj.name = "photon,spawnJumper," + (string)settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 223.2f, 179f, 64f, 30f), "Crawler"))
                        {
                            if (!float.TryParse((string)settings[83], out float _))
                            {
                                settings[83] = "30";
                            }
                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject)RCassets.Load("spawnCrawler");
                            selectedObj = (GameObject)Instantiate(obj7);
                            num30 = (int)settings[84];
                            selectedObj.name = "photon,spawnCrawler," + (string)settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 7.8f, 293f, 64f, 30f), "Punk"))
                        {
                            if (!float.TryParse((string)settings[83], out float _))
                            {
                                settings[83] = "30";
                            }
                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject)RCassets.Load("spawnPunk");
                            selectedObj = (GameObject)Instantiate(obj7);
                            num30 = (int)settings[84];
                            selectedObj.name = "photon,spawnPunk," + (string)settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 79.6f, 293f, 64f, 30f), "Annie"))
                        {
                            if (!float.TryParse((string)settings[83], out float _))
                            {
                                settings[83] = "30";
                            }
                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject)RCassets.Load("spawnAnnie");
                            selectedObj = (GameObject)Instantiate(obj7);
                            num30 = (int)settings[84];
                            selectedObj.name = "photon,spawnAnnie," + (string)settings[83] + "," + num30;
                        }
                        GUI.Label(new Rect(num11 + 7f, 379f, 140f, 22f), "Spawn Timer:", "Label");
                        settings[83] = GUI.TextField(new Rect(num11 + 100f, 379f, 50f, 20f), (string)settings[83]);
                        GUI.Label(new Rect(num11 + 7f, 356f, 140f, 22f), "Endless spawn:", "Label");
                        GUI.Label(new Rect(num11 + 7f, 405f, 290f, 80f), "* The above settings apply only to the next placed spawner. You can have unique spawn times and settings for each individual titan spawner.", "Label");
                        bool flag9 = (int)settings[84] == 1;
                        flag10 = GUI.Toggle(new Rect(num11 + 100f, 356f, 40f, 20f), flag9, "On");
                        if (flag9 != flag10)
                        {
                            if (flag10)
                            {
                                settings[84] = 1;
                            }
                            else
                            {
                                settings[84] = 0;
                            }
                        }
                    }
                    else
                    {
                        string[] strArray14;
                        if ((int)settings[64] == 102)
                        {
                            strArray14 = new[] { "cuboid", "plane", "sphere", "cylinder", "capsule", "pyramid", "cone", "prism", "arc90", "arc180", "torus", "tube" };
                            for (num13 = 0; num13 < strArray14.Length; num13++)
                            {
                                num29 = num13 % 4;
                                num28 = num13 / 4;
                                GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, 90f + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, 159f + 114f * num28, 64f, 30f), strArray14[num13]))
                                {
                                    flag2 = true;
                                    obj6 = (GameObject)RCassets.Load(strArray14[num13]);
                                    selectedObj = (GameObject)Instantiate(obj6);
                                    selectedObj.name = "custom," + strArray14[num13];
                                }
                            }
                        }
                        else
                        {
                            List<string> list4;
                            GameObject obj8;
                            switch ((int)settings[64])
                            {
                                case 103:
                                    list4 = new List<string> { "arch1", "house1" };
                                    strArray14 = new[] {
                                        "tower1", "tower2", "tower3", "tower4", "tower5", "house1", "house2", "house3", "house4", "house5", "house6", "house7", "house8", "house9", "house10", "house11",
                                        "house12", "house13", "house14", "pillar1", "pillar2", "village1", "village2", "windmill1", "arch1", "canal1", "castle1", "church1", "cannon1", "statue1", "statue2", "wagon1",
                                        "elevator1", "bridge1", "dummy1", "spike1", "wall1", "wall2", "wall3", "wall4", "arena1", "arena2", "arena3", "arena4"
                                    };
                                    num27 = 110f + 114f * ((strArray14.Length - 1) / 4f);
                                    scroll = GUI.BeginScrollView(new Rect(num11, 90f, 303f, 350f), scroll, new Rect(num11, 90f, 300f, num27), true, true);
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, 90f + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, 159f + 114f * num28, 64f, 30f), strArray14[num13]))
                                        {
                                            flag2 = true;
                                            obj8 = (GameObject)RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject)Instantiate(obj8);
                                            if (list4.Contains(strArray14[num13]))
                                            {
                                                selectedObj.name = "customb," + strArray14[num13];
                                            }
                                            else
                                            {
                                                selectedObj.name = "custom," + strArray14[num13];
                                            }
                                        }
                                    }
                                    GUI.EndScrollView();
                                    break;
                                case 104:
                                    list4 = new List<string> { "tree0" };
                                    strArray14 = new[] {
                                        "leaf0", "leaf1", "leaf2", "field1", "field2", "tree0", "tree1", "tree2", "tree3", "tree4", "tree5", "tree6", "tree7", "log1", "log2", "trunk1",
                                        "boulder1", "boulder2", "boulder3", "boulder4", "boulder5", "cave1", "cave2"
                                    };
                                    num27 = 110f + 114f * ((strArray14.Length - 1) / 4f);
                                    scroll = GUI.BeginScrollView(new Rect(num11, 90f, 303f, 350f), scroll, new Rect(num11, 90f, 300f, num27), true, true);
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, 90f + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, 159f + 114f * num28, 64f, 30f), strArray14[num13]))
                                        {
                                            flag2 = true;
                                            obj8 = (GameObject)RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject)Instantiate(obj8);
                                            if (list4.Contains(strArray14[num13]))
                                            {
                                                selectedObj.name = "customb," + strArray14[num13];
                                            }
                                            else
                                            {
                                                selectedObj.name = "custom," + strArray14[num13];
                                            }
                                        }
                                    }
                                    GUI.EndScrollView();
                                    break;
                                case 108:
                                    string[] strArray15 = { "Cuboid", "Plane", "Sphere", "Cylinder", "Capsule", "Pyramid", "Cone", "Prism", "Arc90", "Arc180", "Torus", "Tube" };
                                    strArray14 = new string[12];
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "start" + strArray15[num13];
                                    }
                                    num27 = 110f + 114f * ((strArray14.Length - 1) / 4f);
                                    num27 *= 4f;
                                    num27 += 200f;
                                    scroll = GUI.BeginScrollView(new Rect(num11, 90f, 303f, 350f), scroll, new Rect(num11, 90f, 300f, num27), true, true);
                                    GUI.Label(new Rect(num11 + 90f, 90f, 200f, 22f), "Racing Start Barrier");
                                    int num33 = 125;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f, 30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject)RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject)Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }
                                    num33 += 114 * (strArray14.Length / 4) + 10;
                                    GUI.Label(new Rect(num11 + 93f, num33, 200f, 22f), "Racing End Trigger");
                                    num33 += 35;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "end" + strArray15[num13];
                                    }
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f, 30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject)RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject)Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }
                                    num33 += 114 * (strArray14.Length / 4) + 10;
                                    GUI.Label(new Rect(num11 + 113f, num33, 200f, 22f), "Kill Trigger");
                                    num33 += 35;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "kill" + strArray15[num13];
                                    }
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f, 30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject)RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject)Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }
                                    num33 += 114 * (strArray14.Length / 4) + 10;
                                    GUI.Label(new Rect(num11 + 95f, num33, 200f, 22f), "Checkpoint Trigger");
                                    num33 += 35;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "checkpoint" + strArray15[num13];
                                    }
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f), RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f, 30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject)RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject)Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }
                                    GUI.EndScrollView();
                                    break;
                                case 106:
                                    GUI.Label(new Rect(num11 + 10f, 80f, 200f, 22f), "- Tree 2 designed by Ken P.", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 105f, 250f, 22f), "- Tower 2, House 5 designed by Matthew Santos", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 130f, 200f, 22f), "- Cannon retextured by Mika", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 155f, 200f, 22f), "- Arena 1,2,3 & 4 created by Gun", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 180f, 250f, 22f), "- Cannon Wall/Ground textured by Bellfox", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 205f, 250f, 120f), "- House 7 - 14, Statue1, Statue2, Wagon1, Wall 1, Wall 2, Wall 3, Wall 4, CannonWall, CannonGround, Tower5, Bridge1, Dummy1, Spike1 created by meecube", "Label");
                                    break;
                            }
                        }
                    }
                }
                if (flag2 && selectedObj != null)
                {
                    float y;
                    float num37;
                    float num38;
                    float num39;
                    float z;
                    float num41;
                    string name1;
                    if (!float.TryParse((string)settings[70], out _))
                    {
                        settings[70] = "1";
                    }
                    if (!float.TryParse((string)settings[71], out _))
                    {
                        settings[71] = "1";
                    }
                    if (!float.TryParse((string)settings[72], out _))
                    {
                        settings[72] = "1";
                    }
                    if (!float.TryParse((string)settings[79], out _))
                    {
                        settings[79] = "1";
                    }
                    if (!float.TryParse((string)settings[80], out _))
                    {
                        settings[80] = "1";
                    }
                    if (!flag3)
                    {
                        float a = 1f;
                        if ((string)settings[69] != "default")
                        {
                            if (((string)settings[69]).StartsWith("transparent"))
                            {
                                float.TryParse(((string)settings[69]).Substring(11), out a);
                                foreach (Renderer renderer2 in selectedObj.GetComponentsInChildren<Renderer>())
                                {
                                    renderer2.material = (Material)RCassets.Load("transparent");
                                    renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle((string)settings[79]), renderer2.material.mainTextureScale.y * Convert.ToSingle((string)settings[80]));
                                }
                            }
                            else
                            {
                                foreach (Renderer renderer2 in selectedObj.GetComponentsInChildren<Renderer>())
                                {
                                    if (!(renderer2.name.Contains("Particle System") && selectedObj.name.Contains("aot_supply")))
                                    {
                                        renderer2.material = (Material)RCassets.Load((string)settings[69]);
                                        renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle((string)settings[79]), renderer2.material.mainTextureScale.y * Convert.ToSingle((string)settings[80]));
                                    }
                                }
                            }
                        }
                        y = 1f;
                        foreach (MeshFilter filter in selectedObj.GetComponentsInChildren<MeshFilter>())
                        {
                            if (selectedObj.name.StartsWith("customb"))
                            {
                                if (y < filter.mesh.bounds.size.y)
                                {
                                    y = filter.mesh.bounds.size.y;
                                }
                            }
                            else if (y < filter.mesh.bounds.size.z)
                            {
                                y = filter.mesh.bounds.size.z;
                            }
                        }
                        num37 = selectedObj.transform.localScale.x * Convert.ToSingle((string)settings[70]);
                        num37 -= 0.001f;
                        num38 = selectedObj.transform.localScale.y * Convert.ToSingle((string)settings[71]);
                        num39 = selectedObj.transform.localScale.z * Convert.ToSingle((string)settings[72]);
                        selectedObj.transform.localScale = new Vector3(num37, num38, num39);
                        if ((int)settings[76] == 1)
                        {
                            color = new Color((float)settings[73], (float)settings[74], (float)settings[75], a);
                            foreach (MeshFilter filter in selectedObj.GetComponentsInChildren<MeshFilter>())
                            {
                                mesh = filter.mesh;
                                colorArray = new Color[mesh.vertexCount];
                                num18 = 0;
                                while (num18 < mesh.vertexCount)
                                {
                                    colorArray[num18] = color;
                                    num18++;
                                }
                                mesh.colors = colorArray;
                            }
                        }
                        z = selectedObj.transform.localScale.z;
                        if (selectedObj.name.Contains("boulder2") || selectedObj.name.Contains("boulder3") || selectedObj.name.Contains("field2"))
                        {
                            z *= 0.01f;
                        }
                        num41 = 10f + z * y * 1.2f / 2f;
                        selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * num41, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * num41);
                        selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
                        name1 = selectedObj.name;
                        string[] strArray3 = new string[21];
                        strArray3[0] = name1;
                        strArray3[1] = ",";
                        strArray3[2] = (string)settings[69];
                        strArray3[3] = ",";
                        strArray3[4] = (string)settings[70];
                        strArray3[5] = ",";
                        strArray3[6] = (string)settings[71];
                        strArray3[7] = ",";
                        strArray3[8] = (string)settings[72];
                        strArray3[9] = ",";
                        strArray3[10] = settings[76].ToString();
                        strArray3[11] = ",";
                        float num42 = (float)settings[73];
                        strArray3[12] = num42.ToString(CultureInfo.CurrentCulture);
                        strArray3[13] = ",";
                        num42 = (float)settings[74];
                        strArray3[14] = num42.ToString(CultureInfo.CurrentCulture);
                        strArray3[15] = ",";
                        strArray3[16] = ((float)settings[75]).ToString(CultureInfo.CurrentCulture);
                        strArray3[17] = ",";
                        strArray3[18] = (string)settings[79];
                        strArray3[19] = ",";
                        strArray3[20] = (string)settings[80];
                        selectedObj.name = string.Concat(strArray3);
                        UnloadAssetsEditor();
                    }
                    else if (selectedObj.name.StartsWith("misc"))
                    {
                        if (selectedObj.name.Contains("barrier") || selectedObj.name.Contains("region") || selectedObj.name.Contains("racing"))
                        {
                            y = 1f;
                            num37 = selectedObj.transform.localScale.x * Convert.ToSingle((string)settings[70]);
                            num37 -= 0.001f;
                            num38 = selectedObj.transform.localScale.y * Convert.ToSingle((string)settings[71]);
                            num39 = selectedObj.transform.localScale.z * Convert.ToSingle((string)settings[72]);
                            selectedObj.transform.localScale = new Vector3(num37, num38, num39);
                            z = selectedObj.transform.localScale.z;
                            num41 = 10f + z * y * 1.2f / 2f;
                            selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * num41, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * num41);
                            if (!selectedObj.name.Contains("region"))
                            {
                                selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
                            }
                            name1 = selectedObj.name;
                            selectedObj.name = name1 + "," + (string)settings[70] + "," + (string)settings[71] + "," + (string)settings[72];
                        }
                    }
                    else if (selectedObj.name.StartsWith("racing"))
                    {
                        y = 1f;
                        num37 = selectedObj.transform.localScale.x * Convert.ToSingle((string)settings[70]);
                        num37 -= 0.001f;
                        num38 = selectedObj.transform.localScale.y * Convert.ToSingle((string)settings[71]);
                        num39 = selectedObj.transform.localScale.z * Convert.ToSingle((string)settings[72]);
                        selectedObj.transform.localScale = new Vector3(num37, num38, num39);
                        z = selectedObj.transform.localScale.z;
                        num41 = 10f + z * y * 1.2f / 2f;
                        selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * num41, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * num41);
                        selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
                        name1 = selectedObj.name;
                        selectedObj.name = name1 + "," + (string)settings[70] + "," + (string)settings[71] + "," + (string)settings[72];
                    }
                    else
                    {
                        selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * 10f, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * 10f);
                        selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
                    }
                    Screen.lockCursor = true;
                    GUI.FocusControl(null);
                }
            }
            else
            {
                float num7;
                float num8;
                if (inputManager != null && inputManager.menuOn)
                {
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                    if ((int)settings[64] != 6)
                    {
                        num7 = Screen.width / 2f - 350f;
                        num8 = Screen.height / 2f - 250f;
                        GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                        GUI.DrawTexture(new Rect(num7 + 2f, num8 + 2f, 696f, 496f), textureBackgroundBlue);
                        GUI.Box(new Rect(num7, num8, 700f, 500f), string.Empty);
                        if (GUI.Button(new Rect(num7 + 7f, num8 + 7f, 59f, 25f), "General", "box"))
                        {
                            settings[64] = 0;
                        }
                        else if (GUI.Button(new Rect(num7 + 71f, num8 + 7f, 60f, 25f), "Rebinds", "box"))
                        {
                            settings[64] = 1;
                        }
                        else if (GUI.Button(new Rect(num7 + 136f, num8 + 7f, 85f, 25f), "Human Skins", "box"))
                        {
                            settings[64] = 2;
                        }
                        else if (GUI.Button(new Rect(num7 + 226f, num8 + 7f, 85f, 25f), "Titan Skins", "box"))
                        {
                            settings[64] = 3;
                        }
                        else if (GUI.Button(new Rect(num7 + 316f, num8 + 7f, 85f, 25f), "Level Skins", "box"))
                        {
                            settings[64] = 7;
                        }
                        else if (GUI.Button(new Rect(num7 + 406f, num8 + 7f, 85f, 25f), "Custom Map", "box"))
                        {
                            settings[64] = 8;
                        }
                        else if (GUI.Button(new Rect(num7 + 496f, num8 + 7f, 88f, 25f), "Custom Logic", "box"))
                        {
                            settings[64] = 9;
                        }
                        else if (GUI.Button(new Rect(num7 + 589f, num8 + 7f, 95f, 25f), "Game Settings", "box"))
                        {
                            settings[64] = 10;
                        }
                        else if (GUI.Button(new Rect(num7 + 7f, num8 + 37f, 70f, 25f), "Abilities", "box"))
                        {
                            settings[64] = 11;
                        }
                        if ((int)settings[64] == 9)
                        {
                            currentScriptLogic = GUI.TextField(new Rect(num7 + 50f, num8 + 82f, 600f, 270f), currentScriptLogic);
                            if (GUI.Button(new Rect(num7 + 250f, num8 + 365f, 50f, 20f), "Copy"))
                            {
                                editor = new TextEditor
                                {
                                    content = new GUIContent(currentScriptLogic)
                                };
                                editor.SelectAll();
                                editor.Copy();
                            }
                            else if (GUI.Button(new Rect(num7 + 400f, num8 + 365f, 50f, 20f), "Clear"))
                            {
                                currentScriptLogic = string.Empty;
                            }
                        }
                        else if ((int)settings[64] == 11)
                        {
                            GUI.Label(new Rect(num7 + 150f, num8 + 80f, 185f, 22f), "Bomb Mode", "Label");
                            GUI.Label(new Rect(num7 + 80f, num8 + 110f, 80f, 22f), "Color:", "Label");
                            textured = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                            textured.SetPixel(0, 0, new Color((float)settings[246], (float)settings[247], (float)settings[248], (float)settings[249]));
                            textured.Apply();
                            GUI.DrawTexture(new Rect(num7 + 120f, num8 + 113f, 40f, 15f), textured, ScaleMode.StretchToFill);
                            Destroy(textured);
                            GUI.Label(new Rect(num7 + 72f, num8 + 135f, 20f, 22f), "R:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 160f, 20f, 22f), "G:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 185f, 20f, 22f), "B:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 210f, 20f, 22f), "A:", "Label");
                            settings[246] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 138f, 100f, 20f), (float)settings[246], 0f, 1f);
                            settings[247] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 163f, 100f, 20f), (float)settings[247], 0f, 1f);
                            settings[248] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 188f, 100f, 20f), (float)settings[248], 0f, 1f);
                            settings[249] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 213f, 100f, 20f), (float)settings[249], 0.5f, 1f);
                            GUI.Label(new Rect(num7 + 72f, num8 + 235f, 95f, 22f), "Bomb Radius:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 260f, 95f, 22f), "Bomb Range:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 285f, 95f, 22f), "Bomb Speed:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 310f, 95f, 22f), "Bomb CD:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 335f, 95f, 22f), "Unused Points:", "Label");
                            num30 = (int)settings[250];
                            GUI.Label(new Rect(num7 + 168f, num8 + 235f, 20f, 22f), num30.ToString(), "Label");
                            num30 = (int)settings[251];
                            GUI.Label(new Rect(num7 + 168f, num8 + 260f, 20f, 22f), num30.ToString(), "Label");
                            num30 = (int)settings[252];
                            GUI.Label(new Rect(num7 + 168f, num8 + 285f, 20f, 22f), num30.ToString(), "Label");
                            GUI.Label(new Rect(num7 + 168f, num8 + 310f, 20f, 22f), ((int)settings[253]).ToString(), "Label");
                            int num43 = 20 - (int)settings[250] - (int)settings[251] - (int)settings[252] - (int)settings[253];
                            GUI.Label(new Rect(num7 + 168f, num8 + 335f, 20f, 22f), num43.ToString(), "Label");
                            if (GUI.Button(new Rect(num7 + 190f, num8 + 235f, 20f, 20f), "-"))
                            {
                                if ((int)settings[250] > 0)
                                {
                                    settings[250] = (int)settings[250] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 235f, 20f, 20f), "+") && (int)settings[250] < 10 && num43 > 0)
                            {
                                settings[250] = (int)settings[250] + 1;
                            }
                            if (GUI.Button(new Rect(num7 + 190f, num8 + 260f, 20f, 20f), "-"))
                            {
                                if ((int)settings[251] > 0)
                                {
                                    settings[251] = (int)settings[251] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 260f, 20f, 20f), "+") && (int)settings[251] < 10 && num43 > 0)
                            {
                                settings[251] = (int)settings[251] + 1;
                            }
                            if (GUI.Button(new Rect(num7 + 190f, num8 + 285f, 20f, 20f), "-"))
                            {
                                if ((int)settings[252] > 0)
                                {
                                    settings[252] = (int)settings[252] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 285f, 20f, 20f), "+") && (int)settings[252] < 10 && num43 > 0)
                            {
                                settings[252] = (int)settings[252] + 1;
                            }
                            if (GUI.Button(new Rect(num7 + 190f, num8 + 310f, 20f, 20f), "-"))
                            {
                                if ((int)settings[253] > 0)
                                {
                                    settings[253] = (int)settings[253] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 310f, 20f, 20f), "+") && (int)settings[253] < 10 && num43 > 0)
                            {
                                settings[253] = (int)settings[253] + 1;
                            }
                        }
                        else
                        {
                            float num44;
                            switch ((int)settings[64])
                            {
                                case 2:
                                    GUI.Label(new Rect(num7 + 205f, num8 + 52f, 120f, 30f), "Human Skin Mode:", "Label");
                                    flag2 = (int)settings[0] == 1;
                                    flag5 = GUI.Toggle(new Rect(num7 + 325f, num8 + 52f, 40f, 20f), flag2, "On");
                                    if (flag2 != flag5)
                                    {
                                        if (flag5)
                                        {
                                            settings[0] = 1;
                                        }
                                        else
                                        {
                                            settings[0] = 0;
                                        }
                                    }
                                    num44 = 44f;
                                    switch ((int)settings[133])
                                    {
                                        case 0:
                                            if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Human Set 1"))
                                            {
                                                settings[133] = 1;
                                            }
                                            settings[3] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 0f, 230f, 20f), (string)settings[3]);
                                            settings[4] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 1f, 230f, 20f), (string)settings[4]);
                                            settings[5] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 2f, 230f, 20f), (string)settings[5]);
                                            settings[6] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 3f, 230f, 20f), (string)settings[6]);
                                            settings[7] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 4f, 230f, 20f), (string)settings[7]);
                                            settings[8] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 5f, 230f, 20f), (string)settings[8]);
                                            settings[14] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 6f, 230f, 20f), (string)settings[14]);
                                            settings[9] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 0f, 230f, 20f), (string)settings[9]);
                                            settings[10] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 1f, 230f, 20f), (string)settings[10]);
                                            settings[11] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 2f, 230f, 20f), (string)settings[11]);
                                            settings[12] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 3f, 230f, 20f), (string)settings[12]);
                                            settings[13] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 4f, 230f, 20f), (string)settings[13]);
                                            settings[94] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 5f, 230f, 20f), (string)settings[94]);
                                            break;
                                        case 1:
                                            if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Human Set 2"))
                                            {
                                                settings[133] = 2;
                                            }
                                            settings[134] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 0f, 230f, 20f), (string)settings[134]);
                                            settings[135] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 1f, 230f, 20f), (string)settings[135]);
                                            settings[136] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 2f, 230f, 20f), (string)settings[136]);
                                            settings[137] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 3f, 230f, 20f), (string)settings[137]);
                                            settings[138] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 4f, 230f, 20f), (string)settings[138]);
                                            settings[139] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 5f, 230f, 20f), (string)settings[139]);
                                            settings[145] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 6f, 230f, 20f), (string)settings[145]);
                                            settings[140] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 0f, 230f, 20f), (string)settings[140]);
                                            settings[141] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 1f, 230f, 20f), (string)settings[141]);
                                            settings[142] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 2f, 230f, 20f), (string)settings[142]);
                                            settings[143] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 3f, 230f, 20f), (string)settings[143]);
                                            settings[144] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 4f, 230f, 20f), (string)settings[144]);
                                            settings[146] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 5f, 230f, 20f), (string)settings[146]);
                                            break;
                                        case 2:
                                            if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Human Set 3"))
                                            {
                                                settings[133] = 0;
                                            }
                                            settings[147] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 0f, 230f, 20f), (string)settings[147]);
                                            settings[148] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 1f, 230f, 20f), (string)settings[148]);
                                            settings[149] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 2f, 230f, 20f), (string)settings[149]);
                                            settings[150] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 3f, 230f, 20f), (string)settings[150]);
                                            settings[151] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 4f, 230f, 20f), (string)settings[151]);
                                            settings[152] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 5f, 230f, 20f), (string)settings[152]);
                                            settings[158] = GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 6f, 230f, 20f), (string)settings[158]);
                                            settings[153] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 0f, 230f, 20f), (string)settings[153]);
                                            settings[154] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 1f, 230f, 20f), (string)settings[154]);
                                            settings[155] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 2f, 230f, 20f), (string)settings[155]);
                                            settings[156] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 3f, 230f, 20f), (string)settings[156]);
                                            settings[157] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 4f, 230f, 20f), (string)settings[157]);
                                            settings[159] = GUI.TextField(new Rect(num7 + 390f, num8 + 114f + num44 * 5f, 230f, 20f), (string)settings[159]);
                                            break;
                                    }
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 0f, 150f, 20f), "Horse:", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 1f, 227f, 20f), "Hair (model dependent):", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 2f, 150f, 20f), "Eyes:", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 3f, 240f, 20f), "Glass (must have a glass enabled):", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 4f, 150f, 20f), "Face:", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 5f, 150f, 20f), "Skin:", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 6f, 240f, 20f), "Hoodie (costume dependent):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 0f, 240f, 20f), "Costume (model dependent):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 1f, 150f, 20f), "Logo & Cape:", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 2f, 240f, 20f), "3DMG Center & 3DMG/Blade/Gun(left):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 3f, 227f, 20f), "3DMG/Blade/Gun(right):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 4f, 150f, 20f), "Gas:", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 5f, 150f, 20f), "Weapon Trail:", "Label");
                                    break;
                                case 3:
                                    int num45;
                                    int num46;
                                    GUI.Label(new Rect(num7 + 270f, num8 + 52f, 120f, 30f), "Titan Skin Mode:", "Label");
                                    flag6 = (int)settings[1] == 1;
                                    bool flag11 = GUI.Toggle(new Rect(num7 + 390f, num8 + 52f, 40f, 20f), flag6, "On");
                                    if (flag6 != flag11)
                                    {
                                        if (flag11)
                                        {
                                            settings[1] = 1;
                                        }
                                        else
                                        {
                                            settings[1] = 0;
                                        }
                                    }
                                    GUI.Label(new Rect(num7 + 270f, num8 + 77f, 120f, 30f), "Randomized Pairs:", "Label");
                                    flag6 = (int)settings[32] == 1;
                                    flag11 = GUI.Toggle(new Rect(num7 + 390f, num8 + 77f, 40f, 20f), flag6, "On");
                                    if (flag6 != flag11)
                                    {
                                        if (flag11)
                                        {
                                            settings[32] = 1;
                                        }
                                        else
                                        {
                                            settings[32] = 0;
                                        }
                                    }
                                    GUI.Label(new Rect(num7 + 158f, num8 + 112f, 150f, 20f), "Titan Hair:", "Label");
                                    settings[21] = GUI.TextField(new Rect(num7 + 80f, num8 + 134f, 165f, 20f), (string)settings[21]);
                                    settings[22] = GUI.TextField(new Rect(num7 + 80f, num8 + 156f, 165f, 20f), (string)settings[22]);
                                    settings[23] = GUI.TextField(new Rect(num7 + 80f, num8 + 178f, 165f, 20f), (string)settings[23]);
                                    settings[24] = GUI.TextField(new Rect(num7 + 80f, num8 + 200f, 165f, 20f), (string)settings[24]);
                                    settings[25] = GUI.TextField(new Rect(num7 + 80f, num8 + 222f, 165f, 20f), (string)settings[25]);
                                    if (GUI.Button(new Rect(num7 + 250f, num8 + 134f, 60f, 20f), GetHairType((int)settings[16])))
                                    {
                                        num45 = 16;
                                        num46 = (int)settings[16];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }
                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 156f, 60f, 20f), GetHairType((int)settings[17])))
                                    {
                                        num45 = 17;
                                        num46 = (int)settings[17];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }
                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 178f, 60f, 20f), GetHairType((int)settings[18])))
                                    {
                                        num45 = 18;
                                        num46 = (int)settings[18];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }
                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 200f, 60f, 20f), GetHairType((int)settings[19])))
                                    {
                                        num45 = 19;
                                        num46 = (int)settings[19];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }
                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 222f, 60f, 20f), GetHairType((int)settings[20])))
                                    {
                                        num45 = 20;
                                        num46 = (int)settings[20];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }
                                        settings[num45] = num46;
                                    }
                                    GUI.Label(new Rect(num7 + 158f, num8 + 252f, 150f, 20f), "Titan Eye:", "Label");
                                    settings[26] = GUI.TextField(new Rect(num7 + 80f, num8 + 274f, 230f, 20f), (string)settings[26]);
                                    settings[27] = GUI.TextField(new Rect(num7 + 80f, num8 + 296f, 230f, 20f), (string)settings[27]);
                                    settings[28] = GUI.TextField(new Rect(num7 + 80f, num8 + 318f, 230f, 20f), (string)settings[28]);
                                    settings[29] = GUI.TextField(new Rect(num7 + 80f, num8 + 340f, 230f, 20f), (string)settings[29]);
                                    settings[30] = GUI.TextField(new Rect(num7 + 80f, num8 + 362f, 230f, 20f), (string)settings[30]);
                                    GUI.Label(new Rect(num7 + 455f, num8 + 112f, 150f, 20f), "Titan Body:", "Label");
                                    settings[86] = GUI.TextField(new Rect(num7 + 390f, num8 + 134f, 230f, 20f), (string)settings[86]);
                                    settings[87] = GUI.TextField(new Rect(num7 + 390f, num8 + 156f, 230f, 20f), (string)settings[87]);
                                    settings[88] = GUI.TextField(new Rect(num7 + 390f, num8 + 178f, 230f, 20f), (string)settings[88]);
                                    settings[89] = GUI.TextField(new Rect(num7 + 390f, num8 + 200f, 230f, 20f), (string)settings[89]);
                                    settings[90] = GUI.TextField(new Rect(num7 + 390f, num8 + 222f, 230f, 20f), (string)settings[90]);
                                    GUI.Label(new Rect(num7 + 472f, num8 + 252f, 150f, 20f), "Eren:", "Label");
                                    settings[65] = GUI.TextField(new Rect(num7 + 390f, num8 + 274f, 230f, 20f), (string)settings[65]);
                                    GUI.Label(new Rect(num7 + 470f, num8 + 296f, 150f, 20f), "Annie:", "Label");
                                    settings[66] = GUI.TextField(new Rect(num7 + 390f, num8 + 318f, 230f, 20f), (string)settings[66]);
                                    GUI.Label(new Rect(num7 + 465f, num8 + 340f, 150f, 20f), "Colossal:", "Label");
                                    settings[67] = GUI.TextField(new Rect(num7 + 390f, num8 + 362f, 230f, 20f), (string)settings[67]);
                                    break;
                                case 7:
                                    num44 = 22f;
                                    GUI.Label(new Rect(num7 + 205f, num8 + 52f, 145f, 30f), "Level Skin Mode:", "Label");
                                    bool flag12 = (int)settings[2] == 1;
                                    bool flag13 = GUI.Toggle(new Rect(num7 + 325f, num8 + 52f, 40f, 20f), flag12, "On");
                                    if (flag12 != flag13)
                                    {
                                        if (flag13)
                                        {
                                            settings[2] = 1;
                                        }
                                        else
                                        {
                                            settings[2] = 0;
                                        }
                                    }
                                    if ((int)settings[188] == 0)
                                    {
                                        if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Forest Skins"))
                                        {
                                            settings[188] = 1;
                                        }
                                        GUI.Label(new Rect(num7 + 205f, num8 + 77f, 145f, 30f), "Randomized Pairs:", "Label");
                                        flag12 = (int)settings[50] == 1;
                                        flag13 = GUI.Toggle(new Rect(num7 + 325f, num8 + 77f, 40f, 20f), flag12, "On");
                                        if (flag12 != flag13)
                                        {
                                            if (flag13)
                                            {
                                                settings[50] = 1;
                                            }
                                            else
                                            {
                                                settings[50] = 0;
                                            }
                                        }
                                        scroll = GUI.BeginScrollView(new Rect(num7, num8 + 115f, 712f, 340f), scroll, new Rect(num7, num8 + 115f, 700f, 475f), true, true);
                                        GUI.Label(new Rect(num7 + 79f, num8 + 117f + num44 * 0f, 150f, 20f), "Ground:", "Label");
                                        settings[49] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 1f, 227f, 20f), (string)settings[49]);
                                        GUI.Label(new Rect(num7 + 79f, num8 + 117f + num44 * 2f, 150f, 20f), "Forest Trunks", "Label");
                                        settings[33] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 3f, 227f, 20f), (string)settings[33]);
                                        settings[34] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 4f, 227f, 20f), (string)settings[34]);
                                        settings[35] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 5f, 227f, 20f), (string)settings[35]);
                                        settings[36] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 6f, 227f, 20f), (string)settings[36]);
                                        settings[37] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 7f, 227f, 20f), (string)settings[37]);
                                        settings[38] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 8f, 227f, 20f), (string)settings[38]);
                                        settings[39] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 9f, 227f, 20f), (string)settings[39]);
                                        settings[40] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 10f, 227f, 20f), (string)settings[40]);
                                        GUI.Label(new Rect(num7 + 79f, num8 + 117f + num44 * 11f, 150f, 20f), "Forest Leaves:", "Label");
                                        settings[41] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 12f, 227f, 20f), (string)settings[41]);
                                        settings[42] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 13f, 227f, 20f), (string)settings[42]);
                                        settings[43] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 14f, 227f, 20f), (string)settings[43]);
                                        settings[44] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 15f, 227f, 20f), (string)settings[44]);
                                        settings[45] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 16f, 227f, 20f), (string)settings[45]);
                                        settings[46] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 17f, 227f, 20f), (string)settings[46]);
                                        settings[47] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 18f, 227f, 20f), (string)settings[47]);
                                        settings[48] = GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 19f, 227f, 20f), (string)settings[48]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 0f, 150f, 20f), "Skybox Front:", "Label");
                                        settings[163] = GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 1f, 227f, 20f), (string)settings[163]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 2f, 150f, 20f), "Skybox Back:", "Label");
                                        settings[164] = GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 3f, 227f, 20f), (string)settings[164]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 4f, 150f, 20f), "Skybox Left:", "Label");
                                        settings[165] = GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 5f, 227f, 20f), (string)settings[165]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 6f, 150f, 20f), "Skybox Right:", "Label");
                                        settings[166] = GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 7f, 227f, 20f), (string)settings[166]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 8f, 150f, 20f), "Skybox Up:", "Label");
                                        settings[167] = GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 9f, 227f, 20f), (string)settings[167]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 10f, 150f, 20f), "Skybox Down:", "Label");
                                        settings[168] = GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 11f, 227f, 20f), (string)settings[168]);
                                        GUI.EndScrollView();
                                    }
                                    else if ((int)settings[188] == 1)
                                    {
                                        if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "City Skins"))
                                        {
                                            settings[188] = 0;
                                        }
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 0f, 150f, 20f), "Ground:", "Label");
                                        settings[59] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 1f, 230f, 20f), (string)settings[59]);
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 2f, 150f, 20f), "Wall:", "Label");
                                        settings[60] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 3f, 230f, 20f), (string)settings[60]);
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 4f, 150f, 20f), "Gate:", "Label");
                                        settings[61] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 5f, 230f, 20f), (string)settings[61]);
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 6f, 150f, 20f), "Houses:", "Label");
                                        settings[51] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 7f, 230f, 20f), (string)settings[51]);
                                        settings[52] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 8f, 230f, 20f), (string)settings[52]);
                                        settings[53] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 9f, 230f, 20f), (string)settings[53]);
                                        settings[54] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 10f, 230f, 20f), (string)settings[54]);
                                        settings[55] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 11f, 230f, 20f), (string)settings[55]);
                                        settings[56] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 12f, 230f, 20f), (string)settings[56]);
                                        settings[57] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 13f, 230f, 20f), (string)settings[57]);
                                        settings[58] = GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 14f, 230f, 20f), (string)settings[58]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 0f, 150f, 20f), "Skybox Front:", "Label");
                                        settings[169] = GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 1f, 230f, 20f), (string)settings[169]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 2f, 150f, 20f), "Skybox Back:", "Label");
                                        settings[170] = GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 3f, 230f, 20f), (string)settings[170]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 4f, 150f, 20f), "Skybox Left:", "Label");
                                        settings[171] = GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 5f, 230f, 20f), (string)settings[171]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 6f, 150f, 20f), "Skybox Right:", "Label");
                                        settings[172] = GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 7f, 230f, 20f), (string)settings[172]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 8f, 150f, 20f), "Skybox Up:", "Label");
                                        settings[173] = GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 9f, 230f, 20f), (string)settings[173]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 10f, 150f, 20f), "Skybox Down:", "Label");
                                        settings[174] = GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 11f, 230f, 20f), (string)settings[174]);
                                    }
                                    break;
                                case 4:
                                    GUI.TextArea(new Rect(num7 + 80f, num8 + 52f, 270f, 30f), "Settings saved to playerprefs!", 100, "Label");
                                    break;
                                case 5:
                                    GUI.TextArea(new Rect(num7 + 80f, num8 + 52f, 270f, 30f), "Settings reloaded from playerprefs!", 100, "Label");
                                    break;
                                default:
                                    string[] strArray16;
                                    if ((int)settings[64] == 0)
                                    {
                                        bool flag19;
                                        bool flag20;
                                        bool flag21;
                                        bool flag22;
                                        bool flag25;
                                        bool flag32;
                                        GUI.Label(new Rect(num7 + 150f, num8 + 51f, 185f, 22f), "Graphics", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 81f, 185f, 22f), "Disable custom gas textures:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 106f, 185f, 22f), "Disable weapon trail:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 131f, 185f, 22f), "Disable wind effect:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 156f, 185f, 22f), "Enable vSync:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 184f, 227f, 20f), "FPS Cap (0 for disabled):", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 212f, 150f, 22f), "Texture Quality:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 242f, 150f, 22f), "Overall Quality:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 272f, 185f, 22f), "Disable Mipmapping:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 297f, 185f, 65f), "*Disabling mipmapping will increase custom texture quality at the cost of performance.", "Label");
                                        qualitySlider = GUI.HorizontalSlider(new Rect(num7 + 199f, num8 + 247f, 115f, 20f), qualitySlider, 0f, 1f);
                                        PlayerPrefs.SetFloat("GameQuality", qualitySlider);
                                        if (qualitySlider < 0.167f)
                                        {
                                            QualitySettings.SetQualityLevel(0, true);
                                        }
                                        else if (qualitySlider < 0.33f)
                                        {
                                            QualitySettings.SetQualityLevel(1, true);
                                        }
                                        else if (qualitySlider < 0.5f)
                                        {
                                            QualitySettings.SetQualityLevel(2, true);
                                        }
                                        else if (qualitySlider < 0.67f)
                                        {
                                            QualitySettings.SetQualityLevel(3, true);
                                        }
                                        else if (qualitySlider < 0.83f)
                                        {
                                            QualitySettings.SetQualityLevel(4, true);
                                        }
                                        else if (qualitySlider <= 1f)
                                        {
                                            QualitySettings.SetQualityLevel(5, true);
                                        }
                                        if (!(qualitySlider < 0.9f || Level.StartsWith("Custom")))
                                        {
                                            Camera.main.GetComponent<TiltShift>().enabled = true;
                                        }
                                        else
                                        {
                                            Camera.main.GetComponent<TiltShift>().enabled = false;
                                        }
                                        bool flag14 = false;
                                        bool flag15 = false;
                                        bool flag16 = false;
                                        bool flag17 = false;
                                        bool flag18 = false;
                                        if ((int)settings[15] == 1)
                                        {
                                            flag14 = true;
                                        }
                                        if ((int)settings[92] == 1)
                                        {
                                            flag15 = true;
                                        }
                                        if ((int)settings[93] == 1)
                                        {
                                            flag16 = true;
                                        }
                                        if ((int)settings[63] == 1)
                                        {
                                            flag17 = true;
                                        }
                                        if ((int)settings[183] == 1)
                                        {
                                            flag18 = true;
                                        }
                                        if ((flag19 = GUI.Toggle(new Rect(num7 + 274f, num8 + 81f, 40f, 20f), flag14, "On")) != flag14)
                                        {
                                            if (flag19)
                                            {
                                                settings[15] = 1;
                                            }
                                            else
                                            {
                                                settings[15] = 0;
                                            }
                                        }
                                        if ((flag10 = GUI.Toggle(new Rect(num7 + 274f, num8 + 106f, 40f, 20f), flag15, "On")) != flag15)
                                        {
                                            if (flag10)
                                            {
                                                settings[92] = 1;
                                            }
                                            else
                                            {
                                                settings[92] = 0;
                                            }
                                        }
                                        if ((flag20 = GUI.Toggle(new Rect(num7 + 274f, num8 + 131f, 40f, 20f), flag16, "On")) != flag16)
                                        {
                                            if (flag20)
                                            {
                                                settings[93] = 1;
                                            }
                                            else
                                            {
                                                settings[93] = 0;
                                            }
                                        }
                                        if ((flag21 = GUI.Toggle(new Rect(num7 + 274f, num8 + 156f, 40f, 20f), flag18, "On")) != flag18)
                                        {
                                            if (flag21)
                                            {
                                                settings[183] = 1;
                                                QualitySettings.vSyncCount = 1;
                                            }
                                            else
                                            {
                                                settings[183] = 0;
                                                QualitySettings.vSyncCount = 0;
                                            }
                                            Minimap.WaitAndTryRecaptureInstance(0.5f);
                                        }
                                        if ((flag22 = GUI.Toggle(new Rect(num7 + 274f, num8 + 272f, 40f, 20f), flag17, "On")) != flag17)
                                        {
                                            if (flag22)
                                            {
                                                settings[63] = 1;
                                            }
                                            else
                                            {
                                                settings[63] = 0;
                                            }
                                            linkHash[0].Clear();
                                            linkHash[1].Clear();
                                            linkHash[2].Clear();
                                        }
                                        if (GUI.Button(new Rect(num7 + 254f, num8 + 212f, 60f, 20f), QualityToString(QualitySettings.masterTextureLimit)))
                                        {
                                            if (QualitySettings.masterTextureLimit <= 0)
                                            {
                                                QualitySettings.masterTextureLimit = 2;
                                            }
                                            else
                                            {
                                                QualitySettings.masterTextureLimit--;
                                            }
                                            linkHash[0].Clear();
                                            linkHash[1].Clear();
                                            linkHash[2].Clear();
                                        }
                                        settings[184] = GUI.TextField(new Rect(num7 + 234f, num8 + 184f, 80f, 20f), (string)settings[184]);
                                        Application.targetFrameRate = -1;
                                        if (int.TryParse((string)settings[184], out var num47) && num47 > 0)
                                        {
                                            Application.targetFrameRate = num47;
                                        }
                                        GUI.Label(new Rect(num7 + 470f, num8 + 51f, 185f, 22f), "Snapshots", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 81f, 185f, 22f), "Enable Snapshots:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 106f, 185f, 22f), "Show In Game:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 131f, 227f, 22f), "Snapshot Minimum Damage:", "Label");
                                        settings[95] = GUI.TextField(new Rect(num7 + 563f, num8 + 131f, 65f, 20f), (string)settings[95]);
                                        bool flag23 = false;
                                        bool flag24 = false;
                                        if (PlayerPrefs.GetInt("EnableSS", 0) == 1)
                                        {
                                            flag23 = true;
                                        }
                                        if (PlayerPrefs.GetInt("showSSInGame", 0) == 1)
                                        {
                                            flag24 = true;
                                        }
                                        if ((flag25 = GUI.Toggle(new Rect(num7 + 588f, num8 + 81f, 40f, 20f), flag23, "On")) != flag23)
                                        {
                                            if (flag25)
                                            {
                                                PlayerPrefs.SetInt("EnableSS", 1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("EnableSS", 0);
                                            }
                                        }
                                        bool flag26 = GUI.Toggle(new Rect(num7 + 588f, num8 + 106f, 40f, 20f), flag24, "On");
                                        if (flag24 != flag26)
                                        {
                                            if (flag26)
                                            {
                                                PlayerPrefs.SetInt("showSSInGame", 1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("showSSInGame", 0);
                                            }
                                        }
                                        GUI.Label(new Rect(num7 + 485f, num8 + 161f, 185f, 22f), "Other", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 186f, 80f, 20f), "Volume:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 211f, 95f, 20f), "Mouse Speed:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 236f, 95f, 20f), "Camera Dist:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 261f, 80f, 20f), "Camera Tilt:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 283f, 80f, 20f), "Invert Mouse:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 305f, 80f, 20f), "Speedometer:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 375f, 80f, 20f), "Minimap:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 397f, 100f, 20f), "Game Feed:", "Label");
                                        strArray16 = new[] { "Off", "Speed", "Damage" };
                                        settings[189] = GUI.SelectionGrid(new Rect(num7 + 480f, num8 + 305f, 140f, 60f), (int)settings[189], strArray16, 1, GUI.skin.toggle);
                                        AudioListener.volume = GUI.HorizontalSlider(new Rect(num7 + 478f, num8 + 191f, 150f, 20f), AudioListener.volume, 0f, 1f);
                                        mouseSlider = GUI.HorizontalSlider(new Rect(num7 + 478f, num8 + 216f, 150f, 20f), mouseSlider, 0.1f, 1f);
                                        PlayerPrefs.SetFloat("MouseSensitivity", mouseSlider);
                                        IN_GAME_MAIN_CAMERA.sensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
                                        distanceSlider = GUI.HorizontalSlider(new Rect(num7 + 478f, num8 + 241f, 150f, 20f), distanceSlider, 0f, 1f);
                                        PlayerPrefs.SetFloat("cameraDistance", distanceSlider);
                                        IN_GAME_MAIN_CAMERA.cameraDistance = 0.3f + distanceSlider;
                                        bool flag27 = false;
                                        bool flag28 = false;
                                        bool flag29 = false;
                                        bool flag30 = false;
                                        if ((int)settings[231] == 1)
                                        {
                                            flag29 = true;
                                        }
                                        if ((int)settings[244] == 1)
                                        {
                                            flag30 = true;
                                        }
                                        if (PlayerPrefs.HasKey("cameraTilt"))
                                        {
                                            if (PlayerPrefs.GetInt("cameraTilt") == 1)
                                            {
                                                flag27 = true;
                                            }
                                        }
                                        else
                                        {
                                            PlayerPrefs.SetInt("cameraTilt", 1);
                                        }
                                        if (PlayerPrefs.HasKey("invertMouseY"))
                                        {
                                            if (PlayerPrefs.GetInt("invertMouseY") == -1)
                                            {
                                                flag28 = true;
                                            }
                                        }
                                        else
                                        {
                                            PlayerPrefs.SetInt("invertMouseY", 1);
                                        }
                                        bool flag31 = GUI.Toggle(new Rect(num7 + 480f, num8 + 261f, 40f, 20f), flag27, "On");
                                        if (flag27 != flag31)
                                        {
                                            if (flag31)
                                            {
                                                PlayerPrefs.SetInt("cameraTilt", 1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("cameraTilt", 0);
                                            }
                                        }
                                        if ((flag32 = GUI.Toggle(new Rect(num7 + 480f, num8 + 283f, 40f, 20f), flag28, "On")) != flag28)
                                        {
                                            if (flag32)
                                            {
                                                PlayerPrefs.SetInt("invertMouseY", -1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("invertMouseY", 1);
                                            }
                                        }
                                        bool flag33 = GUI.Toggle(new Rect(num7 + 480f, num8 + 375f, 40f, 20f), flag29, "On");
                                        if (flag29 != flag33)
                                        {
                                            if (flag33)
                                            {
                                                settings[231] = 1;
                                            }
                                            else
                                            {
                                                settings[231] = 0;
                                            }
                                        }
                                        bool flag34 = GUI.Toggle(new Rect(num7 + 480f, num8 + 397f, 40f, 20f), flag30, "On");
                                        if (flag30 != flag34)
                                        {
                                            if (flag34)
                                            {
                                                settings[244] = 1;
                                            }
                                            else
                                            {
                                                settings[244] = 0;
                                            }
                                        }
                                        IN_GAME_MAIN_CAMERA.cameraTilt = PlayerPrefs.GetInt("cameraTilt");
                                        IN_GAME_MAIN_CAMERA.invertY = PlayerPrefs.GetInt("invertMouseY");
                                    }
                                    else if ((int)settings[64] == 10)
                                    {
                                        bool flag35;
                                        bool flag36;
                                        GUI.Label(new Rect(num7 + 200f, num8 + 382f, 400f, 22f), "Master Client only. Changes will take effect upon restart.");
                                        if (GUI.Button(new Rect(num7 + 267.5f, num8 + 50f, 60f, 25f), "Titans"))
                                        {
                                            settings[230] = 0;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 332.5f, num8 + 50f, 40f, 25f), "PVP"))
                                        {
                                            settings[230] = 1;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 377.5f, num8 + 50f, 50f, 25f), "Misc"))
                                        {
                                            settings[230] = 2;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 320f, num8 + 415f, 60f, 30f), "Reset"))
                                        {
                                            settings[192] = 0;
                                            settings[193] = 0;
                                            settings[194] = 0;
                                            settings[195] = 0;
                                            settings[196] = "30";
                                            settings[197] = 0;
                                            settings[198] = "100";
                                            settings[199] = "200";
                                            settings[200] = 0;
                                            settings[201] = "1";
                                            settings[202] = 0;
                                            settings[203] = 0;
                                            settings[204] = "1";
                                            settings[205] = 0;
                                            settings[206] = "1000";
                                            settings[207] = 0;
                                            settings[208] = "1.0";
                                            settings[209] = "3.0";
                                            settings[210] = 0;
                                            settings[211] = "20.0";
                                            settings[212] = "20.0";
                                            settings[213] = "20.0";
                                            settings[214] = "20.0";
                                            settings[215] = "20.0";
                                            settings[216] = 0;
                                            settings[217] = 0;
                                            settings[218] = "1";
                                            settings[219] = 0;
                                            settings[220] = 0;
                                            settings[221] = 0;
                                            settings[222] = "20";
                                            settings[223] = 0;
                                            settings[224] = "10";
                                            settings[225] = string.Empty;
                                            settings[226] = 0;
                                            settings[227] = "50";
                                            settings[228] = 0;
                                            settings[229] = 0;
                                            settings[235] = 0;
                                        }
                                        if ((int)settings[230] == 0)
                                        {
                                            GUI.Label(new Rect(num7 + 100f, num8 + 90f, 160f, 22f), "Custom Titan Number:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 112f, 200f, 22f), "Amount (Integer):", "Label");
                                            settings[204] = GUI.TextField(new Rect(num7 + 250f, num8 + 112f, 50f, 22f), (string)settings[204]);
                                            flag35 = (int)settings[203] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 90f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[203] = 1;
                                                }
                                                else
                                                {
                                                    settings[203] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 152f, 160f, 22f), "Custom Titan Spawns:", "Label");
                                            flag35 = (int)settings[210] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 152f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[210] = 1;
                                                }
                                                else
                                                {
                                                    settings[210] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 174f, 150f, 22f), "Normal (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 196f, 150f, 22f), "Aberrant (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 218f, 150f, 22f), "Jumper (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 240f, 150f, 22f), "Crawler (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 262f, 150f, 22f), "Punk (Decimal):", "Label");
                                            settings[211] = GUI.TextField(new Rect(num7 + 250f, num8 + 174f, 50f, 22f), (string)settings[211]);
                                            settings[212] = GUI.TextField(new Rect(num7 + 250f, num8 + 196f, 50f, 22f), (string)settings[212]);
                                            settings[213] = GUI.TextField(new Rect(num7 + 250f, num8 + 218f, 50f, 22f), (string)settings[213]);
                                            settings[214] = GUI.TextField(new Rect(num7 + 250f, num8 + 240f, 50f, 22f), (string)settings[214]);
                                            settings[215] = GUI.TextField(new Rect(num7 + 250f, num8 + 262f, 50f, 22f), (string)settings[215]);
                                            GUI.Label(new Rect(num7 + 100f, num8 + 302f, 160f, 22f), "Titan Size Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 324f, 150f, 22f), "Minimum (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 346f, 150f, 22f), "Maximum (Decimal):", "Label");
                                            settings[208] = GUI.TextField(new Rect(num7 + 250f, num8 + 324f, 50f, 22f), (string)settings[208]);
                                            settings[209] = GUI.TextField(new Rect(num7 + 250f, num8 + 346f, 50f, 22f), (string)settings[209]);
                                            flag35 = (int)settings[207] == 1;
                                            if ((flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 302f, 40f, 20f), flag35, "On")) != flag35)
                                            {
                                                if (flag36)
                                                {
                                                    settings[207] = 1;
                                                }
                                                else
                                                {
                                                    settings[207] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 90f, 160f, 22f), "Titan Health Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 161f, 150f, 22f), "Minimum (Integer):", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 183f, 150f, 22f), "Maximum (Integer):", "Label");
                                            settings[198] = GUI.TextField(new Rect(num7 + 550f, num8 + 161f, 50f, 22f), (string)settings[198]);
                                            settings[199] = GUI.TextField(new Rect(num7 + 550f, num8 + 183f, 50f, 22f), (string)settings[199]);
                                            strArray16 = new[] { "Off", "Fixed", "Scaled" };
                                            settings[197] = GUI.SelectionGrid(new Rect(num7 + 550f, num8 + 90f, 100f, 66f), (int)settings[197], strArray16, 1, GUI.skin.toggle);
                                            GUI.Label(new Rect(num7 + 400f, num8 + 223f, 160f, 22f), "Titan Damage Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 245f, 150f, 22f), "Damage (Integer):", "Label");
                                            settings[206] = GUI.TextField(new Rect(num7 + 550f, num8 + 245f, 50f, 22f), (string)settings[206]);
                                            flag35 = (int)settings[205] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 223f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[205] = 1;
                                                }
                                                else
                                                {
                                                    settings[205] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 285f, 160f, 22f), "Titan Explode Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 307f, 160f, 22f), "Radius (Integer):", "Label");
                                            settings[196] = GUI.TextField(new Rect(num7 + 550f, num8 + 307f, 50f, 22f), (string)settings[196]);
                                            flag35 = (int)settings[195] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 285f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[195] = 1;
                                                }
                                                else
                                                {
                                                    settings[195] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 347f, 160f, 22f), "Disable Rock Throwing:", "Label");
                                            flag35 = (int)settings[194] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 347f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[194] = 1;
                                                }
                                                else
                                                {
                                                    settings[194] = 0;
                                                }
                                            }
                                        }
                                        else if ((int)settings[230] == 1)
                                        {
                                            GUI.Label(new Rect(num7 + 100f, num8 + 90f, 160f, 22f), "Point Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 112f, 160f, 22f), "Max Points (Integer):", "Label");
                                            settings[227] = GUI.TextField(new Rect(num7 + 250f, num8 + 112f, 50f, 22f), (string)settings[227]);
                                            flag35 = (int)settings[226] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 90f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[226] = 1;
                                                }
                                                else
                                                {
                                                    settings[226] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 152f, 160f, 22f), "PVP Bomb Mode:", "Label");
                                            flag35 = (int)settings[192] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 152f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[192] = 1;
                                                }
                                                else
                                                {
                                                    settings[192] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 182f, 100f, 66f), "Team Mode:", "Label");
                                            strArray16 = new[] { "Off", "No Sort", "Size-Lock", "Skill-Lock" };
                                            settings[193] = GUI.SelectionGrid(new Rect(num7 + 250f, num8 + 182f, 120f, 88f), (int)settings[193], strArray16, 1, GUI.skin.toggle);
                                            GUI.Label(new Rect(num7 + 100f, num8 + 278f, 160f, 22f), "Infection Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 300f, 160f, 22f), "Starting Titans (Integer):", "Label");
                                            settings[201] = GUI.TextField(new Rect(num7 + 250f, num8 + 300f, 50f, 22f), (string)settings[201]);
                                            flag35 = (int)settings[200] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 278f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[200] = 1;
                                                }
                                                else
                                                {
                                                    settings[200] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 330f, 160f, 22f), "Friendly Mode:", "Label");
                                            flag35 = (int)settings[219] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 330f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[219] = 1;
                                                }
                                                else
                                                {
                                                    settings[219] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 90f, 160f, 22f), "Sword/AHSS PVP:", "Label");
                                            strArray16 = new[] { "Off", "Teams", "FFA" };
                                            settings[220] = GUI.SelectionGrid(new Rect(num7 + 550f, num8 + 90f, 100f, 66f), (int)settings[220], strArray16, 1, GUI.skin.toggle);
                                            GUI.Label(new Rect(num7 + 400f, num8 + 164f, 160f, 22f), "No AHSS Air-Reloading:", "Label");
                                            flag35 = (int)settings[228] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 164f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[228] = 1;
                                                }
                                                else
                                                {
                                                    settings[228] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 194f, 160f, 22f), "Cannons kill humans:", "Label");
                                            flag35 = (int)settings[261] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 194f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[261] = 1;
                                                }
                                                else
                                                {
                                                    settings[261] = 0;
                                                }
                                            }
                                        }
                                        else if ((int)settings[230] == 2)
                                        {
                                            GUI.Label(new Rect(num7 + 100f, num8 + 90f, 160f, 22f), "Custom Titans/Wave:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 112f, 160f, 22f), "Amount (Integer):", "Label");
                                            settings[218] = GUI.TextField(new Rect(num7 + 250f, num8 + 112f, 50f, 22f), (string)settings[218]);
                                            flag35 = (int)settings[217] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 90f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[217] = 1;
                                                }
                                                else
                                                {
                                                    settings[217] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 152f, 160f, 22f), "Maximum Waves:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 174f, 160f, 22f), "Amount (Integer):", "Label");
                                            settings[222] = GUI.TextField(new Rect(num7 + 250f, num8 + 174f, 50f, 22f), (string)settings[222]);
                                            flag35 = (int)settings[221] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 152f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[221] = 1;
                                                }
                                                else
                                                {
                                                    settings[221] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 214f, 160f, 22f), "Punks every 5 waves:", "Label");
                                            flag35 = (int)settings[229] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 214f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[229] = 1;
                                                }
                                                else
                                                {
                                                    settings[229] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 100f, num8 + 244f, 160f, 22f), "Global Minimap Disable:", "Label");
                                            flag35 = (int)settings[235] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 244f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[235] = 1;
                                                }
                                                else
                                                {
                                                    settings[235] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 90f, 160f, 22f), "Endless Respawn:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 112f, 160f, 22f), "Respawn Time (Integer):", "Label");
                                            settings[224] = GUI.TextField(new Rect(num7 + 550f, num8 + 112f, 50f, 22f), (string)settings[224]);
                                            flag35 = (int)settings[223] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 90f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[223] = 1;
                                                }
                                                else
                                                {
                                                    settings[223] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 152f, 160f, 22f), "Kick Eren Titan:", "Label");
                                            flag35 = (int)settings[202] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 152f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[202] = 1;
                                                }
                                                else
                                                {
                                                    settings[202] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 182f, 160f, 22f), "Allow Horses:", "Label");
                                            flag35 = (int)settings[216] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 182f, 40f, 20f), flag35, "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[216] = 1;
                                                }
                                                else
                                                {
                                                    settings[216] = 0;
                                                }
                                            }
                                            GUI.Label(new Rect(num7 + 400f, num8 + 212f, 180f, 22f), "Message of the day:", "Label");
                                            settings[225] = GUI.TextField(new Rect(num7 + 400f, num8 + 234f, 200f, 22f), (string)settings[225]);
                                        }
                                    }
                                    else if ((int)settings[64] == 1)
                                    {
                                        List<string> list7;
                                        float num48;
                                        if (GUI.Button(new Rect(num7 + 233f, num8 + 51f, 55f, 25f), "Human"))
                                        {
                                            settings[190] = 0;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 293f, num8 + 51f, 52f, 25f), "Titan"))
                                        {
                                            settings[190] = 1;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 350f, num8 + 51f, 53f, 25f), "Horse"))
                                        {
                                            settings[190] = 2;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 408f, num8 + 51f, 59f, 25f), "Cannon"))
                                        {
                                            settings[190] = 3;
                                        }
                                        if ((int)settings[190] == 0)
                                        {
                                            list7 = new List<string> {
                                                "Forward:", "Backward:", "Left:", "Right:", "Jump:", "Dodge:", "Left Hook:", "Right Hook:", "Both Hooks:", "Lock:", "Attack:", "Special:", "Salute:", "Change Camera:", "Reset:", "Pause:",
                                                "Show/Hide Cursor:", "Fullscreen:", "Change Blade:", "Flare Green:", "Flare Red:", "Flare Black:", "Reel in:", "Reel out:", "Gas Burst:", "Minimap Max:", "Minimap Toggle:", "Minimap Reset:", "Open Chat:", "Live Spectate"
                                            };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 14)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 15;
                                                }
                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f), list7[num13], "Label");
                                            }
                                            bool flag37 = (int)settings[97] == 1;
                                            bool flag38 = (int)settings[116] == 1;
                                            bool flag39 = (int)settings[181] == 1;
                                            bool flag40 = GUI.Toggle(new Rect(num7 + 457f, num8 + 261f, 40f, 20f), flag37, "On");
                                            if (flag37 != flag40)
                                            {
                                                if (flag40)
                                                {
                                                    settings[97] = 1;
                                                }
                                                else
                                                {
                                                    settings[97] = 0;
                                                }
                                            }
                                            bool flag41 = GUI.Toggle(new Rect(num7 + 457f, num8 + 286f, 40f, 20f), flag38, "On");
                                            if (flag38 != flag41)
                                            {
                                                if (flag41)
                                                {
                                                    settings[116] = 1;
                                                }
                                                else
                                                {
                                                    settings[116] = 0;
                                                }
                                            }
                                            bool flag42 = GUI.Toggle(new Rect(num7 + 457f, num8 + 311f, 40f, 20f), flag39, "On");
                                            if (flag39 != flag42)
                                            {
                                                if (flag42)
                                                {
                                                    settings[181] = 1;
                                                }
                                                else
                                                {
                                                    settings[181] = 0;
                                                }
                                            }
                                            for (num13 = 0; num13 < 22; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 14)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 15;
                                                }
                                                if (GUI.Button(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f), inputManager.getKeyRC(num13), "box"))
                                                {
                                                    settings[100] = num13 + 1;
                                                    inputManager.setNameRC(num13, "waiting...");
                                                }
                                            }
                                            if (GUI.Button(new Rect(num7 + 500f, num8 + 261f, 120f, 20f), (string)settings[98], "box"))
                                            {
                                                settings[98] = "waiting...";
                                                settings[100] = 98;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 286f, 120f, 20f), (string)settings[99], "box"))
                                            {
                                                settings[99] = "waiting...";
                                                settings[100] = 99;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 311f, 120f, 20f), (string)settings[182], "box"))
                                            {
                                                settings[182] = "waiting...";
                                                settings[100] = 182;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 336f, 120f, 20f), (string)settings[232], "box"))
                                            {
                                                settings[232] = "waiting...";
                                                settings[100] = 232;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 361f, 120f, 20f), (string)settings[233], "box"))
                                            {
                                                settings[233] = "waiting...";
                                                settings[100] = 233;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 386f, 120f, 20f), (string)settings[234], "box"))
                                            {
                                                settings[234] = "waiting...";
                                                settings[100] = 234;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 411f, 120f, 20f), (string)settings[236], "box"))
                                            {
                                                settings[236] = "waiting...";
                                                settings[100] = 236;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 436f, 120f, 20f), (string)settings[262], "box"))
                                            {
                                                settings[262] = "waiting...";
                                                settings[100] = 262;
                                            }
                                            if ((int)settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 7)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode)(323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }
                                                        num13++;
                                                    }
                                                }
                                                if (flag4)
                                                {
                                                    if ((int)settings[100] == 98)
                                                    {
                                                        settings[98] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.reelin, str4);
                                                    }
                                                    else if ((int)settings[100] == 99)
                                                    {
                                                        settings[99] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.reelout, str4);
                                                    }
                                                    else if ((int)settings[100] == 182)
                                                    {
                                                        settings[182] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.dash, str4);
                                                    }
                                                    else if ((int)settings[100] == 232)
                                                    {
                                                        settings[232] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.mapMaximize, str4);
                                                    }
                                                    else if ((int)settings[100] == 233)
                                                    {
                                                        settings[233] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.mapToggle, str4);
                                                    }
                                                    else if ((int)settings[100] == 234)
                                                    {
                                                        settings[234] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.mapReset, str4);
                                                    }
                                                    else if ((int)settings[100] == 236)
                                                    {
                                                        settings[236] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.chat, str4);
                                                    }
                                                    else if ((int)settings[100] == 262)
                                                    {
                                                        settings[262] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.liveCam, str4);
                                                    }
                                                    else
                                                    {
                                                        for (num13 = 0; num13 < 22; num13++)
                                                        {
                                                            num23 = num13 + 1;
                                                            if ((int)settings[100] == num23)
                                                            {
                                                                inputManager.setKeyRC(num13, str4);
                                                                settings[100] = 0;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((int)settings[190] == 1)
                                        {
                                            list7 = new List<string> { "Forward:", "Back:", "Left:", "Right:", "Walk:", "Jump:", "Punch:", "Slam:", "Grab (front):", "Grab (back):", "Grab (nape):", "Slap:", "Bite:", "Cover Nape:" };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 6)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 7;
                                                }
                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f), list7[num13], "Label");
                                            }
                                            for (num13 = 0; num13 < 14; num13++)
                                            {
                                                num23 = 101 + num13;
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 6)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 7;
                                                }
                                                if (GUI.Button(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f), (string)settings[num23], "box"))
                                                {
                                                    settings[num23] = "waiting...";
                                                    settings[100] = num23;
                                                }
                                            }
                                            if ((int)settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 7)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode)(323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }
                                                        num13++;
                                                    }
                                                }
                                                if (flag4)
                                                {
                                                    for (num13 = 0; num13 < 14; num13++)
                                                    {
                                                        num23 = 101 + num13;
                                                        if ((int)settings[100] == num23)
                                                        {
                                                            settings[num23] = str4;
                                                            settings[100] = 0;
                                                            inputRC.setInputTitan(num13, str4);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((int)settings[190] == 2)
                                        {
                                            list7 = new List<string> { "Forward:", "Back:", "Left:", "Right:", "Walk:", "Jump:", "Mount:" };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 4;
                                                }
                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f), list7[num13], "Label");
                                            }
                                            for (num13 = 0; num13 < 7; num13++)
                                            {
                                                num23 = 237 + num13;
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 4;
                                                }
                                                if (GUI.Button(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f), (string)settings[num23], "box"))
                                                {
                                                    settings[num23] = "waiting...";
                                                    settings[100] = num23;
                                                }
                                            }
                                            if ((int)settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 7)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode)(323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }
                                                        num13++;
                                                    }
                                                }
                                                if (flag4)
                                                {
                                                    for (num13 = 0; num13 < 7; num13++)
                                                    {
                                                        num23 = 237 + num13;
                                                        if ((int)settings[100] == num23)
                                                        {
                                                            settings[num23] = str4;
                                                            settings[100] = 0;
                                                            inputRC.setInputHorse(num13, str4);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((int)settings[190] == 3)
                                        {
                                            list7 = new List<string> { "Rotate Up:", "Rotate Down:", "Rotate Left:", "Rotate Right:", "Fire:", "Mount:", "Slow Rotate:" };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 4;
                                                }
                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f), list7[num13], "Label");
                                            }
                                            for (num13 = 0; num13 < 7; num13++)
                                            {
                                                num23 = 254 + num13;
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 4;
                                                }
                                                if (GUI.Button(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f), (string)settings[num23], "box"))
                                                {
                                                    settings[num23] = "waiting...";
                                                    settings[100] = num23;
                                                }
                                            }
                                            if ((int)settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 6)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode)(323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }
                                                        num13++;
                                                    }
                                                }
                                                if (flag4)
                                                {
                                                    for (num13 = 0; num13 < 7; num13++)
                                                    {
                                                        num23 = 254 + num13;
                                                        if ((int)settings[100] == num23)
                                                        {
                                                            settings[num23] = str4;
                                                            settings[100] = 0;
                                                            inputRC.setInputCannon(num13, str4);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if ((int)settings[64] == 8)
                                    {
                                        GUI.Label(new Rect(num7 + 150f, num8 + 51f, 120f, 22f), "Map Settings", "Label");
                                        GUI.Label(new Rect(num7 + 50f, num8 + 81f, 140f, 20f), "Titan Spawn Cap:", "Label");
                                        settings[85] = GUI.TextField(new Rect(num7 + 155f, num8 + 81f, 30f, 20f), (string)settings[85]);
                                        strArray16 = new[] { "1 Round", "Waves", "PVP", "Racing", "Custom" };
                                        RCSettings.gameType = GUI.SelectionGrid(new Rect(num7 + 190f, num8 + 80f, 140f, 60f), RCSettings.gameType, strArray16, 2, GUI.skin.toggle);
                                        GUI.Label(new Rect(num7 + 150f, num8 + 155f, 150f, 20f), "Level Script:", "Label");
                                        currentScript = GUI.TextField(new Rect(num7 + 50f, num8 + 180f, 275f, 220f), currentScript);
                                        if (GUI.Button(new Rect(num7 + 100f, num8 + 410f, 50f, 25f), "Copy"))
                                        {
                                            editor = new TextEditor
                                            {
                                                content = new GUIContent(currentScript)
                                            };
                                            editor.SelectAll();
                                            editor.Copy();
                                        }
                                        else if (GUI.Button(new Rect(num7 + 225f, num8 + 410f, 50f, 25f), "Clear"))
                                        {
                                            currentScript = string.Empty;
                                        }
                                        GUI.Label(new Rect(num7 + 455f, num8 + 51f, 180f, 20f), "Custom Textures", "Label");
                                        GUI.Label(new Rect(num7 + 375f, num8 + 81f, 180f, 20f), "Ground Skin:", "Label");
                                        settings[162] = GUI.TextField(new Rect(num7 + 375f, num8 + 103f, 275f, 20f), (string)settings[162]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 125f, 150f, 20f), "Skybox Front:", "Label");
                                        settings[175] = GUI.TextField(new Rect(num7 + 375f, num8 + 147f, 275f, 20f), (string)settings[175]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 169f, 150f, 20f), "Skybox Back:", "Label");
                                        settings[176] = GUI.TextField(new Rect(num7 + 375f, num8 + 191f, 275f, 20f), (string)settings[176]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 213f, 150f, 20f), "Skybox Left:", "Label");
                                        settings[177] = GUI.TextField(new Rect(num7 + 375f, num8 + 235f, 275f, 20f), (string)settings[177]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 257f, 150f, 20f), "Skybox Right:", "Label");
                                        settings[178] = GUI.TextField(new Rect(num7 + 375f, num8 + 279f, 275f, 20f), (string)settings[178]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 301f, 150f, 20f), "Skybox Up:", "Label");
                                        settings[179] = GUI.TextField(new Rect(num7 + 375f, num8 + 323f, 275f, 20f), (string)settings[179]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 345f, 150f, 20f), "Skybox Down:", "Label");
                                        settings[180] = GUI.TextField(new Rect(num7 + 375f, num8 + 367f, 275f, 20f), (string)settings[180]);
                                    }
                                    break;
                            }
                        }
                        if (GUI.Button(new Rect(num7 + 408f, num8 + 465f, 42f, 25f), "Save"))
                        {
                            PlayerPrefs.SetInt("human", (int)settings[0]);
                            PlayerPrefs.SetInt("titan", (int)settings[1]);
                            PlayerPrefs.SetInt("level", (int)settings[2]);
                            PlayerPrefs.SetString("horse", (string)settings[3]);
                            PlayerPrefs.SetString("hair", (string)settings[4]);
                            PlayerPrefs.SetString("eye", (string)settings[5]);
                            PlayerPrefs.SetString("glass", (string)settings[6]);
                            PlayerPrefs.SetString("face", (string)settings[7]);
                            PlayerPrefs.SetString("skin", (string)settings[8]);
                            PlayerPrefs.SetString("costume", (string)settings[9]);
                            PlayerPrefs.SetString("logo", (string)settings[10]);
                            PlayerPrefs.SetString("bladel", (string)settings[11]);
                            PlayerPrefs.SetString("blader", (string)settings[12]);
                            PlayerPrefs.SetString("gas", (string)settings[13]);
                            PlayerPrefs.SetString("haircolor", (string)settings[14]);
                            PlayerPrefs.SetInt("gasenable", (int)settings[15]);
                            PlayerPrefs.SetInt("titantype1", (int)settings[16]);
                            PlayerPrefs.SetInt("titantype2", (int)settings[17]);
                            PlayerPrefs.SetInt("titantype3", (int)settings[18]);
                            PlayerPrefs.SetInt("titantype4", (int)settings[19]);
                            PlayerPrefs.SetInt("titantype5", (int)settings[20]);
                            PlayerPrefs.SetString("titanhair1", (string)settings[21]);
                            PlayerPrefs.SetString("titanhair2", (string)settings[22]);
                            PlayerPrefs.SetString("titanhair3", (string)settings[23]);
                            PlayerPrefs.SetString("titanhair4", (string)settings[24]);
                            PlayerPrefs.SetString("titanhair5", (string)settings[25]);
                            PlayerPrefs.SetString("titaneye1", (string)settings[26]);
                            PlayerPrefs.SetString("titaneye2", (string)settings[27]);
                            PlayerPrefs.SetString("titaneye3", (string)settings[28]);
                            PlayerPrefs.SetString("titaneye4", (string)settings[29]);
                            PlayerPrefs.SetString("titaneye5", (string)settings[30]);
                            PlayerPrefs.SetInt("titanR", (int)settings[32]);
                            PlayerPrefs.SetString("tree1", (string)settings[33]);
                            PlayerPrefs.SetString("tree2", (string)settings[34]);
                            PlayerPrefs.SetString("tree3", (string)settings[35]);
                            PlayerPrefs.SetString("tree4", (string)settings[36]);
                            PlayerPrefs.SetString("tree5", (string)settings[37]);
                            PlayerPrefs.SetString("tree6", (string)settings[38]);
                            PlayerPrefs.SetString("tree7", (string)settings[39]);
                            PlayerPrefs.SetString("tree8", (string)settings[40]);
                            PlayerPrefs.SetString("leaf1", (string)settings[41]);
                            PlayerPrefs.SetString("leaf2", (string)settings[42]);
                            PlayerPrefs.SetString("leaf3", (string)settings[43]);
                            PlayerPrefs.SetString("leaf4", (string)settings[44]);
                            PlayerPrefs.SetString("leaf5", (string)settings[45]);
                            PlayerPrefs.SetString("leaf6", (string)settings[46]);
                            PlayerPrefs.SetString("leaf7", (string)settings[47]);
                            PlayerPrefs.SetString("leaf8", (string)settings[48]);
                            PlayerPrefs.SetString("forestG", (string)settings[49]);
                            PlayerPrefs.SetInt("forestR", (int)settings[50]);
                            PlayerPrefs.SetString("house1", (string)settings[51]);
                            PlayerPrefs.SetString("house2", (string)settings[52]);
                            PlayerPrefs.SetString("house3", (string)settings[53]);
                            PlayerPrefs.SetString("house4", (string)settings[54]);
                            PlayerPrefs.SetString("house5", (string)settings[55]);
                            PlayerPrefs.SetString("house6", (string)settings[56]);
                            PlayerPrefs.SetString("house7", (string)settings[57]);
                            PlayerPrefs.SetString("house8", (string)settings[58]);
                            PlayerPrefs.SetString("cityG", (string)settings[59]);
                            PlayerPrefs.SetString("cityW", (string)settings[60]);
                            PlayerPrefs.SetString("cityH", (string)settings[61]);
                            PlayerPrefs.SetInt("skinQ", QualitySettings.masterTextureLimit);
                            PlayerPrefs.SetInt("skinQL", (int)settings[63]);
                            PlayerPrefs.SetString("eren", (string)settings[65]);
                            PlayerPrefs.SetString("annie", (string)settings[66]);
                            PlayerPrefs.SetString("colossal", (string)settings[67]);
                            PlayerPrefs.SetString("hoodie", (string)settings[14]);
                            PlayerPrefs.SetString("cnumber", (string)settings[82]);
                            PlayerPrefs.SetString("cmax", (string)settings[85]);
                            PlayerPrefs.SetString("titanbody1", (string)settings[86]);
                            PlayerPrefs.SetString("titanbody2", (string)settings[87]);
                            PlayerPrefs.SetString("titanbody3", (string)settings[88]);
                            PlayerPrefs.SetString("titanbody4", (string)settings[89]);
                            PlayerPrefs.SetString("titanbody5", (string)settings[90]);
                            PlayerPrefs.SetInt("customlevel", (int)settings[91]);
                            PlayerPrefs.SetInt("traildisable", (int)settings[92]);
                            PlayerPrefs.SetInt("wind", (int)settings[93]);
                            PlayerPrefs.SetString("trailskin", (string)settings[94]);
                            PlayerPrefs.SetString("snapshot", (string)settings[95]);
                            PlayerPrefs.SetString("trailskin2", (string)settings[96]);
                            PlayerPrefs.SetInt("reel", (int)settings[97]);
                            PlayerPrefs.SetString("reelin", (string)settings[98]);
                            PlayerPrefs.SetString("reelout", (string)settings[99]);
                            PlayerPrefs.SetFloat("vol", AudioListener.volume);
                            PlayerPrefs.SetString("tforward", (string)settings[101]);
                            PlayerPrefs.SetString("tback", (string)settings[102]);
                            PlayerPrefs.SetString("tleft", (string)settings[103]);
                            PlayerPrefs.SetString("tright", (string)settings[104]);
                            PlayerPrefs.SetString("twalk", (string)settings[105]);
                            PlayerPrefs.SetString("tjump", (string)settings[106]);
                            PlayerPrefs.SetString("tpunch", (string)settings[107]);
                            PlayerPrefs.SetString("tslam", (string)settings[108]);
                            PlayerPrefs.SetString("tgrabfront", (string)settings[109]);
                            PlayerPrefs.SetString("tgrabback", (string)settings[110]);
                            PlayerPrefs.SetString("tgrabnape", (string)settings[111]);
                            PlayerPrefs.SetString("tantiae", (string)settings[112]);
                            PlayerPrefs.SetString("tbite", (string)settings[113]);
                            PlayerPrefs.SetString("tcover", (string)settings[114]);
                            PlayerPrefs.SetString("tsit", (string)settings[115]);
                            PlayerPrefs.SetInt("reel2", (int)settings[116]);
                            PlayerPrefs.SetInt("humangui", (int)settings[133]);
                            PlayerPrefs.SetString("horse2", (string)settings[134]);
                            PlayerPrefs.SetString("hair2", (string)settings[135]);
                            PlayerPrefs.SetString("eye2", (string)settings[136]);
                            PlayerPrefs.SetString("glass2", (string)settings[137]);
                            PlayerPrefs.SetString("face2", (string)settings[138]);
                            PlayerPrefs.SetString("skin2", (string)settings[139]);
                            PlayerPrefs.SetString("costume2", (string)settings[140]);
                            PlayerPrefs.SetString("logo2", (string)settings[141]);
                            PlayerPrefs.SetString("bladel2", (string)settings[142]);
                            PlayerPrefs.SetString("blader2", (string)settings[143]);
                            PlayerPrefs.SetString("gas2", (string)settings[144]);
                            PlayerPrefs.SetString("hoodie2", (string)settings[145]);
                            PlayerPrefs.SetString("trail2", (string)settings[146]);
                            PlayerPrefs.SetString("horse3", (string)settings[147]);
                            PlayerPrefs.SetString("hair3", (string)settings[148]);
                            PlayerPrefs.SetString("eye3", (string)settings[149]);
                            PlayerPrefs.SetString("glass3", (string)settings[150]);
                            PlayerPrefs.SetString("face3", (string)settings[151]);
                            PlayerPrefs.SetString("skin3", (string)settings[152]);
                            PlayerPrefs.SetString("costume3", (string)settings[153]);
                            PlayerPrefs.SetString("logo3", (string)settings[154]);
                            PlayerPrefs.SetString("bladel3", (string)settings[155]);
                            PlayerPrefs.SetString("blader3", (string)settings[156]);
                            PlayerPrefs.SetString("gas3", (string)settings[157]);
                            PlayerPrefs.SetString("hoodie3", (string)settings[158]);
                            PlayerPrefs.SetString("trail3", (string)settings[159]);
                            PlayerPrefs.SetString("customGround", (string)settings[162]);
                            PlayerPrefs.SetString("forestskyfront", (string)settings[163]);
                            PlayerPrefs.SetString("forestskyback", (string)settings[164]);
                            PlayerPrefs.SetString("forestskyleft", (string)settings[165]);
                            PlayerPrefs.SetString("forestskyright", (string)settings[166]);
                            PlayerPrefs.SetString("forestskyup", (string)settings[167]);
                            PlayerPrefs.SetString("forestskydown", (string)settings[168]);
                            PlayerPrefs.SetString("cityskyfront", (string)settings[169]);
                            PlayerPrefs.SetString("cityskyback", (string)settings[170]);
                            PlayerPrefs.SetString("cityskyleft", (string)settings[171]);
                            PlayerPrefs.SetString("cityskyright", (string)settings[172]);
                            PlayerPrefs.SetString("cityskyup", (string)settings[173]);
                            PlayerPrefs.SetString("cityskydown", (string)settings[174]);
                            PlayerPrefs.SetString("customskyfront", (string)settings[175]);
                            PlayerPrefs.SetString("customskyback", (string)settings[176]);
                            PlayerPrefs.SetString("customskyleft", (string)settings[177]);
                            PlayerPrefs.SetString("customskyright", (string)settings[178]);
                            PlayerPrefs.SetString("customskyup", (string)settings[179]);
                            PlayerPrefs.SetString("customskydown", (string)settings[180]);
                            PlayerPrefs.SetInt("dashenable", (int)settings[181]);
                            PlayerPrefs.SetString("dashkey", (string)settings[182]);
                            PlayerPrefs.SetInt("vsync", (int)settings[183]);
                            PlayerPrefs.SetString("fpscap", (string)settings[184]);
                            PlayerPrefs.SetInt("speedometer", (int)settings[189]);
                            PlayerPrefs.SetInt("bombMode", (int)settings[192]);
                            PlayerPrefs.SetInt("teamMode", (int)settings[193]);
                            PlayerPrefs.SetInt("rockThrow", (int)settings[194]);
                            PlayerPrefs.SetInt("explodeModeOn", (int)settings[195]);
                            PlayerPrefs.SetString("explodeModeNum", (string)settings[196]);
                            PlayerPrefs.SetInt("healthMode", (int)settings[197]);
                            PlayerPrefs.SetString("healthLower", (string)settings[198]);
                            PlayerPrefs.SetString("healthUpper", (string)settings[199]);
                            PlayerPrefs.SetInt("infectionModeOn", (int)settings[200]);
                            PlayerPrefs.SetString("infectionModeNum", (string)settings[201]);
                            PlayerPrefs.SetInt("banEren", (int)settings[202]);
                            PlayerPrefs.SetInt("moreTitanOn", (int)settings[203]);
                            PlayerPrefs.SetString("moreTitanNum", (string)settings[204]);
                            PlayerPrefs.SetInt("damageModeOn", (int)settings[205]);
                            PlayerPrefs.SetString("damageModeNum", (string)settings[206]);
                            PlayerPrefs.SetInt("sizeMode", (int)settings[207]);
                            PlayerPrefs.SetString("sizeLower", (string)settings[208]);
                            PlayerPrefs.SetString("sizeUpper", (string)settings[209]);
                            PlayerPrefs.SetInt("spawnModeOn", (int)settings[210]);
                            PlayerPrefs.SetString("nRate", (string)settings[211]);
                            PlayerPrefs.SetString("aRate", (string)settings[212]);
                            PlayerPrefs.SetString("jRate", (string)settings[213]);
                            PlayerPrefs.SetString("cRate", (string)settings[214]);
                            PlayerPrefs.SetString("pRate", (string)settings[215]);
                            PlayerPrefs.SetInt("horseMode", (int)settings[216]);
                            PlayerPrefs.SetInt("waveModeOn", (int)settings[217]);
                            PlayerPrefs.SetString("waveModeNum", (string)settings[218]);
                            PlayerPrefs.SetInt("friendlyMode", (int)settings[219]);
                            PlayerPrefs.SetInt("pvpMode", (int)settings[220]);
                            PlayerPrefs.SetInt("maxWaveOn", (int)settings[221]);
                            PlayerPrefs.SetString("maxWaveNum", (string)settings[222]);
                            PlayerPrefs.SetInt("endlessModeOn", (int)settings[223]);
                            PlayerPrefs.SetString("endlessModeNum", (string)settings[224]);
                            PlayerPrefs.SetString("motd", (string)settings[225]);
                            PlayerPrefs.SetInt("pointModeOn", (int)settings[226]);
                            PlayerPrefs.SetString("pointModeNum", (string)settings[227]);
                            PlayerPrefs.SetInt("ahssReload", (int)settings[228]);
                            PlayerPrefs.SetInt("punkWaves", (int)settings[229]);
                            PlayerPrefs.SetInt("mapOn", (int)settings[231]);
                            PlayerPrefs.SetString("mapMaximize", (string)settings[232]);
                            PlayerPrefs.SetString("mapToggle", (string)settings[233]);
                            PlayerPrefs.SetString("mapReset", (string)settings[234]);
                            PlayerPrefs.SetInt("globalDisableMinimap", (int)settings[235]);
                            PlayerPrefs.SetString("chatRebind", (string)settings[236]);
                            PlayerPrefs.SetString("hforward", (string)settings[237]);
                            PlayerPrefs.SetString("hback", (string)settings[238]);
                            PlayerPrefs.SetString("hleft", (string)settings[239]);
                            PlayerPrefs.SetString("hright", (string)settings[240]);
                            PlayerPrefs.SetString("hwalk", (string)settings[241]);
                            PlayerPrefs.SetString("hjump", (string)settings[242]);
                            PlayerPrefs.SetString("hmount", (string)settings[243]);
                            PlayerPrefs.SetInt("chatfeed", (int)settings[244]);
                            PlayerPrefs.SetFloat("bombR", (float)settings[246]);
                            PlayerPrefs.SetFloat("bombG", (float)settings[247]);
                            PlayerPrefs.SetFloat("bombB", (float)settings[248]);
                            PlayerPrefs.SetFloat("bombA", (float)settings[249]);
                            PlayerPrefs.SetInt("bombRadius", (int)settings[250]);
                            PlayerPrefs.SetInt("bombRange", (int)settings[251]);
                            PlayerPrefs.SetInt("bombSpeed", (int)settings[252]);
                            PlayerPrefs.SetInt("bombCD", (int)settings[253]);
                            PlayerPrefs.SetString("cannonUp", (string)settings[254]);
                            PlayerPrefs.SetString("cannonDown", (string)settings[255]);
                            PlayerPrefs.SetString("cannonLeft", (string)settings[256]);
                            PlayerPrefs.SetString("cannonRight", (string)settings[257]);
                            PlayerPrefs.SetString("cannonFire", (string)settings[258]);
                            PlayerPrefs.SetString("cannonMount", (string)settings[259]);
                            PlayerPrefs.SetString("cannonSlow", (string)settings[260]);
                            PlayerPrefs.SetInt("deadlyCannon", (int)settings[261]);
                            PlayerPrefs.SetString("liveCam", (string)settings[262]);
                            settings[64] = 4;
                        }
                        else if (GUI.Button(new Rect(num7 + 455f, num8 + 465f, 40f, 25f), "Load"))
                        {
                            LoadConfig();
                            settings[64] = 5;
                        }
                        else if (GUI.Button(new Rect(num7 + 500f, num8 + 465f, 60f, 25f), "Default"))
                        {
                            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().setToDefault();
                        }
                        else if (GUI.Button(new Rect(num7 + 565f, num8 + 465f, 75f, 25f), "Continue"))
                        {
                            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                            {
                                Time.timeScale = 1f;
                            }
                            if (!Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled)
                            {
                                Screen.showCursor = true;
                                Screen.lockCursor = true;
                                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
                                Camera.main.GetComponent<SpectatorMovement>().disable = false;
                                Camera.main.GetComponent<MouseLook>().disable = false;
                            }
                            else
                            {
                                IN_GAME_MAIN_CAMERA.isPausing = false;
                                if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
                                {
                                    Screen.showCursor = false;
                                    Screen.lockCursor = true;
                                }
                                else
                                {
                                    Screen.showCursor = false;
                                    Screen.lockCursor = false;
                                }
                                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
                                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().justUPDATEME();
                            }
                        }
                        else if (GUI.Button(new Rect(num7 + 645f, num8 + 465f, 40f, 25f), "Quit"))
                        {
                            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                            {
                                Time.timeScale = 1f;
                            }
                            else
                            {
                                PhotonNetwork.Disconnect();
                            }
                            Screen.lockCursor = false;
                            Screen.showCursor = true;
                            IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
                            gameStart = false;
                            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
                            DestroyAllExistingCloths();
                            Destroy(GameObject.Find("MultiplayerManager"));
                            Application.LoadLevel("menu");
                        }
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                {
                    if (Time.timeScale <= 0.1f)
                    {
                        num7 = Screen.width / 2f;
                        num8 = Screen.height / 2f;
                        GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                        GUI.DrawTexture(new Rect(num7 - 98f, num8 - 48f, 196f, 96f), textureBackgroundBlue);
                        GUI.Box(new Rect(num7 - 100f, num8 - 50f, 200f, 100f), string.Empty);
                        if (pauseWaitTime <= 3f)
                        {
                            GUI.Label(new Rect(num7 - 43f, num8 - 15f, 200f, 22f), "Unpausing in:");
                            GUI.Label(new Rect(num7 - 8f, num8 + 5f, 200f, 22f), pauseWaitTime.ToString("F1"));
                        }
                        else
                        {
                            GUI.Label(new Rect(num7 - 43f, num8 - 10f, 200f, 22f), "Game Paused.");
                        }
                    }
                    else if (!(logicLoaded && customLevelLoaded))
                    {
                        num7 = Screen.width / 2f;
                        num8 = Screen.height / 2f;
                        GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), textureBackgroundBlack);
                        GUI.DrawTexture(new Rect(num7 - 98f, num8 - 48f, 196f, 146f), textureBackgroundBlue);
                        GUI.Box(new Rect(num7 - 100f, num8 - 50f, 200f, 150f), string.Empty);
                        int length = Player.Self.Properties.CurrentLevel.Length;
                        int num50 = PhotonNetwork.masterClient.Properties.CurrentLevel.Length;
                        GUI.Label(new Rect(num7 - 60f, num8 - 30f, 200f, 22f), "Loading Level (" + length + "/" + num50 + ")");
                        retryTime += Time.deltaTime;
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        if (GUI.Button(new Rect(num7 - 20f, num8 + 50f, 40f, 30f), "Quit"))
                        {
                            PhotonNetwork.Disconnect();
                            Screen.lockCursor = false;
                            Screen.showCursor = true;
                            IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
                            gameStart = false;
                            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
                            DestroyAllExistingCloths();
                            Destroy(GameObject.Find("MultiplayerManager"));
                            Application.LoadLevel("menu");
                        }
                    }
                }
            }
        }
    }

    public void OnJoinedLobby()
    {
        Loading.Stop("ConnectingToLobby");
        ServerList.Alpha = 0f;

        //        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiStart, false);
        //        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiROOM, true);
        //        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiJoinPrivate, false);
    }

    public static Difficulty DifficultyToEnum(string difficulty)
    {
        switch (difficulty.ToLowerInvariant())
        {
            default:
                return Difficulty.Normal;
            case "hard":
                return Difficulty.Hard;
            case "abnormal":
                return Difficulty.Abnormal;
        }
    }

    public static DayLight DayLightToEnum(string dayLight)
    {
        switch (dayLight.ToLowerInvariant())
        {
            default:
                return DayLight.Day;
            case "dawn":
                return DayLight.Dawn;
            case "night":
                return DayLight.Night;
        }
    }

    public void OnJoinedRoom()
    {
        Shelter.OnJoinedGame();
        var room = PhotonNetwork.Room;
        Mod.Interface.Chat.System("Joined " + room.Name.RemoveColors());
        maxPlayers = room.MaxPlayers;
        playerList = string.Empty;
        gameTimesUp = false;
        Level = room.Map.LevelName;
        IN_GAME_MAIN_CAMERA.difficulty = (int) room.Difficulty;
        IN_GAME_MAIN_CAMERA.dayLight = room.DayLight;
        time = room.Time * 60;
        
        IN_GAME_MAIN_CAMERA.gamemode = room.Map.Gamemode;
        PhotonNetwork.LoadLevel(room.Map.LevelName);
        Player.Self.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { PlayerProperty.Name, Shelter.Profile.Name },
            { PlayerProperty.Guild, Shelter.Profile.Guild },
            { PlayerProperty.Kills, 0 },
            { PlayerProperty.MaxDamage, 0 },
            { PlayerProperty.TotalDamage, 0 },
            { PlayerProperty.Deaths, 0 },
            { PlayerProperty.Dead, true },
            { PlayerProperty.IsTitan, 0 },
            { PlayerProperty.RCTeam, 0 },
            { PlayerProperty.CurrentLevel, room.Map.LevelName } // was string.Empty
        });
        humanScore = 0;
        titanScore = 0;
        PVPtitanScore = 0;
        PVPhumanScore = 0;
        wave = 1;
        highestwave = 1;
        localRacingResult = string.Empty;
        needChooseSide = true;
        chatContent = new ArrayList();
        killInfoGO = new ArrayList();
        //        InRoomChat.messages = new List<string>();
        if (!PhotonNetwork.isMasterClient)
            photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
        assetCacheTextures = new Dictionary<string, Texture2D>();
        isFirstLoad = true;
        name = Shelter.Profile.Name;
        if (OnPrivateServer)
            ServerRequestAuthentication(PrivateServerAuthPass);
    }

    public void OnLeftLobby()
    {
        print("OnLeftLobby");
    }

    public void OnLeftRoom()
    {
        Shelter.OnMainMenu();

        if (Application.loadedLevel != 0)
        {
            Time.timeScale = 1f;
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }
            ResetSettings(true);
            LoadConfig();
            IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
            gameStart = false;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            inputManager.menuOn = false;
            DestroyAllExistingCloths();
            Destroy(GameObject.Find("MultiplayerManager"));
            Application.LoadLevel("menu");
        }
    }

    public void OnUpdate()
    {
        if (RCEvents.ContainsKey("OnUpdate"))
        {
            if (updateTime > 0f)
            {
                updateTime -= Time.deltaTime;
            }
            else
            {
                ((RCEvent)RCEvents["OnUpdate"]).checkEvent();
                updateTime = 1f;
            }
        }
    }

    public void OnUpdatedFriendList()
    {
        print("OnUpdatedFriendList");
    }

    private void OnLevelWasLoaded(int level1) //TODO: todo
    {
        if (level1 != 0 && Application.loadedLevelName != "characterCreation" && Application.loadedLevelName != "SnapShot")
        {
            ChangeQuality.setCurrentQuality();
            foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
            {
                if (!(obj2.GetPhotonView() != null && obj2.GetPhotonView().owner.IsMasterClient))
                {
                    Destroy(obj2);
                }
            }
            isWinning = false;
            gameStart = true;
            ShowHUDInfoCenter(string.Empty);
            GameObject obj3 = (GameObject)Instantiate(Resources.Load("MainCamera_mono"), GameObject.Find("cameraDefaultPosition").transform.position, GameObject.Find("cameraDefaultPosition").transform.rotation);
            Destroy(GameObject.Find("cameraDefaultPosition"));
            obj3.name = "MainCamera";
            Screen.lockCursor = true;
            Screen.showCursor = true;
            //TODO: Remove UI_IN_GAME
            ui = (GameObject)Instantiate(Resources.Load("UI_IN_GAME"));
            ui.name = "UI_IN_GAME";
            ui.SetActive(true);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], true);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
            LevelInfo info = LevelInfoManager.GetInfo(Level);
            Cache();
            LoadMapCustom();
            //TODO: Remove SetInterfacePosition
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetDayLight(IN_GAME_MAIN_CAMERA.dayLight);
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                single_kills = 0;
                single_maxDamage = 0;
                single_totalDamage = 0;
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
                Camera.main.GetComponent<SpectatorMovement>().disable = true;
                Camera.main.GetComponent<MouseLook>().disable = true;
                IN_GAME_MAIN_CAMERA.gamemode = info.Gamemode;
                SpawnPlayer(IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper());
                if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
                {
                    Screen.lockCursor = true;
                }
                else
                {
                    Screen.lockCursor = false;
                }
                Screen.showCursor = false;
                int abnormal = 90;
                if (difficulty == 1)
                {
                    abnormal = 70;
                }
                SpawnTitanCustom("titanRespawn", abnormal, info.EnemyNumber, false);
            }
            else
            {
                PVPcheckPoint.chkPts = new ArrayList();
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = false;
                Camera.main.GetComponent<CameraShake>().enabled = false;
                IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.MULTIPLAYER;
                if (info.Gamemode == GAMEMODE.TROST)
                {
                    GameObject.Find(PlayerRespawnTag).SetActive(false);
                    Destroy(GameObject.Find(PlayerRespawnTag));
                    GameObject.Find("rock").animation["lift"].speed = 0f;
                    GameObject.Find("door_fine").SetActive(false);
                    GameObject.Find("door_broke").SetActive(true);
                    Destroy(GameObject.Find("ppl"));
                }
                else if (info.Gamemode == GAMEMODE.BOSS_FIGHT_CT)
                {
                    GameObject.Find("playerRespawnTrost").SetActive(false);
                    Destroy(GameObject.Find("playerRespawnTrost"));
                }
                if (needChooseSide)
                {
                    Notify.New("Join the game", "Press 1 to join the game", 2500, 50f);
                }
                else if ((int)settings[245] == 0)
                {
                    if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
                    {
                        Screen.lockCursor = true;
                    }
                    else
                    {
                        Screen.lockCursor = false;
                    }
                    switch (IN_GAME_MAIN_CAMERA.gamemode)
                    {
                        case GAMEMODE.PVP_CAPTURE when Player.Self.Properties.PlayerType == PlayerType.Titan:
                            checkpoint = GameObject.Find("PVPchkPtT");
                            break;
                        case GAMEMODE.PVP_CAPTURE:
                            checkpoint = GameObject.Find("PVPchkPtH");
                            break;
                    }
                    if (Player.Self.Properties.PlayerType == PlayerType.Titan)
                    {
                        SpawnPlayerTitan(myLastHero);
                    }
                    else
                    {
                        SpawnPlayer(myLastHero);
                    }
                }
                if (info.Gamemode == GAMEMODE.BOSS_FIGHT_CT)
                {
                    Destroy(GameObject.Find("rock"));
                }
                if (PhotonNetwork.isMasterClient)
                {
                    if (info.Gamemode == GAMEMODE.TROST)
                    {
                        if (!IsAnyPlayerAlive())
                        {
                            PhotonNetwork.Instantiate("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f), Quaternion.Euler(0f, 180f, 0f), 0).GetComponent<TITAN_EREN>().rockLift = true;
                            int rate = 90;
                            if (difficulty == 1)
                            {
                                rate = 70;
                            }
                            GameObject[] objArray2 = GameObject.FindGameObjectsWithTag("titanRespawn");
                            GameObject obj4 = GameObject.Find("titanRespawnTrost");
                            if (obj4 != null)
                            {
                                foreach (GameObject obj5 in objArray2)
                                {
                                    if (obj5.transform.parent.gameObject == obj4)
                                    {
                                        SpawnTitan(rate, obj5.transform.position, obj5.transform.rotation);
                                    }
                                }
                            }
                        }
                    }
                    else if (info.Gamemode == GAMEMODE.BOSS_FIGHT_CT)
                    {
                        if (!IsAnyPlayerAlive())
                        {
                            PhotonNetwork.Instantiate("COLOSSAL_TITAN", -Vector3.up * 10000f, Quaternion.Euler(0f, 180f, 0f), 0);
                        }
                    }
                    else if (info.Gamemode == GAMEMODE.KILL_TITAN || info.Gamemode == GAMEMODE.ENDLESS_TITAN || info.Gamemode == GAMEMODE.SURVIVE_MODE)
                    {
                        if (info.Name == "Annie" || info.Name == "Annie II")
                        {
                            PhotonNetwork.Instantiate("FEMALE_TITAN", GameObject.Find("titanRespawn").transform.position, GameObject.Find("titanRespawn").transform.rotation, 0);
                        }
                        else
                        {
                            int num4 = 90;
                            if (difficulty == 1)
                            {
                                num4 = 70;
                            }
                            SpawnTitanCustom("titanRespawn", num4, info.EnemyNumber, false);
                        }
                    }
                    else if (info.Gamemode != GAMEMODE.TROST && info.Gamemode == GAMEMODE.PVP_CAPTURE && info.LevelName == "OutSide")
                    {
                        GameObject[] objArray3 = GameObject.FindGameObjectsWithTag("titanRespawn");
                        if (objArray3.Length <= 0)
                        {
                            return;
                        }
                        foreach (GameObject obj in objArray3)
                        {
                            SpawnTitanRaw(obj.transform.position, obj.transform.rotation).GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
                        }
                    }
                }
                if (!info.Supply)
                {
                    Destroy(GameObject.Find("aot_supply"));
                }
                if (!PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
                }
                if (info.IsLava)
                {
                    Instantiate(Resources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
                    GameObject.Find("aot_supply").transform.position = GameObject.Find("aot_supply_lava_position").transform.position;
                    GameObject.Find("aot_supply").transform.rotation = GameObject.Find("aot_supply_lava_position").transform.rotation;
                }
                if ((int)settings[245] == 1)
                {
                    EnterSpecMode(true);
                }
            }
        }
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!noRestart)
        {
            if (PhotonNetwork.isMasterClient)
            {
                restartingMC = true;
                if (RCSettings.infectionMode > 0)
                {
                    restartingTitan = true;
                }
                if (RCSettings.bombMode > 0)
                {
                    restartingBomb = true;
                }
                if (RCSettings.horseMode > 0)
                {
                    restartingHorse = true;
                }
                if (RCSettings.banEren == 0)
                {
                    restartingEren = true;
                }
            }
            ResetSettings(false);
            if (!LevelInfoManager.GetInfo(Level).PlayerTitansAllowed)
            {
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.IsTitan, 1 }
                };
                Player.Self.SetCustomProperties(propertiesToSet);
            }
            if (!(gameTimesUp || !PhotonNetwork.isMasterClient))
            {
                RestartGame(true);
                photonView.RPC("setMasterRC", PhotonTargets.All);
            }
        }
        noRestart = false;
    }

    public void OnPhotonCreateRoomFailed()
    {
        print("OnPhotonCreateRoomFailed");
    }

    public void OnPhotonCustomRoomPropertiesChanged()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //TODO: Ignore the player
            
            if (!PhotonNetwork.Room.IsOpen)
                PhotonNetwork.Room.IsOpen = true;
            
            if (!PhotonNetwork.Room.IsVisible)
                PhotonNetwork.Room.IsVisible = true;
            
            if (PhotonNetwork.Room.MaxPlayers != maxPlayers)
                PhotonNetwork.Room.MaxPlayers = maxPlayers;
        }
        else
        {
            maxPlayers = PhotonNetwork.Room.MaxPlayers;
        }
    }

    public void OnPhotonInstantiate()
    {
        print("OnPhotonInstantiate");
    }

    public void OnPhotonJoinRoomFailed()
    {
        print("OnPhotonJoinRoomFailed");
    }

    public void OnPhotonMaxCccuReached()
    {
        print("OnPhotonMaxCccuReached");
    }

    public void OnPhotonPlayerConnected(Player player)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView photonView1 = photonView;
            if (banHash.ContainsValue(player.Properties.Name))
            {
                KickPlayerRC(player, false, "banned.");
            }
            else
            {
                //TODO: Wasn't the limit 125 on all?
                if (player.Properties.Acceleration > 150 || player.Properties.Blade > 125 || player.Properties.Gas > 150 || player.Properties.Speed > 140)
                {
                    KickPlayerRC(player, true, "excessive stats.");
                    return;
                }
                if (RCSettings.asoPreservekdr == 1)
                {
                    StartCoroutine(WaitAndReloadKDR(player));
                }
                if (Level.StartsWith("Custom"))
                {
                    StartCoroutine(CustomLevelEnumerator(new List<Player> { player }));
                }
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                if (RCSettings.bombMode == 1)
                {
                    hashtable.Add("bomb", 1);
                }
                if (RCSettings.globalDisableMinimap == 1)
                {
                    hashtable.Add("globalDisableMinimap", 1);
                }
                if (RCSettings.teamMode > 0)
                {
                    hashtable.Add("team", RCSettings.teamMode);
                }
                if (RCSettings.pointMode > 0)
                {
                    hashtable.Add("point", RCSettings.pointMode);
                }
                if (RCSettings.disableRock > 0)
                {
                    hashtable.Add("rock", RCSettings.disableRock);
                }
                if (RCSettings.explodeMode > 0)
                {
                    hashtable.Add("explode", RCSettings.explodeMode);
                }
                if (RCSettings.healthMode > 0)
                {
                    hashtable.Add("healthMode", RCSettings.healthMode);
                    hashtable.Add("healthLower", RCSettings.healthLower);
                    hashtable.Add("healthUpper", RCSettings.healthUpper);
                }
                if (RCSettings.infectionMode > 0)
                {
                    hashtable.Add("infection", RCSettings.infectionMode);
                }
                if (RCSettings.banEren == 1)
                {
                    hashtable.Add("eren", RCSettings.banEren);
                }
                if (RCSettings.moreTitans > 0)
                {
                    hashtable.Add("titanc", RCSettings.moreTitans);
                }
                if (RCSettings.damageMode > 0)
                {
                    hashtable.Add("damage", RCSettings.damageMode);
                }
                if (RCSettings.sizeMode > 0)
                {
                    hashtable.Add("sizeMode", RCSettings.sizeMode);
                    hashtable.Add("sizeLower", RCSettings.sizeLower);
                    hashtable.Add("sizeUpper", RCSettings.sizeUpper);
                }
                if (RCSettings.spawnMode > 0)
                {
                    hashtable.Add("spawnMode", RCSettings.spawnMode);
                    hashtable.Add("nRate", RCSettings.nRate);
                    hashtable.Add("aRate", RCSettings.aRate);
                    hashtable.Add("jRate", RCSettings.jRate);
                    hashtable.Add("cRate", RCSettings.cRate);
                    hashtable.Add("pRate", RCSettings.pRate);
                }
                if (RCSettings.waveModeOn > 0)
                {
                    hashtable.Add("waveModeOn", 1);
                    hashtable.Add("waveModeNum", RCSettings.waveModeNum);
                }
                if (RCSettings.friendlyMode > 0)
                {
                    hashtable.Add("friendly", 1);
                }
                if (RCSettings.pvpMode > 0)
                {
                    hashtable.Add("pvp", RCSettings.pvpMode);
                }
                if (RCSettings.maxWave > 0)
                {
                    hashtable.Add("maxwave", RCSettings.maxWave);
                }
                if (RCSettings.endlessMode > 0)
                {
                    hashtable.Add("endless", RCSettings.endlessMode);
                }
                if (RCSettings.motd != string.Empty)
                {
                    hashtable.Add("motd", RCSettings.motd);
                }
                if (RCSettings.horseMode > 0)
                {
                    hashtable.Add("horse", RCSettings.horseMode);
                }
                if (RCSettings.ahssReload > 0)
                {
                    hashtable.Add("ahssReload", RCSettings.ahssReload);
                }
                if (RCSettings.punkWaves > 0)
                {
                    hashtable.Add("punkWaves", RCSettings.punkWaves);
                }
                if (RCSettings.deadlyCannons > 0)
                {
                    hashtable.Add("deadlycannons", RCSettings.deadlyCannons);
                }
                if (RCSettings.racingStatic > 0)
                {
                    hashtable.Add("asoracing", RCSettings.racingStatic);
                }
                if (ignoreList != null && ignoreList.Count > 0)
                {
                    photonView1.RPC("ignorePlayerArray", player, ignoreList.ToArray());
                }
                photonView1.RPC("settingRPC", player, hashtable);
                photonView1.RPC("setMasterRC", player);
                if (Time.timeScale <= 0.1f && pauseWaitTime > 3f)
                {
                    photonView1.RPC("pauseRPC", player, true);
                    Mod.Interface.Chat.SendMessage("<color=#FFCC00>MasterClient has paused the game.</color>");
                }
            }
        }
    }

    public void OnPhotonPlayerDisconnected(Player player)
    {
        if (!gameTimesUp)
        {
            OneTitanDown(string.Empty, true);
            SomeOneIsDead(0);
        }
        if (ignoreList.Contains(player.ID))
        {
            ignoreList.Remove(player.ID);
        }
        InstantiateTracker.instance.TryRemovePlayer(player.ID);
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("verifyPlayerHasLeft", PhotonTargets.All, player.ID);
        }
        if (RCSettings.asoPreservekdr == 1)
        {
            string key = player.Properties.Name;
            if (PreservedPlayerKDR.ContainsKey(key))
            {
                PreservedPlayerKDR.Remove(key);
            }
            int[] numArray2 =
            {
                player.Properties.Kills, 
                player.Properties.Deaths, 
                player.Properties.MaxDamage, 
                player.Properties.TotalDamage
            };
            PreservedPlayerKDR.Add(key, numArray2);
        }
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        if (playerAndUpdatedProps != null && playerAndUpdatedProps.Length >= 2 && (Player)playerAndUpdatedProps[0] == Player.Self)
        {
            Hashtable hashtable = (Hashtable) playerAndUpdatedProps[1];

            if (hashtable.ContainsKey(PlayerProperty.Name) && Shelter.Profile.Name != (string) hashtable[PlayerProperty.Name])
            {
                Player.Self.SetCustomProperties(new Hashtable
                {
                    { PlayerProperty.Name, Shelter.Profile.Name }
                });
            }
            if (hashtable.ContainsKey("statACL") || hashtable.ContainsKey("statBLA") || hashtable.ContainsKey("statGAS") || hashtable.ContainsKey("statSPD"))
            {
                Player player = Player.Self;
                int num = player.Properties.Acceleration;
                int num2 = player.Properties.Blade;
                int num3 = player.Properties.Gas;
                int num4 = player.Properties.Speed;
                if (num > 150)
                {
                    Player.Self.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Acceleration, 100 }
                    });
                }
                if (num2 > 125)
                {
                    Player.Self.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Blade, 100 }
                    });
                }
                if (num3 > 150)
                {
                    Player.Self.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Gas, 100 }
                    });
                }
                if (num4 > 140)
                {
                    Player.Self.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Speed, 100 }
                    });
                }
            }
        }
    }

    public void OnPhotonRandomJoinFailed()
    {
        print("OnPhotonRandomJoinFailed");
    }

    public void OnPhotonSerializeView()
    {
        print("OnPhotonSerializeView");
    }

    public void OnReceivedRoomListUpdate()
    {
    }

    private void Update()
    {
        //        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE && GameObject.Find("LabelNetworkStatus") != null)
        //        {
        //            GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>().text = PhotonNetwork.connectionStatesDetailed.ToString();
        //            if (PhotonNetwork.connected)
        //            {
        //                UILabel component = GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>();
        //                component.text = component.text + " ping:" + PhotonNetwork.GetPing();
        //            }
        //        }
        if (gameStart)
        {
            IEnumerator enumerator = heroes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = (HERO)enumerator.Current;
                    if (current != null)
                        current.update2();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            enumerator = hooks.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = (Bullet)enumerator.Current;
                    if (current != null)
                        current.update();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            if (mainCamera != null)
            {
                mainCamera.snapShotUpdate();
            }
            enumerator = eT.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var titanEren = (TITAN_EREN)enumerator.Current;
                    if (titanEren != null)
                        titanEren.update();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            enumerator = titans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = (TITAN)enumerator.Current;
                    if (current != null)
                        current.update2();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            enumerator = fT.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var femaleTitan = (FEMALE_TITAN)enumerator.Current;
                    if (femaleTitan != null)
                        femaleTitan.update();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            enumerator = cT.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var colossalTitan = (COLOSSAL_TITAN)enumerator.Current;
                    if (colossalTitan != null)
                        colossalTitan.update2();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            if (mainCamera != null)
            {
                mainCamera.update2();
            }
        }
    }

    private void Start()
    {
        instance = this;
        gameObject.name = "MultiplayerManager";
        HeroCostume.init2();
        CharacterMaterials.init();
        DontDestroyOnLoad(gameObject);
        heroes = new ArrayList();
        eT = new ArrayList();
        titans = new ArrayList();
        fT = new ArrayList();
        cT = new ArrayList();
        hooks = new ArrayList();
        name = string.Empty;
        if (nameField == null)
        {
            nameField = "Hawk of the Death";
//            nameField = "[ff00ca]S[eb02ba]h[d703ab]e[c2059b]l[ae068c]t[9a087c]e[7b2396]r[5c3eb0] [3e59cb]M[1f74e5]o[008fff]d - #" + new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz0123456789", 10).Select(x => x[UnityEngine.Random.Range(0, x.Length)]).ToArray());
        }
        if (privateServerField == null)
        {
            privateServerField = string.Empty;
        }
        usernameField = string.Empty;
        passwordField = string.Empty;
        ResetGameSettings();
        banHash = new ExitGames.Client.Photon.Hashtable();
        imatitan = new ExitGames.Client.Photon.Hashtable();
        oldScript = string.Empty;
        currentLevel = string.Empty;
        if (currentScript == null)
        {
            currentScript = string.Empty;
        }
        titanSpawns = new List<Vector3>();
        playerSpawnsC = new List<Vector3>();
        playerSpawnsM = new List<Vector3>();
        playersRPC = new List<Player>();
        levelCache = new List<string[]>();
        titanSpawners = new List<TitanSpawner>();
        restartCount = new List<float>();
        ignoreList = new List<int>();
        groundList = new List<GameObject>();
        noRestart = false;
        masterRC = false;
        isSpawning = false;
        intVariables = new ExitGames.Client.Photon.Hashtable();
        heroHash = new ExitGames.Client.Photon.Hashtable();
        boolVariables = new ExitGames.Client.Photon.Hashtable();
        stringVariables = new ExitGames.Client.Photon.Hashtable();
        floatVariables = new ExitGames.Client.Photon.Hashtable();
        globalVariables = new ExitGames.Client.Photon.Hashtable();
        RCRegions = new ExitGames.Client.Photon.Hashtable();
        RCEvents = new ExitGames.Client.Photon.Hashtable();
        RCVariableNames = new ExitGames.Client.Photon.Hashtable();
        RCRegionTriggers = new ExitGames.Client.Photon.Hashtable();
        playerVariables = new ExitGames.Client.Photon.Hashtable();
        titanVariables = new ExitGames.Client.Photon.Hashtable();
        logicLoaded = false;
        customLevelLoaded = false;
        oldScriptLogic = string.Empty;
        currentScriptLogic = string.Empty;
        retryTime = 0f;
        playerList = string.Empty;
        updateTime = 0f;
        if (textureBackgroundBlack == null)
        {
            textureBackgroundBlack = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            textureBackgroundBlack.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
            textureBackgroundBlack.Apply();
        }
        if (textureBackgroundBlue == null)
        {
            textureBackgroundBlue = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            textureBackgroundBlue.SetPixel(0, 0, new Color(0.08f, 0.3f, 0.4f, 1f));
            textureBackgroundBlue.Apply();
        }
        LoadConfig();
        ChangeQuality.setCurrentQuality();

        DestroyOldMenu();
    }

    private void LateUpdate()
    {
        if (gameStart)
        {
            IEnumerator enumerator = heroes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = (HERO)enumerator.Current;
                    if (current != null)
                        current.lateUpdate2();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            IEnumerator enumerator2 = eT.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    var titanEren = (TITAN_EREN)enumerator2.Current;
                    if (titanEren != null)
                        titanEren.lateUpdate();
                }
            }
            finally
            {
                if (enumerator2 is IDisposable disposable2)
                {
                    disposable2.Dispose();
                }
            }
            IEnumerator enumerator3 = titans.GetEnumerator();
            try
            {
                while (enumerator3.MoveNext())
                {
                    var current = (TITAN)enumerator3.Current;
                    if (current != null)
                        current.lateUpdate2();
                }
            }
            finally
            {
                if (enumerator3 is IDisposable disposable3)
                    disposable3.Dispose();
            }
            IEnumerator enumerator4 = fT.GetEnumerator();
            try
            {
                while (enumerator4.MoveNext())
                {
                    var femaleTitan = (FEMALE_TITAN)enumerator4.Current;
                    if (femaleTitan != null)
                        femaleTitan.lateUpdate2();
                }
            }
            finally
            {
                if (enumerator4 is IDisposable disposable4)
                    disposable4.Dispose();
            }
            Core();
        }
    }

    #endregion

    // ReSharper restore UnusedMember.Local

    public void AddCamera(IN_GAME_MAIN_CAMERA c)
    {
        mainCamera = c;
    }

    public void AddColossalTitan(COLOSSAL_TITAN titan)
    {
        cT.Add(titan);
    }

    public void AddTitanEren(TITAN_EREN hero)
    {
        eT.Add(hero);
    }

    public void AddFemaleTitan(FEMALE_TITAN titan)
    {
        fT.Add(titan);
    }

    public void AddHero(HERO hero)
    {
        heroes.Add(hero);
    }

    public void AddHook(Bullet h)
    {
        hooks.Add(h);
    }

    public void AddTime(float t)
    {
        timeTotalServer -= t;
    }

    public void AddTitan(TITAN titan)
    {
        titans.Add(titan);
    }

    private void Cache()
    {
        ClothFactory.ClearClothCache();
        inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
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
        Camera.main.farClipPlane = 1500f;
        pauseWaitTime = 0f;
        spectateSprites = new List<GameObject>();
        isRestarting = false;
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(WaitAndResetRestarts());
        }
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
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
                    SetGameSettings(CheckGameGUI());
                }
                if (RCSettings.endlessMode > 0)
                {
//                    StartCoroutine(RespawnEnumerator(RCSettings.endlessMode)); TODO: Check what the actual fuck is this and make it better
                }
            }
            if ((int) settings[244] == 1)
            {
                Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round Start.");
            }
        }
        isFirstLoad = false;
    }

    private ExitGames.Client.Photon.Hashtable CheckGameGUI() //TODO: Remove? If it's related to gui
    {
        int num;
        int num2;
        Player player;
        int num4;
        float num8;
        float num9;
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        if ((int) settings[200] > 0)
        {
            settings[192] = 0;
            settings[193] = 0;
            settings[226] = 0;
            settings[220] = 0;
            if (!int.TryParse((string) settings[201], out num) || num > PhotonNetwork.countOfPlayers || num < 0)
            {
                settings[201] = "1";
            }
            hashtable.Add("infection", num);
            if (RCSettings.infectionMode != num)
            {
                imatitan.Clear();
                for (num2 = 0; num2 < PhotonNetwork.playerList.Length; num2++)
                {
                    player = PhotonNetwork.playerList[num2];
                    ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                    {
                        { PlayerProperty.IsTitan, 1 }
                    };
                    player.SetCustomProperties(propertiesToSet);
                }
                int length = PhotonNetwork.playerList.Length;
                num4 = num;
                for (num2 = 0; num2 < PhotonNetwork.playerList.Length; num2++)
                {
                    Player player2 = PhotonNetwork.playerList[num2];
                    if (length > 0 && UnityEngine.Random.Range(0f, 1f) <= num4 / (float)length)
                    {
                        ExitGames.Client.Photon.Hashtable hashtable3 = new ExitGames.Client.Photon.Hashtable
                        {
                            { PlayerProperty.IsTitan, 2 }
                        };
                        player2.SetCustomProperties(hashtable3);
                        imatitan.Add(player2.ID, 2);
                        num4--;
                    }
                    length--;
                }
            }
        }
        if ((int) settings[192] > 0)
        {
            hashtable.Add("bomb", (int) settings[192]);
        }
        if ((int) settings[235] > 0)
        {
            hashtable.Add("globalDisableMinimap", (int) settings[235]);
        }
        if ((int) settings[193] > 0)
        {
            hashtable.Add("team", (int) settings[193]);
            if (RCSettings.teamMode != (int) settings[193])
            {
                num4 = 1;
                for (num2 = 0; num2 < PhotonNetwork.playerList.Length; num2++)
                {
                    player = PhotonNetwork.playerList[num2];
                    switch (num4)
                    {
                        case 1:
                            photonView.RPC("setTeamRPC", player, 1);
                            num4 = 2;
                            break;

                        case 2:
                            photonView.RPC("setTeamRPC", player, 2);
                            num4 = 1;
                            break;
                    }
                }
            }
        }
        if ((int) settings[226] > 0)
        {
            if (!int.TryParse((string) settings[227], out num) || num > 1000 || num < 0)
            {
                settings[227] = "50";
            }
            hashtable.Add("point", num);
        }
        if ((int) settings[194] > 0)
        {
            hashtable.Add("rock", (int) settings[194]);
        }
        if ((int) settings[195] > 0)
        {
            if (!int.TryParse((string) settings[196], out num) || num > 100 || num < 0)
            {
                settings[196] = "30";
            }
            hashtable.Add("explode", num);
        }
        if ((int) settings[197] > 0)
        {
            if (!int.TryParse((string) settings[198], out var result) || result > 100000 || result < 0)
            {
                settings[198] = "100";
            }
            if (!int.TryParse((string) settings[199], out var num7) || num7 > 100000 || num7 < 0)
            {
                settings[199] = "200";
            }
            hashtable.Add("healthMode", (int) settings[197]);
            hashtable.Add("healthLower", result);
            hashtable.Add("healthUpper", num7);
        }
        if ((int) settings[202] > 0)
        {
            hashtable.Add("eren", (int) settings[202]);
        }
        if ((int) settings[203] > 0)
        {
            if (!int.TryParse((string) settings[204], out num) || num > 50 || num < 0)
            {
                settings[204] = "1";
            }
            hashtable.Add("titanc", num);
        }
        if ((int) settings[205] > 0)
        {
            if (!int.TryParse((string) settings[206], out num) || num > 100000 || num < 0)
            {
                settings[206] = "1000";
            }
            hashtable.Add("damage", num);
        }
        if ((int) settings[207] > 0)
        {
            if (!float.TryParse((string) settings[208], out num8) || num8 > 100f || num8 < 0f)
            {
                settings[208] = "1.0";
            }
            if (!float.TryParse((string) settings[209], out num9) || num9 > 100f || num9 < 0f)
            {
                settings[209] = "3.0";
            }
            hashtable.Add("sizeMode", (int) settings[207]);
            hashtable.Add("sizeLower", num8);
            hashtable.Add("sizeUpper", num9);
        }
        if ((int) settings[210] > 0)
        {
            if (!(float.TryParse((string) settings[211], out num8) && num8 >= 0f))
            {
                settings[211] = "20.0";
            }
            if (!(float.TryParse((string) settings[212], out num9) && num9 >= 0f))
            {
                settings[212] = "20.0";
            }
            if (!float.TryParse((string)settings[213], out _))
            {
                settings[213] = "20.0";
            }
            if (!(float.TryParse((string) settings[214], out var num11) && num11 >= 0f))
            {
                settings[214] = "20.0";
            }
            if (!(float.TryParse((string) settings[215], out var num12) && num12 >= 0f))
            {
                settings[215] = "20.0";
            }
            if (num8 + num9 + 20f + num11 + num12 > 100f)
            {
                settings[211] = "20.0";
                settings[212] = "20.0";
                settings[213] = "20.0";
                settings[214] = "20.0";
                settings[215] = "20.0";
                num8 = 20f;
                num9 = 20f;
                num11 = 20f;
                num12 = 20f;
            }
            hashtable.Add("spawnMode", (int) settings[210]);
            hashtable.Add("nRate", num8);
            hashtable.Add("aRate", num9);
            hashtable.Add("jRate", 20f);
            hashtable.Add("cRate", num11);
            hashtable.Add("pRate", num12);
        }
        if ((int) settings[216] > 0)
        {
            hashtable.Add("horse", (int) settings[216]);
        }
        if ((int) settings[217] > 0)
        {
            if (!(int.TryParse((string) settings[218], out num) && num <= 50))
            {
                settings[218] = "1";
            }
            hashtable.Add("waveModeOn", (int) settings[217]);
            hashtable.Add("waveModeNum", num);
        }
        if ((int) settings[219] > 0)
        {
            hashtable.Add("friendly", (int) settings[219]);
        }
        if ((int) settings[220] > 0)
        {
            hashtable.Add("pvp", (int) settings[220]);
        }
        if ((int) settings[221] > 0)
        {
            if (!int.TryParse((string) settings[222], out num) || num > 1000000 || num < 0)
            {
                settings[222] = "20";
            }
            hashtable.Add("maxwave", num);
        }
        if ((int) settings[223] > 0)
        {
            if (!int.TryParse((string) settings[224], out num) || num > 1000000 || num < 5)
            {
                settings[224] = "5";
            }
            hashtable.Add("endless", num);
        }
        if ((string) settings[225] != string.Empty)
        {
            hashtable.Add("motd", (string) settings[225]);
        }
        if ((int) settings[228] > 0)
        {
            hashtable.Add("ahssReload", (int) settings[228]);
        }
        if ((int) settings[229] > 0)
        {
            hashtable.Add("punkWaves", (int) settings[229]);
        }
        if ((int) settings[261] > 0)
        {
            hashtable.Add("deadlycannons", (int) settings[261]);
        }
        if (RCSettings.racingStatic > 0)
        {
            hashtable.Add("asoracing", 1);
        }
        return hashtable;
    }

    private bool IsAnyTitanAlive()
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (obj2.GetComponent<TITAN>() != null && !obj2.GetComponent<TITAN>().hasDie)
            {
                return false;
            }
            if (obj2.GetComponent<FEMALE_TITAN>() != null)
            {
                return false;
            }
        }
        return true;
    }

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
        string key = skybox[6];
        bool mipmap = true;
        bool iteratorVariable2 = false;
        if ((int)settings[63] == 1)
        {
            mipmap = false;
        }
        if (skybox[0] != string.Empty || skybox[1] != string.Empty || skybox[2] != string.Empty || skybox[3] != string.Empty || skybox[4] != string.Empty || skybox[5] != string.Empty)
        {
            string iteratorVariable3 = string.Join(",", skybox);
            if (!linkHash[1].ContainsKey(iteratorVariable3))
            {
                iteratorVariable2 = true;
                Material material = Camera.main.GetComponent<Skybox>().material;
                string url = skybox[0];
                string iteratorVariable6 = skybox[1];
                string iteratorVariable7 = skybox[2];
                string iteratorVariable8 = skybox[3];
                string iteratorVariable9 = skybox[4];
                string iteratorVariable10 = skybox[5];
                if (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg")) //TODO: Check for validity of ip
                {
                    WWW link = new WWW(url);
                    yield return link;
                    Texture2D texture = RCextensions.loadimage(link, mipmap, 500000);
                    link.Dispose();
                    material.SetTexture("_FrontTex", texture);
                }
                if (iteratorVariable6.EndsWith(".jpg") || iteratorVariable6.EndsWith(".png") || iteratorVariable6.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable13 = new WWW(iteratorVariable6);
                    yield return iteratorVariable13;
                    Texture2D iteratorVariable14 = RCextensions.loadimage(iteratorVariable13, mipmap, 500000);
                    iteratorVariable13.Dispose();
                    material.SetTexture("_BackTex", iteratorVariable14);
                }
                if (iteratorVariable7.EndsWith(".jpg") || iteratorVariable7.EndsWith(".png") || iteratorVariable7.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable15 = new WWW(iteratorVariable7);
                    yield return iteratorVariable15;
                    Texture2D iteratorVariable16 = RCextensions.loadimage(iteratorVariable15, mipmap, 500000);
                    iteratorVariable15.Dispose();
                    material.SetTexture("_LeftTex", iteratorVariable16);
                }
                if (iteratorVariable8.EndsWith(".jpg") || iteratorVariable8.EndsWith(".png") || iteratorVariable8.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable17 = new WWW(iteratorVariable8);
                    yield return iteratorVariable17;
                    Texture2D iteratorVariable18 = RCextensions.loadimage(iteratorVariable17, mipmap, 500000);
                    iteratorVariable17.Dispose();
                    material.SetTexture("_RightTex", iteratorVariable18);
                }
                if (iteratorVariable9.EndsWith(".jpg") || iteratorVariable9.EndsWith(".png") || iteratorVariable9.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable19 = new WWW(iteratorVariable9);
                    yield return iteratorVariable19;
                    Texture2D iteratorVariable20 = RCextensions.loadimage(iteratorVariable19, mipmap, 500000);
                    iteratorVariable19.Dispose();
                    material.SetTexture("_UpTex", iteratorVariable20);
                }
                if (iteratorVariable10.EndsWith(".jpg") || iteratorVariable10.EndsWith(".png") || iteratorVariable10.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable21 = new WWW(iteratorVariable10);
                    yield return iteratorVariable21;
                    Texture2D iteratorVariable22 = RCextensions.loadimage(iteratorVariable21, mipmap, 500000);
                    iteratorVariable21.Dispose();
                    material.SetTexture("_DownTex", iteratorVariable22);
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
        if (key.EndsWith(".jpg") || key.EndsWith(".png") || key.EndsWith(".jpeg"))
        {
            foreach (GameObject iteratorVariable23 in groundList)
            {
                if (iteratorVariable23 != null && iteratorVariable23.renderer != null)
                {
                    foreach (Renderer iteratorVariable24 in iteratorVariable23.GetComponentsInChildren<Renderer>())
                    {
                        if (!linkHash[0].ContainsKey(key))
                        {
                            WWW iteratorVariable25 = new WWW(key);
                            yield return iteratorVariable25;
                            Texture2D iteratorVariable26 = RCextensions.loadimage(iteratorVariable25, mipmap, 200000);
                            iteratorVariable25.Dispose();
                            if (!linkHash[0].ContainsKey(key))
                            {
                                iteratorVariable2 = true;
                                iteratorVariable24.material.mainTexture = iteratorVariable26;
                                linkHash[0].Add(key, iteratorVariable24.material);
                                iteratorVariable24.material = (Material)linkHash[0][key];
                            }
                            else
                            {
                                iteratorVariable24.material = (Material)linkHash[0][key];
                            }
                        }
                        else
                        {
                            iteratorVariable24.material = (Material)linkHash[0][key];
                        }
                    }
                }
            }
        }
        else if (key.ToLower() == "transparent")
        {
            foreach (GameObject obj2 in groundList)
            {
                if (obj2 != null && obj2.renderer != null)
                {
                    foreach (Renderer renderer1 in obj2.GetComponentsInChildren<Renderer>())
                    {
                        renderer1.enabled = false;
                    }
                }
            }
        }
        if (iteratorVariable2)
        {
            UnloadAssets();
        }
    }

    public void CompileScript(string str)
    {
        int num3;
        string[] strArray2 = str.Replace(" ", string.Empty).Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
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
    
    private void Core()
    {
        if ((int) settings[64] >= 100)
        {
            CoreEditor();
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE && needChooseSide)
            {
                if (inputManager.isInputDown[InputCode.flare1])
                {
                    if (NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3])) //TODO: Remove UI_IN_GAME
                    {
                        Screen.lockCursor = true;
                        Screen.showCursor = true;
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], true);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
                        Camera.main.GetComponent<SpectatorMovement>().disable = false;
                        Camera.main.GetComponent<MouseLook>().disable = false;
                    }
                    else
                    {
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
                        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], true);
                        Camera.main.GetComponent<SpectatorMovement>().disable = true;
                        Camera.main.GetComponent<MouseLook>().disable = true;
                    }
                }
                if (inputManager.isInputDown[15] && !inputManager.menuOn)
                {
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                    Camera.main.GetComponent<SpectatorMovement>().disable = true;
                    Camera.main.GetComponent<MouseLook>().disable = true;
                    inputManager.menuOn = true;
                }
            }
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
            {
                int length;
                switch (IN_GAME_MAIN_CAMERA.gametype)
                {
                    case GAMETYPE.MULTIPLAYER:
                        CoreAdd();
//                    this.ShowHUDInfoTopLeft(this.playerList); Custom scoreboard
                        if (Camera.main != null && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.RACING && Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver && !needChooseSide && (int) settings[245] == 0)
                        {
                            ShowHUDInfoCenter("Press [F7D358]" + inputManager.inputString[InputCode.flare1] + "[-] to spectate the next player. \nPress [F7D358]" + inputManager.inputString[InputCode.flare2] + "[-] to spectate the previous player.\nPress [F7D358]" + inputManager.inputString[InputCode.attack1] + "[-] to enter the spectator mode.\n\n\n\n");
                            if (LevelInfoManager.GetInfo(Level).RespawnMode == RespawnMode.DEATHMATCH || RCSettings.endlessMode > 0 || !(RCSettings.bombMode == 1 || RCSettings.pvpMode > 0 ? RCSettings.pointMode <= 0 : true))
                            {
                                myRespawnTime += Time.deltaTime;
                                int endlessMode = 5;
                                if (Player.Self.Properties.PlayerType == PlayerType.Titan)
                                {
                                    endlessMode = 10;
                                }
                                if (RCSettings.endlessMode > 0)
                                {
                                    endlessMode = RCSettings.endlessMode;
                                }
                                length = endlessMode - (int)myRespawnTime;
                                ShowHUDInfoCenterADD("Respawn in " + length + "s.");
                                if (myRespawnTime > endlessMode)
                                {
                                    myRespawnTime = 0f;
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
                                    ShowHUDInfoCenter(string.Empty);
                                }
                            }
                        }
                        break;
                    case GAMETYPE.SINGLE:
                        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
                        {
                            if (!isLosing)
                            {
                                currentSpeed = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity.magnitude;
                                maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
                                ShowHUDInfoTopLeft(string.Concat("Current Speed : ", (int)currentSpeed, "\nMax Speed:", maxSpeed));
                            }
                        }
                        else
                        {
                            ShowHUDInfoTopLeft(string.Concat("Kills:", single_kills, "\nMax Damage:", single_maxDamage, "\nTotal Damage:", single_totalDamage));
                        }
                        break;
                }
                if (isLosing && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.RACING)
                {
                    switch (IN_GAME_MAIN_CAMERA.gametype)
                    {
                        case GAMETYPE.SINGLE when IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE:
                            ShowHUDInfoCenter(string.Concat("Survive ", wave, " Waves!\n Press ", inputManager.inputString[InputCode.restart], " to Restart.\n\n\n"));
                            break;
                        case GAMETYPE.SINGLE:
                            ShowHUDInfoCenter("Humanity Fail!\n Press " + inputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                            break;
                        default:
                            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                            {
                                ShowHUDInfoCenter(string.Concat("Survive ", wave, " Waves!\nGame Restart in ", (int)gameEndCD, "s\n\n"));
                            }
                            else
                            {
                                ShowHUDInfoCenter("Humanity Fail!\nAgain!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                            }
                            if (gameEndCD <= 0f)
                            {
                                gameEndCD = 0f;
                                if (PhotonNetwork.isMasterClient)
                                {
                                    RestartGameRC();
                                }
                                ShowHUDInfoCenter(string.Empty);
                            }
                            else
                            {
                                gameEndCD -= Time.deltaTime;
                            }

                            break;
                    }
                }
                if (isWinning)
                {
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        switch (IN_GAME_MAIN_CAMERA.gamemode)
                        {
                            case GAMEMODE.RACING:
                                var num3 = (int) (timeTotalServer * 10f) * 0.1f - 5f;
                                ShowHUDInfoCenter(num3 + "s !\n Press " + inputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                                break;
                            case GAMEMODE.SURVIVE_MODE:
                                ShowHUDInfoCenter("Survive All Waves!\n Press " + inputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                                break;
                            default:
                                ShowHUDInfoCenter("Humanity Win!\n Press " + inputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                                break;
                        }
                    }
                    else
                    {
                        switch (IN_GAME_MAIN_CAMERA.gamemode)
                        {
                            case GAMEMODE.RACING:
                                ShowHUDInfoCenter(string.Concat(localRacingResult, "\n\nGame Restart in ", (int)gameEndCD, "s"));
                                break;
                            case GAMEMODE.SURVIVE_MODE:
                                ShowHUDInfoCenter("Survive All Waves!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                                break;
                            case GAMEMODE.PVP_AHSS:
                                if (RCSettings.pvpMode == 0 && RCSettings.bombMode == 0)
                                {
                                    ShowHUDInfoCenter(string.Concat("Team ", teamWinner, " Win!\nGame Restart in ", (int)gameEndCD, "s\n\n"));
                                }
                                else
                                {
                                    ShowHUDInfoCenter(string.Concat("Round Ended!\nGame Restart in ", (int)gameEndCD, "s\n\n"));
                                }
                                break;
                            default:
                                ShowHUDInfoCenter("Humanity Win!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                                break;
                        }
                        if (gameEndCD <= 0f)
                        {
                            gameEndCD = 0f;
                            if (PhotonNetwork.isMasterClient)
                            {
                                RestartGameRC();
                            }
                            ShowHUDInfoCenter(string.Empty);
                        }
                        else
                        {
                            gameEndCD -= Time.deltaTime;
                        }
                    }
                }
                timeElapse += Time.deltaTime;
                roundTime += Time.deltaTime;
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                {
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
                    {
                        if (!isWinning)
                        {
                            timeTotalServer += Time.deltaTime;
                        }
                    }
                    else if (!(isLosing || isWinning))
                    {
                        timeTotalServer += Time.deltaTime;
                    }
                }
                else
                {
                    timeTotalServer += Time.deltaTime;
                }
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
                {
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        if (!isWinning)
                        {
                            ShowHUDInfoTopCenter("Time : " + ((int) (timeTotalServer * 10f) * 0.1f - 5f));
                        }
                        if (timeTotalServer < 5f)
                        {
                            ShowHUDInfoCenter("RACE START IN " + (int) (5f - timeTotalServer));
                        }
                        else if (!startRacing)
                        {
                            ShowHUDInfoCenter(string.Empty);
                            startRacing = true;
                            endRacing = false;
                            GameObject.Find("door").SetActive(false);
                        }
                    }
                    else
                    {
                        ShowHUDInfoTopCenter("Time : " + (roundTime >= 20f ? ((int) (roundTime * 10f) * 0.1f - 20f).ToString(CultureInfo.InvariantCulture) : "WAITING"));
                        if (roundTime < 20f)
                        {
                            ShowHUDInfoCenter("RACE START IN " + (int) (20f - roundTime) + (localRacingResult != string.Empty ? "\nLast Round\n" + localRacingResult : "\n\n"));
                        }
                        else if (!startRacing)
                        {
                            ShowHUDInfoCenter(string.Empty);
                            startRacing = true;
                            endRacing = false;
                            GameObject obj2 = GameObject.Find("door");
                            if (obj2 != null)
                            {
                                obj2.SetActive(false);
                            }
                            if (racingDoors != null && customLevelLoaded)
                            {
                                foreach (GameObject obj3 in racingDoors)
                                {
                                    obj3.SetActive(false);
                                }
                                racingDoors = null;
                            }
                        }
                        else if (racingDoors != null && customLevelLoaded)
                        {
                            foreach (GameObject obj3 in racingDoors)
                            {
                                obj3.SetActive(false);
                            }
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
                            ShowHUDInfoCenter(string.Empty);
                        }
                    }
                }
                if (timeElapse > 1f)
                {
                    timeElapse--;
                    string content = string.Empty;
                    switch (IN_GAME_MAIN_CAMERA.gamemode)
                    {
                        case GAMEMODE.ENDLESS_TITAN:
                            length = time - (int)timeTotalServer;
                            content = content + "Time : " + length;
                            break;
                        case GAMEMODE.KILL_TITAN:
                        case GAMEMODE.None:
                            content = "Titan Left: ";
                            length = GameObject.FindGameObjectsWithTag("titan").Length;
                            content = content + length + "  Time : ";
                            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                            {
                                length = (int)timeTotalServer;
                                content = content + length;
                            }
                            else
                            {
                                length = time - (int)timeTotalServer;
                                content = content + length;
                            }
                            break;
                        case GAMEMODE.SURVIVE_MODE:
                            content = "Titan Left: ";
                            object[] objArray = new object[4];
                            objArray[0] = content;
                            length = GameObject.FindGameObjectsWithTag("titan").Length;
                            objArray[1] = length;
                            objArray[2] = " Wave : ";
                            objArray[3] = wave;
                            content = string.Concat(objArray);
                            break;
                        case GAMEMODE.BOSS_FIGHT_CT:
                            content = "Time : ";
                            length = time - (int)timeTotalServer;
                            content = content + length + "\nDefeat the Colossal Titan.\nPrevent abnormal titan from running to the north gate";
                            break;
                        case GAMEMODE.PVP_CAPTURE:
                            string str2 = PVPcheckPoint.chkPts.Cast<PVPcheckPoint>().Aggregate("| ", (current, t) => current + t.getStateString() + " ");
                            str2 = str2 + "|";
                            length = time - (int)timeTotalServer;
                            content = string.Concat(PVPtitanScoreMax - PVPtitanScore, "  ", str2, "  ", PVPhumanScoreMax - PVPhumanScore, "\n") + "Time : " + length;
                            break;
                    }
                    if (RCSettings.teamMode > 0)
                    {
                        content = content + "\n[00FFFF]Cyan:" + Convert.ToString(cyanKills) + "       [FF00FF]Magenta:" + Convert.ToString(magentaKills) + "[ffffff]";
                    }
                    ShowHUDInfoTopCenter(content);
                    content = string.Empty;
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                        {
                            content = "Time : ";
                            length = (int)timeTotalServer;
                            content = content + length;
                        }
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
                    {
                        content = string.Concat("Humanity ", humanScore, " : Titan ", titanScore, " ");
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
                    {
                        content = string.Concat("Humanity ", humanScore, " : Titan ", titanScore, " ");
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.CAGE_FIGHT)
                    {
                        switch (IN_GAME_MAIN_CAMERA.gamemode)
                        {
                            case GAMEMODE.SURVIVE_MODE:
                                content = "Time : ";
                                length = time - (int)timeTotalServer;
                                content = content + length;
                                break;
                            case GAMEMODE.PVP_AHSS:
                                for (int j = 0; j < teamScores.Length; j++)
                                {
                                    string str3 = content;
                                    content = string.Concat(str3, j == 0 ? string.Empty : " : ", "Team", j + 1, " ", teamScores[j], string.Empty);
                                }
                                content = content + "\nTime : " + (time - (int)timeTotalServer);
                                break;
                        }
                    }
                    ShowHUDInfoTopRight(content);
                    string str4 = IN_GAME_MAIN_CAMERA.difficulty >= 0 ? (IN_GAME_MAIN_CAMERA.difficulty != 0 ? (IN_GAME_MAIN_CAMERA.difficulty != 1 ? "Abnormal" : "Hard") : "Normal") : "Trainning";
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
                    {
                        ShowHUDInfoTopRightMAPNAME(string.Concat((int)roundTime, "s\n", Level, " : ", str4));
                    }
                    else
                    {
                        ShowHUDInfoTopRightMAPNAME("\n" + Level + " : " + str4);
                    }
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                    {
                        string roomName = PhotonNetwork.Room.Name;
                        if (roomName.Length > 20)
                        {
                            roomName = roomName.Remove(19) + "...";
                        }
                        ShowHUDInfoTopRightMAPNAME("\n" + roomName + " [FFC000](" + Convert.ToString(Room.CurrentPlayers) + "/" + Convert.ToString(PhotonNetwork.Room.MaxPlayers) + ")");
                        if (needChooseSide)
                        {
                            ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
                        }
                    }
                }
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && killInfoGO.Count > 0 && killInfoGO[0] == null)
                {
                    killInfoGO.RemoveAt(0);
                }
                if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE && PhotonNetwork.isMasterClient && timeTotalServer > time)
                {
                    string str11;
                    IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
                    gameStart = false;
                    Screen.lockCursor = false;
                    Screen.showCursor = true;
                    string str6 = string.Empty;
                    string str7 = string.Empty;
                    string str8 = string.Empty;
                    string str9 = string.Empty;
                    string str10 = string.Empty;
                    foreach (Player player in PhotonNetwork.playerList)
                    {
                        if (player != null)
                        {
                            str6 = str6 + player.Properties.Name + "\n";
                            str7 = str7 + player.Properties.Kills + "\n";
                            str8 = str8 + player.Properties.Deaths + "\n";
                            str9 = str9 + player.Properties.MaxDamage + "\n";
                            str10 = str10 + player.Properties.TotalDamage + "\n";
                        }
                    }
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
                    {
                        str11 = string.Empty;
                        for (int k = 0; k < teamScores.Length; k++)
                        {
                            str11 = str11 + (k == 0 ? string.Concat("Team", k + 1, " ", teamScores[k], " " ) : " : ");
                        }
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                    {
                        str11 = "Highest Wave : " + highestwave;
                    }
                    else
                    {
                        str11 = string.Concat("Humanity ", humanScore, " : Titan ", titanScore);
                    }
                    object[] parameters = { str6, str7, str8, str9, str10, str11 };
                    photonView.RPC("showResult", PhotonTargets.AllBuffered, parameters);
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
                    if (item.time <= 0f && titans.Count + fT.Count < Math.Min(RCSettings.titanCap, 80))
                    {
                        string name1 = item.name;
                        if (name1 == "spawnAnnie")
                        {
                            PhotonNetwork.Instantiate("FEMALE_TITAN", item.location, new Quaternion(0f, 0f, 0f, 1f), 0);
                        }
                        else
                        {
                            GameObject obj2 = PhotonNetwork.Instantiate("TITAN_VER3.1", item.location, new Quaternion(0f, 0f, 0f, 1f), 0);
                            if (name1 == "spawnAbnormal")
                            {
                                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_I, false);
                            }
                            else if (name1 == "spawnJumper")
                            {
                                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
                            }
                            else if (name1 == "spawnCrawler")
                            {
                                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
                            }
                            else if (name1 == "spawnPunk")
                            {
                                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_PUNK, false);
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
        if (Time.timeScale <= 0.1f)
        {
            if (pauseWaitTime <= 3f)
            {
                pauseWaitTime -= Time.deltaTime * 1000000f;
                if (pauseWaitTime <= 1f)
                {
                    Camera.main.farClipPlane = 1500f;
                }
                if (pauseWaitTime <= 0f)
                {
                    pauseWaitTime = 0f;
                    Time.timeScale = 1f;
                }
            }
//            this.justRecompileThePlayerList();
        }
    }

    private void CoreEditor()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            GUI.FocusControl(null);
        }
        if (selectedObj != null)
        {
            float num = 0.2f;
            if (inputRC.isInputLevel(InputCodeRC.levelSlow))
            {
                num = 0.04f;
            }
            else if (inputRC.isInputLevel(InputCodeRC.levelFast))
            {
                num = 0.6f;
            }
            if (inputRC.isInputLevel(InputCodeRC.levelForward))
            {
                Transform transform1 = selectedObj.transform;
                transform1.position += num * new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
            }
            else if (inputRC.isInputLevel(InputCodeRC.levelBack))
            {
                Transform transform9 = selectedObj.transform;
                transform9.position -= num * new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
            }
            if (inputRC.isInputLevel(InputCodeRC.levelLeft))
            {
                Transform transform10 = selectedObj.transform;
                transform10.position -= num * new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z);
            }
            else if (inputRC.isInputLevel(InputCodeRC.levelRight))
            {
                Transform transform11 = selectedObj.transform;
                transform11.position += num * new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z);
            }
            if (inputRC.isInputLevel(InputCodeRC.levelDown))
            {
                Transform transform12 = selectedObj.transform;
                transform12.position -= Vector3.up * num;
            }
            else if (inputRC.isInputLevel(InputCodeRC.levelUp))
            {
                Transform transform13 = selectedObj.transform;
                transform13.position += Vector3.up * num;
            }
            if (!selectedObj.name.StartsWith("misc,region"))
            {
                if (inputRC.isInputLevel(InputCodeRC.levelRRight))
                {
                    selectedObj.transform.Rotate(Vector3.up * num);
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelRLeft))
                {
                    selectedObj.transform.Rotate(Vector3.down * num);
                }
                if (inputRC.isInputLevel(InputCodeRC.levelRCCW))
                {
                    selectedObj.transform.Rotate(Vector3.forward * num);
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelRCW))
                {
                    selectedObj.transform.Rotate(Vector3.back * num);
                }
                if (inputRC.isInputLevel(InputCodeRC.levelRBack))
                {
                    selectedObj.transform.Rotate(Vector3.left * num);
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelRForward))
                {
                    selectedObj.transform.Rotate(Vector3.right * num);
                }
            }
            if (inputRC.isInputLevel(InputCodeRC.levelPlace))
            {
                linkHash[3].Add(
                    selectedObj.GetInstanceID(),             //key
                    selectedObj.name + "," +                 // Name
                    selectedObj.transform.position.x + "," + // x 1
                    selectedObj.transform.position.y + "," + // y 1
                    selectedObj.transform.position.z + "," + // z 1
                    selectedObj.transform.rotation.x + "," + // x 2
                    selectedObj.transform.rotation.y + "," + // y 2
                    selectedObj.transform.rotation.z + "," + // z 2
                    selectedObj.transform.rotation.w);
                selectedObj = null;
                Camera.main.GetComponent<MouseLook>().enabled = true;
                Screen.lockCursor = true;
            }
            if (inputRC.isInputLevel(InputCodeRC.levelDelete))
            {
                linkHash[3].Remove(selectedObj?.GetInstanceID());
                Destroy(selectedObj);
                selectedObj = null;
                Camera.main.GetComponent<MouseLook>().enabled = true;
                Screen.lockCursor = true;
            }
        }
        else
        {
            if (Screen.lockCursor)
            {
                float num2 = 100f;
                if (inputRC.isInputLevel(InputCodeRC.levelSlow))
                {
                    num2 = 20f;
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelFast))
                {
                    num2 = 400f;
                }
                Transform transform7 = Camera.main.transform;
                if (inputRC.isInputLevel(InputCodeRC.levelForward))
                {
                    transform7.position += transform7.forward * num2 * Time.deltaTime;
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelBack))
                {
                    transform7.position -= transform7.forward * num2 * Time.deltaTime;
                }
                if (inputRC.isInputLevel(InputCodeRC.levelLeft))
                {
                    transform7.position -= transform7.right * num2 * Time.deltaTime;
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelRight))
                {
                    transform7.position += transform7.right * num2 * Time.deltaTime;
                }
                if (inputRC.isInputLevel(InputCodeRC.levelUp))
                {
                    transform7.position += transform7.up * num2 * Time.deltaTime;
                }
                else if (inputRC.isInputLevel(InputCodeRC.levelDown))
                {
                    transform7.position -= transform7.up * num2 * Time.deltaTime;
                }
            }
            if (inputRC.isInputLevelDown(InputCodeRC.levelCursor))
            {
                if (Screen.lockCursor)
                {
                    Camera.main.GetComponent<MouseLook>().enabled = false;
                    Screen.lockCursor = false;
                }
                else
                {
                    Camera.main.GetComponent<MouseLook>().enabled = true;
                    Screen.lockCursor = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && !Screen.lockCursor && GUIUtility.hotControl == 0 && !((Input.mousePosition.x <= 300f || Input.mousePosition.x >= Screen.width - 300f) && Screen.height - Input.mousePosition.y <= 600f))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
                {
                    Transform transform8 = hitInfo.transform;
                    if (transform8.gameObject.name.StartsWith("custom") || transform8.gameObject.name.StartsWith("base") || transform8.gameObject.name.StartsWith("racing") || transform8.gameObject.name.StartsWith("photon") || transform8.gameObject.name.StartsWith("spawnpoint") || transform8.gameObject.name.StartsWith("misc"))
                    {
                        selectedObj = transform8.gameObject;
                        Camera.main.GetComponent<MouseLook>().enabled = false;
                        Screen.lockCursor = true;
                        linkHash[3].Remove(selectedObj.GetInstanceID());
                    }
                    else if (transform8.parent.gameObject.name.StartsWith("custom") || transform8.parent.gameObject.name.StartsWith("base") || transform8.parent.gameObject.name.StartsWith("racing") || transform8.parent.gameObject.name.StartsWith("photon"))
                    {
                        selectedObj = transform8.parent.gameObject;
                        Camera.main.GetComponent<MouseLook>().enabled = false;
                        Screen.lockCursor = true;
                        linkHash[3].Remove(selectedObj.GetInstanceID());
                    }
                }
            }
        }
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
            Camera.main.GetComponent<TiltShift>().enabled = false;
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
            ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.CurrentLevel, currentLevel }
            };
            Player.Self.SetCustomProperties(propertiesToSet);
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
                        if (strArray[2].StartsWith("transparent"))
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
                            if (strArray[2].StartsWith("transparent"))
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
                            go.GetPhotonView().RPC("SetSize", PhotonTargets.AllBuffered, content[num]);
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
                            photonView.RPC("customlevelRPC", player, new object[] { strArray });
                        }
                    }
                    else
                    {
                        photonView.RPC("customlevelRPC", player, new object[] { levelCache[i] });
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
                photonView.RPC("customlevelRPC", player, new object[] { strArray });
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
        imatitan.Clear();
        for (num = 0; num < PhotonNetwork.playerList.Length; num++)
        {
            Player player = PhotonNetwork.playerList[num];
            ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.IsTitan, 1 }
            };
            player.SetCustomProperties(propertiesToSet);
        }
        int length = PhotonNetwork.playerList.Length;
        int infectionMode = RCSettings.infectionMode;
        for (num = 0; num < PhotonNetwork.playerList.Length; num++)
        {
            Player player2 = PhotonNetwork.playerList[num];
            if (length > 0 && UnityEngine.Random.Range(0f, 1f) <= infectionMode / (float)length)
            {
                ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.IsTitan, 2 }
                };
                player2.SetCustomProperties(hashtable2);
                imatitan.Add(player2.ID, 2);
                infectionMode--;
            }
            length--;
        }
        gameEndCD = 0f;
        RestartGame();
    }

    private void GameEndRC()
    {
        if (RCSettings.pointMode > 0)
        {
            foreach (Player player in PhotonNetwork.playerList)
            {
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.Kills, 0 },
                    { PlayerProperty.Deaths, 0 },
                    { PlayerProperty.MaxDamage, 0 },
                    { PlayerProperty.TotalDamage, 0 }
                };
                player.SetCustomProperties(propertiesToSet);
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
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.ORIGINAL)
            {
                Screen.lockCursor = false;
                Screen.showCursor = false;
            }
            GameObject obj4 = GameObject.FindGameObjectWithTag("Player");
            if (obj4 != null && obj4.GetComponent<HERO>() != null)
            {
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj4);
            }
            else
            {
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            }
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
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
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
        }
    }

    public void GameLose()
    {
        if (!(isWinning || isLosing))
        {
            isLosing = true;
            titanScore++;
            gameEndCD = gameEndTotalCDtime;
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
            {
                object[] parameters = { titanScore };
                photonView.RPC("netGameLose", PhotonTargets.Others, parameters);
                if ((int) settings[244] == 1)
                {
                    Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round ended (game lose).");
                }
            }
        }
    }

    public void GameWin()
    {
        if (!isLosing && !isWinning)
        {
            isWinning = true;
            humanScore++;
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
            {
                if (RCSettings.racingStatic == 1)
                {
                    gameEndCD = 1000f;
                }
                else
                {
                    gameEndCD = 20f;
                }
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                {
                    object[] parameters = { 0 };
                    photonView.RPC("netGameWin", PhotonTargets.Others, parameters);
                    if ((int) settings[244] == 1)
                    {
                        Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round ended (game win).");
                    }
                }
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
            {
                gameEndCD = gameEndTotalCDtime;
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                {
                    object[] objArray3 = { teamWinner };
                    photonView.RPC("netGameWin", PhotonTargets.Others, objArray3);
                    if ((int) settings[244] == 1)
                    {
                        Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round ended (game win).");
                    }
                }
                teamScores[teamWinner - 1]++;
            }
            else
            {
                gameEndCD = gameEndTotalCDtime;
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                {
                    object[] objArray4 = { humanScore };
                    photonView.RPC("netGameWin", PhotonTargets.Others, objArray4);
                    if ((int) settings[244] == 1)
                    {
                        Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> Round ended (game win).");
                    }
                }
            }
        }
    }

    public ArrayList GetPlayers()
    {
        return heroes;
    }

    public ArrayList GetTitans()
    {
        return titans;
    }

    private string GetHairType(int hairType)
    {
        if (hairType < 0)
        {
            return "Random";
        }
        return "Male " + hairType;
    }

    public static GameObject InstantiateCustomAsset(string key)
    {
        key = key.Substring(8);
        return (GameObject) RCassets.Load(key);
    }

    public bool IsAnyPlayerAlive()
    {
        return PhotonNetwork.playerList.Any(player => player.Properties.PlayerType == PlayerType.Human && player.Properties.Alive == true);
    }

    public bool IsAnyTeamMemberAlive(int team)
    {
        return PhotonNetwork.playerList.Any(player =>
            player.Properties.PlayerType == PlayerType.Human &&
            player.Properties.Team == team &&
            player.Properties.Alive == true);
    }

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
            photonView.RPC("ignorePlayer", PhotonTargets.Others, player.ID);
            if (!ignoreList.Contains(player.ID))
            {
                ignoreList.Add(player.ID);
                PhotonNetwork.RaiseEvent(254, null, true, new RaiseEventOptions
                {
                    TargetActors = new[] { player.ID }
                });
            }
            if (!(!ban || banHash.ContainsKey(player.ID)))
            {
                str = player.Properties.Name;
                banHash.Add(player.ID, str);
            }
            if (reason != string.Empty)
            {
                Mod.Interface.Chat.System("Player " + player.ID + " was autobanned. Reason:" + reason);
            }
        }
    }

    private void LoadConfig()
    {
        int num;
        object[] objArray = new object[270];
        objArray[0] = PlayerPrefs.GetInt("human", 1);
        objArray[1] = PlayerPrefs.GetInt("titan", 1);
        objArray[2] = PlayerPrefs.GetInt("level", 1);
        objArray[3] = PlayerPrefs.GetString("horse", string.Empty);
        objArray[4] = PlayerPrefs.GetString("hair", string.Empty);
        objArray[5] = PlayerPrefs.GetString("eye", string.Empty);
        objArray[6] = PlayerPrefs.GetString("glass", string.Empty);
        objArray[7] = PlayerPrefs.GetString("face", string.Empty);
        objArray[8] = PlayerPrefs.GetString("skin", string.Empty);
        objArray[9] = PlayerPrefs.GetString("costume", string.Empty);
        objArray[10] = PlayerPrefs.GetString("logo", string.Empty);
        objArray[11] = PlayerPrefs.GetString("bladel", string.Empty);
        objArray[12] = PlayerPrefs.GetString("blader", string.Empty);
        objArray[13] = PlayerPrefs.GetString("gas", string.Empty);
        objArray[14] = PlayerPrefs.GetString("hoodie", string.Empty);
        objArray[15] = PlayerPrefs.GetInt("gasenable", 0);
        objArray[16] = PlayerPrefs.GetInt("titantype1", -1);
        objArray[17] = PlayerPrefs.GetInt("titantype2", -1);
        objArray[18] = PlayerPrefs.GetInt("titantype3", -1);
        objArray[19] = PlayerPrefs.GetInt("titantype4", -1);
        objArray[20] = PlayerPrefs.GetInt("titantype5", -1);
        objArray[21] = PlayerPrefs.GetString("titanhair1", string.Empty);
        objArray[22] = PlayerPrefs.GetString("titanhair2", string.Empty);
        objArray[23] = PlayerPrefs.GetString("titanhair3", string.Empty);
        objArray[24] = PlayerPrefs.GetString("titanhair4", string.Empty);
        objArray[25] = PlayerPrefs.GetString("titanhair5", string.Empty);
        objArray[26] = PlayerPrefs.GetString("titaneye1", string.Empty);
        objArray[27] = PlayerPrefs.GetString("titaneye2", string.Empty);
        objArray[28] = PlayerPrefs.GetString("titaneye3", string.Empty);
        objArray[29] = PlayerPrefs.GetString("titaneye4", string.Empty);
        objArray[30] = PlayerPrefs.GetString("titaneye5", string.Empty);
        objArray[31] = 0;
        objArray[32] = PlayerPrefs.GetInt("titanR", 0);
        objArray[33] = PlayerPrefs.GetString("tree1", "http://i.imgur.com/QhvQaOY.png");
        objArray[34] = PlayerPrefs.GetString("tree2", "http://i.imgur.com/QhvQaOY.png");
        objArray[35] = PlayerPrefs.GetString("tree3", "http://i.imgur.com/k08IX81.png");
        objArray[36] = PlayerPrefs.GetString("tree4", "http://i.imgur.com/k08IX81.png");
        objArray[37] = PlayerPrefs.GetString("tree5", "http://i.imgur.com/JQPNchU.png");
        objArray[38] = PlayerPrefs.GetString("tree6", "http://i.imgur.com/JQPNchU.png");
        objArray[39] = PlayerPrefs.GetString("tree7", "http://i.imgur.com/IZdYWv4.png");
        objArray[40] = PlayerPrefs.GetString("tree8", "http://i.imgur.com/IZdYWv4.png");
        objArray[41] = PlayerPrefs.GetString("leaf1", "http://i.imgur.com/oFGV5oL.png");
        objArray[42] = PlayerPrefs.GetString("leaf2", "http://i.imgur.com/oFGV5oL.png");
        objArray[43] = PlayerPrefs.GetString("leaf3", "http://i.imgur.com/mKzawrQ.png");
        objArray[44] = PlayerPrefs.GetString("leaf4", "http://i.imgur.com/mKzawrQ.png");
        objArray[45] = PlayerPrefs.GetString("leaf5", "http://i.imgur.com/Ymzavsi.png");
        objArray[46] = PlayerPrefs.GetString("leaf6", "http://i.imgur.com/Ymzavsi.png");
        objArray[47] = PlayerPrefs.GetString("leaf7", "http://i.imgur.com/oQfD1So.png");
        objArray[48] = PlayerPrefs.GetString("leaf8", "http://i.imgur.com/oQfD1So.png");
        objArray[49] = PlayerPrefs.GetString("forestG", "http://i.imgur.com/IsDTn7x.png");
        objArray[50] = PlayerPrefs.GetInt("forestR", 0);
        objArray[51] = PlayerPrefs.GetString("house1", "http://i.imgur.com/wuy77R8.png");
        objArray[52] = PlayerPrefs.GetString("house2", "http://i.imgur.com/wuy77R8.png");
        objArray[53] = PlayerPrefs.GetString("house3", "http://i.imgur.com/wuy77R8.png");
        objArray[54] = PlayerPrefs.GetString("house4", "http://i.imgur.com/wuy77R8.png");
        objArray[55] = PlayerPrefs.GetString("house5", "http://i.imgur.com/wuy77R8.png");
        objArray[56] = PlayerPrefs.GetString("house6", "http://i.imgur.com/wuy77R8.png");
        objArray[57] = PlayerPrefs.GetString("house7", "http://i.imgur.com/wuy77R8.png");
        objArray[58] = PlayerPrefs.GetString("house8", "http://i.imgur.com/wuy77R8.png");
        objArray[59] = PlayerPrefs.GetString("cityG", "http://i.imgur.com/Mr9ZXip.png");
        objArray[60] = PlayerPrefs.GetString("cityW", "http://i.imgur.com/Tm7XfQP.png");
        objArray[61] = PlayerPrefs.GetString("cityH", "http://i.imgur.com/Q3YXkNM.png");
        objArray[62] = PlayerPrefs.GetInt("skinQ", 0);
        objArray[63] = PlayerPrefs.GetInt("skinQL", 0);
        objArray[64] = 0;
        objArray[65] = PlayerPrefs.GetString("eren", string.Empty);
        objArray[66] = PlayerPrefs.GetString("annie", string.Empty);
        objArray[67] = PlayerPrefs.GetString("colossal", string.Empty);
        objArray[68] = 100;
        objArray[69] = "default";
        objArray[70] = "1";
        objArray[71] = "1";
        objArray[72] = "1";
        objArray[73] = 1f;
        objArray[74] = 1f;
        objArray[75] = 1f;
        objArray[76] = 0;
        objArray[77] = string.Empty;
        objArray[78] = 0;
        objArray[79] = "1.0";
        objArray[80] = "1.0";
        objArray[81] = 0;
        objArray[82] = PlayerPrefs.GetString("cnumber", "1");
        objArray[83] = "30";
        objArray[84] = 0;
        objArray[85] = PlayerPrefs.GetString("cmax", "20");
        objArray[86] = PlayerPrefs.GetString("titanbody1", string.Empty);
        objArray[87] = PlayerPrefs.GetString("titanbody2", string.Empty);
        objArray[88] = PlayerPrefs.GetString("titanbody3", string.Empty);
        objArray[89] = PlayerPrefs.GetString("titanbody4", string.Empty);
        objArray[90] = PlayerPrefs.GetString("titanbody5", string.Empty);
        objArray[91] = 0;
        objArray[92] = PlayerPrefs.GetInt("traildisable", 0);
        objArray[93] = PlayerPrefs.GetInt("wind", 0);
        objArray[94] = PlayerPrefs.GetString("trailskin", string.Empty);
        objArray[95] = PlayerPrefs.GetString("snapshot", "0");
        objArray[96] = PlayerPrefs.GetString("trailskin2", string.Empty);
        objArray[97] = PlayerPrefs.GetInt("reel", 0);
        objArray[98] = PlayerPrefs.GetString("reelin", "LeftControl");
        objArray[99] = PlayerPrefs.GetString("reelout", "LeftAlt");
        objArray[100] = 0;
        objArray[101] = PlayerPrefs.GetString("tforward", "W");
        objArray[102] = PlayerPrefs.GetString("tback", "S");
        objArray[103] = PlayerPrefs.GetString("tleft", "A");
        objArray[104] = PlayerPrefs.GetString("tright", "D");
        objArray[105] = PlayerPrefs.GetString("twalk", "LeftShift");
        objArray[106] = PlayerPrefs.GetString("tjump", "Space");
        objArray[107] = PlayerPrefs.GetString("tpunch", "Q");
        objArray[108] = PlayerPrefs.GetString("tslam", "E");
        objArray[109] = PlayerPrefs.GetString("tgrabfront", "Alpha1");
        objArray[110] = PlayerPrefs.GetString("tgrabback", "Alpha3");
        objArray[111] = PlayerPrefs.GetString("tgrabnape", "Mouse1");
        objArray[112] = PlayerPrefs.GetString("tantiae", "Mouse0");
        objArray[113] = PlayerPrefs.GetString("tbite", "Alpha2");
        objArray[114] = PlayerPrefs.GetString("tcover", "Z");
        objArray[115] = PlayerPrefs.GetString("tsit", "X");
        objArray[116] = PlayerPrefs.GetInt("reel2", 0);
        objArray[117] = PlayerPrefs.GetString("lforward", "W");
        objArray[118] = PlayerPrefs.GetString("lback", "S");
        objArray[119] = PlayerPrefs.GetString("lleft", "A");
        objArray[120] = PlayerPrefs.GetString("lright", "D");
        objArray[121] = PlayerPrefs.GetString("lup", "Mouse1");
        objArray[122] = PlayerPrefs.GetString("ldown", "Mouse0");
        objArray[123] = PlayerPrefs.GetString("lcursor", "X");
        objArray[124] = PlayerPrefs.GetString("lplace", "Space");
        objArray[125] = PlayerPrefs.GetString("ldel", "Backspace");
        objArray[126] = PlayerPrefs.GetString("lslow", "LeftShift");
        objArray[127] = PlayerPrefs.GetString("lrforward", "R");
        objArray[128] = PlayerPrefs.GetString("lrback", "F");
        objArray[129] = PlayerPrefs.GetString("lrleft", "Q");
        objArray[130] = PlayerPrefs.GetString("lrright", "E");
        objArray[131] = PlayerPrefs.GetString("lrccw", "Z");
        objArray[132] = PlayerPrefs.GetString("lrcw", "C");
        objArray[133] = PlayerPrefs.GetInt("humangui", 0);
        objArray[134] = PlayerPrefs.GetString("horse2", string.Empty);
        objArray[135] = PlayerPrefs.GetString("hair2", string.Empty);
        objArray[136] = PlayerPrefs.GetString("eye2", string.Empty);
        objArray[137] = PlayerPrefs.GetString("glass2", string.Empty);
        objArray[138] = PlayerPrefs.GetString("face2", string.Empty);
        objArray[139] = PlayerPrefs.GetString("skin2", string.Empty);
        objArray[140] = PlayerPrefs.GetString("costume2", string.Empty);
        objArray[141] = PlayerPrefs.GetString("logo2", string.Empty);
        objArray[142] = PlayerPrefs.GetString("bladel2", string.Empty);
        objArray[143] = PlayerPrefs.GetString("blader2", string.Empty);
        objArray[144] = PlayerPrefs.GetString("gas2", string.Empty);
        objArray[145] = PlayerPrefs.GetString("hoodie2", string.Empty);
        objArray[146] = PlayerPrefs.GetString("trail2", string.Empty);
        objArray[147] = PlayerPrefs.GetString("horse3", string.Empty);
        objArray[148] = PlayerPrefs.GetString("hair3", string.Empty);
        objArray[149] = PlayerPrefs.GetString("eye3", string.Empty);
        objArray[150] = PlayerPrefs.GetString("glass3", string.Empty);
        objArray[151] = PlayerPrefs.GetString("face3", string.Empty);
        objArray[152] = PlayerPrefs.GetString("skin3", string.Empty);
        objArray[153] = PlayerPrefs.GetString("costume3", string.Empty);
        objArray[154] = PlayerPrefs.GetString("logo3", string.Empty);
        objArray[155] = PlayerPrefs.GetString("bladel3", string.Empty);
        objArray[156] = PlayerPrefs.GetString("blader3", string.Empty);
        objArray[157] = PlayerPrefs.GetString("gas3", string.Empty);
        objArray[158] = PlayerPrefs.GetString("hoodie3", string.Empty);
        objArray[159] = PlayerPrefs.GetString("trail3", string.Empty);
        objArray[161] = PlayerPrefs.GetString("lfast", "LeftControl");
        objArray[162] = PlayerPrefs.GetString("customGround", string.Empty);
        objArray[163] = PlayerPrefs.GetString("forestskyfront", string.Empty);
        objArray[164] = PlayerPrefs.GetString("forestskyback", string.Empty);
        objArray[165] = PlayerPrefs.GetString("forestskyleft", string.Empty);
        objArray[166] = PlayerPrefs.GetString("forestskyright", string.Empty);
        objArray[167] = PlayerPrefs.GetString("forestskyup", string.Empty);
        objArray[168] = PlayerPrefs.GetString("forestskydown", string.Empty);
        objArray[169] = PlayerPrefs.GetString("cityskyfront", string.Empty);
        objArray[170] = PlayerPrefs.GetString("cityskyback", string.Empty);
        objArray[171] = PlayerPrefs.GetString("cityskyleft", string.Empty);
        objArray[172] = PlayerPrefs.GetString("cityskyright", string.Empty);
        objArray[173] = PlayerPrefs.GetString("cityskyup", string.Empty);
        objArray[174] = PlayerPrefs.GetString("cityskydown", string.Empty);
        objArray[175] = PlayerPrefs.GetString("customskyfront", string.Empty);
        objArray[176] = PlayerPrefs.GetString("customskyback", string.Empty);
        objArray[177] = PlayerPrefs.GetString("customskyleft", string.Empty);
        objArray[178] = PlayerPrefs.GetString("customskyright", string.Empty);
        objArray[179] = PlayerPrefs.GetString("customskyup", string.Empty);
        objArray[180] = PlayerPrefs.GetString("customskydown", string.Empty);
        objArray[181] = PlayerPrefs.GetInt("dashenable", 0);
        objArray[182] = PlayerPrefs.GetString("dashkey", "RightControl");
        objArray[183] = PlayerPrefs.GetInt("vsync", 0);
        objArray[184] = PlayerPrefs.GetString("fpscap", "0");
        objArray[185] = 0;
        objArray[186] = 0;
        objArray[187] = 0;
        objArray[188] = 0;
        objArray[189] = PlayerPrefs.GetInt("speedometer", 0);
        objArray[190] = 0;
        objArray[191] = string.Empty;
        objArray[192] = PlayerPrefs.GetInt("bombMode", 0);
        objArray[193] = PlayerPrefs.GetInt("teamMode", 0);
        objArray[194] = PlayerPrefs.GetInt("rockThrow", 0);
        objArray[195] = PlayerPrefs.GetInt("explodeModeOn", 0);
        objArray[196] = PlayerPrefs.GetString("explodeModeNum", "30");
        objArray[197] = PlayerPrefs.GetInt("healthMode", 0);
        objArray[198] = PlayerPrefs.GetString("healthLower", "100");
        objArray[199] = PlayerPrefs.GetString("healthUpper", "200");
        objArray[200] = PlayerPrefs.GetInt("infectionModeOn", 0);
        objArray[201] = PlayerPrefs.GetString("infectionModeNum", "1");
        objArray[202] = PlayerPrefs.GetInt("banEren", 0);
        objArray[203] = PlayerPrefs.GetInt("moreTitanOn", 0);
        objArray[204] = PlayerPrefs.GetString("moreTitanNum", "1");
        objArray[205] = PlayerPrefs.GetInt("damageModeOn", 0);
        objArray[206] = PlayerPrefs.GetString("damageModeNum", "1000");
        objArray[207] = PlayerPrefs.GetInt("sizeMode", 0);
        objArray[208] = PlayerPrefs.GetString("sizeLower", "1.0");
        objArray[209] = PlayerPrefs.GetString("sizeUpper", "3.0");
        objArray[210] = PlayerPrefs.GetInt("spawnModeOn", 0);
        objArray[211] = PlayerPrefs.GetString("nRate", "20.0");
        objArray[212] = PlayerPrefs.GetString("aRate", "20.0");
        objArray[213] = PlayerPrefs.GetString("jRate", "20.0");
        objArray[214] = PlayerPrefs.GetString("cRate", "20.0");
        objArray[215] = PlayerPrefs.GetString("pRate", "20.0");
        objArray[216] = PlayerPrefs.GetInt("horseMode", 0);
        objArray[217] = PlayerPrefs.GetInt("waveModeOn", 0);
        objArray[218] = PlayerPrefs.GetString("waveModeNum", "1");
        objArray[219] = PlayerPrefs.GetInt("friendlyMode", 0);
        objArray[220] = PlayerPrefs.GetInt("pvpMode", 0);
        objArray[221] = PlayerPrefs.GetInt("maxWaveOn", 0);
        objArray[222] = PlayerPrefs.GetString("maxWaveNum", "20");
        objArray[223] = PlayerPrefs.GetInt("endlessModeOn", 0);
        objArray[224] = PlayerPrefs.GetString("endlessModeNum", "10");
        objArray[225] = PlayerPrefs.GetString("motd", string.Empty);
        objArray[226] = PlayerPrefs.GetInt("pointModeOn", 0);
        objArray[227] = PlayerPrefs.GetString("pointModeNum", "50");
        objArray[228] = PlayerPrefs.GetInt("ahssReload", 0);
        objArray[229] = PlayerPrefs.GetInt("punkWaves", 0);
        objArray[230] = 0;
        objArray[231] = PlayerPrefs.GetInt("mapOn", 0);
        objArray[232] = PlayerPrefs.GetString("mapMaximize", "Tab");
        objArray[233] = PlayerPrefs.GetString("mapToggle", "M");
        objArray[234] = PlayerPrefs.GetString("mapReset", "K");
        objArray[235] = PlayerPrefs.GetInt("globalDisableMinimap", 0);
        objArray[236] = PlayerPrefs.GetString("chatRebind", "None");
        objArray[237] = PlayerPrefs.GetString("hforward", "W");
        objArray[238] = PlayerPrefs.GetString("hback", "S");
        objArray[239] = PlayerPrefs.GetString("hleft", "A");
        objArray[240] = PlayerPrefs.GetString("hright", "D");
        objArray[241] = PlayerPrefs.GetString("hwalk", "LeftShift");
        objArray[242] = PlayerPrefs.GetString("hjump", "Q");
        objArray[243] = PlayerPrefs.GetString("hmount", "LeftControl");
        objArray[244] = PlayerPrefs.GetInt("chatfeed", 0);
        objArray[245] = 0;
        objArray[246] = PlayerPrefs.GetFloat("bombR", 1f);
        objArray[247] = PlayerPrefs.GetFloat("bombG", 1f);
        objArray[248] = PlayerPrefs.GetFloat("bombB", 1f);
        objArray[249] = PlayerPrefs.GetFloat("bombA", 1f);
        objArray[250] = PlayerPrefs.GetInt("bombRadius", 5);
        objArray[251] = PlayerPrefs.GetInt("bombRange", 5);
        objArray[252] = PlayerPrefs.GetInt("bombSpeed", 5);
        objArray[253] = PlayerPrefs.GetInt("bombCD", 5);
        objArray[254] = PlayerPrefs.GetString("cannonUp", "W");
        objArray[255] = PlayerPrefs.GetString("cannonDown", "S");
        objArray[256] = PlayerPrefs.GetString("cannonLeft", "A");
        objArray[257] = PlayerPrefs.GetString("cannonRight", "D");
        objArray[258] = PlayerPrefs.GetString("cannonFire", "Q");
        objArray[259] = PlayerPrefs.GetString("cannonMount", "G");
        objArray[260] = PlayerPrefs.GetString("cannonSlow", "LeftShift");
        objArray[261] = PlayerPrefs.GetInt("deadlyCannon", 0);
        objArray[262] = PlayerPrefs.GetString("liveCam", "Y");
        objArray[263] = 0;
        inputRC = new InputManagerRC();
        inputRC.setInputHuman(InputCodeRC.reelin, (string) objArray[98]);
        inputRC.setInputHuman(InputCodeRC.reelout, (string) objArray[99]);
        inputRC.setInputHuman(InputCodeRC.dash, (string) objArray[182]);
        inputRC.setInputHuman(InputCodeRC.mapMaximize, (string) objArray[232]);
        inputRC.setInputHuman(InputCodeRC.mapToggle, (string) objArray[233]);
        inputRC.setInputHuman(InputCodeRC.mapReset, (string) objArray[234]);
        inputRC.setInputHuman(InputCodeRC.chat, (string) objArray[236]);
        inputRC.setInputHuman(InputCodeRC.liveCam, (string) objArray[262]);
        if (!Enum.IsDefined(typeof(KeyCode), (string) objArray[232]))
        {
            objArray[232] = "None";
        }
        if (!Enum.IsDefined(typeof(KeyCode), (string) objArray[233]))
        {
            objArray[233] = "None";
        }
        if (!Enum.IsDefined(typeof(KeyCode), (string) objArray[234]))
        {
            objArray[234] = "None";
        }
        for (num = 0; num < 15; num++)
        {
            inputRC.setInputTitan(num, (string) objArray[101 + num]);
        }
        for (num = 0; num < 16; num++)
        {
            inputRC.setInputLevel(num, (string) objArray[117 + num]);
        }
        for (num = 0; num < 7; num++)
        {
            inputRC.setInputHorse(num, (string) objArray[237 + num]);
        }
        for (num = 0; num < 7; num++)
        {
            inputRC.setInputCannon(num, (string) objArray[254 + num]);
        }
        inputRC.setInputLevel(InputCodeRC.levelFast, (string) objArray[161]);
        Application.targetFrameRate = -1;
        if (int.TryParse((string) objArray[184], out var num2) && num2 > 0)
        {
            Application.targetFrameRate = num2;
        }
        QualitySettings.vSyncCount = 0;
        if ((int) objArray[183] == 1)
        {
            QualitySettings.vSyncCount = 1;
        }
        AudioListener.volume = PlayerPrefs.GetFloat("vol", 1f);
        QualitySettings.masterTextureLimit = PlayerPrefs.GetInt("skinQ", 0);
        linkHash = new[] { new ExitGames.Client.Photon.Hashtable(), new ExitGames.Client.Photon.Hashtable(), new ExitGames.Client.Photon.Hashtable(), new ExitGames.Client.Photon.Hashtable(), new ExitGames.Client.Photon.Hashtable() };
        settings = objArray;
        scroll = Vector2.zero;
        scroll2 = Vector2.zero;
        distanceSlider = PlayerPrefs.GetFloat("cameraDistance", 1f);
        mouseSlider = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
        qualitySlider = PlayerPrefs.GetFloat("GameQuality", 0f);
        transparencySlider = 1f;
    }

    private void LoadMapCustom()
    {
        GameObject[] objArray;
        int num;
        GameObject obj2;
        if ((int) settings[64] >= 100)
        {
            string[] strArray2 = { "Flare", "LabelInfoBottomRight", "LabelNetworkStatus", "skill_cd_bottom", "GasUI" };
            objArray = (GameObject[]) FindObjectsOfType(typeof(GameObject));
            for (num = 0; num < objArray.Length; num++)
            {
                obj2 = objArray[num];
                if (obj2.name.Contains("TREE") || obj2.name.Contains("aot_supply") || obj2.name.Contains("gameobjectOutSide"))
                {
                    Destroy(obj2);
                }
            }
            GameObject.Find("Cube_001").renderer.material.mainTexture = ((Material) RCassets.Load("grass")).mainTexture;
            Instantiate(RCassets.Load("spawnPlayer"), new Vector3(-10f, 1f, -10f), new Quaternion(0f, 0f, 0f, 1f));
            for (num = 0; num < strArray2.Length; num++)
            {
                string name1 = strArray2[num];
                GameObject obj3 = GameObject.Find(name1);
                if (obj3 != null)
                {
                    Destroy(obj3);
                }
            }
            Camera.main.GetComponent<SpectatorMovement>().disable = true;
        }
        else
        {
            GameObject obj4;
            string[] strArray3;
            int num2;
            InstantiateTracker.instance.Dispose();
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
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
                    {
                        RCEvent event2 = (RCEvent) RCEvents["OnFirstLoad"];
                        event2.checkEvent();
                    }
                }
                if (RCEvents.ContainsKey("OnRoundStart"))
                {
                    ((RCEvent) RCEvents["OnRoundStart"]).checkEvent();
                }
                photonView.RPC("setMasterRC", PhotonTargets.All);
            }
            logicLoaded = true;
            racingSpawnPoint = new Vector3(0f, 0f, 0f);
            racingSpawnPointSet = false;
            racingDoors = new List<GameObject>();
            allowedToCannon = new Dictionary<int, CannonValues>();
            if (!Level.StartsWith("Custom") && (int) settings[2] == 1 && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || PhotonNetwork.isMasterClient))
            {
                obj4 = GameObject.Find("aot_supply");
                if (obj4 != null && Minimap.instance != null)
                {
                    Minimap.instance.TrackGameObjectOnMinimap(obj4, Color.white, false, true, Minimap.IconStyle.SUPPLY);
                }
                string url = string.Empty;
                string str3 = string.Empty;
                string n = string.Empty;
                strArray3 = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
                if (LevelInfoManager.GetInfo(Level).LevelName.Contains("City"))
                {
                    for (num = 51; num < 59; num++)
                    {
                        url = url + (string) settings[num] + ",";
                    }
                    num2 = 0;
                    while (num2 < 250)
                    {
                        n = n + Convert.ToString((int) UnityEngine.Random.Range(0f, 8f));
                        num2++;
                    }
                    str3 = (string) settings[59] + "," + (string) settings[60] + "," + (string) settings[61];
                    for (num = 0; num < 6; num++)
                    {
                        strArray3[num] = (string) settings[num + 169];
                    }
                }
                else if (LevelInfoManager.GetInfo(Level).LevelName.Contains("Forest"))
                {
                    for (int i = 33; i < 41; i++)
                    {
                        url = url + (string) settings[i] + ",";
                    }
                    for (int j = 41; j < 49; j++)
                    {
                        str3 = str3 + (string) settings[j] + ",";
                    }
                    str3 = str3 + (string) settings[49];
                    for (int k = 0; k < 150; k++)
                    {
                        string str5 = Convert.ToString((int) UnityEngine.Random.Range(0f, 8f));
                        n = n + str5;
                        if ((int) settings[50] == 0)
                        {
                            n = n + str5;
                        }
                        else
                        {
                            n = n + Convert.ToString((int) UnityEngine.Random.Range(0f, 8f));
                        }
                    }
                    for (num = 0; num < 6; num++)
                    {
                        strArray3[num] = (string) settings[num + 163];
                    }
                }
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                {
                    StartCoroutine(LoadSkinEnumerator(n, url, str3, strArray3));
                }
                else if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, n, url, str3, strArray3);
                }
            }
            else if (Level.StartsWith("Custom") && IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
            {
                GameObject[] objArray3 = GameObject.FindGameObjectsWithTag(PlayerRespawnTag);
                for (num = 0; num < objArray3.Length; num++)
                {
                    obj4 = objArray3[num];
                    obj4.transform.position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));
                }
                objArray = (GameObject[]) FindObjectsOfType(typeof(GameObject));
                for (num = 0; num < objArray.Length; num++)
                {
                    obj2 = objArray[num];
                    if (obj2.name.Contains("TREE") || obj2.name.Contains("aot_supply"))
                    {
                        Destroy(obj2);
                    }
                    else if (obj2.name == "Cube_001" && obj2.transform.parent.gameObject.tag != "player" && obj2.renderer != null)
                    {
                        groundList.Add(obj2);
                        obj2.renderer.material.mainTexture = ((Material) RCassets.Load("grass")).mainTexture;
                    }
                }
                if (PhotonNetwork.isMasterClient)
                {
                    strArray3 = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
                    for (num = 0; num < 6; num++)
                    {
                        strArray3[num] = (string) settings[num + 175];
                    }
                    strArray3[6] = (string) settings[162];
                    if (int.TryParse((string) settings[85], out int num6))
                    {
                        RCSettings.titanCap = num6;
                    }
                    else
                    {
                        RCSettings.titanCap = 0;
                        settings[85] = "0";
                    }
                    RCSettings.titanCap = Math.Min(50, RCSettings.titanCap);
                    photonView.RPC("clearlevel", PhotonTargets.AllBuffered, strArray3, RCSettings.gameType);
                    RCRegions.Clear();
                    if (oldScript != currentScript)
                    {
                        ExitGames.Client.Photon.Hashtable hashtable;
                        levelCache.Clear();
                        titanSpawns.Clear();
                        playerSpawnsC.Clear();
                        playerSpawnsM.Clear();
                        titanSpawners.Clear();
                        currentLevel = string.Empty;
                        if (currentScript == string.Empty)
                        {
                            hashtable = new ExitGames.Client.Photon.Hashtable
                            {
                                { PlayerProperty.CurrentLevel, currentLevel }
                            };
                            Player.Self.SetCustomProperties(hashtable);
                            oldScript = currentScript;
                        }
                        else
                        {
                            string[] strArray4 = Regex.Replace(currentScript, @"\s+", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Split(';');
                            for (num = 0; num < Mathf.FloorToInt(((strArray4.Length - 1) / 100f)) + 1; num++)
                            {
                                string[] strArray5;
                                int num7;
                                string[] strArray6;
                                string str6;
                                if (num < strArray4.Length / 100)
                                {
                                    strArray5 = new string[101];
                                    num7 = 0;
                                    num2 = 100 * num;
                                    while (num2 < 100 * num + 100)
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
                                    for (num2 = 100 * num; num2 < 100 * num + strArray4.Length % 100; num2++)
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
                            hashtable = new ExitGames.Client.Photon.Hashtable
                            {
                                { PlayerProperty.CurrentLevel, currentLevel }
                            };
                            Player.Self.SetCustomProperties(hashtable);
                            oldScript = currentScript;
                        }
                    }
                    for (num = 0; num < PhotonNetwork.playerList.Length; num++)
                    {
                        Player player = PhotonNetwork.playerList[num];
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

    private IEnumerator LoadSkinEnumerator(string n, string url, string url2, string[] skybox)
    {
        bool mipmap = true;
        bool iteratorVariable1 = false;
        if ((int)settings[63] == 1)
        {
            mipmap = false;
        }
        if (skybox.Length > 5 && (skybox[0] != string.Empty || skybox[1] != string.Empty || skybox[2] != string.Empty || skybox[3] != string.Empty || skybox[4] != string.Empty || skybox[5] != string.Empty))
        {
            string key = string.Join(",", skybox);
            if (!linkHash[1].ContainsKey(key))
            {
                iteratorVariable1 = true;
                Material material = Camera.main.GetComponent<Skybox>().material;
                string iteratorVariable4 = skybox[0];
                string iteratorVariable5 = skybox[1];
                string iteratorVariable6 = skybox[2];
                string iteratorVariable7 = skybox[3];
                string iteratorVariable8 = skybox[4];
                string iteratorVariable9 = skybox[5];
                if (iteratorVariable4.EndsWith(".jpg") || iteratorVariable4.EndsWith(".png") || iteratorVariable4.EndsWith(".jpeg"))
                {
                    WWW link = new WWW(iteratorVariable4);
                    yield return link;
                    Texture2D texture = RCextensions.loadimage(link, mipmap, 500000);
                    link.Dispose();
                    texture.wrapMode = TextureWrapMode.Clamp;
                    material.SetTexture("_FrontTex", texture);
                }
                if (iteratorVariable5.EndsWith(".jpg") || iteratorVariable5.EndsWith(".png") || iteratorVariable5.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable12 = new WWW(iteratorVariable5);
                    yield return iteratorVariable12;
                    Texture2D iteratorVariable13 = RCextensions.loadimage(iteratorVariable12, mipmap, 500000);
                    iteratorVariable12.Dispose();
                    iteratorVariable13.wrapMode = TextureWrapMode.Clamp;
                    material.SetTexture("_BackTex", iteratorVariable13);
                }
                if (iteratorVariable6.EndsWith(".jpg") || iteratorVariable6.EndsWith(".png") || iteratorVariable6.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable14 = new WWW(iteratorVariable6);
                    yield return iteratorVariable14;
                    Texture2D iteratorVariable15 = RCextensions.loadimage(iteratorVariable14, mipmap, 500000);
                    iteratorVariable14.Dispose();
                    iteratorVariable15.wrapMode = TextureWrapMode.Clamp;
                    material.SetTexture("_LeftTex", iteratorVariable15);
                }
                if (iteratorVariable7.EndsWith(".jpg") || iteratorVariable7.EndsWith(".png") || iteratorVariable7.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable16 = new WWW(iteratorVariable7);
                    yield return iteratorVariable16;
                    Texture2D iteratorVariable17 = RCextensions.loadimage(iteratorVariable16, mipmap, 500000);
                    iteratorVariable16.Dispose();
                    iteratorVariable17.wrapMode = TextureWrapMode.Clamp;
                    material.SetTexture("_RightTex", iteratorVariable17);
                }
                if (iteratorVariable8.EndsWith(".jpg") || iteratorVariable8.EndsWith(".png") || iteratorVariable8.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable18 = new WWW(iteratorVariable8);
                    yield return iteratorVariable18;
                    Texture2D iteratorVariable19 = RCextensions.loadimage(iteratorVariable18, mipmap, 500000);
                    iteratorVariable18.Dispose();
                    iteratorVariable19.wrapMode = TextureWrapMode.Clamp;
                    material.SetTexture("_UpTex", iteratorVariable19);
                }
                if (iteratorVariable9.EndsWith(".jpg") || iteratorVariable9.EndsWith(".png") || iteratorVariable9.EndsWith(".jpeg"))
                {
                    WWW iteratorVariable20 = new WWW(iteratorVariable9);
                    yield return iteratorVariable20;
                    Texture2D iteratorVariable21 = RCextensions.loadimage(iteratorVariable20, mipmap, 500000);
                    iteratorVariable20.Dispose();
                    iteratorVariable21.wrapMode = TextureWrapMode.Clamp;
                    material.SetTexture("_DownTex", iteratorVariable21);
                }
                Camera.main.GetComponent<Skybox>().material = material;
                skyMaterial = material;
                linkHash[1].Add(key, material);
            }
            else
            {
                Camera.main.GetComponent<Skybox>().material = (Material)linkHash[1][key];
                skyMaterial = (Material)linkHash[1][key];
            }
        }
        if (LevelInfoManager.GetInfo(Level).LevelName.Contains("Forest"))
        {
            string[] iteratorVariable22 = url.Split(',');
            string[] iteratorVariable23 = url2.Split(',');
            int startIndex = 0;
            foreach (object iteratorVariable261 in FindObjectsOfType(typeof(GameObject)))
            {
                var iteratorVariable26 = (GameObject)iteratorVariable261;
                if (iteratorVariable26 != null)
                {
                    if (iteratorVariable26.name.Contains("TREE") && n.Length > startIndex + 1)
                    {
                        string s = n.Substring(startIndex, 1);
                        string iteratorVariable30 = n.Substring(startIndex + 1, 1);
                        if (int.TryParse(s, out var iteratorVariable27) && int.TryParse(iteratorVariable30, out var iteratorVariable28) && iteratorVariable27 >= 0 && iteratorVariable27 < 8 && iteratorVariable28 >= 0 && iteratorVariable28 < 8 && iteratorVariable22.Length >= 8 && iteratorVariable23.Length >= 8 && iteratorVariable22[iteratorVariable27] != null && iteratorVariable23[iteratorVariable28] != null)
                        {
                            string iteratorVariable31 = iteratorVariable22[iteratorVariable27];
                            string iteratorVariable32 = iteratorVariable23[iteratorVariable28];
                            foreach (Renderer iteratorVariable33 in iteratorVariable26.GetComponentsInChildren<Renderer>())
                            {
                                if (iteratorVariable33.name.Contains("Cube"))
                                {
                                    if (iteratorVariable31.EndsWith(".jpg") || iteratorVariable31.EndsWith(".png") || iteratorVariable31.EndsWith(".jpeg"))
                                    {
                                        if (!linkHash[2].ContainsKey(iteratorVariable31))
                                        {
                                            WWW iteratorVariable34 = new WWW(iteratorVariable31);
                                            yield return iteratorVariable34;
                                            Texture2D iteratorVariable35 = RCextensions.loadimage(iteratorVariable34, mipmap, 1000000);
                                            iteratorVariable34.Dispose();
                                            if (!linkHash[2].ContainsKey(iteratorVariable31))
                                            {
                                                iteratorVariable1 = true;
                                                iteratorVariable33.material.mainTexture = iteratorVariable35;
                                                linkHash[2].Add(iteratorVariable31, iteratorVariable33.material);
                                                iteratorVariable33.material = (Material)linkHash[2][iteratorVariable31];
                                            }
                                            else
                                            {
                                                iteratorVariable33.material = (Material)linkHash[2][iteratorVariable31];
                                            }
                                        }
                                        else
                                        {
                                            iteratorVariable33.material = (Material)linkHash[2][iteratorVariable31];
                                        }
                                    }
                                }
                                else if (iteratorVariable33.name.Contains("Plane_031"))
                                {
                                    if (iteratorVariable32.EndsWith(".jpg") || iteratorVariable32.EndsWith(".png") || iteratorVariable32.EndsWith(".jpeg"))
                                    {
                                        if (!linkHash[0].ContainsKey(iteratorVariable32))
                                        {
                                            WWW iteratorVariable36 = new WWW(iteratorVariable32);
                                            yield return iteratorVariable36;
                                            Texture2D iteratorVariable37 = RCextensions.loadimage(iteratorVariable36, mipmap, 200000);
                                            iteratorVariable36.Dispose();
                                            if (!linkHash[0].ContainsKey(iteratorVariable32))
                                            {
                                                iteratorVariable1 = true;
                                                iteratorVariable33.material.mainTexture = iteratorVariable37;
                                                linkHash[0].Add(iteratorVariable32, iteratorVariable33.material);
                                                iteratorVariable33.material = (Material)linkHash[0][iteratorVariable32];
                                            }
                                            else
                                            {
                                                iteratorVariable33.material = (Material)linkHash[0][iteratorVariable32];
                                            }
                                        }
                                        else
                                        {
                                            iteratorVariable33.material = (Material)linkHash[0][iteratorVariable32];
                                        }
                                    }
                                    else if (iteratorVariable32.ToLower() == "transparent")
                                    {
                                        iteratorVariable33.enabled = false;
                                    }
                                }
                            }
                        }
                        startIndex += 2;
                    }
                    else if (iteratorVariable26.name.Contains("Cube_001") && iteratorVariable26.transform.parent.gameObject.tag != "Player" && iteratorVariable23.Length > 8 && iteratorVariable23[8] != null)
                    {
                        string iteratorVariable38 = iteratorVariable23[8];
                        if (iteratorVariable38.EndsWith(".jpg") || iteratorVariable38.EndsWith(".png") || iteratorVariable38.EndsWith(".jpeg"))
                        {
                            foreach (Renderer iteratorVariable39 in iteratorVariable26.GetComponentsInChildren<Renderer>())
                            {
                                if (!linkHash[0].ContainsKey(iteratorVariable38))
                                {
                                    WWW iteratorVariable40 = new WWW(iteratorVariable38);
                                    yield return iteratorVariable40;
                                    Texture2D iteratorVariable41 = RCextensions.loadimage(iteratorVariable40, mipmap, 200000);
                                    iteratorVariable40.Dispose();
                                    if (!linkHash[0].ContainsKey(iteratorVariable38))
                                    {
                                        iteratorVariable1 = true;
                                        iteratorVariable39.material.mainTexture = iteratorVariable41;
                                        linkHash[0].Add(iteratorVariable38, iteratorVariable39.material);
                                        iteratorVariable39.material = (Material)linkHash[0][iteratorVariable38];
                                    }
                                    else
                                    {
                                        iteratorVariable39.material = (Material)linkHash[0][iteratorVariable38];
                                    }
                                }
                                else
                                {
                                    iteratorVariable39.material = (Material)linkHash[0][iteratorVariable38];
                                }
                            }
                        }
                        else if (iteratorVariable38.ToLower() == "transparent")
                        {
                            foreach (Renderer renderer1 in iteratorVariable26.GetComponentsInChildren<Renderer>())
                            {
                                renderer1.enabled = false;
                            }
                        }
                    }
                }
            }
        }
        else if (LevelInfoManager.GetInfo(Level).LevelName.Contains("City"))
        {
            string[] iteratorVariable42 = url.Split(',');
            string[] iteratorVariable43 = url2.Split(',');
            int iteratorVariable45 = 0;
            var iteratorVariable46 = FindObjectsOfType(typeof(GameObject));
            foreach (object iteratorVariable471 in iteratorVariable46)
            {
                var iteratorVariable47 = (GameObject) iteratorVariable471;
                if (iteratorVariable47 != null && iteratorVariable47.name.Contains("Cube_") && iteratorVariable47.transform.parent.gameObject.tag != "Player")
                {
                    if (iteratorVariable47.name.EndsWith("001"))
                    {
                        if (iteratorVariable43.Length > 0 && iteratorVariable43[0] != null)
                        {
                            string iteratorVariable48 = iteratorVariable43[0];
                            if (iteratorVariable48.EndsWith(".jpg") || iteratorVariable48.EndsWith(".png") || iteratorVariable48.EndsWith(".jpeg"))
                            {
                                foreach (Renderer iteratorVariable49 in iteratorVariable47.GetComponentsInChildren<Renderer>())
                                {
                                    if (!linkHash[0].ContainsKey(iteratorVariable48))
                                    {
                                        WWW iteratorVariable50 = new WWW(iteratorVariable48);
                                        yield return iteratorVariable50;
                                        Texture2D iteratorVariable51 = RCextensions.loadimage(iteratorVariable50, mipmap, 200000);
                                        iteratorVariable50.Dispose();
                                        if (!linkHash[0].ContainsKey(iteratorVariable48))
                                        {
                                            iteratorVariable1 = true;
                                            iteratorVariable49.material.mainTexture = iteratorVariable51;
                                            linkHash[0].Add(iteratorVariable48, iteratorVariable49.material);
                                            iteratorVariable49.material = (Material)linkHash[0][iteratorVariable48];
                                        }
                                        else
                                        {
                                            iteratorVariable49.material = (Material)linkHash[0][iteratorVariable48];
                                        }
                                    }
                                    else
                                    {
                                        iteratorVariable49.material = (Material)linkHash[0][iteratorVariable48];
                                    }
                                }
                            }
                            else if (iteratorVariable48.ToLower() == "transparent")
                            {
                                foreach (Renderer renderer1 in iteratorVariable47.GetComponentsInChildren<Renderer>())
                                {
                                    renderer1.enabled = false;
                                }
                            }
                        }
                    }
                    else if (iteratorVariable47.name.EndsWith("006") || iteratorVariable47.name.EndsWith("007") || iteratorVariable47.name.EndsWith("015") || iteratorVariable47.name.EndsWith("000") || iteratorVariable47.name.EndsWith("002") && iteratorVariable47.transform.position.x == 0f && iteratorVariable47.transform.position.y == 0f && iteratorVariable47.transform.position.z == 0f)
                    {
                        if (iteratorVariable43.Length > 0 && iteratorVariable43[1] != null)
                        {
                            string iteratorVariable52 = iteratorVariable43[1];
                            if (iteratorVariable52.EndsWith(".jpg") || iteratorVariable52.EndsWith(".png") || iteratorVariable52.EndsWith(".jpeg"))
                            {
                                foreach (Renderer iteratorVariable53 in iteratorVariable47.GetComponentsInChildren<Renderer>())
                                {
                                    if (!linkHash[0].ContainsKey(iteratorVariable52))
                                    {
                                        WWW iteratorVariable54 = new WWW(iteratorVariable52);
                                        yield return iteratorVariable54;
                                        Texture2D iteratorVariable55 = RCextensions.loadimage(iteratorVariable54, mipmap, 200000);
                                        iteratorVariable54.Dispose();
                                        if (!linkHash[0].ContainsKey(iteratorVariable52))
                                        {
                                            iteratorVariable1 = true;
                                            iteratorVariable53.material.mainTexture = iteratorVariable55;
                                            linkHash[0].Add(iteratorVariable52, iteratorVariable53.material);
                                            iteratorVariable53.material = (Material)linkHash[0][iteratorVariable52];
                                        }
                                        else
                                        {
                                            iteratorVariable53.material = (Material)linkHash[0][iteratorVariable52];
                                        }
                                    }
                                    else
                                    {
                                        iteratorVariable53.material = (Material)linkHash[0][iteratorVariable52];
                                    }
                                }
                            }
                        }
                    }
                    else if (iteratorVariable47.name.EndsWith("005") || iteratorVariable47.name.EndsWith("003") || iteratorVariable47.name.EndsWith("002") && (iteratorVariable47.transform.position.x != 0f || iteratorVariable47.transform.position.y != 0f || iteratorVariable47.transform.position.z != 0f) && n.Length > iteratorVariable45)
                    {
                        string iteratorVariable57 = n.Substring(iteratorVariable45, 1);
                        if (int.TryParse(iteratorVariable57, out var iteratorVariable56) && iteratorVariable56 >= 0 && iteratorVariable56 < 8 && iteratorVariable42.Length >= 8 && iteratorVariable42[iteratorVariable56] != null)
                        {
                            string iteratorVariable58 = iteratorVariable42[iteratorVariable56];
                            if (iteratorVariable58.EndsWith(".jpg") || iteratorVariable58.EndsWith(".png") || iteratorVariable58.EndsWith(".jpeg"))
                            {
                                foreach (Renderer iteratorVariable59 in iteratorVariable47.GetComponentsInChildren<Renderer>())
                                {
                                    if (!linkHash[2].ContainsKey(iteratorVariable58))
                                    {
                                        WWW iteratorVariable60 = new WWW(iteratorVariable58);
                                        yield return iteratorVariable60;
                                        Texture2D iteratorVariable61 = RCextensions.loadimage(iteratorVariable60, mipmap, 1000000);
                                        iteratorVariable60.Dispose();
                                        if (!linkHash[2].ContainsKey(iteratorVariable58))
                                        {
                                            iteratorVariable1 = true;
                                            iteratorVariable59.material.mainTexture = iteratorVariable61;
                                            linkHash[2].Add(iteratorVariable58, iteratorVariable59.material);
                                            iteratorVariable59.material = (Material)linkHash[2][iteratorVariable58];
                                        }
                                        else
                                        {
                                            iteratorVariable59.material = (Material)linkHash[2][iteratorVariable58];
                                        }
                                    }
                                    else
                                    {
                                        iteratorVariable59.material = (Material)linkHash[2][iteratorVariable58];
                                    }
                                }
                            }
                        }
                        iteratorVariable45++;
                    }
                    else if ((iteratorVariable47.name.EndsWith("019") || iteratorVariable47.name.EndsWith("020")) && iteratorVariable43.Length > 2 && iteratorVariable43[2] != null)
                    {
                        string iteratorVariable62 = iteratorVariable43[2];
                        if (iteratorVariable62.EndsWith(".jpg") || iteratorVariable62.EndsWith(".png") || iteratorVariable62.EndsWith(".jpeg"))
                        {
                            foreach (Renderer iteratorVariable63 in iteratorVariable47.GetComponentsInChildren<Renderer>())
                            {
                                if (!linkHash[2].ContainsKey(iteratorVariable62))
                                {
                                    WWW iteratorVariable64 = new WWW(iteratorVariable62);
                                    yield return iteratorVariable64;
                                    Texture2D iteratorVariable65 = RCextensions.loadimage(iteratorVariable64, mipmap, 1000000);
                                    iteratorVariable64.Dispose();
                                    if (!linkHash[2].ContainsKey(iteratorVariable62))
                                    {
                                        iteratorVariable1 = true;
                                        iteratorVariable63.material.mainTexture = iteratorVariable65;
                                        linkHash[2].Add(iteratorVariable62, iteratorVariable63.material);
                                        iteratorVariable63.material = (Material)linkHash[2][iteratorVariable62];
                                    }
                                    else
                                    {
                                        iteratorVariable63.material = (Material)linkHash[2][iteratorVariable62];
                                    }
                                }
                                else
                                {
                                    iteratorVariable63.material = (Material)linkHash[2][iteratorVariable62];
                                }
                            }
                        }
                    }
                }
            }
        }
        Minimap.TryRecaptureInstance();
        if (iteratorVariable1)
        {
            UnloadAssets();
        }
    }

    private string QualityToString(int textureType)
    {
        if (textureType == 0)
        {
            return "High";
        }
        if (textureType == 1)
        {
            return "Med";
        }
        return "Low";
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
            object[] parameters = { Shelter.Profile.Name, time1 };
            photonView.RPC("getRacingResult", PhotonTargets.MasterClient, parameters);
        }
        GameWin();
    }

    public void SpawnPlayerTitanAfterGameEnd(string id) // Called after choose of titan
    {
        myLastHero = id.ToUpper();
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { "dead", true }
        };
        ExitGames.Client.Photon.Hashtable propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { PlayerProperty.IsTitan, 2 }
        };
        propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = true;
        ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public void SpawnPlayerTitanAfterGameEndRC(string id)
    {
        myLastHero = id.ToUpper();
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { "dead", true }
        };
        ExitGames.Client.Photon.Hashtable propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { PlayerProperty.IsTitan, 2 }
        };
        propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = true;
        ShowHUDInfoCenter("Syncing spawn locations...");
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public void SpawnPlayerAfterGameEnd(string id)
    {
        myLastHero = id.ToUpper();
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { "dead", true }
        };
        ExitGames.Client.Photon.Hashtable propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { PlayerProperty.IsTitan, 1 }
        };
        propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = false;
        ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
    }

    public void SpawnPlayerAfterGameEndRC(string id)
    {
        myLastHero = id.ToUpper();
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { "dead", true }
        };
        ExitGames.Client.Photon.Hashtable propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { PlayerProperty.IsTitan, 1 }
        };
        propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
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
        single_kills++;
        single_maxDamage = Mathf.Max(dmg, single_maxDamage);
        single_totalDamage += dmg;
    }

    public void PlayerKillInfoUpdate(Player player, int dmg)
    {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
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
        IComparer comparer = new IComparerRacingResult();
        racingResult.Sort(comparer);
        int num = Mathf.Min(racingResult.Count, 10);
        for (int i = 0; i < num; i++)
        {
            string localRacingResult1 = this.localRacingResult;
            object[] objArray2 = { localRacingResult1, "Rank ", i + 1, " : " };
            this.localRacingResult = string.Concat(objArray2);
            this.localRacingResult = this.localRacingResult + ((RacingResult) racingResult[i]).name;
            this.localRacingResult = this.localRacingResult + "   " + ((int) (((RacingResult) racingResult[i]).time * 100f) * 0.01f) + "s";
            this.localRacingResult = this.localRacingResult + "\n";
        }
        object[] parameters = { this.localRacingResult };
        photonView.RPC("netRefreshRacingResult", PhotonTargets.All, parameters);
    }

    public IEnumerator ReloadSkyEnumerator()
    {
        yield return new WaitForSeconds(0.5f);
        if (skyMaterial != null && Camera.main.GetComponent<Skybox>().material != skyMaterial)
            Camera.main.GetComponent<Skybox>().material = skyMaterial;
        Screen.lockCursor = !Screen.lockCursor; //tf? Probably feng's try to fix something lul
        Screen.lockCursor = !Screen.lockCursor;
    }

    public void RemoveColossalTitan(COLOSSAL_TITAN titan)
    {
        cT.Remove(titan);
    }

    public void RemoveTitanEren(TITAN_EREN hero)
    {
        eT.Remove(hero);
    }

    public void RemoveFemaleTitan(FEMALE_TITAN titan)
    {
        fT.Remove(titan);
    }

    public void RemoveHero(HERO hero)
    {
        heroes.Remove(hero);
    }

    public void RemoveHook(Bullet h)
    {
        hooks.Remove(h);
    }

    public void RemoveTitan(TITAN titan)
    {
        titans.Remove(titan);
    }

    private void ResetGameSettings()
    {
        RCSettings.bombMode = 0;
        RCSettings.teamMode = 0;
        RCSettings.pointMode = 0;
        RCSettings.disableRock = 0;
        RCSettings.explodeMode = 0;
        RCSettings.healthMode = 0;
        RCSettings.healthLower = 0;
        RCSettings.healthUpper = 0;
        RCSettings.infectionMode = 0;
        RCSettings.banEren = 0;
        RCSettings.moreTitans = 0;
        RCSettings.damageMode = 0;
        RCSettings.sizeMode = 0;
        RCSettings.sizeLower = 0f;
        RCSettings.sizeUpper = 0f;
        RCSettings.spawnMode = 0;
        RCSettings.nRate = 0f;
        RCSettings.aRate = 0f;
        RCSettings.jRate = 0f;
        RCSettings.cRate = 0f;
        RCSettings.pRate = 0f;
        RCSettings.horseMode = 0;
        RCSettings.waveModeOn = 0;
        RCSettings.waveModeNum = 0;
        RCSettings.friendlyMode = 0;
        RCSettings.pvpMode = 0;
        RCSettings.maxWave = 0;
        RCSettings.endlessMode = 0;
        RCSettings.ahssReload = 0;
        RCSettings.punkWaves = 0;
        RCSettings.globalDisableMinimap = 0;
        RCSettings.motd = string.Empty;
        RCSettings.deadlyCannons = 0;
        RCSettings.asoPreservekdr = 0;
        RCSettings.racingStatic = 0;
    }

    private void ResetSettings(bool isLeave)
    {
        name = Shelter.Profile.Name;
        masterRC = false;
        ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
        {
            { PlayerProperty.RCTeam, 0 }
        };
        if (isLeave)
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
        Player.Self.SetCustomProperties(propertiesToSet);
        ResetGameSettings();
        banHash = new ExitGames.Client.Photon.Hashtable();
        imatitan = new ExitGames.Client.Photon.Hashtable();
        oldScript = string.Empty;
        ignoreList = new List<int>();
        restartCount = new List<float>();
        heroHash = new ExitGames.Client.Photon.Hashtable();
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
//                        photonView.RPC("respawnHeroInNewRound", targetPlayer);
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
        if (!gameTimesUp)
        {
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
            racingResult = new ArrayList();
            ShowHUDInfoCenter(string.Empty);
            isRestarting = true;
            DestroyAllExistingCloths();
            PhotonNetwork.DestroyAll();
            ExitGames.Client.Photon.Hashtable hash = CheckGameGUI();
            photonView.RPC("settingRPC", PhotonTargets.Others, hash);
            photonView.RPC("RPCLoadLevel", PhotonTargets.All);
            SetGameSettings(hash);
            if (masterclientSwitched)
            {
                SendChatContentInfo("<color=#A8FF24>MasterClient has switched to </color>" + Player.Self.Properties.Name.hexColor());
            }
        }
    }
    
    public void RestartSingleplayer()
    {
        startRacing = false;
        endRacing = false;
        checkpoint = null;
        single_kills = 0;
        single_maxDamage = 0;
        single_totalDamage = 0;
        timeElapse = 0f;
        roundTime = 0f;
        timeTotalServer = 0f;
        isWinning = false;
        isLosing = false;
        isPlayer1Winning = false;
        isPlayer2Winning = false;
        wave = 1;
        myRespawnTime = 0f;
        ShowHUDInfoCenter(string.Empty);
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
        if (RCSettings.infectionMode > 0)
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

    public static PeerStates returnPeerState(int peerstate)
    {
        switch (peerstate)
        {
            case 0:
                return PeerStates.Authenticated;

            case 1:
                return PeerStates.ConnectedToMaster;

            case 2:
                return PeerStates.DisconnectingFromMasterserver;

            case 3:
                return PeerStates.DisconnectingFromGameserver;

            case 4:
                return PeerStates.DisconnectingFromNameServer;
        }
        return PeerStates.ConnectingToMasterserver;
    }

    public void SendChatContentInfo(string content)
    {
        photonView.RPC("Chat", PhotonTargets.All, content, string.Empty);
    }

    public void SendKillInfo(bool t1, string killer, bool t2, string victim, int dmg = 0)
    {
        photonView.RPC("updateKillInfo", PhotonTargets.All, t1, killer, t2, victim, dmg);
    }

    public static void ServerCloseConnection(Player targetPlayer, bool requestIpBan, string inGameName = null)
    {
        RaiseEventOptions options = new RaiseEventOptions {
            TargetActors = new[] { targetPlayer.ID }
        };
        if (requestIpBan) // I have litterally no idea on what this is.
        {
            ExitGames.Client.Photon.Hashtable eventContent = new ExitGames.Client.Photon.Hashtable
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

    public static void ServerRequestAuthentication(string authPassword) //TODO: No idea of what this is
    {
        if (!string.IsNullOrEmpty(authPassword))
        {
            ExitGames.Client.Photon.Hashtable eventContent = new ExitGames.Client.Photon.Hashtable
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
            ExitGames.Client.Photon.Hashtable eventContent = new ExitGames.Client.Photon.Hashtable
            {
                [(byte)0] = bannedAddress
            };
            PhotonNetwork.RaiseEvent(199, eventContent, true, new RaiseEventOptions());
        }
    }

    private void SetGameSettings(ExitGames.Client.Photon.Hashtable hash)
    {
        string str;
        ExitGames.Client.Photon.Hashtable hashtable;
        restartingEren = false;
        restartingBomb = false;
        restartingHorse = false;
        restartingTitan = false;
        if (hash.ContainsKey("bomb"))
        {
            if (RCSettings.bombMode != (int) hash["bomb"])
            {
                RCSettings.bombMode = (int) hash["bomb"];
                Mod.Interface.Chat.System("PVP Bomb Mode enabled.");
            }
        }
        else if (RCSettings.bombMode != 0)
        {
            RCSettings.bombMode = 0;
            Mod.Interface.Chat.System("PVP Bomb Mode disabled.");
            if (PhotonNetwork.isMasterClient)
            {
                restartingBomb = true;
            }
        }
        if (hash.ContainsKey("globalDisableMinimap"))
        {
            if (RCSettings.globalDisableMinimap != (int) hash["globalDisableMinimap"])
            {
                RCSettings.globalDisableMinimap = (int) hash["globalDisableMinimap"];
                Mod.Interface.Chat.System("Minimaps are not allowed.");
            }
        }
        else if (RCSettings.globalDisableMinimap != 0)
        {
            RCSettings.globalDisableMinimap = 0;
            Mod.Interface.Chat.System("Minimaps are allowed.");
        }
        if (hash.ContainsKey("horse"))
        {
            if (RCSettings.horseMode != (int) hash["horse"])
            {
                RCSettings.horseMode = (int) hash["horse"];
                Mod.Interface.Chat.System("Horses enabled.");
            }
        }
        else if (RCSettings.horseMode != 0)
        {
            RCSettings.horseMode = 0;
            Mod.Interface.Chat.System("Horses disabled.");
            if (PhotonNetwork.isMasterClient)
            {
                restartingHorse = true;
            }
        }
        if (hash.ContainsKey("punkWaves"))
        {
            if (RCSettings.punkWaves != (int) hash["punkWaves"])
            {
                RCSettings.punkWaves = (int) hash["punkWaves"];
                Mod.Interface.Chat.System("Punk override every 5 waves enabled.");
            }
        }
        else if (RCSettings.punkWaves != 0)
        {
            RCSettings.punkWaves = 0;
            Mod.Interface.Chat.System("Punk override every 5 waves disabled.");
        }
        if (hash.ContainsKey("ahssReload"))
        {
            if (RCSettings.ahssReload != (int) hash["ahssReload"])
            {
                RCSettings.ahssReload = (int) hash["ahssReload"];
                Mod.Interface.Chat.System("AHSS Air-Reload disabled.");
            }
        }
        else if (RCSettings.ahssReload != 0)
        {
            RCSettings.ahssReload = 0;
            Mod.Interface.Chat.System("AHSS Air-Reload allowed.");
        }
        if (hash.ContainsKey("team"))
        {
            if (RCSettings.teamMode != (int) hash["team"])
            {
                RCSettings.teamMode = (int) hash["team"];
                str = string.Empty;
                switch (RCSettings.teamMode)
                {
                    case 1:
                        str = "no sort";
                        break;
                    case 2:
                        str = "locked by size";
                        break;
                    case 3:
                        str = "locked by skill";
                        break;
                }
                Mod.Interface.Chat.System("Team Mode enabled (" + str + ").");
                if (Player.Self.Properties.RCTeam == 0)
                {
                    SetTeam(3);
                }
            }
        }
        else if (RCSettings.teamMode != 0)
        {
            RCSettings.teamMode = 0;
            SetTeam(0);
            Mod.Interface.Chat.System("Team mode disabled.");
        }
        if (hash.ContainsKey("point"))
        {
            if (RCSettings.pointMode != (int) hash["point"])
            {
                RCSettings.pointMode = (int) hash["point"];
                Mod.Interface.Chat.System("Point limit enabled (" + Convert.ToString(RCSettings.pointMode) + ").");
            }
        }
        else if (RCSettings.pointMode != 0)
        {
            RCSettings.pointMode = 0;
            Mod.Interface.Chat.System("Point limit disabled.");
        }
        if (hash.ContainsKey("rock"))
        {
            if (RCSettings.disableRock != (int) hash["rock"])
            {
                RCSettings.disableRock = (int) hash["rock"];
                Mod.Interface.Chat.System("Punk rock throwing disabled.");
            }
        }
        else if (RCSettings.disableRock != 0)
        {
            RCSettings.disableRock = 0;
            Mod.Interface.Chat.System("Punk rock throwing enabled.");
        }
        if (hash.ContainsKey("explode"))
        {
            if (RCSettings.explodeMode != (int) hash["explode"])
            {
                RCSettings.explodeMode = (int) hash["explode"];
                Mod.Interface.Chat.System("Titan Explode Mode enabled (Radius " + Convert.ToString(RCSettings.explodeMode) + ").");
            }
        }
        else if (RCSettings.explodeMode != 0)
        {
            RCSettings.explodeMode = 0;
            Mod.Interface.Chat.System("Titan Explode Mode disabled.");
        }
        if (hash.ContainsKey("healthMode") && hash.ContainsKey("healthLower") && hash.ContainsKey("healthUpper"))
        {
            if (RCSettings.healthMode != (int) hash["healthMode"] || RCSettings.healthLower != (int) hash["healthLower"] || RCSettings.healthUpper != (int) hash["healthUpper"])
            {
                RCSettings.healthMode = (int) hash["healthMode"];
                RCSettings.healthLower = (int) hash["healthLower"];
                RCSettings.healthUpper = (int) hash["healthUpper"];
                str = "Static";
                if (RCSettings.healthMode == 2)
                {
                    str = "Scaled";
                }
                Mod.Interface.Chat.System("Titan Health (" + str + ", " + RCSettings.healthLower + " to " + RCSettings.healthUpper + ") enabled.");
            }
        }
        else if (RCSettings.healthMode != 0 || RCSettings.healthLower != 0 || RCSettings.healthUpper != 0)
        {
            RCSettings.healthMode = 0;
            RCSettings.healthLower = 0;
            RCSettings.healthUpper = 0;
            Mod.Interface.Chat.System("Titan Health disabled.");
        }
        if (hash.ContainsKey("infection"))
        {
            if (RCSettings.infectionMode != (int) hash["infection"])
            {
                RCSettings.infectionMode = (int) hash["infection"];
                name = Shelter.Profile.Name;
                hashtable = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.RCTeam, 0 }
                };
                Player.Self.SetCustomProperties(hashtable);
                Mod.Interface.Chat.System("Infection mode (" + Convert.ToString(RCSettings.infectionMode) + ") enabled. Make sure your first character is human.");
            }
        }
        else if (RCSettings.infectionMode != 0)
        {
            RCSettings.infectionMode = 0;
            hashtable = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.IsTitan, 1 }
            };
            Player.Self.SetCustomProperties(hashtable);
            Mod.Interface.Chat.System("Infection Mode disabled.");
            if (PhotonNetwork.isMasterClient)
            {
                restartingTitan = true;
            }
        }
        if (hash.ContainsKey("eren"))
        {
            if (RCSettings.banEren != (int) hash["eren"])
            {
                RCSettings.banEren = (int) hash["eren"];
                Mod.Interface.Chat.System("Anti-Eren enabled. Using eren transform will get you kicked.");
                if (PhotonNetwork.isMasterClient)
                {
                    restartingEren = true;
                }
            }
        }
        else if (RCSettings.banEren != 0)
        {
            RCSettings.banEren = 0;
            Mod.Interface.Chat.System("Anti-Eren disabled. Eren transform is allowed.");
        }
        if (hash.ContainsKey("titanc"))
        {
            if (RCSettings.moreTitans != (int) hash["titanc"])
            {
                RCSettings.moreTitans = (int) hash["titanc"];
                Mod.Interface.Chat.System("" + Convert.ToString(RCSettings.moreTitans) + " titans will spawn each round.");
            }
        }
        else if (RCSettings.moreTitans != 0)
        {
            RCSettings.moreTitans = 0;
            Mod.Interface.Chat.System("Default titans will spawn each round.");
        }
        if (hash.ContainsKey("damage"))
        {
            if (RCSettings.damageMode != (int) hash["damage"])
            {
                RCSettings.damageMode = (int) hash["damage"];
                Mod.Interface.Chat.System("Nape minimum damage (" + Convert.ToString(RCSettings.damageMode) + ") enabled.");
            }
        }
        else if (RCSettings.damageMode != 0)
        {
            RCSettings.damageMode = 0;
            Mod.Interface.Chat.System("Nape minimum damage disabled.");
        }
        if (hash.ContainsKey("sizeMode") && hash.ContainsKey("sizeLower") && hash.ContainsKey("sizeUpper"))
        {
            if (RCSettings.sizeMode != (int) hash["sizeMode"] || RCSettings.sizeLower != (float) hash["sizeLower"] || RCSettings.sizeUpper != (float) hash["sizeUpper"])
            {
                RCSettings.sizeMode = (int) hash["sizeMode"];
                RCSettings.sizeLower = (float) hash["sizeLower"];
                RCSettings.sizeUpper = (float) hash["sizeUpper"];
                Mod.Interface.Chat.System("Custom titan size (" + RCSettings.sizeLower.ToString("F2") + "," + RCSettings.sizeUpper.ToString("F2") + ") enabled.");
            }
        }
        else if (RCSettings.sizeMode != 0 || RCSettings.sizeLower != 0f || RCSettings.sizeUpper != 0f)
        {
            RCSettings.sizeMode = 0;
            RCSettings.sizeLower = 0f;
            RCSettings.sizeUpper = 0f;
            Mod.Interface.Chat.System("Custom titan size disabled.");
        }
        if (hash.ContainsKey("spawnMode") && hash.ContainsKey("nRate") && hash.ContainsKey("aRate") && hash.ContainsKey("jRate") && hash.ContainsKey("cRate") && hash.ContainsKey("pRate"))
        {
            if (RCSettings.spawnMode != (int) hash["spawnMode"] || RCSettings.nRate != (float) hash["nRate"] || RCSettings.aRate != (float) hash["aRate"] || RCSettings.jRate != (float) hash["jRate"] || RCSettings.cRate != (float) hash["cRate"] || RCSettings.pRate != (float) hash["pRate"])
            {
                RCSettings.spawnMode = (int) hash["spawnMode"];
                RCSettings.nRate = (float) hash["nRate"];
                RCSettings.aRate = (float) hash["aRate"];
                RCSettings.jRate = (float) hash["jRate"];
                RCSettings.cRate = (float) hash["cRate"];
                RCSettings.pRate = (float) hash["pRate"];
                Mod.Interface.Chat.System("Custom spawn rate enabled (" + RCSettings.nRate.ToString("F2") + "% Normal, " + RCSettings.aRate.ToString("F2") + "% Abnormal, " + RCSettings.jRate.ToString("F2") + "% Jumper, " + RCSettings.cRate.ToString("F2") + "% Crawler, " + RCSettings.pRate.ToString("F2") + "% Punk ");
            }
        }
        else if (RCSettings.spawnMode != 0 || RCSettings.nRate != 0f || RCSettings.aRate != 0f || RCSettings.jRate != 0f || RCSettings.cRate != 0f || RCSettings.pRate != 0f)
        {
            RCSettings.spawnMode = 0;
            RCSettings.nRate = 0f;
            RCSettings.aRate = 0f;
            RCSettings.jRate = 0f;
            RCSettings.cRate = 0f;
            RCSettings.pRate = 0f;
            Mod.Interface.Chat.System("Custom spawn rate disabled.");
        }
        if (hash.ContainsKey("waveModeOn") && hash.ContainsKey("waveModeNum"))
        {
            if (RCSettings.waveModeOn != (int) hash["waveModeOn"] || RCSettings.waveModeNum != (int) hash["waveModeNum"])
            {
                RCSettings.waveModeOn = (int) hash["waveModeOn"];
                RCSettings.waveModeNum = (int) hash["waveModeNum"];
                Mod.Interface.Chat.System("Custom wave mode (" + RCSettings.waveModeNum + ") enabled.");
            }
        }
        else if (RCSettings.waveModeOn != 0 || RCSettings.waveModeNum != 0)
        {
            RCSettings.waveModeOn = 0;
            RCSettings.waveModeNum = 0;
            Mod.Interface.Chat.System("Custom wave mode disabled.");
        }
        if (hash.ContainsKey("friendly"))
        {
            if (RCSettings.friendlyMode != (int) hash["friendly"])
            {
                RCSettings.friendlyMode = (int) hash["friendly"];
                Mod.Interface.Chat.System("PVP is prohibited.");
            }
        }
        else if (RCSettings.friendlyMode != 0)
        {
            RCSettings.friendlyMode = 0;
            Mod.Interface.Chat.System("PVP is allowed.");
        }
        if (hash.ContainsKey("pvp"))
        {
            if (RCSettings.pvpMode != (int) hash["pvp"])
            {
                RCSettings.pvpMode = (int) hash["pvp"];
                str = string.Empty;
                if (RCSettings.pvpMode == 1)
                {
                    str = "Team-Based";
                }
                else if (RCSettings.pvpMode == 2)
                {
                    str = "FFA";
                }
                Mod.Interface.Chat.System("Blade/AHSS PVP enabled (" + str + ").");
            }
        }
        else if (RCSettings.pvpMode != 0)
        {
            RCSettings.pvpMode = 0;
            Mod.Interface.Chat.System("Blade/AHSS PVP disabled.");
        }
        if (hash.ContainsKey("maxwave"))
        {
            if (RCSettings.maxWave != (int) hash["maxwave"])
            {
                RCSettings.maxWave = (int) hash["maxwave"];
                Mod.Interface.Chat.System("Max wave is " + RCSettings.maxWave + ".");
            }
        }
        else if (RCSettings.maxWave != 0)
        {
            RCSettings.maxWave = 0;
            Mod.Interface.Chat.System("Max wave set to default.");
        }
        if (hash.ContainsKey("endless"))
        {
            if (RCSettings.endlessMode != (int) hash["endless"])
            {
                RCSettings.endlessMode = (int) hash["endless"];
                Mod.Interface.Chat.System("Endless respawn enabled (" + RCSettings.endlessMode + " seconds).");
            }
        }
        else if (RCSettings.endlessMode != 0)
        {
            RCSettings.endlessMode = 0;
            Mod.Interface.Chat.System("Endless respawn disabled.");
        }
        if (hash.ContainsKey("motd"))
        {
            if (RCSettings.motd != (string) hash["motd"])
            {
                RCSettings.motd = (string) hash["motd"];
                Mod.Interface.Chat.System("MOTD:" + RCSettings.motd + "");
            }
        }
        else
        {
            RCSettings.motd = string.Empty;
        }
        if (hash.ContainsKey("deadlycannons"))
        {
            if (RCSettings.deadlyCannons != (int) hash["deadlycannons"])
            {
                RCSettings.deadlyCannons = (int) hash["deadlycannons"];
                Mod.Interface.Chat.System("Cannons will now kill players.");
            }
        }
        else if (RCSettings.deadlyCannons != 0)
        {
            RCSettings.deadlyCannons = 0;
            Mod.Interface.Chat.System("Cannons will no longer kill players.");
        }
        if (hash.ContainsKey("asoracing"))
        {
            if (RCSettings.racingStatic != (int) hash["asoracing"])
            {
                RCSettings.racingStatic = (int) hash["asoracing"];
                Mod.Interface.Chat.System("Racing will not restart on win.");
            }
        }
        else if (RCSettings.racingStatic != 0)
        {
            RCSettings.racingStatic = 0;
            Mod.Interface.Chat.System("Racing will restart on win.");
        }
    }

    private void SetTeam(int setting)
    {
        if (setting == 0)
        {
            name = Shelter.Profile.Name;
            ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.RCTeam, 0 },
                { PlayerProperty.Name, name }
            };
            Player.Self.SetCustomProperties(propertiesToSet);
        }
        else if (setting == 1)
        {
            ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.RCTeam, 1 }
            };
            string playerName = Shelter.Profile.Name;
            while (playerName.Contains('[') && playerName.Length >= playerName.IndexOf('[') + 8)
            {
                int index = playerName.IndexOf('[');
                playerName = playerName.Remove(index, 8);
            }
            if (!playerName.StartsWith("[00FFFF]"))
            {
                playerName = "[00FFFF]" + playerName;
            }
            this.name = playerName;
            hashtable2.Add(PlayerProperty.Name, this.name);
            Player.Self.SetCustomProperties(hashtable2);
        }
        else if (setting == 2)
        {
            ExitGames.Client.Photon.Hashtable hashtable3 = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.RCTeam, 2 }
            };
            string str2 = Shelter.Profile.Name;
            while (str2.Contains('[') && str2.Length >= str2.IndexOf('[') + 8)
            {
                int startIndex = str2.IndexOf('[');
                str2 = str2.Remove(startIndex, 8);
            }
            if (!str2.StartsWith("[FF00FF]"))
            {
                str2 = "[FF00FF]" + str2;
            }
            name = str2;
            hashtable3.Add(PlayerProperty.Name, name);
            Player.Self.SetCustomProperties(hashtable3);
        }
        else if (setting == 3)
        {
            int num3 = 0;
            int num4 = 0;
            int num5 = 1;
            foreach (Player player in PhotonNetwork.playerList)
            {
                switch (player.Properties.RCTeam)
                {
                    case 1:
                        num3++;
                        break;
                    case 2:
                        num4++;
                        break;
                }
            }
            if (num3 > num4)
            {
                num5 = 2;
            }
            SetTeam(num5);
        }
        if (setting == 0 || setting == 1 || setting == 2)
        {
            foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (obj2.GetPhotonView().isMine)
                {
                    photonView.RPC("labelRPC", PhotonTargets.All, obj2.GetPhotonView().viewID);
                }
            }
        }
    }

    public void ShowHUDInfoCenter(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoCenter");
        if (obj2 != null)
        {
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    public void ShowHUDInfoCenterADD(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoCenter");
        if (obj2 != null)
        {
            UILabel component = obj2.GetComponent<UILabel>();
            component.text = component.text + content;
        }
    }

    private void ShowHUDInfoTopCenter(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopCenter");
        if (obj2 != null)
        {
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopCenterADD(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopCenter");
        if (obj2 != null)
        {
            UILabel component = obj2.GetComponent<UILabel>();
            component.text = component.text + content;
        }
    }

    private void ShowHUDInfoTopLeft(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopLeft");
        if (obj2 != null)
        {
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopRight(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopRight");
        if (obj2 != null)
        {
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopRightMAPNAME(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopRight");
        if (obj2 != null)
        {
            UILabel component = obj2.GetComponent<UILabel>();
            component.text = component.text + content;
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
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
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
                obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
            }
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
            GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
            GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
            {
                { "dead", false }
            };
            ExitGames.Client.Photon.Hashtable propertiesToSet = hashtable;
            Player.Self.SetCustomProperties(propertiesToSet);
            hashtable = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.IsTitan, 2 }
            };
            propertiesToSet = hashtable;
            Player.Self.SetCustomProperties(propertiesToSet);
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            Screen.showCursor = true;
            ShowHUDInfoCenter(string.Empty);
        }
        else
        {
            SpawnPlayerTitanAfterGameEndRC(id);
        }
    }

    public void SpawnPlayer(string id)
    {
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
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
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
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
                        costume?.checkstat();
                        CostumeConverter.HeroCostumeToLocalData(costume, IN_GAME_MAIN_CAMERA.singleCharacter);
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
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
                        component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
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
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[index];
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(HeroCostume.costume[index].name);
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                                component.main_object.GetComponent<HERO>().setStat2();
                                component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
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
                    costume2?.checkstat();
                    CostumeConverter.HeroCostumeToLocalData(costume2, id);
                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
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
                    component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
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
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[num4];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.GetInfo(HeroCostume.costume[num4].name);
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                            component.main_object.GetComponent<HERO>().setStat2();
                            component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
                            break;
                        }
                    }
                }
                CostumeConverter.HeroCostumeToPhotonData(component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, Player.Self);
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
                {
                    Transform transform1 = component.main_object.transform;
                    transform1.position += new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20));
                }
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
                {
                    { "dead", false }
                };
                ExitGames.Client.Photon.Hashtable propertiesToSet = hashtable;
                Player.Self.SetCustomProperties(propertiesToSet);
                hashtable = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.IsTitan, 1 }
                };
                propertiesToSet = hashtable;
                Player.Self.SetCustomProperties(propertiesToSet);
            }
            component.enabled = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
            GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
            component.gameOver = false;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            Screen.showCursor = false;
            isLosing = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    private void SpawnPlayerCustomMap()
    {
        if (!needChooseSide && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            if (Player.Self.Properties.PlayerType == PlayerType.Titan)
            {
                SpawnPlayerTitan(myLastHero);
            }
            else
            {
                SpawnPlayer(myLastHero);
            }
            ShowHUDInfoCenter(string.Empty);
        }
    }

    public GameObject SpawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
    {
        GameObject obj3;
        GameObject obj2 = SpawnTitanRaw(position, rotation);
        if (punk)
        {
            obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_PUNK, false);
        }
        else if (UnityEngine.Random.Range(0, 100) < rate)
        {
            if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                if (UnityEngine.Random.Range(0f, 1f) >= 0.7f && !LevelInfoManager.GetInfo(Level).NoCrawler)
                {
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, false);
                }
                else
                {
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
        {
            if (UnityEngine.Random.Range(0f, 1f) >= 0.7f && !LevelInfoManager.GetInfo(Level).NoCrawler)
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, false);
            }
            else
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
            }
        }
        else if (UnityEngine.Random.Range(0, 100) < rate)
        {
            if (UnityEngine.Random.Range(0f, 1f) >= 0.8f && !LevelInfoManager.GetInfo(Level).NoCrawler)
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, false);
            }
            else
            {
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_I, false);
            }
        }
        else if (UnityEngine.Random.Range(0f, 1f) >= 0.8f && !LevelInfoManager.GetInfo(Level).NoCrawler)
        {
            obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, false);
        }
        else
        {
            obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
        }
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
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
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.NORMAL, false);
                    break;

                case 1:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_I, false);
                    break;

                case 2:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
                    break;

                case 3:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
                    break;

                case 4:
                    obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_PUNK, false);
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
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.NORMAL, false);
                    break;

                case 1:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_I, false);
                    break;

                case 2:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
                    break;

                case 3:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
                    break;

                case 4:
                    obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_PUNK, false);
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
            if (RCSettings.gameType == 1)
            {
                moreTitans = 3;
            }
            else if (RCSettings.gameType == 2 || RCSettings.gameType == 3)
            {
                moreTitans = 0;
            }
        }
        if (RCSettings.moreTitans > 0 || RCSettings.moreTitans == 0 && Level.StartsWith("Custom") && RCSettings.gameType >= 2)
        {
            moreTitans = RCSettings.moreTitans;
        }
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
        {
            if (punk)
            {
                moreTitans = rate;
            }
            else
            {
                int waveModeNum;
                if (RCSettings.moreTitans == 0)
                {
                    waveModeNum = 1;
                    if (RCSettings.waveModeOn == 1)
                    {
                        waveModeNum = RCSettings.waveModeNum;
                    }
                    moreTitans += (wave - 1) * (waveModeNum - 1);
                }
                else if (RCSettings.moreTitans > 0)
                {
                    waveModeNum = 1;
                    if (RCSettings.waveModeOn == 1)
                    {
                        waveModeNum = RCSettings.waveModeNum;
                    }
                    moreTitans += (wave - 1) * waveModeNum;
                }
            }
        }
        moreTitans = Math.Min(50, moreTitans);
        if (RCSettings.spawnMode == 1)
        {
            float nRate = RCSettings.nRate;
            float aRate = RCSettings.aRate;
            float jRate = RCSettings.jRate;
            float cRate = RCSettings.cRate;
            float pRate = RCSettings.pRate;
            if (punk && RCSettings.punkWaves == 1)
            {
                nRate = 0f;
                aRate = 0f;
                jRate = 0f;
                cRate = 0f;
                pRate = 100f;
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
                float num10 = UnityEngine.Random.Range(0f, 100f);
                if (num10 <= nRate + aRate + jRate + cRate + pRate)
                {
                    GameObject obj3 = SpawnTitanRaw(position, rotation);
                    if (num10 < nRate)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.NORMAL, false);
                    }
                    else if (num10 >= nRate && num10 < nRate + aRate)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_I, false);
                    }
                    else if (num10 >= nRate + aRate && num10 < nRate + aRate + jRate)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_JUMPER, false);
                    }
                    else if (num10 >= nRate + aRate + jRate && num10 < nRate + aRate + jRate + cRate)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
                    }
                    else if (num10 >= nRate + aRate + jRate + cRate && num10 < nRate + aRate + jRate + cRate + pRate)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.TYPE_PUNK, false);
                    }
                    else
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.NORMAL, false);
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
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
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
            "MainCamera_Mono",
            "PhotonMono",
            "EventSystem",
            "UIRefer",
            "MultiplayerManager",
            "InputManagerController",
            //"UI Root",
            //"Camera",
            "MenuBackGround", // Without this you can't join rooms.
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
            ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
            {
                { PlayerProperty.Kills, numArray[0] },
                { PlayerProperty.Deaths, numArray[1] },
                { PlayerProperty.MaxDamage, numArray[2] },
                { PlayerProperty.TotalDamage, numArray[3] }
            };
            player.SetCustomProperties(propertiesToSet);
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