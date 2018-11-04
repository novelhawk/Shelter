using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using JetBrains.Annotations;
using Mod;
using Mod.Exceptions;
using Mod.GameSettings;
using Mod.Interface;
using Mod.Keybinds;
using Mod.Managers;
using Mod.Modules;
using Photon;
using Photon.Enums;
using RC;
using UnityEngine;
using Xft;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// ReSharper disable once CheckNamespace
public class HERO : Photon.MonoBehaviour
{
    private HeroState _state;
    private bool almostSingleHook;
    private string attackAnimation;
    private int attackLoop;
    private bool attackMove;
    private bool attackReleased;
    public AudioSource audio_ally;
    public AudioSource audio_hitwall;
    private GameObject badGuy;
    public Animation baseAnimation;
    public Rigidbody baseRigidBody;
    public Transform baseTransform;
    public bool bigLean;
    public float bombCD;
    public bool bombImmune;
    public float bombRadius;
    public float bombSpeed;
    public float bombTime;
    public float bombTimeMax;
    private float _speedupTime;
    public GameObject bulletLeft;
    private int bulletMAX = 7;
    public GameObject bulletRight;
    private bool buttonAttackRelease;
    public Dictionary<string, UISprite> cachedSprites;
    public float CameraMultiplier;
    public bool canJump = true;
    public GameObject checkBoxLeft;
    public GameObject checkBoxRight;
    public GameObject cross1;
    public GameObject cross2;
    public string currentAnimation;
    private int currentBladeNum = 5;
    private float currentBladeSta = 100f;
    private bool _isSpeedup; //TODO: Replace with (_speedupTime > 0f)
    public Camera currentCamera;
    private float currentGas = 100f;
    public float currentSpeed;
    public Vector3 currentV;
    private bool dashD;
    private Vector3 dashDirection;
    private bool dashL;
    private bool dashR;
    private float dashTime;
    private bool dashU;
    private Vector3 dashV;
    public bool detonate;
    private float dTapTime = -1f;
    private bool EHold;
    private GameObject eren_titan;
    private int escapeTimes = 1;
    private float facingDirection;
    private float flare1CD;
    private float flare2CD;
    private float flare3CD;
    private float flareTotalCD = 30f;
    private Transform forearmL;
    private Transform forearmR;
    private float gravity = 20f;
    private bool grounded;
    private GameObject gunDummy;
    private Vector3 gunTarget;
    private Transform handL;
    private Transform handR;
    private bool hasDied;
    public bool hasspawn;
    private bool hookBySomeOne = true;
    public GameObject hookRefL1;
    public GameObject hookRefL2;
    public GameObject hookRefR1;
    public GameObject hookRefR2;
    private bool hookSomeOne;
    private GameObject hookTarget;
    private float invincible = 3f;
    public bool isCannon;
    private bool isLaunchLeft;
    private bool isLaunchRight;
    private bool isLeftHandHooked;
    private bool isMounted;
    public bool isPhotonCamera;
    private bool isRightHandHooked;
    public float jumpHeight = 2f;
    private bool justGrounded;
    public GameObject LabelDistance;
    public Transform lastHook;
    private float launchElapsedTimeL;
    private float launchElapsedTimeR;
    private Vector3 launchForce;
    private Vector3 launchPointLeft;
    private Vector3 launchPointRight;
    private bool leanLeft;
    private bool leftArmAim;
    public XWeaponTrail leftbladetrail;
    public XWeaponTrail leftbladetrail2;
    private int leftBulletLeft = 7;
    private bool leftGunHasBullet = true;
    private float lTapTime = -1f;
    public GameObject maincamera;
    public float maxVelocityChange = 10f;
    public AudioSource meatDie;
    public Bomb myBomb;
    public GameObject myCannon;
    public Transform myCannonBase;
    public Transform myCannonPlayer;
    public CannonPropRegion myCannonRegion;
    public Group myGroup;
    private GameObject myHorse;
    public GameObject myNetWorkName;
    public float myScale = 1f;
    public int myTeam = 1;
    public List<TITAN> myTitans;
    private bool needLean;
    private Quaternion oldHeadRotation;
    private float originVM;
    private bool QHold;
    private string reloadAnimation = string.Empty;
    private bool rightArmAim;
    public XWeaponTrail rightbladetrail;
    public XWeaponTrail rightbladetrail2;
    private int rightBulletLeft = 7;
    private bool rightGunHasBullet = true;
    public AudioSource rope;
    private float rTapTime = -1f;
    public HERO_SETUP setup;
    private GameObject skillCD;
    public float skillCDDuration;
    public float skillCDLast;
    public float skillCDLastCannon;
    private string skillId;
    public string skillIDHUD;
    public AudioSource slash;
    public AudioSource slashHit;
    private ParticleSystem smoke_3dmg;
    private ParticleSystem sparks;
    public float speed = 10f;
    public GameObject speedFX;
    public GameObject speedFX1;
    private ParticleSystem speedFXPS;
    public bool spinning;
    private string standAnimation = "stand";
    private Quaternion targetHeadRotation;
    private Quaternion targetRotation;
    public Vector3 targetV;
    private bool throwedBlades;
    public bool titanForm;
    private GameObject titanWhoGrabMe;
    private int titanWhoGrabMeID;
    private int totalBladeNum = 5;
    public float totalBladeSta = 100f;
    public float totalGas = 100f;
    private Transform upperarmL;
    private Transform upperarmR;
    private float useGasSpeed = 0.2f;
    public bool useGun;
    private float uTapTime = -1f;
    private bool wallJump;
    private float wallRunTime;

