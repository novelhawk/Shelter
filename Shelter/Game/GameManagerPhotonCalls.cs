using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Enums;
using JetBrains.Annotations;
using Mod;
using Mod.Discord;
using Mod.Interface;
using Mod.Keybinds;
using Mod.Managers;
using NGUI.Internal;
using Photon;
using Photon.Enums;
using RC;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// ReSharper disable once CheckNamespace
public partial class GameManager
{
    #region Unity & Photon methods

    [UsedImplicitly]
    public void OnConnectionFail(DisconnectCause cause)
    {
        Shelter.LogBoth("Failed to connect: {0}", cause);
        
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.NotInRoom;
        gameStart = false;
    }

    [UsedImplicitly]
    public void OnCreatedRoom()
    {
        racingResult = new List<RacingResult>();
        teamScores = new int[2];
    }

    [UsedImplicitly]
    public void OnDisconnectedFromPhoton()
    {
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    [UsedImplicitly]
    public void OnJoinedLobby()
    {
        Loading.Stop();
        ServerList.Alpha = 0f;
    }

    [UsedImplicitly]
    public void OnJoinedRoom()
    {
        Shelter.OnJoinedGame();
        DiscordRpc.SendInGameMulti();
        var room = PhotonNetwork.Room;
        Shelter.Chat.System("Joined {0}", room.Name.RemoveColors());
        Shelter.LogConsole("Joined room {0}",  room.Name.RemoveColors());
        maxPlayers = room.MaxPlayers;
        playerList = string.Empty;
        gameTimesUp = false;
        Level = room.Map.Name;
        IN_GAME_MAIN_CAMERA.difficulty = (int) room.Difficulty;
        IN_GAME_MAIN_CAMERA.Daylight = room.Daylight;
        time = room.Time * 60;

        IN_GAME_MAIN_CAMERA.GameMode = room.Map.Gamemode;
        PhotonNetwork.LoadLevel(room.Map.LevelName);
//        currentLevel = string.Empty;
        Player.Self.SetCustomProperties(new Hashtable
        {
            {PlayerProperty.Name, Shelter.Profile.Name},
            {PlayerProperty.Guild, Shelter.Profile.Guild},
            {PlayerProperty.Kills, 0},
            {PlayerProperty.MaxDamage, 0},
            {PlayerProperty.TotalDamage, 0},
            {PlayerProperty.Deaths, 0},
            {PlayerProperty.Dead, true},
            {PlayerProperty.IsTitan, 0},
            {PlayerProperty.RCTeam, 0},
            {PlayerProperty.CurrentLevel, string.Empty},
            {PlayerProperty.Shelter, "http://github.com/ITALIA195/Shelter"},
            {PlayerProperty.UUID, nameof(NotImplementedException)}
        });
        humanScore = 0;
        titanScore = 0;
        PVPtitanScore = 0;
        PVPhumanScore = 0;
        wave = 1;
        highestwave = 1;
        localRacingResult = string.Empty;
        needChooseSide = true;
        killInfoGO = new ArrayList();
        //        InRoomChat.messages = new List<string>();
        if (!PhotonNetwork.isMasterClient)
            photonView.RPC(Rpc.RequireStatus, PhotonTargets.MasterClient);
        assetCacheTextures = new Dictionary<string, Texture2D>();
        isFirstLoad = true;
        if (OnPrivateServer)
            ServerRequestAuthentication(PrivateServerAuthPass);
    }

    public void OnLeftRoom()
    {
        Shelter.Chat.Edit(_endingMessageId, "Restart aborted: You left the room");
        _endingMessageId = null;
        Shelter.Chat.Edit(_endingMessageId, "Racing aborted: You left the room");
        _racingMessageId = null;
        Shelter.Chat.Edit(_endingMessageId, "Pause aborted: You left the room");
        _pauseMessageId = null;
        
        Shelter.OnMainMenu();

        if (Application.loadedLevel != 0)
        {
            Time.timeScale = 1f;
            PhotonNetwork.Disconnect();

            ResetSettings(true);
            LoadConfig();
            IN_GAME_MAIN_CAMERA.GameType = GameType.NotInRoom;
            gameStart = false;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            _isVisible = false;
            DestroyAllExistingCloths();
            Destroy(GameObject.Find("MultiplayerManager"));
            Application.LoadLevel("menu");
        }
    }

    private void OnLevelWasLoaded(int levelId) //TODO: todo 
    {
        if (levelId != 0 && Application.loadedLevelName != "characterCreation" &&
            Application.loadedLevelName != "SnapShot")
        {
            ChangeQuality.LoadFromPlayerPrefs();
            foreach (GameObject titan in GameObject.FindGameObjectsWithTag("titan"))
            {
                if (!(titan.GetPhotonView() != null && titan.GetPhotonView().owner.IsMasterClient))
                {
                    Destroy(titan);
                }
            }

            isWinning = false;
            gameStart = true;
            GameObject obj3 = (GameObject) Instantiate(Resources.Load("MainCamera_mono"),
                GameObject.Find("cameraDefaultPosition").transform.position,
                GameObject.Find("cameraDefaultPosition").transform.rotation);
            Destroy(GameObject.Find("cameraDefaultPosition"));
            obj3.name = "MainCamera";
            Screen.lockCursor = true;
            Screen.showCursor = true;
            //TODO: Remove UI_IN_GAME
            ui = (GameObject) Instantiate(Resources.Load("UI_IN_GAME"));
            ui.name = "UI_IN_GAME";
            ui.SetActive(true);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], true);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
            LevelInfo info = LevelInfoManager.Get(Level);
            Cache();
            LoadMapCustom();
            //TODO: Remove SetInterfacePosition
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetDayLight(IN_GAME_MAIN_CAMERA.Daylight);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
                Camera.main.GetComponent<SpectatorMovement>().disable = true;
                Camera.main.GetComponent<MouseLook>().disable = true;
                IN_GAME_MAIN_CAMERA.GameMode = info.Gamemode;
                SpawnPlayer(IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper());
                if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
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
                PVPcheckPoint.checkPoints = new List<PVPcheckPoint>();
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = false;
//                UnityEngine.Camera.main.GetComponent<CameraShake>().enabled = false;
                IN_GAME_MAIN_CAMERA.GameType = GameType.Multiplayer;
                if (info.Gamemode == GameMode.Trost)
                {
                    GameObject.Find(PlayerRespawnTag).SetActive(false);
                    Destroy(GameObject.Find(PlayerRespawnTag));
                    GameObject.Find("rock").animation["lift"].speed = 0f;
                    GameObject.Find("door_fine").SetActive(false);
                    GameObject.Find("door_broke").SetActive(true);
                    Destroy(GameObject.Find("ppl"));
                }
                else if (info.Gamemode == GameMode.BossFight)
                {
                    GameObject.Find("playerRespawnTrost").SetActive(false);
                    Destroy(GameObject.Find("playerRespawnTrost"));
                }

                if (needChooseSide)
                {
                    Notify.New("Join the game", "Press 1 to join the game", 2.5f, 50f);
                }
                else if (!settings.InSpectatorMode)
                {
                    if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
                    {
                        Screen.lockCursor = true;
                    }
                    else
                    {
                        Screen.lockCursor = false;
                    }

                    switch (IN_GAME_MAIN_CAMERA.GameMode)
                    {
                        case GameMode.PvpCapture when Player.Self.Properties.PlayerType == PlayerType.Titan:
                            checkpoint = GameObject.Find("PVPchkPtT");
                            break;
                        case GameMode.PvpCapture:
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

                if (info.Gamemode == GameMode.BossFight)
                {
                    Destroy(GameObject.Find("rock"));
                }

                if (PhotonNetwork.isMasterClient)
                {
                    if (info.Gamemode == GameMode.Trost)
                    {
                        if (IsAnyPlayerAlive())
                        {
                            PhotonNetwork
                                .Instantiate("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f),
                                    Quaternion.Euler(0f, 180f, 0f), 0).GetComponent<TITAN_EREN>().rockLift = true;
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
                    else if (info.Gamemode == GameMode.BossFight)
                    {
                        if (IsAnyPlayerAlive())
                        {
                            PhotonNetwork.Instantiate("COLOSSAL_TITAN", -Vector3.up * 10000f,
                                Quaternion.Euler(0f, 180f, 0f), 0);
                        }
                    }
                    else if (info.Gamemode == GameMode.KillTitan || info.Gamemode == GameMode.EndlessTitan ||
                             info.Gamemode == GameMode.SurviveMode)
                    {
                        if (info.Name == "Annie" || info.Name == "Annie II")
                        {
                            PhotonNetwork.Instantiate("FEMALE_TITAN",
                                GameObject.Find("titanRespawn").transform.position,
                                GameObject.Find("titanRespawn").transform.rotation, 0);
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
                    else if (info.Gamemode != GameMode.Trost && info.Gamemode == GameMode.PvpCapture &&
                             info.LevelName == "OutSide")
                    {
                        GameObject[] objArray3 = GameObject.FindGameObjectsWithTag("titanRespawn");
                        if (objArray3.Length <= 0)
                        {
                            return;
                        }

                        foreach (GameObject obj in objArray3)
                        {
                            SpawnTitanRaw(obj.transform.position, obj.transform.rotation).GetComponent<TITAN>()
                                .setAbnormalType2(AbnormalType.Crawler, true);
                        }
                    }
                }

                if (!info.Supply)
                {
                    Destroy(GameObject.Find("aot_supply"));
                }

                if (!PhotonNetwork.isMasterClient)
                {
                    photonView.RPC(Rpc.RequireStatus, PhotonTargets.MasterClient);
                }

                if (info.IsLava)
                {
                    Instantiate(Resources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f),
                        Quaternion.Euler(0f, 0f, 0f));
                    GameObject.Find("aot_supply").transform.position =
                        GameObject.Find("aot_supply_lava_position").transform.position;
                    GameObject.Find("aot_supply").transform.rotation =
                        GameObject.Find("aot_supply_lava_position").transform.rotation;
                }

                if (settings.InSpectatorMode)
                    EnterSpecMode(true);
            }
        }
    }

    [UsedImplicitly]
    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!noRestart)
        {
            if (PhotonNetwork.isMasterClient)
            {
                restartingMC = true;
                if (settings.IsInfectionMode)
                {
                    restartingTitan = true;
                }

                if (settings.IsBombMode)
                {
                    restartingBomb = true;
                }

                if (settings.EnableHorse)
                {
                    restartingHorse = true;
                }

                if (!settings.AllowErenTitan)
                {
                    restartingEren = true;
                }
            }

            ResetSettings(false);
            if (!LevelInfoManager.Get(Level).PlayerTitansNotAllowed)
            {
                Player.Self.SetCustomProperties(new Hashtable
                {
                    {PlayerProperty.IsTitan, 1}
                });
            }

            if (!(gameTimesUp || !PhotonNetwork.isMasterClient))
            {
                RestartGame(true);
                photonView.RPC(Rpc.SetMasterclient, PhotonTargets.All);
            }
        }

        noRestart = false;
    }

    [UsedImplicitly]
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

    [UsedImplicitly]
    public void OnPhotonPlayerConnected(Player player)
    {
        Shelter.LogConsole("{0} connected", player.ToString().RemoveColors());
        DiscordRpc.SendInGameMulti();
        
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView photonView1 = photonView;
            if (banHash.ContainsValue(player.Properties.Name))
            {
                KickPlayerRC(player, false, "banned.");
                return;
            }

            if (!Utility.ValidateHEROStats(player.Properties))
            {
                KickPlayerRC(player, true, "excessive stats.");
                return;
            }

            if (settings.AsoPreserveKDA)
            {
                StartCoroutine(WaitAndReloadKDR(player));
            }

            if (Level.StartsWith("Custom"))
            {
                StartCoroutine(CustomLevelEnumerator(new List<Player> {player}));
            }

            object[] ignores = PhotonNetwork.PlayerList.Where(x => x.IsIgnored).Cast<object>().ToArray();
            if (ignores.Length > 0)
                photonView1.RPC(Rpc.IgnoreMultiple, player, ignores);

            photonView1.RPC(Rpc.Settings, player, GameSettingsManager.EncodeToHashtable(settings));
            photonView1.RPC(Rpc.SetMasterclient, player);
            if (Time.timeScale <= 0.1f && pauseWaitTime > 3f)
            {
                photonView1.RPC(Rpc.Pause, player, true);
                Shelter.Chat.SendMessage("MasterClient has paused the game", string.Empty, player);
            }
        }
    }

