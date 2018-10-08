using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mod;
using Mod.GameSettings;
using UnityEngine;
using Random = UnityEngine.Random;

public class TITAN : Photon.MonoBehaviour
{
    #region Unity Initialized Fields
    
    public AbnormalType abnormalType;
    public int activeRad = 2147483647;
    public bool asClientLookTarget;
    public float attackDistance = 13f;
    public float attackWait = 1f;
    public Animation baseAnimation;
    public AudioSource baseAudioSource;
    public List<Collider> baseColliders;
    public Transform baseGameObjectTransform;
    public Rigidbody baseRigidBody;
    public Transform baseTransform;
    public float chaseDistance = 80f;
    public ArrayList checkPoints = new ArrayList();
    public bool colliderEnabled;
    public TITAN_CONTROLLER controller;
    public GameObject currentCamera;
    public int currentHealth;
    public bool eye;
    public GameObject grabTF;
    public bool hasDie;
    public bool hasExplode;
    public bool hasload;
    public bool hasSetLevel;
    public bool hasSpawn;
    public GameObject healthLabel;
    public bool healthLabelEnabled;
    public float healthTime;
    public bool isAlarm;
    public bool isHooked;
    public bool isLook;
    public float lagMax;
    public GameObject mainMaterial;
    public int maxHealth;
    public float maxVelocityChange = 10f;
    public FengGameManagerMKII MultiplayerManager;
    public int myDifficulty;
    public float myDistance;
    public Group myGroup = Group.Titan;
    public GameObject myHero;
    public float myLevel = 1f;
    public TitanTrigger myTitanTrigger;
    public bool nonAI;
    public PVPcheckPoint PVPfromCheckPt;
    public bool rockthrow;
    public int skin;
    public float speed = 7f;
    
    #endregion

    private Vector3 abnorma_jump_bite_horizon_v;
    private float angle;
    private string attackAnimation;
    private float attackCheckTime;
    private float attackCheckTimeA;
    private float attackCheckTimeB;
    private int attackCount;
    private bool attacked;
    private float attackEndWait;
    private float between2;
    private Transform currentGrabHand;
    private float desDeg;
    private float dieTime;
    private string fxName;
    private Vector3 fxPosition;
    private Quaternion fxRotation;
    private float getdownTime;
    private GameObject grabbedTarget;
    private float gravity = 120f;
    private bool grounded;
    private bool hasDieSteam;
    private Transform head;
    private Vector3 headscale = Vector3.one;
    private string hitAnimation;
    private float hitPause;
    private bool isAttackMoveByCore;
    private bool isGrabHandLeft;
    private bool leftHandAttack;
    private float maxStamina = 320f;
    private Transform neck;
    private bool needFreshCorePosition;
    private string nextAttackAnimation;
    private bool nonAIcombo;
    private Vector3 oldCorePosition;
    private Quaternion oldHeadRotation;
    private float random_run_time;
    private float rockInterval;
    private string runAnimation;
    private float sbtime;
    private Vector3 spawnPt;
    private float stamina = 320f;
    private TitanState state;
    private int stepSoundPhase = 2;
    private bool stuck;
    private float stuckTime;
    private float stuckTurnAngle;
    private Vector3 targetCheckPt;
    private Quaternion targetHeadRotation;
    private float targetR;
    private float tauntTime;
    private GameObject throwRock;
    private string turnAnimation;
    private float turnDeg;
    private GameObject whoHasTauntMe;

    private void Attack(string type)
    {
        this.state = TitanState.Attacking;
        this.attacked = false;
        this.isAlarm = true;
        if (this.attackAnimation == type)
        {
            this.attackAnimation = type;
            this.playAnimationAt("attack_" + type, 0f);
        }
        else
        {
            this.attackAnimation = type;
            this.playAnimationAt("attack_" + type, 0f);
        }
        this.nextAttackAnimation = null;
        this.fxName = null;
        this.isAttackMoveByCore = false;
        this.attackCheckTime = 0f;
        this.attackCheckTimeA = 0f;
        this.attackCheckTimeB = 0f;
        this.attackEndWait = 0f;
        this.fxRotation = Quaternion.Euler(270f, 0f, 0f);
        if (type != null)
        {
            switch (type)
            {
                case "abnormal_getup":
                    this.attackCheckTime = 0f;
                    this.fxName = string.Empty;
                    break;
                
                case "abnormal_jump":
                    this.nextAttackAnimation = "abnormal_getup";
                    if (this.nonAI)
                    {
                        this.attackEndWait = 0f;
                    }
                    else
                    {
                        this.attackEndWait = this.myDifficulty <= 0 ? UnityEngine.Random.Range(1f, 4f) : UnityEngine.Random.Range(0f, 1f);
                    }
                    this.attackCheckTime = 0.75f;
                    this.fxName = "boom4";
                    this.fxRotation = Quaternion.Euler(270f, this.baseTransform.rotation.eulerAngles.y, 0f);
                    break;
                
                case "combo_1":
                    this.nextAttackAnimation = "combo_2";
                    this.attackCheckTimeA = 0.54f;
                    this.attackCheckTimeB = 0.76f;
                    this.nonAIcombo = false;
                    this.isAttackMoveByCore = true;
                    this.leftHandAttack = false;
                    break;
                
                case "combo_2":
                    if (!(this.abnormalType == AbnormalType.Punk || this.nonAI))
                    {
                        this.nextAttackAnimation = "combo_3";
                    }
                    this.attackCheckTimeA = 0.37f;
                    this.attackCheckTimeB = 0.57f;
                    this.nonAIcombo = false;
                    this.isAttackMoveByCore = true;
                    this.leftHandAttack = true;
                    break;
                
                case "combo_3":
                    this.nonAIcombo = false;
                    this.isAttackMoveByCore = true;
                    this.attackCheckTime = 0.21f;
                    this.fxName = "boom1";
                    break;
                
                case "front_ground":
                    this.fxName = "boom1";
                    this.attackCheckTime = 0.45f;
                    break;
                
                case "kick":
                    this.fxName = "boom5";
                    this.fxRotation = this.baseTransform.rotation;
                    this.attackCheckTime = 0.43f;
                    break;
                
                case "slap_back":
                    this.fxName = "boom3";
                    this.attackCheckTime = 0.66f;
                    break;
                
                case "slap_face":
                    this.fxName = "boom3";
                    this.attackCheckTime = 0.655f;
                    break;
                
                case "stomp":
                    this.fxName = "boom2";
                    this.attackCheckTime = 0.42f;
                    break;
                
                case "bite":
                    this.fxName = "bite";
                    this.attackCheckTime = 0.6f;
                    break;
                
                case "bite_l":
                    this.fxName = "bite";
                    this.attackCheckTime = 0.4f;
                    break;
                
                case "bite_r":
                    this.fxName = "bite";
                    this.attackCheckTime = 0.4f;
                    break;
                
                case "jumper_0":
                    this.abnorma_jump_bite_horizon_v = Vector3.zero;
                    break;
                
                case "crawler_jump_0":
                    this.abnorma_jump_bite_horizon_v = Vector3.zero;
                    break;
                
                case "anti_AE_l":
                    this.attackCheckTimeA = 0.31f;
                    this.attackCheckTimeB = 0.4f;
                    this.leftHandAttack = true;
                    break;
                
                case "anti_AE_r":
                    this.attackCheckTimeA = 0.31f;
                    this.attackCheckTimeB = 0.4f;
                    this.leftHandAttack = false;
                    break;
                
                case "anti_AE_low_l":
                    this.attackCheckTimeA = 0.31f;
                    this.attackCheckTimeB = 0.4f;
                    this.leftHandAttack = true;
                    break;
                
                case "anti_AE_low_r":
                    this.attackCheckTimeA = 0.31f;
                    this.attackCheckTimeB = 0.4f;
                    this.leftHandAttack = false;
                    break;
                
                case "quick_turn_l":
                    this.attackCheckTimeA = 2f;
                    this.attackCheckTimeB = 2f;
                    this.isAttackMoveByCore = true;
                    break;
                
                case "quick_turn_r":
                    this.attackCheckTimeA = 2f;
                    this.attackCheckTimeB = 2f;
                    this.isAttackMoveByCore = true;
                    break;
                
                case "throw":
                    this.isAlarm = true;
                    this.chaseDistance = 99999f;
                    break;
            }
        }
        this.needFreshCorePosition = true;
    }

    private void Awake()
    {
        this.Cache();
        this.baseRigidBody.freezeRotation = true;
        this.baseRigidBody.useGravity = false;
    }

