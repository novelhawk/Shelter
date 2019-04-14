using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Game;
using Game.Enums;
using JetBrains.Annotations;
using Mod;
using Mod.Exceptions;
using Mod.GameSettings;
using Mod.Keybinds;
using Mod.Managers;
using Mod.Modules;
using Photon;
using Photon.Enums;
using RC;
using UnityEngine;
using Xft;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using LogType = Mod.Logging.LogType;

// ReSharper disable once CheckNamespace
public class HERO : Photon.MonoBehaviour
{
    //TODO: Remove
    public const float CameraMultiplier = 1f;
    
    // Audio Sources
    [UnityInitialized] public AudioSource audio_ally;
    [UnityInitialized] public AudioSource audio_hitwall;
    [UnityInitialized] public AudioSource meatDie;
    [UnityInitialized] public AudioSource slash;
    [UnityInitialized] public AudioSource slashHit;
    [UnityInitialized] public AudioSource rope;
    
    // FXs
    [UnityInitialized] public GameObject speedFX;
    [UnityInitialized] public GameObject speedFX1;

    // HeroStat
    [UnityInitialized] public float totalBladeSta = 100f;
    [UnityInitialized] public float totalGas = 100f;
    
    // AHSS
    [UnityInitialized] public GameObject bulletLeft;         // [AHSS] LEFT  Bullet Icon
    [UnityInitialized] public GameObject bulletRight;        // [AHSS] RIGHT Bullet Icon
    
    // Hero Properties
    [UnityInitialized] public HERO_SETUP setup;
    [UnityInitialized] public Group myGroup;                 // [HERO] HUMAN, TITAN, AHSS
    [UnityInitialized] public int myTeam = 1;
    [UnityInitialized] public bool hasDied;                  // [HERO] Is Dead
    [UnityInitialized] public float currentSpeed;            // [HERO] Speed
    [UnityInitialized] public float speed = 10f;
    [UnityInitialized] public bool titanForm;
    [UnityInitialized] public bool useGun;
    [UnityInitialized] public bool canJump = true; // TODO: Can probably remove safely

    // Blades
    [UnityInitialized] public GameObject checkBoxLeft;       // [HERO] LEFT   Blade
    [UnityInitialized] public XWeaponTrail leftbladetrail;   // [HERO] LEFT   Blade Trail 1
    [UnityInitialized] public XWeaponTrail leftbladetrail2;  // [HERO] LEFT   Blade Trail 2
    [UnityInitialized] public GameObject checkBoxRight;      // [HERO] RIGHT  Blade
    [UnityInitialized] public XWeaponTrail rightbladetrail;  // [HERO] RIGHT  Blade Trail 1
    [UnityInitialized] public XWeaponTrail rightbladetrail2; // [HERO] RIGHT  Blade Trail 2
    
    // Other References
    [UnityInitialized] public List<TITAN> myTitans;
    [UnityInitialized] public string currentAnimation;
    [UnityInitialized] public Camera currentCamera;
    [UnityInitialized] public Transform lastHook;
    
    // Skill
    [UnityInitialized] public float skillCDDuration;
    [UnityInitialized] public float skillCDLast;
    
    // Dunno
    [UnityInitialized] public GameObject hookRefL1;
    [UnityInitialized] public GameObject hookRefL2;
    [UnityInitialized] public GameObject hookRefR1;
    [UnityInitialized] public GameObject hookRefR2;
    
    // Constants
    [UnityInitialized] public float jumpHeight = 2f;
    [UnityInitialized] public float gravity = 20f;
    [UnityInitialized] public float myScale = 1f;
    [UnityInitialized] public float maxVelocityChange = 10f;
    
    private HeroState _state;
    
    // Shelter
    private float _spawnTime;
    private bool _instantiated;

    // Flares CDs
    private const float FlareCD = 30f;
    private float _flare1CD;
    private float _flare2CD;
    private float _flare3CD;
    
    // Blades
    private const int MaxBlades = 5;
    private int   _bladesRemaining = 5;       // [HeroStat] BLADE Number of blades remaining
    private float _bladeDuration = 100f;  // [HeroStat] BLADE Duration
    
    // Gas
    private float _gasRemaining = 100f;       // [HeroStat] GAS   Gas remaining
    
    // Bullets
    private const int MaxBullets = 7;      // [AHSS] Max Bullets
    private bool _leftHasBullet = true;    // [AHSS] LEFT  Has bullet
    private int  _shotsRemainingLeft = 7;  // [AHSS] LEFT  Remaining shots
    private bool _rightHasBullet = true;   // [AHSS] RIGHT Has bullet
    private int  _shotsRemainingRight = 7; // [AHSS] RIGHT Remaining shots
    
    private GameObject _horse;     // [HERO] Horse
    private GameObject _nameLabel; // [HERO] Name label
    private bool _areHooksClose;   // [HERO] Both hooks are really close to each other
    private bool _isGrounded;      // [HERO] Is Grounded
    
    
    private GameObject _hooker;    // [HERO] Hooked by -- Type HERO 
    private bool _isHooked = true; // [HERO] Is hooked
    
    // TODO: Convert to enum
    private string _animationStand = "stand"; // [ANIMATION] Stand animation || Can be "AHSS_stand_gun" for AHSS, "stand" for females, "stand_levi" for males
    // TODO: Convert to enum
    private string _animationAttack;          // [ANIMATION] Attack animation || A lot of possibilities, attack1; attack2; attack3_1; special_... etc
    private int attackLoop;                   // [ANIMATION] Number of times to repeat the animation
    private bool attackMove;          // Cannot figure out what this does
    private bool attackReleased;      // Whether the attack animation is still going
    private bool buttonAttackRelease; // Whether the attack button has been released

    // Lean
    private bool needLean;
    private bool leanLeft;

    // Particles
    private ParticleSystem _particleGas;
    private ParticleSystem _particleSparks;

    // Dash
    private float dashTime;        // [DASH] Time
    private Vector3 dashDirection; // [DASH] Position
    private Vector3 dashV;         // [DASH] Direction
    private bool dashU;            // [DASH] Direction: Up
    private bool dashD;            // [DASH] Direction: Down
    private bool dashL;            // [DASH] Direction: Left
    private bool dashR;            // [DASH] Direction: Right
    private float uTapTime = -1f;  // [DASH] something: Up
    private float dTapTime = -1f;  // [DASH] something: Down
    private float lTapTime = -1f;  // [DASH] something: Left
    private float rTapTime = -1f;  // [DASH] something: Right
    

    private bool _holdingLeftHook;
    private bool _holdingRightHook;
    private GameObject _erenTitan;
    private int escapeTimes = 1;
    private float facingDirection;
    private Transform forearmL;
    private Transform forearmR;
    private GameObject gunDummy;
    private Vector3 gunTarget;
    private Transform handL;
    private Transform handR;
    private bool hookSomeOne;
    private GameObject hookTarget;
    private float invincible = 3f; // Might be initialized by Unity
    private bool isLaunchLeft;
    private bool isLaunchRight;
    private bool isLeftHandHooked;
    private bool isMounted;
    private bool isRightHandHooked;
    private float launchElapsedTimeL;
    private float launchElapsedTimeR;
    private Vector3 launchForce;
    private Vector3 launchPointLeft;
    private Vector3 launchPointRight;
    private bool leftArmAim;
    private Quaternion oldHeadRotation;
    private float _preDashVelocity;
    private string reloadAnimation = string.Empty;
    private bool rightArmAim;
    private GameObject skillCD;
    private string skillId;
    private ParticleSystem speedFXPS;
    private Quaternion targetHeadRotation;
    private Quaternion targetRotation;
    private bool throwedBlades;
    private GameObject titanWhoGrabMe;
    private int titanWhoGrabMeID;
    private Transform upperarmL;
    private Transform upperarmR;
    private float useGasSpeed = 0.2f;
    private bool wallJump;
    private float wallRunTime;

    // ===============
    // == RC Fields ==
    // ===============
    
    // Sprite cache
    private Dictionary<string, UISprite> cachedSprites;
    
    // RC Bomb
    private float bombCD;
    private float bombRadius;
    private float bombSpeed;
    private float bombTime;
    private float bombTimeMax; 
    private Vector3 currentV; // Bomb start position 
    private Vector3 targetV;  // Bomb direction
    
    // Base properties
    private Animation baseAnimation;
    private Rigidbody baseRigidBody;
    private Transform baseTransform;
    
    // Cursor
    private GameObject cross1;
    private GameObject cross2;
    
    private bool _isSpeedup; //TODO: Replace with (_speedupTime > 0f)
    private float _speedupTime;       // Sasha skill time
    private bool detonate;            // [Bomb] Check input for pre-explode the bomb
    private bool isCannon;            // Is inside a cannon
    private bool isPhotonCamera;      // ?
    private GameObject LabelDistance; // Label under the reticle
    private GameObject maincamera;    // Main camera?

    private string skillIDHUD;        // Skill ID HUD
    
    // Instances of different objects
    private Bomb myBomb;
    private GameObject myCannon;
    private Transform myCannonBase;
    private Transform myCannonPlayer;
    private CannonPropRegion myCannonRegion;


    private void applyForceToBody(GameObject GO, Vector3 v)
    {
        GO.rigidbody.AddForce(v);
        GO.rigidbody.AddTorque(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
    }

    public void attackAccordingToMouse()
    {
        if (Input.mousePosition.x < Screen.width * 0.5)
            _animationAttack = "attack2";
        else
            _animationAttack = "attack1";
    }

    public void attackAccordingToTarget(Transform a)
    {
        var vector = a.position - transform.position;
        var current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        var f = -Mathf.DeltaAngle(current, transform.rotation.eulerAngles.y - 90f);
        if (Mathf.Abs(f) < 90f && vector.magnitude < 6f && a.position.y <= transform.position.y + 2f && a.position.y >= transform.position.y - 5f)
            _animationAttack = "attack4";
        else if (f > 0f)
            _animationAttack = "attack1";
        else
            _animationAttack = "attack2";
    }

    private void Awake()
    {
        cache();
        setup = gameObject.GetComponent<HERO_SETUP>();
        baseRigidBody.freezeRotation = true;
        baseRigidBody.useGravity = false;
        handL = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
        handR = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
        forearmL = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L");
        forearmR = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
        upperarmL = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
        upperarmR = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
    }

    public void backToHuman()
    {
        gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
        rigidbody.velocity = Vector3.zero;
        titanForm = false;
        ungrabbed();
        falseAttack();
        skillCDDuration = skillCDLast;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer) photonView.RPC(Rpc.SwitchToHuman, PhotonTargets.Others);
    }

    [RPC]
    [UsedImplicitly]
    private void BackToHumanRPC()
    {
        titanForm = false;
        _erenTitan = null;
        gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
    }

