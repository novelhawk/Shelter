using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Enums;
using JetBrains.Annotations;
using Mod;
using Photon;
using Photon.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
public class PVPcheckPoint : Photon.MonoBehaviour, IComparable
{
    private bool annie;
    public GameObject[] chkPtNextArr;
    public GameObject[] chkPtPreviousArr;
    public static List<PVPcheckPoint> checkPoints;
    private float getPtsInterval = 20f;
    private float getPtsTimer;
    public bool hasAnnie;
    private float hitTestR = 15f;
    public GameObject humanCyc;
    public float humanPt;
    public float humanPtMax = 40f;
    public int id;
    public bool isBase;
    public int normalTitanRate = 70;
    private bool playerOn;
    public float size = 1f;
    private float spawnTitanTimer;
    public CheckPointState state;
    private GameObject supply;
    private float syncInterval = 0.6f;
    private float syncTimer;
    public GameObject titanCyc;
    public float titanInterval = 30f;
    private bool titanOn;
    public float titanPt;
    public float titanPtMax = 40f;

    [RPC]
    [UsedImplicitly]
    private void ChangeHumanPt(float pt)
    {
        this.humanPt = pt;
    }

    [RPC]
    [UsedImplicitly]
    private void ChangeState(int num)
    {
        if (num == 0)
        {
            this.state = CheckPointState.Non;
        }
        if (num == 1)
        {
            this.state = CheckPointState.Human;
        }
        if (num == 2)
        {
            this.state = CheckPointState.Titan;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void ChangeTitanPt(float pt)
    {
        this.titanPt = pt;
    }

    private void checkIfBeingCapture()
    {
        int num;
        this.playerOn = false;
        this.titanOn = false;
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] objArray2 = GameObject.FindGameObjectsWithTag("titan");
        for (num = 0; num < objArray.Length; num++)
        {
            if (Vector3.Distance(objArray[num].transform.position, transform.position) < this.hitTestR)
            {
                this.playerOn = true;
                if (this.state == CheckPointState.Human && objArray[num].GetPhotonView().isMine)
                {
                    if (GameManager.instance.checkpoint != gameObject)
                    {
                        GameManager.instance.checkpoint = gameObject;
                        Shelter.Chat.System("<color=#A8FF24>Respawn point changed to point" + this.id + "</color>");
                    }
                    break;
                }
            }
        }
        for (num = 0; num < objArray2.Length; num++)
        {
            if (Vector3.Distance(objArray2[num].transform.position, transform.position) < this.hitTestR + 5f && (objArray2[num].GetComponent<TITAN>() == null || !objArray2[num].GetComponent<TITAN>().hasDie))
            {
                this.titanOn = true;
                if (this.state == CheckPointState.Titan && objArray2[num].GetPhotonView().isMine && objArray2[num].GetComponent<TITAN>() != null && objArray2[num].GetComponent<TITAN>().nonAI)
                {
                    if (GameManager.instance.checkpoint != gameObject)
                    {
                        GameManager.instance.checkpoint = gameObject;
                        Shelter.Chat.System("<color=#A8FF24>Respawn point changed to point" + this.id + "</color>");
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Check whether the round was won by Humanity.
    /// </summary>
    /// <returns>True if all objectives were captured by Humans.</returns>
    private static bool DidHumanityWin()
    {
        return checkPoints.All(x => x.state == CheckPointState.Human);
    }

    /// <summary>
    /// Check whether the round was won by Titans.
    /// </summary>
    /// <returns>True if all objectives were captured by Titans.</returns>
    private static bool DidTitanWon()
    {
        return checkPoints.All(x => x.state == CheckPointState.Titan);
    }

    private float GetHeight(Vector3 pt)
    {
        LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(pt, -Vector3.up, out var hit, 1000f, mask2.value))
        {
            return hit.point.y;
        }
        return 0f;
    }

    private void OnHumanCapture()
    {
        if (this.humanPt >= this.humanPtMax)
        {
            this.humanPt = this.humanPtMax;
            this.titanPt = 0f;
            this.SendRPCs();
            this.state = CheckPointState.Human;
            photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.All, 1);
            if (LevelInfoManager.Get(GameManager.Level).LevelName != "The City I")
                this.supply = PhotonNetwork.Instantiate("aot_supply", transform.position - Vector3.up * (transform.position.y - this.GetHeight(transform.position)), transform.rotation, 0);
            
            GameManager.instance.PVPhumanScore += 2;
            GameManager.instance.CheckPVPPoints();
            if (DidHumanityWin())
                GameManager.instance.GameWin();
            return;
        }
        
        this.humanPt += Time.deltaTime;
    }

    private void OnCapturing() // Supposedly called every frame
    {
        if (this.humanPt > 0f)
        {
            this.humanPt -= Time.deltaTime * 3f;
            if (this.humanPt <= 0f)
            {
                this.humanPt = 0f;
                this.SendRPCs();
                if (this.state != CheckPointState.Titan)
                {
                    this.state = CheckPointState.Non;
                    photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.Others, 0);
                }
            }
        }
    }

    private void SpawnTitan()
    {
        GameObject obj2 = GameManager.instance.SpawnTitan(this.normalTitanRate, transform.position - Vector3.up * (transform.position.y - this.GetHeight(transform.position)), transform.rotation);
        if (LevelInfoManager.Get(GameManager.Level).LevelName == "The City I")
        {
            obj2.GetComponent<TITAN>().chaseDistance = 120f;
        }
        else
        {
            obj2.GetComponent<TITAN>().chaseDistance = 200f;
        }
        obj2.GetComponent<TITAN>().PVPfromCheckPt = this;
    }

    private void Start()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            Destroy(gameObject);
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode != GameMode.PvpCapture)
        {
            if (photonView.isMine)
                Destroy(gameObject);
            
            Destroy(gameObject);
        }
        else
        {
            checkPoints.Add(this);
            checkPoints.Sort();
            if (this.humanPt == this.humanPtMax)
            {
                this.state = CheckPointState.Human;
                if (photonView.isMine && LevelInfoManager.Get(GameManager.Level).LevelName != "The City I")
                {
                    this.supply = PhotonNetwork.Instantiate("aot_supply", transform.position - Vector3.up * (transform.position.y - this.GetHeight(transform.position)), transform.rotation, 0);
                }
            }
            else if (photonView.isMine && !this.hasAnnie)
            {
                if (Random.Range(0, 100) < 50)
                {
                    int num = Random.Range(1, 2);
                    for (int i = 0; i < num; i++)
                    {
                        this.SpawnTitan();
                    }
                }
                if (this.isBase)
                {
                    this.SpawnTitan();
                }
            }
            if (this.titanPt == this.titanPtMax)
            {
                this.state = CheckPointState.Titan;
            }
            this.hitTestR = 15f * this.size;
            transform.localScale = new Vector3(this.size, this.size, this.size);
        }
    }

    private void SendRPCs()
    {
        photonView.RPC(Rpc.TitanOverCheckpoint, PhotonTargets.Others, titanPt);
        photonView.RPC(Rpc.HumanOverCheckpoint, PhotonTargets.Others, humanPt);
    }

    private void titanGetsPoint()
    {
        if (this.titanPt >= this.titanPtMax)
        {
            this.titanPt = this.titanPtMax;
            this.humanPt = 0f;
            this.SendRPCs();
            if (this.state == CheckPointState.Human && this.supply != null)
            {
                PhotonNetwork.Destroy(this.supply);
            }
            this.state = CheckPointState.Titan;
            object[] parameters = new object[] { 2 };
            photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.All, parameters);
            GameManager component = GameManager.instance;
            component.PVPtitanScore += 2;
            GameManager.instance.CheckPVPPoints();
            if (DidTitanWon())
            {
                GameManager.instance.GameLose();
            }
            if (this.hasAnnie)
            {
                if (!this.annie)
                {
                    this.annie = true;
                    PhotonNetwork.Instantiate("FEMALE_TITAN", transform.position - Vector3.up * (transform.position.y - this.GetHeight(transform.position)), transform.rotation, 0);
                }
                else
                {
                    this.SpawnTitan();
                }
            }
            else
            {
                this.SpawnTitan();
            }
        }
        else
        {
            this.titanPt += Time.deltaTime;
        }
    }

