using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using JetBrains.Annotations;
using Mod;
using Mod.Exceptions;
using Mod.GameSettings;
using Mod.Managers;
using Mod.Modules;
using Photon;
using Photon.Enums;
using RC;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class COLOSSAL_TITAN : Photon.MonoBehaviour
{
    private string actionName;
    private string attackAnimation;
    private float attackCheckTime;
    private float attackCheckTimeA;
    private float attackCheckTimeB;
    private bool attackChkOnce;
    private int attackCount;
    private int attackPattern = -1;
    public GameObject bottomObject;
    private Transform checkHitCapsuleEnd;
    private Vector3 checkHitCapsuleEndOld;
    private float checkHitCapsuleR;
    private Transform checkHitCapsuleStart;
    public GameObject door_broken;
    public GameObject door_closed;
    public bool hasDie;
    public bool hasspawn;
    public GameObject healthLabel;
    public float healthTime;
    private bool isSteamNeed;
    public float lagMax;
    public int maxHealth;
    public float myDistance;
    public GameObject myHero;
    public int NapeArmor = 10000;
    public int NapeArmorTotal = 10000;
    public GameObject neckSteamObject;
    public float size;
    private string state = "idle";
    public GameObject sweepSmokeObject;
    public float tauntTime;
    private float waitTime = 2f;

    private void attack_sweep(string type = "")
    {
        this.CallTitan();
        this.state = "attack_sweep";
        this.attackAnimation = "sweep" + type;
        this.attackCheckTimeA = 0.4f;
        this.attackCheckTimeB = 0.57f;
        this.checkHitCapsuleStart = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
        this.checkHitCapsuleEnd = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        this.checkHitCapsuleR = 20f;
        this.crossFade("attack_" + this.attackAnimation, 0.1f);
        this.attackChkOnce = false;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().Play();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            if (GameManager.LAN)
            {
                if (Network.peerType != NetworkPeerType.Server)
                {
                }
            }
            else if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC(Rpc.StartSweepSmoke, PhotonTargets.Others);
            }
        }
    }

    private void Awake()
    {
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    public void BlowPlayer(GameObject player, Transform neck)
    {
        Vector3 vector = -(neck.position + transform.forward * 50f - player.transform.position);
        switch (IN_GAME_MAIN_CAMERA.GameType)
        {
            case GameType.Singleplayer:
                player.GetComponent<HERO>().BlowAway(vector.normalized * 20f + Vector3.up * 1f);
                break;
            
            case GameType.Multiplayer when PhotonNetwork.isMasterClient:
                player.GetComponent<HERO>().photonView.RPC(Rpc.BlowAway, PhotonTargets.All, 
                    vector.normalized * 20f + Vector3.up * 1f);
                break;
        }
    }

    private void callTitan(bool special = false)
    {
        if (special || GameObject.FindGameObjectsWithTag("titan").Length <= 6)
        {
            GameObject obj4;
            GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
            ArrayList list = new ArrayList();
            foreach (GameObject obj2 in objArray)
            {
                if (obj2.transform.parent.name == "titanRespawnCT")
                {
                    list.Add(obj2);
                }
            }
            GameObject obj3 = (GameObject) list[UnityEngine.Random.Range(0, list.Count)];
            string[] strArray = new string[] { "TITAN_VER3.1" };
            if (GameManager.LAN)
            {
                obj4 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), obj3.transform.position, obj3.transform.rotation, 0);
            }
            else
            {
                obj4 = PhotonNetwork.Instantiate(strArray[UnityEngine.Random.Range(0, strArray.Length)], obj3.transform.position, obj3.transform.rotation, 0);
            }
            if (special)
            {
                GameObject[] objArray3 = GameObject.FindGameObjectsWithTag("route");
                GameObject route = objArray3[UnityEngine.Random.Range(0, objArray3.Length)];
                while (route.name != "routeCT")
                {
                    route = objArray3[UnityEngine.Random.Range(0, objArray3.Length)];
                }
                obj4.GetComponent<TITAN>().setRoute(route);
                obj4.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Abnormal, false);
                obj4.GetComponent<TITAN>().activeRad = 0;
                obj4.GetComponent<TITAN>().toCheckPoint((Vector3) obj4.GetComponent<TITAN>().checkPoints[0], 10f);
            }
            else
            {
                float num2 = 0.7f;
                float num3 = 0.7f;
                if (IN_GAME_MAIN_CAMERA.difficulty != 0)
                {
                    if (IN_GAME_MAIN_CAMERA.difficulty == 1)
                    {
                        num2 = 0.4f;
                        num3 = 0.7f;
                    }
                    else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                    {
                        num2 = -1f;
                        num3 = 0.7f;
                    }
                }
                if (GameObject.FindGameObjectsWithTag("titan").Length == 5)
                {
                    obj4.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                }
                else if (UnityEngine.Random.Range(0f, 1f) >= num2)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < num3)
                    {
                        obj4.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Jumper, false);
                    }
                    else
                    {
                        obj4.GetComponent<TITAN>().setAbnormalType2(AbnormalType.Crawler, false);
                    }
                }
                obj4.GetComponent<TITAN>().activeRad = 200;
            }
            if (GameManager.LAN)
            {
                GameObject obj6 = (GameObject) Network.Instantiate(Resources.Load("FX/FXtitanSpawn"), obj4.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
                obj6.transform.localScale = obj4.transform.localScale;
            }
            else
            {
                PhotonNetwork.Instantiate("FX/FXtitanSpawn", obj4.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = obj4.transform.localScale;
            }
        }
    }

    private void CallTitan()
    {
        ++attackCount;
        
        int attacksBeforeTitan;
        int attacksBeforeSpecialTitan;
        
        switch (IN_GAME_MAIN_CAMERA.difficulty)
        {
            default:
                attacksBeforeTitan = 4;
                attacksBeforeSpecialTitan = 7;
                break;
            case 1:
                attacksBeforeTitan = 4;
                attacksBeforeSpecialTitan = 6;
                break;
            case 2:
                attacksBeforeTitan = 3;
                attacksBeforeSpecialTitan = 5;
                break;
        }
        
        // Spawn titan every 'attacksBeforeTitan' attacks
        if (this.attackCount % attacksBeforeTitan == 0)
            this.callTitan();

        // When below 30% spawn special titans twice as fast
        if (this.NapeArmor < this.NapeArmorTotal * 0.3)
            attacksBeforeSpecialTitan /= 2;
        
        // Spawn special titan every 'attacksBeforeSpecialTitan' attacks
        if (this.attackCount % attacksBeforeSpecialTitan == 0)
            this.callTitan(true);
    }

    [RPC]
    [UsedImplicitly]
    private void ChangeDoor()
    {
        this.door_broken.SetActive(true);
        this.door_closed.SetActive(false);
    }

    private IEnumerable<RaycastHit> CheckHitCapsule(Vector3 start, Vector3 end, float r)
    {
        return Physics.SphereCastAll(start, r, end - start, Vector3.Distance(start, end));
    }

    private GameObject checkIfHitHand(Component hand)
    {
        foreach (Collider c in Physics.OverlapSphere(hand.GetComponent<SphereCollider>().transform.position, 31f))
        {
            if (c.transform.root.CompareTag("Player"))
            {
                GameObject go = c.transform.root.gameObject;
                if (go.GetComponent<TITAN_EREN>() != null)
                {
                    if (!go.GetComponent<TITAN_EREN>().isHit)
                    {
                        go.GetComponent<TITAN_EREN>().hitByTitan();
                    }
                    return go;
                }
                if (go.GetComponent<HERO>() != null && !go.GetComponent<HERO>().isInvincible())
                {
                    return go;
                }
            }
        }
        return null;
    }

    private void crossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
        if (!GameManager.LAN && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            photonView.RPC(Rpc.CrossFade, PhotonTargets.Others, aniName, time);
        }
    }

    private void findNearestHero()
    {
        this.myHero = this.getNearestHero();
    }

    private GameObject getNearestHero()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        foreach (GameObject obj3 in objArray)
        {
            if ((obj3.GetComponent<HERO>() == null || !obj3.GetComponent<HERO>().HasDied()) && (obj3.GetComponent<TITAN_EREN>() == null || !obj3.GetComponent<TITAN_EREN>().hasDied))
            {
                float num3 = Mathf.Sqrt((obj3.transform.position.x - transform.position.x) * (obj3.transform.position.x - transform.position.x) + (obj3.transform.position.z - transform.position.z) * (obj3.transform.position.z - transform.position.z));
                if (obj3.transform.position.y - transform.position.y < 450f && num3 < positiveInfinity)
                {
                    obj2 = obj3;
                    positiveInfinity = num3;
                }
            }
        }
        return obj2;
    }

    private void idle()
    {
        this.state = "idle";
        this.crossFade("idle", 0.2f);
    }

    private void kick()
    {
        this.state = "kick";
        this.actionName = "attack_kick_wall";
        this.attackCheckTime = 0.64f;
        this.attackChkOnce = false;
        this.crossFade(this.actionName, 0.1f);
    }

    private void killPlayer(GameObject hitHero)
    {
        if (hitHero != null)
        {
            Vector3 position = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                if (!hitHero.GetComponent<HERO>().HasDied())
                {
                    hitHero.GetComponent<HERO>().die((hitHero.transform.position - position) * 15f * 4f, false);
                }
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                if (GameManager.LAN)
                {
                    if (!hitHero.GetComponent<HERO>().HasDied())
                    {
                        hitHero.GetComponent<HERO>().markDie();
                    }
                }
                else if (!hitHero.GetComponent<HERO>().HasDied())
                {
                    hitHero.GetComponent<HERO>().markDie();
                    hitHero.GetComponent<HERO>().photonView.RPC(Rpc.Die, PhotonTargets.All, (hitHero.transform.position - position) * 15f * 4f, false, -1, "Colossal Titan", true);
                }
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void LabelRPC(int health, int titanMaxHealth, PhotonMessageInfo info)
    {
        if (titanMaxHealth <= 0)
            throw new NotAllowedException(nameof(LabelRPC), info);
        
        if (health < 0)
        {
            if (healthLabel != null)
                Destroy(healthLabel);
            return;
        }
        
        if (healthLabel == null)
        {
            healthLabel = (GameObject) Instantiate(Resources.Load("UI/LabelNameOverHead"));
            healthLabel.name = "LabelNameOverHead";
            healthLabel.transform.parent = transform;
            healthLabel.transform.localPosition = new Vector3(0f, 430f, 0f);
            float a = 15f;
            if (this.size > 0f && this.size < 1f)
            {
                a = 15f / this.size;
                a = Mathf.Min(a, 100f);
            }
            healthLabel.transform.localScale = new Vector3(a, a, a);
        }
        float num2 = health / (float)titanMaxHealth;
        
        string str = "[7FFF00]";
        if (num2 < 0.75f && num2 >= 0.5f)
            str = "[f2b50f]";
        else if (num2 < 0.5f && num2 >= 0.25f)
            str = "[ff8100]";
        else if (num2 < 0.25f)
            str = "[ff3333]";
        
        this.healthLabel.GetComponent<UILabel>().text = str + Convert.ToString(health);
    }

    public void loadskin()
    {
        if (!PhotonNetwork.isMasterClient || !ModuleManager.Enabled(nameof(ModuleEnableSkins))) 
            return;
        
        photonView.RPC(Rpc.LoadSkin, PhotonTargets.AllBuffered, GameManager.settings.TitanSkin.Colossal);
    }

    public IEnumerator loadskinE(string url)
    {
        while (!this.hasspawn)
        {
            yield return null;
        }
        bool mipmap = GameManager.settings.UseMipmap;
        bool iteratorVariable1 = false;
        foreach (Renderer iteratorVariable2 in this.GetComponentsInChildren<Renderer>())
        {
            if (iteratorVariable2.name.Contains("hair"))
            {
                if (!GameManager.linkHash[2].ContainsKey(url))
                {
                    WWW link = new WWW(url);
                    yield return link;
                    Texture2D iteratorVariable4 = RCextensions.LoadImageRC(link, mipmap, 1000000);
                    link.Dispose();
                    if (!GameManager.linkHash[2].ContainsKey(url))
                    {
                        iteratorVariable1 = true;
                        iteratorVariable2.material.mainTexture = iteratorVariable4;
                        GameManager.linkHash[2].Add(url, iteratorVariable2.material);
                        iteratorVariable2.material = (Material)GameManager.linkHash[2][url];
                    }
                    else
                    {
                        iteratorVariable2.material = (Material)GameManager.linkHash[2][url];
                    }
                }
                else
                {
                    iteratorVariable2.material = (Material)GameManager.linkHash[2][url];
                }
            }
        }
        if (iteratorVariable1)
        {
            GameManager.instance.UnloadAssets();
        }
    }

    [RPC]
    [UsedImplicitly]
    public void LoadskinRPC(string url)
    {
        Shelter.LogBoth("LOADING {0} SKIN", nameof(COLOSSAL_TITAN));
        Shelter.LogBoth("{0}: {1}", nameof(url), url);
        
        if (ModuleManager.Enabled(nameof(ModuleEnableSkins)) && Utility.IsValidImageUrl(url))
        {
            StartCoroutine(this.loadskinE(url));
        }
    }

    private void neckSteam()
    {
        this.neckSteamObject.GetComponent<ParticleSystem>().Stop();
        this.neckSteamObject.GetComponent<ParticleSystem>().Play();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            if (GameManager.LAN)
            {
                if (Network.peerType != NetworkPeerType.Server)
                {
                }
            }
            else if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC(Rpc.StartNeckStream, PhotonTargets.Others);
            }
        }
        this.isSteamNeed = true;
        Transform neck = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        float radius = 30f;
        foreach (Collider c in Physics.OverlapSphere(neck.transform.position - transform.forward * 10f, radius))
        {
            if (c.transform.root.CompareTag("Player"))
            {
                GameObject go = c.transform.root.gameObject;
                if (go.GetComponent<TITAN_EREN>() == null && go.GetComponent<HERO>() != null)
                {
                    this.BlowPlayer(go, neck);
                }
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetCrossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
    }

    [RPC]
    [UsedImplicitly]
    public void NetDie()
    {
        if (!this.hasDie)
        {
            this.hasDie = true;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetPlayAnimation(string aniName)
    {
        animation.Play(aniName);
    }

    [RPC]
    [UsedImplicitly]
    private void NetPlayAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
    }

    private void OnDestroy()
    {
        if (GameObject.Find("MultiplayerManager") != null)
        {
            GameManager.instance.ColossalTitans.Remove(this);
        }
    }

    private void playAnimation(string aniName)
    {
        animation.Play(aniName);
        if (!GameManager.LAN && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            photonView.RPC(Rpc.PlayAnimation, PhotonTargets.Others, aniName);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
        if (!GameManager.LAN && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            photonView.RPC(Rpc.PlayAnimationAt, PhotonTargets.Others, aniName, normalizedTime);
        }
    }

    private void playSound(string sndname)
    {
        this.PlaysoundRPC(sndname);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            if (GameManager.LAN)
            {
                if (Network.peerType != NetworkPeerType.Server)
                {
                }
            }
            else if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC(Rpc.PlaySound, PhotonTargets.Others, sndname);
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    private void PlaysoundRPC(string sndname)
    {
        transform.Find(sndname).GetComponent<AudioSource>().Play();
    }

    [RPC]
    [UsedImplicitly]
    private void RemoveMe()
    {
        Destroy(gameObject);
    }

    [RPC]
    [UsedImplicitly]
    public void SetSize(float newSize, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient) 
            throw new NotAllowedException(nameof(SetSize), info);
        
        newSize = Mathf.Clamp(newSize, 0.1f, 50f);
        transform.localScale = transform.localScale * (newSize * 0.05f);
        size = newSize;
    }

    private void Slap(string type)
    {
        this.CallTitan();
        this.state = "slap";
        this.attackAnimation = type;
        
        switch (type)
        {
            case "r1":
            case "r2":
                this.checkHitCapsuleStart = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
                break;
            case "l1":
            case "l2":
                this.checkHitCapsuleStart = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
                break;
        }
        
        this.attackCheckTime = 0.57f;
        this.attackChkOnce = false;
        this.crossFade("attack_slap_" + this.attackAnimation, 0.1f);
    }

    private void Start()
    {
        this.startMain();
        this.size = 20f;
        if (Minimap.instance != null)
            Minimap.instance.TrackGameObjectOnMinimap(gameObject, Color.black, false, true);
        
        if (photonView.isMine)
        {
            if (GameManager.settings.EnableCustomSize)
                photonView.RPC(Rpc.SetSize, PhotonTargets.AllBuffered, GameManager.settings.TitanSize.Random);
            
            this.lagMax = 150f + this.size * 3f;
            this.healthTime = 0f;
            this.maxHealth = this.NapeArmor;
            
            if (GameManager.settings.HealthMode > HealthMode.Off)
                this.maxHealth = this.NapeArmor = (int) GameManager.settings.TitanHealth.Random;
            
            if (this.NapeArmor > 0)
                photonView.RPC(Rpc.UpdateHealthLabel, PhotonTargets.AllBuffered, NapeArmor, maxHealth);
            
            this.loadskin();
        }
        this.hasspawn = true;
    }

    private void startMain()
    {
        GameManager.instance.ColossalTitans.Add(this);
        if (this.myHero == null)
            this.findNearestHero();
        
        name = "COLOSSAL_TITAN";
        this.NapeArmor = 1000;
        bool flag = LevelInfoManager.Get(GameManager.Level).RespawnMode == RespawnMode.NEVER;
        switch (IN_GAME_MAIN_CAMERA.difficulty)
        {
            case 0:
                this.NapeArmor = !flag ? 5000 : 2000;
                break;
            
            case 1:
                this.NapeArmor = !flag ? 8000 : 3500;
                foreach (AnimationState current in animation)
                    if (current != null)
                        current.speed = 1.02f;
                break;
            
            case 2:
                this.NapeArmor = !flag ? 12000 : 5000;
                foreach (AnimationState current in animation)
                    if (current != null)
                        current.speed = 1.05f;
                break;
        }
        this.NapeArmorTotal = this.NapeArmor;
        this.state = "wait";
        transform.position += -Vector3.up * 10000f;
        if (!GameManager.LAN)
            GetComponent<PhotonView>().enabled = false;
        else
            GetComponent<NetworkView>().enabled = false;
        this.door_broken = GameObject.Find("door_broke");
        this.door_closed = GameObject.Find("door_fine");
        this.door_broken.SetActive(false);
        this.door_closed.SetActive(true);
    }

    [RPC]
    [UsedImplicitly]
    private void StartNeckSteam()
    {
        this.neckSteamObject.GetComponent<ParticleSystem>().Stop();
        this.neckSteamObject.GetComponent<ParticleSystem>().Play();
    }

    [RPC]
    [UsedImplicitly]
    private void StartSweepSmoke()
    {
        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().Play();
    }

    private void Steam()
    {
        this.CallTitan(); // Is feng for real?
        this.state = "steam";
        this.actionName = "attack_steam";
        this.attackCheckTime = 0.45f;
        this.crossFade(this.actionName, 0.1f);
        this.attackChkOnce = false;
    }

    [RPC]
    [UsedImplicitly]
    private void StopSweepSmoke()
    {
        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
    }

    [RPC]
    [UsedImplicitly]
    public void TitanGetHit(int viewID, int speed)
    {
        Transform t = this.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            Vector3 vector = view.gameObject.transform.position - t.transform.position;
            if (vector.magnitude < this.lagMax && this.healthTime <= 0f)
            {
                if (speed >= GameManager.settings.MinimumDamage)
                {
                    this.NapeArmor -= speed;
                }
                if (this.maxHealth > 0f)
                {
                    photonView.RPC(Rpc.UpdateHealthLabel, PhotonTargets.AllBuffered, this.NapeArmor, this.maxHealth);
                }
                this.neckSteam();
                if (this.NapeArmor <= 0)
                {
                    this.NapeArmor = 0;
                    if (!this.hasDie)
                    {
                        if (GameManager.LAN)
                        {
                            this.NetDie();
                        }
                        else
                        {
                            photonView.RPC(Rpc.Die, PhotonTargets.OthersBuffered);
                            this.NetDie();
                            GameManager.instance.TitanGetKill(view.owner, speed, name, null);
                        }
                    }
                }
                else
                {
                    GameManager.instance.SendKillInfo(false, view.owner.Properties.Name, true, "Colossal Titan's neck", speed);
                    GameManager.instance.photonView.RPC(Rpc.ShowDamage, view.owner, speed);
                }
                this.healthTime = 0.2f;
            }
        }
    }
    
    public void DoUpdate()
    {
        this.healthTime -= Time.deltaTime;
        this.updateLabel();
        if (this.state != "null")
        {
            if (this.state == "wait")
            {
                this.waitTime -= Time.deltaTime;
                if (this.waitTime <= 0f)
                {
                    transform.position = new Vector3(30f, 0f, 784f);
                    Instantiate(Resources.Load("FX/ThunderCT"), transform.position + Vector3.up * 350f, Quaternion.Euler(270f, 0f, 0f));
                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    {
                        this.idle();
                    }
                    else if (!GameManager.LAN ? photonView.isMine : networkView.isMine)
                    {
                        this.idle();
                    }
                    else
                    {
                        this.state = "null";
                    }
                }
            }
            else if (this.state != "idle")
            {
                if (this.state == "attack_sweep")
                {
                    if (this.attackCheckTimeA != 0f && !(animation["attack_" + this.attackAnimation].normalizedTime < this.attackCheckTimeA || animation["attack_" + this.attackAnimation].normalizedTime > this.attackCheckTimeB ? this.attackChkOnce || animation["attack_" + this.attackAnimation].normalizedTime < this.attackCheckTimeA : false))
                    {
                        if (!this.attackChkOnce)
                        {
                            this.attackChkOnce = true;
                        }
                        foreach (RaycastHit hit in this.CheckHitCapsule(this.checkHitCapsuleStart.position, this.checkHitCapsuleEnd.position, this.checkHitCapsuleR))
                        {
                            GameObject go = hit.collider.gameObject;
                            if (go.CompareTag("Player"))
                            {
                                this.killPlayer(go);
                            }
                            if (go.CompareTag("erenHitbox") && 
                                this.attackAnimation == "combo_3" &&
                                IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && 
                                (!GameManager.LAN ? PhotonNetwork.isMasterClient : Network.isServer))
                            {
                                go.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(3);
                            }
                        }
                        foreach (RaycastHit hit2 in this.CheckHitCapsule(this.checkHitCapsuleEndOld, this.checkHitCapsuleEnd.position, this.checkHitCapsuleR))
                        {
                            GameObject hitHero = hit2.collider.gameObject;
                            if (hitHero.CompareTag("Player"))
                            {
                                this.killPlayer(hitHero);
                            }
                        }
                        this.checkHitCapsuleEndOld = this.checkHitCapsuleEnd.position;
                    }
                    if (animation["attack_" + this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
                        this.sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
                        if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || GameManager.LAN))
                        {
                            photonView.RPC(Rpc.StopSweepSmoke, PhotonTargets.Others);
                        }
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == "kick")
                {
                    if (!this.attackChkOnce && animation[this.actionName].normalizedTime >= this.attackCheckTime)
                    {
                        this.attackChkOnce = true;
                        this.door_broken.SetActive(true);
                        this.door_closed.SetActive(false);
                        if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || GameManager.LAN))
                        {
                            photonView.RPC(Rpc.ChangeDoor, PhotonTargets.OthersBuffered);
                        }
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                        {
                            if (GameManager.LAN)
                            {
                                Network.Instantiate(Resources.Load("FX/boom1_CT_KICK"), transform.position + transform.forward * 120f + transform.right * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
                                Network.Instantiate(Resources.Load("rock"), transform.position + transform.forward * 120f + transform.right * 30f, Quaternion.Euler(0f, 0f, 0f), 0);
                            }
                            else
                            {
                                PhotonNetwork.Instantiate("FX/boom1_CT_KICK", transform.position + transform.forward * 120f + transform.right * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
                                PhotonNetwork.Instantiate("rock", transform.position + transform.forward * 120f + transform.right * 30f, Quaternion.Euler(0f, 0f, 0f), 0);
                            }
                        }
                        else
                        {
                            Instantiate(Resources.Load("FX/boom1_CT_KICK"), transform.position + transform.forward * 120f + transform.right * 30f, Quaternion.Euler(270f, 0f, 0f));
                            Instantiate(Resources.Load("rock"), transform.position + transform.forward * 120f + transform.right * 30f, Quaternion.Euler(0f, 0f, 0f));
                        }
                    }
                    if (animation[this.actionName].normalizedTime >= 1f)
                    {
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == "slap")
                {
                    if (!this.attackChkOnce && animation["attack_slap_" + this.attackAnimation].normalizedTime >= this.attackCheckTime)
                    {
                        GameObject obj4;
                        this.attackChkOnce = true;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                        {
                            if (GameManager.LAN)
                            {
                                obj4 = (GameObject) Network.Instantiate(Resources.Load("FX/boom1"), this.checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f), 0);
                            }
                            else
                            {
                                obj4 = PhotonNetwork.Instantiate("FX/boom1", this.checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f), 0);
                            }
                            if (obj4.GetComponent<EnemyfxIDcontainer>() != null)
                            {
                                obj4.GetComponent<EnemyfxIDcontainer>().titanName = name;
                            }
                        }
                        else
                        {
                            obj4 = (GameObject) Instantiate(Resources.Load("FX/boom1"), this.checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f));
                        }
                        obj4.transform.localScale = new Vector3(5f, 5f, 5f);
                    }
                    if (animation["attack_slap_" + this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == "steam")
                {
                    if (!this.attackChkOnce && animation[this.actionName].normalizedTime >= this.attackCheckTime)
                    {
                        this.attackChkOnce = true;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                        {
                            if (GameManager.LAN)
                            {
                                Network.Instantiate(Resources.Load("FX/colossal_steam"), transform.position + transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
                                Network.Instantiate(Resources.Load("FX/colossal_steam"), transform.position + transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
                                Network.Instantiate(Resources.Load("FX/colossal_steam"), transform.position + transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
                            }
                            else
                            {
                                PhotonNetwork.Instantiate("FX/colossal_steam", transform.position + transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
                                PhotonNetwork.Instantiate("FX/colossal_steam", transform.position + transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
                                PhotonNetwork.Instantiate("FX/colossal_steam", transform.position + transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
                            }
                        }
                        else
                        {
                            Instantiate(Resources.Load("FX/colossal_steam"), transform.position + transform.forward * 185f, Quaternion.Euler(270f, 0f, 0f));
                            Instantiate(Resources.Load("FX/colossal_steam"), transform.position + transform.forward * 303f, Quaternion.Euler(270f, 0f, 0f));
                            Instantiate(Resources.Load("FX/colossal_steam"), transform.position + transform.forward * 50f, Quaternion.Euler(270f, 0f, 0f));
                        }
                    }
                    if (animation[this.actionName].normalizedTime >= 1f)
                    {
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                        {
                            if (GameManager.LAN)
                            {
                                Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), transform.position + transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
                                Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), transform.position + transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
                                Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), transform.position + transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
                            }
                            else
                            {
                                GameObject obj5 = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", transform.position + transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
                                if (obj5.GetComponent<EnemyfxIDcontainer>() != null)
                                {
                                    obj5.GetComponent<EnemyfxIDcontainer>().titanName = name;
                                }
                                obj5 = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", transform.position + transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
                                if (obj5.GetComponent<EnemyfxIDcontainer>() != null)
                                {
                                    obj5.GetComponent<EnemyfxIDcontainer>().titanName = name;
                                }
                                obj5 = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", transform.position + transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
                                if (obj5.GetComponent<EnemyfxIDcontainer>() != null)
                                {
                                    obj5.GetComponent<EnemyfxIDcontainer>().titanName = name;
                                }
                            }
                        }
                        else
                        {
                            Instantiate(Resources.Load("FX/colossal_steam_dmg"), transform.position + transform.forward * 185f, Quaternion.Euler(270f, 0f, 0f));
                            Instantiate(Resources.Load("FX/colossal_steam_dmg"), transform.position + transform.forward * 303f, Quaternion.Euler(270f, 0f, 0f));
                            Instantiate(Resources.Load("FX/colossal_steam_dmg"), transform.position + transform.forward * 50f, Quaternion.Euler(270f, 0f, 0f));
                        }
                        if (this.hasDie)
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                Destroy(gameObject);
                            }
                            else if (GameManager.LAN)
                            {
                                if (networkView.isMine)
                                {
                                }
                            }
                            else if (PhotonNetwork.isMasterClient)
                            {
                                PhotonNetwork.Destroy(photonView);
                            }
                            GameManager.instance.GameWin();
                        }
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == string.Empty)
                {
                }
            }
            else if (this.attackPattern == -1)
            {
                this.Slap("r1");
                this.attackPattern++;
            }
            else if (this.attackPattern == 0)
            {
                this.attack_sweep(string.Empty);
                this.attackPattern++;
            }
            else if (this.attackPattern == 1)
            {
                this.Steam();
                this.attackPattern++;
            }
            else if (this.attackPattern == 2)
            {
                this.kick();
                this.attackPattern++;
            }
            else if (this.isSteamNeed || this.hasDie)
            {
                this.Steam();
                this.isSteamNeed = false;
            }
            else if (this.myHero == null)
            {
                this.findNearestHero();
            }
            else
            {
                Vector3 vector = this.myHero.transform.position - transform.position;
                float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                float f = -Mathf.DeltaAngle(current, gameObject.transform.rotation.eulerAngles.y - 90f);
                this.myDistance = Mathf.Sqrt((this.myHero.transform.position.x - transform.position.x) * (this.myHero.transform.position.x - transform.position.x) + (this.myHero.transform.position.z - transform.position.z) * (this.myHero.transform.position.z - transform.position.z));
                float num4 = this.myHero.transform.position.y - transform.position.y;
                if (this.myDistance < 85f && UnityEngine.Random.Range(0, 100) < 5)
                {
                    this.Steam();
                }
                else
                {
                    if (num4 > 310f && num4 < 350f)
                    {
                        if (Vector3.Distance(this.myHero.transform.position, transform.Find("APL1").position) < 40f)
                        {
                            this.Slap("l1");
                            return;
                        }
                        if (Vector3.Distance(this.myHero.transform.position, transform.Find("APL2").position) < 40f)
                        {
                            this.Slap("l2");
                            return;
                        }
                        if (Vector3.Distance(this.myHero.transform.position, transform.Find("APR1").position) < 40f)
                        {
                            this.Slap("r1");
                            return;
                        }
                        if (Vector3.Distance(this.myHero.transform.position, transform.Find("APR2").position) < 40f)
                        {
                            this.Slap("r2");
                            return;
                        }
                        if (this.myDistance < 150f && Mathf.Abs(f) < 80f)
                        {
                            this.attack_sweep(string.Empty);
                            return;
                        }
                    }
                    if (num4 < 300f && Mathf.Abs(f) < 80f && this.myDistance < 85f)
                    {
                        this.attack_sweep("_vertical");
                    }
                    else
                    {
                        switch (UnityEngine.Random.Range(0, 7))
                        {
                            case 0:
                                this.Slap("l1");
                                break;

                            case 1:
                                this.Slap("l2");
                                break;

                            case 2:
                                this.Slap("r1");
                                break;

                            case 3:
                                this.Slap("r2");
                                break;

                            case 4:
                                this.attack_sweep(string.Empty);
                                break;

                            case 5:
                                this.attack_sweep("_vertical");
                                break;

                            case 6:
                                this.Steam();
                                break;
                        }
                    }
                }
            }
        }
    }

    public void updateLabel()
    {
        if (this.healthLabel != null && this.healthLabel.GetComponent<UILabel>().isVisible)
        {
            this.healthLabel.transform.LookAt(2f * this.healthLabel.transform.position - Camera.main.transform.position);
        }
    }
}