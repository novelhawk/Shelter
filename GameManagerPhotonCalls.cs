using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Mod;
using Mod.Interface;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public partial class FengGameManagerMKII
{
    #region Unity & Photon methods

    public void OnConnectionFail(DisconnectCause cause)
    {
        //TODO: Log
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.NotInRoom;
        gameStart = false;
    }

    public void OnCreatedRoom()
    {
        racingResult = new ArrayList();
        teamScores = new int[2];
    }

    public void OnDisconnectedFromPhoton()
    {
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    public void OnJoinedLobby()
    {
        Loading.Stop("ConnectingToLobby");
        ServerList.Alpha = 0f;
    }

    public void OnJoinedRoom()
    {
        Shelter.OnJoinedGame();
        var room = PhotonNetwork.Room;
        Mod.Interface.Chat.System("Joined " + room.Name.RemoveColors());
        maxPlayers = room.MaxPlayers;
        playerList = string.Empty;
        gameTimesUp = false;
        Level = room.Map.Name;
        IN_GAME_MAIN_CAMERA.difficulty = (int) room.Difficulty;
        IN_GAME_MAIN_CAMERA.dayLight = room.DayLight;
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
            {PlayerProperty.CurrentLevel, string.Empty}
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
            photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
        assetCacheTextures = new Dictionary<string, Texture2D>();
        isFirstLoad = true;
        if (OnPrivateServer)
            ServerRequestAuthentication(PrivateServerAuthPass);
    }

    public void OnLeftRoom()
    {
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
            inputManager.menuOn = false;
            DestroyAllExistingCloths();
            Destroy(GameObject.Find("MultiplayerManager"));
            Application.LoadLevel("menu");
        }
    }

    public void OnUpdate()
    {
//        if (RCEvents.ContainsKey("OnUpdate"))
//        {
//            if (updateTime > 0f)
//            {
//                updateTime -= Time.deltaTime;
//            }
//            else
//            {
//                ((RCEvent) RCEvents["OnUpdate"]).checkEvent();
//                updateTime = 1f;
//            }
//        }
    }

    private void OnLevelWasLoaded(int levelId) //TODO: todo 
    {
        if (levelId != 0 && Application.loadedLevelName != "characterCreation" &&
            Application.loadedLevelName != "SnapShot")
        {
            ChangeQuality.setCurrentQuality();
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
            LevelInfo info = LevelInfoManager.GetInfo(Level);
            Cache();
            LoadMapCustom();
            //TODO: Remove SetInterfacePosition
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetDayLight(IN_GAME_MAIN_CAMERA.dayLight);
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
                PVPcheckPoint.checkPoints = new ArrayList();
                UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = false;
                UnityEngine.Camera.main.GetComponent<CameraShake>().enabled = false;
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
                    Notify.New("Join the game", "Press 1 to join the game", 2500, 50f);
                }
                else if ((int) settings[245] == 0)
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
                        if (!IsAnyPlayerAlive())
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
                        if (!IsAnyPlayerAlive())
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
                                .setAbnormalType2(AbnormalType.TYPE_CRAWLER, true);
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
                    Instantiate(Resources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f),
                        Quaternion.Euler(0f, 0f, 0f));
                    GameObject.Find("aot_supply").transform.position =
                        GameObject.Find("aot_supply_lava_position").transform.position;
                    GameObject.Find("aot_supply").transform.rotation =
                        GameObject.Find("aot_supply_lava_position").transform.rotation;
                }

                if ((int) settings[245] == 1)
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
                    {PlayerProperty.IsTitan, 1}
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
                if (player.Properties.Acceleration > 150 || player.Properties.Blade > 125 ||
                    player.Properties.Gas > 150 || player.Properties.Speed > 140)
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
                    StartCoroutine(CustomLevelEnumerator(new List<Player> {player}));
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
        if (playerAndUpdatedProps == null || playerAndUpdatedProps.Length < 2 || !(playerAndUpdatedProps[1] is Hashtable hashtable))
            return;
        
        Player player = playerAndUpdatedProps[0] as Player;
        if (player == null)
            return;

        if (player.Hero != null)
            player.Hero.UpdateName(player.Properties.Name);
            
        if (player != Player.Self) 
            return;

//        if (hashtable.ContainsKey(PlayerProperty.Name) && Shelter.Profile.Name != (string) hashtable[PlayerProperty.Name])
//        {
//            Player.Self.SetCustomProperties(new Hashtable
//            {
//                {PlayerProperty.Name, Shelter.Profile.Name}
//            });
//        } TODO: Add anti name change

        if (hashtable.ContainsKey("statACL") || hashtable.ContainsKey("statBLA") ||
            hashtable.ContainsKey("statGAS") || hashtable.ContainsKey("statSPD"))
        {
            int num = player.Properties.Acceleration;
            int num2 = player.Properties.Blade;
            int num3 = player.Properties.Gas;
            int num4 = player.Properties.Speed;
            if (num > 150)
            {
                player.SetCustomProperties(new Hashtable
                {
                    {PlayerProperty.Acceleration, 100}
                });
            }

            if (num2 > 125)
            {
                player.SetCustomProperties(new Hashtable
                {
                    {PlayerProperty.Blade, 100}
                });
            }

            if (num3 > 150)
            {
                player.SetCustomProperties(new Hashtable
                {
                    {PlayerProperty.Gas, 100}
                });
            }

            if (num4 > 140)
            {
                player.SetCustomProperties(new Hashtable
                {
                    {PlayerProperty.Speed, 100}
                });
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
        ignoreList = new List<int>();
        gameObject.name = "MultiplayerManager";
        HeroCostume.init2();
        CharacterMaterials.init();
        DontDestroyOnLoad(gameObject);

        if (privateServerField == null)
        {
            privateServerField = string.Empty;
        }

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