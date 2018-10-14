using System.Collections;
using Mod;
using Mod.Interface;
using UnityEngine;

public class PVPcheckPoint : Photon.MonoBehaviour
{
    private bool annie;
    public GameObject[] chkPtNextArr;
    public GameObject[] chkPtPreviousArr;
    public static ArrayList checkPoints;
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
    private void ChangeHumanPt(float pt)
    {
        this.humanPt = pt;
    }

    [RPC]
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
                    if (FengGameManagerMKII.instance.checkpoint != gameObject)
                    {
                        FengGameManagerMKII.instance.checkpoint = gameObject;
                        Chat.System("<color=#A8FF24>Respawn point changed to point" + this.id + "</color>");
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
                    if (FengGameManagerMKII.instance.checkpoint != gameObject)
                    {
                        FengGameManagerMKII.instance.checkpoint = gameObject;
                        Chat.System("<color=#A8FF24>Respawn point changed to point" + this.id + "</color>");
                    }
                    break;
                }
            }
        }
    }

    private bool checkIfHumanWins()
    {
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if ((checkPoints[i] as PVPcheckPoint).state != CheckPointState.Human)
            {
                return false;
            }
        }
        return true;
    }

    private bool checkIfTitanWins()
    {
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if ((checkPoints[i] as PVPcheckPoint).state != CheckPointState.Titan)
            {
                return false;
            }
        }
        return true;
    }

    private float getHeight(Vector3 pt)
    {
        LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(pt, -Vector3.up, out var hit, 1000f, mask2.value))
        {
            return hit.point.y;
        }
        return 0f;
    }

    public string getStateString()
    {
        if (this.state == CheckPointState.Human)
        {
            return "[" + ColorSet.color_human + "]H[-]";
        }
        if (this.state == CheckPointState.Titan)
        {
            return "[" + ColorSet.color_titan_player + "]T[-]";
        }
        return "[" + ColorSet.color_D + "]_[-]";
    }

    private void humanGetsPoint()
    {
        if (this.humanPt >= this.humanPtMax)
        {
            this.humanPt = this.humanPtMax;
            this.titanPt = 0f;
            this.syncPts();
            this.state = CheckPointState.Human;
            photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.All, 1);
            if (LevelInfoManager.Get(FengGameManagerMKII.Level).LevelName != "The City I")
            {
                this.supply = PhotonNetwork.Instantiate("aot_supply", transform.position - Vector3.up * (transform.position.y - this.getHeight(transform.position)), transform.rotation, 0);
            }
            FengGameManagerMKII component = FengGameManagerMKII.instance;
            component.PVPhumanScore += 2;
            FengGameManagerMKII.instance.CheckPVPPoints();
            if (this.checkIfHumanWins())
            {
                FengGameManagerMKII.instance.GameWin();
            }
        }
        else
        {
            this.humanPt += Time.deltaTime;
        }
    }

    private void humanLosePoint()
    {
        if (this.humanPt > 0f)
        {
            this.humanPt -= Time.deltaTime * 3f;
            if (this.humanPt <= 0f)
            {
                this.humanPt = 0f;
                this.syncPts();
                if (this.state != CheckPointState.Titan)
                {
                    this.state = CheckPointState.Non;
                    photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.Others, 0);
                }
            }
        }
    }

    private void newTitan()
    {
        GameObject obj2 = FengGameManagerMKII.instance.SpawnTitan(this.normalTitanRate, transform.position - Vector3.up * (transform.position.y - this.getHeight(transform.position)), transform.rotation, false);
        if (LevelInfoManager.Get(FengGameManagerMKII.Level).LevelName == "The City I")
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
            {
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }
        else
        {
            checkPoints.Add(this);
            IComparer comparer = new IComparerPVPchkPtID();
            checkPoints.Sort(comparer);
            if (this.humanPt == this.humanPtMax)
            {
                this.state = CheckPointState.Human;
                if (photonView.isMine && LevelInfoManager.Get(FengGameManagerMKII.Level).LevelName != "The City I")
                {
                    this.supply = PhotonNetwork.Instantiate("aot_supply", transform.position - Vector3.up * (transform.position.y - this.getHeight(transform.position)), transform.rotation, 0);
                }
            }
            else if (photonView.isMine && !this.hasAnnie)
            {
                if (Random.Range(0, 100) < 50)
                {
                    int num = Random.Range(1, 2);
                    for (int i = 0; i < num; i++)
                    {
                        this.newTitan();
                    }
                }
                if (this.isBase)
                {
                    this.newTitan();
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

    private void syncPts()
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
            this.syncPts();
            if (this.state == CheckPointState.Human && this.supply != null)
            {
                PhotonNetwork.Destroy(this.supply);
            }
            this.state = CheckPointState.Titan;
            object[] parameters = new object[] { 2 };
            photonView.RPC(Rpc.CheckpointChangeState, PhotonTargets.All, parameters);
            FengGameManagerMKII component = FengGameManagerMKII.instance;
            component.PVPtitanScore += 2;
            FengGameManagerMKII.instance.CheckPVPPoints();
            if (this.checkIfTitanWins())
            {
                FengGameManagerMKII.instance.GameLose();
            }
            if (this.hasAnnie)
            {
                if (!this.annie)
                {
                    this.annie = true;
                    PhotonNetwork.Instantiate("FEMALE_TITAN", transform.position - Vector3.up * (transform.position.y - this.getHeight(transform.position)), transform.rotation, 0);
                }
                else
                {
                    this.newTitan();
                }
            }
            else
            {
                this.newTitan();
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
                this.syncPts();
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
                    this.humanGetsPoint();
                    this.titanLosePoint();
                }
                else if (this.titanOn && !this.playerOn)
                {
                    this.titanGetsPoint();
                    this.humanLosePoint();
                }
                else
                {
                    this.humanLosePoint();
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
                        FengGameManagerMKII component = FengGameManagerMKII.instance;
                        component.PVPhumanScore++;
                    }
                    FengGameManagerMKII.instance.CheckPVPPoints();
                }
            }
            else if (this.state == CheckPointState.Titan)
            {
                if (this.playerOn && !this.titanOn)
                {
                    this.humanGetsPoint();
                }
                else
                {
                    this.humanLosePoint();
                }
                this.getPtsTimer += Time.deltaTime;
                if (this.getPtsTimer > this.getPtsInterval)
                {
                    this.getPtsTimer = 0f;
                    if (!this.isBase)
                    {
                        FengGameManagerMKII local2 = FengGameManagerMKII.instance;
                        local2.PVPtitanScore++;
                    }
                    FengGameManagerMKII.instance.CheckPVPPoints();
                }
                this.spawnTitanTimer += Time.deltaTime;
                if (this.spawnTitanTimer > this.titanInterval)
                {
                    this.spawnTitanTimer = 0f;
                    if (LevelInfoManager.Get(FengGameManagerMKII.Level).LevelName == "The City I")
                    {
                        if (GameObject.FindGameObjectsWithTag("titan").Length < 12)
                        {
                            this.newTitan();
                        }
                    }
                    else if (GameObject.FindGameObjectsWithTag("titan").Length < 20)
                    {
                        this.newTitan();
                    }
                }
            }
            this.syncTimer += Time.deltaTime;
            if (this.syncTimer > this.syncInterval)
            {
                this.syncTimer = 0f;
                this.checkIfBeingCapture();
                this.syncPts();
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
}