    [UsedImplicitly]
    public void OnPhotonPlayerDisconnected(Player player)
    {
        Shelter.LogConsole("{0} disconnected!", player.ToString().RemoveColors());
        DiscordRpc.SendInGameMulti();
        
        if (!gameTimesUp)
        {
            OneTitanDown(string.Empty, true);
            SomeOneIsDead(0);
        }

        InstantiateTracker.instance.TryRemovePlayer(player.ID);
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC(Rpc.CheckPlayerPresence, PhotonTargets.All, player.ID);
        }

        if (settings.AsoPreserveKDA)
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

    [UsedImplicitly]
    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) //TODO: Add anti name change
    {
        if (playerAndUpdatedProps == null || 
            playerAndUpdatedProps.Length < 2 || 
            !(playerAndUpdatedProps[0] is Player player) || 
            !(playerAndUpdatedProps[1] is Hashtable hashtable))
            return;
        
        if (player.Hero != null && (hashtable.ContainsKey(PlayerProperty.Name) || hashtable.ContainsKey(PlayerProperty.Guild)))
            player.Hero.UpdateName(player.Properties);

        if (!player.IsLocal)
            return;
        
        if (hashtable.ContainsKey(PlayerProperty.Acceleration) || 
            hashtable.ContainsKey(PlayerProperty.Blade) ||
            hashtable.ContainsKey(PlayerProperty.Gas) || 
            hashtable.ContainsKey(PlayerProperty.Speed))
        {
            if (!Utility.ValidateHEROStats(player.Properties))
            {
                player.SetCustomProperties(new Hashtable
                {
                    [PlayerProperty.Acceleration] = 100,
                    [PlayerProperty.Blade] = 100,
                    [PlayerProperty.Gas] = 100,
                    [PlayerProperty.Speed] = 100
                });
            }
        }
    }

    private void Update()
    {
        if (Shelter.InputManager.IsDown(InputAction.OpenSettingsMenu))
            _isVisible = !_isVisible;
        
        if (gameStart)
        {
            foreach (HERO hero in Heroes)
                if (hero != null)
                    hero.DoUpdate();
            
            foreach (Bullet bullet in Hooks)
                if (bullet != null)
                    bullet.DoUpdate();

            foreach (TITAN_EREN et in ErenTitans)
                if (et != null)
                    et.DoUpdate();

            foreach (TITAN titan in Titans)
                if (titan != null)
                    titan.DoUpdate();

            foreach (FEMALE_TITAN ft in FemaleTitans)
                if (ft != null)
                    ft.DoUpdate();

            foreach (COLOSSAL_TITAN ct in ColossalTitans)
                if (ct != null)
                    ct.DoUpdate();

            if (IN_GAME_MAIN_CAMERA.instance != null)
            {
                IN_GAME_MAIN_CAMERA.instance.DoUpdate();
                IN_GAME_MAIN_CAMERA.instance.SnapshotUpdate();
            }
        }
    }

    private void Start()
    {
        instance = this;
        gameObject.name = "MultiplayerManager";
        DontDestroyOnLoad(gameObject);

        if (privateServerField == null)
        {
            privateServerField = string.Empty;
        }

        ResetGameSettings();
        banHash = new Hashtable();
        _infectionTitans = new Hashtable();
        oldScript = string.Empty;
        currentLevel = string.Empty;
        if (currentScript == null)
            currentScript = string.Empty;

        titanSpawns = new List<Vector3>();
        playerSpawnsC = new List<Vector3>();
        playerSpawnsM = new List<Vector3>();
        playersRPC = new List<Player>();
        levelCache = new List<string[]>();
        titanSpawners = new List<TitanSpawner>();
        restartCount = new List<float>();
        
        groundList = new List<GameObject>();
        noRestart = false;
        masterRC = false;
        isSpawning = false;
        intVariables = new Hashtable();
        heroHash = new Hashtable();
        boolVariables = new Hashtable();
        stringVariables = new Hashtable();
        floatVariables = new Hashtable();
        globalVariables = new Hashtable();
        RCRegions = new Hashtable();
        RCEvents = new Hashtable();
        RCVariableNames = new Hashtable();
        RCRegionTriggers = new Hashtable();
        playerVariables = new Hashtable();
        titanVariables = new Hashtable();
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
        ChangeQuality.LoadFromPlayerPrefs();

        DestroyOldMenu();
    }

    private void LateUpdate()
    {
        if (!gameStart) 
            return;
        
        foreach (HERO hero in Heroes)
            if (hero != null)
                hero.DoLateUpdate();
            
        foreach (TITAN_EREN eren in ErenTitans)
            if (eren != null)
                eren.DoLateUpdate();

        foreach (TITAN titan in Titans)
            if (titan != null)
                titan.DoLateUpdate();
            
        foreach (FEMALE_TITAN ft in FemaleTitans)
            if (ft != null)
                ft.DoLateUpdate();
            
        Core();
    }

    #endregion
}