    private void titanLosePoint()
    {
        if (this.titanPt > 0f)
        {
            this.titanPt -= Time.deltaTime * 3f;
            if (this.titanPt <= 0f)
            {
                this.titanPt = 0f;
                this.SendRPCs();
                if (this.state != CheckPointState.Human)
                {
                    this.state = CheckPointState.Non;
                    object[] parameters = new object[] { 0 };
                    photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.All, parameters);
                }
            }
        }
    }

    private void Update()
    {
        float x = this.humanPt / this.humanPtMax;
        float num2 = this.titanPt / this.titanPtMax;
        if (!photonView.isMine)
        {
            x = this.humanPt / this.humanPtMax;
            num2 = this.titanPt / this.titanPtMax;
            this.humanCyc.transform.localScale = new Vector3(x, x, 1f);
            this.titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
            this.syncTimer += Time.deltaTime;
            if (this.syncTimer > this.syncInterval)
            {
                this.syncTimer = 0f;
                this.checkIfBeingCapture();
            }
        }
        else
        {
            if (this.state == CheckPointState.Non)
            {
                if (this.playerOn && !this.titanOn)
                {
                    this.OnHumanCapture();
                    this.titanLosePoint();
                }
                else if (this.titanOn && !this.playerOn)
                {
                    this.titanGetsPoint();
                    this.OnCapturing();
                }
                else
                {
                    this.OnCapturing();
                    this.titanLosePoint();
                }
            }
            else if (this.state == CheckPointState.Human)
            {
                if (this.titanOn && !this.playerOn)
                {
                    this.titanGetsPoint();
                }
                else
                {
                    this.titanLosePoint();
                }
                this.getPtsTimer += Time.deltaTime;
                if (this.getPtsTimer > this.getPtsInterval)
                {
                    this.getPtsTimer = 0f;
                    if (!this.isBase)
                    {
                        GameManager component = GameManager.instance;
                        component.PVPhumanScore++;
                    }
                    GameManager.instance.CheckPVPPoints();
                }
            }
            else if (this.state == CheckPointState.Titan)
            {
                if (this.playerOn && !this.titanOn)
                {
                    this.OnHumanCapture();
                }
                else
                {
                    this.OnCapturing();
                }
                this.getPtsTimer += Time.deltaTime;
                if (this.getPtsTimer > this.getPtsInterval)
                {
                    this.getPtsTimer = 0f;
                    if (!this.isBase)
                    {
                        GameManager local2 = GameManager.instance;
                        local2.PVPtitanScore++;
                    }
                    GameManager.instance.CheckPVPPoints();
                }
                this.spawnTitanTimer += Time.deltaTime;
                if (this.spawnTitanTimer > this.titanInterval)
                {
                    this.spawnTitanTimer = 0f;
                    if (LevelInfoManager.Get(GameManager.Level).LevelName == "The City I")
                    {
                        if (GameObject.FindGameObjectsWithTag("titan").Length < 12)
                        {
                            this.SpawnTitan();
                        }
                    }
                    else if (GameObject.FindGameObjectsWithTag("titan").Length < 20)
                    {
                        this.SpawnTitan();
                    }
                }
            }
            this.syncTimer += Time.deltaTime;
            if (this.syncTimer > this.syncInterval)
            {
                this.syncTimer = 0f;
                this.checkIfBeingCapture();
                this.SendRPCs();
            }
            x = this.humanPt / this.humanPtMax;
            num2 = this.titanPt / this.titanPtMax;
            this.humanCyc.transform.localScale = new Vector3(x, x, 1f);
            this.titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
        }
    }

    public GameObject chkPtNext
    {
        get
        {
            if (this.chkPtNextArr.Length <= 0)
            {
                return null;
            }
            return this.chkPtNextArr[Random.Range(0, this.chkPtNextArr.Length)];
        }
    }

    public GameObject chkPtPrevious
    {
        get
        {
            if (this.chkPtPreviousArr.Length <= 0)
            {
                return null;
            }
            return this.chkPtPreviousArr[Random.Range(0, this.chkPtPreviousArr.Length)];
        }
    }

    public int CompareTo(object obj)
    {
        if (obj is PVPcheckPoint other)
        {
            if (id > other.id)
                return 1;
            if (id < other.id)
                return -1;
            return 0;
        }

        return 1;
    }
}

