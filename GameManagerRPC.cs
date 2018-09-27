using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using ExitGames.Client.Photon;
using Mod;
using Mod.Exceptions;
using UnityEngine;

public partial class FengGameManagerMKII
{
    public Dictionary<int, CannonValues> allowedToCannon;
    public Dictionary<string, Texture2D> assetCacheTextures;
    public InRoomChat chatRoom;
    public GameObject checkpoint;
    public int cyanKills;
    public int difficulty;
    public float distanceSlider;
    public bool gameStart;
    public List<GameObject> groundList;
    public bool isFirstLoad;
    public bool isPlayer1Winning;
    public bool isPlayer2Winning;
    public bool isRecompiling;
    public bool isRestarting;
    public bool isSpawning;
    public bool isUnloading;
    public bool justSuicide;
    public List<string[]> levelCache;
    public int magentaKills;
    public int maxPlayers;
    public float mouseSlider;
    public float myRespawnTime;
    public bool needChooseSide;
    public float pauseWaitTime;
    public string playerList;
    public List<Vector3> playerSpawnsC;
    public List<Vector3> playerSpawnsM;
    public List<Player> playersRPC;
    public Dictionary<string, int[]> PreservedPlayerKDR;
    public int PVPhumanScore;
    public int PVPtitanScore;
    public float qualitySlider;
    public List<GameObject> racingDoors;
    public Vector3 racingSpawnPoint;
    public bool racingSpawnPointSet;
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
    public List<GameObject> spectateSprites;
    public Texture2D textureBackgroundBlack;
    public Texture2D textureBackgroundBlue;
    public int time = 600;
    public List<TitanSpawner> titanSpawners;
    public List<Vector3> titanSpawns;
    public float transparencySlider;
    public float updateTime;
    public int wave = 1;