    private void applyForceToBody(GameObject GO, Vector3 v)
    {
        GO.rigidbody.AddForce(v);
        GO.rigidbody.AddTorque(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
    }

    public void attackAccordingToMouse()
    {
        if (Input.mousePosition.x < Screen.width * 0.5)
        {
            this.attackAnimation = "attack2";
        }
        else
        {
            this.attackAnimation = "attack1";
        }
    }

    public void attackAccordingToTarget(Transform a)
    {
        Vector3 vector = a.position - transform.position;
        float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        float f = -Mathf.DeltaAngle(current, transform.rotation.eulerAngles.y - 90f);
        if (Mathf.Abs(f) < 90f && vector.magnitude < 6f && a.position.y <= transform.position.y + 2f && a.position.y >= transform.position.y - 5f)
        {
            this.attackAnimation = "attack4";
        }
        else if (f > 0f)
        {
            this.attackAnimation = "attack1";
        }
        else
        {
            this.attackAnimation = "attack2";
        }
    }

    private void Awake()
    {
        this.cache();
        this.setup = gameObject.GetComponent<HERO_SETUP>();
        this.baseRigidBody.freezeRotation = true;
        this.baseRigidBody.useGravity = false;
        this.handL = this.baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
        this.handR = this.baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
        this.forearmL = this.baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L");
        this.forearmR = this.baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
        this.upperarmL = this.baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
        this.upperarmR = this.baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
    }

    public void backToHuman()
    {
        gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
        rigidbody.velocity = Vector3.zero;
        this.titanForm = false;
        this.ungrabbed();
        this.falseAttack();
        this.skillCDDuration = this.skillCDLast;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject, true, false);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            photonView.RPC(Rpc.SwitchToHuman, PhotonTargets.Others, new object[0]);
        }
    }

    [RPC]
    [UsedImplicitly]
    private void BackToHumanRPC()
    {
        this.titanForm = false;
        this.eren_titan = null;
        gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
    }

    [RPC]
    [UsedImplicitly]
    public void BadGuyReleaseMe()
    {
        this.hookBySomeOne = false;
        this.badGuy = null;
    }

    [RPC]
    [UsedImplicitly]
    public void BlowAway(Vector3 force, PhotonMessageInfo info = null)
    {
        if (info != null && !info.sender.IsLocal && !info.sender.IsMasterClient && info.sender.Properties.PlayerType != PlayerType.Titan) // This allows MC to blowAway need TODO: To check for force valididy
            throw new NotAllowedException(nameof(BlowAway), info);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            rigidbody.AddForce(force, ForceMode.Impulse);
            transform.LookAt(transform.position);
        }
    }

    private void bodyLean()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            float z = 0f;
            this.needLean = false;
            if (!this.useGun && this.State == HeroState.Attack && this.attackAnimation != "attack3_1" && this.attackAnimation != "attack3_2")
            {
                float y = rigidbody.velocity.y;
                float x = rigidbody.velocity.x;
                float num4 = rigidbody.velocity.z;
                float num5 = Mathf.Sqrt(x * x + num4 * num4);
                float num6 = Mathf.Atan2(y, num5) * 57.29578f;
                this.targetRotation = Quaternion.Euler(-num6 * (1f - Vector3.Angle(rigidbody.velocity, transform.forward) / 90f), this.facingDirection, 0f);
                if (this.isLeftHandHooked && this.bulletLeft != null || this.isRightHandHooked && this.bulletRight != null)
                {
                    transform.rotation = this.targetRotation;
                }
            }
            else
            {
                if (this.isLeftHandHooked && this.bulletLeft != null && this.isRightHandHooked && this.bulletRight != null)
                {
                    if (this.almostSingleHook)
                    {
                        this.needLean = true;
                        z = this.getLeanAngle(this.bulletRight.transform.position, true);
                    }
                }
                else if (this.isLeftHandHooked && this.bulletLeft != null)
                {
                    this.needLean = true;
                    z = this.getLeanAngle(this.bulletLeft.transform.position, true);
                }
                else if (this.isRightHandHooked && this.bulletRight != null)
                {
                    this.needLean = true;
                    z = this.getLeanAngle(this.bulletRight.transform.position, false);
                }
                if (this.needLean)
                {
                    float a = 0f;
                    if (!this.useGun && this.State != HeroState.Attack)
                    {
                        a = this.currentSpeed * 0.1f;
                        a = Mathf.Min(a, 20f);
                    }
                    this.targetRotation = Quaternion.Euler(-a, this.facingDirection, z);
                }
                else if (this.State != HeroState.Attack)
                {
                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                }
            }
        }
    }

    public void bombInit()
    {
        this.skillIDHUD = this.skillId;
        this.skillCDDuration = this.skillCDLast;
        if (FengGameManagerMKII.settings.IsBombMode)
        {
            int num = FengGameManagerMKII.settings.BombRadius;
            int num2 = FengGameManagerMKII.settings.BombRange;
            int num3 = FengGameManagerMKII.settings.BombSpeed;
            int num4 = FengGameManagerMKII.settings.BombCountdown;
            if (num < 0 || num > 10)
            {
                num = 5;
                FengGameManagerMKII.settings.BombRadius = 5;
            }
            if (num2 < 0 || num2 > 10)
            {
                num2 = 5;
                FengGameManagerMKII.settings.BombRange = 5;
            }
            if (num3 < 0 || num3 > 10)
            {
                num3 = 5;
                FengGameManagerMKII.settings.BombSpeed = 5;
            }
            if (num4 < 0 || num4 > 10)
            {
                num4 = 5;
                FengGameManagerMKII.settings.BombCountdown = 5;
            }
            if (num + num2 + num3 + num4 > 20)
            {
                num = 5;
                num2 = 5;
                num3 = 5;
                num4 = 5;
                FengGameManagerMKII.settings.BombRadius = 5;
                FengGameManagerMKII.settings.BombRange = 5;
                FengGameManagerMKII.settings.BombSpeed = 5;
                FengGameManagerMKII.settings.BombCountdown = 5;
            }
            this.bombTimeMax = (num2 * 60f + 200f) / (num3 * 60f + 200f);
            this.bombRadius = num * 4f + 20f;
            this.bombCD = num4 * -0.4f + 5f;
            this.bombSpeed = num3 * 60f + 200f;
            var color = FengGameManagerMKII.settings.BombColor;
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.RCBombR, color.r },
                { PlayerProperty.RCBombG, color.g },
                { PlayerProperty.RCBombB, color.b },
                { PlayerProperty.RCBombA, color.a },
                { PlayerProperty.RCBombRadius, this.bombRadius }
            });
            this.skillId = "bomb";
            this.skillIDHUD = "armin";
            this.skillCDLast = this.bombCD;
            this.skillCDDuration = 10f;
            if (FengGameManagerMKII.instance.roundTime > 10f)
            {
                this.skillCDDuration = 5f;
            }
        }
    }

    private void BreakApart(Vector3 force, bool isBite)
    {
        GameObject obj6;
        GameObject obj7;
        GameObject obj8;
        GameObject obj9;
        GameObject obj10;
        GameObject obj2 = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), this.transform.position, this.transform.rotation);
        obj2.gameObject.GetComponent<HERO_SETUP>().myCostume = this.setup.myCostume;
        obj2.GetComponent<HERO_SETUP>().isDeadBody = true;
        obj2.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, animation[this.currentAnimation].normalizedTime, BodyPart.RightArm);
        if (!isBite)
        {
            GameObject gO = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), this.transform.position, this.transform.rotation);
            GameObject obj4 = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), this.transform.position, this.transform.rotation);
            GameObject obj5 = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), this.transform.position, this.transform.rotation);
            gO.gameObject.GetComponent<HERO_SETUP>().myCostume = this.setup.myCostume;
            obj4.gameObject.GetComponent<HERO_SETUP>().myCostume = this.setup.myCostume;
            obj5.gameObject.GetComponent<HERO_SETUP>().myCostume = this.setup.myCostume;
            gO.GetComponent<HERO_SETUP>().isDeadBody = true;
            obj4.GetComponent<HERO_SETUP>().isDeadBody = true;
            obj5.GetComponent<HERO_SETUP>().isDeadBody = true;
            gO.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, animation[this.currentAnimation].normalizedTime, BodyPart.Upper);
            obj4.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, animation[this.currentAnimation].normalizedTime, BodyPart.Lower);
            obj5.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, animation[this.currentAnimation].normalizedTime, BodyPart.LeftArm);
            this.applyForceToBody(gO, force);
            this.applyForceToBody(obj4, force);
            this.applyForceToBody(obj5, force);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gO, false, false);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj2, false, false);
        }
        this.applyForceToBody(obj2, force);
        Transform transform = this.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L").transform;
        Transform transform2 = this.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R").transform;
        if (this.useGun)
        {
            obj6 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_l"), transform.position, transform.rotation);
            obj7 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_r"), transform2.position, transform2.rotation);
            obj8 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg_2"), this.transform.position, this.transform.rotation);
            obj9 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_mag_l"), this.transform.position, this.transform.rotation);
            obj10 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_mag_r"), this.transform.position, this.transform.rotation);
        }
        else
        {
            obj6 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_l"), transform.position, transform.rotation);
            obj7 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
            obj8 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg"), this.transform.position, this.transform.rotation);
            obj9 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg_gas_l"), this.transform.position, this.transform.rotation);
            obj10 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg_gas_r"), this.transform.position, this.transform.rotation);
        }
        obj6.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        obj7.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        obj8.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        obj9.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        obj10.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        this.applyForceToBody(obj6, force);
        this.applyForceToBody(obj7, force);
        this.applyForceToBody(obj8, force);
        this.applyForceToBody(obj9, force);
        this.applyForceToBody(obj10, force);
    }

    private void SpeedupUpdate()
    {
        if (_isSpeedup)
        {
            _speedupTime -= Time.deltaTime;
            if (_speedupTime <= 0f)
            {
                _speedupTime = 0f;
                if (_isSpeedup && animation.IsPlaying("run_sasha"))
                    this.crossFade("run", 0.1f);
                
                _isSpeedup = false;
            }
        }
    }

    public void cache()
    {
        this.baseTransform = transform;
        this.baseRigidBody = rigidbody;
        this.maincamera = GameObject.Find("MainCamera");
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            this.baseAnimation = animation;
            this.cross1 = GameObject.Find("cross1");
            this.cross2 = GameObject.Find("cross2");
            this.LabelDistance = GameObject.Find("LabelDistance");
            this.cachedSprites = new Dictionary<string, UISprite>();
            foreach (var o in FindObjectsOfType(typeof(GameObject)))
            {
                var sprite = (GameObject) o;
                if (sprite.GetComponent<UISprite>() != null && sprite.activeInHierarchy)
                {
                    string spriteName = sprite.name;
                    
                    // (name.Contains("blade") || name.Contains("bullet") || name.Contains("gas") || name.Contains("flare") || name.Contains("skill_cd")) && !cachedSprites.ContainsKey(name) )
                    
                    
                    if ((spriteName.Contains("blade") ||
                         spriteName.Contains("bullet") ||
                         spriteName.Contains("gas") || 
                         spriteName.Contains("flare") || 
                         spriteName.Contains("skill_cd")) && !cachedSprites.ContainsKey(spriteName))
                    {
                        this.cachedSprites.Add(spriteName, sprite.GetComponent<UISprite>());
                    }
                }
            }
        }
    }


    private void calcFlareCD()
    {
        if (this.flare1CD > 0f)
        {
            this.flare1CD -= Time.deltaTime;
            if (flare1CD < 0) flare1CD = 0;
        }
        if (this.flare2CD > 0f)
        {
            this.flare2CD -= Time.deltaTime;
            if (flare2CD < 0) flare2CD = 0;
        }
        if (this.flare3CD > 0f)
        {
            this.flare3CD -= Time.deltaTime;
            if (flare3CD < 0) flare3CD = 0;
        }
    }

    private void calcSkillCD()
    {
        if (this.skillCDDuration > 0f)
        {
            this.skillCDDuration -= Time.deltaTime;
            if (skillCDDuration < 0f) skillCDDuration = 0f;
            
            if (ModuleManager.Enabled(nameof(ModuleNoSkillCD)))
                skillCDDuration = 0;
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2f * this.jumpHeight * this.gravity);
    }

    private void changeBlade()
    {
        if (!this.useGun || this.grounded || LevelInfoManager.Get(FengGameManagerMKII.Level).Gamemode != GameMode.PvpAHSS)
        {
            this.State = HeroState.ChangeBlade;
            this.throwedBlades = false;
            if (this.useGun)
            {
                if (!this.leftGunHasBullet && !this.rightGunHasBullet)
                {
                    if (this.grounded)
                        this.reloadAnimation = "AHSS_gun_reload_both";
                    else
                        this.reloadAnimation = "AHSS_gun_reload_both_air";
                }
                else if (!this.leftGunHasBullet)
                {
                    if (this.grounded)
                        this.reloadAnimation = "AHSS_gun_reload_l";
                    else
                        this.reloadAnimation = "AHSS_gun_reload_l_air";
                }
                else if (!this.rightGunHasBullet)
                {
                    if (this.grounded)
                        this.reloadAnimation = "AHSS_gun_reload_r";
                    else
                        this.reloadAnimation = "AHSS_gun_reload_r_air";
                }
                else
                {
                    if (this.grounded)
                        this.reloadAnimation = "AHSS_gun_reload_both";
                    else
                        this.reloadAnimation = "AHSS_gun_reload_both_air";
                    this.rightGunHasBullet = false;
                    this.leftGunHasBullet = false;
                }
                this.crossFade(this.reloadAnimation, 0.05f);
            }
            else
            {
                if (!this.grounded)
                    this.reloadAnimation = "changeBlade_air";
                else
                    this.reloadAnimation = "changeBlade";
                this.crossFade(this.reloadAnimation, 0.1f);
            }
        }
    }

    private void checkDashDoubleTap()
    {
        if (this.uTapTime >= 0f)
        {
            this.uTapTime += Time.deltaTime;
            if (this.uTapTime > 0.2f)
                this.uTapTime = -1f;
        }
        if (this.dTapTime >= 0f)
        {
            this.dTapTime += Time.deltaTime;
            if (this.dTapTime > 0.2f)
                this.dTapTime = -1f;
        }
        if (this.lTapTime >= 0f)
        {
            this.lTapTime += Time.deltaTime;
            if (this.lTapTime > 0.2f)
                this.lTapTime = -1f;
        }
        if (this.rTapTime >= 0f)
        {
            this.rTapTime += Time.deltaTime;
            if (this.rTapTime > 0.2f)
                this.rTapTime = -1f;
        }
        if (Shelter.InputManager.IsDown(InputAction.Forward))
        {
            if (this.uTapTime == -1f)
                this.uTapTime = 0f;
            
            if (this.uTapTime != 0f)
                this.dashU = true;
        }
        if (Shelter.InputManager.IsDown(InputAction.Back))
        {
            if (this.dTapTime == -1f)
                this.dTapTime = 0f;
            
            if (this.dTapTime != 0f)
                this.dashD = true;
        }
        if (Shelter.InputManager.IsDown(InputAction.Left))
        {
            if (this.lTapTime == -1f)
                this.lTapTime = 0f;
            
            if (this.lTapTime != 0f)
                this.dashL = true;
        }
        if (Shelter.InputManager.IsDown(InputAction.Right))
        {
            if (this.rTapTime == -1f)
                this.rTapTime = 0f;
            
            if (this.rTapTime != 0f)
                this.dashR = true;
        }
    }

    public void checkTitan()
    {
        int count;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = 1 << LayerMask.NameToLayer("PlayerAttackBox");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask3 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask4 = mask | mask2 | mask3;
        RaycastHit[] hitArray = Physics.RaycastAll(ray, 180f, mask4.value);
        List<RaycastHit> list = new List<RaycastHit>();
        List<TITAN> list2 = new List<TITAN>();
        for (count = 0; count < hitArray.Length; count++)
        {
            RaycastHit item = hitArray[count];
            list.Add(item);
        }
        list.Sort((x, y) => x.distance.CompareTo(y.distance));
        float num2 = 180f;
        for (count = 0; count < list.Count; count++)
        {
            RaycastHit hit2 = list[count];
            GameObject gameObject = hit2.collider.gameObject;
            if (gameObject.layer == 16)
            {
                if (gameObject.name.Contains("PlayerDetectorRC") && (hit2 = list[count]).distance < num2)
                {
                    num2 -= 60f;
                    if (num2 <= 60f)
                    {
                        count = list.Count;
                    }
                    TITAN component = gameObject.transform.root.gameObject.GetComponent<TITAN>();
                    if (component != null)
                    {
                        list2.Add(component);
                    }
                }
            }
            else
            {
                count = list.Count;
            }
        }
        for (count = 0; count < this.myTitans.Count; count++)
        {
            TITAN titan2 = this.myTitans[count];
            if (!list2.Contains(titan2))
            {
                titan2.isLook = false;
            }
        }
        for (count = 0; count < list2.Count; count++)
        {
            TITAN titan3 = list2[count];
            titan3.isLook = true;
        }
        this.myTitans = list2;
    }

    public void continueAnimation()
    {
        foreach (AnimationState current in animation)
        {
            if (current == null)
                continue;
            
            if (current.speed == 1f)
                return;

            current.speed = 1f;
        }
        
        this.customAnimationSpeed();
        this.playAnimation(this.currentPlayingClipName());
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
            photonView.RPC(Rpc.ContinueAnimation, PhotonTargets.Others);
    }

    public void crossFade(string aniName, float time)
    {
        this.currentAnimation = aniName;
        animation.CrossFade(aniName, time);
        if (PhotonNetwork.connected && photonView.isMine)
            photonView.RPC(Rpc.CrossFade, PhotonTargets.Others, aniName, time);
    }

    public string currentPlayingClipName()
    {
        foreach (AnimationState current in animation)
        {
            if (current != null && animation.IsPlaying(current.name))
                return current.name;
        }
        return string.Empty;
    }

    private void customAnimationSpeed()
    {
        animation["attack5"].speed = 1.85f;
        animation["changeBlade"].speed = 1.2f;
        animation["air_release"].speed = 0.6f;
        animation["changeBlade_air"].speed = 0.8f;
        animation["AHSS_gun_reload_both"].speed = 0.38f;
        animation["AHSS_gun_reload_both_air"].speed = 0.5f;
        animation["AHSS_gun_reload_l"].speed = 0.4f;
        animation["AHSS_gun_reload_l_air"].speed = 0.5f;
        animation["AHSS_gun_reload_r"].speed = 0.4f;
        animation["AHSS_gun_reload_r_air"].speed = 0.5f;
    }

    private void dash(float horizontal, float vertical)
    {
        if (this.dashTime <= 0f && this.currentGas > 0f && !this.isMounted)
        {
            this.useGas(this.totalGas * 0.04f);
            this.facingDirection = this.getGlobalFacingDirection(horizontal, vertical);
            this.dashV = this.getGlobaleFacingVector3(this.facingDirection);
            this.originVM = this.currentSpeed;
            Quaternion quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
            rigidbody.rotation = quaternion;
            this.targetRotation = quaternion;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                Instantiate(Resources.Load("FX/boost_smoke"), transform.position, transform.rotation);
            }
            else
            {
                PhotonNetwork.Instantiate("FX/boost_smoke", transform.position, transform.rotation, 0);
            }
            this.dashTime = 0.5f;
            this.crossFade("dash", 0.1f);
            animation["dash"].time = 0.1f;
            this.State = HeroState.AirDodge;
            this.falseAttack();
            rigidbody.AddForce(this.dashV * 40f, ForceMode.VelocityChange);
        }
    }

    public void die(Vector3 v, bool isBite)
    {
        if (this.invincible <= 0f)
        {
            if (this.titanForm && this.eren_titan != null)
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.bulletLeft != null)
            {
                this.bulletLeft.GetComponent<Bullet>().removeMe();
            }
            if (this.bulletRight != null)
            {
                this.bulletRight.GetComponent<Bullet>().removeMe();
            }
            this.meatDie.Play();
            if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine) && !this.useGun)
            {
                this.leftbladetrail.Deactivate();
                this.rightbladetrail.Deactivate();
                this.leftbladetrail2.Deactivate();
                this.rightbladetrail2.Deactivate();
            }
            this.BreakApart(v, isBite);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            FengGameManagerMKII.instance.GameLose();
            this.falseAttack();
            this.hasDied = true;
            Transform transform = this.transform.Find("audio_die");
            transform.parent = null;
            transform.GetComponent<AudioSource>().Play();
            IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(this.transform.position, 0, null, 0.02f);
            Destroy(gameObject);
        }
    }

    public void die2(Transform tf)
    {
        if (this.invincible <= 0f)
        {
            if (this.titanForm && this.eren_titan != null)
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.bulletLeft != null)
            {
                this.bulletLeft.GetComponent<Bullet>().removeMe();
            }
            if (this.bulletRight != null)
            {
                this.bulletRight.GetComponent<Bullet>().removeMe();
            }
            Transform transform = this.transform.Find("audio_die");
            transform.parent = null;
            transform.GetComponent<AudioSource>().Play();
            this.meatDie.Play();
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null, true, false);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            FengGameManagerMKII.instance.GameLose();
            this.falseAttack();
            this.hasDied = true;
            GameObject obj2 = (GameObject) Instantiate(Resources.Load("hitMeat2"));
            obj2.transform.position = this.transform.position;
            Destroy(gameObject);
        }
    }

    private void Dodge(bool offTheWall = false)
    {
        this.State = HeroState.GroundDodge;
        if (!offTheWall)
        {
            float x = 0;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
                x = 1f;
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
                x = -1f;
                
            float y = 0;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                y = -1f;
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                y = 1f;

            if (y != 0f || x != 0f)
                facingDirection = this.getGlobalFacingDirection(y, x) + 180f;
            else
                facingDirection = currentCamera.transform.rotation.eulerAngles.y + 180f;
            this.targetRotation = Quaternion.Euler(0f, facingDirection + 180f, 0f);

            this.crossFade("dodge", 0.1f);
        }
        else
        {
            this.playAnimation("dodge");
            this.playAnimationAt("dodge", 0.2f);
        }
        this.sparks.enableEmission = false;
    }

    private void erenTransform()
    {
        this.skillCDDuration = this.skillCDLast;
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            this.eren_titan = (GameObject) Instantiate(Resources.Load("TITAN_EREN"), transform.position, transform.rotation);
        }
        else
        {
            this.eren_titan = PhotonNetwork.Instantiate("TITAN_EREN", transform.position, transform.rotation, 0);
        }
        this.eren_titan.GetComponent<TITAN_EREN>().realBody = gameObject;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(this.eren_titan, true, false);
        this.eren_titan.GetComponent<TITAN_EREN>().born();
        this.eren_titan.rigidbody.velocity = rigidbody.velocity;
        rigidbody.velocity = Vector3.zero;
        transform.position = this.eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
        this.titanForm = true;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            photonView.RPC(Rpc.SetErenTitanOwner, PhotonTargets.Others, this.eren_titan.GetPhotonView().viewID);
        }
        if (this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
        {
            object[] objArray2 = new object[] { false };
            photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, objArray2);
        }
        this.smoke_3dmg.enableEmission = false;
    }

    public void falseAttack()
    {
        this.attackMove = false;
        if (this.useGun)
        {
            if (!this.attackReleased)
            {
                this.continueAnimation();
                this.attackReleased = true;
            }
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                this.leftbladetrail.StopSmoothly(0.2f);
                this.rightbladetrail.StopSmoothly(0.2f);
                this.leftbladetrail2.StopSmoothly(0.2f);
                this.rightbladetrail2.StopSmoothly(0.2f);
            }
            this.attackLoop = 0;
            if (!this.attackReleased)
            {
                this.continueAnimation();
                this.attackReleased = true;
            }
        }
    }

    public void fillGas()
    {
        this.currentGas = this.totalGas;
    }

    private GameObject findNearestTitan()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("titan");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        Vector3 position = transform.position;
        foreach (GameObject obj3 in objArray)
        {
            Vector3 vector2 = obj3.transform.position - position;
            float sqrMagnitude = vector2.sqrMagnitude;
            if (sqrMagnitude < positiveInfinity)
            {
                obj2 = obj3;
                positiveInfinity = sqrMagnitude;
            }
        }
        return obj2;
    }

    private void FixedUpdate()
    {
        if (this.titanForm || this.isCannon || baseRigidBody == null || IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer) 
            return;
        
        this.currentSpeed = this.baseRigidBody.velocity.magnitude;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            if (!this.baseAnimation.IsPlaying("attack3_2") && !this.baseAnimation.IsPlaying("attack5") && !this.baseAnimation.IsPlaying("special_petra"))
                this.baseRigidBody.rotation = Quaternion.Lerp(gameObject.transform.rotation, this.targetRotation, Time.deltaTime * 6f);
            
            if (this.State == HeroState.Grab)
            {
                this.baseRigidBody.AddForce(-this.baseRigidBody.velocity, ForceMode.VelocityChange);
            }
            else
            {
                if (this.IsGrounded())
                {
                    if (!this.grounded)
                        this.justGrounded = true;
                    
                    this.grounded = true;
                }
                else
                {
                    this.grounded = false;
                }
                
                if (this.hookSomeOne)
                {
                    if (this.hookTarget != null)
                    {
                        Vector3 vector2 = this.hookTarget.transform.position - this.baseTransform.position;
                        float magnitude = vector2.magnitude;
                        if (magnitude > 2f)
                        {
                            this.baseRigidBody.AddForce(vector2.normalized * Mathf.Pow(magnitude, 0.15f) * 30f - this.baseRigidBody.velocity * 0.95f, ForceMode.VelocityChange);
                        }
                    }
                    else
                    {
                        this.hookSomeOne = false;
                    }
                }
                else if (this.hookBySomeOne)
                {
                    if (this.badGuy != null && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing)
                    {
                        Vector3 vector3 = this.badGuy.transform.position - this.baseTransform.position;
                        if (vector3.magnitude > 5f)
                            this.baseRigidBody.AddForce(vector3.normalized * Mathf.Pow(vector3.magnitude, 0.15f) * 0.2f, ForceMode.Impulse);
                    }
                    else
                    {
                        this.hookBySomeOne = false;
                    }
                }
                float z = 0;
                float x = 0;
                if (!IN_GAME_MAIN_CAMERA.isTyping)
                {
                    if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
                        z = 1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
                        z = -1f;

                    if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                        x = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                        x = 1f;
                }
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                this.isLeftHandHooked = false;
                this.isRightHandHooked = false;
                if (this.isLaunchLeft)
                {
                    if (this.bulletLeft != null && this.bulletLeft.GetComponent<Bullet>().isHooked())
                    {
                        this.isLeftHandHooked = true;
                        Vector3 to = this.bulletLeft.transform.position - this.baseTransform.position;
                        to.Normalize();
                        to = to * 10f;
                        if (!this.isLaunchRight)
                        {
                            to = to * 2f;
                        }
                        if (Vector3.Angle(this.baseRigidBody.velocity, to) > 90f && Shelter.InputManager.IsKeyPressed(InputAction.Jump))
                        {
                            flag3 = true;
                            flag2 = true;
                        }
                        if (!flag3)
                        {
                            this.baseRigidBody.AddForce(to);
                            if (Vector3.Angle(this.baseRigidBody.velocity, to) > 90f)
                            {
                                this.baseRigidBody.AddForce(-this.baseRigidBody.velocity * 2f, ForceMode.Acceleration);
                            }
                        }
                    }
                    this.launchElapsedTimeL += Time.deltaTime;
                    if (this.QHold && this.currentGas > 0f)
                    {
                        this.useGas(this.useGasSpeed * Time.deltaTime);
                    }
                    else if (this.launchElapsedTimeL > 0.3f)
                    {
                        this.isLaunchLeft = false;
                        if (this.bulletLeft != null)
                        {
                            this.bulletLeft.GetComponent<Bullet>().disable();
                            this.releaseIfIHookSb();
                            this.bulletLeft = null;
                            flag3 = false;
                        }
                    }
                }
                if (this.isLaunchRight)
                {
                    if (this.bulletRight != null && this.bulletRight.GetComponent<Bullet>().isHooked())
                    {
                        this.isRightHandHooked = true;
                        Vector3 vector5 = this.bulletRight.transform.position - this.baseTransform.position;
                        vector5.Normalize();
                        vector5 = vector5 * 10f;
                        if (!this.isLaunchLeft)
                        {
                            vector5 = vector5 * 2f;
                        }
                        if (Vector3.Angle(this.baseRigidBody.velocity, vector5) > 90f && Shelter.InputManager.IsKeyPressed(InputAction.Jump))
                        {
                            flag4 = true;
                            flag2 = true;
                        }
                        if (!flag4)
                        {
                            this.baseRigidBody.AddForce(vector5);
                            if (Vector3.Angle(this.baseRigidBody.velocity, vector5) > 90f)
                            {
                                this.baseRigidBody.AddForce(-this.baseRigidBody.velocity * 2f, ForceMode.Acceleration);
                            }
                        }
                    }
                    this.launchElapsedTimeR += Time.deltaTime;
                    if (this.EHold && this.currentGas > 0f)
                    {
                        this.useGas(this.useGasSpeed * Time.deltaTime);
                    }
                    else if (this.launchElapsedTimeR > 0.3f)
                    {
                        this.isLaunchRight = false;
                        if (this.bulletRight != null)
                        {
                            this.bulletRight.GetComponent<Bullet>().disable();
                            this.releaseIfIHookSb();
                            this.bulletRight = null;
                            flag4 = false;
                        }
                    }
                }
                if (this.grounded)
                {
                    Vector3 vector7;
                    Vector3 zero = Vector3.zero;
                    if (this.State == HeroState.Attack)
                    {
                        if (this.attackAnimation == "attack5")
                        {
                            if (this.baseAnimation[this.attackAnimation].normalizedTime > 0.4f && this.baseAnimation[this.attackAnimation].normalizedTime < 0.61f)
                            {
                                this.baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                            }
                        }
                        else if (this.attackAnimation == "special_petra")
                        {
                            if (this.baseAnimation[this.attackAnimation].normalizedTime > 0.35f && this.baseAnimation[this.attackAnimation].normalizedTime < 0.48f)
                            {
                                this.baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                            }
                        }
                        else if (this.baseAnimation.IsPlaying("attack3_2"))
                        {
                            zero = Vector3.zero;
                        }
                        else if (this.baseAnimation.IsPlaying("attack1") || this.baseAnimation.IsPlaying("attack2"))
                        {
                            this.baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                        }
                        if (this.baseAnimation.IsPlaying("attack3_2"))
                        {
                            zero = Vector3.zero;
                        }
                    }
                    if (this.justGrounded)
                    {
                        if (this.State != HeroState.Attack || this.attackAnimation != "attack3_1" && this.attackAnimation != "attack5" && this.attackAnimation != "special_petra")
                        {
                            if (this.State != HeroState.Attack && x == 0f && z == 0f && this.bulletLeft == null && this.bulletRight == null && this.State != HeroState.FillGas)
                            {
                                this.State = HeroState.Land;
                                this.crossFade("dash_land", 0.01f);
                            }
                            else
                            {
                                this.buttonAttackRelease = true;
                                if (this.State != HeroState.Attack && this.baseRigidBody.velocity.x * this.baseRigidBody.velocity.x + this.baseRigidBody.velocity.z * this.baseRigidBody.velocity.z > this.speed * this.speed * 1.5f && this.State != HeroState.FillGas)
                                {
                                    this.State = HeroState.Slide;
                                    this.crossFade("slide", 0.05f);
                                    this.facingDirection = Mathf.Atan2(this.baseRigidBody.velocity.x, this.baseRigidBody.velocity.z) * 57.29578f;
                                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                                    this.sparks.enableEmission = true;
                                }
                            }
                        }
                        this.justGrounded = false;
                        zero = this.baseRigidBody.velocity;
                    }
                    if (this.State == HeroState.Attack && this.attackAnimation == "attack3_1" && this.baseAnimation[this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.playAnimation("attack3_2");
                        this.resetAnimationSpeed();
                        vector7 = Vector3.zero;
                        this.baseRigidBody.velocity = vector7;
                        zero = vector7;
                        this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f, 0.95f);
                    }
                    if (this.State == HeroState.GroundDodge)
                    {
                        if (this.baseAnimation["dodge"].normalizedTime >= 0.2f && this.baseAnimation["dodge"].normalizedTime < 0.8f)
                        {
                            zero = -this.baseTransform.forward * 2.4f * this.speed;
                        }
                        if (this.baseAnimation["dodge"].normalizedTime > 0.8f)
                        {
                            zero = this.baseRigidBody.velocity * 0.9f;
                        }
                    }
                    else if (this.State == HeroState.Idle)
                    {
                        Vector3 vector8 = new Vector3(x, 0f, z);
                        float resultAngle = this.getGlobalFacingDirection(x, z);
                        zero = this.getGlobaleFacingVector3(resultAngle);
                        float num6 = vector8.magnitude <= 0.95f ? (vector8.magnitude >= 0.25f ? vector8.magnitude : 0f) : 1f;
                        zero = zero * num6;
                        zero = zero * this.speed;
                        if (_speedupTime > 0f && _isSpeedup)
                            zero = zero * 4f;
                        
                        if (x != 0f || z != 0f)
                        {
                            if (!this.baseAnimation.IsPlaying("run") && !this.baseAnimation.IsPlaying("jump") && !this.baseAnimation.IsPlaying("run_sasha") && (!this.baseAnimation.IsPlaying("horse_geton") || this.baseAnimation["horse_geton"].normalizedTime >= 0.5f))
                            {
                                if (_speedupTime > 0f && _isSpeedup)
                                    this.crossFade("run_sasha", 0.1f);
                                else
                                    this.crossFade("run", 0.1f);
                            }
                        }
                        else
                        {
                            if (!(this.baseAnimation.IsPlaying(this.standAnimation) || this.State == HeroState.Land || this.baseAnimation.IsPlaying("jump") || this.baseAnimation.IsPlaying("horse_geton") || this.baseAnimation.IsPlaying("grabbed")))
                            {
                                this.crossFade(this.standAnimation, 0.1f);
                                zero = zero * 0f;
                            }
                            resultAngle = -874f;
                        }
                        if (resultAngle != -874f)
                        {
                            this.facingDirection = resultAngle;
                            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                        }
                    }
                    else if (this.State == HeroState.Land)
                    {
                        zero = this.baseRigidBody.velocity * 0.96f;
                    }
                    else if (this.State == HeroState.Slide)
                    {
                        zero = this.baseRigidBody.velocity * 0.99f;
                        if (this.currentSpeed < this.speed * 1.2f)
                        {
                            this.idle();
                            this.sparks.enableEmission = false;
                        }
                    }
                    Vector3 velocity = this.baseRigidBody.velocity;
                    Vector3 force = zero - velocity;
                    force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                    force.y = 0f;
                    if (this.baseAnimation.IsPlaying("jump") && this.baseAnimation["jump"].normalizedTime > 0.18f)
                    {
                        force.y += 8f;
                    }
                    if (this.baseAnimation.IsPlaying("horse_geton") && this.baseAnimation["horse_geton"].normalizedTime > 0.18f && this.baseAnimation["horse_geton"].normalizedTime < 1f)
                    {
                        float num7 = 6f;
                        force = -this.baseRigidBody.velocity;
                        force.y = num7;
                        float num8 = Vector3.Distance(this.myHorse.transform.position, this.baseTransform.position);
                        float num9 = 0.6f * this.gravity * num8 / 12f;
                        vector7 = this.myHorse.transform.position - this.baseTransform.position;
                        force += num9 * vector7.normalized;
                    }
                    if (!(this.State == HeroState.Attack && this.useGun))
                    {
                        this.baseRigidBody.AddForce(force, ForceMode.VelocityChange);
                        this.baseRigidBody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                    }
                }
                else
                {
                    if (this.sparks.enableEmission)
                    {
                        this.sparks.enableEmission = false;
                    }
                    if (this.myHorse != null && (this.baseAnimation.IsPlaying("horse_geton") || this.baseAnimation.IsPlaying("air_fall")) && this.baseRigidBody.velocity.y < 0f && Vector3.Distance(this.myHorse.transform.position + Vector3.up * 1.65f, this.baseTransform.position) < 0.5f)
                    {
                        this.baseTransform.position = this.myHorse.transform.position + Vector3.up * 1.65f;
                        this.baseTransform.rotation = this.myHorse.transform.rotation;
                        this.isMounted = true;
                        this.crossFade("horse_idle", 0.1f);
                        this.myHorse.GetComponent<Horse>().mounted();
                    }
                    if (!((this.State != HeroState.Idle || this.baseAnimation.IsPlaying("dash") || this.baseAnimation.IsPlaying("wallrun") || this.baseAnimation.IsPlaying("toRoof") || this.baseAnimation.IsPlaying("horse_geton") || this.baseAnimation.IsPlaying("horse_getoff") || this.baseAnimation.IsPlaying("air_release") || this.isMounted || this.baseAnimation.IsPlaying("air_hook_l_just") && this.baseAnimation["air_hook_l_just"].normalizedTime < 1f || this.baseAnimation.IsPlaying("air_hook_r_just") && this.baseAnimation["air_hook_r_just"].normalizedTime < 1f) && this.baseAnimation["dash"].normalizedTime < 0.99f))
                    {
                        if (!this.isLeftHandHooked && !this.isRightHandHooked && (this.baseAnimation.IsPlaying("air_hook_l") || this.baseAnimation.IsPlaying("air_hook_r") || this.baseAnimation.IsPlaying("air_hook")) && this.baseRigidBody.velocity.y > 20f)
                        {
                            this.baseAnimation.CrossFade("air_release");
                        }
                        else
                        {
                            bool flag5 = Mathf.Abs(this.baseRigidBody.velocity.x) + Mathf.Abs(this.baseRigidBody.velocity.z) > 25f;
                            bool flag6 = this.baseRigidBody.velocity.y < 0f;
                            if (!flag5)
                            {
                                if (flag6)
                                {
                                    if (!this.baseAnimation.IsPlaying("air_fall"))
                                    {
                                        this.crossFade("air_fall", 0.2f);
                                    }
                                }
                                else if (!this.baseAnimation.IsPlaying("air_rise"))
                                {
                                    this.crossFade("air_rise", 0.2f);
                                }
                            }
                            else if (!this.isLeftHandHooked && !this.isRightHandHooked)
                            {
                                float current = -Mathf.Atan2(this.baseRigidBody.velocity.z, this.baseRigidBody.velocity.x) * 57.29578f;
                                float num11 = -Mathf.DeltaAngle(current, this.baseTransform.rotation.eulerAngles.y - 90f);
                                if (Mathf.Abs(num11) < 45f)
                                {
                                    if (!this.baseAnimation.IsPlaying("air2"))
                                    {
                                        this.crossFade("air2", 0.2f);
                                    }
                                }
                                else if (num11 < 135f && num11 > 0f)
                                {
                                    if (!this.baseAnimation.IsPlaying("air2_right"))
                                    {
                                        this.crossFade("air2_right", 0.2f);
                                    }
                                }
                                else if (num11 > -135f && num11 < 0f)
                                {
                                    if (!this.baseAnimation.IsPlaying("air2_left"))
                                    {
                                        this.crossFade("air2_left", 0.2f);
                                    }
                                }
                                else if (!this.baseAnimation.IsPlaying("air2_backward"))
                                {
                                    this.crossFade("air2_backward", 0.2f);
                                }
                            }
                            else if (this.useGun)
                            {
                                if (!this.isRightHandHooked)
                                {
                                    if (!this.baseAnimation.IsPlaying("AHSS_hook_forward_l"))
                                    {
                                        this.crossFade("AHSS_hook_forward_l", 0.1f);
                                    }
                                }
                                else if (!this.isLeftHandHooked)
                                {
                                    if (!this.baseAnimation.IsPlaying("AHSS_hook_forward_r"))
                                    {
                                        this.crossFade("AHSS_hook_forward_r", 0.1f);
                                    }
                                }
                                else if (!this.baseAnimation.IsPlaying("AHSS_hook_forward_both"))
                                {
                                    this.crossFade("AHSS_hook_forward_both", 0.1f);
                                }
                            }
                            else if (!this.isRightHandHooked)
                            {
                                if (!this.baseAnimation.IsPlaying("air_hook_l"))
                                {
                                    this.crossFade("air_hook_l", 0.1f);
                                }
                            }
                            else if (!this.isLeftHandHooked)
                            {
                                if (!this.baseAnimation.IsPlaying("air_hook_r"))
                                {
                                    this.crossFade("air_hook_r", 0.1f);
                                }
                            }
                            else if (!this.baseAnimation.IsPlaying("air_hook"))
                            {
                                this.crossFade("air_hook", 0.1f);
                            }
                        }
                    }
                    if (this.State == HeroState.Idle && this.baseAnimation.IsPlaying("air_release") && this.baseAnimation["air_release"].normalizedTime >= 1f)
                    {
                        this.crossFade("air_rise", 0.2f);
                    }
                    if (this.baseAnimation.IsPlaying("horse_getoff") && this.baseAnimation["horse_getoff"].normalizedTime >= 1f)
                    {
                        this.crossFade("air_rise", 0.2f);
                    }
                    if (this.baseAnimation.IsPlaying("toRoof"))
                    {
                        if (this.baseAnimation["toRoof"].normalizedTime < 0.22f)
                        {
                            this.baseRigidBody.velocity = Vector3.zero;
                            this.baseRigidBody.AddForce(new Vector3(0f, this.gravity * this.baseRigidBody.mass, 0f));
                        }
                        else
                        {
                            if (!this.wallJump)
                            {
                                this.wallJump = true;
                                this.baseRigidBody.AddForce(Vector3.up * 8f, ForceMode.Impulse);
                            }
                            this.baseRigidBody.AddForce(this.baseTransform.forward * 0.05f, ForceMode.Impulse);
                        }
                        if (this.baseAnimation["toRoof"].normalizedTime >= 1f)
                        {
                            this.playAnimation("air_rise");
                        }
                    }
                    else if (!(this.State != HeroState.Idle || !this.isPressDirectionTowardsHero(x, z) || Shelter.InputManager.IsKeyPressed(InputAction.Jump) || Shelter.InputManager.IsKeyPressed(InputAction.LeftHook) || Shelter.InputManager.IsKeyPressed(InputAction.RightHook) || Shelter.InputManager.IsKeyPressed(InputAction.BothHooks) || !this.IsFrontGrounded() || this.baseAnimation.IsPlaying("wallrun") || this.baseAnimation.IsPlaying("dodge")))
                    {
                        this.crossFade("wallrun", 0.1f);
                        this.wallRunTime = 0f;
                    }
                    else if (this.baseAnimation.IsPlaying("wallrun"))
                    {
                        this.baseRigidBody.AddForce(Vector3.up * this.speed - this.baseRigidBody.velocity, ForceMode.VelocityChange);
                        this.wallRunTime += Time.deltaTime;
                        if (this.wallRunTime > 1f || z == 0f && x == 0f)
                        {
                            this.baseRigidBody.AddForce(-this.baseTransform.forward * this.speed * 0.75f, ForceMode.Impulse);
                            this.Dodge(true);
                        }
                        else if (!this.IsUpFrontGrounded())
                        {
                            this.wallJump = false;
                            this.crossFade("toRoof", 0.1f);
                        }
                        else if (!this.IsFrontGrounded())
                        {
                            this.crossFade("air_fall", 0.1f);
                        }
                    }
                    else if (!this.baseAnimation.IsPlaying("attack5") && !this.baseAnimation.IsPlaying("special_petra") && !this.baseAnimation.IsPlaying("dash") && !this.baseAnimation.IsPlaying("jump"))
                    {
                        Vector3 vector11 = new Vector3(x, 0f, z);
                        float num12 = this.getGlobalFacingDirection(x, z);
                        Vector3 vector12 = this.getGlobaleFacingVector3(num12);
                        float num13 = vector11.magnitude <= 0.95f ? (vector11.magnitude >= 0.25f ? vector11.magnitude : 0f) : 1f;
                        vector12 = vector12 * num13;
                        vector12 = vector12 * ((float)this.setup.myCostume.stat.Acceleration / 10f * 2f);
                        if (x == 0f && z == 0f)
                        {
                            if (this.State == HeroState.Attack)
                            {
                                vector12 = vector12 * 0f;
                            }
                            num12 = -874f;
                        }
                        if (num12 != -874f)
                        {
                            this.facingDirection = num12;
                            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                        }
                        if (!flag3 && !flag4 && !this.isMounted && Shelter.InputManager.IsKeyPressed(InputAction.Jump) && this.currentGas > 0f)
                        {
                            if (x != 0f || z != 0f)
                            {
                                this.baseRigidBody.AddForce(vector12, ForceMode.Acceleration);
                            }
                            else
                            {
                                this.baseRigidBody.AddForce(this.baseTransform.forward * vector12.magnitude, ForceMode.Acceleration);
                            }
                            flag2 = true;
                        }
                    }
                    if (this.baseAnimation.IsPlaying("air_fall") && this.currentSpeed < 0.2f && this.IsFrontGrounded())
                    {
                        this.crossFade("onWall", 0.3f);
                    }
                }
                this.spinning = false;
                if (flag3 && flag4)
                {
                    float num14 = this.currentSpeed + 0.1f;
                    this.baseRigidBody.AddForce(-this.baseRigidBody.velocity, ForceMode.VelocityChange);
                    Vector3 vector13 = (this.bulletRight.transform.position + this.bulletLeft.transform.position) * 0.5f - this.baseTransform.position;
                    
                    float num15;
                    if (Shelter.InputManager.IsKeyPressed(InputAction.ReelIn))
                        num15 = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.ReelOut))
                        num15 = 1f;
                    else
                        num15 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    
                    num15 = Mathf.Clamp(num15, -0.8f, 0.8f);
                    float num16 = 1f + num15;
                    Vector3 vector14 = Vector3.RotateTowards(vector13, this.baseRigidBody.velocity, 1.53938f * num16, 1.53938f * num16);
                    vector14.Normalize();
                    this.spinning = true;
                    this.baseRigidBody.velocity = vector14 * num14;
                }
                else if (flag3)
                {
                    float num17 = this.currentSpeed + 0.1f;
                    this.baseRigidBody.AddForce(-this.baseRigidBody.velocity, ForceMode.VelocityChange);
                    Vector3 vector15 = this.bulletLeft.transform.position - this.baseTransform.position;
                    
                    float num18;
                    if (Shelter.InputManager.IsKeyPressed(InputAction.ReelIn))
                        num18 = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.ReelOut))
                        num18 = 1f;
                    else
                        num18 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    
                    num18 = Mathf.Clamp(num18, -0.8f, 0.8f);
                    float num19 = 1f + num18;
                    Vector3 vector16 = Vector3.RotateTowards(vector15, this.baseRigidBody.velocity, 1.53938f * num19, 1.53938f * num19);
                    vector16.Normalize();
                    this.spinning = true;
                    this.baseRigidBody.velocity = vector16 * num17;
                }
                else if (flag4)
                {
                    float num20 = this.currentSpeed + 0.1f;
                    this.baseRigidBody.AddForce(-this.baseRigidBody.velocity, ForceMode.VelocityChange);
                    Vector3 vector17 = this.bulletRight.transform.position - this.baseTransform.position;
                    float num21 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    if (Shelter.InputManager.IsKeyPressed(InputAction.ReelIn))
                        num21 = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.ReelOut))
                        num21 = 1f;
                        
                    num21 = Mathf.Clamp(num21, -0.8f, 0.8f);
                    float num22 = 1f + num21;
                    Vector3 vector18 = Vector3.RotateTowards(vector17, this.baseRigidBody.velocity, 1.53938f * num22, 1.53938f * num22);
                    vector18.Normalize();
                    this.spinning = true;
                    this.baseRigidBody.velocity = vector18 * num20;
                }
                if (this.State == HeroState.Attack && (this.attackAnimation == "attack5" || this.attackAnimation == "special_petra") && this.baseAnimation[this.attackAnimation].normalizedTime > 0.4f && !this.attackMove)
                {
                    this.attackMove = true;
                    if (this.launchPointRight.magnitude > 0f)
                    {
                        Vector3 vector19 = this.launchPointRight - this.baseTransform.position;
                        vector19.Normalize();
                        vector19 = vector19 * 13f;
                        this.baseRigidBody.AddForce(vector19, ForceMode.Impulse);
                    }
                    if (this.attackAnimation == "special_petra" && this.launchPointLeft.magnitude > 0f)
                    {
                        Vector3 vector20 = this.launchPointLeft - this.baseTransform.position;
                        vector20.Normalize();
                        vector20 = vector20 * 13f;
                        this.baseRigidBody.AddForce(vector20, ForceMode.Impulse);
                        if (this.bulletRight != null)
                        {
                            this.bulletRight.GetComponent<Bullet>().disable();
                            this.releaseIfIHookSb();
                        }
                        if (this.bulletLeft != null)
                        {
                            this.bulletLeft.GetComponent<Bullet>().disable();
                            this.releaseIfIHookSb();
                        }
                    }
                    this.baseRigidBody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                }
                bool flag7 = false;
                if (this.bulletLeft != null || this.bulletRight != null)
                {
                    if (this.bulletLeft != null && this.bulletLeft.transform.position.y > gameObject.transform.position.y && this.isLaunchLeft && this.bulletLeft.GetComponent<Bullet>().isHooked())
                    {
                        flag7 = true;
                    }
                    if (this.bulletRight != null && this.bulletRight.transform.position.y > gameObject.transform.position.y && this.isLaunchRight && this.bulletRight.GetComponent<Bullet>().isHooked())
                    {
                        flag7 = true;
                    }
                }
                if (flag7)
                {
                    this.baseRigidBody.AddForce(new Vector3(0f, -10f * this.baseRigidBody.mass, 0f));
                }
                else
                {
                    this.baseRigidBody.AddForce(new Vector3(0f, -this.gravity * this.baseRigidBody.mass, 0f));
                }
                if (this.currentSpeed > 10f)
                {
                    this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, this.currentSpeed + 40f), 0.1f);
                }
                else
                {
                    this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
                }
                if (flag2)
                {
                    this.useGas(this.useGasSpeed * Time.deltaTime);
                    if (!this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
                    {
                        photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, true);
                    }
                    this.smoke_3dmg.enableEmission = true;
                }
                else
                {
                    if (this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
                    {
                        object[] objArray3 = new object[] { false };
                        photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, objArray3);
                    }
                    this.smoke_3dmg.enableEmission = false;
                }
                if (this.currentSpeed > 80f)
                {
                    if (!this.speedFXPS.enableEmission)
                    {
                        this.speedFXPS.enableEmission = true;
                    }
                    this.speedFXPS.startSpeed = this.currentSpeed;
                    this.speedFX.transform.LookAt(this.baseTransform.position + this.baseRigidBody.velocity);
                }
                else if (this.speedFXPS.enableEmission)
                {
                    this.speedFXPS.enableEmission = false;
                }
            }
        }
    }

    public string getDebugInfo()
    {
        string str = "\n";
        str = "Left:" + this.isLeftHandHooked + " ";
        if (this.isLeftHandHooked && this.bulletLeft != null)
        {
            Vector3 vector = this.bulletLeft.transform.position - transform.position;
            str = str + (int) (Mathf.Atan2(vector.x, vector.z) * 57.29578f);
        }
        string str2 = str;
        object[] objArray1 = new object[] { str2, "\nRight:", this.isRightHandHooked, " " };
        str = string.Concat(objArray1);
        if (this.isRightHandHooked && this.bulletRight != null)
        {
            Vector3 vector2 = this.bulletRight.transform.position - transform.position;
            str = str + (int) (Mathf.Atan2(vector2.x, vector2.z) * 57.29578f);
        }
        str = str + "\nfacingDirection:" + (int) this.facingDirection + "\nActual facingDirection:" + (int) transform.rotation.eulerAngles.y + "\nState:" + this.State + "\n\n\n\n\n";
        if (this.State == HeroState.Attack)
        {
            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
        }
        return str;
    }

    private Vector3 getGlobaleFacingVector3(float resultAngle)
    {
        float num = -resultAngle + 90f;
        float x = Mathf.Cos(num * 0.01745329f);
        return new Vector3(x, 0f, Mathf.Sin(num * 0.01745329f));
    }

    private Vector3 getGlobaleFacingVector3(float horizontal, float vertical)
    {
        float num = -this.getGlobalFacingDirection(horizontal, vertical) + 90f;
        float x = Mathf.Cos(num * 0.01745329f);
        return new Vector3(x, 0f, Mathf.Sin(num * 0.01745329f));
    }

    private float getGlobalFacingDirection(float horizontal, float vertical)
    {
        if (vertical == 0f && horizontal == 0f)
        {
            return transform.rotation.eulerAngles.y;
        }
        float y = this.currentCamera.transform.rotation.eulerAngles.y;
        float num2 = Mathf.Atan2(vertical, horizontal) * 57.29578f;
        num2 = -num2 + 90f;
        return y + num2;
    }

    private float getLeanAngle(Vector3 p, bool left)
    {
        if (!this.useGun && this.State == HeroState.Attack)
        {
            return 0f;
        }
        float num = p.y - transform.position.y;
        float num2 = Vector3.Distance(p, transform.position);
        float a = Mathf.Acos(num / num2) * 57.29578f;
        a *= 0.1f;
        a *= 1f + Mathf.Pow(rigidbody.velocity.magnitude, 0.2f);
        Vector3 vector3 = p - transform.position;
        float current = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
        float target = Mathf.Atan2(rigidbody.velocity.x, rigidbody.velocity.z) * 57.29578f;
        float num6 = Mathf.DeltaAngle(current, target);
        a += Mathf.Abs(num6 * 0.5f);
        if (this.State != HeroState.Attack)
        {
            a = Mathf.Min(a, 80f);
        }
        if (num6 > 0f)
        {
            this.leanLeft = true;
        }
        else
        {
            this.leanLeft = false;
        }
        if (this.useGun)
        {
            return a * (num6 >= 0f ? 1 : -1);
        }
        float num7 = 0f;
        if (left && num6 < 0f || !left && num6 > 0f)
        {
            num7 = 0.1f;
        }
        else
        {
            num7 = 0.5f;
        }
        return a * (num6 >= 0f ? num7 : -num7);
    }

    private void getOffHorse()
    {
        this.playAnimation("horse_getoff");
        rigidbody.AddForce(Vector3.up * 10f - transform.forward * 2f - transform.right * 1f, ForceMode.VelocityChange);
        this.unmounted();
    }

    private void getOnHorse()
    {
        this.playAnimation("horse_geton");
        this.facingDirection = this.myHorse.transform.rotation.eulerAngles.y;
        this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
    }

    public void getSupply()
    {
        if ((animation.IsPlaying(this.standAnimation) || animation.IsPlaying("run") || animation.IsPlaying("run_sasha")) && (this.currentBladeSta != this.totalBladeSta || this.currentBladeNum != this.totalBladeNum || this.currentGas != this.totalGas || this.leftBulletLeft != this.bulletMAX || this.rightBulletLeft != this.bulletMAX))
        {
            this.State = HeroState.FillGas;
            this.crossFade("supply", 0.1f);
        }
    }

    public void grabbed(GameObject titan, bool leftHand)
    {
        if (this.isMounted)
        {
            this.unmounted();
        }
        this.State = HeroState.Grab;
        GetComponent<CapsuleCollider>().isTrigger = true;
        this.falseAttack();
        this.titanWhoGrabMe = titan;
        if (this.titanForm && this.eren_titan != null)
        {
            this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (!this.useGun && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.smoke_3dmg.enableEmission = false;
        this.sparks.enableEmission = false;
    }

    public bool HasDied()
    {
        return this.hasDied || this.isInvincible();
    }

    private void headMovement()
    {
        Transform transform = this.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
        Transform transform2 = this.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck");
        float x = Mathf.Sqrt((this.gunTarget.x - this.transform.position.x) * (this.gunTarget.x - this.transform.position.x) + (this.gunTarget.z - this.transform.position.z) * (this.gunTarget.z - this.transform.position.z));
        this.targetHeadRotation = transform.rotation;
        Vector3 vector5 = this.gunTarget - this.transform.position;
        float current = -Mathf.Atan2(vector5.z, vector5.x) * 57.29578f;
        float num3 = -Mathf.DeltaAngle(current, this.transform.rotation.eulerAngles.y - 90f);
        num3 = Mathf.Clamp(num3, -40f, 40f);
        float y = transform2.position.y - this.gunTarget.y;
        float num5 = Mathf.Atan2(y, x) * 57.29578f;
        num5 = Mathf.Clamp(num5, -40f, 30f);
        this.targetHeadRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + num5, transform.rotation.eulerAngles.y + num3, transform.rotation.eulerAngles.z);
        this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 60f);
        transform.rotation = this.oldHeadRotation;
    }

    public void hookedByHuman(int hooker, Vector3 hookPosition)
    {
        photonView.RPC(Rpc.HookedByHuman, photonView.owner, hooker, hookPosition);
    }

    [RPC]
    [UsedImplicitly]
    public void HookFail()
    {
        this.hookTarget = null;
        this.hookSomeOne = false;
    }

    public void hookToHuman(GameObject target, Vector3 hookPosition)
    {
        this.releaseIfIHookSb();
        this.hookTarget = target;
        this.hookSomeOne = true;
        if (target.GetComponent<HERO>() != null)
        {
            target.GetComponent<HERO>().hookedByHuman(photonView.viewID, hookPosition);
        }
        this.launchForce = hookPosition - transform.position;
        float num = Mathf.Pow(this.launchForce.magnitude, 0.1f);
        if (this.grounded)
        {
            rigidbody.AddForce(Vector3.up * Mathf.Min((float)(this.launchForce.magnitude * 0.2f), (float)10f), ForceMode.Impulse);
        }
        rigidbody.AddForce(this.launchForce * num * 0.1f, ForceMode.Impulse);
    }

    private void idle()
    {
        if (this.State == HeroState.Attack)
        {
            this.falseAttack();
        }
        this.State = HeroState.Idle;
        this.crossFade(this.standAnimation, 0.1f);
    }

    private bool IsFrontGrounded()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(gameObject.transform.position + gameObject.transform.up * 1f, gameObject.transform.forward, 1f, mask3.value);
    }

    public bool IsGrounded()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, mask3.value);
    }

    public bool isInvincible()
    {
        return this.invincible > 0f;
    }

    private bool isPressDirectionTowardsHero(float h, float v)
    {
        if (h == 0f && v == 0f)
        {
            return false;
        }
        return Mathf.Abs(Mathf.DeltaAngle(this.getGlobalFacingDirection(h, v), transform.rotation.eulerAngles.y)) < 45f;
    }

    private bool IsUpFrontGrounded()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(gameObject.transform.position + gameObject.transform.up * 3f, gameObject.transform.forward, 1.2f, mask3.value);
    }

    [RPC]
    [UsedImplicitly]
    private void KillObject()
    {
        Destroy(gameObject);
    }

    private void UpdateNameScale()
    {
        if (Player.Self.Properties.Alive == false || Player.Self.Hero == null || photonView.isMine || !ModuleManager.Enabled(nameof(ModuleNameScaling)))
        {
            myNetWorkName.transform.localScale = new Vector3(14, 14, 14);
            return;
        } 

        var dist = Vector3.Distance(Player.Self.Hero.transform.position, transform.position);
        var size = 14 + -8 * Mathf.Clamp01(dist / 300);
        
        myNetWorkName.transform.localScale = new Vector3(size, size, size);
    }

    public void DoLateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && this.myNetWorkName != null)
        {
            UpdateNameScale();
            
            if (this.titanForm && this.eren_titan != null)
                this.myNetWorkName.transform.localPosition = Vector3.up * Screen.height * 1.5f;

            Vector3 start = new Vector3(this.baseTransform.position.x, this.baseTransform.position.y + 2f, this.baseTransform.position.z);
            LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
            LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
            if (Vector3.Angle(maincamera.transform.forward, start - maincamera.transform.position) > 90f || Physics.Linecast(start, maincamera.transform.position, mask2 | mask))
                this.myNetWorkName.transform.localPosition = Vector3.up * Screen.height * 1.5f;
            else
            {
                Vector2 vector2 = this.maincamera.GetComponent<Camera>().WorldToScreenPoint(start);
                this.myNetWorkName.transform.localPosition = new Vector3((int)(vector2.x - Screen.width * 0.5f), (int)(vector2.y - Screen.height * 0.5f), 0f);
            }
        }
        if (!this.titanForm && !this.isCannon)
        {
            if (ModuleManager.Enabled(nameof(ModuleCameraTilt)) && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
            {
                Quaternion quaternion2;
                Vector3 zero = Vector3.zero;
                Vector3 position = Vector3.zero;
                if (this.isLaunchLeft && this.bulletLeft != null && this.bulletLeft.GetComponent<Bullet>().isHooked())
                {
                    zero = this.bulletLeft.transform.position;
                }
                if (this.isLaunchRight && this.bulletRight != null && this.bulletRight.GetComponent<Bullet>().isHooked())
                {
                    position = this.bulletRight.transform.position;
                }
                Vector3 vector5 = Vector3.zero;
                if (zero.magnitude != 0f && position.magnitude == 0f)
                {
                    vector5 = zero;
                }
                else if (zero.magnitude == 0f && position.magnitude != 0f)
                {
                    vector5 = position;
                }
                else if (zero.magnitude != 0f && position.magnitude != 0f)
                {
                    vector5 = (zero + position) * 0.5f;
                }
                Vector3 from = Vector3.Project(vector5 - this.baseTransform.position, this.maincamera.transform.up);
                Vector3 vector7 = Vector3.Project(vector5 - this.baseTransform.position, this.maincamera.transform.right);
                if (vector5.magnitude > 0f)
                {
                    Vector3 to = from + vector7;
                    float num = Vector3.Angle(vector5 - this.baseTransform.position, this.baseRigidBody.velocity) * 0.005f;
                    Vector3 vector9 = this.maincamera.transform.right + vector7.normalized;
                    quaternion2 = Quaternion.Euler(this.maincamera.transform.rotation.eulerAngles.x, this.maincamera.transform.rotation.eulerAngles.y, vector9.magnitude >= 1f ? -Vector3.Angle(@from, to) * num : Vector3.Angle(@from, to) * num);
                }
                else
                {
                    quaternion2 = Quaternion.Euler(this.maincamera.transform.rotation.eulerAngles.x, this.maincamera.transform.rotation.eulerAngles.y, 0f);
                }
                this.maincamera.transform.rotation = Quaternion.Lerp(this.maincamera.transform.rotation, quaternion2, Time.deltaTime * 2f);
            }
            if (this.State == HeroState.Grab && this.titanWhoGrabMe != null)
            {
                if (this.titanWhoGrabMe.GetComponent<TITAN>() != null)
                {
                    this.baseTransform.position = this.titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
                    this.baseTransform.rotation = this.titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
                }
                else if (this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>() != null)
                {
                    this.baseTransform.position = this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
                    this.baseTransform.rotation = this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
                }
            }
            if (this.useGun)
            {
                if (this.leftArmAim || this.rightArmAim)
                {
                    Vector3 vector10 = this.gunTarget - this.baseTransform.position;
                    float current = -Mathf.Atan2(vector10.z, vector10.x) * 57.29578f;
                    float num3 = -Mathf.DeltaAngle(current, this.baseTransform.rotation.eulerAngles.y - 90f);
                    this.headMovement();
                    if (!this.isLeftHandHooked && this.leftArmAim && num3 < 40f && num3 > -90f)
                    {
                        this.leftArmAimTo(this.gunTarget);
                    }
                    if (!this.isRightHandHooked && this.rightArmAim && num3 > -40f && num3 < 90f)
                    {
                        this.rightArmAimTo(this.gunTarget);
                    }
                }
                else if (!this.grounded)
                {
                    this.handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    this.handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                }
                if (this.isLeftHandHooked && this.bulletLeft != null)
                {
                    this.leftArmAimTo(this.bulletLeft.transform.position);
                }
                if (this.isRightHandHooked && this.bulletRight != null)
                {
                    this.rightArmAimTo(this.bulletRight.transform.position);
                }
            }
            this.setHookedPplDirection();
            this.bodyLean();
        }
    }

    public void launch(Vector3 des, bool left = true, bool leviMode = false)
    {
        if (this.isMounted)
        {
            this.unmounted();
        }
        if (this.State != HeroState.Attack)
        {
            this.idle();
        }
        Vector3 vector = des - transform.position;
        if (left)
        {
            this.launchPointLeft = des;
        }
        else
        {
            this.launchPointRight = des;
        }
        vector.Normalize();
        vector = vector * 20f;
        if (this.bulletLeft != null && this.bulletRight != null && this.bulletLeft.GetComponent<Bullet>().isHooked() && this.bulletRight.GetComponent<Bullet>().isHooked())
        {
            vector = vector * 0.8f;
        }
        if (!animation.IsPlaying("attack5") && !animation.IsPlaying("special_petra"))
        {
            leviMode = false;
        }
        else
        {
            leviMode = true;
        }
        if (!leviMode)
        {
            this.falseAttack();
            this.idle();
            if (this.useGun)
            {
                this.crossFade("AHSS_hook_forward_both", 0.1f);
            }
            else if (left && !this.isRightHandHooked)
            {
                this.crossFade("air_hook_l_just", 0.1f);
            }
            else if (!left && !this.isLeftHandHooked)
            {
                this.crossFade("air_hook_r_just", 0.1f);
            }
            else
            {
                this.crossFade("dash", 0.1f);
                animation["dash"].time = 0f;
            }
        }
        if (left)
        {
            this.isLaunchLeft = true;
        }
        if (!left)
        {
            this.isLaunchRight = true;
        }
        this.launchForce = vector;
        if (!leviMode)
        {
            if (vector.y < 30f)
            {
                this.launchForce += Vector3.up * (30f - vector.y);
            }
            if (des.y >= transform.position.y)
            {
                this.launchForce += Vector3.up * (des.y - transform.position.y) * 10f;
            }
            rigidbody.AddForce(this.launchForce);
        }
        this.facingDirection = Mathf.Atan2(this.launchForce.x, this.launchForce.z) * 57.29578f;
        Quaternion quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
        gameObject.transform.rotation = quaternion;
        rigidbody.rotation = quaternion;
        this.targetRotation = quaternion;
        if (left)
        {
            this.launchElapsedTimeL = 0f;
        }
        else
        {
            this.launchElapsedTimeR = 0f;
        }
        if (leviMode)
        {
            this.launchElapsedTimeR = -100f;
        }
        if (animation.IsPlaying("special_petra"))
        {
            this.launchElapsedTimeR = -100f;
            this.launchElapsedTimeL = -100f;
            if (this.bulletRight != null)
            {
                this.bulletRight.GetComponent<Bullet>().disable();
                this.releaseIfIHookSb();
            }
            if (this.bulletLeft != null)
            {
                this.bulletLeft.GetComponent<Bullet>().disable();
                this.releaseIfIHookSb();
            }
        }
        this.sparks.enableEmission = false;
    }

    private void launchLeftRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (this.currentGas != 0f)
        {
            this.useGas(0f);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                this.bulletLeft = (GameObject) Instantiate(Resources.Load("hook"));
            }
            else if (photonView.isMine)
            {
                this.bulletLeft = PhotonNetwork.Instantiate("hook", transform.position, transform.rotation, 0);
            }
            GameObject obj2 = !this.useGun ? this.hookRefL1 : this.hookRefL2;
            string str = !this.useGun ? "hookRefL1" : "hookRefL2";
            this.bulletLeft.transform.position = obj2.transform.position;
            Bullet component = this.bulletLeft.GetComponent<Bullet>();
            float num = !single ? (hit.distance <= 50f ? hit.distance * 0.05f : hit.distance * 0.3f) : 0f;
            Vector3 vector = hit.point - transform.right * num - this.bulletLeft.transform.position;
            vector.Normalize();
            if (mode == 1)
            {
                component.launch(vector * 3f, rigidbody.velocity, str, true, gameObject, true);
            }
            else
            {
                component.launch(vector * 3f, rigidbody.velocity, str, true, gameObject, false);
            }
            this.launchPointLeft = Vector3.zero;
        }
    }

    private void launchRightRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (this.currentGas != 0f)
        {
            this.useGas(0f);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                this.bulletRight = (GameObject) Instantiate(Resources.Load("hook"));
            }
            else if (photonView.isMine)
            {
                this.bulletRight = PhotonNetwork.Instantiate("hook", transform.position, transform.rotation, 0);
            }
            GameObject obj2 = !this.useGun ? this.hookRefR1 : this.hookRefR2;
            string str = !this.useGun ? "hookRefR1" : "hookRefR2";
            this.bulletRight.transform.position = obj2.transform.position;
            Bullet component = this.bulletRight.GetComponent<Bullet>();
            float num = !single ? (hit.distance <= 50f ? hit.distance * 0.05f : hit.distance * 0.3f) : 0f;
            Vector3 vector = hit.point + transform.right * num - this.bulletRight.transform.position;
            vector.Normalize();
            if (mode == 1)
            {
                component.launch(vector * 5f, rigidbody.velocity, str, false, gameObject, true);
            }
            else
            {
                component.launch(vector * 3f, rigidbody.velocity, str, false, gameObject, false);
            }
            this.launchPointRight = Vector3.zero;
        }
    }

    private void leftArmAimTo(Vector3 target)
    {
        float y = target.x - this.upperarmL.transform.position.x;
        float num2 = target.y - this.upperarmL.transform.position.y;
        float x = target.z - this.upperarmL.transform.position.z;
        float num4 = Mathf.Sqrt(y * y + x * x);
        this.handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
        this.forearmL.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        this.upperarmL.rotation = Quaternion.Euler(0f, 90f + Mathf.Atan2(y, x) * 57.29578f, -Mathf.Atan2(num2, num4) * 57.29578f);
    }

    public void loadskin()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            if (!ModuleManager.Enabled(nameof(ModuleWind)))
            {
                var windRenderer = GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.name.Contains("speed"));
                if (windRenderer != null)
                    windRenderer.enabled = false;
            }
            if (ModuleManager.Enabled(nameof(ModuleEnableSkins)))
            {
                var skins = FengGameManagerMKII.settings.HumanSkin.Set;
                StringBuilder url = new StringBuilder(skins.Length * 45);
                foreach (var skin in skins)
                    url.AppendFormat("{0},", skin);
                url.Remove(url.Length - 2, 1);
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    StartCoroutine(this.loadskinE(-1, url.ToString()));
                }
                else
                {
                    int viewID = -1;
                    if (this.myHorse != null)
                    {
                        viewID = this.myHorse.GetPhotonView().viewID;
                    }
                    photonView.RPC(Rpc.LoadSkin, PhotonTargets.AllBuffered, viewID, url.ToString());
                }
            }
        }
    }

    public IEnumerator loadskinE(int horse, string url)
    {
        while (!this.hasspawn)
            yield return null;
        
        bool unloadAssets = false;
        bool mipmap = FengGameManagerMKII.settings.UseMipmap;
        string[] urls = url.Split(',');
        if (urls.Length < 13)
            yield break; // Not allowed exception?
        bool skinGas = ModuleManager.Enabled(nameof(ModuleEnableSkins));
        bool hasHorse = LevelInfoManager.Get(FengGameManagerMKII.Level).Horse || FengGameManagerMKII.settings.EnableHorse;
        bool isMe = IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || this.photonView.isMine;

        Renderer myRenderer;
        if (this.setup.part_hair_1 != null)
        {
            myRenderer = this.setup.part_hair_1.renderer;
            if (Utility.IsValidImageUrl(urls[1]))
            {
                if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[1]))
                {
                    unloadAssets = true;
                    if (this.setup.myCostume.hairInfo.ID >= 0)
                        myRenderer.material = CharacterMaterials.materials[this.setup.myCostume.hairInfo.Texture];
                    
                    using (WWW www = new WWW(urls[1]))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    }
                    FengGameManagerMKII.linkHash[0].Add(urls[1], myRenderer.material);
                    myRenderer.material = (Material)FengGameManagerMKII.linkHash[0][urls[1]];
                }
                else
                {
                    myRenderer.material = (Material)FengGameManagerMKII.linkHash[0][urls[1]];
                }
            }
            else if (urls[1].EqualsIgnoreCase("transparent"))
            {
                myRenderer.enabled = false;
            }
        }
        if (this.setup.part_cape != null)
        {
            myRenderer = this.setup.part_cape.renderer;
            if (Utility.IsValidImageUrl(urls[7]))
            {
                if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[7]))
                {
                    unloadAssets = true;
                    FengGameManagerMKII.linkHash[0].Add(urls[7], myRenderer.material);
                    using (WWW www = new WWW(urls[7]))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    }
                    myRenderer.material = (Material)FengGameManagerMKII.linkHash[0][urls[7]];
                }
                else
                {
                    myRenderer.material = (Material)FengGameManagerMKII.linkHash[0][urls[7]];
                }
            }
            else if (urls[7].EqualsIgnoreCase("transparent"))
            {
                myRenderer.enabled = false;
            }
        }
        if (this.setup.part_chest_3 != null)
        {
            myRenderer = this.setup.part_chest_3.renderer;
            if (Utility.IsValidImageUrl(urls[6]))
            {
                if (!FengGameManagerMKII.linkHash[1].ContainsKey(urls[6]))
                {
                    unloadAssets = true;
                    using (WWW www = new WWW(urls[6]))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                    }

                    FengGameManagerMKII.linkHash[1].Add(urls[6], myRenderer.material);
                    myRenderer.material = (Material) FengGameManagerMKII.linkHash[1][urls[6]];
                }
                else
                {
                    myRenderer.material = (Material) FengGameManagerMKII.linkHash[1][urls[6]];
                }
            }
            else if (urls[6].EqualsIgnoreCase("transparent"))
            {
                myRenderer.enabled = false;
            }
        }

        foreach (Renderer currentRender in this.GetComponentsInChildren<Renderer>())
        {
            if (currentRender.name.Contains("hair"))
            {
                if (Utility.IsValidImageUrl(urls[1]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[1]))
                    {
                        unloadAssets = true;
                        if (this.setup.myCostume.hairInfo.ID >= 0)
                            currentRender.material = CharacterMaterials.materials[this.setup.myCostume.hairInfo.Texture];
                        
                        using (WWW www = new WWW(urls[1]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[1], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[1]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[1]];
                    }
                }
                else if (urls[1].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_eye"))
            {
                if (Utility.IsValidImageUrl(urls[2]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[2]))
                    {
                        unloadAssets = true;
                        currentRender.material.mainTextureScale = currentRender.material.mainTextureScale * 8f;
                        currentRender.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (WWW www = new WWW(urls[2]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[2], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[2]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[2]];
                    }
                }
                else if (urls[2].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("glass"))
            {
                if (Utility.IsValidImageUrl(urls[3]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[3]))
                    {
                        unloadAssets = true;
                        currentRender.material.mainTextureScale = currentRender.material.mainTextureScale * 8f;
                        currentRender.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (WWW www = new WWW(urls[3]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[3], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[3]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[3]];
                    }
                }
                else if (urls[3].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_face"))
            {
                if (Utility.IsValidImageUrl(urls[4]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[4]))
                    {
                        unloadAssets = true;
                        currentRender.material.mainTextureScale = currentRender.material.mainTextureScale * 8f;
                        currentRender.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (WWW www = new WWW(urls[4]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[4], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[4]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[4]];
                    }
                }
                else if (urls[4].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_head") || currentRender.name.Contains("character_hand") || currentRender.name.Contains("character_chest"))
            {
                if (Utility.IsValidImageUrl(urls[5]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[5]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[5]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[5], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[5]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[5]];
                    }
                }
                else if (urls[5].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_body") || currentRender.name.Contains("character_arm") || currentRender.name.Contains("character_leg") || currentRender.name.Contains("mikasa_asset"))
            {
                if (Utility.IsValidImageUrl(urls[6]))
                {
                    if (!FengGameManagerMKII.linkHash[1].ContainsKey(urls[6]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[6]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                        }
                        FengGameManagerMKII.linkHash[1].Add(urls[6], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[1][urls[6]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[1][urls[6]];
                    }
                }
                else if (urls[6].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_cape") || currentRender.name.Contains("character_brand"))
            {
                if (Utility.IsValidImageUrl(urls[7]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[7]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[7]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[7], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[7]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[7]];
                    }
                }
                else if (urls[7].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_blade_l") || (currentRender.name.Contains("character_3dmg") || currentRender.name.Contains("character_gun")) && !currentRender.name.Contains("_r"))
            {
                if (Utility.IsValidImageUrl(urls[8]))
                {
                    if (!FengGameManagerMKII.linkHash[1].ContainsKey(urls[8]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[8]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                        }
                        FengGameManagerMKII.linkHash[1].Add(urls[8], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[1][urls[8]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[1][urls[8]];
                    }
                }
                else if (urls[8].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_blade_r") || currentRender.name.Contains("character_3dmg_gas_r") || currentRender.name.Contains("character_gun") && currentRender.name.Contains("_r")) //TODO: _r tf is this
            {
                if (Utility.IsValidImageUrl(urls[9]))
                {
                    if (!FengGameManagerMKII.linkHash[1].ContainsKey(urls[9]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[9]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                        }
                        FengGameManagerMKII.linkHash[1].Add(urls[9], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[1][urls[9]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[1][urls[9]];
                    }
                }
                else if (urls[9].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name == "3dmg_smoke" && skinGas)
            {
                if (Utility.IsValidImageUrl(urls[10]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[10]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[10]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[10], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[10]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[10]];
                    }
                }
                else if (urls[10].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_cap_"))
            {
                if (Utility.IsValidImageUrl(urls[11]))
                {
                    if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[11]))
                    {
                        unloadAssets = true;
                        using (WWW www = new WWW(urls[11]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        FengGameManagerMKII.linkHash[0].Add(urls[11], currentRender.material);
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[11]];
                    }
                    else
                    {
                        currentRender.material = (Material)FengGameManagerMKII.linkHash[0][urls[11]];
                    }
                }
                else if (urls[11].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
        }
        if (hasHorse && horse >= 0)
        {
            GameObject gameObject = PhotonView.Find(horse).gameObject;
            if (gameObject != null)
            {
                foreach (Renderer currentRenderer in gameObject.GetComponentsInChildren<Renderer>())
                {
                    if (currentRenderer.name.Contains("HORSE"))
                    {
                        if (Utility.IsValidImageUrl(urls[0]))
                        {
                            if (!FengGameManagerMKII.linkHash[1].ContainsKey(urls[0]))
                            {
                                unloadAssets = true;
                                using (WWW www = new WWW(urls[0]))
                                {
                                    yield return www;
                                    if (www.error != null)
                                        yield break;
                                    currentRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                                }
                                FengGameManagerMKII.linkHash[1].Add(urls[0], currentRenderer.material);
                                currentRenderer.material = (Material)FengGameManagerMKII.linkHash[1][urls[0]];
                            }
                            else
                            {
                                currentRenderer.material = (Material)FengGameManagerMKII.linkHash[1][urls[0]];
                            }
                        }
                        else if (urls[0].EqualsIgnoreCase("transparent"))
                        {
                            currentRenderer.enabled = false;
                        }
                    }
                }
            }
        }
        if (isMe && Utility.IsValidImageUrl(urls[12]))
        {
            if (!FengGameManagerMKII.linkHash[0].ContainsKey(urls[12]))
            {
                unloadAssets = true;
                using (WWW www = new WWW(urls[12]))
                {
                    yield return www;
                    if (www.error != null)
                        yield break;
                    leftbladetrail.MyMaterial.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    rightbladetrail.MyMaterial.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                }
                FengGameManagerMKII.linkHash[0].Add(urls[12], this.leftbladetrail.MyMaterial);
                this.leftbladetrail.MyMaterial = (Material)FengGameManagerMKII.linkHash[0][urls[12]];
                this.rightbladetrail.MyMaterial = (Material)FengGameManagerMKII.linkHash[0][urls[12]];
                this.leftbladetrail2.MyMaterial = this.leftbladetrail.MyMaterial;
                this.rightbladetrail2.MyMaterial = this.leftbladetrail.MyMaterial;
            }
            else
            {
                this.leftbladetrail2.MyMaterial = (Material)FengGameManagerMKII.linkHash[0][urls[12]];
                this.rightbladetrail2.MyMaterial = (Material)FengGameManagerMKII.linkHash[0][urls[12]];
                this.leftbladetrail.MyMaterial = (Material)FengGameManagerMKII.linkHash[0][urls[12]];
                this.rightbladetrail.MyMaterial = (Material)FengGameManagerMKII.linkHash[0][urls[12]];
            }
        }
        if (unloadAssets)
            FengGameManagerMKII.instance.UnloadAssets();
    }

    [RPC]
    [UsedImplicitly]
    public void LoadskinRPC(int horse, string url)
    {
        if (ModuleManager.Enabled(nameof(ModuleEnableSkins)))
        {
            StartCoroutine(this.loadskinE(horse, url));
        }
    }

    public void markDie()
    {
        this.hasDied = true;
        this.State = HeroState.Die;
    }

    [RPC]
    [UsedImplicitly]
    public void MoveToRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            transform.position = new Vector3(posX, posY, posZ);
        }
    }

    [RPC]
    [UsedImplicitly]
    private void Net3DMGSMOKE(bool ifON)
    {
        if (this.smoke_3dmg != null)
        {
            this.smoke_3dmg.enableEmission = ifON;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetContinueAnimation()
    {
        foreach (AnimationState current in animation)
        {
            if (current == null)
                continue;
            if (current.speed == 1f)
                return; // Should it be continue; ?
            
            current.speed = 1f;
        }
        this.playAnimation(this.currentPlayingClipName());
    }

    [RPC]
    [UsedImplicitly]
    private void NetCrossFade(string aniName, float time)
    {
        this.currentAnimation = aniName;
        if (animation != null)
            animation.CrossFade(aniName, time);
    }

    [RPC]
    [UsedImplicitly]
    public void NetDie(Vector3 force, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true, PhotonMessageInfo info = null)
    {
        if (photonView.isMine && info != null && IN_GAME_MAIN_CAMERA.GameMode != GameMode.BossFight)
        {
            if (info.sender.IsIgnored)
            {
                photonView.RPC(Rpc.SwitchToHuman, PhotonTargets.Others);
                return;
            }
            if (!info.sender.IsLocal && !info.sender.IsMasterClient)
            {
                if (info.sender.Properties.Name == null || info.sender.Properties.PlayerType == PlayerType.Unknown)
                {
                    Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (viewID < 0)
                {
                    if (titanName == "")
                    {
                        Chat.System("Unusual Kill from ID " + info.sender.ID + " (possibly valid).");
                    }
                    else
                    {
                        Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                    }
                }
                else if (PhotonView.Find(viewID) == null)
                {
                    Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
                {
                    Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
            }
        }
        if (PhotonNetwork.isMasterClient)
        {
            this.onDeathEvent(viewID, killByTitan);
            int id = photonView.owner.ID;
            if (FengGameManagerMKII.heroHash.ContainsKey(id))
            {
                FengGameManagerMKII.heroHash.Remove(id);
            }
        }
        if (photonView.isMine)
        {
            Vector3 vector = Vector3.up * 5000f;
            if (this.myBomb != null)
            {
                this.myBomb.destroyMe();
            }
            if (this.myCannon != null)
            {
                PhotonNetwork.Destroy(this.myCannon);
            }
            if (this.titanForm && this.eren_titan != null)
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.skillCD != null)
            {
                this.skillCD.transform.localPosition = vector;
            }
        }
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        this.meatDie.Play();
        if (!(this.useGun || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine))
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.falseAttack();
        this.BreakApart(force, isBite);
        if (photonView.isMine)
        {
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            FengGameManagerMKII.instance.myRespawnTime = 0f;
        }
        this.hasDied = true;
        Transform audioTransform = this.transform.Find("audio_die");
        if (audioTransform != null)
        {
            audioTransform.parent = null;
            audioTransform.GetComponent<AudioSource>().Play();
        }
        gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        if (photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Dead, true },
                { PlayerProperty.Deaths, Player.Self.Properties.Deaths + 1 }
            });
            FengGameManagerMKII.instance.photonView.RPC(Rpc.OnPlayerDeath, PhotonTargets.MasterClient, titanName != string.Empty ? 1 : 0);
            if (viewID != -1)
            {
                PhotonView view2 = PhotonView.Find(viewID);
                if (view2 != null)
                {
                    FengGameManagerMKII.instance.SendKillInfo(killByTitan, "[FFC000][" + info.sender.ID + "][FFFFFF]" + view2.owner.Properties.Name, false, Player.Self.Properties.Name, 0);
                    view2.owner.SetCustomProperties(new Hashtable
                    {
                        {PlayerProperty.Kills, view2.owner.Properties.Kills + 1}
                    });
                }
            }
            else
            {
                FengGameManagerMKII.instance.SendKillInfo(titanName != string.Empty, "[FFC000][" + info.sender.ID + "][FFFFFF]" + titanName, false, Player.Self.Properties.Name, 0);
            }
        }
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetDie2(int viewID = -1, string titanName = "", PhotonMessageInfo info = null)
    {
        GameObject obj2;
        if (photonView.isMine && info != null && IN_GAME_MAIN_CAMERA.GameMode != GameMode.BossFight)
        {
            if (info.sender.IsIgnored)
            {
                photonView.RPC(Rpc.SwitchToHuman, PhotonTargets.Others, new object[0]);
                return;
            }
            if (!info.sender.IsLocal && !info.sender.IsMasterClient)
            {
                if (info.sender.Properties.Name == null || info.sender.Properties.PlayerType == PlayerType.Unknown)
                {
                    Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (viewID < 0)
                {
                    if (titanName == "")
                    {
                        Chat.System("Unusual Kill from ID " + info.sender.ID + " (possibly valid).");
                    }
                    else if (!FengGameManagerMKII.settings.IsBombMode && FengGameManagerMKII.settings.AllowCannonHumanKills)
                    {
                        Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                    }
                }
                else if (PhotonView.Find(viewID) == null)
                {
                    Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
                {
                    Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
            }
        }
        if (photonView.isMine)
        {
            Vector3 vector = Vector3.up * 5000f;
            if (this.myBomb != null)
            {
                this.myBomb.destroyMe();
            }
            if (this.myCannon != null)
            {
                PhotonNetwork.Destroy(this.myCannon);
            }
            PhotonNetwork.RemoveRPCs(photonView);
            if (this.titanForm && this.eren_titan != null)
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.skillCD != null)
            {
                this.skillCD.transform.localPosition = vector;
            }
        }
        this.meatDie.Play();
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        Transform transform = this.transform.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        if (photonView.isMine)
        {
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null, true, false);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            FengGameManagerMKII.instance.myRespawnTime = 0f;
        }
        this.falseAttack();
        this.hasDied = true;
        gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Dead, true },
                { PlayerProperty.Deaths, Player.Self.Properties.Deaths + 1 }
            });
            if (viewID != -1)
            {
                PhotonView view2 = PhotonView.Find(viewID);
                if (view2 != null)
                {
                    FengGameManagerMKII.instance.SendKillInfo(true, "[FFC000][" + info?.sender.ID + "][FFFFFF]" + view2.owner.Properties.Name, false, Player.Self.Properties.Name, 0);
                    view2.owner.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Kills, view2.owner.Properties.Kills + 1 }
                    });
                }
            }
            else
            {
                FengGameManagerMKII.instance.SendKillInfo(true, "[FFC000][" + info?.sender.ID + "][FFFFFF]" + titanName, false, Player.Self.Properties.Name, 0);
            }
            FengGameManagerMKII.instance.photonView.RPC(Rpc.OnPlayerDeath, PhotonTargets.MasterClient, titanName != string.Empty ? 1 : 0);
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
        {
            obj2 = PhotonNetwork.Instantiate("hitMeat2", this.transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            obj2 = (GameObject) Instantiate(Resources.Load("hitMeat2"));
        }
        obj2.transform.position = this.transform.position;
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
        if (PhotonNetwork.isMasterClient)
        {
            this.onDeathEvent(viewID, true);
            int iD = photonView.owner.ID;
            if (FengGameManagerMKII.heroHash.ContainsKey(iD))
            {
                FengGameManagerMKII.heroHash.Remove(iD);
            }
        }
    }

    public void netDieLocal(Vector3 force, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
    {
        if (photonView.isMine)
        {
            Vector3 vector = Vector3.up * 5000f;
            if (this.titanForm && this.eren_titan != null)
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.myBomb != null)
            {
                this.myBomb.destroyMe();
            }
            if (this.myCannon != null)
            {
                PhotonNetwork.Destroy(this.myCannon);
            }
            if (this.skillCD != null)
            {
                this.skillCD.transform.localPosition = vector;
            }
        }
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        this.meatDie.Play();
        if (!(this.useGun || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine))
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.falseAttack();
        this.BreakApart(force, isBite);
        if (photonView.isMine)
        {
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            FengGameManagerMKII.instance.myRespawnTime = 0f;
        }
        this.hasDied = true;
        Transform transform = this.transform.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        if (photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Dead, true },
                { PlayerProperty.Deaths, Player.Self.Properties.Deaths + 1 }
            });
            FengGameManagerMKII.instance.photonView.RPC(Rpc.OnPlayerDeath, PhotonTargets.MasterClient, titanName != string.Empty ? 1 : 0);
            if (viewID != -1)
            {
                PhotonView view = PhotonView.Find(viewID);
                if (view != null)
                {
                    FengGameManagerMKII.instance.SendKillInfo(killByTitan, view.owner.Properties.Name, false, Player.Self.Properties.Name, 0);
                    view.owner.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Kills, view.owner.Properties.Kills + 1 }
                    });
                }
            }
            else
            {
                FengGameManagerMKII.instance.SendKillInfo(titanName != string.Empty, titanName, false, Player.Self.Properties.Name, 0);
            }
        }
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
        if (PhotonNetwork.isMasterClient)
        {
            this.onDeathEvent(viewID, killByTitan);
            int iD = photonView.owner.ID;
            if (FengGameManagerMKII.heroHash.ContainsKey(iD))
            {
                FengGameManagerMKII.heroHash.Remove(iD);
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetGrabbed(int id, bool leftHand)
    {
        this.titanWhoGrabMeID = id;
        this.grabbed(PhotonView.Find(id).gameObject, leftHand);
    }

    [RPC]
    [UsedImplicitly]
    private void NetlaughAttack()
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (Vector3.Distance(obj2.transform.position, transform.position) < 50f && Vector3.Angle(obj2.transform.forward, transform.position - obj2.transform.position) < 90f && obj2.GetComponent<TITAN>() != null)
            {
                obj2.GetComponent<TITAN>().StartLaughing();
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetPauseAnimation()
    {
        foreach (AnimationState current in animation)
            current.speed = 0f;
    }

    [RPC]
    [UsedImplicitly]
    private void NetPlayAnimation(string aniName)
    {
        this.currentAnimation = aniName;
        if (animation != null)
        {
            animation.Play(aniName);
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetPlayAnimationAt(string aniName, float normalizedTime)
    {
        this.currentAnimation = aniName;
        if (animation != null)
        {
            animation.Play(aniName);
            animation[aniName].normalizedTime = normalizedTime;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetSetIsGrabbedFalse()
    {
        this.State = HeroState.Idle;
    }

    [RPC]
    [UsedImplicitly]
    private void NetTauntAttack(float tauntTime, float distance = 100f)
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (Vector3.Distance(obj2.transform.position, transform.position) < distance && obj2.GetComponent<TITAN>() != null)
            {
                obj2.GetComponent<TITAN>().GetTaunted(gameObject, tauntTime);
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetUngrabbed()
    {
        this.ungrabbed();
        this.NetPlayAnimation(this.standAnimation);
        this.falseAttack();
    }

    public void onDeathEvent(int viewID, bool isTitan)
    {
        RCEvent event2;
        string[] strArray;
        if (isTitan)
        {
            if (FengGameManagerMKII.RCEvents.ContainsKey("OnPlayerDieByTitan"))
            {
                event2 = (RCEvent) FengGameManagerMKII.RCEvents["OnPlayerDieByTitan"];
                strArray = (string[]) FengGameManagerMKII.RCVariableNames["OnPlayerDieByTitan"];
                if (FengGameManagerMKII.playerVariables.ContainsKey(strArray[0]))
                {
                    FengGameManagerMKII.playerVariables[strArray[0]] = photonView.owner;
                }
                else
                {
                    FengGameManagerMKII.playerVariables.Add(strArray[0], photonView.owner);
                }
                if (FengGameManagerMKII.titanVariables.ContainsKey(strArray[1]))
                {
                    FengGameManagerMKII.titanVariables[strArray[1]] = PhotonView.Find(viewID).gameObject.GetComponent<TITAN>();
                }
                else
                {
                    FengGameManagerMKII.titanVariables.Add(strArray[1], PhotonView.Find(viewID).gameObject.GetComponent<TITAN>());
                }
                event2.checkEvent();
            }
        }
        else if (FengGameManagerMKII.RCEvents.ContainsKey("OnPlayerDieByPlayer"))
        {
            event2 = (RCEvent) FengGameManagerMKII.RCEvents["OnPlayerDieByPlayer"];
            strArray = (string[]) FengGameManagerMKII.RCVariableNames["OnPlayerDieByPlayer"];
            if (FengGameManagerMKII.playerVariables.ContainsKey(strArray[0]))
            {
                FengGameManagerMKII.playerVariables[strArray[0]] = photonView.owner;
            }
            else
            {
                FengGameManagerMKII.playerVariables.Add(strArray[0], photonView.owner);
            }
            if (FengGameManagerMKII.playerVariables.ContainsKey(strArray[1]))
            {
                FengGameManagerMKII.playerVariables[strArray[1]] = PhotonView.Find(viewID).owner;
            }
            else
            {
                FengGameManagerMKII.playerVariables.Add(strArray[1], PhotonView.Find(viewID).owner);
            }
            event2.checkEvent();
        }
    }

    private void OnDestroy()
    {
        if (this.myNetWorkName != null)
        {
            Destroy(this.myNetWorkName);
        }
        if (this.gunDummy != null)
        {
            Destroy(this.gunDummy);
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            this.releaseIfIHookSb();
        }
        if (GameObject.Find("MultiplayerManager") != null)
        {
            FengGameManagerMKII.instance.Heroes.Remove(this);
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            Vector3 vector = Vector3.up * 5000f;
            this.cross1.transform.localPosition = vector;
            this.cross2.transform.localPosition = vector;
            this.LabelDistance.transform.localPosition = vector;
        }
        if (this.setup.part_cape != null)
        {
            ClothFactory.DisposeObject(this.setup.part_cape);
        }
        if (this.setup.part_hair_1 != null)
        {
            ClothFactory.DisposeObject(this.setup.part_hair_1);
        }
        if (this.setup.part_hair_2 != null)
        {
            ClothFactory.DisposeObject(this.setup.part_hair_2);
        }
    }

    public void pauseAnimation()
    {
        foreach (AnimationState current in animation)
            if (current != null)
                current.speed = 0f;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
        {
            photonView.RPC(Rpc.PauseAnimation, PhotonTargets.Others, new object[0]);
        }
    }

    public void playAnimation(string aniName)
    {
        this.currentAnimation = aniName;
        animation.Play(aniName);
        if (PhotonNetwork.connected && photonView.isMine)
        {
            photonView.RPC(Rpc.PlayAnimation, PhotonTargets.Others, aniName);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        this.currentAnimation = aniName;
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
        if (PhotonNetwork.connected && photonView.isMine)
        {
            photonView.RPC(Rpc.PlayAnimationAt, PhotonTargets.Others, aniName, normalizedTime);
        }
    }

    private void releaseIfIHookSb()
    {
        if (this.hookSomeOne && this.hookTarget != null)
        {
            this.hookTarget.GetPhotonView().RPC(Rpc.ReleasePlayerHook, this.hookTarget.GetPhotonView().owner, new object[0]);
            this.hookTarget = null;
            this.hookSomeOne = false;
        }
    }

    public IEnumerator reloadSky()
    {
        yield return new WaitForSeconds(0.5f);
        if (FengGameManagerMKII.skyMaterial != null && Camera.main.GetComponent<Skybox>().material != FengGameManagerMKII.skyMaterial)
        {
            Camera.main.GetComponent<Skybox>().material = FengGameManagerMKII.skyMaterial;
        }
    }

    public void resetAnimationSpeed()
    {
        foreach (AnimationState current in animation)
            if (current != null)
                current.speed = 1f;
        this.customAnimationSpeed();
    }

    [RPC]
    [UsedImplicitly]
    public void ReturnFromCannon(PhotonMessageInfo info)
    {
        if (info.sender == photonView.owner)
        {
            this.isCannon = false;
            gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
        }
    }

    private void rightArmAimTo(Vector3 target)
    {
        float y = target.x - this.upperarmR.transform.position.x;
        float num2 = target.y - this.upperarmR.transform.position.y;
        float x = target.z - this.upperarmR.transform.position.z;
        float num4 = Mathf.Sqrt(y * y + x * x);
        this.handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        this.forearmR.localRotation = Quaternion.Euler(90f, 0f, 0f);
        this.upperarmR.rotation = Quaternion.Euler(180f, 90f + Mathf.Atan2(y, x) * 57.29578f, Mathf.Atan2(num2, num4) * 57.29578f);
    }

    [RPC]
    [UsedImplicitly]
    private void RPCHookedByHuman(int hooker, Vector3 hookPosition, PhotonMessageInfo info)
    {
        if (!PhotonView.TryParse(hooker, out PhotonView view))
            throw new NotAllowedException(nameof(RPCHookedByHuman), info, false);
        
        if (Vector3.Distance(hookPosition, transform.position) < 15f && IN_GAME_MAIN_CAMERA.GameMode == GameMode.Racing)
        {
            this.badGuy = view.gameObject;
            this.hookBySomeOne = true;
            
            this.launchForce = PhotonView.Find(hooker).gameObject.transform.position - transform.position;
            rigidbody.AddForce(-rigidbody.velocity * 0.9f, ForceMode.VelocityChange);
            float num = Mathf.Pow(this.launchForce.magnitude, 0.1f);
            if (this.grounded)
                rigidbody.AddForce(Vector3.up * Mathf.Min(this.launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
            
            rigidbody.AddForce(this.launchForce * num * 0.1f, ForceMode.Impulse);
            if (this.State != HeroState.Grab)
            {
                this.dashTime = 1f;
                this.crossFade("dash", 0.05f);
                animation["dash"].time = 0.1f;
                this.State = HeroState.AirDodge;
                this.falseAttack();
                this.facingDirection = Mathf.Atan2(this.launchForce.x, this.launchForce.z) * 57.29578f;
                Quaternion quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
                gameObject.transform.rotation = quaternion;
                rigidbody.rotation = quaternion;
                this.targetRotation = quaternion;
            }
        }
        else
        {
            this.hookBySomeOne = false;
            this.badGuy = null;
            view.RPC(Rpc.HookFailed, view.owner);
        }
    }

    private void salute()
    {
        this.State = HeroState.Salute;
        this.crossFade("salute", 0.1f);
    }

    private void setHookedPplDirection()
    {
        this.almostSingleHook = false;
        if (this.isRightHandHooked && this.isLeftHandHooked)
        {
            if (this.bulletLeft != null && this.bulletRight != null)
            {
                Vector3 normal = this.bulletLeft.transform.position - this.bulletRight.transform.position;
                if (normal.sqrMagnitude < 4f)
                {
                    Vector3 vector2 = (this.bulletLeft.transform.position + this.bulletRight.transform.position) * 0.5f - transform.position;
                    this.facingDirection = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
                    if (this.useGun && this.State != HeroState.Attack)
                    {
                        float current = -Mathf.Atan2(rigidbody.velocity.z, rigidbody.velocity.x) * 57.29578f;
                        float target = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        float num3 = -Mathf.DeltaAngle(current, target);
                        this.facingDirection += num3;
                    }
                    this.almostSingleHook = true;
                }
                else
                {
                    Vector3 to = transform.position - this.bulletLeft.transform.position;
                    Vector3 vector6 = transform.position - this.bulletRight.transform.position;
                    Vector3 vector7 = (this.bulletLeft.transform.position + this.bulletRight.transform.position) * 0.5f;
                    Vector3 from = transform.position - vector7;
                    if (Vector3.Angle(@from, to) < 30f && Vector3.Angle(@from, vector6) < 30f)
                    {
                        this.almostSingleHook = true;
                        Vector3 vector9 = vector7 - transform.position;
                        this.facingDirection = Mathf.Atan2(vector9.x, vector9.z) * 57.29578f;
                    }
                    else
                    {
                        this.almostSingleHook = false;
                        Vector3 forward = transform.forward;
                        Vector3.OrthoNormalize(ref normal, ref forward);
                        this.facingDirection = Mathf.Atan2(forward.x, forward.z) * 57.29578f;
                        float num4 = Mathf.Atan2(to.x, to.z) * 57.29578f;
                        if (Mathf.DeltaAngle(num4, this.facingDirection) > 0f)
                        {
                            this.facingDirection += 180f;
                        }
                    }
                }
            }
        }
        else
        {
            this.almostSingleHook = true;
            Vector3 zero = Vector3.zero;
            if (this.isRightHandHooked && this.bulletRight != null)
            {
                zero = this.bulletRight.transform.position - transform.position;
            }
            else
            {
                if (!this.isLeftHandHooked || this.bulletLeft == null)
                {
                    return;
                }
                zero = this.bulletLeft.transform.position - transform.position;
            }
            this.facingDirection = Mathf.Atan2(zero.x, zero.z) * 57.29578f;
            if (this.State != HeroState.Attack)
            {
                float num6 = -Mathf.Atan2(rigidbody.velocity.z, rigidbody.velocity.x) * 57.29578f;
                float num7 = -Mathf.Atan2(zero.z, zero.x) * 57.29578f;
                float num8 = -Mathf.DeltaAngle(num6, num7);
                if (this.useGun)
                {
                    this.facingDirection += num8;
                }
                else
                {
                    float num9 = 0f;
                    if (this.isLeftHandHooked && num8 < 0f || this.isRightHandHooked && num8 > 0f)
                    {
                        num9 = -0.1f;
                    }
                    else
                    {
                        num9 = 0.1f;
                    }
                    this.facingDirection += num8 * num9;
                }
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void SetMyCannon(int viewID, PhotonMessageInfo info)
    {
        if (info.sender == photonView.owner)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view != null)
            {
                this.myCannon = view.gameObject;
                if (this.myCannon != null)
                {
                    this.myCannonBase = this.myCannon.transform;
                    this.myCannonPlayer = this.myCannonBase.Find("PlayerPoint");
                    this.isCannon = true;
                }
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void SetMyPhotonCamera(float offset, PhotonMessageInfo info)
    {
        if (photonView.owner == info.sender)
        {
            this.CameraMultiplier = offset;
            GetComponent<SmoothSyncMovement>().PhotonCamera = true;
            this.isPhotonCamera = true;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void SetMyTeam(int val)
    {
        this.myTeam = val;
        TriggerColliderWeapon colliderWeapon;
        if (checkBoxLeft != null && (colliderWeapon = checkBoxLeft.GetComponent<TriggerColliderWeapon>()) != null)
            colliderWeapon.myTeam = val;
        if (checkBoxRight != null && (colliderWeapon = checkBoxRight.GetComponent<TriggerColliderWeapon>()) != null)
            colliderWeapon.myTeam = val;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            if (FengGameManagerMKII.settings.IsFriendlyMode)
            {
                if (val != 1)
                    photonView.RPC(Rpc.SetTeam, PhotonTargets.AllBuffered, 1);
            }
            else switch (FengGameManagerMKII.settings.PVPMode)
            {
                case PVPMode.Teams:
                    if (photonView.owner.Properties.RCTeam.HasValue && val != photonView.owner.Properties.RCTeam)
                        photonView.RPC(Rpc.SetTeam, PhotonTargets.AllBuffered, photonView.owner.Properties.RCTeam);
                    break;
                
                case PVPMode.FFA when val != photonView.owner.ID:
                    photonView.RPC(Rpc.SetTeam, PhotonTargets.AllBuffered, photonView.owner.ID);
                    break;
            }
        }
    }

    public void SetSkillHUDPosition()
    {
        this.skillCD = GameObject.Find("skill_cd_" + this.skillIDHUD);
        if (this.skillCD != null)
        {
            this.skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
            if (this.useGun && !FengGameManagerMKII.settings.IsBombMode)
                this.skillCD.transform.localPosition = Vector3.up * 5000f;
        }
    }

    private void SetRacingUI()
    {
        switch (IN_GAME_MAIN_CAMERA.GameMode)
        {
            case GameMode.Racing when ModuleManager.Enabled(nameof(ModuleRacingInterface)):
                GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0, 4000, 0);
                skillCD = GameObject.Find("skill_cd_" + this.skillIDHUD);
                
                GameObject.Find("GasUI").transform.localPosition = new Vector3(0, 4000, 0);
                GameObject.Find("gasR").transform.position = new Vector3(Screen.width / 2f - 50, -Screen.height / 2f + 50, 0);
                GameObject.Find("gasR1").transform.position = new Vector3(Screen.width / 2f - 50, -Screen.height / 2f + 50, 0);
                break;
            
            default:
                GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, -Screen.height * 0.5f + 10f, 0f);
                this.skillCD = GameObject.Find("skill_cd_" + this.skillIDHUD);
                this.skillCD.transform.localPosition = new Vector3(0f, -Screen.height * 0.5f + 10f, 0f);
                GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                break;
        }
    }

    public void setStat2()
    {
        this.skillCDLast = 1.5f;
        this.customAnimationSpeed();
        this.skillId = this.setup.myCostume.stat.SkillName;
        switch (this.skillId)
        {
            case "levi":
                this.skillCDLast = 3.5f;
                break;
            case "armin":
                this.skillCDLast = 5f;
                break;
            case "marco":
                this.skillCDLast = 10f;
                break;
            case "jean":
                this.skillCDLast = 0.001f;
                break;
            case "eren":
                this.skillCDLast = 240;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    var level = LevelInfoManager.Get(FengGameManagerMKII.Level);
                    if (level.PlayerTitansNotAllowed || 
                        level.Gamemode == GameMode.Racing || 
                        level.Gamemode == GameMode.PvpCapture || 
                        level.Gamemode == GameMode.Trost)
                    {
                        this.skillId = "petra";
                        this.skillCDLast = 1f;
                    }
                }

                break;
            case "sasha":
                this.skillCDLast = 20f;
                break;
            case "petra":
                this.skillCDLast = 3.5f;
                break;
        }

        this.bombInit();
        this.speed = this.setup.myCostume.stat.Speed / 10f;
        this.totalGas = this.currentGas = this.setup.myCostume.stat.Gas;
        this.totalBladeSta = this.currentBladeSta = this.setup.myCostume.stat.Blade;
        this.baseRigidBody.mass = 0.5f - (this.setup.myCostume.stat.Acceleration - 100) * 0.001f;
        
        SetRacingUI();
        
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            GameObject.Find("bulletL").GetComponent<UISprite>().enabled = false;
            GameObject.Find("bulletR").GetComponent<UISprite>().enabled = false;
            for (int i = 1; i <= 7; i++)
            {
                GameObject.Find($"bulletL{i}").GetComponent<UISprite>().enabled = false;
                GameObject.Find($"bulletR{i}").GetComponent<UISprite>().enabled = false;
            }
        }
        if (this.setup.myCostume.uniform_type == UniformType.CasualAHSS)
        {
            this.standAnimation = "AHSS_stand_gun";
            this.useGun = true;
            this.gunDummy = new GameObject("gunDummy");
            this.gunDummy.transform.position = this.baseTransform.position;
            this.gunDummy.transform.rotation = this.baseTransform.rotation;
            this.myGroup = Group.AHSS;
            this.setTeam2(2);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                GameObject.Find("bladeCL").GetComponent<UISprite>().enabled = false;
                GameObject.Find("bladeCR").GetComponent<UISprite>().enabled = false;
                for (int i = 1; i <= 5; i++)
                {
                    GameObject.Find($"bladel{i}").GetComponent<UISprite>().enabled = false;
                    GameObject.Find($"blader{i}").GetComponent<UISprite>().enabled = false;
                }
                
                GameObject.Find("bulletL").GetComponent<UISprite>().enabled = true;
                GameObject.Find("bulletR").GetComponent<UISprite>().enabled = true;
                for (int i = 1; i <= 7; i++)
                {
                    GameObject.Find($"bulletL{i}").GetComponent<UISprite>().enabled = true;
                    GameObject.Find($"bulletR{i}").GetComponent<UISprite>().enabled = true;
                }
                if (this.skillId != "bomb")
                {
                    this.skillCD.transform.localPosition = Vector3.up * 5000f;
                }
            }
        }
        else if (this.setup.myCostume.sex == Sex.Female)
        {
            this.standAnimation = "stand";
            this.setTeam2(1);
        }
        else
        {
            this.standAnimation = "stand_levi";
            this.setTeam2(1);
        }
    }

    public void setTeam(int team)
    {
        this.SetMyTeam(team);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            photonView.RPC(Rpc.SetTeam, PhotonTargets.OthersBuffered, team);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Team, team }
            });
        }
    }

    public void setTeam2(int team)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            photonView.RPC(Rpc.SetTeam, PhotonTargets.AllBuffered, team);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Team, team }
            });
        }
        else
        {
            this.SetMyTeam(team);
        }
    }

    public void shootFlare(int type)
    {
        switch (type)
        {
            case 1 when this.flare1CD == 0f:
                this.flare1CD = this.flareTotalCD;
                break;
            case 2 when this.flare2CD == 0f:
                this.flare2CD = this.flareTotalCD;
                break;
            case 3 when this.flare3CD == 0f:
                this.flare3CD = this.flareTotalCD;
                break;
            default: 
                return;
        }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            GameObject obj2 = (GameObject) Instantiate(Resources.Load("FX/flareBullet" + type), transform.position, transform.rotation);
            obj2.GetComponent<FlareMovement>().dontShowHint();
            Destroy(obj2, 25f);
        }
        else
        {
            PhotonNetwork.Instantiate("FX/flareBullet" + type, transform.position, transform.rotation, 0).GetComponent<FlareMovement>().dontShowHint();
        }
    }

    private void DrawReticle() // TODO: Add custom cursor
    {
        if (Screen.showCursor) //TODO: Destroy objects instead of moving them offscreen
        {
            Vector3 vector = Vector3.up * 10000f;
            cross1        .transform.localPosition = vector;
            cross2        .transform.localPosition = vector;
            LabelDistance .transform.localPosition = vector;
        }
        else
        {
            this.checkTitan();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = (1 << LayerMask.NameToLayer("EnemyBox")) | (1 << LayerMask.NameToLayer("Ground"));
            if (Physics.Raycast(ray, out var hit, 1E+07f, mask.value))
            {
                cross1.transform.localPosition =
                    cross2.transform.localPosition = 
                        Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                float magnitude = (hit.point - baseTransform.position).magnitude;
                LabelDistance.GetComponent<UILabel>().text = string.Format(ModuleCursorLabel.LabelFormat,
                    magnitude, currentSpeed, currentSpeed / 100);
                if (magnitude > 120f)
                {
                    cross1.transform.localPosition += Vector3.up * 10000f;
                    LabelDistance.transform.localPosition = cross2.transform.localPosition;
                }
                else
                {
                    cross2.transform.localPosition += Vector3.up * 10000f;
                    LabelDistance.transform.localPosition = cross1.transform.localPosition;
                }
                LabelDistance.transform.localPosition -= new Vector3(0f, 15f, 0f);
            }
        }
    }

    private void ShowFlareCD()
    {
//        if (this.cachedSprites["UIflare1"] != null)
//        {
//            this.cachedSprites["UIflare1"].fillAmount = (this.flareTotalCD - this.flare1CD) / this.flareTotalCD;
//            this.cachedSprites["UIflare2"].fillAmount = (this.flareTotalCD - this.flare2CD) / this.flareTotalCD;
//            this.cachedSprites["UIflare3"].fillAmount = (this.flareTotalCD - this.flare3CD) / this.flareTotalCD;
//        }
    }
    
    private void UpdateUsageUI()
    {
        float gasUsage = currentGas / totalGas;
        
        Color color;
        if (gasUsage <= .30f)
            color = Color.red;
        else if (gasUsage < .50f)
            color = Color.yellow;
        else
            color = Color.white;

        if (cachedSprites.ContainsKey("gasL1")) // assuming that if key is contained value non-null
        {
            cachedSprites["gasL1"].fillAmount = gasUsage;
            cachedSprites["gasL1"].color = color;
        }
        if (cachedSprites.ContainsKey("gasR1"))
        {
            cachedSprites["gasR1"].fillAmount = gasUsage;
            cachedSprites["gasR1"].color = color;
        }
            
        if (useGun)
        {
            if (cachedSprites.ContainsKey("bulletL"))
                cachedSprites["bulletL"].enabled = leftGunHasBullet;
            if (cachedSprites.ContainsKey("bulletR"))
                cachedSprites["bulletR"].enabled = leftGunHasBullet;
            return;
        }

        float bladeUsage = currentBladeSta / totalBladeSta;

        if (bladeUsage <= .10f)
            color = Color.red;
        else if (bladeUsage < .35f)
            color = Color.yellow;
        else
            color = Color.white;

        if (cachedSprites.ContainsKey("bladeCL"))
            cachedSprites["bladeCL"].fillAmount = bladeUsage;
        if (cachedSprites.ContainsKey("bladeCR"))
            cachedSprites["bladeCR"].fillAmount = bladeUsage;
        
        if (cachedSprites.ContainsKey("bladel1"))
            cachedSprites["bladel1"].color = color;
        if (cachedSprites.ContainsKey("blader1"))
            cachedSprites["blader1"].color = color;

        for (int i = 0; i < 5; i++)
        {
            if (cachedSprites.ContainsKey("bladel" + (i + 1)))
                cachedSprites["bladel" + (i + 1)].enabled = currentBladeNum > i;
            if (cachedSprites.ContainsKey("blader" + (i + 1)))
                cachedSprites["blader" + (i + 1)].enabled = currentBladeNum > i;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void ShowHitDamage()
    {
        GameObject target = GameObject.Find("LabelScore");
        if (target != null)
        {
            this.speed = Mathf.Max(10f, this.speed);
            target.GetComponent<UILabel>().text = this.speed.ToString();
            target.transform.localScale = Vector3.zero;
            this.speed = (int) (this.speed * 0.1f);
            this.speed = Mathf.Clamp(this.speed, 40f, 150f);
            iTween.Stop(target);
            object[] args = new object[] { "x", this.speed, "y", this.speed, "z", this.speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f };
            iTween.ScaleTo(target, iTween.Hash(args));
            object[] objArray2 = new object[] { "x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f };
            iTween.ScaleTo(target, iTween.Hash(objArray2));
        }
    }

    private void showSkillCD()
    {
        if (this.skillCD != null)
        {
            this.skillCD.GetComponent<UISprite>().fillAmount = (this.skillCDLast - this.skillCDDuration) / this.skillCDLast;
        }
    }

    [RPC]
    [UsedImplicitly]
    public void SpawnCannonRPC(string settings, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient && photonView.isMine && this.myCannon == null)
        {
            if (this.myHorse != null && this.isMounted)
            {
                this.getOffHorse();
            }
            this.idle();
            if (this.bulletLeft != null)
            {
                this.bulletLeft.GetComponent<Bullet>().removeMe();
            }
            if (this.bulletRight != null)
            {
                this.bulletRight.GetComponent<Bullet>().removeMe();
            }
            if (this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
            {
                photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, false);
            }
            this.smoke_3dmg.enableEmission = false;
            rigidbody.velocity = Vector3.zero;
            string[] strArray = settings.Split(new char[] { ',' });
            if (strArray.Length > 15)
            {
                this.myCannon = PhotonNetwork.Instantiate("RCAsset/" + strArray[1], new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[16]), Convert.ToSingle(strArray[17]), Convert.ToSingle(strArray[18])), 0);
            }
            else
            {
                this.myCannon = PhotonNetwork.Instantiate("RCAsset/" + strArray[1], new Vector3(Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4])), new Quaternion(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8])), 0);
            }
            this.myCannonBase = this.myCannon.transform;
            this.myCannonPlayer = this.myCannon.transform.Find("PlayerPoint");
            this.isCannon = true;
            this.myCannon.GetComponent<Cannon>().myHero = this;
            this.myCannonRegion = null;
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(this.myCannon.transform.Find("Barrel").Find("FiringPoint").gameObject, true, false);
            Camera.main.fieldOfView = 55f;
            photonView.RPC(Rpc.SetMyCannon, PhotonTargets.OthersBuffered, new object[] { this.myCannon.GetPhotonView().viewID });
            this.skillCDLastCannon = this.skillCDLast;
            this.skillCDLast = 3.5f;
            this.skillCDDuration = 3.5f;
        }
    }
    
    private void Start()
    {
        var player = photonView.owner;
        if (player == null) // Should never happen
            return;
        
        FengGameManagerMKII.instance.Heroes.Add(this);
        player.Hero = this;
        
        if ((LevelInfoManager.Get(FengGameManagerMKII.Level).Horse || FengGameManagerMKII.settings.EnableHorse) && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            this.myHorse = PhotonNetwork.Instantiate("horse", this.baseTransform.position + Vector3.up * 5f, this.baseTransform.rotation, 0);
            this.myHorse.GetComponent<Horse>().myHero = gameObject;
            this.myHorse.GetComponent<TITAN_CONTROLLER>().isHorse = true;
        }
        this.sparks = this.baseTransform.Find("slideSparks").GetComponent<ParticleSystem>();
        this.smoke_3dmg = this.baseTransform.Find("3dmg_smoke").GetComponent<ParticleSystem>();
        this.baseTransform.localScale = new Vector3(this.myScale, this.myScale, this.myScale);
        this.facingDirection = this.baseTransform.rotation.eulerAngles.y;
        this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
        this.smoke_3dmg.enableEmission = false;
        this.sparks.enableEmission = false;
        this.speedFXPS = this.speedFX1.GetComponent<ParticleSystem>();
        this.speedFXPS.enableEmission = false;
        Color circleColor = Color.green;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            if (PhotonNetwork.isMasterClient)
            {
                int id = player.ID;
                if (FengGameManagerMKII.heroHash.ContainsKey(id))
                    FengGameManagerMKII.heroHash[id] = this;
                else
                    FengGameManagerMKII.heroHash.Add(id, this);
            }
            this.myNetWorkName = (GameObject) Instantiate(Resources.Load("UI/LabelNameOverHead"));
            this.myNetWorkName.name = "LabelNameOverHead";
            this.myNetWorkName.transform.parent = GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform;
            this.myNetWorkName.transform.localScale = new Vector3(14f, 14f, 14f);
            this.myNetWorkName.GetComponent<UILabel>().text = string.Empty;

            if (photonView.isMine)
            {
                circleColor = Color.green;
                GetComponent<SmoothSyncMovement>().PhotonCamera = true;
                photonView.RPC(Rpc.SetMyPhotonCamera, PhotonTargets.OthersBuffered, PlayerPrefs.GetFloat("cameraDistance") + 0.3f);
            }
            else
            {
                circleColor = Color.blue;
                switch (player.Properties.RCTeam)
                {
                    case 1:
                        circleColor = Color.cyan;
                        break;
                    case 2:
                        circleColor = Color.magenta;
                        break;
                    default:
                        if (player.Properties.IsAHSS == true)
                            circleColor = Color.red;
                        break;
                }
            }
            
            StringBuilder builder = new StringBuilder(100);
            if (!photonView.isMine && player.Properties.IsAHSS == true)
                builder.Append("[FF0000]AHSS\n");
            if (!string.IsNullOrEmpty(player.Properties.Guild))
                builder.AppendFormat("[FFFF00]{0}\n", player.Properties.Guild);
            builder.AppendFormat("[FFFFFF]{0}", player.Properties.Name);
            myNetWorkName.GetComponent<UILabel>().text = builder.ToString();
        }
        if (Minimap.instance != null)
            Minimap.instance.TrackGameObjectOnMinimap(gameObject, circleColor, false, true);
        
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine)
        {
            gameObject.layer = LayerMask.NameToLayer("NetworkObject");
            if (IN_GAME_MAIN_CAMERA.Daylight == Daylight.Night)
            {
                GameObject obj3 = (GameObject) Instantiate(Resources.Load("flashlight"));
                obj3.transform.parent = this.baseTransform;
                obj3.transform.position = this.baseTransform.position + Vector3.up;
                obj3.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
            }
            this.setup.myCostume = new HeroCostume();
            this.setup.myCostume = CostumeConverter.PhotonDataToHeroCostume(player);
            this.setup.setCharacterComponent();
            Destroy(this.checkBoxLeft);
            Destroy(this.checkBoxRight);
            Destroy(this.leftbladetrail);
            Destroy(this.rightbladetrail);
            Destroy(this.leftbladetrail2);
            Destroy(this.rightbladetrail2);
            this.hasspawn = true;
        }
        else
        {
            this.currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            this.loadskin();
            this.hasspawn = true;
            StartCoroutine(this.reloadSky());
        }
        this.bombImmune = false;
        if (FengGameManagerMKII.settings.IsBombMode)
        {
            this.bombImmune = true;
            StartCoroutine(this.stopImmunity());
        }
    }

    public IEnumerator stopImmunity()
    {
        yield return new WaitForSeconds(5f);
        this.bombImmune = false;
    }

    private void Suicide()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            this.netDieLocal(rigidbody.velocity * 50f, false, -1, string.Empty, true);
            FengGameManagerMKII.instance.needChooseSide = true;
            FengGameManagerMKII.instance.justSuicide = true;
        }
    }

    private void throwBlades()
    {
        Transform transform = this.setup.part_blade_l.transform;
        Transform transform2 = this.setup.part_blade_r.transform;
        GameObject obj2 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_l"), transform.position, transform.rotation);
        GameObject obj3 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
        obj2.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        obj3.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
        Vector3 force = this.transform.forward + this.transform.up * 2f - this.transform.right;
        obj2.rigidbody.AddForce(force, ForceMode.Impulse);
        Vector3 vector2 = (this.transform.forward + this.transform.up * 2f) + this.transform.right;
        obj3.rigidbody.AddForce(vector2, ForceMode.Impulse);
        Vector3 torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        torque.Normalize();
        obj2.rigidbody.AddTorque(torque);
        torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        torque.Normalize();
        obj3.rigidbody.AddTorque(torque);
        this.setup.part_blade_l.SetActive(false);
        this.setup.part_blade_r.SetActive(false);
        this.currentBladeNum--;
        if (this.currentBladeNum == 0)
        {
            this.currentBladeSta = 0f;
        }
        if (this.State == HeroState.Attack)
        {
            this.falseAttack();
        }
    }

    public void ungrabbed()
    {
        this.facingDirection = 0f;
        this.targetRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.parent = null;
        GetComponent<CapsuleCollider>().isTrigger = false;
        this.State = HeroState.Idle;
    }

    private void unmounted()
    {
        this.myHorse.GetComponent<Horse>().unmounted();
        this.isMounted = false;
    }

    public void UpdateName(PlayerProperties props)
    {
        if (photonView == null || myNetWorkName.GetComponent<UILabel>() == null) 
            return;

        if (string.IsNullOrEmpty(props.Guild)) 
            myNetWorkName.GetComponent<UILabel>().text = props.Name;
        else  
            myNetWorkName.GetComponent<UILabel>().text = $"{props.Guild}\n{props.Name}";
    }

    public void DoUpdate()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing)
        {
            if (this.invincible > 0f)
            {
                this.invincible -= Time.deltaTime;
            }
            if (!this.hasDied)
            {
                if (this.titanForm && this.eren_titan != null)
                {
                    this.baseTransform.position = this.eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
                    gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
                }
                else if (this.isCannon && this.myCannon != null)
                {
                    this.updateCannon();
                    gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
                }
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
                {
                    if (this.myCannonRegion != null)
                    {
                        if (Shelter.InputManager.IsDown(InputAction.EnterCannon))
                        {
                            this.myCannonRegion.photonView.RPC(Rpc.RequestControl, PhotonTargets.MasterClient, new object[] { photonView.viewID });
                        }
                    }
                    if (this.State == HeroState.Grab && !this.useGun)
                    {
                        if (Shelter.InputManager.IsDown(InputAction.Suicide))
                        {
                            Suicide(); //TODO: Check if PT and give kill to him
                        }
                        
                        if (this.skillId == "jean")
                        {
                            if (this.State != HeroState.Attack && (Shelter.InputManager.IsDown(InputAction.Attack) || Shelter.InputManager.IsDown(InputAction.Special)) && this.escapeTimes > 0 && !this.baseAnimation.IsPlaying("grabbed_jean"))
                            {
                                this.playAnimation("grabbed_jean");
                                this.baseAnimation["grabbed_jean"].time = 0f;
                                this.escapeTimes--;
                            }
                            if (this.baseAnimation.IsPlaying("grabbed_jean") && this.baseAnimation["grabbed_jean"].normalizedTime > 0.64f && this.titanWhoGrabMe.GetComponent<TITAN>() != null)
                            {
                                this.ungrabbed();
                                this.baseRigidBody.velocity = Vector3.up * 30f;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    this.titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                                }
                                else
                                {
                                    photonView.RPC(Rpc.GrabRemove, PhotonTargets.All, new object[0]);
                                    if (PhotonNetwork.isMasterClient)
                                    {
                                        this.titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                                    }
                                    else
                                    {
                                        PhotonView.Find(this.titanWhoGrabMeID).RPC(Rpc.GrabEscape, PhotonTargets.MasterClient, new object[0]);
                                    }
                                }
                            }
                        }
                        else if (this.skillId == "eren")
                        {
                            this.showSkillCD();
                            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer || IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer && !IN_GAME_MAIN_CAMERA.isPausing)
                            {
                                this.calcSkillCD();
                                this.calcFlareCD();
                            }
                            if (Shelter.InputManager.IsDown(InputAction.Special))
                            {
                                bool flag2 = false;
                                if (this.skillCDDuration > 0f || flag2)
                                {
                                    flag2 = true;
                                }
                                else
                                {
                                    this.skillCDDuration = this.skillCDLast;
                                    if (this.skillId == "eren" && this.titanWhoGrabMe.GetComponent<TITAN>() != null)
                                    {
                                        this.ungrabbed();
                                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                        {
                                            this.titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                                        }
                                        else
                                        {
                                            photonView.RPC(Rpc.GrabRemove, PhotonTargets.All, new object[0]);
                                            if (PhotonNetwork.isMasterClient)
                                            {
                                                this.titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                                            }
                                            else
                                            {
                                                PhotonView.Find(this.titanWhoGrabMeID).photonView.RPC(Rpc.GrabEscape, PhotonTargets.MasterClient, new object[0]);
                                            }
                                        }
                                        this.erenTransform();
                                    }
                                }
                            }
                        }
                    }
                    else if (!titanForm && !isCannon)
                    {
                        if (Shelter.InputManager.IsDown(InputAction.Suicide))
                            this.Suicide();
                        
                        this.SpeedupUpdate();
                        this.updateExt();
                        
                        if (!this.grounded && this.State != HeroState.AirDodge)
                        {
                            this.checkDashDoubleTap();
                            if (this.dashD)
                            {
                                this.dashD = false;
                                this.dash(0f, -1f);
                                return;
                            }
                            if (this.dashU)
                            {
                                this.dashU = false;
                                this.dash(0f, 1f);
                                return;
                            }
                            if (this.dashL)
                            {
                                this.dashL = false;
                                this.dash(-1f, 0f);
                                return;
                            }
                            if (this.dashR)
                            {
                                this.dashR = false;
                                this.dash(1f, 0f);
                                return;
                            }
                        }
                        if (this.grounded && this.State == HeroState.Idle)
                        {
                            if (Shelter.InputManager.IsKeyPressed(InputAction.Jump) && !this.baseAnimation.IsPlaying("jump") && !this.baseAnimation.IsPlaying("horse_geton"))
                            {
                                this.idle();
                                this.crossFade("jump", 0.1f);
                                this.sparks.enableEmission = false;
                            }
                            if (Shelter.InputManager.IsDown(InputAction.Dodge) && !this.baseAnimation.IsPlaying("jump") && !this.baseAnimation.IsPlaying("horse_geton"))
                            {
                                if (this.myHorse != null && !this.isMounted && Vector3.Distance(this.myHorse.transform.position, transform.position) < 15f)
                                    this.getOnHorse();
                                else
                                    this.Dodge(false);
                            }
                        }
                        if (this.State == HeroState.Idle)
                        {
                            if (Shelter.InputManager.IsDown(InputAction.GreenFlare))
                            {
                                this.shootFlare(1);
                            }
                            if (Shelter.InputManager.IsDown(InputAction.RedFlare))
                            {
                                this.shootFlare(2);
                            }
                            if (Shelter.InputManager.IsDown(InputAction.BlackFlare))
                            {
                                this.shootFlare(3);
                            }
                            if (this.myHorse != null && this.isMounted && Shelter.InputManager.IsDown(InputAction.Dodge))
                            {
                                this.getOffHorse();
                            }
                            if ((animation.IsPlaying(this.standAnimation) || !this.grounded) && Shelter.InputManager.IsDown(InputAction.Reload) && (!this.useGun || !FengGameManagerMKII.settings.AllowAirAHSSReload || this.grounded))
                            {
                                this.changeBlade();
                                return;
                            }
                            if (this.baseAnimation.IsPlaying(this.standAnimation) && Shelter.InputManager.IsDown(InputAction.Salute))
                            {
                                this.salute();
                                return;
                            }
                            if (!this.isMounted && (Shelter.InputManager.IsDown(InputAction.Attack) || Shelter.InputManager.IsDown(InputAction.Special)) && !Screen.showCursor && !this.useGun)
                            {
                                bool isSkillInCD = false;
                                if (Shelter.InputManager.IsDown(InputAction.Special))
                                {
                                    isSkillInCD = skillCDDuration > 0f;
                                    if (!isSkillInCD)
                                    {
                                        this.skillCDDuration = this.skillCDLast;
                                        if (this.skillId == "eren")
                                        {
                                            this.erenTransform();
                                            return;
                                        }
                                        if (this.skillId == "marco")
                                        {
                                            if (this.IsGrounded())
                                            {
                                                this.attackAnimation = UnityEngine.Random.Range(0, 2) != 0 ? "special_marco_1" : "special_marco_0";
                                                this.playAnimation(this.attackAnimation);
                                            }
                                            else
                                            {
                                                isSkillInCD = true;
                                                this.skillCDDuration = 0f;
                                            }
                                        }
                                        else if (this.skillId == "armin")
                                        {
                                            if (this.IsGrounded())
                                            {
                                                this.attackAnimation = "special_armin";
                                                this.playAnimation("special_armin");
                                            }
                                            else
                                            {
                                                isSkillInCD = true;
                                                this.skillCDDuration = 0f;
                                            }
                                        }
                                        else if (this.skillId == "sasha")
                                        {
                                            if (this.IsGrounded())
                                            {
                                                this.attackAnimation = "special_sasha";
                                                this.playAnimation("special_sasha");
                                                _isSpeedup = true;
                                                this._speedupTime = 10f;
                                            }
                                            else
                                            {
                                                isSkillInCD = true;
                                                this.skillCDDuration = 0f;
                                            }
                                        }
                                        else if (this.skillId == "mikasa")
                                        {
                                            this.attackAnimation = "attack3_1";
                                            this.playAnimation("attack3_1");
                                            this.baseRigidBody.velocity = Vector3.up * 10f;
                                        }
                                        else if (this.skillId == "levi")
                                        {
                                            RaycastHit hit;
                                            this.attackAnimation = "attack5";
                                            this.playAnimation("attack5");
                                            this.baseRigidBody.velocity += Vector3.up * 5f;
                                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                            LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
                                            LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
                                            LayerMask mask3 = mask2 | mask;
                                            if (Physics.Raycast(ray, out hit, 1E+07f, mask3.value))
                                            {
                                                if (this.bulletRight != null)
                                                {
                                                    this.bulletRight.GetComponent<Bullet>().disable();
                                                    this.releaseIfIHookSb();
                                                }
                                                this.dashDirection = hit.point - this.baseTransform.position;
                                                this.launchRightRope(hit, true, 1);
                                                this.rope.Play();
                                            }
                                            this.facingDirection = Mathf.Atan2(this.dashDirection.x, this.dashDirection.z) * 57.29578f;
                                            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                                            this.attackLoop = 3;
                                        }
                                        else if (this.skillId == "petra")
                                        {
                                            RaycastHit hit2;
                                            this.attackAnimation = "special_petra";
                                            this.playAnimation("special_petra");
                                            this.baseRigidBody.velocity += Vector3.up * 5f;
                                            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                            LayerMask mask4 = 1 << LayerMask.NameToLayer("Ground");
                                            LayerMask mask5 = 1 << LayerMask.NameToLayer("EnemyBox");
                                            LayerMask mask6 = mask5 | mask4;
                                            if (Physics.Raycast(ray2, out hit2, 1E+07f, mask6.value))
                                            {
                                                if (this.bulletRight != null)
                                                {
                                                    this.bulletRight.GetComponent<Bullet>().disable();
                                                    this.releaseIfIHookSb();
                                                }
                                                if (this.bulletLeft != null)
                                                {
                                                    this.bulletLeft.GetComponent<Bullet>().disable();
                                                    this.releaseIfIHookSb();
                                                }
                                                this.dashDirection = hit2.point - this.baseTransform.position;
                                                this.launchLeftRope(hit2, true, 0);
                                                this.launchRightRope(hit2, true, 0);
                                                this.rope.Play();
                                            }
                                            this.facingDirection = Mathf.Atan2(this.dashDirection.x, this.dashDirection.z) * 57.29578f;
                                            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                                            this.attackLoop = 3;
                                        }
                                        else
                                        {
                                            if (this.needLean)
                                            {
                                                if (this.leanLeft)
                                                {
                                                    this.attackAnimation = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                                }
                                                else
                                                {
                                                    this.attackAnimation = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                                }
                                            }
                                            else
                                            {
                                                this.attackAnimation = "attack1";
                                            }
                                            this.playAnimation(this.attackAnimation);
                                        }
                                    }
                                }
                                else if (Shelter.InputManager.IsKeyPressed(InputAction.Attack))
                                {
                                    if (this.needLean)
                                    {
                                        if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                                        {
                                            this.attackAnimation = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                        }
                                        else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                                        {
                                            this.attackAnimation = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                        }
                                        else if (this.leanLeft)
                                        {
                                            this.attackAnimation = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                        }
                                        else
                                        {
                                            this.attackAnimation = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                        }
                                    }
                                    else if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                                    {
                                        this.attackAnimation = "attack2";
                                    }
                                    else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                                    {
                                        this.attackAnimation = "attack1";
                                    }
                                    else if (this.lastHook != null)
                                    {
                                        if (this.lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck") != null)
                                        {
                                            this.attackAccordingToTarget(this.lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck"));
                                        }
                                        else
                                        {
                                            isSkillInCD = true;
                                        }
                                    }
                                    else if (this.bulletLeft != null && this.bulletLeft.transform.parent != null)
                                    {
                                        Transform a = this.bulletLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                        if (a != null)
                                        {
                                            this.attackAccordingToTarget(a);
                                        }
                                        else
                                        {
                                            this.attackAccordingToMouse();
                                        }
                                    }
                                    else if (this.bulletRight != null && this.bulletRight.transform.parent != null)
                                    {
                                        Transform transform2 = this.bulletRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                        if (transform2 != null)
                                        {
                                            this.attackAccordingToTarget(transform2);
                                        }
                                        else
                                        {
                                            this.attackAccordingToMouse();
                                        }
                                    }
                                    else
                                    {
                                        GameObject obj2 = this.findNearestTitan();
                                        if (obj2 != null)
                                        {
                                            Transform transform3 = obj2.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                            if (transform3 != null)
                                            {
                                                this.attackAccordingToTarget(transform3);
                                            }
                                            else
                                            {
                                                this.attackAccordingToMouse();
                                            }
                                        }
                                        else
                                        {
                                            this.attackAccordingToMouse();
                                        }
                                    }
                                }
                                if (!isSkillInCD)
                                {
                                    this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                    this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                                    if (this.grounded)
                                    {
                                        this.baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                                    }
                                    this.playAnimation(this.attackAnimation);
                                    this.baseAnimation[this.attackAnimation].time = 0f;
                                    this.buttonAttackRelease = false;
                                    this.State = HeroState.Attack;
                                    if (this.grounded || this.attackAnimation == "attack3_1" || this.attackAnimation == "attack5" || this.attackAnimation == "special_petra")
                                    {
                                        this.attackReleased = true;
                                        this.buttonAttackRelease = true;
                                    }
                                    else
                                    {
                                        this.attackReleased = false;
                                    }
                                    this.sparks.enableEmission = false;
                                }
                            }
                            if (this.useGun && !Screen.showCursor)
                            {
                                if (Shelter.InputManager.IsDown(InputAction.Special))
                                {
                                    this.leftArmAim = true;
                                    this.rightArmAim = true;
                                }
                                else if (Shelter.InputManager.IsKeyPressed(InputAction.Attack))
                                {
                                    if (this.leftGunHasBullet)
                                    {
                                        this.leftArmAim = true;
                                        this.rightArmAim = false;
                                    }
                                    else
                                    {
                                        this.leftArmAim = false;
                                        if (this.rightGunHasBullet)
                                        {
                                            this.rightArmAim = true;
                                        }
                                        else
                                        {
                                            this.rightArmAim = false;
                                        }
                                    }
                                }
                                else
                                {
                                    this.leftArmAim = false;
                                    this.rightArmAim = false;
                                }
                                if (this.leftArmAim || this.rightArmAim)
                                {
                                    RaycastHit hit3;
                                    Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    LayerMask mask7 = 1 << LayerMask.NameToLayer("Ground");
                                    LayerMask mask8 = 1 << LayerMask.NameToLayer("EnemyBox");
                                    LayerMask mask9 = mask8 | mask7;
                                    if (Physics.Raycast(ray3, out hit3, 1E+07f, mask9.value))
                                    {
                                        this.gunTarget = hit3.point;
                                    }
                                }
                                bool flag4 = false;
                                bool flag5 = false;
                                bool flag6 = false;
                                if (Shelter.InputManager.IsUp(InputAction.Special) && this.skillId != "bomb")
                                {
                                    if (this.leftGunHasBullet && this.rightGunHasBullet)
                                    {
                                        if (this.grounded)
                                        {
                                            this.attackAnimation = "AHSS_shoot_both";
                                        }
                                        else
                                        {
                                            this.attackAnimation = "AHSS_shoot_both_air";
                                        }
                                        flag4 = true;
                                    }
                                    else if (!(this.leftGunHasBullet || this.rightGunHasBullet))
                                    {
                                        flag5 = true;
                                    }
                                    else
                                    {
                                        flag6 = true;
                                    }
                                }
                                if (flag6 || Shelter.InputManager.IsUp(InputAction.Attack))
                                {
                                    if (this.grounded)
                                    {
                                        if (this.leftGunHasBullet && this.rightGunHasBullet)
                                        {
                                            if (this.isLeftHandHooked)
                                            {
                                                this.attackAnimation = "AHSS_shoot_r";
                                            }
                                            else
                                            {
                                                this.attackAnimation = "AHSS_shoot_l";
                                            }
                                        }
                                        else if (this.leftGunHasBullet)
                                        {
                                            this.attackAnimation = "AHSS_shoot_l";
                                        }
                                        else if (this.rightGunHasBullet)
                                        {
                                            this.attackAnimation = "AHSS_shoot_r";
                                        }
                                    }
                                    else if (this.leftGunHasBullet && this.rightGunHasBullet)
                                    {
                                        if (this.isLeftHandHooked)
                                        {
                                            this.attackAnimation = "AHSS_shoot_r_air";
                                        }
                                        else
                                        {
                                            this.attackAnimation = "AHSS_shoot_l_air";
                                        }
                                    }
                                    else if (this.leftGunHasBullet)
                                    {
                                        this.attackAnimation = "AHSS_shoot_l_air";
                                    }
                                    else if (this.rightGunHasBullet)
                                    {
                                        this.attackAnimation = "AHSS_shoot_r_air";
                                    }
                                    if (this.leftGunHasBullet || this.rightGunHasBullet)
                                    {
                                        flag4 = true;
                                    }
                                    else
                                    {
                                        flag5 = true;
                                    }
                                }
                                if (flag4)
                                {
                                    this.State = HeroState.Attack;
                                    this.crossFade(this.attackAnimation, 0.05f);
                                    this.gunDummy.transform.position = this.baseTransform.position;
                                    this.gunDummy.transform.rotation = this.baseTransform.rotation;
                                    this.gunDummy.transform.LookAt(this.gunTarget);
                                    this.attackReleased = false;
                                    this.facingDirection = this.gunDummy.transform.rotation.eulerAngles.y;
                                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                                }
                                else if (flag5 && (this.grounded || LevelInfoManager.Get(FengGameManagerMKII.Level).Gamemode != GameMode.PvpAHSS && !FengGameManagerMKII.settings.AllowAirAHSSReload))
                                {
                                    this.changeBlade();
                                }
                            }
                        }
                        else if (this.State == HeroState.Attack)
                        {
                            if (!this.useGun)
                            {
                                if (!Shelter.InputManager.IsKeyPressed(InputAction.Attack))
                                {
                                    this.buttonAttackRelease = true;
                                }
                                if (!this.attackReleased)
                                {
                                    if (this.buttonAttackRelease)
                                    {
                                        this.continueAnimation();
                                        this.attackReleased = true;
                                    }
                                    else if (this.baseAnimation[this.attackAnimation].normalizedTime >= 0.32f)
                                    {
                                        this.pauseAnimation();
                                    }
                                }
                                if (this.attackAnimation == "attack3_1" && this.currentBladeSta > 0f)
                                {
                                    if (this.baseAnimation[this.attackAnimation].normalizedTime >= 0.8f)
                                    {
                                        if (!this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                        {
                                            this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                                            if (ModuleManager.Enabled(nameof(ModuleWeaponTrail)))
                                            {
                                                this.leftbladetrail2.Activate();
                                                this.rightbladetrail2.Activate();
                                                this.leftbladetrail.Activate();
                                                this.rightbladetrail.Activate();
                                            }
                                            this.baseRigidBody.velocity = -Vector3.up * 30f;
                                        }
                                        if (!this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
                                        {
                                            this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
                                            this.slash.Play();
                                        }
                                    }
                                    else if (this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                    {
                                        this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                                        this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                                        this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                        this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                                        this.leftbladetrail.StopSmoothly(0.1f);
                                        this.rightbladetrail.StopSmoothly(0.1f);
                                        this.leftbladetrail2.StopSmoothly(0.1f);
                                        this.rightbladetrail2.StopSmoothly(0.1f);
                                    }
                                }
                                else
                                {
                                    float num;
                                    float num2;
                                    if (this.currentBladeSta == 0f)
                                    {
                                        num = -1f;
                                        num2 = -1f;
                                    }
                                    else if (this.attackAnimation == "attack5")
                                    {
                                        num2 = 0.35f;
                                        num = 0.5f;
                                    }
                                    else if (this.attackAnimation == "special_petra")
                                    {
                                        num2 = 0.35f;
                                        num = 0.48f;
                                    }
                                    else if (this.attackAnimation == "special_armin")
                                    {
                                        num2 = 0.25f;
                                        num = 0.35f;
                                    }
                                    else if (this.attackAnimation == "attack4")
                                    {
                                        num2 = 0.6f;
                                        num = 0.9f;
                                    }
                                    else if (this.attackAnimation == "special_sasha")
                                    {
                                        num = -1f;
                                        num2 = -1f;
                                    }
                                    else
                                    {
                                        num2 = 0.5f;
                                        num = 0.85f;
                                    }
                                    if (this.baseAnimation[this.attackAnimation].normalizedTime > num2 && this.baseAnimation[this.attackAnimation].normalizedTime < num)
                                    {
                                        if (!this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                        {
                                            this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                                            this.slash.Play();
                                            if (ModuleManager.Enabled(nameof(ModuleWeaponTrail)))
                                            {
                                                this.leftbladetrail2.Activate();
                                                this.rightbladetrail2.Activate();
                                                this.leftbladetrail.Activate();
                                                this.rightbladetrail.Activate();
                                            }
                                        }
                                        if (!this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
                                        {
                                            this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
                                        }
                                    }
                                    else if (this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                    {
                                        this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                                        this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                                        this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                        this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                                        this.leftbladetrail2.StopSmoothly(0.1f);
                                        this.rightbladetrail2.StopSmoothly(0.1f);
                                        this.leftbladetrail.StopSmoothly(0.1f);
                                        this.rightbladetrail.StopSmoothly(0.1f);
                                    }
                                    if (this.attackLoop > 0 && this.baseAnimation[this.attackAnimation].normalizedTime > num)
                                    {
                                        this.attackLoop--;
                                        this.playAnimationAt(this.attackAnimation, num2);
                                    }
                                }
                                if (this.baseAnimation[this.attackAnimation].normalizedTime >= 1f)
                                {
                                    if (this.attackAnimation == "special_marco_0" || this.attackAnimation == "special_marco_1")
                                    {
                                        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
                                        {
                                            if (!PhotonNetwork.isMasterClient)
                                            {
                                                photonView.RPC(Rpc.TauntAttack, PhotonTargets.MasterClient, 5f, 100f);
                                            }
                                            else
                                            {
                                                this.NetTauntAttack(5f);
                                            }
                                        }
                                        else
                                        {
                                            this.NetTauntAttack(5f, 100f);
                                        }
                                        this.falseAttack();
                                        this.idle();
                                    }
                                    else if (this.attackAnimation == "special_armin")
                                    {
                                        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
                                        {
                                            if (!PhotonNetwork.isMasterClient)
                                            {
                                                photonView.RPC(Rpc.LaughAttack, PhotonTargets.MasterClient, new object[0]);
                                            }
                                            else
                                            {
                                                this.NetlaughAttack();
                                            }
                                        }
                                        else
                                        {
                                            foreach (GameObject obj3 in GameObject.FindGameObjectsWithTag("titan"))
                                            {
                                                if (Vector3.Distance(obj3.transform.position, this.baseTransform.position) < 50f && Vector3.Angle(obj3.transform.forward, this.baseTransform.position - obj3.transform.position) < 90f && obj3.GetComponent<TITAN>() != null)
                                                {
                                                    obj3.GetComponent<TITAN>().StartLaughing();
                                                }
                                            }
                                        }
                                        this.falseAttack();
                                        this.idle();
                                    }
                                    else if (this.attackAnimation == "attack3_1")
                                    {
                                        this.baseRigidBody.velocity -= Vector3.up * Time.deltaTime * 30f;
                                    }
                                    else
                                    {
                                        this.falseAttack();
                                        this.idle();
                                    }
                                }
                                if (this.baseAnimation.IsPlaying("attack3_2") && this.baseAnimation["attack3_2"].normalizedTime >= 1f)
                                {
                                    this.falseAttack();
                                    this.idle();
                                }
                            }
                            else
                            {
                                this.baseTransform.rotation = Quaternion.Lerp(this.baseTransform.rotation, this.gunDummy.transform.rotation, Time.deltaTime * 30f);
                                if (!this.attackReleased && this.baseAnimation[this.attackAnimation].normalizedTime > 0.167f)
                                {
                                    GameObject obj4;
                                    this.attackReleased = true;
                                    bool flag7 = false;
                                    if (this.attackAnimation == "AHSS_shoot_both" || this.attackAnimation == "AHSS_shoot_both_air")
                                    {
                                        flag7 = true;
                                        this.leftGunHasBullet = false;
                                        this.rightGunHasBullet = false;
                                        this.baseRigidBody.AddForce(-this.baseTransform.forward * 1000f, ForceMode.Acceleration);
                                    }
                                    else
                                    {
                                        if (this.attackAnimation == "AHSS_shoot_l" || this.attackAnimation == "AHSS_shoot_l_air")
                                        {
                                            this.leftGunHasBullet = false;
                                        }
                                        else
                                        {
                                            this.rightGunHasBullet = false;
                                        }
                                        this.baseRigidBody.AddForce(-this.baseTransform.forward * 600f, ForceMode.Acceleration);
                                    }
                                    this.baseRigidBody.AddForce(Vector3.up * 200f, ForceMode.Acceleration);
                                    string prefabName = "FX/shotGun";
                                    if (flag7)
                                    {
                                        prefabName = "FX/shotGun 1";
                                    }
                                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                                    {
                                        obj4 = PhotonNetwork.Instantiate(prefabName, this.baseTransform.position + this.baseTransform.up * 0.8f - this.baseTransform.right * 0.1f, this.baseTransform.rotation, 0);
                                        if (obj4.GetComponent<EnemyfxIDcontainer>() != null)
                                        {
                                            obj4.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = photonView.viewID;
                                        }
                                    }
                                    else
                                    {
                                        obj4 = (GameObject) Instantiate(Resources.Load(prefabName), this.baseTransform.position + this.baseTransform.up * 0.8f - this.baseTransform.right * 0.1f, this.baseTransform.rotation);
                                    }
                                }
                                if (this.baseAnimation[this.attackAnimation].normalizedTime >= 1f)
                                {
                                    this.falseAttack();
                                    this.idle();
                                }
                                if (!this.baseAnimation.IsPlaying(this.attackAnimation))
                                {
                                    this.falseAttack();
                                    this.idle();
                                }
                            }
                        }
                        else if (this.State == HeroState.ChangeBlade)
                        {
                            if (this.useGun)
                            {
                                if (this.baseAnimation[this.reloadAnimation].normalizedTime > 0.22f)
                                {
                                    if (!(this.leftGunHasBullet || !this.setup.part_blade_l.activeSelf))
                                    {
                                        this.setup.part_blade_l.SetActive(false);
                                        Transform transform = this.setup.part_blade_l.transform;
                                        GameObject obj5 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_l"), transform.position, transform.rotation);
                                        obj5.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
                                        Vector3 force = -this.baseTransform.forward * 10f + this.baseTransform.up * 5f - this.baseTransform.right;
                                        obj5.rigidbody.AddForce(force, ForceMode.Impulse);
                                        Vector3 torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
                                        obj5.rigidbody.AddTorque(torque, ForceMode.Acceleration);
                                    }
                                    if (!(this.rightGunHasBullet || !this.setup.part_blade_r.activeSelf))
                                    {
                                        this.setup.part_blade_r.SetActive(false);
                                        Transform transform5 = this.setup.part_blade_r.transform;
                                        GameObject obj6 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_r"), transform5.position, transform5.rotation);
                                        obj6.renderer.material = CharacterMaterials.materials[this.setup.myCostume._3dmg_texture];
                                        Vector3 vector3 = (-this.baseTransform.forward * 10f + this.baseTransform.up * 5f) + this.baseTransform.right;
                                        obj6.rigidbody.AddForce(vector3, ForceMode.Impulse);
                                        Vector3 vector4 = new Vector3(UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300));
                                        obj6.rigidbody.AddTorque(vector4, ForceMode.Acceleration);
                                    }
                                }
                                if (this.baseAnimation[this.reloadAnimation].normalizedTime > 0.62f && !this.throwedBlades)
                                {
                                    this.throwedBlades = true;
                                    if (!(this.leftBulletLeft <= 0 || this.leftGunHasBullet))
                                    {
                                        this.leftBulletLeft--;
                                        this.setup.part_blade_l.SetActive(true);
                                        this.leftGunHasBullet = true;
                                    }
                                    if (!(this.rightBulletLeft <= 0 || this.rightGunHasBullet))
                                    {
                                        this.setup.part_blade_r.SetActive(true);
                                        this.rightBulletLeft--;
                                        this.rightGunHasBullet = true;
                                    }
                                    this.updateRightMagUI();
                                    this.updateLeftMagUI();
                                }
                                if (this.baseAnimation[this.reloadAnimation].normalizedTime > 1f)
                                {
                                    this.idle();
                                }
                            }
                            else
                            {
                                if (!this.grounded)
                                {
                                    if (!(animation[this.reloadAnimation].normalizedTime < 0.2f || this.throwedBlades))
                                    {
                                        this.throwedBlades = true;
                                        if (this.setup.part_blade_l.activeSelf)
                                        {
                                            this.throwBlades();
                                        }
                                    }
                                    if (animation[this.reloadAnimation].normalizedTime >= 0.56f && this.currentBladeNum > 0)
                                    {
                                        this.setup.part_blade_l.SetActive(true);
                                        this.setup.part_blade_r.SetActive(true);
                                        this.currentBladeSta = this.totalBladeSta;
                                    }
                                }
                                else
                                {
                                    if (!(this.baseAnimation[this.reloadAnimation].normalizedTime < 0.13f || this.throwedBlades))
                                    {
                                        this.throwedBlades = true;
                                        if (this.setup.part_blade_l.activeSelf)
                                        {
                                            this.throwBlades();
                                        }
                                    }
                                    if (this.baseAnimation[this.reloadAnimation].normalizedTime >= 0.37f && this.currentBladeNum > 0)
                                    {
                                        this.setup.part_blade_l.SetActive(true);
                                        this.setup.part_blade_r.SetActive(true);
                                        this.currentBladeSta = this.totalBladeSta;
                                    }
                                }
                                if (this.baseAnimation[this.reloadAnimation].normalizedTime >= 1f)
                                {
                                    this.idle();
                                }
                            }
                        }
                        else if (this.State == HeroState.Salute)
                        {
                            if (this.baseAnimation["salute"].normalizedTime >= 1f)
                            {
                                this.idle();
                            }
                        }
                        else if (this.State == HeroState.GroundDodge)
                        {
                            if (this.baseAnimation.IsPlaying("dodge"))
                            {
                                if (!(this.grounded || this.baseAnimation["dodge"].normalizedTime <= 0.6f))
                                {
                                    this.idle();
                                }
                                if (this.baseAnimation["dodge"].normalizedTime >= 1f)
                                {
                                    this.idle();
                                }
                            }
                        }
                        else if (this.State == HeroState.Land)
                        {
                            if (this.baseAnimation.IsPlaying("dash_land") && this.baseAnimation["dash_land"].normalizedTime >= 1f)
                            {
                                this.idle();
                            }
                        }
                        else if (this.State == HeroState.FillGas)
                        {
                            if (this.baseAnimation.IsPlaying("supply") && this.baseAnimation["supply"].normalizedTime >= 1f)
                            {
                                this.currentBladeSta = this.totalBladeSta;
                                this.currentBladeNum = this.totalBladeNum;
                                this.currentGas = this.totalGas;
                                if (!this.useGun)
                                {
                                    this.setup.part_blade_l.SetActive(true);
                                    this.setup.part_blade_r.SetActive(true);
                                }
                                else
                                {
                                    this.leftBulletLeft = this.rightBulletLeft = this.bulletMAX;
                                    this.rightGunHasBullet = true;
                                    this.leftGunHasBullet = true;
                                    this.setup.part_blade_l.SetActive(true);
                                    this.setup.part_blade_r.SetActive(true);
                                    this.updateRightMagUI();
                                    this.updateLeftMagUI();
                                }
                                this.idle();
                            }
                        }
                        else if (this.State == HeroState.Slide)
                        {
                            if (!this.grounded)
                            {
                                this.idle();
                            }
                        }
                        else if (this.State == HeroState.AirDodge)
                        {
                            if (this.dashTime > 0f)
                            {
                                this.dashTime -= Time.deltaTime;
                                if (this.currentSpeed > this.originVM)
                                {
                                    this.baseRigidBody.AddForce(-this.baseRigidBody.velocity * Time.deltaTime * 1.7f, ForceMode.VelocityChange);
                                }
                            }
                            else
                            {
                                this.dashTime = 0f;
                                this.idle();
                            }
                        }
                        if (!(!Shelter.InputManager.IsKeyPressed(InputAction.LeftHook) || ((this.baseAnimation.IsPlaying("attack3_1") || this.baseAnimation.IsPlaying("attack5") || this.baseAnimation.IsPlaying("special_petra") || this.State == HeroState.Grab) && this.State != HeroState.Idle)))
                        {
                            if (this.bulletLeft != null)
                            {
                                this.QHold = true;
                            }
                            else
                            {
                                Ray ray4 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                LayerMask mask10 = 1 << LayerMask.NameToLayer("Ground");
                                LayerMask mask11 = 1 << LayerMask.NameToLayer("EnemyBox");
                                LayerMask mask12 = mask11 | mask10;
                                if (Physics.Raycast(ray4, out var hit4, 10000f, mask12.value))
                                {
                                    this.launchLeftRope(hit4, true, 0);
                                    this.rope.Play();
                                }
                            }
                        }
                        else
                        {
                            this.QHold = false;
                        }
                        if (!(!Shelter.InputManager.IsKeyPressed(InputAction.RightHook) || (this.baseAnimation.IsPlaying("attack3_1") || this.baseAnimation.IsPlaying("attack5") || this.baseAnimation.IsPlaying("special_petra") || this.State == HeroState.Grab) && this.State != HeroState.Idle))
                        {
                            if (this.bulletRight != null)
                            {
                                this.EHold = true;
                            }
                            else
                            {
                                Ray ray5 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                LayerMask mask13 = 1 << LayerMask.NameToLayer("Ground");
                                LayerMask mask14 = 1 << LayerMask.NameToLayer("EnemyBox");
                                LayerMask mask15 = mask14 | mask13;
                                if (Physics.Raycast(ray5, out var hit5, 10000f, mask15.value))
                                {
                                    this.launchRightRope(hit5, true, 0);
                                    this.rope.Play();
                                }
                            }
                        }
                        else
                        {
                            this.EHold = false;
                        }
                        if (!(!Shelter.InputManager.IsKeyPressed(InputAction.BothHooks) || ((this.baseAnimation.IsPlaying("attack3_1") || this.baseAnimation.IsPlaying("attack5") || this.baseAnimation.IsPlaying("special_petra") || this.State == HeroState.Grab) && this.State != HeroState.Idle)))
                        {
                            this.QHold = true;
                            this.EHold = true;
                            if (this.bulletLeft == null && this.bulletRight == null)
                            {
                                Ray ray6 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                LayerMask mask16 = 1 << LayerMask.NameToLayer("Ground");
                                LayerMask mask17 = 1 << LayerMask.NameToLayer("EnemyBox");
                                LayerMask mask18 = mask17 | mask16;
                                if (Physics.Raycast(ray6, out var hit6, 1000000f, mask18.value))
                                {
                                    this.launchLeftRope(hit6, false, 0);
                                    this.launchRightRope(hit6, false, 0);
                                    this.rope.Play();
                                }
                            }
                        }
                        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer || IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer && !IN_GAME_MAIN_CAMERA.isPausing)
                        {
                            this.calcSkillCD();
                            this.calcFlareCD();
                        }
                        if (!this.useGun)
                        {
                            if (this.leftbladetrail.gameObject.GetActive())
                            {
                                this.leftbladetrail.update();
                                this.rightbladetrail.update();
                            }
                            if (this.leftbladetrail2.gameObject.GetActive())
                            {
                                this.leftbladetrail2.update();
                                this.rightbladetrail2.update();
                            }
                            if (this.leftbladetrail.gameObject.GetActive())
                            {
                                this.leftbladetrail.lateUpdate();
                                this.rightbladetrail.lateUpdate();
                            }
                            if (this.leftbladetrail2.gameObject.GetActive())
                            {
                                this.leftbladetrail2.lateUpdate();
                                this.rightbladetrail2.lateUpdate();
                            }
                        }
                        if (!IN_GAME_MAIN_CAMERA.isPausing)
                        {
                            this.showSkillCD();
                            this.ShowFlareCD();
                            this.UpdateUsageUI();
                            this.DrawReticle();
                        }
                    }
                    else if (this.isCannon && !IN_GAME_MAIN_CAMERA.isPausing)
                    {
                        this.DrawReticle();
                        this.calcSkillCD();
                        this.showSkillCD();
                    }
                }
            }
        }
    }

    public void updateCannon()
    {
        this.baseTransform.position = this.myCannonPlayer.position;
        this.baseTransform.rotation = this.myCannonBase.rotation;
    }

    public void updateExt()
    {
        if (this.skillId == "bomb")
        {
            if (Shelter.InputManager.IsDown(InputAction.Special) && this.skillCDDuration <= 0f)
            {
                if (!(this.myBomb == null || this.myBomb.disabled))
                {
                    this.myBomb.Explode(this.bombRadius);
                }
                this.detonate = false;
                this.skillCDDuration = this.bombCD;
                RaycastHit hitInfo = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
                LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
                LayerMask mask3 = mask2 | mask;
                this.currentV = this.baseTransform.position;
                this.targetV = this.currentV + Vector3.forward * 200f;
                if (Physics.Raycast(ray, out hitInfo, 1000000f, mask3.value))
                {
                    this.targetV = hitInfo.point;
                }
                Vector3 vector = Vector3.Normalize(this.targetV - this.currentV);
                GameObject obj2 = PhotonNetwork.Instantiate("RCAsset/BombMain", this.currentV + vector * 4f, new Quaternion(0f, 0f, 0f, 1f), 0);
                obj2.rigidbody.velocity = vector * this.bombSpeed;
                this.myBomb = obj2.GetComponent<Bomb>();
                this.bombTime = 0f;
            }
            else if (this.myBomb != null && !this.myBomb.disabled)
            {
                this.bombTime += Time.deltaTime;
                bool flag2 = false;
                if (Shelter.InputManager.IsUp(InputAction.Special))
                {
                    this.detonate = true;
                }
                else if (Shelter.InputManager.IsDown(InputAction.Special) && this.detonate)
                {
                    this.detonate = false;
                    flag2 = true;
                }
                if (this.bombTime >= this.bombTimeMax)
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    this.myBomb.Explode(this.bombRadius);
                    this.detonate = false;
                }
            }
        }
    }

    private void updateLeftMagUI()
    {
        for (int i = 1; i <= this.bulletMAX; i++)
        {
            GameObject.Find("bulletL" + i).GetComponent<UISprite>().enabled = false;
        }
        for (int j = 1; j <= this.leftBulletLeft; j++)
        {
            GameObject.Find("bulletL" + j).GetComponent<UISprite>().enabled = true;
        }
    }

    private void updateRightMagUI()
    {
        for (int i = 1; i <= this.bulletMAX; i++)
        {
            GameObject.Find("bulletR" + i).GetComponent<UISprite>().enabled = false;
        }
        for (int j = 1; j <= this.rightBulletLeft; j++)
        {
            GameObject.Find("bulletR" + j).GetComponent<UISprite>().enabled = true;
        }
    }

    public void useBlade(int amount = 0)
    {
        if (ModuleManager.Enabled(nameof(ModuleInfiniteBlade)))
            return;

        if (amount == 0)
            amount = 1;
        
        amount *= 2;
        if (this.currentBladeSta > 0f)
        {
            this.currentBladeSta -= amount;
            if (this.currentBladeSta <= 0f)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
                {
                    this.leftbladetrail.Deactivate();
                    this.rightbladetrail.Deactivate();
                    this.leftbladetrail2.Deactivate();
                    this.rightbladetrail2.Deactivate();
                    this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                    this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                }
                this.currentBladeSta = 0f;
                this.throwBlades();
            }
        }
    }

    private void useGas(float amount = 0)
    {
        if (ModuleManager.Enabled(nameof(ModuleInfiniteGas)))
            return;
        
        if (amount == 0f)
            amount = this.useGasSpeed;
        
        if (this.currentGas > 0f)
            this.currentGas -= amount;
        if (this.currentGas < 0f)
            this.currentGas = 0f;
    }

    [RPC]
    [UsedImplicitly]
    private void WhoIsMyErenTitan(int id)
    {
        if (PhotonView.TryParse(id, out PhotonView view))
        {
            this.eren_titan = view.gameObject;
            this.titanForm = true;
        }
    }

    public bool IsGrabbed => this.State == HeroState.Grab;
    private HeroState State
    {
        get => this._state;
        set
        {
            if (this._state == HeroState.AirDodge || this._state == HeroState.GroundDodge)
            {
                this.dashTime = 0f;
            }
            this._state = value;
        }
    }
}