    public void StartLaughing()
    {
        if (!this.hasDie && this.abnormalType != AbnormalType.Crawler)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                photonView.RPC("laugh", PhotonTargets.All, 0f);
            }
            else if (this.state == TitanState.Idle || this.state == TitanState.Turning || this.state == TitanState.Chasing)
            {
                this.Laugh(0f);
            }
        }
    }

    public void GetTaunted(GameObject target, float time)
    {
        whoHasTauntMe = target;
        tauntTime = time;
        isAlarm = true;
    }

    private void Cache()
    {
        this.baseAudioSource = transform.Find("snd_titan_foot").GetComponent<AudioSource>();
        this.baseAnimation = animation;
        this.baseTransform = transform;
        this.baseRigidBody = rigidbody;
        this.baseColliders = new List<Collider>();
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            if (collider.name != "AABB")
            {
                this.baseColliders.Add(collider);
            }
        }
        GameObject obj2 = new GameObject {
            name = "PlayerDetectorRC"
        };
        CapsuleCollider collider2 = obj2.AddComponent<CapsuleCollider>();
        CapsuleCollider component = this.baseTransform.Find("AABB").GetComponent<CapsuleCollider>();
        collider2.center = component.center;
        collider2.radius = Math.Abs(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position.y - this.baseTransform.position.y);
        collider2.height = component.height * 1.2f;
        collider2.material = component.material;
        collider2.isTrigger = true;
        collider2.name = "PlayerDetectorRC";
        this.myTitanTrigger = obj2.AddComponent<TitanTrigger>();
        this.myTitanTrigger.isCollide = false;
        obj2.layer = 16;
        obj2.transform.parent = this.baseTransform.Find("AABB");
        obj2.transform.localPosition = new Vector3(0f, 0f, 0f);
        this.MultiplayerManager = FengGameManagerMKII.instance;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            this.baseGameObjectTransform = gameObject.transform;
        }
    }

    private void Chase()
    {
        this.state = TitanState.Chasing;
        this.isAlarm = true;
        this.crossFade(this.runAnimation, 0.5f);
    }

    private GameObject checkIfHitCrawlerMouth(Transform head, float rad)
    {
        float num = rad * this.myLevel;
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj2.GetComponent<TITAN_EREN>() == null && (obj2.GetComponent<HERO>() == null || !obj2.GetComponent<HERO>().isInvincible()))
            {
                float num3 = obj2.GetComponent<CapsuleCollider>().height * 0.5f;
                if (Vector3.Distance(obj2.transform.position + Vector3.up * num3, head.position - Vector3.up * 1.5f * this.myLevel) < num + num3)
                {
                    return obj2;
                }
            }
        }
        return null;
    }

    private GameObject checkIfHitHand(Transform hand)
    {
        float num = 2.4f * this.myLevel;
        foreach (Collider collider in Physics.OverlapSphere(hand.GetComponent<SphereCollider>().transform.position, num + 1f))
        {
            if (collider.transform.root.CompareTag("Player"))
            {
                GameObject gameObject = collider.transform.root.gameObject;
                if (gameObject.GetComponent<TITAN_EREN>() != null)
                {
                    if (!gameObject.GetComponent<TITAN_EREN>().isHit)
                    {
                        gameObject.GetComponent<TITAN_EREN>().hitByTitan();
                    }
                }
                else if (gameObject.GetComponent<HERO>() != null && !gameObject.GetComponent<HERO>().isInvincible())
                {
                    return gameObject;
                }
            }
        }
        return null;
    }

    private GameObject checkIfHitHead(Transform head, float rad)
    {
        float num = rad * this.myLevel;
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj2.GetComponent<TITAN_EREN>() == null && (obj2.GetComponent<HERO>() == null || !obj2.GetComponent<HERO>().isInvincible()))
            {
                float num3 = obj2.GetComponent<CapsuleCollider>().height * 0.5f;
                if (Vector3.Distance(obj2.transform.position + Vector3.up * num3, head.position + Vector3.up * 1.5f * this.myLevel) < num + num3)
                {
                    return obj2;
                }
            }
        }
        return null;
    }

    private void crossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            photonView.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
        }
    }

    public bool die()
    {
        if (hasDie)
            return false;
        
        hasDie = true;
        FengGameManagerMKII.instance.OneTitanDown(string.Empty, false);
        this.dieAnimation();
        return true;
    }

    private void dieAnimation()
    {
        if (!animation.IsPlaying("sit_idle") && !animation.IsPlaying("sit_hit_eye"))
        {
            if (this.abnormalType == AbnormalType.Crawler)
            {
                this.crossFade("crawler_die", 0.2f);
            }
            else if (this.abnormalType == AbnormalType.Normal)
            {
                this.crossFade("die_front", 0.05f);
            }
            else if (animation.IsPlaying("attack_abnormal_jump") && animation["attack_abnormal_jump"].normalizedTime > 0.7f || animation.IsPlaying("attack_abnormal_getup") && animation["attack_abnormal_getup"].normalizedTime < 0.7f || animation.IsPlaying("tired"))
            {
                this.crossFade("die_ground", 0.2f);
            }
            else
            {
                this.crossFade("die_back", 0.05f);
            }
        }
        else
        {
            this.crossFade("sit_die", 0.1f);
        }
    }

    public void dieBlow(Vector3 attacker, float hitPauseTime)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            this.dieBlowFunc(attacker, hitPauseTime);
            if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
            {
                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            }
        }
        else
        {
            photonView.RPC("dieBlowRPC", PhotonTargets.All, attacker, hitPauseTime);
        }
    }

    public void dieBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (!this.hasDie)
        {
            transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - transform.position).eulerAngles.y, 0f);
            this.hasDie = true;
            this.hitAnimation = "die_blow";
            this.hitPause = hitPauseTime;
            this.playAnimation(this.hitAnimation);
            animation[this.hitAnimation].time = 0f;
            animation[this.hitAnimation].speed = 0f;
            this.needFreshCorePosition = true;
            FengGameManagerMKII.instance.OneTitanDown(string.Empty, false);
            if (photonView.isMine)
            {
                if (this.grabbedTarget != null)
                {
                    this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
                }
                if (this.nonAI)
                {
                    this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null, true, false);
                    this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
                    this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                    ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                    {
                        { PlayerProperty.Dead, true }
                    };
                    Player.Self.SetCustomProperties(propertiesToSet);
                    propertiesToSet = new ExitGames.Client.Photon.Hashtable
                    {
                        { PlayerProperty.Deaths, (int)Player.Self.Properties.Deaths + 1 }
                    };
                    Player.Self.SetCustomProperties(propertiesToSet);
                }
            }
        }
    }

    [RPC]
    private void DieBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (photonView.isMine)
        {
            Vector3 vector = attacker - transform.position;
            if (vector.magnitude < 80f)
            {
                this.dieBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    public void DieByCannon(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            int damage = 0;
            if (PhotonNetwork.isMasterClient)
            {
                this.OnTitanDie(view);
            }
            if (this.nonAI)
            {
                FengGameManagerMKII.instance.TitanGetKill(view.owner, damage, Player.Self.Properties.Name, null);
            }
            else
            {
                FengGameManagerMKII.instance.TitanGetKill(view.owner, damage, name, null);
            }
        }
        else
        {
            FengGameManagerMKII.instance.photonView.RPC("netShowDamage", view.owner, new object[] { this.speed });
        }
    }

    public void dieHeadBlow(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType != AbnormalType.Crawler)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                this.dieHeadBlowFunc(attacker, hitPauseTime);
                if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
                {
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                }
            }
            else
            {
                photonView.RPC("dieHeadBlowRPC", PhotonTargets.All, attacker, hitPauseTime);
            }
        }
    }

    public void dieHeadBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (!this.hasDie)
        {
            GameObject obj2;
            this.playSound("snd_titan_head_blow");
            transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - transform.position).eulerAngles.y, 0f);
            this.hasDie = true;
            this.hitAnimation = "die_headOff";
            this.hitPause = hitPauseTime;
            this.playAnimation(this.hitAnimation);
            animation[this.hitAnimation].time = 0f;
            animation[this.hitAnimation].speed = 0f;
            FengGameManagerMKII.instance.OneTitanDown(string.Empty, false);
            this.needFreshCorePosition = true;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                obj2 = PhotonNetwork.Instantiate("bloodExplore", this.head.position + Vector3.up * 1f * this.myLevel, Quaternion.Euler(270f, 0f, 0f), 0);
            }
            else
            {
                obj2 = (GameObject) Instantiate(Resources.Load("bloodExplore"), this.head.position + Vector3.up * 1f * this.myLevel, Quaternion.Euler(270f, 0f, 0f));
            }
            obj2.transform.localScale = transform.localScale;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                obj2 = PhotonNetwork.Instantiate("bloodsplatter", this.head.position, Quaternion.Euler(270f + this.neck.rotation.eulerAngles.x, this.neck.rotation.eulerAngles.y, this.neck.rotation.eulerAngles.z), 0);
            }
            else
            {
                obj2 = (GameObject) Instantiate(Resources.Load("bloodsplatter"), this.head.position, Quaternion.Euler(270f + this.neck.rotation.eulerAngles.x, this.neck.rotation.eulerAngles.y, this.neck.rotation.eulerAngles.z));
            }
            obj2.transform.localScale = transform.localScale;
            obj2.transform.parent = this.neck;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                obj2 = PhotonNetwork.Instantiate("FX/justSmoke", this.neck.position, Quaternion.Euler(270f, 0f, 0f), 0);
            }
            else
            {
                obj2 = (GameObject) Instantiate(Resources.Load("FX/justSmoke"), this.neck.position, Quaternion.Euler(270f, 0f, 0f));
            }
            obj2.transform.parent = this.neck;
            if (photonView.isMine)
            {
                if (this.grabbedTarget != null)
                {
                    this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
                }
                if (this.nonAI)
                {
                    this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null, true, false);
                    this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
                    this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                    ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                    {
                        { PlayerProperty.Dead, true }
                    };
                    Player.Self.SetCustomProperties(propertiesToSet);
                    propertiesToSet = new ExitGames.Client.Photon.Hashtable
                    {
                        { PlayerProperty.Deaths, (int)Player.Self.Properties.Deaths + 1 }
                    };
                    Player.Self.SetCustomProperties(propertiesToSet);
                }
            }
        }
    }

    [RPC]
    private void DieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (photonView.isMine)
        {
            Vector3 vector = attacker - this.neck.position;
            if (vector.magnitude < this.lagMax)
            {
                this.dieHeadBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    private void eat()
    {
        this.state = TitanState.Eating;
        this.attacked = false;
        if (this.isGrabHandLeft)
        {
            this.attackAnimation = "eat_l";
            this.crossFade("eat_l", 0.1f);
        }
        else
        {
            this.attackAnimation = "eat_r";
            this.crossFade("eat_r", 0.1f);
        }
    }

    private void eatSet(GameObject grabTarget)
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && (IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine) || !grabTarget.GetComponent<HERO>().IsGrabbed)
        {
            this.GrabToRight();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                photonView.RPC("grabToRight", PhotonTargets.Others, new object[0]);
                grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
                grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, photonView.viewID, false);
            }
            else
            {
                grabTarget.GetComponent<HERO>().grabbed(gameObject, false);
                grabTarget.GetComponent<HERO>().animation.Play("grabbed");
            }
        }
    }

    private void eatSetL(GameObject grabTarget)
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && (IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine) || !grabTarget.GetComponent<HERO>().IsGrabbed)
        {
            this.GrabToLeft();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                photonView.RPC("grabToLeft", PhotonTargets.Others, new object[0]);
                grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
                grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, photonView.viewID, true);
            }
            else
            {
                grabTarget.GetComponent<HERO>().grabbed(gameObject, true);
                grabTarget.GetComponent<HERO>().animation.Play("grabbed");
            }
        }
    }

    private bool ExecuteAttack(string decidedAction)
    {
        if (decidedAction != null)
        {
            switch (decidedAction)
            {
                case "grab_ground_front_l":
                    this.Grab("ground_front_l");
                    return true;

                case "grab_ground_front_r":
                    this.Grab("ground_front_r");
                    return true;

                case "grab_ground_back_l":
                    this.Grab("ground_back_l");
                    return true;

                case "grab_ground_back_r":
                    this.Grab("ground_back_r");
                    return true;

                case "grab_head_front_l":
                    this.Grab("head_front_l");
                    return true;

                case "grab_head_front_r":
                    this.Grab("head_front_r");
                    return true;

                case "grab_head_back_l":
                    this.Grab("head_back_l");
                    return true;

                case "grab_head_back_r":
                    this.Grab("head_back_r");
                    return true;

                case "attack_abnormal_jump":
                    this.Attack("abnormal_jump");
                    return true;

                case "attack_combo":
                    this.Attack("combo_1");
                    return true;

                case "attack_front_ground":
                    this.Attack("front_ground");
                    return true;

                case "attack_kick":
                    this.Attack("kick");
                    return true;

                case "attack_slap_back":
                    this.Attack("slap_back");
                    return true;

                case "attack_slap_face":
                    this.Attack("slap_face");
                    return true;

                case "attack_stomp":
                    this.Attack("stomp");
                    return true;

                case "attack_bite":
                    this.Attack("bite");
                    return true;

                case "attack_bite_l":
                    this.Attack("bite_l");
                    return true;

                case "attack_bite_r":
                    this.Attack("bite_r");
                    return true;
            }
        }
        return false;
    }

    public void Explode()
    {
        if (FengGameManagerMKII.settings.IsExplodeMode && this.hasDie && this.dieTime >= 1f && !this.hasExplode)
        {
            int num = 0;
            float num2 = this.myLevel * 10f;
            if (this.abnormalType == AbnormalType.Crawler)
            {
                if (this.dieTime >= 2f)
                {
                    this.hasExplode = true;
                    num2 = 0f;
                    num = 1;
                }
            }
            else
            {
                num = 1;
                this.hasExplode = true;
            }
            if (num == 1)
            {
                Vector3 position = this.baseTransform.position + Vector3.up * num2;
                PhotonNetwork.Instantiate("FX/Thunder", position, Quaternion.Euler(270f, 0f, 0f), 0);
                PhotonNetwork.Instantiate("FX/boom1", position, Quaternion.Euler(270f, 0f, 0f), 0);
                foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (Vector3.Distance(obj2.transform.position, position) < FengGameManagerMKII.settings.ExplodeRadius)
                    {
                        obj2.GetComponent<HERO>().markDie();
                        obj2.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Server " });
                    }
                }
            }
        }
    }

    private void FindNearestFacingHero()
    {
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        Vector3 position = this.baseTransform.position;
        float num3 = this.abnormalType != AbnormalType.Normal ? 180f : 100f;
        foreach (HERO hero in this.MultiplayerManager.GetPlayers())
        {
            GameObject gameObject = hero.gameObject;
            float num5 = Vector3.Distance(gameObject.transform.position, position);
            if (num5 < positiveInfinity)
            {
                Vector3 vector2 = gameObject.transform.position - this.baseTransform.position;
                float current = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                float f = Mathf.Abs(-Mathf.DeltaAngle(current, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f));
                if (f < num3)
                {
                    obj2 = gameObject;
                    positiveInfinity = num5;
                }
            }
        }
        if (obj2 != null)
        {
            GameObject myHero = this.myHero;
            this.myHero = obj2;
            if (myHero != this.myHero && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
            {
                if (this.myHero == null)
                    photonView.RPC("setMyTarget", PhotonTargets.Others, -1);
                else
                    photonView.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
            }
            this.tauntTime = 5f;
        }
    }

    private void FindNeareastHero()
    {
        GameObject oldHero = this.myHero;
        this.myHero = this.GetNearestHero();
        if (this.myHero != oldHero && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            if (this.myHero == null)
                photonView.RPC("setMyTarget", PhotonTargets.Others, -1);
            else 
                photonView.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
        }
        this.oldHeadRotation = this.head.rotation;
    }

    private void FixedUpdate()
    {
        if (!(IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine))
        {
            this.baseRigidBody.AddForce(new Vector3(0f, -this.gravity * this.baseRigidBody.mass, 0f));
            if (this.needFreshCorePosition)
            {
                this.oldCorePosition = this.baseTransform.position - this.baseTransform.Find("Amarture/Core").position;
                this.needFreshCorePosition = false;
            }
            if (this.hasDie)
            {
                if (this.hitPause <= 0f && this.baseAnimation.IsPlaying("die_headOff"))
                {
                    Vector3 vector = this.baseTransform.position - this.baseTransform.Find("Amarture/Core").position - this.oldCorePosition;
                    this.baseRigidBody.velocity = vector / Time.deltaTime + Vector3.up * this.baseRigidBody.velocity.y;
                }
                this.oldCorePosition = this.baseTransform.position - this.baseTransform.Find("Amarture/Core").position;
            }
            else if (!((this.state != TitanState.Attacking || !this.isAttackMoveByCore) && this.state != TitanState.Hitting))
            {
                Vector3 vector2 = this.baseTransform.position - this.baseTransform.Find("Amarture/Core").position - this.oldCorePosition;
                this.baseRigidBody.velocity = vector2 / Time.deltaTime + Vector3.up * this.baseRigidBody.velocity.y;
                this.oldCorePosition = this.baseTransform.position - this.baseTransform.Find("Amarture/Core").position;
            }
            if (this.hasDie)
            {
                if (this.hitPause > 0f)
                {
                    this.hitPause -= Time.deltaTime;
                    if (this.hitPause <= 0f)
                    {
                        this.baseAnimation[this.hitAnimation].speed = 1f;
                        this.hitPause = 0f;
                    }
                }
                else if (this.baseAnimation.IsPlaying("die_blow"))
                {
                    if (this.baseAnimation["die_blow"].normalizedTime < 0.55f)
                    {
                        this.baseRigidBody.velocity = -this.baseTransform.forward * 300f + Vector3.up * this.baseRigidBody.velocity.y;
                    }
                    else if (this.baseAnimation["die_blow"].normalizedTime < 0.83f)
                    {
                        this.baseRigidBody.velocity = -this.baseTransform.forward * 100f + Vector3.up * this.baseRigidBody.velocity.y;
                    }
                    else
                    {
                        this.baseRigidBody.velocity = Vector3.up * this.baseRigidBody.velocity.y;
                    }
                }
            }
            else
            {
                if (this.nonAI && !IN_GAME_MAIN_CAMERA.isPausing && (this.state == TitanState.Idle || this.state == TitanState.Attacking && this.attackAnimation == "jumper_1"))
                {
                    Vector3 zero = Vector3.zero;
                    if (this.controller.targetDirection != -874f)
                    {
                        bool flag2 = false;
                        if (this.stamina < 5f)
                        {
                            flag2 = true;
                        }
                        else if (!(this.stamina >= 40f || this.baseAnimation.IsPlaying("run_abnormal") || this.baseAnimation.IsPlaying("crawler_run")))
                        {
                            flag2 = true;
                        }
                        if (this.controller.isWALKDown || flag2)
                        {
                            zero = this.baseTransform.forward * this.speed * Mathf.Sqrt(this.myLevel) * 0.2f;
                        }
                        else
                        {
                            zero = this.baseTransform.forward * this.speed * Mathf.Sqrt(this.myLevel);
                        }
                        this.baseGameObjectTransform.rotation = Quaternion.Lerp(this.baseGameObjectTransform.rotation, Quaternion.Euler(0f, this.controller.targetDirection, 0f), this.speed * 0.15f * Time.deltaTime);
                        if (this.state == TitanState.Idle)
                        {
                            if (this.controller.isWALKDown || flag2)
                            {
                                if (this.abnormalType == AbnormalType.Crawler)
                                {
                                    if (!this.baseAnimation.IsPlaying("crawler_run"))
                                    {
                                        this.crossFade("crawler_run", 0.1f);
                                    }
                                }
                                else if (!this.baseAnimation.IsPlaying("run_walk"))
                                {
                                    this.crossFade("run_walk", 0.1f);
                                }
                            }
                            else if (this.abnormalType == AbnormalType.Crawler)
                            {
                                if (!this.baseAnimation.IsPlaying("crawler_run"))
                                {
                                    this.crossFade("crawler_run", 0.1f);
                                }
                                GameObject obj2 = this.checkIfHitCrawlerMouth(this.head, 2.2f);
                                if (obj2 != null)
                                {
                                    Vector3 position = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                    {
                                        obj2.GetComponent<HERO>().die((obj2.transform.position - position) * 15f * this.myLevel, false);
                                    }
                                    else if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine || obj2.GetComponent<HERO>().HasDied()))
                                    {
                                        obj2.GetComponent<HERO>().markDie();
                                        obj2.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, 
                                            (obj2.transform.position - position) * 15f * this.myLevel, 
                                            true, 
                                            !this.nonAI ? -1 : photonView.viewID, 
                                            name, 
                                            true);
                                    }
                                }
                            }
                            else if (!this.baseAnimation.IsPlaying("run_abnormal"))
                            {
                                this.crossFade("run_abnormal", 0.1f);
                            }
                        }
                    }
                    else if (this.state == TitanState.Idle)
                    {
                        if (this.abnormalType == AbnormalType.Crawler)
                        {
                            if (!this.baseAnimation.IsPlaying("crawler_idle"))
                            {
                                this.crossFade("crawler_idle", 0.1f);
                            }
                        }
                        else if (!this.baseAnimation.IsPlaying("idle"))
                        {
                            this.crossFade("idle", 0.1f);
                        }
                        zero = Vector3.zero;
                    }
                    switch (this.state)
                    {
                        case TitanState.Idle:
                            Vector3 velocity = this.baseRigidBody.velocity;
                            Vector3 force = zero - velocity;
                            force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                            force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                            force.y = 0f;
                            this.baseRigidBody.AddForce(force, ForceMode.VelocityChange);
                            break;
                        case TitanState.Attacking when this.attackAnimation == "jumper_0":
                            Vector3 vector7 = this.baseRigidBody.velocity;
                            Vector3 vector8 = zero * 0.8f - vector7;
                            vector8.x = Mathf.Clamp(vector8.x, -this.maxVelocityChange, this.maxVelocityChange);
                            vector8.z = Mathf.Clamp(vector8.z, -this.maxVelocityChange, this.maxVelocityChange);
                            vector8.y = 0f;
                            this.baseRigidBody.AddForce(vector8, ForceMode.VelocityChange);
                            break;
                    }
                }
                if (!(this.abnormalType != AbnormalType.Abnormal && this.abnormalType != AbnormalType.Jumper || this.nonAI || this.state != TitanState.Attacking || this.attackAnimation != "jumper_0"))
                {
                    Vector3 vector9 = this.baseTransform.forward * this.speed * this.myLevel * 0.5f;
                    Vector3 vector10 = this.baseRigidBody.velocity;
                    if (this.baseAnimation["attack_jumper_0"].normalizedTime <= 0.28f || this.baseAnimation["attack_jumper_0"].normalizedTime >= 0.8f)
                    {
                        vector9 = Vector3.zero;
                    }
                    Vector3 vector11 = vector9 - vector10;
                    vector11.x = Mathf.Clamp(vector11.x, -this.maxVelocityChange, this.maxVelocityChange);
                    vector11.z = Mathf.Clamp(vector11.z, -this.maxVelocityChange, this.maxVelocityChange);
                    vector11.y = 0f;
                    this.baseRigidBody.AddForce(vector11, ForceMode.VelocityChange);
                }
                if (this.state == TitanState.Chasing || this.state == TitanState.Wandering || this.state == TitanState.GoingToCheckpoint || this.state == TitanState.GoingToPVP || this.state == TitanState.RunningRandomly)
                {
                    Vector3 vector12 = this.baseTransform.forward * this.speed;
                    Vector3 vector13 = this.baseRigidBody.velocity;
                    Vector3 vector14 = vector12 - vector13;
                    vector14.x = Mathf.Clamp(vector14.x, -this.maxVelocityChange, this.maxVelocityChange);
                    vector14.z = Mathf.Clamp(vector14.z, -this.maxVelocityChange, this.maxVelocityChange);
                    vector14.y = 0f;
                    this.baseRigidBody.AddForce(vector14, ForceMode.VelocityChange);
                    if (!this.stuck && this.abnormalType != AbnormalType.Crawler && !this.nonAI)
                    {
                        if (this.baseAnimation.IsPlaying(this.runAnimation) && this.baseRigidBody.velocity.magnitude < this.speed * 0.5f)
                        {
                            this.stuck = true;
                            this.stuckTime = 2f;
                            this.stuckTurnAngle = UnityEngine.Random.Range(0, 2) * 140f - 70f;
                        }
                        if (this.state == TitanState.Chasing && this.myHero != null && this.myDistance > this.attackDistance && this.myDistance < 150f)
                        {
                            float num = 0.05f;
                            if (this.myDifficulty > 1)
                            {
                                num += 0.05f;
                            }
                            if (this.abnormalType != AbnormalType.Normal)
                            {
                                num += 0.1f;
                            }
                            if (UnityEngine.Random.Range(0f, 1f) < num)
                            {
                                this.stuck = true;
                                this.stuckTime = 1f;
                                float num2 = UnityEngine.Random.Range(20f, 50f);
                                this.stuckTurnAngle = UnityEngine.Random.Range(0, 2) * num2 * 2f - num2;
                            }
                        }
                    }
                    float current;
                    switch (this.state)
                    {
                        case TitanState.Wandering:
                            current = this.baseTransform.rotation.eulerAngles.y - 90f;
                            break;
                        
                        case TitanState.GoingToCheckpoint:
                        case TitanState.GoingToPVP:
                        case TitanState.RunningRandomly:
                            Vector3 vector16 = this.targetCheckPt - this.baseTransform.position;
                            current = -Mathf.Atan2(vector16.z, vector16.x) * 57.29578f;
                            break;
                        
                        default:
                            if (this.myHero == null)
                                return;
                            
                            Vector3 vector17 = this.myHero.transform.position - this.baseTransform.position;
                            current = -Mathf.Atan2(vector17.z, vector17.x) * 57.29578f;
                            break;
                    }
                    if (this.stuck)
                    {
                        this.stuckTime -= Time.deltaTime;
                        
                        if (this.stuckTime < 0f)
                            this.stuck = false;
                        if (this.stuckTurnAngle > 0f)
                            this.stuckTurnAngle -= Time.deltaTime * 10f;
                        else
                            this.stuckTurnAngle += Time.deltaTime * 10f;
                        
                        current += this.stuckTurnAngle;
                    }
                    float num4 = -Mathf.DeltaAngle(current, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f);
                    
                    if (this.abnormalType == AbnormalType.Crawler)
                        this.baseGameObjectTransform.rotation = Quaternion.Lerp(this.baseGameObjectTransform.rotation, Quaternion.Euler(0f, this.baseGameObjectTransform.rotation.eulerAngles.y + num4, 0f), this.speed * 0.3f * Time.deltaTime / this.myLevel);
                    else
                        this.baseGameObjectTransform.rotation = Quaternion.Lerp(this.baseGameObjectTransform.rotation, Quaternion.Euler(0f, this.baseGameObjectTransform.rotation.eulerAngles.y + num4, 0f), this.speed * 0.5f * Time.deltaTime / this.myLevel);
                }
            }
        }
    }

    private string[] GetAttackStrategy()
    {
        string[] strArray = null;
        if (!this.isAlarm && this.myHero.transform.position.y + 3f > this.neck.position.y + 10f * this.myLevel)
            return null;
        
        if (this.myHero.transform.position.y > this.neck.position.y - 3f * this.myLevel)
        {
            if (this.myDistance < this.attackDistance * 0.5f)
            {
                if (Vector3.Distance(this.myHero.transform.position, transform.Find("chkOverHead").position) < 3.6f * this.myLevel)
                {
                    if (this.between2 > 0f)
                        return new[] { "grab_head_front_r" };
                    else
                        return new[] { "grab_head_front_l" };
                }
                if (Mathf.Abs(this.between2) < 90f)
                {
                    if (Mathf.Abs(this.between2) < 30f)
                    {
                        if (Vector3.Distance(this.myHero.transform.position, transform.Find("chkFront").position) < 2.5f * this.myLevel)
                        {
                            return new[] { "attack_bite", "attack_bite", "attack_slap_face" };
                        }
                    }
                    else if (this.between2 > 0f)
                    {
                        if (Vector3.Distance(this.myHero.transform.position, transform.Find("chkFrontRight").position) < 2.5f * this.myLevel)
                        {
                            return new[] { "attack_bite_r" };
                        }
                    }
                    else if (Vector3.Distance(this.myHero.transform.position, transform.Find("chkFrontLeft").position) < 2.5f * this.myLevel)
                    {
                        return new[] { "attack_bite_l" };
                    }
                }
                else if (this.between2 > 0f)
                {
                    if (Vector3.Distance(this.myHero.transform.position, transform.Find("chkBackRight").position) < 2.8f * this.myLevel)
                    {
                        return new[] { "grab_head_back_r", "grab_head_back_r", "attack_slap_back" };
                    }
                }
                else if (Vector3.Distance(this.myHero.transform.position, transform.Find("chkBackLeft").position) < 2.8f * this.myLevel)
                {
                    return new[] { "grab_head_back_l", "grab_head_back_l", "attack_slap_back" };
                }
            }

            if (this.abnormalType != AbnormalType.Normal && this.abnormalType != AbnormalType.Punk)
            {
                if (this.abnormalType != AbnormalType.Abnormal && this.abnormalType != AbnormalType.Jumper)
                    return null;
                
                if (this.myDifficulty <= 0 && UnityEngine.Random.Range(0, 100) >= 50)
                    return null;
                
                return new[] { "attack_abnormal_jump" };
            }
            
            if ((this.myDifficulty > 0 || UnityEngine.Random.Range(0, 1000) < 3) && Mathf.Abs(this.between2) < 60f)
                return new[] { "attack_combo" };

            return null;
        }
        if (Mathf.Abs(this.between2) < 90f)
        {
            if (this.between2 > 0f)
            {
                if (this.myDistance < this.attackDistance * 0.25f)
                {
                    switch (this.abnormalType)
                    {
                        case AbnormalType.Punk:
                            return new[] { "attack_kick", "attack_stomp" };
                        case AbnormalType.Normal:
                            return new[] { "attack_front_ground", "attack_stomp" };
                        default: 
                            return new[] { "attack_kick" };
                    }
                }
                if (this.myDistance < this.attackDistance * 0.5f)
                {
                    switch (abnormalType)
                    {
                        case AbnormalType.Normal:
                            return new[] { "grab_ground_front_r", "grab_ground_front_r", "attack_stomp" };
                        default:
                            return new[] { "grab_ground_front_r", "grab_ground_front_r", "attack_abnormal_jump" };
                    }
                }
                switch (this.abnormalType)
                {
                    case AbnormalType.Punk:
                        return new[] { "attack_combo", "attack_combo", "attack_abnormal_jump" };
                    case AbnormalType.Normal when this.myDifficulty > 0:
                        return new[] { "attack_front_ground", "attack_combo", "attack_combo" };
                    case AbnormalType.Normal:
                        return new[] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" };
                    default:
                        return new[] { "attack_abnormal_jump" };
                }
            }
            if (this.myDistance < this.attackDistance * 0.25f)
            {
                switch (this.abnormalType)
                {
                    case AbnormalType.Punk:
                        return new[] { "attack_kick", "attack_stomp" };
                    case AbnormalType.Normal:
                        return new[] { "attack_front_ground", "attack_stomp" };
                    default:
                        return new[] { "attack_kick" };
                }
            }
            if (this.myDistance < this.attackDistance * 0.5f)
            {
                switch (this.abnormalType)
                {
                    case AbnormalType.Normal:
                        return new[] { "grab_ground_front_l", "grab_ground_front_l", "attack_stomp" };
                    default:
                        return new[] {"grab_ground_front_l", "grab_ground_front_l", "attack_abnormal_jump"};
                }
            }
            switch (this.abnormalType)
            {
                case AbnormalType.Punk:
                    return new[] { "attack_combo", "attack_combo", "attack_abnormal_jump" };
                case AbnormalType.Normal when this.myDifficulty > 0:
                    return new[] { "attack_front_ground", "attack_combo", "attack_combo" };
                case AbnormalType.Normal:
                    return new[] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" };
                default:
                    return new[] { "attack_abnormal_jump" };
            }
        }
        
        if (this.myDistance >= this.attackDistance * 0.5f)
            return null;
        
        if (this.between2 > 0f)
        {
            switch (this.abnormalType)
            {
                case AbnormalType.Normal:
                    return new[] { "grab_ground_back_r" };
                default:
                    return new[] { "grab_ground_back_r" };
            }
        }

        switch (this.abnormalType)
        {
            case AbnormalType.Normal:
                return new[] {"grab_ground_back_l"};
            default:
                return new[] {"grab_ground_back_l"};
        }
    }

    private void GetDown()
    {
        this.state = TitanState.Down;
        this.isAlarm = true;
        this.playAnimation("sit_hunt_down");
        this.getdownTime = UnityEngine.Random.Range(3f, 5f);
    }

    private GameObject GetNearestHero()
    {
        GameObject returnPlayer = null;
        float lowestDistance = float.PositiveInfinity;
        foreach (HERO hero in MultiplayerManager.GetPlayers())
        {
            if (hero == null)
                continue;
            
            float distance = Vector3.Distance(hero.gameObject.transform.position, baseTransform.position);
            if (distance < lowestDistance)
            {
                returnPlayer = hero.gameObject;
                lowestDistance = distance;
            }
        }
        return returnPlayer;
    }

    private static int PunkNumber => 
        GameObject.FindGameObjectsWithTag("titan").Count(obj2 => obj2.GetComponent<TITAN>() != null && obj2.GetComponent<TITAN>().name == "Punk");

    private void Grab(string type)
    {
        this.state = TitanState.Grabbing;
        this.attacked = false;
        this.isAlarm = true;
        this.attackAnimation = type;
        this.crossFade("grab_" + type, 0.1f);
        this.isGrabHandLeft = true;
        this.grabbedTarget = null;
        string key = type;
        if (key != null)
        {
            switch (key)
            {
                case "ground_back_l":
                    this.attackCheckTimeA = 0.34f;
                    this.attackCheckTimeB = 0.49f;
                    break;
                case "ground_back_r":
                    this.attackCheckTimeA = 0.34f;
                    this.attackCheckTimeB = 0.49f;
                    this.isGrabHandLeft = false;
                    break;
                case "ground_front_l":
                    this.attackCheckTimeA = 0.37f;
                    this.attackCheckTimeB = 0.6f;
                    break;
                case "ground_front_r":
                    this.attackCheckTimeA = 0.37f;
                    this.attackCheckTimeB = 0.6f;
                    this.isGrabHandLeft = false;
                    break;
                case "head_back_l":
                    this.attackCheckTimeA = 0.45f;
                    this.attackCheckTimeB = 0.5f;
                    this.isGrabHandLeft = false;
                    break;
                case "head_back_r":
                    this.attackCheckTimeA = 0.45f;
                    this.attackCheckTimeB = 0.5f;
                    break;
                case "head_front_l":
                    this.attackCheckTimeA = 0.38f;
                    this.attackCheckTimeB = 0.55f;
                    break;
                case "head_front_r":
                    this.attackCheckTimeA = 0.38f;
                    this.attackCheckTimeB = 0.55f;
                    this.isGrabHandLeft = false;
                    break;
            }
        }
        if (this.isGrabHandLeft)
        {
            this.currentGrabHand = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
        }
        else
        {
            this.currentGrabHand = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        }
    }

    [RPC]
    public void GrabbedTargetEscape()
    {
        this.grabbedTarget = null;
    }

    [RPC]
    public void GrabToLeft()
    {
        Transform transform = this.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
        this.grabTF.transform.parent = transform;
        this.grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
        this.grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
        Transform transform1 = this.grabTF.transform;
        transform1.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
        Transform transform2 = this.grabTF.transform;
        transform2.localPosition -= Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
        Transform transform3 = this.grabTF.transform;
        transform3.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
        this.grabTF.transform.localRotation = Quaternion.Euler(this.grabTF.transform.localRotation.eulerAngles.x, this.grabTF.transform.localRotation.eulerAngles.y + 180f, this.grabTF.transform.localRotation.eulerAngles.z + 180f);
    }

    [RPC]
    public void GrabToRight()
    {
        Transform transform = this.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        this.grabTF.transform.parent = transform;
        this.grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
        this.grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
        Transform transform1 = this.grabTF.transform;
        transform1.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
        Transform transform2 = this.grabTF.transform;
        transform2.localPosition += Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
        Transform transform3 = this.grabTF.transform;
        transform3.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
        this.grabTF.transform.localRotation = Quaternion.Euler(this.grabTF.transform.localRotation.eulerAngles.x, this.grabTF.transform.localRotation.eulerAngles.y + 180f, this.grabTF.transform.localRotation.eulerAngles.z);
    }

    public void HeadMovement()
    {
        if (!this.hasDie)
        {
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
            {
                if (photonView.isMine)
                {
                    this.targetHeadRotation = this.head.rotation;
                    bool flag2 = false;
                    if (this.abnormalType != AbnormalType.Crawler && this.state != TitanState.Attacking && this.state != TitanState.Down && this.state != TitanState.Hitting && (this.state != TitanState.Recovering && this.state != TitanState.Eating) && this.state != TitanState.Blinded && !this.hasDie && this.myDistance < 100f && this.myHero != null)
                    {
                        Vector3 vector = this.myHero.transform.position - transform.position;
                        this.angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                        float num = -Mathf.DeltaAngle(this.angle, transform.rotation.eulerAngles.y - 90f);
                        num = Mathf.Clamp(num, -40f, 40f);
                        float y = this.neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
                        float num3 = Mathf.Atan2(y, this.myDistance) * 57.29578f;
                        num3 = Mathf.Clamp(num3, -40f, 30f);
                        this.targetHeadRotation = Quaternion.Euler(this.head.rotation.eulerAngles.x + num3, this.head.rotation.eulerAngles.y + num, this.head.rotation.eulerAngles.z);
                        if (!this.asClientLookTarget)
                        {
                            this.asClientLookTarget = true;
                            photonView.RPC("setIfLookTarget", PhotonTargets.Others, true);
                        }
                        flag2 = true;
                    }
                    if (!(flag2 || !this.asClientLookTarget))
                    {
                        this.asClientLookTarget = false;
                        object[] objArray3 = new object[] { false };
                        photonView.RPC("setIfLookTarget", PhotonTargets.Others, objArray3);
                    }
                    if (this.state == TitanState.Attacking || this.state == TitanState.Hitting || this.state == TitanState.Blinded)
                    {
                        this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 20f);
                    }
                    else
                    {
                        this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
                    }
                }
                else
                {
                    bool flag3;
                    if (flag3 = this.myHero != null)
                    {
                        this.myDistance = Mathf.Sqrt((this.myHero.transform.position.x - this.baseTransform.position.x) * (this.myHero.transform.position.x - this.baseTransform.position.x) + (this.myHero.transform.position.z - this.baseTransform.position.z) * (this.myHero.transform.position.z - this.baseTransform.position.z));
                    }
                    else
                    {
                        this.myDistance = float.MaxValue;
                    }
                    this.targetHeadRotation = this.head.rotation;
                    if (this.asClientLookTarget && flag3 && this.myDistance < 100f)
                    {
                        Vector3 vector2 = this.myHero.transform.position - this.baseTransform.position;
                        this.angle = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        float num4 = -Mathf.DeltaAngle(this.angle, this.baseTransform.rotation.eulerAngles.y - 90f);
                        num4 = Mathf.Clamp(num4, -40f, 40f);
                        float num5 = this.neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
                        float num6 = Mathf.Atan2(num5, this.myDistance) * 57.29578f;
                        num6 = Mathf.Clamp(num6, -40f, 30f);
                        this.targetHeadRotation = Quaternion.Euler(this.head.rotation.eulerAngles.x + num6, this.head.rotation.eulerAngles.y + num4, this.head.rotation.eulerAngles.z);
                    }
                    if (!this.hasDie)
                    {
                        this.oldHeadRotation = Quaternion.Slerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
                    }
                }
            }
            else
            {
                this.targetHeadRotation = this.head.rotation;
                if (this.abnormalType != AbnormalType.Crawler && this.state != TitanState.Attacking && this.state != TitanState.Down && this.state != TitanState.Hitting && (this.state != TitanState.Recovering && this.state != TitanState.Blinded) && !this.hasDie && this.myDistance < 100f && this.myHero != null)
                {
                    Vector3 vector3 = this.myHero.transform.position - transform.position;
                    this.angle = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
                    float num7 = -Mathf.DeltaAngle(this.angle, transform.rotation.eulerAngles.y - 90f);
                    num7 = Mathf.Clamp(num7, -40f, 40f);
                    float num8 = this.neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
                    float num9 = Mathf.Atan2(num8, this.myDistance) * 57.29578f;
                    num9 = Mathf.Clamp(num9, -40f, 30f);
                    this.targetHeadRotation = Quaternion.Euler(this.head.rotation.eulerAngles.x + num9, this.head.rotation.eulerAngles.y + num7, this.head.rotation.eulerAngles.z);
                }
                if (this.state == TitanState.Attacking || this.state == TitanState.Hitting || this.state == TitanState.Blinded)
                {
                    this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 20f);
                }
                else
                {
                    this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
                }
            }
            this.head.rotation = this.oldHeadRotation;
        }
        if (!animation.IsPlaying("die_headOff"))
        {
            this.head.localScale = this.headscale;
        }
    }

    private void hit(string animationName, Vector3 attacker, float hitPauseTime)
    {
        this.state = TitanState.Hitting;
        this.hitAnimation = animationName;
        this.hitPause = hitPauseTime;
        this.playAnimation(this.hitAnimation);
        animation[this.hitAnimation].time = 0f;
        animation[this.hitAnimation].speed = 0f;
        transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - transform.position).eulerAngles.y, 0f);
        this.needFreshCorePosition = true;
        if (photonView.isMine && this.grabbedTarget != null)
        {
            this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
        }
    }

    public void hitAnkle()
    {
        if (!this.hasDie && this.state != TitanState.Down)
        {
            if (this.grabbedTarget != null)
            {
                this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
            }
            this.GetDown();
        }
    }

    [RPC]
    public void HitAnkleRPC(int viewID)
    {
        if (!this.hasDie && this.state != TitanState.Down)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view != null)
            {
                Vector3 vector = view.gameObject.transform.position - transform.position;
                if (vector.magnitude < 20f)
                {
                    if (photonView.isMine && this.grabbedTarget != null)
                    {
                        this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
                    }
                    this.GetDown();
                }
            }
        }
    }

    public void HitEye()
    {
        if (!this.hasDie)
            this.justHitEye();
    }

    [RPC]
    public void HitEyeRPC(int viewID)
    {
        if (!this.hasDie)
        {
            Vector3 vector = PhotonView.Find(viewID).gameObject.transform.position - this.neck.position;
            if (vector.magnitude < 20f)
            {
                if (photonView.isMine && this.grabbedTarget != null)
                {
                    this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
                }
                if (!this.hasDie)
                {
                    this.justHitEye();
                }
            }
        }
    }

    public void HitLeft(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType != AbnormalType.Crawler)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                this.hit("hit_eren_L", attacker, hitPauseTime);
            else
                photonView.RPC("hitLRPC", PhotonTargets.All, attacker, hitPauseTime);
        }
    }

    [RPC]
    private void HitLRPC(Vector3 attacker, float hitPauseTime)
    {
        if (photonView.isMine)
        {
            Vector3 vector = attacker - transform.position;
            if (vector.magnitude < 80f)
                this.hit("hit_eren_L", attacker, hitPauseTime);
        }
    }

    public void HitRight(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType != AbnormalType.Crawler)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                this.hit("hit_eren_R", attacker, hitPauseTime);
            else
                photonView.RPC("hitRRPC", PhotonTargets.All, attacker, hitPauseTime);
        }
    }

    [RPC]
    private void HitRRPC(Vector3 attacker, float hitPauseTime)
    {
        if (photonView.isMine && !this.hasDie)
        {
            Vector3 vector = attacker - transform.position;
            if (vector.magnitude < 80f)
                this.hit("hit_eren_R", attacker, hitPauseTime);
        }
    }

    private void idle(float sbtime = 0f)
    {
        this.stuck = false;
        this.sbtime = sbtime;
        if (this.myDifficulty == 2 && (this.abnormalType == AbnormalType.Jumper || this.abnormalType == AbnormalType.Abnormal))
        {
            this.sbtime = UnityEngine.Random.Range(0f, 1.5f);
        }
        else if (this.myDifficulty >= 1)
        {
            this.sbtime = 0f;
        }
        this.sbtime = Mathf.Max(0.5f, this.sbtime);
        if (this.abnormalType == AbnormalType.Punk)
        {
            this.sbtime = 0.1f;
            if (this.myDifficulty == 1)
            {
                this.sbtime += 0.4f;
            }
        }
        this.state = TitanState.Idle;
        if (this.abnormalType == AbnormalType.Crawler)
        {
            this.crossFade("crawler_idle", 0.2f);
        }
        else
        {
            this.crossFade("idle", 0.2f);
        }
    }

    public bool IsGrounded()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyAABB");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, mask3.value);
    }

    private void justEatHero(GameObject target, Transform hand)
    {
        if (target != null)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                if (!target.GetComponent<HERO>().HasDied())
                {
                    target.GetComponent<HERO>().markDie();
                    if (this.nonAI)
                    {
                        target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, photonView.viewID, name);
                    }
                    else
                    {
                        target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, name);
                    }
                }
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                target.GetComponent<HERO>().die2(hand);
            }
        }
    }

    private void justHitEye()
    {
        if (this.state != TitanState.Blinded)
        {
            if (this.state != TitanState.Down && this.state != TitanState.Sit)
            {
                this.playAnimation("hit_eye");
            }
            else
            {
                this.playAnimation("sit_hit_eye");
            }
            this.state = TitanState.Blinded;
        }
    }

    [RPC]
    public void LabelRPC(int health, int maxHealth)
    {
        if (health < 0)
        {
            if (this.healthLabel != null)
            {
                Destroy(this.healthLabel);
            }
        }
        else
        {
            if (this.healthLabel == null)
            {
                this.healthLabel = (GameObject) Instantiate(Resources.Load("UI/LabelNameOverHead"));
                this.healthLabel.name = "LabelNameOverHead";
                this.healthLabel.transform.parent = transform;
                this.healthLabel.transform.localPosition = new Vector3(0f, 20f + 1f / this.myLevel, 0f);
                if (this.abnormalType == AbnormalType.Crawler)
                {
                    this.healthLabel.transform.localPosition = new Vector3(0f, 10f + 1f / this.myLevel, 0f);
                }
                float x = 1f;
                if (this.myLevel < 1f)
                {
                    x = 1f / this.myLevel;
                }
                this.healthLabel.transform.localScale = new Vector3(x, x, x);
                this.healthLabelEnabled = true;
            }
            string str = "[7FFF00]";
            float num2 = health / (float)maxHealth;
            if (num2 < 0.75f && num2 >= 0.5f)
            {
                str = "[f2b50f]";
            }
            else if (num2 < 0.5f && num2 >= 0.25f)
            {
                str = "[ff8100]";
            }
            else if (num2 < 0.25f)
            {
                str = "[ff3333]";
            }
            this.healthLabel.GetComponent<UILabel>().text = str + Convert.ToString(health);
        }
    }

    public void DoLateUpdate()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            if (this.baseAnimation.IsPlaying("run_walk"))
            {
                if (this.baseAnimation["run_walk"].normalizedTime % 1f > 0.1f && this.baseAnimation["run_walk"].normalizedTime % 1f < 0.6f && this.stepSoundPhase == 2)
                {
                    this.stepSoundPhase = 1;
                    this.baseAudioSource.Stop();
                    this.baseAudioSource.Play();
                }
                else if (this.baseAnimation["run_walk"].normalizedTime % 1f > 0.6f && this.stepSoundPhase == 1)
                {
                    this.stepSoundPhase = 2;
                    this.baseAudioSource.Stop();
                    this.baseAudioSource.Play();
                }
            }
            else if (this.baseAnimation.IsPlaying("crawler_run"))
            {
                if (this.baseAnimation["crawler_run"].normalizedTime % 1f > 0.1f && this.baseAnimation["crawler_run"].normalizedTime % 1f < 0.56f && this.stepSoundPhase == 2)
                {
                    this.stepSoundPhase = 1;
                    this.baseAudioSource.Stop();
                    this.baseAudioSource.Play();
                }
                else if (this.baseAnimation["crawler_run"].normalizedTime % 1f > 0.56f && this.stepSoundPhase == 1)
                {
                    this.stepSoundPhase = 2;
                    this.baseAudioSource.Stop();
                    this.baseAudioSource.Play();
                }
            }
            else if (this.baseAnimation.IsPlaying("run_abnormal"))
            {
                if (this.baseAnimation["run_abnormal"].normalizedTime % 1f > 0.47f && this.baseAnimation["run_abnormal"].normalizedTime % 1f < 0.95f && this.stepSoundPhase == 2)
                {
                    this.stepSoundPhase = 1;
                    this.baseAudioSource.Stop();
                    this.baseAudioSource.Play();
                }
                else if (!(!(this.baseAnimation["run_abnormal"].normalizedTime % 1f > 0.95f) && !(this.baseAnimation["run_abnormal"].normalizedTime % 1f < 0.47f) || this.stepSoundPhase != 1))
                {
                    this.stepSoundPhase = 2;
                    this.baseAudioSource.Stop();
                    this.baseAudioSource.Play();
                }
            }
            this.HeadMovement();
            this.grounded = false;
            this.updateLabel();
            this.updateCollider();
        }
    }

    [RPC]
    private void Laugh(float sbtime = 0f)
    {
        if (this.state == TitanState.Idle || this.state == TitanState.Turning || this.state == TitanState.Chasing)
        {
            this.sbtime = sbtime;
            this.state = TitanState.Laughing;
            this.crossFade("laugh", 0.2f);
        }
    }

    public void loadskin()
    {
        this.skin = 86;
        this.eye = false;
        if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine) && FengGameManagerMKII.settings.EnableTitanSkins)
        {
            var titanSkin = FengGameManagerMKII.settings.TitanSkin;
            var index = Random.Range(0, titanSkin.Body.Length);
            
            var body = titanSkin.Body[index];
            string eye;
            if (FengGameManagerMKII.settings.Randomize)
                eye = titanSkin.Eyes[Random.Range(0, titanSkin.Eyes.Length)];                
            else
                eye = titanSkin.Eyes[index];
            
            this.skin += index;
            
            if (Utility.IsValidImageUrl(eye))
                this.eye = true;
            GetComponent<TITAN_SETUP>().setVar(this.skin, this.eye);
            
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                StartCoroutine(this.LoadSkinEnumerator(body, eye));
            else
                photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, body, eye);
        }
    }

    public IEnumerator LoadSkinEnumerator(string body, string eye)
    {
        while (!this.hasSpawn)
        {
            yield return null;
        }
        bool mipmap = FengGameManagerMKII.settings.UseMipmap;
        bool unloadAssets = false;
        
        foreach (Renderer myRenderer in this.GetComponentsInChildren<Renderer>())
        {
            if (myRenderer.name.Contains("eye"))
            {
                if (eye.EqualsIgnoreCase("transparent"))
                {
                    myRenderer.enabled = false;
                }
                else if (Utility.IsValidImageUrl(eye))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(eye))
                    {
                        unloadAssets = true;
                        myRenderer.material.mainTextureScale = new Vector2(myRenderer.material.mainTextureScale.x * 4f, myRenderer.material.mainTextureScale.y * 8f);
                        myRenderer.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (WWW www = new WWW(eye))
                        {
                            yield return www;
                            if (www.error != null)
                                continue;
                            myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(eye, myRenderer.material);
                        myRenderer.material = (Material)FengGameManagerMKII.linkHash[0][eye];
                    }
                    else
                    {
                        myRenderer.material = (Material)FengGameManagerMKII.linkHash[0][eye];
                    }
                }
            }
            else if (myRenderer.name == "hair" && Utility.IsValidImageUrl(body))
            {
                if (!FengGameManagerMKII.linkHash[2].ContainsKey(body))
                {
                    unloadAssets = true;
                    myRenderer.material = this.mainMaterial.GetComponent<SkinnedMeshRenderer>().material;
                    using (WWW www = new WWW(body))
                    {
                        yield return www;
                        if (www.error != null)
                            continue;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 1000000);
                    }
                    FengGameManagerMKII.linkHash[2].Add(body, myRenderer.material);
                    myRenderer.material = (Material)FengGameManagerMKII.linkHash[2][body];
                }
                else
                {
                    myRenderer.material = (Material)FengGameManagerMKII.linkHash[2][body];
                }
            }
        }
        if (unloadAssets)
            FengGameManagerMKII.instance.UnloadAssets();
    }

    [RPC]
    public void LoadskinRPC(string body, string eye)
    {
        if (FengGameManagerMKII.settings.EnableTitanSkins)
        {
            StartCoroutine(this.LoadSkinEnumerator(body, eye));
        }
    }

    private bool LongRangeAttackCheck()
    {
        if (this.abnormalType == AbnormalType.Punk && this.myHero != null)
        {
            Vector3 line = this.myHero.rigidbody.velocity * Time.deltaTime * 30f;
            if (line.sqrMagnitude > 10f)
            {
                if (this.simpleHitTestLineAndBall(line, this.baseTransform.Find("chkAeLeft").position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.Attack("anti_AE_l");
                    return true;
                }
                if (this.simpleHitTestLineAndBall(line, this.baseTransform.Find("chkAeLLeft").position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.Attack("anti_AE_low_l");
                    return true;
                }
                if (this.simpleHitTestLineAndBall(line, this.baseTransform.Find("chkAeRight").position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.Attack("anti_AE_r");
                    return true;
                }
                if (this.simpleHitTestLineAndBall(line, this.baseTransform.Find("chkAeLRight").position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.Attack("anti_AE_low_r");
                    return true;
                }
            }
            Vector3 vector2 = this.myHero.transform.position - this.baseTransform.position;
            float current = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
            float f = -Mathf.DeltaAngle(current, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f);
            if (this.rockInterval > 0f)
            {
                this.rockInterval -= Time.deltaTime;
            }
            else if (Mathf.Abs(f) < 5f)
            {
                Vector3 vector3 = this.myHero.transform.position + line;
                Vector3 vector4 = vector3 - this.baseTransform.position;
                float sqrMagnitude = vector4.sqrMagnitude;
                if (sqrMagnitude > 8000f && sqrMagnitude < 90000f && FengGameManagerMKII.settings.EnableRockThrow)
                {
                    this.Attack("throw");
                    this.rockInterval = 2f;
                    return true;
                }
            }
        }
        return false;
    }

    public void moveTo(float posX, float posY, float posZ)
    {
        transform.position = new Vector3(posX, posY, posZ);
    }

    [RPC]
    public void MoveToRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            transform.position = new Vector3(posX, posY, posZ);
        }
    }

    [RPC]
    private void NetCrossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
    }

    [RPC]
    private void NetDie()
    {
        this.asClientLookTarget = false;
        if (!this.hasDie)
        {
            this.hasDie = true;
            if (this.nonAI)
            {
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null, true, false);
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.Dead, true }
                };
                Player.Self.SetCustomProperties(propertiesToSet);
                propertiesToSet = new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerProperty.Deaths, (int)Player.Self.Properties.Deaths + 1 }
                };
                Player.Self.SetCustomProperties(propertiesToSet);
            }
            this.dieAnimation();
        }
    }

    [RPC]
    private void NetPlayAnimation(string aniName)
    {
        animation.Play(aniName);
    }

    [RPC]
    private void NetPlayAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void NetSetAbnormalType(int type)
    {
        if (!this.hasload)
        {
            this.hasload = true;
            this.loadskin();
        }
        if (type == 0)
        {
            this.abnormalType = AbnormalType.Normal;
            name = "Titan";
            this.runAnimation = "run_walk";
            GetComponent<TITAN_SETUP>().SetHair();
        }
        else if (type == 1)
        {
            this.abnormalType = AbnormalType.Abnormal;
            name = "Aberrant";
            this.runAnimation = "run_abnormal";
            GetComponent<TITAN_SETUP>().SetHair();
        }
        else if (type == 2)
        {
            this.abnormalType = AbnormalType.Jumper;
            name = "Jumper";
            this.runAnimation = "run_abnormal";
            GetComponent<TITAN_SETUP>().SetHair();
        }
        else if (type == 3)
        {
            this.abnormalType = AbnormalType.Crawler;
            name = "Crawler";
            this.runAnimation = "crawler_run";
            GetComponent<TITAN_SETUP>().SetHair();
        }
        else if (type == 4)
        {
            this.abnormalType = AbnormalType.Punk;
            name = "Punk";
            this.runAnimation = "run_abnormal_1";
            GetComponent<TITAN_SETUP>().SetHair();
        }
        if (this.abnormalType == AbnormalType.Abnormal || this.abnormalType == AbnormalType.Jumper || this.abnormalType == AbnormalType.Punk)
        {
            this.speed = 18f;
            if (this.myLevel > 1f)
            {
                this.speed *= Mathf.Sqrt(this.myLevel);
            }
            if (this.myDifficulty == 1)
            {
                this.speed *= 1.4f;
            }
            if (this.myDifficulty == 2)
            {
                this.speed *= 1.6f;
            }
            this.baseAnimation["turnaround1"].speed = 2f;
            this.baseAnimation["turnaround2"].speed = 2f;
        }
        if (this.abnormalType == AbnormalType.Crawler)
        {
            this.chaseDistance += 50f;
            this.speed = 25f;
            if (this.myLevel > 1f)
            {
                this.speed *= Mathf.Sqrt(this.myLevel);
            }
            if (this.myDifficulty == 1)
            {
                this.speed *= 2f;
            }
            if (this.myDifficulty == 2)
            {
                this.speed *= 2.2f;
            }
            this.baseTransform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().height = 10f;
            this.baseTransform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().radius = 5f;
            this.baseTransform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
        }
        if (this.nonAI)
        {
            if (this.abnormalType == AbnormalType.Crawler)
            {
                this.speed = Mathf.Min(70f, this.speed);
            }
            else
            {
                this.speed = Mathf.Min(60f, this.speed);
            }
            this.baseAnimation["attack_jumper_0"].speed = 7f;
            this.baseAnimation["attack_crawler_jump_0"].speed = 4f;
        }
        this.baseAnimation["attack_combo_1"].speed = 1f;
        this.baseAnimation["attack_combo_2"].speed = 1f;
        this.baseAnimation["attack_combo_3"].speed = 1f;
        this.baseAnimation["attack_quick_turn_l"].speed = 1f;
        this.baseAnimation["attack_quick_turn_r"].speed = 1f;
        this.baseAnimation["attack_anti_AE_l"].speed = 1.1f;
        this.baseAnimation["attack_anti_AE_low_l"].speed = 1.1f;
        this.baseAnimation["attack_anti_AE_r"].speed = 1.1f;
        this.baseAnimation["attack_anti_AE_low_r"].speed = 1.1f;
        this.idle(0f);
    }

    [RPC]
    private void NetSetLevel(float level, int AI, int skinColor)
    {
        this.setLevel2(level, AI, skinColor);
        if (level > 5f)
        {
            this.headscale = new Vector3(1f, 1f, 1f);
        }
        else if (level < 1f && FengGameManagerMKII.Level.StartsWith("Custom"))
        {
            CapsuleCollider component = this.myTitanTrigger.GetComponent<CapsuleCollider>();
            component.radius *= 2.5f - level;
        }
    }

    private void OnCollisionStay()
    {
        this.grounded = true;
    }

    private void OnDestroy()
    {
        if (GameObject.Find("MultiplayerManager") != null)
        {
            FengGameManagerMKII.instance.Titans.Remove(this);
        }
    }

    public void OnTitanDie(PhotonView view)
    {
        if (FengGameManagerMKII.logicLoaded && FengGameManagerMKII.RCEvents.ContainsKey("OnTitanDie"))
        {
            RCEvent event2 = (RCEvent) FengGameManagerMKII.RCEvents["OnTitanDie"];
            string[] strArray = (string[]) FengGameManagerMKII.RCVariableNames["OnTitanDie"];
            if (FengGameManagerMKII.titanVariables.ContainsKey(strArray[0]))
            {
                FengGameManagerMKII.titanVariables[strArray[0]] = this;
            }
            else
            {
                FengGameManagerMKII.titanVariables.Add(strArray[0], this);
            }
            if (FengGameManagerMKII.playerVariables.ContainsKey(strArray[1]))
            {
                FengGameManagerMKII.playerVariables[strArray[1]] = view.owner;
            }
            else
            {
                FengGameManagerMKII.playerVariables.Add(strArray[1], view.owner);
            }
            event2.checkEvent();
        }
    }

    private void playAnimation(string aniName)
    {
        animation.Play(aniName);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
        }
    }

    private void playSound(string sndname)
    {
        this.PlaysoundRPC(sndname);
        if (photonView.isMine)
        {
            photonView.RPC("playsoundRPC", PhotonTargets.Others, sndname);
        }
    }

    [RPC]
    private void PlaysoundRPC(string sndname)
    {
        transform.Find(sndname).GetComponent<AudioSource>().Play();
    }

    public void pt()
    {
        if (this.controller.bite)
        {
            this.Attack("bite");
        }
        if (this.controller.bitel)
        {
            this.Attack("bite_l");
        }
        if (this.controller.biter)
        {
            this.Attack("bite_r");
        }
        if (this.controller.chopl)
        {
            this.Attack("anti_AE_low_l");
        }
        if (this.controller.choptr)
        {
            this.Attack("anti_AE_r");
        }
        if (this.controller.choptl)
        {
            this.Attack("anti_AE_l");
        }
        if (this.controller.cover && this.stamina > 75f)
        {
            this.recoverpt();
            this.stamina -= 75f;
        }
        if (this.controller.grabbackl)
        {
            this.Grab("ground_back_l");
        }
        if (this.controller.grabbackr)
        {
            this.Grab("ground_back_r");
        }
        if (this.controller.grabfrontl)
        {
            this.Grab("ground_front_l");
        }
        if (this.controller.grabfrontr)
        {
            this.Grab("ground_front_r");
        }
        if (this.controller.grabnapel)
        {
            this.Grab("head_back_l");
        }
        if (this.controller.grabnaper)
        {
            this.Grab("head_back_r");
        }
    }

    public void randomRun(Vector3 targetPt, float r)
    {
        this.state = TitanState.RunningRandomly;
        this.targetCheckPt = targetPt;
        this.targetR = r;
        this.random_run_time = UnityEngine.Random.Range(1f, 2f);
        this.crossFade(this.runAnimation, 0.5f);
    }

    private void recover()
    {
        this.state = TitanState.Recovering;
        this.playAnimation("idle_recovery");
        this.getdownTime = UnityEngine.Random.Range(2f, 5f);
    }

    private void recoverpt()
    {
        this.state = TitanState.Recovering;
        this.playAnimation("idle_recovery");
        this.getdownTime = UnityEngine.Random.Range(1.8f, 2.5f);
    }

    public IEnumerator reloadSky()
    {
        yield return new WaitForSeconds(0.5f);
        if (FengGameManagerMKII.skyMaterial != null && Camera.main.GetComponent<Skybox>().material != FengGameManagerMKII.skyMaterial)
        {
            Camera.main.GetComponent<Skybox>().material = FengGameManagerMKII.skyMaterial;
        }
    }

    private void remainSitdown()
    {
        this.state = TitanState.Sit;
        this.playAnimation("sit_idle");
        this.getdownTime = UnityEngine.Random.Range(10f, 30f);
    }

    public void resetLevel(float level)
    {
        this.myLevel = level;
        this.setmyLevel();
    }

    public void setAbnormalType2(AbnormalType type, bool forceCrawler)
    {
        bool flag = FengGameManagerMKII.settings.UseCustomSpawnRates || FengGameManagerMKII.Level.StartsWith("Custom"); // Might be broken
        int num = 0;
        float num2 = 0.02f * (IN_GAME_MAIN_CAMERA.difficulty + 1);
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpAHSS)
        {
            num2 = 100f;
        }
        if (type == AbnormalType.Normal)
        {
            if (UnityEngine.Random.Range(0f, 1f) < num2)
            {
                num = 4;
            }
            else
            {
                num = 0;
            }
            if (flag)
            {
                num = 0;
            }
        }
        else if (type == AbnormalType.Abnormal)
        {
            if (UnityEngine.Random.Range(0f, 1f) < num2)
            {
                num = 4;
            }
            else
            {
                num = 1;
            }
            if (flag)
            {
                num = 1;
            }
        }
        else if (type == AbnormalType.Jumper)
        {
            if (UnityEngine.Random.Range(0f, 1f) < num2)
            {
                num = 4;
            }
            else
            {
                num = 2;
            }
            if (flag)
            {
                num = 2;
            }
        }
        else if (type == AbnormalType.Crawler)
        {
            num = 3;
            if (GameObject.Find("Crawler") != null && UnityEngine.Random.Range(0, 1000) > 5)
            {
                num = 2;
            }
            if (flag)
            {
                num = 3;
            }
        }
        else if (type == AbnormalType.Punk)
        {
            num = 4;
        }
        if (forceCrawler)
        {
            num = 3;
        }
        if (num == 4)
        {
            if (!LevelInfoManager.Get(FengGameManagerMKII.Level).HasPunk)
            {
                num = 1;
            }
            else
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer && PunkNumber>= 3)
                {
                    num = 1;
                }
                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.SurviveMode)
                {
                    int wave = FengGameManagerMKII.instance.wave;
                    if (wave != 5 && wave != 10 && wave != 15 && wave != 20)
                    {
                        num = 1;
                    }
                }
            }
            if (flag)
            {
                num = 4;
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
        {
            object[] parameters = new object[] { num };
            photonView.RPC("netSetAbnormalType", PhotonTargets.AllBuffered, parameters);
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            this.NetSetAbnormalType(num);
        }
    }

    [RPC]
    private void SetIfLookTarget(bool bo)
    {
        this.asClientLookTarget = bo;
    }

    private void setLevel(float level, int AI, int skinColor)
    {
        this.myLevel = level;
        this.myLevel = Mathf.Clamp(this.myLevel, 0.7f, 3f);
        this.attackWait += UnityEngine.Random.Range(0f, 2f);
        this.chaseDistance += this.myLevel * 10f;
        transform.localScale = new Vector3(this.myLevel, this.myLevel, this.myLevel);
        float x = Mathf.Min(Mathf.Pow(2f / this.myLevel, 0.35f), 1.25f);
        this.headscale = new Vector3(x, x, x);
        this.head = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        this.head.localScale = this.headscale;
        if (skinColor != 0)
        {
            this.mainMaterial.GetComponent<SkinnedMeshRenderer>().material.color = skinColor != 1 ? (skinColor != 2 ? FengColor.titanSkin3 : FengColor.titanSkin2) : FengColor.titanSkin1;
        }
        float num2 = 1.4f - (this.myLevel - 0.7f) * 0.15f;
        num2 = Mathf.Clamp(num2, 0.9f, 1.5f);
        foreach (AnimationState current in animation)
            if (current != null)
                current.speed = num2;
        Rigidbody rigidbody = this.rigidbody;
        rigidbody.mass *= this.myLevel;
        this.rigidbody.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f);
        if (this.myLevel > 1f)
        {
            this.speed *= Mathf.Sqrt(this.myLevel);
        }
        this.myDifficulty = AI;
        if (this.myDifficulty == 1 || this.myDifficulty == 2)
        {
            foreach (AnimationState state2 in animation)
                if (state2 != null)
                    state2.speed = num2 * 1.05f;
            if (this.nonAI)
            {
                this.speed *= 1.1f;
            }
            else
            {
                this.speed *= 1.4f;
            }
            this.chaseDistance *= 1.15f;
        }
        if (this.myDifficulty == 2)
        {
            foreach (AnimationState state3 in animation)
                if (state3 != null)
                    state3.speed = num2 * 1.05f;
            
            this.speed *= nonAI ? 1.1f : 1.5f;
            this.chaseDistance *= 1.3f;
        }
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.EndlessTitan || IN_GAME_MAIN_CAMERA.GameMode == GameMode.SurviveMode)
        {
            this.chaseDistance = 999999f;
        }
        if (this.nonAI)
        {
            if (this.abnormalType == AbnormalType.Crawler)
            {
                this.speed = Mathf.Min(70f, this.speed);
            }
            else
            {
                this.speed = Mathf.Min(60f, this.speed);
            }
        }
        this.attackDistance = Vector3.Distance(transform.position, transform.Find("ap_front_ground").position) * 1.65f;
    }

    private void setLevel2(float level, int AI, int skinColor)
    {
        this.myLevel = level;
        this.myLevel = Mathf.Clamp(this.myLevel, 0.1f, 50f);
        this.attackWait += UnityEngine.Random.Range(0f, 2f);
        this.chaseDistance += this.myLevel * 10f;
        transform.localScale = new Vector3(this.myLevel, this.myLevel, this.myLevel);
        float x = Mathf.Min(Mathf.Pow(2f / this.myLevel, 0.35f), 1.25f);
        this.headscale = new Vector3(x, x, x);
        this.head = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        this.head.localScale = this.headscale;
        if (skinColor != 0)
        {
            this.mainMaterial.GetComponent<SkinnedMeshRenderer>().material.color = skinColor != 1 ? (skinColor != 2 ? FengColor.titanSkin3 : FengColor.titanSkin2) : FengColor.titanSkin1;
        }
        float num2 = 1.4f - (this.myLevel - 0.7f) * 0.15f;
        num2 = Mathf.Clamp(num2, 0.9f, 1.5f);
        foreach (AnimationState state in animation)
        {
            state.speed = num2;
        }
        Rigidbody rigidbody = this.rigidbody;
        rigidbody.mass *= this.myLevel;
        this.rigidbody.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f);
        if (this.myLevel > 1f)
        {
            this.speed *= Mathf.Sqrt(this.myLevel);
        }
        this.myDifficulty = AI;
        if (this.myDifficulty == 1 || this.myDifficulty == 2)
        {
            foreach (AnimationState state2 in animation)
            {
                state2.speed = num2 * 1.05f;
            }
            if (this.nonAI)
            {
                this.speed *= 1.1f;
            }
            else
            {
                this.speed *= 1.4f;
            }
            this.chaseDistance *= 1.15f;
        }
        if (this.myDifficulty == 2)
        {
            foreach (AnimationState state3 in animation)
            {
                state3.speed = num2 * 1.05f;
            }
            if (this.nonAI)
            {
                this.speed *= 1.1f;
            }
            else
            {
                this.speed *= 1.5f;
            }
            this.chaseDistance *= 1.3f;
        }
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.EndlessTitan || IN_GAME_MAIN_CAMERA.GameMode == GameMode.SurviveMode)
        {
            this.chaseDistance = 999999f;
        }
        if (this.nonAI)
        {
            if (this.abnormalType == AbnormalType.Crawler)
            {
                this.speed = Mathf.Min(70f, this.speed);
            }
            else
            {
                this.speed = Mathf.Min(60f, this.speed);
            }
        }
        this.attackDistance = Vector3.Distance(transform.position, transform.Find("ap_front_ground").position) * 1.65f;
    }

    private void setmyLevel()
    {
        animation.cullingType = AnimationCullingType.BasedOnRenderers;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
        {
            object[] parameters = new object[] { this.myLevel, FengGameManagerMKII.instance.difficulty, UnityEngine.Random.Range(0, 4) };
            photonView.RPC("netSetLevel", PhotonTargets.AllBuffered, parameters);
            animation.cullingType = AnimationCullingType.AlwaysAnimate;
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            this.setLevel2(this.myLevel, IN_GAME_MAIN_CAMERA.difficulty, UnityEngine.Random.Range(0, 4));
        }
    }

    [RPC]
    private void SetMyTarget(int ID)
    {
        if (ID == -1)
        {
            this.myHero = null;
        }
        PhotonView view = PhotonView.Find(ID);
        if (view != null)
        {
            this.myHero = view.gameObject;
        }
    }

    public void setRoute(GameObject route)
    {
        this.checkPoints = new ArrayList();
        for (int i = 1; i <= 10; i++)
        {
            this.checkPoints.Add(route.transform.Find("r" + i).position);
        }
        this.checkPoints.Add("end");
    }

    private bool simpleHitTestLineAndBall(Vector3 line, Vector3 ball, float R)
    {
        Vector3 rhs = Vector3.Project(ball, line);
        Vector3 vector2 = ball - rhs;
        if (vector2.magnitude > R)
        {
            return false;
        }
        if (Vector3.Dot(line, rhs) < 0f)
        {
            return false;
        }
        if (rhs.sqrMagnitude > line.sqrMagnitude)
        {
            return false;
        }
        return true;
    }

    private void sitdown()
    {
        this.state = TitanState.Sit;
        this.playAnimation("sit_down");
        this.getdownTime = UnityEngine.Random.Range(10f, 30f);
    }

    private void Start()
    {
        this.MultiplayerManager.Titans.Add(this);
        
        if (photonView.owner != null)
            photonView.owner.Titan = this;
        
        if (Minimap.instance != null)
            Minimap.instance.TrackGameObjectOnMinimap(gameObject, Color.yellow, false, true, Minimap.IconStyle.CIRCLE);
        this.currentCamera = GameObject.Find("MainCamera");
        this.runAnimation = "run_walk";
        this.grabTF = new GameObject();
        this.grabTF.name = "titansTmpGrabTF";
        this.head = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        this.neck = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        this.oldHeadRotation = this.head.rotation;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || photonView.isMine)
        {
            if (!this.hasSetLevel)
            {
                this.myLevel = UnityEngine.Random.Range(0.7f, 3f);
                if (FengGameManagerMKII.settings.EnableCustomSize)
                {
                    this.myLevel = FengGameManagerMKII.settings.TitanSize.Random;
                }
                this.hasSetLevel = true;
            }
            this.spawnPt = this.baseTransform.position;
            this.setmyLevel();
            this.setAbnormalType2(this.abnormalType, false);
            if (this.myHero == null)
            {
                this.FindNeareastHero();
            }
            this.controller = gameObject.GetComponent<TITAN_CONTROLLER>();
            if (this.nonAI)
            {
                StartCoroutine(this.reloadSky());
            }
        }
        if (this.maxHealth == 0 && FengGameManagerMKII.settings.HealthMode > HealthMode.Off)
        {
            switch (FengGameManagerMKII.settings.HealthMode)
            {
                case HealthMode.Fixed:
                    this.maxHealth = this.currentHealth = (int) FengGameManagerMKII.settings.TitanHealth.Random;
                    break;
                case HealthMode.SizeBased:
                    this.maxHealth = this.currentHealth = (int) FengGameManagerMKII.settings.TitanHealth.Clamp(this.myLevel / 4f * FengGameManagerMKII.settings.TitanHealth.Random);
                    break;
            }
        }
        this.lagMax = 150f + this.myLevel * 3f;
        this.healthTime = Time.time;
        if (this.currentHealth > 0 && photonView.isMine)
        {
            photonView.RPC("labelRPC", PhotonTargets.AllBuffered, this.currentHealth, this.maxHealth);
        }
        this.hasExplode = false;
        this.colliderEnabled = true;
        this.isHooked = false;
        this.isLook = false;
        this.hasSpawn = true;
    }

    public void suicide()
    {
        this.NetDie();
        if (this.nonAI)
        {
            FengGameManagerMKII.instance.SendKillInfo(false, string.Empty, true, Player.Self.Properties.Name, 0);
        }
        FengGameManagerMKII.instance.needChooseSide = true;
        FengGameManagerMKII.instance.justSuicide = true;
    }

    public void testVisual(bool setCollider)
    {
        if (setCollider)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = Color.white;
            }
        }
        else
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = Color.black;
            }
        }
    }

    [RPC]
    public void TitanGetHit(int viewID, int speed)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            Vector3 vector = view.gameObject.transform.position - this.neck.position;
            if (vector.magnitude < this.lagMax && !this.hasDie && Time.time - this.healthTime > 0.2f)
            {
                this.healthTime = Time.time;
                if (speed >= FengGameManagerMKII.settings.MinimumDamage || this.abnormalType == AbnormalType.Crawler)
                {
                    this.currentHealth -= speed;
                }
                if (this.maxHealth > 0f)
                {
                    photonView.RPC("labelRPC", PhotonTargets.AllBuffered, this.currentHealth, this.maxHealth);
                }
                if (this.currentHealth < 0f)
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        this.OnTitanDie(view);
                    }
                    photonView.RPC("netDie", PhotonTargets.OthersBuffered);
                    if (this.grabbedTarget != null)
                    {
                        this.grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All, new object[0]);
                    }
                    this.NetDie();
                    if (this.nonAI)
                    {
                        FengGameManagerMKII.instance.TitanGetKill(view.owner, speed, Player.Self.Properties.Name, null);
                    }
                    else
                    {
                        FengGameManagerMKII.instance.TitanGetKill(view.owner, speed, name, null);
                    }
                }
                else
                {
                    FengGameManagerMKII.instance.photonView.RPC("netShowDamage", view.owner, new object[] { speed });
                }
            }
        }
    }

    public void toCheckPoint(Vector3 targetPt, float r)
    {
        this.state = TitanState.GoingToCheckpoint;
        this.targetCheckPt = targetPt;
        this.targetR = r;
        this.crossFade(this.runAnimation, 0.5f);
    }

    public void toPVPCheckPoint(Vector3 targetPt, float r)
    {
        this.state = TitanState.GoingToPVP;
        this.targetCheckPt = targetPt;
        this.targetR = r;
        this.crossFade(this.runAnimation, 0.5f);
    }

    private void turn(float d)
    {
        if (this.abnormalType == AbnormalType.Crawler)
        {
            if (d > 0f)
            {
                this.turnAnimation = "crawler_turnaround_R";
            }
            else
            {
                this.turnAnimation = "crawler_turnaround_L";
            }
        }
        else if (d > 0f)
        {
            this.turnAnimation = "turnaround2";
        }
        else
        {
            this.turnAnimation = "turnaround1";
        }
        this.playAnimation(this.turnAnimation);
        animation[this.turnAnimation].time = 0f;
        d = Mathf.Clamp(d, -120f, 120f);
        this.turnDeg = d;
        this.desDeg = gameObject.transform.rotation.eulerAngles.y + this.turnDeg;
        this.state = TitanState.Turning;
    }

    public void DoUpdate()
    {
        if ((IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer) ||
            this.myDifficulty < 0 || (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine)) 
            return;
        
        this.Explode();
        if (!this.nonAI)
        {
            if (this.activeRad < int.MaxValue && (this.state == TitanState.Idle || this.state == TitanState.Wandering || this.state == TitanState.Chasing))
            {
                if (this.checkPoints.Count > 1)
                {
                    if (Vector3.Distance((Vector3) this.checkPoints[0], this.baseTransform.position) > this.activeRad)
                    {
                        this.toCheckPoint((Vector3) this.checkPoints[0], 10f);
                    }
                }
                else if (Vector3.Distance(this.spawnPt, this.baseTransform.position) > this.activeRad)
                {
                    this.toCheckPoint(this.spawnPt, 10f);
                }
            }
            if (this.whoHasTauntMe != null)
            {
                this.tauntTime -= Time.deltaTime;
                if (this.tauntTime <= 0f)
                {
                    this.whoHasTauntMe = null;
                }
                this.myHero = this.whoHasTauntMe;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
                }
            }
        }
        if (this.hasDie)
        {
            this.dieTime += Time.deltaTime;
            if (this.dieTime > 2f && !this.hasDieSteam)
            {
                this.hasDieSteam = true;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    GameObject obj2 = (GameObject) Instantiate(Resources.Load("FX/FXtitanDie1"));
                    obj2.transform.position = this.baseTransform.Find("Amarture/Core/Controller_Body/hip").position;
                    obj2.transform.localScale = this.baseTransform.localScale;
                }
                else if (photonView.isMine)
                {
                    PhotonNetwork.Instantiate("FX/FXtitanDie1", this.baseTransform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = this.baseTransform.localScale;
                }
            }
            if (this.dieTime > 5f)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    GameObject obj3 = (GameObject) Instantiate(Resources.Load("FX/FXtitanDie"));
                    obj3.transform.position = this.baseTransform.Find("Amarture/Core/Controller_Body/hip").position;
                    obj3.transform.localScale = this.baseTransform.localScale;
                    Destroy(gameObject);
                }
                else if (photonView.isMine)
                {
                    PhotonNetwork.Instantiate("FX/FXtitanDie", this.baseTransform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = this.baseTransform.localScale;
                    PhotonNetwork.Destroy(gameObject);
                    this.myDifficulty = -1;
                }
            }
        }
        else
        {
            if (this.state == TitanState.Hitting)
            {
                if (this.hitPause > 0f)
                {
                    this.hitPause -= Time.deltaTime;
                    if (this.hitPause <= 0f)
                    {
                        this.baseAnimation[this.hitAnimation].speed = 1f;
                        this.hitPause = 0f;
                    }
                }
                if (this.baseAnimation[this.hitAnimation].normalizedTime >= 1f)
                {
                    this.idle(0f);
                }
            }
            if (!this.nonAI)
            {
                if (this.myHero == null)
                {
                    this.FindNeareastHero();
                }
                if (!(this.state != TitanState.Idle && this.state != TitanState.Chasing && this.state != TitanState.Wandering || (this.whoHasTauntMe != null || UnityEngine.Random.Range(0, 100) >= 10)))
                {
                    this.FindNearestFacingHero();
                }
                if (this.myHero == null)
                {
                    this.myDistance = float.MaxValue;
                }
                else
                {
                    this.myDistance = Mathf.Sqrt((this.myHero.transform.position.x - this.baseTransform.position.x) * (this.myHero.transform.position.x - this.baseTransform.position.x) + (this.myHero.transform.position.z - this.baseTransform.position.z) * (this.myHero.transform.position.z - this.baseTransform.position.z));
                }
            }
            else
            {
                if (this.stamina < this.maxStamina)
                {
                    if (this.baseAnimation.IsPlaying("idle"))
                    {
                        this.stamina += Time.deltaTime * 30f;
                    }
                    if (this.baseAnimation.IsPlaying("crawler_idle"))
                    {
                        this.stamina += Time.deltaTime * 35f;
                    }
                    if (this.baseAnimation.IsPlaying("run_walk"))
                    {
                        this.stamina += Time.deltaTime * 10f;
                    }
                }
                if (this.baseAnimation.IsPlaying("run_abnormal_1"))
                {
                    this.stamina -= Time.deltaTime * 5f;
                }
                if (this.baseAnimation.IsPlaying("crawler_run"))
                {
                    this.stamina -= Time.deltaTime * 15f;
                }
                if (this.stamina < 0f)
                {
                    this.stamina = 0f;
                }
                if (!IN_GAME_MAIN_CAMERA.isPausing)
                {
                    GameObject.Find("stamina_titan").transform.localScale = new Vector3(this.stamina, 16f);
                }
            }
            if (this.state == TitanState.Laughing)
            {
                if (this.baseAnimation["laugh"].normalizedTime >= 1f)
                {
                    this.idle(2f);
                }
            }
            else if (this.state != TitanState.Idle)
            {
                if (this.state == TitanState.Attacking)
                {
                    if (this.attackAnimation == "combo")
                    {
                        if (this.nonAI)
                        {
                            if (this.controller.isAttackDown)
                            {
                                this.nonAIcombo = true;
                            }
                            if (!(this.nonAIcombo || this.baseAnimation["attack_" + this.attackAnimation].normalizedTime < 0.385f))
                            {
                                this.idle(0f);
                                return;
                            }
                        }
                        if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 0.11f && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime <= 0.16f)
                        {
                            GameObject obj5 = this.checkIfHitHand(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
                            if (obj5 != null)
                            {
                                Vector3 position = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    obj5.GetComponent<HERO>().die((obj5.transform.position - position) * 15f * this.myLevel, false);
                                }
                                else if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine || obj5.GetComponent<HERO>().HasDied()))
                                {
                                    obj5.GetComponent<HERO>().markDie();
                                    object[] objArray3 = new object[] { (obj5.transform.position - position) * 15f * this.myLevel, false, !this.nonAI ? -1 : photonView.viewID, name, true };
                                    obj5.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, objArray3);
                                }
                            }
                        }
                        if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 0.27f && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime <= 0.32f)
                        {
                            GameObject obj6 = this.checkIfHitHand(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
                            if (obj6 != null)
                            {
                                Vector3 vector3 = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    obj6.GetComponent<HERO>().die((obj6.transform.position - vector3) * 15f * this.myLevel, false);
                                }
                                else if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine || obj6.GetComponent<HERO>().HasDied()))
                                {
                                    obj6.GetComponent<HERO>().markDie();
                                    object[] objArray4 = new object[] { (obj6.transform.position - vector3) * 15f * this.myLevel, false, !this.nonAI ? -1 : photonView.viewID, name, true };
                                    obj6.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, objArray4);
                                }
                            }
                        }
                    }
                    if (this.attackCheckTimeA != 0f && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime <= this.attackCheckTimeB)
                    {
                        if (this.leftHandAttack)
                        {
                            GameObject obj7 = this.checkIfHitHand(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
                            if (obj7 != null)
                            {
                                Vector3 vector4 = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    obj7.GetComponent<HERO>().die((obj7.transform.position - vector4) * 15f * this.myLevel, false);
                                }
                                else if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine || obj7.GetComponent<HERO>().HasDied()))
                                {
                                    obj7.GetComponent<HERO>().markDie();
                                    object[] objArray5 = new object[] { (obj7.transform.position - vector4) * 15f * this.myLevel, false, !this.nonAI ? -1 : photonView.viewID, name, true };
                                    obj7.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, objArray5);
                                }
                            }
                        }
                        else
                        {
                            GameObject obj8 = this.checkIfHitHand(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
                            if (obj8 != null)
                            {
                                Vector3 vector5 = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    obj8.GetComponent<HERO>().die((obj8.transform.position - vector5) * 15f * this.myLevel, false);
                                }
                                else if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine || obj8.GetComponent<HERO>().HasDied()))
                                {
                                    obj8.GetComponent<HERO>().markDie();
                                    object[] objArray6 = new object[] { (obj8.transform.position - vector5) * 15f * this.myLevel, false, !this.nonAI ? -1 : photonView.viewID, name, true };
                                    obj8.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, objArray6);
                                }
                            }
                        }
                    }
                    if (!this.attacked && this.attackCheckTime != 0f && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTime)
                    {
                        GameObject obj9;
                        this.attacked = true;
                        this.fxPosition = this.baseTransform.Find("ap_" + this.attackAnimation).position;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                        {
                            obj9 = PhotonNetwork.Instantiate("FX/" + this.fxName, this.fxPosition, this.fxRotation, 0);
                        }
                        else
                        {
                            obj9 = (GameObject) Instantiate(Resources.Load("FX/" + this.fxName), this.fxPosition, this.fxRotation);
                        }
                        if (this.nonAI)
                        {
                            obj9.transform.localScale = this.baseTransform.localScale * 1.5f;
                            if (obj9.GetComponent<EnemyfxIDcontainer>() != null)
                            {
                                obj9.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = photonView.viewID;
                            }
                        }
                        else
                        {
                            obj9.transform.localScale = this.baseTransform.localScale;
                        }
                        if (obj9.GetComponent<EnemyfxIDcontainer>() != null)
                        {
                            obj9.GetComponent<EnemyfxIDcontainer>().titanName = name;
                        }
                        float b = 1f - Vector3.Distance(this.currentCamera.transform.position, obj9.transform.position) * 0.05f;
                        b = Mathf.Min(1f, b);
                        this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b, 0.95f);
                    }
                    if (this.attackAnimation == "throw")
                    {
                        if (!this.attacked && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 0.11f)
                        {
                            this.attacked = true;
                            Transform transform = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                            {
                                this.throwRock = PhotonNetwork.Instantiate("FX/rockThrow", transform.position, transform.rotation, 0);
                            }
                            else
                            {
                                this.throwRock = (GameObject) Instantiate(Resources.Load("FX/rockThrow"), transform.position, transform.rotation);
                            }
                            this.throwRock.transform.localScale = this.baseTransform.localScale;
                            Transform transform1 = this.throwRock.transform;
                            transform1.position -= this.throwRock.transform.forward * 2.5f * this.myLevel;
                            if (this.throwRock.GetComponent<EnemyfxIDcontainer>() != null)
                            {
                                if (this.nonAI)
                                {
                                    this.throwRock.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = photonView.viewID;
                                }
                                this.throwRock.GetComponent<EnemyfxIDcontainer>().titanName = name;
                            }
                            this.throwRock.transform.parent = transform;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                            {
                                object[] objArray7 = new object[] { photonView.viewID, this.baseTransform.localScale, this.throwRock.transform.localPosition, this.myLevel };
                                this.throwRock.GetPhotonView().RPC("initRPC", PhotonTargets.Others, objArray7);
                            }
                        }
                        if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 0.11f)
                        {
                            float y = Mathf.Atan2(this.myHero.transform.position.x - this.baseTransform.position.x, this.myHero.transform.position.z - this.baseTransform.position.z) * 57.29578f;
                            this.baseGameObjectTransform.rotation = Quaternion.Euler(0f, y, 0f);
                        }
                        if (this.throwRock != null && this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 0.62f)
                        {
                            Vector3 vector6;
                            float num3 = 1f;
                            float num4 = -20f;
                            if (this.myHero != null)
                            {
                                vector6 = (this.myHero.transform.position - this.throwRock.transform.position) / num3 + this.myHero.rigidbody.velocity;
                                float num5 = this.myHero.transform.position.y + 2f * this.myLevel;
                                float num6 = num5 - this.throwRock.transform.position.y;
                                vector6 = new Vector3(vector6.x, num6 / num3 - 0.5f * num4 * num3, vector6.z);
                            }
                            else
                            {
                                vector6 = this.baseTransform.forward * 60f + Vector3.up * 10f;
                            }
                            this.throwRock.GetComponent<RockThrow>().launch(vector6);
                            this.throwRock.transform.parent = null;
                            this.throwRock = null;
                        }
                    }
                    if (this.attackAnimation == "jumper_0" || this.attackAnimation == "crawler_jump_0")
                    {
                        if (!this.attacked)
                        {
                            if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 0.68f)
                            {
                                this.attacked = true;
                                if (this.myHero == null || this.nonAI)
                                {
                                    float num7 = 120f;
                                    Vector3 vector7 = this.baseTransform.forward * this.speed + Vector3.up * num7;
                                    if (this.nonAI && this.abnormalType == AbnormalType.Crawler)
                                    {
                                        num7 = 100f;
                                        float a = this.speed * 2.5f;
                                        a = Mathf.Min(a, 100f);
                                        vector7 = this.baseTransform.forward * a + Vector3.up * num7;
                                    }
                                    this.baseRigidBody.velocity = vector7;
                                }
                                else
                                {
                                    float num18;
                                    float num9 = this.myHero.rigidbody.velocity.y;
                                    float num10 = -20f;
                                    float gravity = this.gravity;
                                    float num12 = this.neck.position.y;
                                    float num13 = (num10 - gravity) * 0.5f;
                                    float num14 = num9;
                                    float num15 = this.myHero.transform.position.y - num12;
                                    float num16 = Mathf.Abs((Mathf.Sqrt(num14 * num14 - 4f * num13 * num15) - num14) / (2f * num13));
                                    Vector3 vector8 = this.myHero.transform.position + this.myHero.rigidbody.velocity * num16 + Vector3.up * 0.5f * num10 * num16 * num16;
                                    float num17 = vector8.y;
                                    if (num15 < 0f || num17 - num12 < 0f)
                                    {
                                        num18 = 60f;
                                        float num19 = this.speed * 2.5f;
                                        num19 = Mathf.Min(num19, 100f);
                                        Vector3 vector9 = this.baseTransform.forward * num19 + Vector3.up * num18;
                                        this.baseRigidBody.velocity = vector9;
                                        return;
                                    }
                                    float num20 = num17 - num12;
                                    float num21 = Mathf.Sqrt(2f * num20 / this.gravity);
                                    num18 = this.gravity * num21;
                                    num18 = Mathf.Max(30f, num18);
                                    Vector3 vector10 = (vector8 - this.baseTransform.position) / num16;
                                    this.abnorma_jump_bite_horizon_v = new Vector3(vector10.x, 0f, vector10.z);
                                    Vector3 velocity = this.baseRigidBody.velocity;
                                    Vector3 force = new Vector3(this.abnorma_jump_bite_horizon_v.x, velocity.y, this.abnorma_jump_bite_horizon_v.z) - velocity;
                                    this.baseRigidBody.AddForce(force, ForceMode.VelocityChange);
                                    this.baseRigidBody.AddForce(Vector3.up * num18, ForceMode.VelocityChange);
                                    float num22 = Vector2.Angle(new Vector2(this.baseTransform.position.x, this.baseTransform.position.z), new Vector2(this.myHero.transform.position.x, this.myHero.transform.position.z));
                                    num22 = Mathf.Atan2(this.myHero.transform.position.x - this.baseTransform.position.x, this.myHero.transform.position.z - this.baseTransform.position.z) * 57.29578f;
                                    this.baseGameObjectTransform.rotation = Quaternion.Euler(0f, num22, 0f);
                                }
                            }
                            else
                            {
                                this.baseRigidBody.velocity = Vector3.zero;
                            }
                        }
                        if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 1f)
                        {
                            Debug.DrawLine(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * this.myLevel, this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * this.myLevel + Vector3.up * 3f * this.myLevel, Color.green);
                            Debug.DrawLine(this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * this.myLevel, this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * this.myLevel + Vector3.forward * 3f * this.myLevel, Color.green);
                            GameObject obj10 = this.checkIfHitHead(this.head, 3f);
                            if (obj10 != null)
                            {
                                Vector3 vector13 = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    obj10.GetComponent<HERO>().die((obj10.transform.position - vector13) * 15f * this.myLevel, false);
                                }
                                else if (!(IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || !photonView.isMine || obj10.GetComponent<HERO>().HasDied()))
                                {
                                    obj10.GetComponent<HERO>().markDie();
                                    object[] objArray8 = new object[] { (obj10.transform.position - vector13) * 15f * this.myLevel, true, !this.nonAI ? -1 : photonView.viewID, name, true };
                                    obj10.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, objArray8);
                                }
                                if (this.abnormalType == AbnormalType.Crawler)
                                {
                                    this.attackAnimation = "crawler_jump_1";
                                }
                                else
                                {
                                    this.attackAnimation = "jumper_1";
                                }
                                this.playAnimation("attack_" + this.attackAnimation);
                            }
                            if (Mathf.Abs(this.baseRigidBody.velocity.y) < 0.5f || this.baseRigidBody.velocity.y < 0f || this.IsGrounded())
                            {
                                if (this.abnormalType == AbnormalType.Crawler)
                                {
                                    this.attackAnimation = "crawler_jump_1";
                                }
                                else
                                {
                                    this.attackAnimation = "jumper_1";
                                }
                                this.playAnimation("attack_" + this.attackAnimation);
                            }
                        }
                    }
                    else if (this.attackAnimation == "jumper_1" || this.attackAnimation == "crawler_jump_1")
                    {
                        if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 1f && this.grounded)
                        {
                            GameObject obj11;
                            if (this.abnormalType == AbnormalType.Crawler)
                            {
                                this.attackAnimation = "crawler_jump_2";
                            }
                            else
                            {
                                this.attackAnimation = "jumper_2";
                            }
                            this.crossFade("attack_" + this.attackAnimation, 0.1f);
                            this.fxPosition = this.baseTransform.position;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                            {
                                obj11 = PhotonNetwork.Instantiate("FX/boom2", this.fxPosition, this.fxRotation, 0);
                            }
                            else
                            {
                                obj11 = (GameObject) Instantiate(Resources.Load("FX/boom2"), this.fxPosition, this.fxRotation);
                            }
                            obj11.transform.localScale = this.baseTransform.localScale * 1.6f;
                            float num23 = 1f - Vector3.Distance(this.currentCamera.transform.position, obj11.transform.position) * 0.05f;
                            num23 = Mathf.Min(1f, num23);
                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(num23, num23, 0.95f);
                        }
                    }
                    else if (this.attackAnimation == "jumper_2" || this.attackAnimation == "crawler_jump_2")
                    {
                        if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 1f)
                        {
                            this.idle(0f);
                        }
                    }
                    else if (this.baseAnimation.IsPlaying("tired"))
                    {
                        if (this.baseAnimation["tired"].normalizedTime >= 1f + Mathf.Max(this.attackEndWait * 2f, 3f))
                        {
                            this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 3f));
                        }
                    }
                    else if (this.baseAnimation["attack_" + this.attackAnimation].normalizedTime >= 1f + this.attackEndWait)
                    {
                        if (this.nextAttackAnimation != null)
                        {
                            this.Attack(this.nextAttackAnimation);
                        }
                        else if (this.attackAnimation == "quick_turn_l" || this.attackAnimation == "quick_turn_r")
                        {
                            this.baseTransform.rotation = Quaternion.Euler(this.baseTransform.rotation.eulerAngles.x, this.baseTransform.rotation.eulerAngles.y + 180f, this.baseTransform.rotation.eulerAngles.z);
                            this.idle(UnityEngine.Random.Range(0.5f, 1f));
                            this.playAnimation("idle");
                        }
                        else if (this.abnormalType == AbnormalType.Abnormal || this.abnormalType == AbnormalType.Jumper)
                        {
                            this.attackCount++;
                            if (this.attackCount > 3 && this.attackAnimation == "abnormal_getup")
                            {
                                this.attackCount = 0;
                                this.crossFade("tired", 0.5f);
                            }
                            else
                            {
                                this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 3f));
                            }
                        }
                        else
                        {
                            this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 3f));
                        }
                    }
                }
                else if (this.state == TitanState.Grabbing)
                {
                    if (this.baseAnimation["grab_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA && this.baseAnimation["grab_" + this.attackAnimation].normalizedTime <= this.attackCheckTimeB && this.grabbedTarget == null)
                    {
                        GameObject grabTarget = this.checkIfHitHand(this.currentGrabHand);
                        if (grabTarget != null)
                        {
                            if (this.isGrabHandLeft)
                            {
                                this.eatSetL(grabTarget);
                                this.grabbedTarget = grabTarget;
                            }
                            else
                            {
                                this.eatSet(grabTarget);
                                this.grabbedTarget = grabTarget;
                            }
                        }
                    }
                    if (this.baseAnimation["grab_" + this.attackAnimation].normalizedTime >= 1f)
                    {
                        if (this.grabbedTarget != null)
                        {
                            this.eat();
                        }
                        else
                        {
                            this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 2f));
                        }
                    }
                }
                else if (this.state == TitanState.Eating)
                {
                    if (!(this.attacked || this.baseAnimation[this.attackAnimation].normalizedTime < 0.48f))
                    {
                        this.attacked = true;
                        this.justEatHero(this.grabbedTarget, this.currentGrabHand);
                    }
                    if (this.grabbedTarget != null)
                    {
                    }
                    if (this.baseAnimation[this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.Chasing)
                {
                    if (this.myHero == null)
                    {
                        this.idle(0f);
                    }
                    else if (!this.LongRangeAttackCheck())
                    {
                        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture && this.PVPfromCheckPt != null && this.myDistance > this.chaseDistance)
                        {
                            this.idle(0f);
                        }
                        else if (this.abnormalType == AbnormalType.Crawler)
                        {
                            Vector3 vector14 = this.myHero.transform.position - this.baseTransform.position;
                            float current = -Mathf.Atan2(vector14.z, vector14.x) * 57.29578f;
                            float f = -Mathf.DeltaAngle(current, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f);
                            if (this.myDistance < this.attackDistance * 3f && UnityEngine.Random.Range(0f, 1f) < 0.1f && Mathf.Abs(f) < 90f && this.myHero.transform.position.y < this.neck.position.y + 30f * this.myLevel && this.myHero.transform.position.y > this.neck.position.y + 10f * this.myLevel)
                            {
                                this.Attack("crawler_jump_0");
                            }
                            else
                            {
                                GameObject obj13 = this.checkIfHitCrawlerMouth(this.head, 2.2f);
                                if (obj13 != null)
                                {
                                    Vector3 vector15 = this.baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                    {
                                        obj13.GetComponent<HERO>().die((obj13.transform.position - vector15) * 15f * this.myLevel, false);
                                    }
                                    else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                                    {
                                        if (obj13.GetComponent<TITAN_EREN>() != null)
                                        {
                                            obj13.GetComponent<TITAN_EREN>().hitByTitan();
                                        }
                                        else if (!obj13.GetComponent<HERO>().HasDied())
                                        {
                                            obj13.GetComponent<HERO>().markDie();
                                            object[] objArray9 = new object[] { (obj13.transform.position - vector15) * 15f * this.myLevel, true, !this.nonAI ? -1 : photonView.viewID, name, true };
                                            obj13.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, objArray9);
                                        }
                                    }
                                }
                                if (this.myDistance < this.attackDistance && UnityEngine.Random.Range(0f, 1f) < 0.02f)
                                {
                                    this.idle(UnityEngine.Random.Range(0.05f, 0.2f));
                                }
                            }
                        }
                        else if (this.abnormalType == AbnormalType.Jumper && (this.myDistance > this.attackDistance && this.myHero.transform.position.y > this.head.position.y + 4f * this.myLevel || this.myHero.transform.position.y > this.head.position.y + 4f * this.myLevel) && Vector3.Distance(this.baseTransform.position, this.myHero.transform.position) < 1.5f * this.myHero.transform.position.y)
                        {
                            this.Attack("jumper_0");
                        }
                        else if (this.myDistance < this.attackDistance)
                        {
                            this.idle(UnityEngine.Random.Range(0.05f, 0.2f));
                        }
                    }
                }
                else if (this.state == TitanState.Wandering)
                {
                    float num26 = 0f;
                    float num27 = 0f;
                    if (this.myDistance < this.chaseDistance || this.whoHasTauntMe != null)
                    {
                        Vector3 vector16 = this.myHero.transform.position - this.baseTransform.position;
                        num26 = -Mathf.Atan2(vector16.z, vector16.x) * 57.29578f;
                        num27 = -Mathf.DeltaAngle(num26, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f);
                        if (this.isAlarm || Mathf.Abs(num27) < 90f)
                        {
                            this.Chase();
                            return;
                        }
                        if (!(this.isAlarm || this.myDistance >= this.chaseDistance * 0.1f))
                        {
                            this.Chase();
                            return;
                        }
                    }
                    if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.Turning)
                {
                    this.baseGameObjectTransform.rotation = Quaternion.Lerp(this.baseGameObjectTransform.rotation, Quaternion.Euler(0f, this.desDeg, 0f), Time.deltaTime * Mathf.Abs(this.turnDeg) * 0.015f);
                    if (this.baseAnimation[this.turnAnimation].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.Blinded)
                {
                    if (this.baseAnimation.IsPlaying("sit_hit_eye") && this.baseAnimation["sit_hit_eye"].normalizedTime >= 1f)
                    {
                        this.remainSitdown();
                    }
                    else if (this.baseAnimation.IsPlaying("hit_eye") && this.baseAnimation["hit_eye"].normalizedTime >= 1f)
                    {
                        if (this.nonAI)
                        {
                            this.idle(0f);
                        }
                        else
                        {
                            this.Attack("combo_1");
                        }
                    }
                }
                else if (this.state == TitanState.GoingToCheckpoint)
                {
                    if (this.checkPoints.Count <= 0 && this.myDistance < this.attackDistance)
                    {
                        string decidedAction = string.Empty;
                        string[] attackStrategy = this.GetAttackStrategy();
                        if (attackStrategy != null)
                        {
                            decidedAction = attackStrategy[UnityEngine.Random.Range(0, attackStrategy.Length)];
                        }
                        if (this.ExecuteAttack(decidedAction))
                        {
                            return;
                        }
                    }
                    if (Vector3.Distance(this.baseTransform.position, this.targetCheckPt) < this.targetR)
                    {
                        if (this.checkPoints.Count > 0)
                        {
                            if (this.checkPoints.Count == 1)
                            {
                                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight)
                                {
                                    this.MultiplayerManager.GameLose();
                                    this.checkPoints = new ArrayList();
                                    this.idle(0f);
                                }
                            }
                            else
                            {
                                if (this.checkPoints.Count == 4)
                                {
                                    this.MultiplayerManager.SendChatContentInfo("<color=#A8FF24>*WARNING!* An abnormal titan is approaching the north gate!</color>");
                                }
                                Vector3 vector17 = (Vector3) this.checkPoints[0];
                                this.targetCheckPt = vector17;
                                this.checkPoints.RemoveAt(0);
                            }
                        }
                        else
                        {
                            this.idle(0f);
                        }
                    }
                }
                else if (this.state == TitanState.GoingToPVP)
                {
                    if (this.myDistance < this.chaseDistance * 0.7f)
                    {
                        this.Chase();
                    }
                    if (Vector3.Distance(this.baseTransform.position, this.targetCheckPt) < this.targetR)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.RunningRandomly)
                {
                    this.random_run_time -= Time.deltaTime;
                    if (Vector3.Distance(this.baseTransform.position, this.targetCheckPt) < this.targetR || this.random_run_time <= 0f)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.Down)
                {
                    this.getdownTime -= Time.deltaTime;
                    if (this.baseAnimation.IsPlaying("sit_hunt_down") && this.baseAnimation["sit_hunt_down"].normalizedTime >= 1f)
                    {
                        this.playAnimation("sit_idle");
                    }
                    if (this.getdownTime <= 0f)
                    {
                        this.crossFade("sit_getup", 0.1f);
                    }
                    if (this.baseAnimation.IsPlaying("sit_getup") && this.baseAnimation["sit_getup"].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.Sit)
                {
                    this.getdownTime -= Time.deltaTime;
                    this.angle = 0f;
                    this.between2 = 0f;
                    if (this.myDistance < this.chaseDistance || this.whoHasTauntMe != null)
                    {
                        if (this.myDistance < 50f)
                        {
                            this.isAlarm = true;
                        }
                        else
                        {
                            Vector3 vector18 = this.myHero.transform.position - this.baseTransform.position;
                            this.angle = -Mathf.Atan2(vector18.z, vector18.x) * 57.29578f;
                            this.between2 = -Mathf.DeltaAngle(this.angle, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f);
                            if (Mathf.Abs(this.between2) < 100f)
                            {
                                this.isAlarm = true;
                            }
                        }
                    }
                    if (this.baseAnimation.IsPlaying("sit_down") && this.baseAnimation["sit_down"].normalizedTime >= 1f)
                    {
                        this.playAnimation("sit_idle");
                    }
                    if (!(this.getdownTime <= 0f || this.isAlarm ? !this.baseAnimation.IsPlaying("sit_idle") : true))
                    {
                        this.crossFade("sit_getup", 0.1f);
                    }
                    if (this.baseAnimation.IsPlaying("sit_getup") && this.baseAnimation["sit_getup"].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                }
                else if (this.state == TitanState.Recovering)
                {
                    this.getdownTime -= Time.deltaTime;
                    if (this.getdownTime <= 0f)
                    {
                        this.idle(0f);
                    }
                    if (this.baseAnimation.IsPlaying("idle_recovery") && this.baseAnimation["idle_recovery"].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                }
            }
            else if (this.nonAI)
            {
                if (!IN_GAME_MAIN_CAMERA.isPausing)
                {
                    this.pt();
                    if (this.abnormalType != AbnormalType.Crawler)
                    {
                        if (this.controller.isAttackDown && this.stamina > 25f)
                        {
                            this.stamina -= 25f;
                            this.Attack("combo_1");
                        }
                        else if (this.controller.isAttackIIDown && this.stamina > 50f)
                        {
                            this.stamina -= 50f;
                            this.Attack("abnormal_jump");
                        }
                        else if (this.controller.isJumpDown && this.stamina > 15f)
                        {
                            this.stamina -= 15f;
                            this.Attack("jumper_0");
                        }
                    }
                    else if (this.controller.isAttackDown && this.stamina > 40f)
                    {
                        this.stamina -= 40f;
                        this.Attack("crawler_jump_0");
                    }
                    if (this.controller.isSuicide)
                    {
                        this.suicide();
                    }
                }
            }
            else if (this.sbtime > 0f)
            {
                this.sbtime -= Time.deltaTime;
            }
            else
            {
                if (!this.isAlarm)
                {
                    if (this.abnormalType != AbnormalType.Punk && this.abnormalType != AbnormalType.Crawler && UnityEngine.Random.Range(0f, 1f) < 0.005f)
                    {
                        this.sitdown();
                        return;
                    }
                    if (UnityEngine.Random.Range(0f, 1f) < 0.02f)
                    {
                        this.wander(0f);
                        return;
                    }
                    if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                    {
                        this.turn(UnityEngine.Random.Range(30, 120));
                        return;
                    }
                    if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                    {
                        this.turn(UnityEngine.Random.Range(-30, -120));
                        return;
                    }
                }
                this.angle = 0f;
                this.between2 = 0f;
                if (this.myDistance < this.chaseDistance || this.whoHasTauntMe != null)
                {
                    Vector3 vector = this.myHero.transform.position - this.baseTransform.position;
                    this.angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                    this.between2 = -Mathf.DeltaAngle(this.angle, this.baseGameObjectTransform.rotation.eulerAngles.y - 90f);
                    if (this.myDistance >= this.attackDistance)
                    {
                        if (this.isAlarm || Mathf.Abs(this.between2) < 90f)
                        {
                            this.Chase();
                            return;
                        }
                        if (!(this.isAlarm || this.myDistance >= this.chaseDistance * 0.1f))
                        {
                            this.Chase();
                            return;
                        }
                    }
                }
                if (!this.LongRangeAttackCheck())
                {
                    if (this.myDistance < this.chaseDistance)
                    {
                        if (this.abnormalType == AbnormalType.Jumper && (this.myDistance > this.attackDistance || this.myHero.transform.position.y > this.head.position.y + 4f * this.myLevel) && Mathf.Abs(this.between2) < 120f && Vector3.Distance(this.baseTransform.position, this.myHero.transform.position) < 1.5f * this.myHero.transform.position.y)
                        {
                            this.Attack("jumper_0");
                            return;
                        }
                        if (this.abnormalType == AbnormalType.Crawler && this.myDistance < this.attackDistance * 3f && Mathf.Abs(this.between2) < 90f && this.myHero.transform.position.y < this.neck.position.y + 30f * this.myLevel && this.myHero.transform.position.y > this.neck.position.y + 10f * this.myLevel)
                        {
                            this.Attack("crawler_jump_0");
                            return;
                        }
                    }
                    if (this.abnormalType == AbnormalType.Punk && this.myDistance < 90f && Mathf.Abs(this.between2) > 90f)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < 0.4f)
                        {
                            this.randomRun(this.baseTransform.position + new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), 10f);
                        }
                        if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
                        {
                            this.recover();
                        }
                        else if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            this.Attack("quick_turn_l");
                        }
                        else
                        {
                            this.Attack("quick_turn_r");
                        }
                    }
                    else
                    {
                        if (this.myDistance < this.attackDistance)
                        {
                            if (this.abnormalType == AbnormalType.Crawler)
                            {
                                if (this.myHero.transform.position.y + 3f <= this.neck.position.y + 20f * this.myLevel && UnityEngine.Random.Range(0f, 1f) < 0.1f)
                                {
                                    this.Chase();
                                }
                                return;
                            }
                            string str = string.Empty;
                            string[] strArray = this.GetAttackStrategy();
                            if (strArray != null)
                            {
                                str = strArray[UnityEngine.Random.Range(0, strArray.Length)];
                            }
                            if (!(this.abnormalType == AbnormalType.Jumper || this.abnormalType == AbnormalType.Abnormal ? Mathf.Abs(this.between2) <= 40f : true))
                            {
                                if (str.Contains("grab") || str.Contains("kick") || str.Contains("slap") || str.Contains("bite"))
                                {
                                    if (UnityEngine.Random.Range(0, 100) < 30)
                                    {
                                        this.turn(this.between2);
                                        return;
                                    }
                                }
                                else if (UnityEngine.Random.Range(0, 100) < 90)
                                {
                                    this.turn(this.between2);
                                    return;
                                }
                            }
                            if (this.ExecuteAttack(str))
                            {
                                return;
                            }
                            if (this.abnormalType == AbnormalType.Normal)
                            {
                                if (UnityEngine.Random.Range(0, 100) < 30 && Mathf.Abs(this.between2) > 45f)
                                {
                                    this.turn(this.between2);
                                    return;
                                }
                            }
                            else if (Mathf.Abs(this.between2) > 45f)
                            {
                                this.turn(this.between2);
                                return;
                            }
                        }
                        if (this.PVPfromCheckPt != null)
                        {
                            if (this.PVPfromCheckPt.state == CheckPointState.Titan)
                            {
                                GameObject chkPtNext;
                                if (UnityEngine.Random.Range(0, 100) > 48)
                                {
                                    chkPtNext = this.PVPfromCheckPt.chkPtNext;
                                    if (chkPtNext != null && (chkPtNext.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || UnityEngine.Random.Range(0, 100) < 20))
                                    {
                                        this.toPVPCheckPoint(chkPtNext.transform.position, 5 + UnityEngine.Random.Range(0, 10));
                                        this.PVPfromCheckPt = chkPtNext.GetComponent<PVPcheckPoint>();
                                    }
                                }
                                else
                                {
                                    chkPtNext = this.PVPfromCheckPt.chkPtPrevious;
                                    if (chkPtNext != null && (chkPtNext.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || UnityEngine.Random.Range(0, 100) < 5))
                                    {
                                        this.toPVPCheckPoint(chkPtNext.transform.position, 5 + UnityEngine.Random.Range(0, 10));
                                        this.PVPfromCheckPt = chkPtNext.GetComponent<PVPcheckPoint>();
                                    }
                                }
                            }
                            else
                            {
                                this.toPVPCheckPoint(this.PVPfromCheckPt.transform.position, 5 + UnityEngine.Random.Range(0, 10));
                            }
                        }
                    }
                }
            }
        }
    }

    public void updateCollider()
    {
        if (this.colliderEnabled)
        {
            if (!this.isHooked && !this.myTitanTrigger.isCollide && !this.isLook)
            {
                foreach (Collider collider in this.baseColliders)
                {
                    if (collider != null)
                    {
                        collider.enabled = false;
                    }
                }
                this.colliderEnabled = false;
            }
        }
        else if (this.isHooked || this.myTitanTrigger.isCollide || this.isLook)
        {
            foreach (Collider collider in this.baseColliders)
            {
                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
            this.colliderEnabled = true;
        }
    }

    public void updateLabel()
    {
        if (this.healthLabel != null && this.healthLabel.GetComponent<UILabel>().isVisible)
        {
            this.healthLabel.transform.LookAt(2f * this.healthLabel.transform.position - Camera.main.transform.position);
        }
    }

    private void wander(float sbtime = 0f)
    {
        this.state = TitanState.Wandering;
        this.crossFade(this.runAnimation, 0.5f);
    }
}