    [RPC]
    private void RequireStatus(PhotonMessageInfo info)
    {
        if (!Player.Self.IsMasterClient && PhotonNetwork.playerList.Any(x => x.ID < Player.Self.ID))
            throw new NotAllowedException(nameof(RequireStatus), info);
        
        photonView.RPC("refreshStatus", PhotonTargets.Others, humanScore, titanScore, wave, highestwave, roundTime, timeTotalServer, startRacing, endRacing);
        photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
        photonView.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, teamScores);
    }

    [RPC]
    private void RefreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(RefreshStatus), info);
        
        humanScore = score1;
        titanScore = score2;
        wave = wav;
        highestwave = highestWav;
        roundTime = time1;
        timeTotalServer = time2;
        startRacing = startRacin;
        endRacing = endRacin;
        if (startRacing && Shelter.TryFind("door", out GameObject door))
            door.SetActive(false);
    }

    [RPC]
    private void RefreshPVPStatus(int score1, int score2, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient && IN_GAME_MAIN_CAMERA.GameMode != GameMode.PvpCapture)
            throw new NotAllowedException(nameof(RefreshPVPStatus), info);
        
        PVPhumanScore = score1;
        PVPtitanScore = score2;
    }

    [RPC]
    private void RefreshPVPStatus_AHSS(int[] score1, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(RefreshStatus), info);
        
        teamScores = score1;
    }

    [RPC]
    public void PauseRPC(bool pause, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(PauseRPC), info);
        
        if (pause)
        {
            pauseWaitTime = 100000f;
            _timeSincePause = 0f;
            Time.timeScale = 1E-06f;
            _pauseMessageId = Mod.Interface.Chat.SendMessage($"{info.sender.Properties.HexName} paused the game.");
        }
        else
        {
            pauseWaitTime = 3f;
            if (!_pauseMessageId.HasValue)
                Mod.Interface.Chat.SendMessage($"{info.sender.Properties.HexName} unpaused the game.");
            else
                Mod.Interface.Chat.EditMessage(_pauseMessageId, $"{info.sender.Properties.HexName} unpaused the game.", false);
        }
    }

    [RPC]
    private void LabelRPC(int viewId, PhotonMessageInfo info)
    {
        if (PhotonView.TryParse(viewId, out PhotonView view))
        {
            if (info.sender != view.owner)
                throw new NotAllowedException(nameof(LabelRPC), info);
            
            // This is already done automatically in HERO
        }
    }

    [RPC]
    public void OneTitanDown(string titanType, bool onPlayerLeave)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || PhotonNetwork.isMasterClient)
        {
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
            {
                if (titanType != string.Empty)
                {
                    switch (titanType)
                    {
                        case "Titan":
                            PVPhumanScore++;
                            break;
                        case "Aberrant":
                            PVPhumanScore += 2;
                            break;
                        case "Jumper":
                            PVPhumanScore += 3;
                            break;
                        case "Crawler":
                            PVPhumanScore += 4;
                            break;
                        case "Female Titan":
                            PVPhumanScore += 10;
                            break;
                        default:
                            PVPhumanScore += 3;
                            break;
                    }
                }

                CheckPVPPoints();
                object[] parameters = {PVPhumanScore, PVPtitanScore};
                photonView.RPC("refreshPVPStatus", PhotonTargets.Others, parameters);
            }
            else if (IN_GAME_MAIN_CAMERA.GameMode != GameMode.CaveFight)
            {
                switch (IN_GAME_MAIN_CAMERA.GameMode)
                {
                    case GameMode.KillTitan:
                        if (!IsAnyTitanAlive)
                        {
                            GameWin();
                            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                        }

                        break;
                    case GameMode.SurviveMode:
                        if (!IsAnyTitanAlive)
                        {
                            wave++;
                            if (!(LevelInfoManager.GetInfo(Level).RespawnMode != RespawnMode.NEWROUND &&
                                  (!Level.StartsWith("Custom") || RCSettings.gameType != 1) ||
                                  IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer))
                            {
                                foreach (Player player in PhotonNetwork.playerList)
                                {
                                    if (player.Properties.PlayerType != PlayerType.Titan)
                                    {
                                        photonView.RPC("respawnHeroInNewRound", player);
                                    }
                                }
                            }

                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                            {
                                SendChatContentInfo("<color=#A8FF24>Wave : " + wave + "</color>");
                            }

                            if (wave > highestwave)
                                highestwave = wave;

                            if (PhotonNetwork.isMasterClient)
                                RequireStatus(null);

                            if (!((RCSettings.maxWave != 0 || wave <= 20) && (RCSettings.maxWave <= 0 || wave <= RCSettings.maxWave)))
                            {
                                GameWin();
                            }
                            else
                            {
                                int abnormal = 90;
                                if (difficulty == 1)
                                    abnormal = 70;

                                if (!LevelInfoManager.GetInfo(Level).HasPunk)
                                    SpawnTitanCustom("titanRespawn", abnormal, wave + 2, false);
                                else if (wave == 5)
                                    SpawnTitanCustom("titanRespawn", abnormal, 1, true);
                                else if (wave == 10)
                                    SpawnTitanCustom("titanRespawn", abnormal, 2, true);
                                else if (wave == 15)
                                    SpawnTitanCustom("titanRespawn", abnormal, 3, true);
                                else if (wave == 20)
                                    SpawnTitanCustom("titanRespawn", abnormal, 4, true);
                                else
                                    SpawnTitanCustom("titanRespawn", abnormal, wave + 2, false);
                            }
                        }

                        break;
                    case GameMode.EndlessTitan:
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
                        break;
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
        }
    }

    [RPC]
    private void NetGameLose(int score, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient && !info.sender.IsLocal)
            throw new NotAllowedException(nameof(NetGameLose), info, false); //TODO: Change to true when sure it doesn't get called by the game

        
        isLosing = true;
        titanScore = score;
        gameEndCD = gameEndTotalCDtime;
        _endingMessageId = Mod.Interface.Chat.System("Game is restarting soon.");
        if ((int) settings[244] == 1)
        {
            Mod.Interface.Chat.System($"<color=#FFC000>({roundTime:F2})</color> Round ended (game lose).");
        }
    }

    [RPC]
    private void RestartGameByClient(PhotonMessageInfo info)
    {
        if (!Player.Self.IsMasterClient)
            throw new NotAllowedException(nameof(RestartGameByClient), info);    
    }

    [RPC]
    private void NetGameWin(int score, PhotonMessageInfo info)
    {
        humanScore = score;
        isWinning = true;
        switch (IN_GAME_MAIN_CAMERA.GameMode)
        {
            case GameMode.PvpAHSS:
                teamWinner = score;
                teamScores[teamWinner - 1]++;
                gameEndCD = gameEndTotalCDtime;
                break;
            case GameMode.Racing when RCSettings.racingStatic == 1:
                gameEndCD = 1000f;
                break;
            case GameMode.Racing:
                gameEndCD = 20f;
                break;
            default:
                gameEndCD = gameEndTotalCDtime;
                break;
        }
        _endingMessageId = Mod.Interface.Chat.System("Game is restarting soon.");

        if ((int) settings[244] == 1)
        {
            Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") +
                                      ")</color> Round ended (game win).");
        }

        if (!(Equals(info.sender, PhotonNetwork.masterClient) || info.sender.IsLocal))
        {
            Mod.Interface.Chat.System("Round end sent from Player " + info.sender.ID);
        }
    }

    [RPC]
    public void SomeOneIsDead(int id = -1)
    {
        switch (IN_GAME_MAIN_CAMERA.GameMode)
        {
            case GameMode.PvpCapture:
                if (id != 0)
                {
                    PVPtitanScore += 2;
                }

                CheckPVPPoints();
                object[] parameters = {PVPhumanScore, PVPtitanScore};
                photonView.RPC("refreshPVPStatus", PhotonTargets.Others, parameters);
                break;
            
            case GameMode.EndlessTitan:
                titanScore++;
                break;
            
            case GameMode.KillTitan:
            case GameMode.SurviveMode:
            case GameMode.BossFight:
            case GameMode.Trost:
                if (!IsAnyPlayerAlive())
                    GameLose();
                break;
            
            case GameMode.PvpAHSS when RCSettings.pvpMode == 0 && RCSettings.bombMode == 0:
                if (!IsAnyPlayerAlive())
                {
                    GameLose();
                    teamWinner = 0;
                    return;
                }

                if (IsAnyTeamMemberAlive(1))
                {
                    teamWinner = 2;
                    GameWin();
                    return;
                }

                if (IsAnyTeamMemberAlive(2))
                {
                    teamWinner = 1;
                    GameWin();
                }
                break;
        }
    }

    [RPC]
    private void NetRefreshRacingResult(string tmp)
    {
        localRacingResult = tmp;
    }

    [RPC]
    public void NetShowDamage(int speed, PhotonMessageInfo info)
    {
        if (info != null && !info.sender.IsMasterClient && !info.sender.IsLocal)
            throw new NotAllowedException(nameof(NetShowDamage), info, false);
            
        if (Shelter.TryFind("Stylish", out GameObject obj))
            obj.GetComponent<StylishComponent>().Style(speed);
        
        if (Shelter.TryFind("LabelScore", out obj))
        {
            obj.GetComponent<UILabel>().text = speed.ToString();
            obj.transform.localScale = Vector3.zero;
            speed = (int) (speed * 0.1f);
            speed = Mathf.Max(40, speed);
            speed = Mathf.Min(150, speed);
            iTween.Stop(obj); 
                        
            iTween.ScaleTo(obj, new System.Collections.Hashtable
            {
                {"x", speed},
                {"y", speed},
                {"z", speed},
                {"easetype", iTween.EaseType.easeOutElastic},
                {"time", 1f},
            });
            iTween.ScaleTo(obj, new System.Collections.Hashtable
            {
                {"x", 0},
                {"y", 0},
                {"z", 0},
                {"easetype", iTween.EaseType.easeInBounce},
                {"time", 0.5f},
                {"delay", 2f},
            });
        }
    }

    [RPC]
    private void LoadskinRPC(string n, string url, string url2, string[] skybox, PhotonMessageInfo info)
    {
        if ((int) settings[2] == 1 && info.sender.IsMasterClient)
        {
            StartCoroutine(LoadSkinEnumerator(n, url, url2, skybox));
        }
    }

    private float _lastMapLoad;
    
    [RPC]
    private void RPCLoadLevel(PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient && PhotonNetwork.isMasterClient) // PhotonNetwork.isMasterClient is needed I think
            throw new NotAllowedException(nameof(RPCLoadLevel), info);
            
        if (info.sender.IsMasterClient)
        {
            DestroyAllExistingCloths();
            if (!string.IsNullOrEmpty(PhotonNetwork.Room.Map.LevelName))
                PhotonNetwork.LoadLevel(PhotonNetwork.Room.Map.LevelName);
        }
        else if (!masterRC)
        {
            if (Time.time - _lastMapLoad > 60f) 
            {
                DestroyAllExistingCloths();
                if (!string.IsNullOrEmpty(PhotonNetwork.Room.Map.LevelName))
                    PhotonNetwork.LoadLevel(PhotonNetwork.Room.Map.LevelName);
                
                _lastMapLoad = Time.time;
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
                TargetActors = new[] {id}
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
                    TargetActors = new[] {id}
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
                Hashtable propertiesToSet = new Hashtable
                {
                    {PlayerProperty.CurrentLevel, string.Empty}
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
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.KillTitan;
            }
            else if (gametype == 1)
            {
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.SurviveMode;
            }
            else if (gametype == 2)
            {
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.PvpAHSS;
            }
            else if (gametype == 3)
            {
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.Racing;
            }
            else if (gametype == 4)
            {
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.None;
            }

            if (info.sender.IsMasterClient && link.Length > 6)
            {
                StartCoroutine(ClearLevelEnumerator(link));
            }
        }
    }

    [RPC]
    private void ShowResult(string t1, string t2, string t3, string t4, string t5, string t6, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(ShowResult), info);
        throw new NotAllowedException(nameof(ShowResult), info, false);
    }

    [RPC]
    private void SpawnTitanRPC(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            foreach (TITAN titan in Titans)
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
    public void Chat(string content, string sender, PhotonMessageInfo info)
    {
//        Shelter.EventManager.Fire(nameof(Chat));
        if (string.IsNullOrEmpty(content))
            return;
        Shelter.LogConsole("Received a message from {0}.", info.sender);
        if (sender != string.Empty)
            Mod.Interface.Chat.AddMessage($"{info.sender.Properties.HexName}: {content}", info.sender);
        else 
            Mod.Interface.Chat.AddMessage(content, info.sender);
    }

    [RPC]
    private void SetTeamRPC(int setting, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient || info.sender.IsLocal)
        {
            SetTeam(setting);
        }
    }

    [RPC]
    private void SettingRPC(Hashtable hash, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient) 
            throw new NotAllowedException(nameof(SettingRPC), info);
        
        SetGameSettings(hash);
    }

    [RPC]
    private void ShowChatContent(string content, PhotonMessageInfo info) 
    {
        throw new NotAllowedException(nameof(ShowChatContent), info);
    }

    [RPC]
    private void UpdateKillInfo(bool t1, string killer, bool t2, string victim, int dmg)
    {
        GameObject obj2 = GameObject.Find("UI_IN_GAME");
        GameObject obj3 = (GameObject) Instantiate(Resources.Load("UI/KillInfo"));
        foreach (GameObject t in killInfoGO)
        {
            if (t != null)
                t.GetComponent<KillInfoComponent>().MoveOn();
        }

        if (killInfoGO.Count > 4)
        {
            GameObject obj4 = (GameObject) killInfoGO[0];
            if (obj4 != null)
            {
                obj4.GetComponent<KillInfoComponent>().Destroy();
            }

            killInfoGO.RemoveAt(0);
        }

        obj3.transform.parent = obj2.GetComponent<UIReferArray>().panels[0].transform;
        obj3.GetComponent<KillInfoComponent>().Show(t1, killer, t2, victim, dmg);
        killInfoGO.Add(obj3);
        if ((int) settings[244] == 1)
        {
            string str2 = "<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> " + killer.HexColor() + " killed ";
            string newLine = str2 + victim.HexColor() + " for " + dmg + " damage.";
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
        if (info.sender.IsMasterClient && logicLoaded && customLevelLoaded && !needChooseSide &&
            UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            Vector3 position = new Vector3(posX, posY, posZ);
            IN_GAME_MAIN_CAMERA component = UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>();
            component.setMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", position, new Quaternion(0f, 0f, 0f, 1f),
                0));
            string slot = myLastHero.ToUpper();
            switch (slot)
            {
                case "SET 1": //TODO: Remove and use ProfileSystem
                case "SET 2":
                case "SET 3":
                {
                    HeroCostume costume = CostumeConverter.LocalDataToHeroCostume(slot);
                    costume?.ValidateHeroStats();
                    CostumeConverter.HeroCostumeToLocalData(costume, slot);
                    if (costume != null)
                    {
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat =
                            costume.stat;
                    }
                    else
                    {
                        costume = HeroCostume.costumeOption[3];
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat =
                            HeroStat.GetInfo(costume.name);
                    }

                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                    component.main_object.GetComponent<HERO>().setStat2();
                    component.main_object.GetComponent<HERO>().SetSkillHUDPosition();
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

                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume =
                                HeroCostume.costume[id];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat =
                                HeroStat.GetInfo(HeroCostume.costume[id].name);
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>()
                                .setCharacterComponent();
                            component.main_object.GetComponent<HERO>().setStat2();
                            component.main_object.GetComponent<HERO>().SetSkillHUDPosition();
                            break;
                        }
                    }

                    break;
            }

            CostumeConverter.HeroCostumeToPhotonData(
                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, Player.Self);
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
            {
                Transform transform1 = component.main_object.transform;
                transform1.position +=
                    new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20));
            }

            Player.Self.SetCustomProperties(new Hashtable
            {
                {"dead", false},
                {PlayerProperty.IsTitan, 1}
            });
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

    [RPC]
    public void TitanGetKill(Player player, int Damage, string name1, PhotonMessageInfo info)
    {
        if (info != null && !info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(TitanGetKill), info);

        Damage = Mathf.Max(10, Damage);
        object[] parameters = {Damage};
        photonView.RPC("netShowDamage", player, parameters);
        object[] objArray2 = {name1, false};
        photonView.RPC("oneTitanDown", PhotonTargets.MasterClient, objArray2);
        SendKillInfo(false, player.Properties.Name, true, name1, Damage);
        PlayerKillInfoUpdate(player, Damage);
    }

    [RPC]
    private void ChatPM(string sender, string content, PhotonMessageInfo info) //TODO: Customize PMs message
    {
        Mod.Interface.Chat.ReceivePrivateMessage($"<color=#1068D4>PM</color><color=#108CD4>></color> <color=#{Mod.Interface.Chat.SystemColor}>{info.sender.Properties.HexName}: {content}</color>", info.sender);
    }
}