    [RPC]
    [UsedImplicitly]
    public void BadGuyReleaseMe()
    {
        _isHooked = false;
        _hooker = null;
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
            var z = 0f;
            needLean = false;
            if (!useGun && State == HeroState.Attack && _animationAttack != "attack3_1" && _animationAttack != "attack3_2")
            {
                var y = rigidbody.velocity.y;
                var x = rigidbody.velocity.x;
                var num4 = rigidbody.velocity.z;
                var num5 = Mathf.Sqrt(x * x + num4 * num4);
                var num6 = Mathf.Atan2(y, num5) * 57.29578f;
                targetRotation = Quaternion.Euler(-num6 * (1f - Vector3.Angle(rigidbody.velocity, transform.forward) / 90f), facingDirection, 0f);
                if (isLeftHandHooked && bulletLeft != null || isRightHandHooked && bulletRight != null) transform.rotation = targetRotation;
            }
            else
            {
                if (isLeftHandHooked && bulletLeft != null && isRightHandHooked && bulletRight != null)
                {
                    if (_areHooksClose)
                    {
                        needLean = true;
                        z = getLeanAngle(bulletRight.transform.position, true);
                    }
                }
                else if (isLeftHandHooked && bulletLeft != null)
                {
                    needLean = true;
                    z = getLeanAngle(bulletLeft.transform.position, true);
                }
                else if (isRightHandHooked && bulletRight != null)
                {
                    needLean = true;
                    z = getLeanAngle(bulletRight.transform.position, false);
                }
                if (needLean)
                {
                    var a = 0f;
                    if (!useGun && State != HeroState.Attack)
                    {
                        a = currentSpeed * 0.1f;
                        a = Mathf.Min(a, 20f);
                    }
                    targetRotation = Quaternion.Euler(-a, facingDirection, z);
                }
                else if (State != HeroState.Attack)
                {
                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                }
            }
        }
    }

    public void bombInit()
    {
        skillIDHUD = skillId;
        skillCDDuration = skillCDLast;
        if (GameManager.settings.IsBombMode)
        {
            var radius = GameManager.settings.BombRadius;
            var range = GameManager.settings.BombRange;
            var speed = GameManager.settings.BombSpeed;
            var countdown = GameManager.settings.BombCountdown;
            if (radius < 0 || radius > 10)
            {
                radius = 5;
                GameManager.settings.BombRadius = 5;
            }
            if (range < 0 || range > 10)
            {
                range = 5;
                GameManager.settings.BombRange = 5;
            }
            if (speed < 0 || speed > 10)
            {
                speed = 5;
                GameManager.settings.BombSpeed = 5;
            }
            if (countdown < 0 || countdown > 10)
            {
                countdown = 5;
                GameManager.settings.BombCountdown = 5;
            }
            if (radius + range + speed + countdown > 20)
            {
                radius = 5;
                range = 5;
                speed = 5;
                countdown = 5;
                GameManager.settings.BombRadius = 5;
                GameManager.settings.BombRange = 5;
                GameManager.settings.BombSpeed = 5;
                GameManager.settings.BombCountdown = 5;
            }
            bombTimeMax = (range * 60f + 200f) / (speed * 60f + 200f);
            bombRadius = radius * 4f + 20f;
            bombCD = countdown * -0.4f + 5f;
            bombSpeed = speed * 60f + 200f;
            var color = GameManager.settings.BombColor;
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.RCBombR, color.r },
                { PlayerProperty.RCBombG, color.g },
                { PlayerProperty.RCBombB, color.b },
                { PlayerProperty.RCBombA, color.a },
                { PlayerProperty.RCBombRadius, bombRadius }
            });
            skillId = "bomb";
            skillIDHUD = "armin";
            skillCDLast = bombCD;
            skillCDDuration = 10f;
            if (GameManager.instance.roundTime > 10f) skillCDDuration = 5f;
        }
    }

    private void BreakApart(Vector3 force, bool isBite)
    {
        GameObject obj6;
        GameObject obj7;
        GameObject obj8;
        GameObject obj9;
        GameObject obj10;
        var obj2 = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), transform.position, transform.rotation);
        obj2.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
        obj2.GetComponent<HERO_SETUP>().isDeadBody = true;
        obj2.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, animation[currentAnimation].normalizedTime, BodyPart.RightArm);
        if (!isBite)
        {
            var gO = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), transform.position, transform.rotation);
            var obj4 = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), transform.position, transform.rotation);
            var obj5 = (GameObject) Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), transform.position, transform.rotation);
            gO.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
            obj4.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
            obj5.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
            gO.GetComponent<HERO_SETUP>().isDeadBody = true;
            obj4.GetComponent<HERO_SETUP>().isDeadBody = true;
            obj5.GetComponent<HERO_SETUP>().isDeadBody = true;
            gO.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, animation[currentAnimation].normalizedTime, BodyPart.Upper);
            obj4.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, animation[currentAnimation].normalizedTime, BodyPart.Lower);
            obj5.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, animation[currentAnimation].normalizedTime, BodyPart.LeftArm);
            applyForceToBody(gO, force);
            applyForceToBody(obj4, force);
            applyForceToBody(obj5, force);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine) currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gO, false);
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj2, false);
        }
        applyForceToBody(obj2, force);
        var t = transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L").transform;
        var transform2 = transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R").transform;
        if (useGun)
        {
            obj6 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_l"), t.position, t.rotation);
            obj7 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_r"), transform2.position, transform2.rotation);
            obj8 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg_2"), transform.position, transform.rotation);
            obj9 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_mag_l"), transform.position, transform.rotation);
            obj10 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_mag_r"), transform.position, transform.rotation);
        }
        else
        {
            obj6 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_l"), t.position, t.rotation);
            obj7 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
            obj8 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg"), transform.position, transform.rotation);
            obj9 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg_gas_l"), transform.position, transform.rotation);
            obj10 = (GameObject) Instantiate(Resources.Load("Character_parts/character_3dmg_gas_r"), transform.position, transform.rotation);
        }
        obj6.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        obj7.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        obj8.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        obj9.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        obj10.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        applyForceToBody(obj6, force);
        applyForceToBody(obj7, force);
        applyForceToBody(obj8, force);
        applyForceToBody(obj9, force);
        applyForceToBody(obj10, force);
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
                    crossFade("run", 0.1f);
                
                _isSpeedup = false;
            }
        }
    }

    public void cache()
    {
        baseTransform = transform;
        baseRigidBody = rigidbody;
        maincamera = GameObject.Find("MainCamera");
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            baseAnimation = animation;
            cross1 = GameObject.Find("cross1");
            cross2 = GameObject.Find("cross2");
            LabelDistance = GameObject.Find("LabelDistance");
            cachedSprites = new Dictionary<string, UISprite>();
            foreach (var o in FindObjectsOfType(typeof(GameObject)))
            {
                var sprite = (GameObject) o;
                if (sprite.GetComponent<UISprite>() != null && sprite.activeInHierarchy)
                {
                    var spriteName = sprite.name;
                    
                    // (name.Contains("blade") || name.Contains("bullet") || name.Contains("gas") || name.Contains("flare") || name.Contains("skill_cd")) && !cachedSprites.ContainsKey(name) )
                    
                    
                    if ((spriteName.Contains("blade") ||
                         spriteName.Contains("bullet") ||
                         spriteName.Contains("gas") || 
                         spriteName.Contains("flare") || 
                         spriteName.Contains("skill_cd")) && !cachedSprites.ContainsKey(spriteName))
                        cachedSprites.Add(spriteName, sprite.GetComponent<UISprite>());
                }
            }
        }
    }


    private void calcFlareCD()
    {
        if (_flare1CD > 0f)
        {
            _flare1CD -= Time.deltaTime;
            if (_flare1CD < 0) _flare1CD = 0;
        }
        if (_flare2CD > 0f)
        {
            _flare2CD -= Time.deltaTime;
            if (_flare2CD < 0) _flare2CD = 0;
        }
        if (_flare3CD > 0f)
        {
            _flare3CD -= Time.deltaTime;
            if (_flare3CD < 0) _flare3CD = 0;
        }
    }

    private void calcSkillCD()
    {
        if (skillCDDuration > 0f)
        {
            skillCDDuration -= Time.deltaTime;
            if (skillCDDuration < 0f) skillCDDuration = 0f;
            
            if (ModuleManager.Enabled(nameof(ModuleNoSkillCD)))
                skillCDDuration = 0;
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2f * jumpHeight * gravity);
    }

    private void changeBlade()
    {
        if (!useGun || _isGrounded || LevelInfoManager.Get(GameManager.Level).Gamemode != GameMode.PvpAHSS)
        {
            State = HeroState.ChangeBlade;
            throwedBlades = false;
            if (useGun)
            {
                if (!_leftHasBullet && !_rightHasBullet)
                {
                    if (_isGrounded)
                        reloadAnimation = "AHSS_gun_reload_both";
                    else
                        reloadAnimation = "AHSS_gun_reload_both_air";
                }
                else if (!_leftHasBullet)
                {
                    if (_isGrounded)
                        reloadAnimation = "AHSS_gun_reload_l";
                    else
                        reloadAnimation = "AHSS_gun_reload_l_air";
                }
                else if (!_rightHasBullet)
                {
                    if (_isGrounded)
                        reloadAnimation = "AHSS_gun_reload_r";
                    else
                        reloadAnimation = "AHSS_gun_reload_r_air";
                }
                else
                {
                    if (_isGrounded)
                        reloadAnimation = "AHSS_gun_reload_both";
                    else
                        reloadAnimation = "AHSS_gun_reload_both_air";
                    _rightHasBullet = false;
                    _leftHasBullet = false;
                }
                crossFade(reloadAnimation, 0.05f);
            }
            else
            {
                if (!_isGrounded)
                    reloadAnimation = "changeBlade_air";
                else
                    reloadAnimation = "changeBlade";
                crossFade(reloadAnimation, 0.1f);
            }
        }
    }

    private void checkDashDoubleTap()
    {
        if (uTapTime >= 0f)
        {
            uTapTime += Time.deltaTime;
            if (uTapTime > 0.2f)
                uTapTime = -1f;
        }
        if (dTapTime >= 0f)
        {
            dTapTime += Time.deltaTime;
            if (dTapTime > 0.2f)
                dTapTime = -1f;
        }
        if (lTapTime >= 0f)
        {
            lTapTime += Time.deltaTime;
            if (lTapTime > 0.2f)
                lTapTime = -1f;
        }
        if (rTapTime >= 0f)
        {
            rTapTime += Time.deltaTime;
            if (rTapTime > 0.2f)
                rTapTime = -1f;
        }
        if (Shelter.InputManager.IsDown(InputAction.Forward))
        {
            if (uTapTime == -1f)
                uTapTime = 0f;
            
            if (uTapTime != 0f)
                dashU = true;
        }
        if (Shelter.InputManager.IsDown(InputAction.Back))
        {
            if (dTapTime == -1f)
                dTapTime = 0f;
            
            if (dTapTime != 0f)
                dashD = true;
        }
        if (Shelter.InputManager.IsDown(InputAction.Left))
        {
            if (lTapTime == -1f)
                lTapTime = 0f;
            
            if (lTapTime != 0f)
                dashL = true;
        }
        if (Shelter.InputManager.IsDown(InputAction.Right))
        {
            if (rTapTime == -1f)
                rTapTime = 0f;
            
            if (rTapTime != 0f)
                dashR = true;
        }
    }

    public void checkTitan()
    {
        int count;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = 1 << LayerMask.NameToLayer("PlayerAttackBox");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask3 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask4 = mask | mask2 | mask3;
        var hitArray = Physics.RaycastAll(ray, 180f, mask4.value);
        var list = new List<RaycastHit>();
        var list2 = new List<TITAN>();
        for (count = 0; count < hitArray.Length; count++)
        {
            var item = hitArray[count];
            list.Add(item);
        }
        list.Sort((x, y) => x.distance.CompareTo(y.distance));
        var num2 = 180f;
        for (count = 0; count < list.Count; count++)
        {
            var hit2 = list[count];
            var go = hit2.collider.gameObject;
            if (go.layer == 16)
            {
                if (go.name.Contains("PlayerDetectorRC") && (hit2 = list[count]).distance < num2)
                {
                    num2 -= 60f;
                    if (num2 <= 60f) count = list.Count;
                    var component = go.transform.root.gameObject.GetComponent<TITAN>();
                    if (component != null) list2.Add(component);
                }
            }
            else
            {
                count = list.Count;
            }
        }
        for (count = 0; count < myTitans.Count; count++)
        {
            var titan2 = myTitans[count];
            if (!list2.Contains(titan2)) titan2.isLook = false;
        }
        for (count = 0; count < list2.Count; count++)
        {
            var titan3 = list2[count];
            titan3.isLook = true;
        }
        myTitans = list2;
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
        
        customAnimationSpeed();
        playAnimation(currentPlayingClipName());
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
            photonView.RPC(Rpc.ContinueAnimation, PhotonTargets.Others);
    }

    public void crossFade(string aniName, float time)
    {
        currentAnimation = aniName;
        animation.CrossFade(aniName, time);
        if (PhotonNetwork.connected && photonView.isMine)
            photonView.RPC(Rpc.CrossFade, PhotonTargets.Others, aniName, time);
    }

    public string currentPlayingClipName()
    {
        foreach (AnimationState current in animation)
            if (current != null && animation.IsPlaying(current.name))
                return current.name;
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
        if (dashTime <= 0f && _gasRemaining > 0f && !isMounted)
        {
            useGas(totalGas * 0.04f);
            facingDirection = getGlobalFacingDirection(horizontal, vertical);
            dashV = getGlobaleFacingVector3(facingDirection);
            _preDashVelocity = currentSpeed;
            var quaternion = Quaternion.Euler(0f, facingDirection, 0f);
            rigidbody.rotation = quaternion;
            targetRotation = quaternion;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                Instantiate(Resources.Load("FX/boost_smoke"), transform.position, transform.rotation);
            else
                PhotonNetwork.Instantiate("FX/boost_smoke", transform.position, transform.rotation, 0);
            dashTime = 0.5f;
            crossFade("dash", 0.1f);
            animation["dash"].time = 0.1f;
            State = HeroState.AirDodge;
            falseAttack();
            rigidbody.AddForce(dashV * 40f, ForceMode.VelocityChange);
        }
    }

    public void die(Vector3 v, bool isBite)
    {
        if (IsInvincible) 
            return;
        
        if (titanForm && _erenTitan != null) _erenTitan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
        if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
        meatDie.Play();
        if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine) && !useGun)
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }
        BreakApart(v, isBite);
        currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
        GameManager.instance.GameLose();
        falseAttack();
        hasDied = true;
        var t = transform.Find("audio_die");
        t.parent = null;
        t.GetComponent<AudioSource>().Play();
        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(transform.position, 0, null, 0.02f);
        Destroy(gameObject);
    }

    public void die2(Transform tf)
    {
        if (IsInvincible) 
            return;

        if (titanForm && _erenTitan != null) _erenTitan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
        if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
        var t = transform.Find("audio_die");
        t.parent = null;
        t.GetComponent<AudioSource>().Play();
        meatDie.Play();
        currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
        currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
        GameManager.instance.GameLose();
        falseAttack();
        hasDied = true;
        var obj2 = (GameObject) Instantiate(Resources.Load("hitMeat2"));
        obj2.transform.position = transform.position;
        Destroy(gameObject);
    }

    private void Dodge(bool offTheWall = false)
    {
        State = HeroState.GroundDodge;
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
                facingDirection = getGlobalFacingDirection(y, x) + 180f;
            else
                facingDirection = currentCamera.transform.rotation.eulerAngles.y + 180f;
            targetRotation = Quaternion.Euler(0f, facingDirection + 180f, 0f);

            crossFade("dodge", 0.1f);
        }
        else
        {
            playAnimation("dodge");
            playAnimationAt("dodge", 0.2f);
        }
        _particleSparks.enableEmission = false;
    }

    private void erenTransform()
    {
        skillCDDuration = skillCDLast;
        if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
        if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            _erenTitan = (GameObject) Instantiate(Resources.Load("TITAN_EREN"), transform.position, transform.rotation);
        else
            _erenTitan = PhotonNetwork.Instantiate("TITAN_EREN", transform.position, transform.rotation, 0);
        _erenTitan.GetComponent<TITAN_EREN>().realBody = gameObject;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(_erenTitan);
        _erenTitan.GetComponent<TITAN_EREN>().born();
        _erenTitan.rigidbody.velocity = rigidbody.velocity;
        rigidbody.velocity = Vector3.zero;
        transform.position = _erenTitan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
        titanForm = true;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer) photonView.RPC(Rpc.SetErenTitanOwner, PhotonTargets.Others, _erenTitan.GetPhotonView().viewID);
        if (_particleGas.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
        {
            var objArray2 = new object[] { false };
            photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, objArray2);
        }
        _particleGas.enableEmission = false;
    }

    public void falseAttack()
    {
        attackMove = false;
        if (useGun)
        {
            if (!attackReleased)
            {
                continueAnimation();
                attackReleased = true;
            }
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                leftbladetrail.StopSmoothly(0.2f);
                rightbladetrail.StopSmoothly(0.2f);
                leftbladetrail2.StopSmoothly(0.2f);
                rightbladetrail2.StopSmoothly(0.2f);
            }
            attackLoop = 0;
            if (!attackReleased)
            {
                continueAnimation();
                attackReleased = true;
            }
        }
    }

    public void fillGas()
    {
        _gasRemaining = totalGas;
    }

    private GameObject findNearestTitan()
    {
        var objArray = GameObject.FindGameObjectsWithTag("titan");
        GameObject obj2 = null;
        var positiveInfinity = float.PositiveInfinity;
        var position = transform.position;
        foreach (var obj3 in objArray)
        {
            var vector2 = obj3.transform.position - position;
            var sqrMagnitude = vector2.sqrMagnitude;
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
        if (titanForm || isCannon || baseRigidBody == null || IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer) 
            return;

        currentSpeed = baseRigidBody.velocity.magnitude;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            if (!baseAnimation.IsPlaying("attack3_2") && !baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra"))
                baseRigidBody.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * 6f);
            
            if (State == HeroState.Grab)
            {
                baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
            }
            else
            {
                var justGrounded = false;
                if (IsGrounded())
                {
                    if (!_isGrounded)
                        justGrounded = true;
                    
                    _isGrounded = true;
                }
                else
                {
                    _isGrounded = false;
                }
                
                if (hookSomeOne)
                {
                    if (hookTarget != null)
                    {
                        var vector2 = hookTarget.transform.position - baseTransform.position;
                        var magnitude = vector2.magnitude;
                        if (magnitude > 2f) baseRigidBody.AddForce(vector2.normalized * Mathf.Pow(magnitude, 0.15f) * 30f - baseRigidBody.velocity * 0.95f, ForceMode.VelocityChange);
                    }
                    else
                    {
                        hookSomeOne = false;
                    }
                }
                else if (_isHooked)
                {
                    if (_hooker != null && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing)
                    {
                        var vector3 = _hooker.transform.position - baseTransform.position;
                        if (vector3.magnitude > 5f)
                            baseRigidBody.AddForce(vector3.normalized * Mathf.Pow(vector3.magnitude, 0.15f) * 0.2f, ForceMode.Impulse);
                    }
                    else
                    {
                        _isHooked = false;
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
                var flag2 = false;
                var flag3 = false;
                var flag4 = false;
                isLeftHandHooked = false;
                isRightHandHooked = false;
                if (isLaunchLeft)
                {
                    if (bulletLeft != null && bulletLeft.GetComponent<Bullet>().isHooked())
                    {
                        isLeftHandHooked = true;
                        var to = bulletLeft.transform.position - baseTransform.position;
                        to.Normalize();
                        to = to * 10f;
                        if (!isLaunchRight) to = to * 2f;
                        if (Vector3.Angle(baseRigidBody.velocity, to) > 90f && Shelter.InputManager.IsKeyPressed(InputAction.Gas))
                        {
                            flag3 = true;
                            flag2 = true;
                        }
                        if (!flag3)
                        {
                            baseRigidBody.AddForce(to);
                            if (Vector3.Angle(baseRigidBody.velocity, to) > 90f) baseRigidBody.AddForce(-baseRigidBody.velocity * 2f, ForceMode.Acceleration);
                        }
                    }
                    launchElapsedTimeL += Time.deltaTime;
                    if (_holdingLeftHook && _gasRemaining > 0f)
                    {
                        useGas(useGasSpeed * Time.deltaTime);
                    }
                    else if (launchElapsedTimeL > 0.3f)
                    {
                        isLaunchLeft = false;
                        if (bulletLeft != null)
                        {
                            bulletLeft.GetComponent<Bullet>().disable();
                            releaseIfIHookSb();
                            bulletLeft = null;
                            flag3 = false;
                        }
                    }
                }
                if (isLaunchRight)
                {
                    if (bulletRight != null && bulletRight.GetComponent<Bullet>().isHooked())
                    {
                        isRightHandHooked = true;
                        var vector5 = bulletRight.transform.position - baseTransform.position;
                        vector5.Normalize();
                        vector5 = vector5 * 10f;
                        if (!isLaunchLeft) vector5 = vector5 * 2f;
                        if (Vector3.Angle(baseRigidBody.velocity, vector5) > 90f && Shelter.InputManager.IsKeyPressed(InputAction.Gas))
                        {
                            flag4 = true;
                            flag2 = true;
                        }
                        if (!flag4)
                        {
                            baseRigidBody.AddForce(vector5);
                            if (Vector3.Angle(baseRigidBody.velocity, vector5) > 90f) baseRigidBody.AddForce(-baseRigidBody.velocity * 2f, ForceMode.Acceleration);
                        }
                    }
                    launchElapsedTimeR += Time.deltaTime;
                    if (_holdingRightHook && _gasRemaining > 0f)
                    {
                        useGas(useGasSpeed * Time.deltaTime);
                    }
                    else if (launchElapsedTimeR > 0.3f)
                    {
                        isLaunchRight = false;
                        if (bulletRight != null)
                        {
                            bulletRight.GetComponent<Bullet>().disable();
                            releaseIfIHookSb();
                            bulletRight = null;
                            flag4 = false;
                        }
                    }
                }
                
                if (_isGrounded)
                {
                    Vector3 vector7;
                    var zero = Vector3.zero;
                    if (State == HeroState.Attack)
                    {
                        if (_animationAttack == "attack5")
                        {
                            if (baseAnimation[_animationAttack].normalizedTime > 0.4f && baseAnimation[_animationAttack].normalizedTime < 0.61f) 
                                baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                        }
                        else if (_animationAttack == "special_petra")
                        {
                            if (baseAnimation[_animationAttack].normalizedTime > 0.35f && baseAnimation[_animationAttack].normalizedTime < 0.48f) 
                                baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                        }
                        else if (baseAnimation.IsPlaying("attack3_2"))
                        {
                            zero = Vector3.zero;
                        }
                        else if (baseAnimation.IsPlaying("attack1") || baseAnimation.IsPlaying("attack2"))
                        {
                            baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                        }
                        if (baseAnimation.IsPlaying("attack3_2")) zero = Vector3.zero;
                    }
                    if (justGrounded)
                    {
                        if (State != HeroState.Attack || _animationAttack != "attack3_1" && _animationAttack != "attack5" && _animationAttack != "special_petra")
                        {
                            if (State != HeroState.Attack && x == 0f && z == 0f && bulletLeft == null && bulletRight == null && State != HeroState.FillGas)
                            {
                                State = HeroState.Land;
                                crossFade("dash_land", 0.01f);
                            }
                            else
                            {
                                buttonAttackRelease = true;
                                if (State != HeroState.Attack && baseRigidBody.velocity.x * baseRigidBody.velocity.x + baseRigidBody.velocity.z * baseRigidBody.velocity.z > speed * speed * 1.5f && State != HeroState.FillGas)
                                {
                                    State = HeroState.Slide;
                                    crossFade("slide", 0.05f);
                                    facingDirection = Mathf.Atan2(baseRigidBody.velocity.x, baseRigidBody.velocity.z) * 57.29578f;
                                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                                    _particleSparks.enableEmission = true;
                                }
                            }
                        }
                        zero = baseRigidBody.velocity;
                    }
                    if (State == HeroState.Attack && _animationAttack == "attack3_1" && baseAnimation[_animationAttack].normalizedTime >= 1f)
                    {
                        playAnimation("attack3_2");
                        resetAnimationSpeed();
                        vector7 = Vector3.zero;
                        baseRigidBody.velocity = vector7;
                        zero = vector7;
                        currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f);
                    }
                    if (State == HeroState.GroundDodge)
                    {
                        if (baseAnimation["dodge"].normalizedTime >= 0.2f && baseAnimation["dodge"].normalizedTime < 0.8f) zero = -baseTransform.forward * 2.4f * speed;
                        if (baseAnimation["dodge"].normalizedTime > 0.8f) zero = baseRigidBody.velocity * 0.9f;
                    }
                    else if (State == HeroState.Idle)
                    {
                        var vector8 = new Vector3(x, 0f, z);
                        var resultAngle = getGlobalFacingDirection(x, z);
                        zero = getGlobaleFacingVector3(resultAngle);
                        var num6 = vector8.magnitude <= 0.95f ? vector8.magnitude >= 0.25f ? vector8.magnitude : 0f : 1f;
                        zero = zero * num6;
                        zero = zero * speed;
                        if (_speedupTime > 0f && _isSpeedup)
                            zero = zero * 4f;
                        
                        if (x != 0f || z != 0f)
                        {
                            if (!baseAnimation.IsPlaying("run") && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("run_sasha") && (!baseAnimation.IsPlaying("horse_geton") || baseAnimation["horse_geton"].normalizedTime >= 0.5f))
                            {
                                if (_speedupTime > 0f && _isSpeedup)
                                    crossFade("run_sasha", 0.1f);
                                else
                                    crossFade("run", 0.1f);
                            }
                        }
                        else
                        {
                            if (!(baseAnimation.IsPlaying(_animationStand) || State == HeroState.Land || baseAnimation.IsPlaying("jump") || baseAnimation.IsPlaying("horse_geton") || baseAnimation.IsPlaying("grabbed")))
                            {
                                crossFade(_animationStand, 0.1f);
                                zero = zero * 0f;
                            }
                            resultAngle = -874f;
                        }
                        if (resultAngle != -874f)
                        {
                            facingDirection = resultAngle;
                            targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                        }
                    }
                    else if (State == HeroState.Land)
                    {
                        zero = baseRigidBody.velocity * 0.96f;
                    }
                    else if (State == HeroState.Slide)
                    {
                        zero = baseRigidBody.velocity * 0.99f;
                        if (currentSpeed < speed * 1.2f)
                        {
                            idle();
                            _particleSparks.enableEmission = false;
                        }
                    }
                    var velocity = baseRigidBody.velocity;
                    var force = zero - velocity;
                    force.x = Mathf.Clamp(force.x, -maxVelocityChange, maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -maxVelocityChange, maxVelocityChange);
                    force.y = 0f;
                    if (baseAnimation.IsPlaying("jump") && baseAnimation["jump"].normalizedTime > 0.18f) force.y += 8f;
                    if (baseAnimation.IsPlaying("horse_geton") && baseAnimation["horse_geton"].normalizedTime > 0.18f && baseAnimation["horse_geton"].normalizedTime < 1f)
                    {
                        var num7 = 6f;
                        force = -baseRigidBody.velocity;
                        force.y = num7;
                        var num8 = Vector3.Distance(_horse.transform.position, baseTransform.position);
                        var num9 = 0.6f * gravity * num8 / 12f;
                        vector7 = _horse.transform.position - baseTransform.position;
                        force += num9 * vector7.normalized;
                    }
                    if (!(State == HeroState.Attack && useGun))
                    {
                        baseRigidBody.AddForce(force, ForceMode.VelocityChange);
                        baseRigidBody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
                    }
                }
                else
                {
                    if (_particleSparks.enableEmission) _particleSparks.enableEmission = false;
                    if (_horse != null && 
                        (baseAnimation.IsPlaying("horse_geton") || baseAnimation.IsPlaying("air_fall")) && 
                        baseRigidBody.velocity.y < 0f && 
                        Vector3.Distance(_horse.transform.position + Vector3.up * 1.65f, baseTransform.position) < 0.5f)
                    {
                        baseTransform.position = _horse.transform.position + Vector3.up * 1.65f;
                        baseTransform.rotation = _horse.transform.rotation;
                        isMounted = true;
                        crossFade("horse_idle", 0.1f);
                        _horse.GetComponent<Horse>().mounted();
                    }
                    if (!((State != HeroState.Idle || baseAnimation.IsPlaying("dash") || baseAnimation.IsPlaying("wallrun") || baseAnimation.IsPlaying("toRoof") || baseAnimation.IsPlaying("horse_geton") || baseAnimation.IsPlaying("horse_getoff") || baseAnimation.IsPlaying("air_release") || isMounted || baseAnimation.IsPlaying("air_hook_l_just") && baseAnimation["air_hook_l_just"].normalizedTime < 1f || baseAnimation.IsPlaying("air_hook_r_just") && baseAnimation["air_hook_r_just"].normalizedTime < 1f) && baseAnimation["dash"].normalizedTime < 0.99f))
                    {
                        if (!isLeftHandHooked && !isRightHandHooked && (baseAnimation.IsPlaying("air_hook_l") || baseAnimation.IsPlaying("air_hook_r") || baseAnimation.IsPlaying("air_hook")) && baseRigidBody.velocity.y > 20f)
                        {
                            baseAnimation.CrossFade("air_release");
                        }
                        else
                        {
                            var isMoving = Mathf.Abs(baseRigidBody.velocity.x) + Mathf.Abs(baseRigidBody.velocity.z) > 25f;
                            var isFalling = baseRigidBody.velocity.y < 0f;
                            if (!isMoving)
                            {
                                if (isFalling)
                                {
                                    if (!baseAnimation.IsPlaying("air_fall")) crossFade("air_fall", 0.2f);
                                }
                                else if (!baseAnimation.IsPlaying("air_rise"))
                                {
                                    crossFade("air_rise", 0.2f);
                                }
                            }
                            else if (!isLeftHandHooked && !isRightHandHooked)
                            {
                                var current = -Mathf.Atan2(baseRigidBody.velocity.z, baseRigidBody.velocity.x) * 57.29578f;
                                var num11 = -Mathf.DeltaAngle(current, baseTransform.rotation.eulerAngles.y - 90f);
                                if (Mathf.Abs(num11) < 45f)
                                {
                                    if (!baseAnimation.IsPlaying("air2")) crossFade("air2", 0.2f);
                                }
                                else if (num11 < 135f && num11 > 0f)
                                {
                                    if (!baseAnimation.IsPlaying("air2_right")) crossFade("air2_right", 0.2f);
                                }
                                else if (num11 > -135f && num11 < 0f)
                                {
                                    if (!baseAnimation.IsPlaying("air2_left")) crossFade("air2_left", 0.2f);
                                }
                                else if (!baseAnimation.IsPlaying("air2_backward"))
                                {
                                    crossFade("air2_backward", 0.2f);
                                }
                            }
                            else if (useGun)
                            {
                                if (!isRightHandHooked)
                                {
                                    if (!baseAnimation.IsPlaying("AHSS_hook_forward_l")) crossFade("AHSS_hook_forward_l", 0.1f);
                                }
                                else if (!isLeftHandHooked)
                                {
                                    if (!baseAnimation.IsPlaying("AHSS_hook_forward_r")) crossFade("AHSS_hook_forward_r", 0.1f);
                                }
                                else if (!baseAnimation.IsPlaying("AHSS_hook_forward_both"))
                                {
                                    crossFade("AHSS_hook_forward_both", 0.1f);
                                }
                            }
                            else if (!isRightHandHooked)
                            {
                                if (!baseAnimation.IsPlaying("air_hook_l")) crossFade("air_hook_l", 0.1f);
                            }
                            else if (!isLeftHandHooked)
                            {
                                if (!baseAnimation.IsPlaying("air_hook_r")) crossFade("air_hook_r", 0.1f);
                            }
                            else if (!baseAnimation.IsPlaying("air_hook"))
                            {
                                crossFade("air_hook", 0.1f);
                            }
                        }
                    }
                    if (State == HeroState.Idle && baseAnimation.IsPlaying("air_release") && baseAnimation["air_release"].normalizedTime >= 1f) crossFade("air_rise", 0.2f);
                    if (baseAnimation.IsPlaying("horse_getoff") && baseAnimation["horse_getoff"].normalizedTime >= 1f) crossFade("air_rise", 0.2f);
                    if (baseAnimation.IsPlaying("toRoof"))
                    {
                        if (baseAnimation["toRoof"].normalizedTime < 0.22f)
                        {
                            baseRigidBody.velocity = Vector3.zero;
                            baseRigidBody.AddForce(new Vector3(0f, gravity * baseRigidBody.mass, 0f));
                        }
                        else
                        {
                            if (!wallJump)
                            {
                                wallJump = true;
                                baseRigidBody.AddForce(Vector3.up * 8f, ForceMode.Impulse);
                            }
                            baseRigidBody.AddForce(baseTransform.forward * 0.05f, ForceMode.Impulse);
                        }
                        if (baseAnimation["toRoof"].normalizedTime >= 1f) playAnimation("air_rise");
                    }
                    else if (!(State != HeroState.Idle || !isPressDirectionTowardsHero(x, z) || Shelter.InputManager.IsKeyPressed(InputAction.Gas) || Shelter.InputManager.IsKeyPressed(InputAction.LeftHook) || Shelter.InputManager.IsKeyPressed(InputAction.RightHook) || Shelter.InputManager.IsKeyPressed(InputAction.BothHooks) || !IsFrontGrounded() || baseAnimation.IsPlaying("wallrun") || baseAnimation.IsPlaying("dodge")))
                    {
                        crossFade("wallrun", 0.1f);
                        wallRunTime = 0f;
                    }
                    else if (baseAnimation.IsPlaying("wallrun"))
                    {
                        baseRigidBody.AddForce(Vector3.up * speed - baseRigidBody.velocity, ForceMode.VelocityChange);
                        wallRunTime += Time.deltaTime;
                        if (wallRunTime > 1f || z == 0f && x == 0f)
                        {
                            baseRigidBody.AddForce(-baseTransform.forward * speed * 0.75f, ForceMode.Impulse);
                            Dodge(true);
                        }
                        else if (!IsUpFrontGrounded())
                        {
                            wallJump = false;
                            crossFade("toRoof", 0.1f);
                        }
                        else if (!IsFrontGrounded())
                        {
                            crossFade("air_fall", 0.1f);
                        }
                    }
                    else if (!baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra") && !baseAnimation.IsPlaying("dash") && !baseAnimation.IsPlaying("jump"))
                    {
                        var vector11 = new Vector3(x, 0f, z);
                        var num12 = getGlobalFacingDirection(x, z);
                        var vector12 = getGlobaleFacingVector3(num12);
                        var num13 = vector11.magnitude <= 0.95f ? vector11.magnitude >= 0.25f ? vector11.magnitude : 0f : 1f;
                        vector12 = vector12 * num13;
                        vector12 = vector12 * (setup.myCostume.stat.Acceleration / 10f * 2f);
                        if (x == 0f && z == 0f)
                        {
                            if (State == HeroState.Attack) vector12 = vector12 * 0f;
                            num12 = -874f;
                        }
                        if (num12 != -874f)
                        {
                            facingDirection = num12;
                            targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                        }
                        if (!flag3 && !flag4 && !isMounted && Shelter.InputManager.IsKeyPressed(InputAction.Gas) && _gasRemaining > 0f)
                        {
                            if (x != 0f || z != 0f)
                                baseRigidBody.AddForce(vector12, ForceMode.Acceleration);
                            else
                                baseRigidBody.AddForce(baseTransform.forward * vector12.magnitude, ForceMode.Acceleration);
                            flag2 = true;
                        }
                    }
                    if (baseAnimation.IsPlaying("air_fall") && currentSpeed < 0.2f && IsFrontGrounded()) crossFade("onWall", 0.3f);
                }
                if (flag3 && flag4)
                {
                    var num14 = currentSpeed + 0.1f;
                    baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
                    var vector13 = (bulletRight.transform.position + bulletLeft.transform.position) * 0.5f - baseTransform.position;
                    
                    float num15;
                    if (Shelter.InputManager.IsKeyPressed(InputAction.ReelIn))
                        num15 = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.ReelOut))
                        num15 = 1f;
                    else
                        num15 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    
                    num15 = Mathf.Clamp(num15, -0.8f, 0.8f);
                    var num16 = 1f + num15;
                    var vector14 = Vector3.RotateTowards(vector13, baseRigidBody.velocity, 1.53938f * num16, 1.53938f * num16);
                    vector14.Normalize();
                    baseRigidBody.velocity = vector14 * num14;
                }
                else if (flag3)
                {
                    var num17 = currentSpeed + 0.1f;
                    baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
                    var vector15 = bulletLeft.transform.position - baseTransform.position;
                    
                    float num18;
                    if (Shelter.InputManager.IsKeyPressed(InputAction.ReelIn))
                        num18 = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.ReelOut))
                        num18 = 1f;
                    else
                        num18 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    
                    num18 = Mathf.Clamp(num18, -0.8f, 0.8f);
                    var num19 = 1f + num18;
                    var vector16 = Vector3.RotateTowards(vector15, baseRigidBody.velocity, 1.53938f * num19, 1.53938f * num19);
                    vector16.Normalize();
                    baseRigidBody.velocity = vector16 * num17;
                }
                else if (flag4)
                {
                    var num20 = currentSpeed + 0.1f;
                    baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
                    var vector17 = bulletRight.transform.position - baseTransform.position;
                    var num21 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    if (Shelter.InputManager.IsKeyPressed(InputAction.ReelIn))
                        num21 = -1f;
                    else if (Shelter.InputManager.IsKeyPressed(InputAction.ReelOut))
                        num21 = 1f;
                        
                    num21 = Mathf.Clamp(num21, -0.8f, 0.8f);
                    var num22 = 1f + num21;
                    var vector18 = Vector3.RotateTowards(vector17, baseRigidBody.velocity, 1.53938f * num22, 1.53938f * num22);
                    vector18.Normalize();
                    baseRigidBody.velocity = vector18 * num20;
                }
                
                // Probably handles the knockback (maybe only AHSS?)
                if (State == HeroState.Attack && (_animationAttack == "attack5" || _animationAttack == "special_petra") && baseAnimation[_animationAttack].normalizedTime > 0.4f && !attackMove)
                {
                    attackMove = true;
                    if (launchPointRight.magnitude > 0f)
                    {
                        var vector19 = launchPointRight - baseTransform.position;
                        vector19.Normalize();
                        vector19 = vector19 * 13f;
                        baseRigidBody.AddForce(vector19, ForceMode.Impulse);
                    }
                    if (_animationAttack == "special_petra" && launchPointLeft.magnitude > 0f)
                    {
                        var vector20 = launchPointLeft - baseTransform.position;
                        vector20.Normalize();
                        vector20 = vector20 * 13f;
                        baseRigidBody.AddForce(vector20, ForceMode.Impulse);
                        if (bulletRight != null)
                        {
                            bulletRight.GetComponent<Bullet>().disable();
                            releaseIfIHookSb();
                        }
                        if (bulletLeft != null)
                        {
                            bulletLeft.GetComponent<Bullet>().disable();
                            releaseIfIHookSb();
                        }
                    }
                    baseRigidBody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                }
                if (bulletLeft != null || bulletRight != null)
                {
                    if (bulletLeft != null && bulletLeft.transform.position.y > gameObject.transform.position.y && isLaunchLeft && bulletLeft.GetComponent<Bullet>().isHooked())
                        baseRigidBody.AddForce(new Vector3(0f, -10f * baseRigidBody.mass, 0f));
                    else if (bulletRight != null && bulletRight.transform.position.y > gameObject.transform.position.y && isLaunchRight && bulletRight.GetComponent<Bullet>().isHooked()) baseRigidBody.AddForce(new Vector3(0f, -10f * baseRigidBody.mass, 0f));
                }
                else
                {
                    baseRigidBody.AddForce(new Vector3(0f, -gravity * baseRigidBody.mass, 0f));
                }
                if (currentSpeed > 10f)
                    currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, currentSpeed + 40f), 0.1f);
                else
                    currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
                if (flag2)
                {
                    useGas(useGasSpeed * Time.deltaTime);
                    if (!_particleGas.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine) photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, true);
                    _particleGas.enableEmission = true;
                }
                else
                {
                    if (_particleGas.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
                    {
                        var objArray3 = new object[] { false };
                        photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, objArray3);
                    }
                    _particleGas.enableEmission = false;
                }
                if (currentSpeed > 80f)
                {
                    if (!speedFXPS.enableEmission) speedFXPS.enableEmission = true;
                    speedFXPS.startSpeed = currentSpeed;
                    speedFX.transform.LookAt(baseTransform.position + baseRigidBody.velocity);
                }
                else if (speedFXPS.enableEmission)
                {
                    speedFXPS.enableEmission = false;
                }
            }
        }
    }

    private Vector3 getGlobaleFacingVector3(float resultAngle)
    {
        var num = -resultAngle + 90f;
        var x = Mathf.Cos(num * 0.01745329f);
        return new Vector3(x, 0f, Mathf.Sin(num * 0.01745329f));
    }

    private Vector3 getGlobaleFacingVector3(float horizontal, float vertical)
    {
        var num = -getGlobalFacingDirection(horizontal, vertical) + 90f;
        var x = Mathf.Cos(num * 0.01745329f);
        return new Vector3(x, 0f, Mathf.Sin(num * 0.01745329f));
    }

    private float getGlobalFacingDirection(float horizontal, float vertical)
    {
        if (vertical == 0f && horizontal == 0f) return transform.rotation.eulerAngles.y;
        var y = currentCamera.transform.rotation.eulerAngles.y;
        var num2 = Mathf.Atan2(vertical, horizontal) * 57.29578f;
        num2 = -num2 + 90f;
        return y + num2;
    }

    private float getLeanAngle(Vector3 p, bool left)
    {
        if (!useGun && State == HeroState.Attack) return 0f;
        var num = p.y - transform.position.y;
        var num2 = Vector3.Distance(p, transform.position);
        var a = Mathf.Acos(num / num2) * 57.29578f;
        a *= 0.1f;
        a *= 1f + Mathf.Pow(rigidbody.velocity.magnitude, 0.2f);
        var vector3 = p - transform.position;
        var current = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
        var target = Mathf.Atan2(rigidbody.velocity.x, rigidbody.velocity.z) * 57.29578f;
        var num6 = Mathf.DeltaAngle(current, target);
        a += Mathf.Abs(num6 * 0.5f);
        if (State != HeroState.Attack) a = Mathf.Min(a, 80f);
        if (num6 > 0f)
            leanLeft = true;
        else
            leanLeft = false;
        if (useGun) return a * (num6 >= 0f ? 1 : -1);
        var num7 = 0f;
        if (left && num6 < 0f || !left && num6 > 0f)
            num7 = 0.1f;
        else
            num7 = 0.5f;
        return a * (num6 >= 0f ? num7 : -num7);
    }

    private void getOffHorse()
    {
        playAnimation("horse_getoff");
        rigidbody.AddForce(Vector3.up * 10f - transform.forward * 2f - transform.right * 1f, ForceMode.VelocityChange);
        unmounted();
    }

    private void getOnHorse()
    {
        playAnimation("horse_geton");
        facingDirection = _horse.transform.rotation.eulerAngles.y;
        targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
    }

    public void getSupply()
    {
        if ((animation.IsPlaying(_animationStand) || animation.IsPlaying("run") || animation.IsPlaying("run_sasha")) && (_bladeDuration != totalBladeSta || _bladesRemaining != MaxBlades || _gasRemaining != totalGas || _shotsRemainingLeft != MaxBullets || _shotsRemainingRight != MaxBullets))
        {
            State = HeroState.FillGas;
            crossFade("supply", 0.1f);
        }
    }

    public void grabbed(GameObject titan, bool leftHand)
    {
        if (isMounted) unmounted();
        State = HeroState.Grab;
        GetComponent<CapsuleCollider>().isTrigger = true;
        falseAttack();
        titanWhoGrabMe = titan;
        if (titanForm && _erenTitan != null) _erenTitan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (!useGun && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }
        _particleGas.enableEmission = false;
        _particleSparks.enableEmission = false;
    }

    /// <returns>True if <see cref="hasDied"/> or <see cref="IsInvincible"/></returns>
    public bool HasDied()
    {
        return hasDied || IsInvincible;
    }

    private void headMovement()
    {
        var t = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
        var transform2 = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck");
        var x = Mathf.Sqrt((gunTarget.x - transform.position.x) * (gunTarget.x - transform.position.x) + (gunTarget.z - transform.position.z) * (gunTarget.z - transform.position.z));
        targetHeadRotation = t.rotation;
        var vector5 = gunTarget - transform.position;
        var current = -Mathf.Atan2(vector5.z, vector5.x) * 57.29578f;
        var num3 = -Mathf.DeltaAngle(current, transform.rotation.eulerAngles.y - 90f);
        num3 = Mathf.Clamp(num3, -40f, 40f);
        var y = transform2.position.y - gunTarget.y;
        var num5 = Mathf.Atan2(y, x) * 57.29578f;
        num5 = Mathf.Clamp(num5, -40f, 30f);
        targetHeadRotation = Quaternion.Euler(t.rotation.eulerAngles.x + num5, t.rotation.eulerAngles.y + num3, t.rotation.eulerAngles.z);
        oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 60f);
        t.rotation = oldHeadRotation;
    }

    public void hookedByHuman(int hooker, Vector3 hookPosition)
    {
        photonView.RPC(Rpc.HookedByHuman, photonView.owner, hooker, hookPosition);
    }

    [RPC]
    [UsedImplicitly]
    public void HookFail()
    {
        hookTarget = null;
        hookSomeOne = false;
    }

    public void hookToHuman(GameObject target, Vector3 hookPosition)
    {
        releaseIfIHookSb();
        hookTarget = target;
        hookSomeOne = true;
        if (target.GetComponent<HERO>() != null) target.GetComponent<HERO>().hookedByHuman(photonView.viewID, hookPosition);
        launchForce = hookPosition - transform.position;
        var num = Mathf.Pow(launchForce.magnitude, 0.1f);
        if (_isGrounded) rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
        rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
    }

    private void idle()
    {
        if (State == HeroState.Attack) falseAttack();
        State = HeroState.Idle;
        crossFade(_animationStand, 0.1f);
    }

    private bool IsFrontGrounded()
    {
        LayerMask ground = 1 << LayerMask.NameToLayer("Ground");
        LayerMask player = 1 << LayerMask.NameToLayer("EnemyBox");
        return Physics.Raycast(gameObject.transform.position + gameObject.transform.up * 1f, gameObject.transform.forward, 1f, ground | player);
    }

    private bool IsGrounded()
    {
        LayerMask ground = 1 << LayerMask.NameToLayer("Ground");
        LayerMask player = 1 << LayerMask.NameToLayer("EnemyBox");
        return Physics.Raycast(gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, ground | player);
    }

    public bool IsInvincible => invincible > 0f;

    private bool isPressDirectionTowardsHero(float h, float v)
    {
        if (h == 0f && v == 0f) return false;
        return Mathf.Abs(Mathf.DeltaAngle(getGlobalFacingDirection(h, v), transform.rotation.eulerAngles.y)) < 45f;
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
            _nameLabel.transform.localScale = new Vector3(14, 14, 14);
            return;
        } 

        var dist = Vector3.Distance(Player.Self.Hero.transform.position, transform.position);
        var size = 14 + -8 * Mathf.Clamp01(dist / 300);
        
        _nameLabel.transform.localScale = new Vector3(size, size, size);
    }

    public void DoLateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && _nameLabel != null)
        {
            UpdateNameScale();
            
            if (titanForm && _erenTitan != null)
                _nameLabel.transform.localPosition = Vector3.up * Screen.height * 1.5f;

            var start = new Vector3(baseTransform.position.x, baseTransform.position.y + 2f, baseTransform.position.z);
            LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
            LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
            if (Vector3.Angle(maincamera.transform.forward, start - maincamera.transform.position) > 90f || Physics.Linecast(start, maincamera.transform.position, mask2 | mask))
            {
                _nameLabel.transform.localPosition = Vector3.up * Screen.height * 1.5f;
            }
            else
            {
                Vector2 vector2 = maincamera.GetComponent<Camera>().WorldToScreenPoint(start);
                _nameLabel.transform.localPosition = new Vector3((int)(vector2.x - Screen.width * 0.5f), (int)(vector2.y - Screen.height * 0.5f), 0f);
            }
        }
        if (!titanForm && !isCannon)
        {
            if (ModuleManager.Enabled(nameof(ModuleCameraTilt)) && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
            {
                Quaternion quaternion2;
                var zero = Vector3.zero;
                var position = Vector3.zero;
                if (isLaunchLeft && bulletLeft != null && bulletLeft.GetComponent<Bullet>().isHooked()) zero = bulletLeft.transform.position;
                if (isLaunchRight && bulletRight != null && bulletRight.GetComponent<Bullet>().isHooked()) position = bulletRight.transform.position;
                var vector5 = Vector3.zero;
                if (zero.magnitude != 0f && position.magnitude == 0f)
                    vector5 = zero;
                else if (zero.magnitude == 0f && position.magnitude != 0f)
                    vector5 = position;
                else if (zero.magnitude != 0f && position.magnitude != 0f) vector5 = (zero + position) * 0.5f;
                var from = Vector3.Project(vector5 - baseTransform.position, maincamera.transform.up);
                var vector7 = Vector3.Project(vector5 - baseTransform.position, maincamera.transform.right);
                if (vector5.magnitude > 0f)
                {
                    var to = from + vector7;
                    var num = Vector3.Angle(vector5 - baseTransform.position, baseRigidBody.velocity) * 0.005f;
                    var vector9 = maincamera.transform.right + vector7.normalized;
                    quaternion2 = Quaternion.Euler(maincamera.transform.rotation.eulerAngles.x, maincamera.transform.rotation.eulerAngles.y, vector9.magnitude >= 1f ? -Vector3.Angle(@from, to) * num : Vector3.Angle(@from, to) * num);
                }
                else
                {
                    quaternion2 = Quaternion.Euler(maincamera.transform.rotation.eulerAngles.x, maincamera.transform.rotation.eulerAngles.y, 0f);
                }
                maincamera.transform.rotation = Quaternion.Lerp(maincamera.transform.rotation, quaternion2, Time.deltaTime * 2f);
            }
            if (State == HeroState.Grab && titanWhoGrabMe != null)
            {
                if (titanWhoGrabMe.GetComponent<TITAN>() != null)
                {
                    baseTransform.position = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
                    baseTransform.rotation = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
                }
                else if (titanWhoGrabMe.GetComponent<FEMALE_TITAN>() != null)
                {
                    baseTransform.position = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
                    baseTransform.rotation = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
                }
            }
            if (useGun)
            {
                if (leftArmAim || rightArmAim)
                {
                    var vector10 = gunTarget - baseTransform.position;
                    var current = -Mathf.Atan2(vector10.z, vector10.x) * 57.29578f;
                    var num3 = -Mathf.DeltaAngle(current, baseTransform.rotation.eulerAngles.y - 90f);
                    headMovement();
                    if (!isLeftHandHooked && leftArmAim && num3 < 40f && num3 > -90f) leftArmAimTo(gunTarget);
                    if (!isRightHandHooked && rightArmAim && num3 > -40f && num3 < 90f) rightArmAimTo(gunTarget);
                }
                else if (!_isGrounded)
                {
                    handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                }
                if (isLeftHandHooked && bulletLeft != null) leftArmAimTo(bulletLeft.transform.position);
                if (isRightHandHooked && bulletRight != null) rightArmAimTo(bulletRight.transform.position);
            }
            setHookedPplDirection();
            bodyLean();
        }
    }

    public void launch(Vector3 des, bool left = true, bool leviMode = false)
    {
        if (isMounted) unmounted();
        if (State != HeroState.Attack) idle();
        var vector = des - transform.position;
        if (left)
            launchPointLeft = des;
        else
            launchPointRight = des;
        vector.Normalize();
        vector = vector * 20f;
        if (bulletLeft != null && bulletRight != null && bulletLeft.GetComponent<Bullet>().isHooked() && bulletRight.GetComponent<Bullet>().isHooked()) vector = vector * 0.8f;
        if (!animation.IsPlaying("attack5") && !animation.IsPlaying("special_petra"))
            leviMode = false;
        else
            leviMode = true;
        if (!leviMode)
        {
            falseAttack();
            idle();
            if (useGun)
            {
                crossFade("AHSS_hook_forward_both", 0.1f);
            }
            else if (left && !isRightHandHooked)
            {
                crossFade("air_hook_l_just", 0.1f);
            }
            else if (!left && !isLeftHandHooked)
            {
                crossFade("air_hook_r_just", 0.1f);
            }
            else
            {
                crossFade("dash", 0.1f);
                animation["dash"].time = 0f;
            }
        }
        if (left) isLaunchLeft = true;
        if (!left) isLaunchRight = true;
        launchForce = vector;
        if (!leviMode)
        {
            if (vector.y < 30f) launchForce += Vector3.up * (30f - vector.y);
            if (des.y >= transform.position.y) launchForce += Vector3.up * (des.y - transform.position.y) * 10f;
            rigidbody.AddForce(launchForce);
        }
        facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
        var quaternion = Quaternion.Euler(0f, facingDirection, 0f);
        gameObject.transform.rotation = quaternion;
        rigidbody.rotation = quaternion;
        targetRotation = quaternion;
        if (left)
            launchElapsedTimeL = 0f;
        else
            launchElapsedTimeR = 0f;
        if (leviMode) launchElapsedTimeR = -100f;
        if (animation.IsPlaying("special_petra"))
        {
            launchElapsedTimeR = -100f;
            launchElapsedTimeL = -100f;
            if (bulletRight != null)
            {
                bulletRight.GetComponent<Bullet>().disable();
                releaseIfIHookSb();
            }
            if (bulletLeft != null)
            {
                bulletLeft.GetComponent<Bullet>().disable();
                releaseIfIHookSb();
            }
        }
        _particleSparks.enableEmission = false;
    }

    private void launchLeftRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (_gasRemaining != 0f)
        {
            useGas();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                bulletLeft = (GameObject) Instantiate(Resources.Load("hook"));
            else if (photonView.isMine) bulletLeft = PhotonNetwork.Instantiate("hook", transform.position, transform.rotation, 0);
            var obj2 = !useGun ? hookRefL1 : hookRefL2;
            var str = !useGun ? "hookRefL1" : "hookRefL2";
            bulletLeft.transform.position = obj2.transform.position;
            var component = bulletLeft.GetComponent<Bullet>();
            var num = !single ? hit.distance <= 50f ? hit.distance * 0.05f : hit.distance * 0.3f : 0f;
            var vector = hit.point - transform.right * num - bulletLeft.transform.position;
            vector.Normalize();
            if (mode == 1)
                component.launch(vector * 3f, rigidbody.velocity, str, true, gameObject, true);
            else
                component.launch(vector * 3f, rigidbody.velocity, str, true, gameObject);
            launchPointLeft = Vector3.zero;
        }
    }

    private void launchRightRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (_gasRemaining != 0f)
        {
            useGas();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                bulletRight = (GameObject) Instantiate(Resources.Load("hook"));
            else if (photonView.isMine) bulletRight = PhotonNetwork.Instantiate("hook", transform.position, transform.rotation, 0);
            var obj2 = !useGun ? hookRefR1 : hookRefR2;
            var str = !useGun ? "hookRefR1" : "hookRefR2";
            bulletRight.transform.position = obj2.transform.position;
            var component = bulletRight.GetComponent<Bullet>();
            var num = !single ? hit.distance <= 50f ? hit.distance * 0.05f : hit.distance * 0.3f : 0f;
            var vector = hit.point + transform.right * num - bulletRight.transform.position;
            vector.Normalize();
            if (mode == 1)
                component.launch(vector * 5f, rigidbody.velocity, str, false, gameObject, true);
            else
                component.launch(vector * 3f, rigidbody.velocity, str, false, gameObject);
            launchPointRight = Vector3.zero;
        }
    }

    private void leftArmAimTo(Vector3 target)
    {
        var y = target.x - upperarmL.transform.position.x;
        var num2 = target.y - upperarmL.transform.position.y;
        var x = target.z - upperarmL.transform.position.z;
        var num4 = Mathf.Sqrt(y * y + x * x);
        handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
        forearmL.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        upperarmL.rotation = Quaternion.Euler(0f, 90f + Mathf.Atan2(y, x) * 57.29578f, -Mathf.Atan2(num2, num4) * 57.29578f);
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
                var skins = GameManager.settings.HumanSkin.Set;
                var url = new StringBuilder(skins.Length * 45);
                foreach (var skin in skins)
                    url.AppendFormat("{0},", skin);
                url.Remove(url.Length - 2, 1);
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    StartCoroutine(loadskinE(-1, url.ToString()));
                }
                else
                {
                    var viewID = -1;
                    if (_horse != null) viewID = _horse.GetPhotonView().viewID;
                    photonView.RPC(Rpc.LoadSkin, PhotonTargets.AllBuffered, viewID, url.ToString());
                }
            }
        }
    }

    public IEnumerator LoadSkin(int horseViewID, HumanSkin skin)
    {
        while (!_instantiated)
            yield return null;
        
        
    }

    public IEnumerator loadskinE(int horse, string url)
    {
        while (!_instantiated)
            yield return null;
        
        var unloadAssets = false;
        var mipmap = GameManager.settings.UseMipmap;
        var urls = url.Split(',');
        if (urls.Length < 13)
            yield break; // Not allowed exception?
        var skinGas = ModuleManager.Enabled(nameof(ModuleEnableSkins));
        var hasHorse = LevelInfoManager.Get(GameManager.Level).Horse || GameManager.settings.EnableHorse;
        var isMe = IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine;

        Renderer myRenderer;
        if (setup.part_hair_1 != null) // urls[1]
        {
            myRenderer = setup.part_hair_1.renderer;
            if (Utility.IsValidImageUrl(urls[1]))
            {
                if (!GameManager.linkHash[0].ContainsKey(urls[1]))
                {
                    unloadAssets = true;
                    if (setup.myCostume.hairInfo.ID >= 0)
                        myRenderer.material = CharacterMaterials.materials[setup.myCostume.hairInfo.Texture];
                    
                    using (var www = new WWW(urls[1]))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    }
                    GameManager.linkHash[0].Add(urls[1], myRenderer.material);
                    myRenderer.material = (Material)GameManager.linkHash[0][urls[1]];
                }
                else
                {
                    myRenderer.material = (Material)GameManager.linkHash[0][urls[1]];
                }
            }
            else if (urls[1].EqualsIgnoreCase("transparent"))
            {
                myRenderer.enabled = false;
            }
        }
        if (setup.part_cape != null) // urls[7]
        {
            myRenderer = setup.part_cape.renderer;
            if (Utility.IsValidImageUrl(urls[7]))
            {
                if (!GameManager.linkHash[0].ContainsKey(urls[7]))
                {
                    unloadAssets = true;
                    GameManager.linkHash[0].Add(urls[7], myRenderer.material);
                    using (var www = new WWW(urls[7]))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    }
                    myRenderer.material = (Material)GameManager.linkHash[0][urls[7]];
                }
                else
                {
                    myRenderer.material = (Material)GameManager.linkHash[0][urls[7]];
                }
            }
            else if (urls[7].EqualsIgnoreCase("transparent"))
            {
                myRenderer.enabled = false;
            }
        }
        if (setup.part_chest_3 != null) // urls[6]
        {
            myRenderer = setup.part_chest_3.renderer;
            if (Utility.IsValidImageUrl(urls[6]))
            {
                if (!GameManager.linkHash[1].ContainsKey(urls[6]))
                {
                    unloadAssets = true;
                    using (var www = new WWW(urls[6]))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        myRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                    }

                    GameManager.linkHash[1].Add(urls[6], myRenderer.material);
                    myRenderer.material = (Material) GameManager.linkHash[1][urls[6]];
                }
                else
                {
                    myRenderer.material = (Material) GameManager.linkHash[1][urls[6]];
                }
            }
            else if (urls[6].EqualsIgnoreCase("transparent"))
            {
                myRenderer.enabled = false;
            }
        }

        /*
         * 0  Horse
         * 1  Hair
         * 2  Eye
         * 3  Glass
         * 4  Face
         * 5  Body
         * 6  Costume
         * 7  Cape
         * 8  Blade, 3dmg, Gun (left)
         * 9  Blade, 3dmg_gas, Gun (right)
         * 10 Gas
         * 11 Hoodie
         * 12 Trail
         */
        
        foreach (var currentRender in GetComponentsInChildren<Renderer>())
            if (currentRender.name.Contains("hair")) // url[1]
            {
                if (Utility.IsValidImageUrl(urls[1]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[1]))
                    {
                        unloadAssets = true;
                        if (setup.myCostume.hairInfo.ID >= 0)
                            currentRender.material = CharacterMaterials.materials[setup.myCostume.hairInfo.Texture];
                        
                        using (var www = new WWW(urls[1]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[1], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[1]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[1]];
                    }
                }
                else if (urls[1].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_eye")) // url[2]
            {
                if (Utility.IsValidImageUrl(urls[2]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[2]))
                    {
                        unloadAssets = true;
                        currentRender.material.mainTextureScale = currentRender.material.mainTextureScale * 8f;
                        currentRender.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (var www = new WWW(urls[2]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[2], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[2]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[2]];
                    }
                }
                else if (urls[2].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("glass")) // url[3]
            {
                if (Utility.IsValidImageUrl(urls[3]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[3]))
                    {
                        unloadAssets = true;
                        currentRender.material.mainTextureScale = currentRender.material.mainTextureScale * 8f;
                        currentRender.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (var www = new WWW(urls[3]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[3], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[3]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[3]];
                    }
                }
                else if (urls[3].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_face")) // url[4]
            {
                if (Utility.IsValidImageUrl(urls[4]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[4]))
                    {
                        unloadAssets = true;
                        currentRender.material.mainTextureScale = currentRender.material.mainTextureScale * 8f;
                        currentRender.material.mainTextureOffset = new Vector2(0f, 0f);
                        using (var www = new WWW(urls[4]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[4], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[4]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[4]];
                    }
                }
                else if (urls[4].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_head") || 
                     currentRender.name.Contains("character_hand") || 
                     currentRender.name.Contains("character_chest")) // url[5]
            {
                if (Utility.IsValidImageUrl(urls[5]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[5]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[5]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[5], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[5]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[5]];
                    }
                }
                else if (urls[5].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_body") || 
                     currentRender.name.Contains("character_arm") || 
                     currentRender.name.Contains("character_leg") || 
                     currentRender.name.Contains("mikasa_asset")) // url[6]
            {
                if (Utility.IsValidImageUrl(urls[6]))
                {
                    if (!GameManager.linkHash[1].ContainsKey(urls[6]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[6]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                        }
                        GameManager.linkHash[1].Add(urls[6], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[1][urls[6]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[1][urls[6]];
                    }
                }
                else if (urls[6].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_cape") || 
                     currentRender.name.Contains("character_brand")) // url[7]
            {
                if (Utility.IsValidImageUrl(urls[7]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[7]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[7]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[7], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[7]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[7]];
                    }
                }
                else if (urls[7].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_blade_l") || 
                     (currentRender.name.Contains("character_3dmg") || 
                      currentRender.name.Contains("character_gun")) && 
                     !currentRender.name.Contains("_r")) // url[8]
            {
                if (Utility.IsValidImageUrl(urls[8]))
                {
                    if (!GameManager.linkHash[1].ContainsKey(urls[8]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[8]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                        }
                        GameManager.linkHash[1].Add(urls[8], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[1][urls[8]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[1][urls[8]];
                    }
                }
                else if (urls[8].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_blade_r") || 
                     currentRender.name.Contains("character_3dmg_gas_r") || 
                     currentRender.name.Contains("character_gun") && 
                     currentRender.name.Contains("_r")) // url[9]
            {
                if (Utility.IsValidImageUrl(urls[9]))
                {
                    if (!GameManager.linkHash[1].ContainsKey(urls[9]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[9]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                        }
                        GameManager.linkHash[1].Add(urls[9], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[1][urls[9]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[1][urls[9]];
                    }
                }
                else if (urls[9].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name == "3dmg_smoke" && skinGas) // url[10]
            {
                if (Utility.IsValidImageUrl(urls[10]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[10]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[10]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[10], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[10]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[10]];
                    }
                }
                else if (urls[10].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }
            else if (currentRender.name.Contains("character_cap_")) // url[11]
            {
                if (Utility.IsValidImageUrl(urls[11]))
                {
                    if (!GameManager.linkHash[0].ContainsKey(urls[11]))
                    {
                        unloadAssets = true;
                        using (var www = new WWW(urls[11]))
                        {
                            yield return www;
                            if (www.error != null)
                                yield break;
                            currentRender.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                        }
                        GameManager.linkHash[0].Add(urls[11], currentRender.material);
                        currentRender.material = (Material)GameManager.linkHash[0][urls[11]];
                    }
                    else
                    {
                        currentRender.material = (Material)GameManager.linkHash[0][urls[11]];
                    }
                }
                else if (urls[11].EqualsIgnoreCase("transparent"))
                {
                    currentRender.enabled = false;
                }
            }

        if (hasHorse && horse >= 0)
        {
            var go = PhotonView.Find(horse).gameObject;
            if (go != null)
                foreach (var currentRenderer in go.GetComponentsInChildren<Renderer>())
                    if (currentRenderer.name.Contains("HORSE")) // url[0]
                    {
                        if (Utility.IsValidImageUrl(urls[0]))
                        {
                            if (!GameManager.linkHash[1].ContainsKey(urls[0]))
                            {
                                unloadAssets = true;
                                using (var www = new WWW(urls[0]))
                                {
                                    yield return www;
                                    if (www.error != null)
                                        yield break;
                                    currentRenderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 500000);
                                }
                                GameManager.linkHash[1].Add(urls[0], currentRenderer.material);
                                currentRenderer.material = (Material)GameManager.linkHash[1][urls[0]];
                            }
                            else
                            {
                                currentRenderer.material = (Material)GameManager.linkHash[1][urls[0]];
                            }
                        }
                        else if (urls[0].EqualsIgnoreCase("transparent"))
                        {
                            currentRenderer.enabled = false;
                        }
                    }
        }
        if (isMe && Utility.IsValidImageUrl(urls[12]))
        {
            if (!GameManager.linkHash[0].ContainsKey(urls[12]))
            {
                unloadAssets = true;
                using (var www = new WWW(urls[12]))
                {
                    yield return www;
                    if (www.error != null)
                        yield break;
                    leftbladetrail.MyMaterial.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    rightbladetrail.MyMaterial.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                }
                GameManager.linkHash[0].Add(urls[12], leftbladetrail.MyMaterial);
                leftbladetrail.MyMaterial = (Material)GameManager.linkHash[0][urls[12]];
                rightbladetrail.MyMaterial = (Material)GameManager.linkHash[0][urls[12]];
                leftbladetrail2.MyMaterial = leftbladetrail.MyMaterial;
                rightbladetrail2.MyMaterial = leftbladetrail.MyMaterial;
            }
            else
            {
                leftbladetrail2.MyMaterial = (Material)GameManager.linkHash[0][urls[12]];
                rightbladetrail2.MyMaterial = (Material)GameManager.linkHash[0][urls[12]];
                leftbladetrail.MyMaterial = (Material)GameManager.linkHash[0][urls[12]];
                rightbladetrail.MyMaterial = (Material)GameManager.linkHash[0][urls[12]];
            }
        }
        if (unloadAssets)
            GameManager.instance.UnloadAssets();
    }

    [RPC]
    [UsedImplicitly]
    public void LoadskinRPC(int horseViewID, string url, PhotonMessageInfo info)
    {
        if (!ModuleManager.Enabled(nameof(ModuleEnableSkins))) 
            return;

        if (!HumanSkin.TryParse(url, out var skin))
        {
            Shelter.Log("Invalid hero skin from {0}.", LogType.Warning, info.sender);
            return;
        }
        
        StartCoroutine(loadskinE(horseViewID, url));
    }

    public void markDie()
    {
        hasDied = true;
        State = HeroState.Die;
    }

    [RPC]
    [UsedImplicitly]
    public void MoveToRPC(float x, float y, float z, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(MoveToRPC), info);

        if (ModuleManager.Enabled(nameof(ModuleDisableTP)))
            return;
        
        transform.position = new Vector3(x, y, z);
    }

    [RPC]
    [UsedImplicitly]
    private void Net3DMGSMOKE(bool ifON)
    {
        if (_particleGas != null) _particleGas.enableEmission = ifON;
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
        playAnimation(currentPlayingClipName());
    }

    [RPC]
    [UsedImplicitly]
    private void NetCrossFade(string aniName, float time)
    {
        currentAnimation = aniName;
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
                    Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (viewID < 0)
                {
                    if (titanName == "")
                        Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + " (possibly valid).");
                    else
                        Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (PhotonView.Find(viewID) == null)
                {
                    Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
                {
                    Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
            }
        }
        if (PhotonNetwork.isMasterClient)
        {
            onDeathEvent(viewID, killByTitan);
            var id = photonView.owner.ID;
            if (GameManager.heroHash.ContainsKey(id)) GameManager.heroHash.Remove(id);
        }
        if (photonView.isMine)
        {
            var vector = Vector3.up * 5000f;
            if (myBomb != null) myBomb.destroyMe();
            if (myCannon != null) PhotonNetwork.Destroy(myCannon);
            if (titanForm && _erenTitan != null) _erenTitan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            if (skillCD != null) skillCD.transform.localPosition = vector;
        }
        if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
        if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
        meatDie.Play();
        if (!(useGun || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }
        falseAttack();
        BreakApart(force, isBite);
        if (photonView.isMine)
        {
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameManager.instance.myRespawnTime = 0f;
        }
        hasDied = true;
        var audioTransform = transform.Find("audio_die");
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
            GameManager.instance.photonView.RPC(Rpc.OnPlayerDeath, PhotonTargets.MasterClient, titanName != string.Empty ? 1 : 0);
            if (viewID != -1)
            {
                var view2 = PhotonView.Find(viewID);
                if (view2 != null)
                {
                    GameManager.instance.SendKillInfo(killByTitan, "[FFC000][" + info.sender.ID + "][FFFFFF]" + view2.owner.Properties.Name, false, Player.Self.Properties.Name);
                    view2.owner.SetCustomProperties(new Hashtable
                    {
                        {PlayerProperty.Kills, view2.owner.Properties.Kills + 1}
                    });
                }
            }
            else
            {
                GameManager.instance.SendKillInfo(titanName != string.Empty, "[FFC000][" + info.sender.ID + "][FFFFFF]" + titanName, false, Player.Self.Properties.Name);
            }
        }
        if (photonView.isMine) PhotonNetwork.Destroy(photonView);
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
                photonView.RPC(Rpc.SwitchToHuman, PhotonTargets.Others);
                return;
            }
            if (!info.sender.IsLocal && !info.sender.IsMasterClient)
            {
                if (info.sender.Properties.Name == null || info.sender.Properties.PlayerType == PlayerType.Unknown)
                {
                    Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (viewID < 0)
                {
                    if (titanName == "")
                        Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + " (possibly valid).");
                    else if (!GameManager.settings.IsBombMode && GameManager.settings.AllowCannonHumanKills) Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (PhotonView.Find(viewID) == null)
                {
                    Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
                else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
                {
                    Shelter.Chat.System("Unusual Kill from ID " + info.sender.ID + "");
                }
            }
        }
        if (photonView.isMine)
        {
            var vector = Vector3.up * 5000f;
            if (myBomb != null) myBomb.destroyMe();
            if (myCannon != null) PhotonNetwork.Destroy(myCannon);
            PhotonNetwork.RemoveRPCs(photonView);
            if (titanForm && _erenTitan != null) _erenTitan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            if (skillCD != null) skillCD.transform.localPosition = vector;
        }
        meatDie.Play();
        if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
        if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
        var t = transform.Find("audio_die");
        t.parent = null;
        t.GetComponent<AudioSource>().Play();
        if (photonView.isMine)
        {
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameManager.instance.myRespawnTime = 0f;
        }
        falseAttack();
        hasDied = true;
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
                var view2 = PhotonView.Find(viewID);
                if (view2 != null)
                {
                    GameManager.instance.SendKillInfo(true, "[FFC000][" + info?.sender.ID + "][FFFFFF]" + view2.owner.Properties.Name, false, Player.Self.Properties.Name);
                    view2.owner.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Kills, view2.owner.Properties.Kills + 1 }
                    });
                }
            }
            else
            {
                GameManager.instance.SendKillInfo(true, "[FFC000][" + info?.sender.ID + "][FFFFFF]" + titanName, false, Player.Self.Properties.Name);
            }
            GameManager.instance.photonView.RPC(Rpc.OnPlayerDeath, PhotonTargets.MasterClient, titanName != string.Empty ? 1 : 0);
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine)
            obj2 = PhotonNetwork.Instantiate("hitMeat2", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
        else
            obj2 = (GameObject) Instantiate(Resources.Load("hitMeat2"));
        obj2.transform.position = transform.position;
        if (photonView.isMine) PhotonNetwork.Destroy(photonView);
        if (PhotonNetwork.isMasterClient)
        {
            onDeathEvent(viewID, true);
            var iD = photonView.owner.ID;
            if (GameManager.heroHash.ContainsKey(iD)) GameManager.heroHash.Remove(iD);
        }
    }

    public void netDieLocal(Vector3 force, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
    {
        if (photonView.isMine)
        {
            var vector = Vector3.up * 5000f;
            if (titanForm && _erenTitan != null) _erenTitan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            if (myBomb != null) myBomb.destroyMe();
            if (myCannon != null) PhotonNetwork.Destroy(myCannon);
            if (skillCD != null) skillCD.transform.localPosition = vector;
        }
        if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
        if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
        meatDie.Play();
        if (!(useGun || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }
        falseAttack();
        BreakApart(force, isBite);
        if (photonView.isMine)
        {
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameManager.instance.myRespawnTime = 0f;
        }
        hasDied = true;
        var t = transform.Find("audio_die");
        t.parent = null;
        t.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        if (photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Dead, true },
                { PlayerProperty.Deaths, Player.Self.Properties.Deaths + 1 }
            });
            GameManager.instance.photonView.RPC(Rpc.OnPlayerDeath, PhotonTargets.MasterClient, titanName != string.Empty ? 1 : 0);
            if (viewID != -1)
            {
                var view = PhotonView.Find(viewID);
                if (view != null)
                {
                    GameManager.instance.SendKillInfo(killByTitan, view.owner.Properties.Name, false, Player.Self.Properties.Name);
                    view.owner.SetCustomProperties(new Hashtable
                    {
                        { PlayerProperty.Kills, view.owner.Properties.Kills + 1 }
                    });
                }
            }
            else
            {
                GameManager.instance.SendKillInfo(titanName != string.Empty, titanName, false, Player.Self.Properties.Name);
            }
        }
        if (photonView.isMine) PhotonNetwork.Destroy(photonView);
        if (PhotonNetwork.isMasterClient)
        {
            onDeathEvent(viewID, killByTitan);
            var iD = photonView.owner.ID;
            if (GameManager.heroHash.ContainsKey(iD)) GameManager.heroHash.Remove(iD);
        }
    }

    [RPC]
    [UsedImplicitly]
    private void NetGrabbed(int id, bool leftHand)
    {
        titanWhoGrabMeID = id;
        grabbed(PhotonView.Find(id).gameObject, leftHand);
    }

    [RPC]
    [UsedImplicitly]
    private void NetlaughAttack()
    {
        foreach (var obj2 in GameObject.FindGameObjectsWithTag("titan"))
            if (Vector3.Distance(obj2.transform.position, transform.position) < 50f && Vector3.Angle(obj2.transform.forward, transform.position - obj2.transform.position) < 90f && obj2.GetComponent<TITAN>() != null) obj2.GetComponent<TITAN>().StartLaughing();
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
        currentAnimation = aniName;
        if (animation != null) animation.Play(aniName);
    }

    [RPC]
    [UsedImplicitly]
    private void NetPlayAnimationAt(string aniName, float normalizedTime)
    {
        currentAnimation = aniName;
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
        State = HeroState.Idle;
    }

    [RPC]
    [UsedImplicitly]
    private void NetTauntAttack(float tauntTime, float distance = 100f)
    {
        foreach (var obj2 in GameObject.FindGameObjectsWithTag("titan"))
            if (Vector3.Distance(obj2.transform.position, transform.position) < distance && obj2.GetComponent<TITAN>() != null) obj2.GetComponent<TITAN>().GetTaunted(gameObject, tauntTime);
    }

    [RPC]
    [UsedImplicitly]
    private void NetUngrabbed()
    {
        ungrabbed();
        NetPlayAnimation(_animationStand);
        falseAttack();
    }

    public void onDeathEvent(int viewID, bool isTitan)
    {
        RCEvent event2;
        string[] strArray;
        if (isTitan)
        {
            if (GameManager.RCEvents.ContainsKey("OnPlayerDieByTitan"))
            {
                event2 = (RCEvent) GameManager.RCEvents["OnPlayerDieByTitan"];
                strArray = (string[]) GameManager.RCVariableNames["OnPlayerDieByTitan"];
                if (GameManager.playerVariables.ContainsKey(strArray[0]))
                    GameManager.playerVariables[strArray[0]] = photonView.owner;
                else
                    GameManager.playerVariables.Add(strArray[0], photonView.owner);
                if (GameManager.titanVariables.ContainsKey(strArray[1]))
                    GameManager.titanVariables[strArray[1]] = PhotonView.Find(viewID).gameObject.GetComponent<TITAN>();
                else
                    GameManager.titanVariables.Add(strArray[1], PhotonView.Find(viewID).gameObject.GetComponent<TITAN>());
                event2.checkEvent();
            }
        }
        else if (GameManager.RCEvents.ContainsKey("OnPlayerDieByPlayer"))
        {
            event2 = (RCEvent) GameManager.RCEvents["OnPlayerDieByPlayer"];
            strArray = (string[]) GameManager.RCVariableNames["OnPlayerDieByPlayer"];
            if (GameManager.playerVariables.ContainsKey(strArray[0]))
                GameManager.playerVariables[strArray[0]] = photonView.owner;
            else
                GameManager.playerVariables.Add(strArray[0], photonView.owner);
            if (GameManager.playerVariables.ContainsKey(strArray[1]))
                GameManager.playerVariables[strArray[1]] = PhotonView.Find(viewID).owner;
            else
                GameManager.playerVariables.Add(strArray[1], PhotonView.Find(viewID).owner);
            event2.checkEvent();
        }
    }

    private void OnDestroy()
    {
        if (_nameLabel != null) Destroy(_nameLabel);
        if (gunDummy != null) Destroy(gunDummy);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer) releaseIfIHookSb();
        if (GameObject.Find("MultiplayerManager") != null) GameManager.instance.Heroes.Remove(this);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            var vector = Vector3.up * 5000f;
            cross1.transform.localPosition = vector;
            cross2.transform.localPosition = vector;
            LabelDistance.transform.localPosition = vector;
        }
        if (setup.part_cape != null) ClothFactory.DisposeObject(setup.part_cape);
        if (setup.part_hair_1 != null) ClothFactory.DisposeObject(setup.part_hair_1);
        if (setup.part_hair_2 != null) ClothFactory.DisposeObject(setup.part_hair_2);
    }

    public void pauseAnimation()
    {
        foreach (AnimationState current in animation)
            if (current != null)
                current.speed = 0f;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine) photonView.RPC(Rpc.PauseAnimation, PhotonTargets.Others);
    }

    public void playAnimation(string aniName)
    {
        currentAnimation = aniName;
        animation.Play(aniName);
        if (PhotonNetwork.connected && photonView.isMine) photonView.RPC(Rpc.PlayAnimation, PhotonTargets.Others, aniName);
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        currentAnimation = aniName;
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
        if (PhotonNetwork.connected && photonView.isMine) photonView.RPC(Rpc.PlayAnimationAt, PhotonTargets.Others, aniName, normalizedTime);
    }

    private void releaseIfIHookSb()
    {
        if (hookSomeOne && hookTarget != null)
        {
            hookTarget.GetPhotonView().RPC(Rpc.ReleasePlayerHook, hookTarget.GetPhotonView().owner);
            hookTarget = null;
            hookSomeOne = false;
        }
    }

    public IEnumerator reloadSky()
    {
        yield return new WaitForSeconds(0.5f);
        if (GameManager.skyMaterial != null && Camera.main.GetComponent<Skybox>().material != GameManager.skyMaterial) Camera.main.GetComponent<Skybox>().material = GameManager.skyMaterial;
    }

    public void resetAnimationSpeed()
    {
        foreach (AnimationState current in animation)
            if (current != null)
                current.speed = 1f;
        customAnimationSpeed();
    }

    [RPC]
    [UsedImplicitly]
    public void ReturnFromCannon(PhotonMessageInfo info)
    {
        if (info.sender == photonView.owner)
        {
            isCannon = false;
            gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
        }
    }

    private void rightArmAimTo(Vector3 target)
    {
        var y = target.x - upperarmR.transform.position.x;
        var num2 = target.y - upperarmR.transform.position.y;
        var x = target.z - upperarmR.transform.position.z;
        var num4 = Mathf.Sqrt(y * y + x * x);
        handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        forearmR.localRotation = Quaternion.Euler(90f, 0f, 0f);
        upperarmR.rotation = Quaternion.Euler(180f, 90f + Mathf.Atan2(y, x) * 57.29578f, Mathf.Atan2(num2, num4) * 57.29578f);
    }

    [RPC]
    [UsedImplicitly]
    private void RPCHookedByHuman(int hooker, Vector3 hookPosition, PhotonMessageInfo info)
    {
        if (!PhotonView.TryParse(hooker, out var view))
            throw new NotAllowedException(nameof(RPCHookedByHuman), info, false);
        
        if (Vector3.Distance(hookPosition, transform.position) < 15f && IN_GAME_MAIN_CAMERA.GameMode == GameMode.Racing)
        {
            _hooker = view.gameObject;
            _isHooked = true;
            
            launchForce = PhotonView.Find(hooker).gameObject.transform.position - transform.position;
            rigidbody.AddForce(-rigidbody.velocity * 0.9f, ForceMode.VelocityChange);
            var num = Mathf.Pow(launchForce.magnitude, 0.1f);
            if (_isGrounded)
                rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
            
            rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
            if (State != HeroState.Grab)
            {
                dashTime = 1f;
                crossFade("dash", 0.05f);
                animation["dash"].time = 0.1f;
                State = HeroState.AirDodge;
                falseAttack();
                facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
                var quaternion = Quaternion.Euler(0f, facingDirection, 0f);
                gameObject.transform.rotation = quaternion;
                rigidbody.rotation = quaternion;
                targetRotation = quaternion;
            }
        }
        else
        {
            _isHooked = false;
            _hooker = null;
            view.RPC(Rpc.HookFailed, view.owner);
        }
    }

    private void salute()
    {
        State = HeroState.Salute;
        crossFade("salute", 0.1f);
    }

    private void setHookedPplDirection()
    {
        _areHooksClose = false;
        if (isRightHandHooked && isLeftHandHooked)
        {
            if (bulletLeft != null && bulletRight != null)
            {
                var normal = bulletLeft.transform.position - bulletRight.transform.position;
                if (normal.sqrMagnitude < 4f)
                {
                    var vector2 = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f - transform.position;
                    facingDirection = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
                    if (useGun && State != HeroState.Attack)
                    {
                        var current = -Mathf.Atan2(rigidbody.velocity.z, rigidbody.velocity.x) * 57.29578f;
                        var target = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        var num3 = -Mathf.DeltaAngle(current, target);
                        facingDirection += num3;
                    }
                    _areHooksClose = true;
                }
                else
                {
                    var to = transform.position - bulletLeft.transform.position;
                    var vector6 = transform.position - bulletRight.transform.position;
                    var vector7 = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f;
                    var from = transform.position - vector7;
                    if (Vector3.Angle(@from, to) < 30f && Vector3.Angle(@from, vector6) < 30f)
                    {
                        _areHooksClose = true;
                        var vector9 = vector7 - transform.position;
                        facingDirection = Mathf.Atan2(vector9.x, vector9.z) * 57.29578f;
                    }
                    else
                    {
                        _areHooksClose = false;
                        var forward = transform.forward;
                        Vector3.OrthoNormalize(ref normal, ref forward);
                        facingDirection = Mathf.Atan2(forward.x, forward.z) * 57.29578f;
                        var num4 = Mathf.Atan2(to.x, to.z) * 57.29578f;
                        if (Mathf.DeltaAngle(num4, facingDirection) > 0f) facingDirection += 180f;
                    }
                }
            }
        }
        else
        {
            _areHooksClose = true;
            var zero = Vector3.zero;
            if (isRightHandHooked && bulletRight != null)
            {
                zero = bulletRight.transform.position - transform.position;
            }
            else
            {
                if (!isLeftHandHooked || bulletLeft == null) return;
                zero = bulletLeft.transform.position - transform.position;
            }
            facingDirection = Mathf.Atan2(zero.x, zero.z) * 57.29578f;
            if (State != HeroState.Attack)
            {
                var num6 = -Mathf.Atan2(rigidbody.velocity.z, rigidbody.velocity.x) * 57.29578f;
                var num7 = -Mathf.Atan2(zero.z, zero.x) * 57.29578f;
                var num8 = -Mathf.DeltaAngle(num6, num7);
                if (useGun)
                {
                    facingDirection += num8;
                }
                else
                {
                    var num9 = 0f;
                    if (isLeftHandHooked && num8 < 0f || isRightHandHooked && num8 > 0f)
                        num9 = -0.1f;
                    else
                        num9 = 0.1f;
                    facingDirection += num8 * num9;
                }
            }
        }
    }

    public void ExitCannon()
    {
        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject);
        isCannon = false;
        myCannonRegion = null;
        photonView.RPC(Rpc.ReturnFromCannon, PhotonTargets.Others);
    }

    [RPC]
    [UsedImplicitly]
    public void SetMyCannon(int viewID, PhotonMessageInfo info)
    {
        if (info.sender == photonView.owner)
        {
            var view = PhotonView.Find(viewID);
            if (view != null)
            {
                myCannon = view.gameObject;
                if (myCannon != null)
                {
                    myCannonBase = myCannon.transform;
                    myCannonPlayer = myCannonBase.Find("PlayerPoint");
                    isCannon = true;
                }
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void SetMyPhotonCamera(float offset, PhotonMessageInfo info)
    {
        if (photonView.owner == info.sender) // Should this be local-only?
        {
//            CameraMultiplier = offset;
            GetComponent<SmoothSyncMovement>().PhotonCamera = true; // This might have effect on others too
            isPhotonCamera = true;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void SetMyTeam(int val)
    {
        myTeam = val;
        TriggerColliderWeapon colliderWeapon;
        if (checkBoxLeft != null && (colliderWeapon = checkBoxLeft.GetComponent<TriggerColliderWeapon>()) != null)
            colliderWeapon.myTeam = val;
        if (checkBoxRight != null && (colliderWeapon = checkBoxRight.GetComponent<TriggerColliderWeapon>()) != null)
            colliderWeapon.myTeam = val;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            if (GameManager.settings.IsFriendlyMode)
            {
                if (val != 1)
                    photonView.RPC(Rpc.SetTeam, PhotonTargets.AllBuffered, 1);
            }
            else
            {
                switch (GameManager.settings.PVPMode)
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
    }

    public void SetSkillHUDPosition()
    {
        skillCD = GameObject.Find("skill_cd_" + skillIDHUD);
        if (skillCD != null)
        {
            skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
            if (useGun && !GameManager.settings.IsBombMode)
                skillCD.transform.localPosition = Vector3.up * 5000f;
        }
    }

    private void SetRacingUI()
    {
        switch (IN_GAME_MAIN_CAMERA.GameMode)
        {
            case GameMode.Racing when ModuleManager.Enabled(nameof(ModuleRacingInterface)):
                GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0, 4000, 0);
                skillCD = GameObject.Find("skill_cd_" + skillIDHUD);
                
                GameObject.Find("GasUI").transform.localPosition = new Vector3(0, 4000, 0);
                GameObject.Find("gasR").transform.position = new Vector3(Screen.width / 2f - 50, -Screen.height / 2f + 50, 0);
                GameObject.Find("gasR1").transform.position = new Vector3(Screen.width / 2f - 50, -Screen.height / 2f + 50, 0);
                break;
            
            default:
                GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, -Screen.height * 0.5f + 10f, 0f);
                skillCD = GameObject.Find("skill_cd_" + skillIDHUD);
                skillCD.transform.localPosition = new Vector3(0f, -Screen.height * 0.5f + 10f, 0f);
                GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                break;
        }
    }

    public void setStat2()
    {
        skillCDLast = 1.5f;
        customAnimationSpeed();
        skillId = setup.myCostume.stat.SkillName;
        switch (skillId)
        {
            case "levi":
                skillCDLast = 3.5f;
                break;
            case "armin":
                skillCDLast = 5f;
                break;
            case "marco":
                skillCDLast = 10f;
                break;
            case "jean":
                skillCDLast = 0.001f;
                break;
            case "eren":
                skillCDLast = 240;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    var level = LevelInfoManager.Get(GameManager.Level);
                    if (level.PlayerTitansNotAllowed || 
                        level.Gamemode == GameMode.Racing || 
                        level.Gamemode == GameMode.PvpCapture || 
                        level.Gamemode == GameMode.Trost)
                    {
                        skillId = "petra";
                        skillCDLast = 1f;
                    }
                }

                break;
            case "sasha":
                skillCDLast = 20f;
                break;
            case "petra":
                skillCDLast = 3.5f;
                break;
        }

        bombInit();
        speed = setup.myCostume.stat.Speed / 10f;
        totalGas = _gasRemaining = setup.myCostume.stat.Gas;
        totalBladeSta = _bladeDuration = setup.myCostume.stat.Blade;
        baseRigidBody.mass = 0.5f - (setup.myCostume.stat.Acceleration - 100) * 0.001f;
        
        SetRacingUI();
        
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            GameObject.Find("bulletL").GetComponent<UISprite>().enabled = false;
            GameObject.Find("bulletR").GetComponent<UISprite>().enabled = false;
            for (var i = 1; i <= 7; i++)
            {
                GameObject.Find($"bulletL{i}").GetComponent<UISprite>().enabled = false;
                GameObject.Find($"bulletR{i}").GetComponent<UISprite>().enabled = false;
            }
        }
        if (setup.myCostume.uniform_type == UniformType.CasualAHSS)
        {
            _animationStand = "AHSS_stand_gun";
            useGun = true;
            gunDummy = new GameObject("gunDummy");
            gunDummy.transform.position = baseTransform.position;
            gunDummy.transform.rotation = baseTransform.rotation;
            myGroup = Group.AHSS;
            setTeam2(2);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                GameObject.Find("bladeCL").GetComponent<UISprite>().enabled = false;
                GameObject.Find("bladeCR").GetComponent<UISprite>().enabled = false;
                for (var i = 1; i <= 5; i++)
                {
                    GameObject.Find($"bladel{i}").GetComponent<UISprite>().enabled = false;
                    GameObject.Find($"blader{i}").GetComponent<UISprite>().enabled = false;
                }
                
                GameObject.Find("bulletL").GetComponent<UISprite>().enabled = true;
                GameObject.Find("bulletR").GetComponent<UISprite>().enabled = true;
                for (var i = 1; i <= 7; i++)
                {
                    GameObject.Find($"bulletL{i}").GetComponent<UISprite>().enabled = true;
                    GameObject.Find($"bulletR{i}").GetComponent<UISprite>().enabled = true;
                }
                if (skillId != "bomb") skillCD.transform.localPosition = Vector3.up * 5000f;
            }
        }
        else if (setup.myCostume.sex == Sex.Female)
        {
            _animationStand = "stand";
            setTeam2(1);
        }
        else
        {
            _animationStand = "stand_levi";
            setTeam2(1);
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
            SetMyTeam(team);
        }
    }

    public void shootFlare(int type)
    {
        switch (type)
        {
            case 1 when _flare1CD == 0f:
                _flare1CD = FlareCD;
                break;
            case 2 when _flare2CD == 0f:
                _flare2CD = FlareCD;
                break;
            case 3 when _flare3CD == 0f:
                _flare3CD = FlareCD;
                break;
            default: 
                return;
        }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            var obj2 = (GameObject) Instantiate(Resources.Load("FX/flareBullet" + type), transform.position, transform.rotation);
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
            var vector = Vector3.up * 10000f;
            cross1        .transform.localPosition = vector;
            cross2        .transform.localPosition = vector;
            LabelDistance .transform.localPosition = vector;
        }
        else
        {
            checkTitan();
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = (1 << LayerMask.NameToLayer("EnemyBox")) | (1 << LayerMask.NameToLayer("Ground"));
            if (Physics.Raycast(ray, out var hit, 1E+07f, mask.value))
            {
                cross1.transform.localPosition = cross2.transform.localPosition = Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                var magnitude = (hit.point - baseTransform.position).magnitude;
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
        var gasUsage = _gasRemaining / totalGas;
        
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
                cachedSprites["bulletL"].enabled = _leftHasBullet;
            if (cachedSprites.ContainsKey("bulletR"))
                cachedSprites["bulletR"].enabled = _leftHasBullet;
            return;
        }

        var bladeUsage = _bladeDuration / totalBladeSta;

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

        for (var i = 0; i < 5; i++)
        {
            if (cachedSprites.ContainsKey("bladel" + (i + 1)))
                cachedSprites["bladel" + (i + 1)].enabled = _bladesRemaining > i;
            if (cachedSprites.ContainsKey("blader" + (i + 1)))
                cachedSprites["blader" + (i + 1)].enabled = _bladesRemaining > i;
        }
    }

    [RPC]
    [UsedImplicitly]
    private void ShowHitDamage()
    {
        var target = GameObject.Find("LabelScore");
        if (target != null)
        {
            speed = Mathf.Max(10f, speed);
            target.GetComponent<UILabel>().text = speed.ToString(CultureInfo.InvariantCulture);
            target.transform.localScale = Vector3.zero;
            speed = (int) (speed * 0.1f);
            speed = Mathf.Clamp(speed, 40f, 150f);
            iTween.Stop(target);
            object[] args = { "x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f };
            iTween.ScaleTo(target, iTween.Hash(args));
            object[] objArray2 = { "x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f };
            iTween.ScaleTo(target, iTween.Hash(objArray2));
        }
    }

    private void showSkillCD()
    {
        if (skillCD != null) skillCD.GetComponent<UISprite>().fillAmount = (skillCDLast - skillCDDuration) / skillCDLast;
    }

    [RPC]
    [UsedImplicitly]
    public void SpawnCannonRPC(string settings, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient && photonView.isMine && myCannon == null)
        {
            if (_horse != null && isMounted) getOffHorse();
            idle();
            if (bulletLeft != null) bulletLeft.GetComponent<Bullet>().removeMe();
            if (bulletRight != null) bulletRight.GetComponent<Bullet>().removeMe();
            if (_particleGas.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && photonView.isMine) photonView.RPC(Rpc.GasEmission, PhotonTargets.Others, false);
            _particleGas.enableEmission = false;
            rigidbody.velocity = Vector3.zero;
            var strArray = settings.Split(',');
            if (strArray.Length > 15)
                myCannon = PhotonNetwork.Instantiate("RCAsset/" + strArray[1], new Vector3(float.Parse(strArray[12]), float.Parse(strArray[13]), float.Parse(strArray[14])), new Quaternion(float.Parse(strArray[15]), float.Parse(strArray[16]), float.Parse(strArray[17]), float.Parse(strArray[18])), 0);
            else
                myCannon = PhotonNetwork.Instantiate("RCAsset/" + strArray[1], new Vector3(float.Parse(strArray[2]), float.Parse(strArray[3]), float.Parse(strArray[4])), new Quaternion(float.Parse(strArray[5]), float.Parse(strArray[6]), float.Parse(strArray[7]), float.Parse(strArray[8])), 0);
            myCannonBase = myCannon.transform;
            myCannonPlayer = myCannon.transform.Find("PlayerPoint");
            isCannon = true;
            myCannon.GetComponent<Cannon>().myHero = this;
            myCannonRegion = null;
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(myCannon.transform.Find("Barrel").Find("FiringPoint").gameObject);
            Camera.main.fieldOfView = 55f;
            photonView.RPC(Rpc.SetMyCannon, PhotonTargets.OthersBuffered, myCannon.GetPhotonView().viewID);
            skillCDLast = 3.5f;
            skillCDDuration = 3.5f;
        }
    }
    
    private void Start()
    {
        GameManager.instance.Heroes.Add(this);
        
        if ((LevelInfoManager.Get(GameManager.Level).Horse || GameManager.settings.EnableHorse) && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            _horse = PhotonNetwork.Instantiate("horse", baseTransform.position + Vector3.up * 5f, baseTransform.rotation, 0);
            _horse.GetComponent<Horse>().myHero = gameObject;
            _horse.GetComponent<TITAN_CONTROLLER>().isHorse = true;
        }
        _particleSparks = baseTransform.Find("slideSparks").GetComponent<ParticleSystem>();
        _particleGas = baseTransform.Find("3dmg_smoke").GetComponent<ParticleSystem>();
        baseTransform.localScale = new Vector3(myScale, myScale, myScale);
        facingDirection = baseTransform.rotation.eulerAngles.y;
        targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
        _particleGas.enableEmission = false;
        _particleSparks.enableEmission = false;
        speedFXPS = speedFX1.GetComponent<ParticleSystem>();
        speedFXPS.enableEmission = false;
        var circleColor = Color.green;
        
        Player player = null;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            player = photonView.owner;
            if (player == null) // Should never happen
                return;
            
            player.Hero = this;

            if (PhotonNetwork.isMasterClient)
            {
                var id = player.ID;
                if (GameManager.heroHash.ContainsKey(id))
                    GameManager.heroHash[id] = this;
                else
                    GameManager.heroHash.Add(id, this);
            }
            _nameLabel = (GameObject) Instantiate(Resources.Load("UI/LabelNameOverHead"));
            _nameLabel.name = "LabelNameOverHead";
            _nameLabel.transform.parent = GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform;
            _nameLabel.transform.localScale = new Vector3(14f, 14f, 14f);
            _nameLabel.GetComponent<UILabel>().text = string.Empty;

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
            
            var builder = new StringBuilder(100);
            if (!photonView.isMine && player.Properties.IsAHSS == true)
                builder.Append("[FF0000]AHSS\n");
            if (!string.IsNullOrEmpty(player.Properties.Guild))
                builder.AppendFormat("[FFFF00]{0}\n", player.Properties.Guild);
            builder.AppendFormat("[FFFFFF]{0}", player.Properties.Name);
            _nameLabel.GetComponent<UILabel>().text = builder.ToString();
        }
        if (Minimap.instance != null)
            Minimap.instance.TrackGameObjectOnMinimap(gameObject, circleColor, false, true);
        
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer && !photonView.isMine)
        {
            gameObject.layer = LayerMask.NameToLayer("NetworkObject");
            if (IN_GAME_MAIN_CAMERA.Daylight == Daylight.Night)
            {
                var obj3 = (GameObject) Instantiate(Resources.Load("flashlight"));
                obj3.transform.parent = baseTransform;
                obj3.transform.position = baseTransform.position + Vector3.up;
                obj3.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
            }
            setup.myCostume = new HeroCostume();
            setup.myCostume = CostumeConverter.PhotonDataToHeroCostume(player);
            setup.setCharacterComponent();
            Destroy(checkBoxLeft);
            Destroy(checkBoxRight);
            Destroy(leftbladetrail);
            Destroy(rightbladetrail);
            Destroy(leftbladetrail2);
            Destroy(rightbladetrail2);
        }
        else
        {
            currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            loadskin();
            StartCoroutine(reloadSky());
        }

        _instantiated = true;
        _spawnTime = Time.time;
    }

    private void Suicide()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            netDieLocal(rigidbody.velocity * 50f, false, -1, string.Empty);
            GameManager.instance.needChooseSide = true;
            GameManager.instance.justSuicide = true;
        }
    }

    private void throwBlades()
    {
        var t = setup.part_blade_l.transform;
        var transform2 = setup.part_blade_r.transform;
        var obj2 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_l"), t.position, t.rotation);
        var obj3 = (GameObject) Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
        obj2.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        obj3.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
        var force = transform.forward + transform.up * 2f - transform.right;
        obj2.rigidbody.AddForce(force, ForceMode.Impulse);
        var vector2 = transform.forward + transform.up * 2f + transform.right;
        obj3.rigidbody.AddForce(vector2, ForceMode.Impulse);
        var torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        torque.Normalize();
        obj2.rigidbody.AddTorque(torque);
        torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        torque.Normalize();
        obj3.rigidbody.AddTorque(torque);
        setup.part_blade_l.SetActive(false);
        setup.part_blade_r.SetActive(false);
        _bladesRemaining--;
        if (_bladesRemaining == 0) _bladeDuration = 0f;
        if (State == HeroState.Attack) falseAttack();
    }

    public void ungrabbed()
    {
        facingDirection = 0f;
        targetRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.parent = null;
        GetComponent<CapsuleCollider>().isTrigger = false;
        State = HeroState.Idle;
    }

    private void unmounted()
    {
        _horse.GetComponent<Horse>().unmounted();
        isMounted = false;
    }

    public void UpdateName(PlayerProperties props)
    {
        if (photonView == null || _nameLabel.GetComponent<UILabel>() == null) 
            return;

        if (string.IsNullOrEmpty(props.Guild)) 
            _nameLabel.GetComponent<UILabel>().text = props.Name;
        else  
            _nameLabel.GetComponent<UILabel>().text = $"{props.Guild}\n{props.Name}";
    }

    public void DoUpdate()
    {
        // If game is paused Exit
        if (IN_GAME_MAIN_CAMERA.isPausing) 
            return;
        
        if (invincible > 0f) 
            invincible -= Time.deltaTime;

        // If HERO is dead Exit
        if (hasDied) 
            return;
        
        // RC Stuff -- Remove
        if (titanForm && _erenTitan != null)
        {
            baseTransform.position = _erenTitan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position; // Remove or optimize
            gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        }
        else if (isCannon && myCannon != null)
        {
            updateCannon();
            gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        }
        
        
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
        {
            if (myCannonRegion != null)
                if (Shelter.InputManager.IsDown(InputAction.EnterCannon)) myCannonRegion.photonView.RPC(Rpc.RequestControl, PhotonTargets.MasterClient, photonView.viewID);
            if (State == HeroState.Grab && !useGun)
            {
                if (Shelter.InputManager.IsDown(InputAction.Suicide)) Suicide(); //TODO: Check if PT and give kill to him

                if (skillId == "jean")
                {
                    if (State != HeroState.Attack && (Shelter.InputManager.IsDown(InputAction.Attack) || Shelter.InputManager.IsDown(InputAction.Special)) && escapeTimes > 0 && !baseAnimation.IsPlaying("grabbed_jean"))
                    {
                        playAnimation("grabbed_jean");
                        baseAnimation["grabbed_jean"].time = 0f;
                        escapeTimes--;
                    }
                    if (baseAnimation.IsPlaying("grabbed_jean") && baseAnimation["grabbed_jean"].normalizedTime > 0.64f && titanWhoGrabMe.GetComponent<TITAN>() != null)
                    {
                        ungrabbed();
                        baseRigidBody.velocity = Vector3.up * 30f;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        {
                            titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                        }
                        else
                        {
                            photonView.RPC(Rpc.GrabRemove, PhotonTargets.All);
                            if (PhotonNetwork.isMasterClient)
                                titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                            else
                                PhotonView.Find(titanWhoGrabMeID).RPC(Rpc.GrabEscape, PhotonTargets.MasterClient);
                        }
                    }
                }
                else if (skillId == "eren")
                {
                    showSkillCD();
                    if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer || IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer && !IN_GAME_MAIN_CAMERA.isPausing)
                    {
                        calcSkillCD();
                        calcFlareCD();
                    }
                    if (Shelter.InputManager.IsDown(InputAction.Special))
                        if (skillCDDuration <= 0f)
                        {
                            skillCDDuration = skillCDLast;
                            if (skillId == "eren" && titanWhoGrabMe.GetComponent<TITAN>() != null)
                            {
                                ungrabbed();
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                                {
                                    titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                                }
                                else
                                {
                                    photonView.RPC(Rpc.GrabRemove, PhotonTargets.All);
                                    if (PhotonNetwork.isMasterClient)
                                        titanWhoGrabMe.GetComponent<TITAN>().GrabbedTargetEscape();
                                    else
                                        PhotonView.Find(titanWhoGrabMeID).photonView.RPC(Rpc.GrabEscape,
                                            PhotonTargets.MasterClient);
                                }

                                erenTransform();
                            }
                        }
                }
            }
            else if (!titanForm && !isCannon)
            {
                if (Shelter.InputManager.IsDown(InputAction.Suicide))
                    Suicide();
                        
                SpeedupUpdate();
                updateExt();
                        
                if (!_isGrounded && State != HeroState.AirDodge)
                {
                    checkDashDoubleTap();
                    if (dashD)
                    {
                        dashD = false;
                        dash(0f, -1f);
                        return;
                    }
                    if (dashU)
                    {
                        dashU = false;
                        dash(0f, 1f);
                        return;
                    }
                    if (dashL)
                    {
                        dashL = false;
                        dash(-1f, 0f);
                        return;
                    }
                    if (dashR)
                    {
                        dashR = false;
                        dash(1f, 0f);
                        return;
                    }
                }
                if (_isGrounded && State == HeroState.Idle)
                {
                    if (Shelter.InputManager.IsKeyPressed(InputAction.Gas) && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("horse_geton"))
                    {
                        idle();
                        crossFade("jump", 0.1f);
                        _particleSparks.enableEmission = false;
                    }
                    if (Shelter.InputManager.IsDown(InputAction.Dodge) && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("horse_geton"))
                    {
                        if (_horse != null && !isMounted && Vector3.Distance(_horse.transform.position, transform.position) < 15f)
                            getOnHorse();
                        else
                            Dodge();
                    }
                }
                if (State == HeroState.Idle)
                {
                    if (Shelter.InputManager.IsDown(InputAction.GreenFlare)) shootFlare(1);
                    if (Shelter.InputManager.IsDown(InputAction.RedFlare)) shootFlare(2);
                    if (Shelter.InputManager.IsDown(InputAction.BlackFlare)) shootFlare(3);
                    if (_horse != null && isMounted && Shelter.InputManager.IsDown(InputAction.Dodge)) getOffHorse();
                    if ((animation.IsPlaying(_animationStand) || !_isGrounded) && Shelter.InputManager.IsDown(InputAction.Reload) && (!useGun || !GameManager.settings.AllowAirAHSSReload || _isGrounded))
                    {
                        changeBlade();
                        return;
                    }
                    if (baseAnimation.IsPlaying(_animationStand) && Shelter.InputManager.IsDown(InputAction.Salute))
                    {
                        salute();
                        return;
                    }
                    if (!isMounted && (Shelter.InputManager.IsDown(InputAction.Attack) || Shelter.InputManager.IsDown(InputAction.Special)) && !Screen.showCursor && !useGun)
                    {
                        var isSkillInCD = false;
                        if (Shelter.InputManager.IsDown(InputAction.Special))
                        {
                            isSkillInCD = skillCDDuration > 0f;
                            if (!isSkillInCD)
                            {
                                skillCDDuration = skillCDLast;
                                if (skillId == "eren")
                                {
                                    erenTransform();
                                    return;
                                }
                                if (skillId == "marco")
                                {
                                    if (IsGrounded())
                                    {
                                        _animationAttack = UnityEngine.Random.Range(0, 2) != 0 ? "special_marco_1" : "special_marco_0";
                                        playAnimation(_animationAttack);
                                    }
                                    else
                                    {
                                        isSkillInCD = true;
                                        skillCDDuration = 0f;
                                    }
                                }
                                else if (skillId == "armin")
                                {
                                    if (IsGrounded())
                                    {
                                        _animationAttack = "special_armin";
                                        playAnimation("special_armin");
                                    }
                                    else
                                    {
                                        isSkillInCD = true;
                                        skillCDDuration = 0f;
                                    }
                                }
                                else if (skillId == "sasha")
                                {
                                    if (IsGrounded())
                                    {
                                        _animationAttack = "special_sasha";
                                        playAnimation("special_sasha");
                                        _isSpeedup = true;
                                        _speedupTime = 10f;
                                    }
                                    else
                                    {
                                        isSkillInCD = true;
                                        skillCDDuration = 0f;
                                    }
                                }
                                else if (skillId == "mikasa")
                                {
                                    _animationAttack = "attack3_1";
                                    playAnimation("attack3_1");
                                    baseRigidBody.velocity = Vector3.up * 10f;
                                }
                                else if (skillId == "levi")
                                {
                                    RaycastHit hit;
                                    _animationAttack = "attack5";
                                    playAnimation("attack5");
                                    baseRigidBody.velocity += Vector3.up * 5f;
                                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
                                    LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
                                    LayerMask mask3 = mask2 | mask;
                                    if (Physics.Raycast(ray, out hit, 1E+07f, mask3.value))
                                    {
                                        if (bulletRight != null)
                                        {
                                            bulletRight.GetComponent<Bullet>().disable();
                                            releaseIfIHookSb();
                                        }
                                        dashDirection = hit.point - baseTransform.position;
                                        launchRightRope(hit, true, 1);
                                        rope.Play();
                                    }
                                    facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
                                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                                    attackLoop = 3;
                                }
                                else if (skillId == "petra")
                                {
                                    RaycastHit hit2;
                                    _animationAttack = "special_petra";
                                    playAnimation("special_petra");
                                    baseRigidBody.velocity += Vector3.up * 5f;
                                    var ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    LayerMask mask4 = 1 << LayerMask.NameToLayer("Ground");
                                    LayerMask mask5 = 1 << LayerMask.NameToLayer("EnemyBox");
                                    LayerMask mask6 = mask5 | mask4;
                                    if (Physics.Raycast(ray2, out hit2, 1E+07f, mask6.value))
                                    {
                                        if (bulletRight != null)
                                        {
                                            bulletRight.GetComponent<Bullet>().disable();
                                            releaseIfIHookSb();
                                        }
                                        if (bulletLeft != null)
                                        {
                                            bulletLeft.GetComponent<Bullet>().disable();
                                            releaseIfIHookSb();
                                        }
                                        dashDirection = hit2.point - baseTransform.position;
                                        launchLeftRope(hit2, true);
                                        launchRightRope(hit2, true);
                                        rope.Play();
                                    }
                                    facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
                                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                                    attackLoop = 3;
                                }
                                else
                                {
                                    if (needLean)
                                    {
                                        if (leanLeft)
                                            _animationAttack = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                        else
                                            _animationAttack = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                    }
                                    else
                                    {
                                        _animationAttack = "attack1";
                                    }
                                    playAnimation(_animationAttack);
                                }
                            }
                        }
                        else if (Shelter.InputManager.IsKeyPressed(InputAction.Attack))
                        {
                            if (needLean)
                            {
                                if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                                    _animationAttack = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                                    _animationAttack = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                else if (leanLeft)
                                    _animationAttack = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                else
                                    _animationAttack = UnityEngine.Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                            }
                            else if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                            {
                                _animationAttack = "attack2";
                            }
                            else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                            {
                                _animationAttack = "attack1";
                            }
                            else if (lastHook != null)
                            {
                                if (lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck") != null)
                                    attackAccordingToTarget(lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck"));
                                else
                                    isSkillInCD = true;
                            }
                            else if (bulletLeft != null && bulletLeft.transform.parent != null)
                            {
                                var a = bulletLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if (a != null)
                                    attackAccordingToTarget(a);
                                else
                                    attackAccordingToMouse();
                            }
                            else if (bulletRight != null && bulletRight.transform.parent != null)
                            {
                                var transform2 = bulletRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if (transform2 != null)
                                    attackAccordingToTarget(transform2);
                                else
                                    attackAccordingToMouse();
                            }
                            else
                            {
                                var obj2 = findNearestTitan();
                                if (obj2 != null)
                                {
                                    var transform3 = obj2.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                    if (transform3 != null)
                                        attackAccordingToTarget(transform3);
                                    else
                                        attackAccordingToMouse();
                                }
                                else
                                {
                                    attackAccordingToMouse();
                                }
                            }
                        }
                        if (!isSkillInCD)
                        {
                            checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                            checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                            if (_isGrounded) baseRigidBody.AddForce(gameObject.transform.forward * 200f);
                            playAnimation(_animationAttack);
                            baseAnimation[_animationAttack].time = 0f;
                            buttonAttackRelease = false;
                            State = HeroState.Attack;
                            if (_isGrounded || _animationAttack == "attack3_1" || _animationAttack == "attack5" || _animationAttack == "special_petra")
                            {
                                attackReleased = true;
                                buttonAttackRelease = true;
                            }
                            else
                            {
                                attackReleased = false;
                            }
                            _particleSparks.enableEmission = false;
                        }
                    }
                    if (useGun && !Screen.showCursor)
                    {
                        if (Shelter.InputManager.IsDown(InputAction.Special))
                        {
                            leftArmAim = true;
                            rightArmAim = true;
                        }
                        else if (Shelter.InputManager.IsKeyPressed(InputAction.Attack))
                        {
                            if (_leftHasBullet)
                            {
                                leftArmAim = true;
                                rightArmAim = false;
                            }
                            else
                            {
                                leftArmAim = false;
                                if (_rightHasBullet)
                                    rightArmAim = true;
                                else
                                    rightArmAim = false;
                            }
                        }
                        else
                        {
                            leftArmAim = false;
                            rightArmAim = false;
                        }
                        if (leftArmAim || rightArmAim)
                        {
                            RaycastHit hit3;
                            var ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
                            LayerMask mask7 = 1 << LayerMask.NameToLayer("Ground");
                            LayerMask mask8 = 1 << LayerMask.NameToLayer("EnemyBox");
                            LayerMask mask9 = mask8 | mask7;
                            if (Physics.Raycast(ray3, out hit3, 1E+07f, mask9.value)) gunTarget = hit3.point;
                        }
                        var flag4 = false;
                        var flag5 = false;
                        var flag6 = false;
                        if (Shelter.InputManager.IsUp(InputAction.Special) && skillId != "bomb")
                        {
                            if (_leftHasBullet && _rightHasBullet)
                            {
                                if (_isGrounded)
                                    _animationAttack = "AHSS_shoot_both";
                                else
                                    _animationAttack = "AHSS_shoot_both_air";
                                flag4 = true;
                            }
                            else if (!(_leftHasBullet || _rightHasBullet))
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
                            if (_isGrounded)
                            {
                                if (_leftHasBullet && _rightHasBullet)
                                {
                                    if (isLeftHandHooked)
                                        _animationAttack = "AHSS_shoot_r";
                                    else
                                        _animationAttack = "AHSS_shoot_l";
                                }
                                else if (_leftHasBullet)
                                {
                                    _animationAttack = "AHSS_shoot_l";
                                }
                                else if (_rightHasBullet)
                                {
                                    _animationAttack = "AHSS_shoot_r";
                                }
                            }
                            else if (_leftHasBullet && _rightHasBullet)
                            {
                                if (isLeftHandHooked)
                                    _animationAttack = "AHSS_shoot_r_air";
                                else
                                    _animationAttack = "AHSS_shoot_l_air";
                            }
                            else if (_leftHasBullet)
                            {
                                _animationAttack = "AHSS_shoot_l_air";
                            }
                            else if (_rightHasBullet)
                            {
                                _animationAttack = "AHSS_shoot_r_air";
                            }
                            if (_leftHasBullet || _rightHasBullet)
                                flag4 = true;
                            else
                                flag5 = true;
                        }
                        if (flag4)
                        {
                            State = HeroState.Attack;
                            crossFade(_animationAttack, 0.05f);
                            gunDummy.transform.position = baseTransform.position;
                            gunDummy.transform.rotation = baseTransform.rotation;
                            gunDummy.transform.LookAt(gunTarget);
                            attackReleased = false;
                            facingDirection = gunDummy.transform.rotation.eulerAngles.y;
                            targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                        }
                        else if (flag5 && (_isGrounded || LevelInfoManager.Get(GameManager.Level).Gamemode != GameMode.PvpAHSS && !GameManager.settings.AllowAirAHSSReload))
                        {
                            changeBlade();
                        }
                    }
                }
                else if (State == HeroState.Attack)
                {
                    if (!useGun)
                    {
                        if (!Shelter.InputManager.IsKeyPressed(InputAction.Attack)) buttonAttackRelease = true;
                        if (!attackReleased)
                        {
                            if (buttonAttackRelease)
                            {
                                continueAnimation();
                                attackReleased = true;
                            }
                            else if (baseAnimation[_animationAttack].normalizedTime >= 0.32f)
                            {
                                pauseAnimation();
                            }
                        }
                        if (_animationAttack == "attack3_1" && _bladeDuration > 0f)
                        {
                            if (baseAnimation[_animationAttack].normalizedTime >= 0.8f)
                            {
                                if (!checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                                    if (ModuleManager.Enabled(nameof(ModuleWeaponTrail)))
                                    {
                                        leftbladetrail2.Activate();
                                        rightbladetrail2.Activate();
                                        leftbladetrail.Activate();
                                        rightbladetrail.Activate();
                                    }
                                    baseRigidBody.velocity = -Vector3.up * 30f;
                                }
                                if (!checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
                                    slash.Play();
                                }
                            }
                            else if (checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                            {
                                checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                                checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                                checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                                leftbladetrail.StopSmoothly(0.1f);
                                rightbladetrail.StopSmoothly(0.1f);
                                leftbladetrail2.StopSmoothly(0.1f);
                                rightbladetrail2.StopSmoothly(0.1f);
                            }
                        }
                        else
                        {
                            float num;
                            float num2;
                            if (_bladeDuration == 0f)
                            {
                                num = -1f;
                                num2 = -1f;
                            }
                            else if (_animationAttack == "attack5")
                            {
                                num2 = 0.35f;
                                num = 0.5f;
                            }
                            else if (_animationAttack == "special_petra")
                            {
                                num2 = 0.35f;
                                num = 0.48f;
                            }
                            else if (_animationAttack == "special_armin")
                            {
                                num2 = 0.25f;
                                num = 0.35f;
                            }
                            else if (_animationAttack == "attack4")
                            {
                                num2 = 0.6f;
                                num = 0.9f;
                            }
                            else if (_animationAttack == "special_sasha")
                            {
                                num = -1f;
                                num2 = -1f;
                            }
                            else
                            {
                                num2 = 0.5f;
                                num = 0.85f;
                            }
                            if (baseAnimation[_animationAttack].normalizedTime > num2 && baseAnimation[_animationAttack].normalizedTime < num)
                            {
                                if (!checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                                    slash.Play();
                                    if (ModuleManager.Enabled(nameof(ModuleWeaponTrail)))
                                    {
                                        leftbladetrail2.Activate();
                                        rightbladetrail2.Activate();
                                        leftbladetrail.Activate();
                                        rightbladetrail.Activate();
                                    }
                                }
                                if (!checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me) checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
                            }
                            else if (checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                            {
                                checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                                checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                                checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                                leftbladetrail2.StopSmoothly(0.1f);
                                rightbladetrail2.StopSmoothly(0.1f);
                                leftbladetrail.StopSmoothly(0.1f);
                                rightbladetrail.StopSmoothly(0.1f);
                            }
                            if (attackLoop > 0 && baseAnimation[_animationAttack].normalizedTime > num)
                            {
                                attackLoop--;
                                playAnimationAt(_animationAttack, num2);
                            }
                        }
                        if (baseAnimation[_animationAttack].normalizedTime >= 1f)
                        {
                            if (_animationAttack == "special_marco_0" || _animationAttack == "special_marco_1")
                            {
                                if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
                                {
                                    if (!PhotonNetwork.isMasterClient)
                                        photonView.RPC(Rpc.TauntAttack, PhotonTargets.MasterClient, 5f, 100f);
                                    else
                                        NetTauntAttack(5f);
                                }
                                else
                                {
                                    NetTauntAttack(5f);
                                }
                                falseAttack();
                                idle();
                            }
                            else if (_animationAttack == "special_armin")
                            {
                                if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
                                {
                                    if (!PhotonNetwork.isMasterClient)
                                        photonView.RPC(Rpc.LaughAttack, PhotonTargets.MasterClient);
                                    else
                                        NetlaughAttack();
                                }
                                else
                                {
                                    foreach (var obj3 in GameObject.FindGameObjectsWithTag("titan"))
                                        if (Vector3.Distance(obj3.transform.position, baseTransform.position) < 50f && Vector3.Angle(obj3.transform.forward, baseTransform.position - obj3.transform.position) < 90f && obj3.GetComponent<TITAN>() != null) obj3.GetComponent<TITAN>().StartLaughing();
                                }
                                falseAttack();
                                idle();
                            }
                            else if (_animationAttack == "attack3_1")
                            {
                                baseRigidBody.velocity -= Vector3.up * Time.deltaTime * 30f;
                            }
                            else
                            {
                                falseAttack();
                                idle();
                            }
                        }
                        if (baseAnimation.IsPlaying("attack3_2") && baseAnimation["attack3_2"].normalizedTime >= 1f)
                        {
                            falseAttack();
                            idle();
                        }
                    }
                    else
                    {
                        baseTransform.rotation = Quaternion.Lerp(baseTransform.rotation, gunDummy.transform.rotation, Time.deltaTime * 30f);
                        if (!attackReleased && baseAnimation[_animationAttack].normalizedTime > 0.167f)
                        {
                            attackReleased = true;
                            var prefabName = "FX/shotGun";
                            if (_animationAttack == "AHSS_shoot_both" || _animationAttack == "AHSS_shoot_both_air")
                            {
                                prefabName = "FX/shotGun 1";
                                _leftHasBullet = false;
                                _rightHasBullet = false;
                                baseRigidBody.AddForce(-baseTransform.forward * 1000f, ForceMode.Acceleration);
                            }
                            else
                            {
                                if (_animationAttack == "AHSS_shoot_l" || _animationAttack == "AHSS_shoot_l_air")
                                    _leftHasBullet = false;
                                else
                                    _rightHasBullet = false;
                                baseRigidBody.AddForce(-baseTransform.forward * 600f, ForceMode.Acceleration);
                            }
                            baseRigidBody.AddForce(Vector3.up * 200f, ForceMode.Acceleration);
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
                            {
                                var obj4 = PhotonNetwork.Instantiate(prefabName, baseTransform.position + baseTransform.up * 0.8f - baseTransform.right * 0.1f, baseTransform.rotation, 0);
                                if (obj4.GetComponent<EnemyfxIDcontainer>() != null)
                                    obj4.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = photonView.viewID;
                            }
                            else
                            {
                                Instantiate(
                                    Resources.Load(prefabName), 
                                    baseTransform.position + baseTransform.up * 0.8f - baseTransform.right * 0.1f, 
                                    baseTransform.rotation);
                            }
                        }
                        if (baseAnimation[_animationAttack].normalizedTime >= 1f)
                        {
                            falseAttack();
                            idle();
                        }
                        if (!baseAnimation.IsPlaying(_animationAttack))
                        {
                            falseAttack();
                            idle();
                        }
                    }
                }
                else if (State == HeroState.ChangeBlade)
                {
                    if (useGun)
                    {
                        if (baseAnimation[reloadAnimation].normalizedTime > 0.22f)
                        {
                            if (!(_leftHasBullet || !setup.part_blade_l.activeSelf))
                            {
                                setup.part_blade_l.SetActive(false);
                                var t = setup.part_blade_l.transform;
                                var obj5 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_l"), t.position, t.rotation);
                                obj5.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
                                var force = -baseTransform.forward * 10f + baseTransform.up * 5f - baseTransform.right;
                                obj5.rigidbody.AddForce(force, ForceMode.Impulse);
                                var torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
                                obj5.rigidbody.AddTorque(torque, ForceMode.Acceleration);
                            }
                            if (!(_rightHasBullet || !setup.part_blade_r.activeSelf))
                            {
                                setup.part_blade_r.SetActive(false);
                                var transform5 = setup.part_blade_r.transform;
                                var obj6 = (GameObject) Instantiate(Resources.Load("Character_parts/character_gun_r"), transform5.position, transform5.rotation);
                                obj6.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
                                var vector3 = -baseTransform.forward * 10f + baseTransform.up * 5f + baseTransform.right;
                                obj6.rigidbody.AddForce(vector3, ForceMode.Impulse);
                                var vector4 = new Vector3(UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300));
                                obj6.rigidbody.AddTorque(vector4, ForceMode.Acceleration);
                            }
                        }
                        if (baseAnimation[reloadAnimation].normalizedTime > 0.62f && !throwedBlades)
                        {
                            throwedBlades = true;
                            if (!(_shotsRemainingLeft <= 0 || _leftHasBullet))
                            {
                                _shotsRemainingLeft--;
                                setup.part_blade_l.SetActive(true);
                                _leftHasBullet = true;
                            }
                            if (!(_shotsRemainingRight <= 0 || _rightHasBullet))
                            {
                                setup.part_blade_r.SetActive(true);
                                _shotsRemainingRight--;
                                _rightHasBullet = true;
                            }
                            updateRightMagUI();
                            updateLeftMagUI();
                        }
                        if (baseAnimation[reloadAnimation].normalizedTime > 1f) idle();
                    }
                    else
                    {
                        if (!_isGrounded)
                        {
                            if (!(animation[reloadAnimation].normalizedTime < 0.2f || throwedBlades))
                            {
                                throwedBlades = true;
                                if (setup.part_blade_l.activeSelf) throwBlades();
                            }
                            if (animation[reloadAnimation].normalizedTime >= 0.56f && _bladesRemaining > 0)
                            {
                                setup.part_blade_l.SetActive(true);
                                setup.part_blade_r.SetActive(true);
                                _bladeDuration = totalBladeSta;
                            }
                        }
                        else
                        {
                            if (!(baseAnimation[reloadAnimation].normalizedTime < 0.13f || throwedBlades))
                            {
                                throwedBlades = true;
                                if (setup.part_blade_l.activeSelf) throwBlades();
                            }
                            if (baseAnimation[reloadAnimation].normalizedTime >= 0.37f && _bladesRemaining > 0)
                            {
                                setup.part_blade_l.SetActive(true);
                                setup.part_blade_r.SetActive(true);
                                _bladeDuration = totalBladeSta;
                            }
                        }
                        if (baseAnimation[reloadAnimation].normalizedTime >= 1f) idle();
                    }
                }
                else if (State == HeroState.Salute)
                {
                    if (baseAnimation["salute"].normalizedTime >= 1f) idle();
                }
                else if (State == HeroState.GroundDodge)
                {
                    if (baseAnimation.IsPlaying("dodge"))
                    {
                        if (!(_isGrounded || baseAnimation["dodge"].normalizedTime <= 0.6f)) idle();
                        if (baseAnimation["dodge"].normalizedTime >= 1f) idle();
                    }
                }
                else if (State == HeroState.Land)
                {
                    if (baseAnimation.IsPlaying("dash_land") && baseAnimation["dash_land"].normalizedTime >= 1f) idle();
                }
                else if (State == HeroState.FillGas)
                {
                    if (baseAnimation.IsPlaying("supply") && baseAnimation["supply"].normalizedTime >= 1f)
                    {
                        _bladeDuration = totalBladeSta;
                        _bladesRemaining = MaxBlades;
                        _gasRemaining = totalGas;
                        if (!useGun)
                        {
                            setup.part_blade_l.SetActive(true);
                            setup.part_blade_r.SetActive(true);
                        }
                        else
                        {
                            _shotsRemainingLeft = _shotsRemainingRight = MaxBullets;
                            _rightHasBullet = true;
                            _leftHasBullet = true;
                            setup.part_blade_l.SetActive(true);
                            setup.part_blade_r.SetActive(true);
                            updateRightMagUI();
                            updateLeftMagUI();
                        }
                        idle();
                    }
                }
                else if (State == HeroState.Slide)
                {
                    if (!_isGrounded) idle();
                }
                else if (State == HeroState.AirDodge)
                {
                    if (dashTime > 0f)
                    {
                        dashTime -= Time.deltaTime;
                        if (currentSpeed > _preDashVelocity) baseRigidBody.AddForce(-baseRigidBody.velocity * Time.deltaTime * 1.7f, ForceMode.VelocityChange);
                    }
                    else
                    {
                        dashTime = 0f;
                        idle();
                    }
                }
                if (!(!Shelter.InputManager.IsKeyPressed(InputAction.LeftHook) || (baseAnimation.IsPlaying("attack3_1") || baseAnimation.IsPlaying("attack5") || baseAnimation.IsPlaying("special_petra") || State == HeroState.Grab) && State != HeroState.Idle))
                {
                    _holdingLeftHook = true;
                    if (bulletLeft == null)
                    {
                        var ray4 = Camera.main.ScreenPointToRay(Input.mousePosition);
                        LayerMask mask10 = 1 << LayerMask.NameToLayer("Ground");
                        LayerMask mask11 = 1 << LayerMask.NameToLayer("EnemyBox");
                        LayerMask mask12 = mask11 | mask10;
                        if (Physics.Raycast(ray4, out var hit4, 10000f, mask12.value))
                        {
                            launchLeftRope(hit4, true);
                            rope.Play();
                        }
                    }
                }
                else
                {
                    _holdingLeftHook = false;
                }
                if (!(!Shelter.InputManager.IsKeyPressed(InputAction.RightHook) || (baseAnimation.IsPlaying("attack3_1") || baseAnimation.IsPlaying("attack5") || baseAnimation.IsPlaying("special_petra") || State == HeroState.Grab) && State != HeroState.Idle))
                {
                    _holdingRightHook = true;
                    if (bulletRight == null)
                    {
                        var ray5 = Camera.main.ScreenPointToRay(Input.mousePosition);
                        LayerMask mask13 = 1 << LayerMask.NameToLayer("Ground");
                        LayerMask mask14 = 1 << LayerMask.NameToLayer("EnemyBox");
                        LayerMask mask15 = mask14 | mask13;
                        if (Physics.Raycast(ray5, out var hit5, 10000f, mask15.value))
                        {
                            launchRightRope(hit5, true);
                            rope.Play();
                        }
                    }
                }
                else
                {
                    _holdingRightHook = false;
                }
                if (!(!Shelter.InputManager.IsKeyPressed(InputAction.BothHooks) || (baseAnimation.IsPlaying("attack3_1") || baseAnimation.IsPlaying("attack5") || baseAnimation.IsPlaying("special_petra") || State == HeroState.Grab) && State != HeroState.Idle))
                {
                    _holdingLeftHook = true;
                    _holdingRightHook = true;
                    if (bulletLeft == null && bulletRight == null)
                    {
                        var ray6 = Camera.main.ScreenPointToRay(Input.mousePosition);
                        LayerMask mask16 = 1 << LayerMask.NameToLayer("Ground");
                        LayerMask mask17 = 1 << LayerMask.NameToLayer("EnemyBox");
                        LayerMask mask18 = mask17 | mask16;
                        if (Physics.Raycast(ray6, out var hit6, 1000000f, mask18.value))
                        {
                            launchLeftRope(hit6, false);
                            launchRightRope(hit6, false);
                            rope.Play();
                        }
                    }
                }
                if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer || IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer && !IN_GAME_MAIN_CAMERA.isPausing)
                {
                    calcSkillCD();
                    calcFlareCD();
                }
                if (!useGun)
                {
                    if (leftbladetrail.gameObject.GetActive())
                    {
                        leftbladetrail.update();
                        rightbladetrail.update();
                    }
                    if (leftbladetrail2.gameObject.GetActive())
                    {
                        leftbladetrail2.update();
                        rightbladetrail2.update();
                    }
                    if (leftbladetrail.gameObject.GetActive())
                    {
                        leftbladetrail.lateUpdate();
                        rightbladetrail.lateUpdate();
                    }
                    if (leftbladetrail2.gameObject.GetActive())
                    {
                        leftbladetrail2.lateUpdate();
                        rightbladetrail2.lateUpdate();
                    }
                }
                if (!IN_GAME_MAIN_CAMERA.isPausing)
                {
                    showSkillCD();
                    ShowFlareCD();
                    UpdateUsageUI();
                    DrawReticle();
                }
            }
            else if (isCannon && !IN_GAME_MAIN_CAMERA.isPausing)
            {
                DrawReticle();
                calcSkillCD();
                showSkillCD();
            }
        }
    }

    public void updateCannon()
    {
        baseTransform.position = myCannonPlayer.position;
        baseTransform.rotation = myCannonBase.rotation;
    }

    public void updateExt()
    {
        if (skillId == "bomb")
        {
            if (Shelter.InputManager.IsDown(InputAction.Special) && skillCDDuration <= 0f)
            {
                if (!(myBomb == null || myBomb.disabled)) myBomb.Explode(bombRadius);
                detonate = false;
                skillCDDuration = bombCD;
                var hitInfo = new RaycastHit();
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
                LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
                LayerMask mask3 = mask2 | mask;
                currentV = baseTransform.position;
                targetV = currentV + Vector3.forward * 200f;
                if (Physics.Raycast(ray, out hitInfo, 1000000f, mask3.value)) targetV = hitInfo.point;
                var vector = Vector3.Normalize(targetV - currentV);
                var obj2 = PhotonNetwork.Instantiate("RCAsset/BombMain", currentV + vector * 4f, new Quaternion(0f, 0f, 0f, 1f), 0);
                obj2.rigidbody.velocity = vector * bombSpeed;
                myBomb = obj2.GetComponent<Bomb>();
                bombTime = 0f;
            }
            else if (myBomb != null && !myBomb.disabled)
            {
                bombTime += Time.deltaTime;
                var flag2 = false;
                if (Shelter.InputManager.IsUp(InputAction.Special))
                {
                    detonate = true;
                }
                else if (Shelter.InputManager.IsDown(InputAction.Special) && detonate)
                {
                    detonate = false;
                    flag2 = true;
                }
                if (bombTime >= bombTimeMax) flag2 = true;
                if (flag2)
                {
                    myBomb.Explode(bombRadius);
                    detonate = false;
                }
            }
        }
    }

    private void updateLeftMagUI()
    {
        for (var i = 1; i <= MaxBullets; i++) GameObject.Find("bulletL" + i).GetComponent<UISprite>().enabled = false;
        for (var j = 1; j <= _shotsRemainingLeft; j++) GameObject.Find("bulletL" + j).GetComponent<UISprite>().enabled = true;
    }

    private void updateRightMagUI()
    {
        for (var i = 1; i <= MaxBullets; i++) GameObject.Find("bulletR" + i).GetComponent<UISprite>().enabled = false;
        for (var j = 1; j <= _shotsRemainingRight; j++) GameObject.Find("bulletR" + j).GetComponent<UISprite>().enabled = true;
    }

    public void useBlade(int amount = 1)
    {
        if (ModuleManager.Enabled(nameof(ModuleInfiniteBlade)))
            return;

        amount *= 2; //TODO: Overflow on 4 calls (with argument int.MaxValue) [int.MaxValue * 2 = -2 (0b11..110)]
        if (_bladeDuration > 0f)
        {
            _bladeDuration -= amount;
            if (_bladeDuration <= 0f)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
                {
                    leftbladetrail.Deactivate();
                    rightbladetrail.Deactivate();
                    leftbladetrail2.Deactivate();
                    rightbladetrail2.Deactivate();
                    checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                    checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                }
                _bladeDuration = 0f;
                throwBlades();
            }
        }
    }

    private void useGas(float amount = 0)
    {
        if (ModuleManager.Enabled(nameof(ModuleInfiniteGas)))
            return;
        
        if (amount == 0f)
            amount = useGasSpeed;
        
        if (_gasRemaining > 0f)
            _gasRemaining -= amount;
        if (_gasRemaining < 0f)
            _gasRemaining = 0f;
    }

    [RPC]
    [UsedImplicitly]
    private void WhoIsMyErenTitan(int id)
    {
        if (PhotonView.TryParse(id, out var view))
        {
            _erenTitan = view.gameObject;
            titanForm = true;
        }
    }

    public bool IsGrabbed => State == HeroState.Grab;
    private HeroState State
    {
        get => _state;
        set
        {
            if (_state == HeroState.AirDodge || _state == HeroState.GroundDodge) dashTime = 0f;
            _state = value;
        }
    }

    /// <summary>
    /// Prevents SpawnKill by making players immune for 5 seconds after respawn in Bomb mode.
    /// </summary>
    /// <remarks>This check is called only for remote players</remarks>
    public bool RCBombImmune => Time.time - _spawnTime < 5f;
    
    /// <summary>
    /// True after the HERO has spawned
    /// </summary>
    public bool IsInstantiated => _instantiated;
}