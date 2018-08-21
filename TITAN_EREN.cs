using System.Collections;
using Mod;
using Mod.Keybinds;
using UnityEngine;

public class TITAN_EREN : Photon.MonoBehaviour
{
    private string attackAnimation;
    private Transform attackBox;
    private bool attackChkOnce;
    public GameObject bottomObject;
    public bool canJump = true;
    private ArrayList checkPoints = new ArrayList();
    public Camera currentCamera;
    public Vector3 dashDirection;
    private float dieTime;
    private float facingDirection;
    private float gravity = 500f;
    private bool grounded;
    public bool hasDied;
    private bool hasDieSteam;
    public bool hasSpawn;
    private string hitAnimation;
    private float hitPause;
    private ArrayList hitTargets;
    public FengCustomInputs inputManager;
    private bool isAttack;
    public bool isHit;
    private bool isHitWhileCarryingRock;
    private bool isNextAttack;
    private bool isPlayRoar;
    private bool isROCKMOVE;
    public float jumpHeight = 2f;
    private bool justGrounded;
    public float lifeTime = 9999f;
    private float lifeTimeMax = 9999f;
    public float maxVelocityChange = 100f;
    public GameObject myNetWorkName;
    private float myR;
    private bool needFreshCorePosition;
    private bool needRoar;
    private Vector3 oldCorePosition;
    public GameObject realBody;
    public GameObject rock;
    private bool rockHitGround;
    public bool rockLift;
    private int rockPhase;
    public float speed = 80f;
    private float sqrt2 = Mathf.Sqrt(2f);
    private int stepSoundPhase = 2;
    private Vector3 targetCheckPt;
    private float waitCounter;

    public void born()
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (obj2.GetComponent<FEMALE_TITAN>() != null)
            {
                obj2.GetComponent<FEMALE_TITAN>().erenIsHere(gameObject);
            }
        }
        if (!this.bottomObject.GetComponent<CheckHitGround>().isGrounded)
        {
            this.playAnimation("jump_air");
            this.needRoar = true;
        }
        else
        {
            this.needRoar = false;
            this.playAnimation("born");
            this.isPlayRoar = false;
        }
        this.playSound("snd_eren_shift");
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            Instantiate(Resources.Load("FX/Thunder"), transform.position + Vector3.up * 23f, Quaternion.Euler(270f, 0f, 0f));
        }
        else if (photonView.isMine)
        {
            PhotonNetwork.Instantiate("FX/Thunder", transform.position + Vector3.up * 23f, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        float num2 = 30f;
        this.lifeTime = 30f;
        this.lifeTimeMax = num2;
    }

    private void crossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
        if (PhotonNetwork.connected && photonView.isMine)
        {
            object[] parameters = new object[] { aniName, time };
            photonView.RPC("netCrossFade", PhotonTargets.Others, parameters);
        }
    }

    [RPC]
    private void EndMovingRock()
    {
        this.isROCKMOVE = false;
    }

    private void falseAttack()
    {
        this.isAttack = false;
        this.isNextAttack = false;
        this.hitTargets = new ArrayList();
        this.attackChkOnce = false;
    }

    private void FixedUpdate()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            if (this.rockLift)
            {
                this.RockUpdate();
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                if (this.hitPause > 0f)
                {
                    rigidbody.velocity = Vector3.zero;
                }
                else if (this.hasDied)
                {
                    rigidbody.velocity = Vector3.zero + Vector3.up * rigidbody.velocity.y;
                    rigidbody.AddForce(new Vector3(0f, -this.gravity * rigidbody.mass, 0f));
                }
                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
                {
                    if (rigidbody.velocity.magnitude > 50f)
                    {
                        this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, rigidbody.velocity.magnitude), 0.1f);
                    }
                    else
                    {
                        this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
                    }
                    if (this.bottomObject.GetComponent<CheckHitGround>().isGrounded)
                    {
                        if (!this.grounded)
                        {
                            this.justGrounded = true;
                        }
                        this.grounded = true;
                        this.bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
                    }
                    else
                    {
                        this.grounded = false;
                    }
                    float x = 0f;
                    float z = 0f;
                    if (!IN_GAME_MAIN_CAMERA.isTyping)
                    {
                        if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
                        {
                            z = 1f;
                        }
                        else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
                        {
                            z = -1f;
                        }
                        else
                        {
                            z = 0f;
                        }
                        if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                        {
                            x = -1f;
                        }
                        else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                        {
                            x = 1f;
                        }
                        else
                        {
                            x = 0f;
                        }
                    }
                    if (this.needFreshCorePosition)
                    {
                        this.oldCorePosition = transform.position - transform.Find("Amarture/Core").position;
                        this.needFreshCorePosition = false;
                    }
                    if (!this.isAttack && !this.isHit)
                    {
                        if (this.grounded)
                        {
                            Vector3 zero = Vector3.zero;
                            if (this.justGrounded)
                            {
                                this.justGrounded = false;
                                zero = rigidbody.velocity;
                                if (animation.IsPlaying("jump_air"))
                                {
                                    GameObject obj2 = (GameObject) Instantiate(Resources.Load("FX/boom2_eren"), transform.position, Quaternion.Euler(270f, 0f, 0f));
                                    obj2.transform.localScale = Vector3.one * 1.5f;
                                    if (this.needRoar)
                                    {
                                        this.playAnimation("born");
                                        this.needRoar = false;
                                        this.isPlayRoar = false;
                                    }
                                    else
                                    {
                                        this.playAnimation("jump_land");
                                    }
                                }
                            }
                            if (!animation.IsPlaying("jump_land") && !this.isAttack && !this.isHit && !animation.IsPlaying("born"))
                            {
                                Vector3 vector7 = new Vector3(x, 0f, z);
                                float y = this.currentCamera.transform.rotation.eulerAngles.y;
                                float num4 = Mathf.Atan2(z, x) * 57.29578f;
                                num4 = -num4 + 90f;
                                float num5 = y + num4;
                                float num6 = -num5 + 90f;
                                float num7 = Mathf.Cos(num6 * 0.01745329f);
                                float num8 = Mathf.Sin(num6 * 0.01745329f);
                                zero = new Vector3(num7, 0f, num8);
                                float num9 = vector7.magnitude <= 0.95f ? (vector7.magnitude >= 0.25f ? vector7.magnitude : 0f) : 1f;
                                zero = zero * num9;
                                zero = zero * this.speed;
                                if (x == 0f && z == 0f)
                                {
                                    if (!animation.IsPlaying("idle") && !animation.IsPlaying("dash_land") && !animation.IsPlaying("dodge") && !animation.IsPlaying("jump_start") && !animation.IsPlaying("jump_air") && !animation.IsPlaying("jump_land"))
                                    {
                                        this.crossFade("idle", 0.1f);
                                        zero = zero * 0f;
                                    }
                                    num5 = -874f;
                                }
                                else if (!animation.IsPlaying("run") && !animation.IsPlaying("jump_start") && !animation.IsPlaying("jump_air"))
                                {
                                    this.crossFade("run", 0.1f);
                                }
                                if (num5 != -874f)
                                {
                                    this.facingDirection = num5;
                                }
                            }
                            Vector3 velocity = rigidbody.velocity;
                            Vector3 force = zero - velocity;
                            force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                            force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                            force.y = 0f;
                            if (animation.IsPlaying("jump_start") && animation["jump_start"].normalizedTime >= 1f)
                            {
                                this.playAnimation("jump_air");
                                force.y += 240f;
                            }
                            else if (animation.IsPlaying("jump_start"))
                            {
                                force = -rigidbody.velocity;
                            }
                            rigidbody.AddForce(force, ForceMode.VelocityChange);
                            rigidbody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                        }
                        else
                        {
                            if (animation.IsPlaying("jump_start") && animation["jump_start"].normalizedTime >= 1f)
                            {
                                this.playAnimation("jump_air");
                                rigidbody.AddForce(Vector3.up * 240f, ForceMode.VelocityChange);
                            }
                            if (!animation.IsPlaying("jump") && !this.isHit)
                            {
                                Vector3 vector11 = new Vector3(x, 0f, z);
                                float num10 = this.currentCamera.transform.rotation.eulerAngles.y;
                                float num11 = Mathf.Atan2(z, x) * 57.29578f;
                                num11 = -num11 + 90f;
                                float num12 = num10 + num11;
                                float num13 = -num12 + 90f;
                                float num14 = Mathf.Cos(num13 * 0.01745329f);
                                float num15 = Mathf.Sin(num13 * 0.01745329f);
                                Vector3 vector13 = new Vector3(num14, 0f, num15);
                                float num16 = vector11.magnitude <= 0.95f ? (vector11.magnitude >= 0.25f ? vector11.magnitude : 0f) : 1f;
                                vector13 = vector13 * num16;
                                vector13 = vector13 * (this.speed * 2f);
                                if (x == 0f && z == 0f)
                                {
                                    num12 = -874f;
                                }
                                else
                                {
                                    rigidbody.AddForce(vector13, ForceMode.Impulse);
                                }
                                if (num12 != -874f)
                                {
                                    this.facingDirection = num12;
                                }
                                if (!animation.IsPlaying(string.Empty) && !animation.IsPlaying("attack3_2") && !animation.IsPlaying("attack5"))
                                {
                                    rigidbody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 6f);
                                }
                            }
                        }
                    }
                    else
                    {
                        Vector3 vector4 = transform.position - transform.Find("Amarture/Core").position - this.oldCorePosition;
                        this.oldCorePosition = transform.position - transform.Find("Amarture/Core").position;
                        rigidbody.velocity = vector4 / Time.deltaTime + Vector3.up * rigidbody.velocity.y;
                        rigidbody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                        if (this.justGrounded)
                        {
                            this.justGrounded = false;
                        }
                    }
                    rigidbody.AddForce(new Vector3(0f, -this.gravity * rigidbody.mass, 0f));
                }
            }
        }
    }

    public void hitByFT(int phase)
    {
        if (!this.hasDied)
        {
            this.isHit = true;
            this.hitAnimation = "hit_annie_" + phase;
            this.falseAttack();
            this.playAnimation(this.hitAnimation);
            this.needFreshCorePosition = true;
            if (phase == 3)
            {
                GameObject obj2;
                this.hasDied = true;
                Transform transform = this.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
                {
                    obj2 = PhotonNetwork.Instantiate("bloodExplore", transform.position + Vector3.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    obj2 = (GameObject) Instantiate(Resources.Load("bloodExplore"), transform.position + Vector3.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f));
                }
                obj2.transform.localScale = this.transform.localScale;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
                {
                    obj2 = PhotonNetwork.Instantiate("bloodsplatter", transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 0);
                }
                else
                {
                    obj2 = (GameObject) Instantiate(Resources.Load("bloodsplatter"), transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
                }
                obj2.transform.localScale = this.transform.localScale;
                obj2.transform.parent = transform;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
                {
                    obj2 = PhotonNetwork.Instantiate("FX/justSmoke", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    obj2 = (GameObject) Instantiate(Resources.Load("FX/justSmoke"), transform.position, Quaternion.Euler(270f, 0f, 0f));
                }
                obj2.transform.parent = transform;
            }
        }
    }

    public void hitByFTByServer(int phase)
    {
        object[] parameters = new object[] { phase };
        photonView.RPC("hitByFTRPC", PhotonTargets.All, parameters);
    }

    [RPC]
    private void HitByFTRPC(int phase)
    {
        if (photonView.isMine)
        {
            this.hitByFT(phase);
        }
    }

    public void hitByTitan()
    {
        if (!this.isHit && !this.hasDied && !animation.IsPlaying("born"))
        {
            if (this.rockLift)
            {
                this.crossFade("die", 0.1f);
                this.isHitWhileCarryingRock = true;
                FengGameManagerMKII.instance.GameLose();
                object[] parameters = new object[] { "set" };
                photonView.RPC("rockPlayAnimation", PhotonTargets.All, parameters);
            }
            else
            {
                this.isHit = true;
                this.hitAnimation = "hit_titan";
                this.falseAttack();
                this.playAnimation(this.hitAnimation);
                this.needFreshCorePosition = true;
            }
        }
    }

    public void hitByTitanByServer()
    {
        photonView.RPC("hitByTitanRPC", PhotonTargets.All, new object[0]);
    }

    [RPC]
    private void HitByTitanRPC()
    {
        if (photonView.isMine)
        {
            this.hitByTitan();
        }
    }

    public bool IsGrounded()
    {
        return this.bottomObject.GetComponent<CheckHitGround>().isGrounded;
    }

    public void DoLateUpdate()
    {
        if ((!IN_GAME_MAIN_CAMERA.isPausing || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer) && !this.rockLift && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
        {
            Quaternion to = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, 0f);
            GameObject.Find("MainCamera").transform.rotation = Quaternion.Lerp(GameObject.Find("MainCamera").transform.rotation, to, Time.deltaTime * 2f);
        }
    }

    public void loadskin()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            string url = (string) FengGameManagerMKII.settings[65];
            if ((int) FengGameManagerMKII.settings[1] == 1 && (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg")))
            {
                StartCoroutine(this.loadskinE(url));
            }
        }
        else if (photonView.isMine && (int) FengGameManagerMKII.settings[1] == 1)
        {
            photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, new object[] { (string) FengGameManagerMKII.settings[65] });
        }
    }

    public IEnumerator loadskinE(string url)
    {
        while (!this.hasSpawn)
        {
            yield return null;
        }
        bool mipmap = true;
        bool iteratorVariable1 = false;
        if ((int)FengGameManagerMKII.settings[63] == 1)
        {
            mipmap = false;
        }
        foreach (Renderer iteratorVariable4 in this.GetComponentsInChildren<Renderer>())
        {
            if (!FengGameManagerMKII.linkHash[2].ContainsKey(url))
            {
                WWW link = new WWW(url);
                yield return link;
                Texture2D iteratorVariable6 = RCextensions.LoadImageRC(link, mipmap, 1000000);
                link.Dispose();
                if (!FengGameManagerMKII.linkHash[2].ContainsKey(url))
                {
                    iteratorVariable1 = true;
                    iteratorVariable4.material.mainTexture = iteratorVariable6;
                    FengGameManagerMKII.linkHash[2].Add(url, iteratorVariable4.material);
                    iteratorVariable4.material = (Material)FengGameManagerMKII.linkHash[2][url];
                }
                else
                {
                    iteratorVariable4.material = (Material)FengGameManagerMKII.linkHash[2][url];
                }
            }
            else
            {
                iteratorVariable4.material = (Material)FengGameManagerMKII.linkHash[2][url];
            }
        }
        if (iteratorVariable1)
        {
            FengGameManagerMKII.instance.UnloadAssets();
        }
    }

    [RPC]
    public void LoadskinRPC(string url)
    {
        if ((int) FengGameManagerMKII.settings[1] == 1 && (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg")))
        {
            StartCoroutine(this.loadskinE(url));
        }
    }

    [RPC]
    private void NetCrossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
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
    private void NetTauntAttack(float tauntTime, float distance = 100f)
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (Vector3.Distance(obj2.transform.position, transform.position) < distance && obj2.GetComponent<TITAN>() != null)
            {
                obj2.GetComponent<TITAN>().GetTaunted(gameObject, tauntTime);
            }
            if (obj2.GetComponent<FEMALE_TITAN>() != null)
            {
                obj2.GetComponent<FEMALE_TITAN>().erenIsHere(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (GameObject.Find("MultiplayerManager") != null)
        {
            FengGameManagerMKII.instance.ErenTitans.Remove(this);
        }
    }

    public void playAnimation(string aniName)
    {
        animation.Play(aniName);
        if (PhotonNetwork.connected && photonView.isMine)
        {
            object[] parameters = new object[] { aniName };
            photonView.RPC("netPlayAnimation", PhotonTargets.Others, parameters);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
        if (PhotonNetwork.connected && photonView.isMine)
        {
            object[] parameters = new object[] { aniName, normalizedTime };
            photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, parameters);
        }
    }

    private void playSound(string sndname)
    {
        this.PlaysoundRPC(sndname);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            object[] parameters = new object[] { sndname };
            photonView.RPC("playsoundRPC", PhotonTargets.Others, parameters);
        }
    }

    [RPC]
    private void PlaysoundRPC(string sndname)
    {
        transform.Find(sndname).GetComponent<AudioSource>().Play();
    }

    [RPC]
    private void RemoveMe()
    {
        PhotonNetwork.RemoveRPCs(photonView);
        Destroy(gameObject);
    }

    [RPC]
    private void RockPlayAnimation(string anim)
    {
        this.rock.animation.Play(anim);
        this.rock.animation[anim].speed = 1f;
    }

    private void RockUpdate()
    {
        if (!this.isHitWhileCarryingRock)
        {
            if (this.isROCKMOVE)
            {
                this.rock.transform.position = transform.position;
                this.rock.transform.rotation = transform.rotation;
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                if (this.rockPhase == 0)
                {
                    rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -10f * rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 20f)
                    {
                        this.rockPhase++;
                        this.crossFade("idle", 1f);
                        this.waitCounter = 0f;
                        this.setRoute();
                    }
                }
                else if (this.rockPhase == 1)
                {
                    rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -this.gravity * rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 2f)
                    {
                        this.rockPhase++;
                        this.crossFade("run", 0.2f);
                        this.waitCounter = 0f;
                    }
                }
                else if (this.rockPhase == 2)
                {
                    Vector3 vector = transform.forward * 30f;
                    Vector3 velocity = rigidbody.velocity;
                    Vector3 force = vector - velocity;
                    force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                    force.y = 0f;
                    rigidbody.AddForce(force, ForceMode.VelocityChange);
                    if (transform.position.z < -238f)
                    {
                        transform.position = new Vector3(transform.position.x, 0f, -238f);
                        this.rockPhase++;
                        this.crossFade("idle", 0.2f);
                        this.waitCounter = 0f;
                    }
                }
                else if (this.rockPhase == 3)
                {
                    rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -10f * rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 1f)
                    {
                        this.rockPhase++;
                        this.crossFade("rock_lift", 0.1f);
                        object[] parameters = new object[] { "lift" };
                        photonView.RPC("rockPlayAnimation", PhotonTargets.All, parameters);
                        this.waitCounter = 0f;
                        this.targetCheckPt = (Vector3) this.checkPoints[0];
                    }
                }
                else if (this.rockPhase == 4)
                {
                    rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -this.gravity * rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 4.2f)
                    {
                        this.rockPhase++;
                        this.crossFade("rock_walk", 0.1f);
                        object[] objArray3 = new object[] { "move" };
                        photonView.RPC("rockPlayAnimation", PhotonTargets.All, objArray3);
                        this.rock.animation["move"].normalizedTime = animation["rock_walk"].normalizedTime;
                        this.waitCounter = 0f;
                        photonView.RPC("startMovingRock", PhotonTargets.All, new object[0]);
                    }
                }
                else if (this.rockPhase == 5)
                {
                    if (Vector3.Distance(transform.position, this.targetCheckPt) < 10f)
                    {
                        if (this.checkPoints.Count > 0)
                        {
                            if (this.checkPoints.Count == 1)
                            {
                                this.rockPhase++;
                            }
                            else
                            {
                                Vector3 vector6 = (Vector3) this.checkPoints[0];
                                this.targetCheckPt = vector6;
                                this.checkPoints.RemoveAt(0);
                                GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn2");
                                GameObject obj2 = GameObject.Find("titanRespawnTrost" + (7 - this.checkPoints.Count));
                                if (obj2 != null)
                                {
                                    foreach (GameObject obj3 in objArray)
                                    {
                                        if (obj3.transform.parent.gameObject == obj2)
                                        {
                                            GameObject obj4 = FengGameManagerMKII.instance.SpawnTitan(70, obj3.transform.position, obj3.transform.rotation, false);
                                            obj4.GetComponent<TITAN>().isAlarm = true;
                                            obj4.GetComponent<TITAN>().chaseDistance = 999999f;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.rockPhase++;
                        }
                    }
                    if (this.checkPoints.Count > 0 && Random.Range(0, 3000) < 10 - this.checkPoints.Count)
                    {
                        Quaternion quaternion;
                        RaycastHit hit;
                        if (Random.Range(0, 10) > 5)
                        {
                            quaternion = transform.rotation * Quaternion.Euler(0f, Random.Range(150f, 210f), 0f);
                        }
                        else
                        {
                            quaternion = transform.rotation * Quaternion.Euler(0f, Random.Range(-30f, 30f), 0f);
                        }
                        Vector3 vector7 = quaternion * new Vector3(Random.Range((float)100f, (float)200f), 0f, 0f);
                        Vector3 position = transform.position + vector7;
                        LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
                        float y = 0f;
                        if (Physics.Raycast(position + Vector3.up * 500f, -Vector3.up, out hit, 1000f, mask2.value))
                        {
                            y = hit.point.y;
                        }
                        position += Vector3.up * y;
                        GameObject obj5 = FengGameManagerMKII.instance.SpawnTitan(70, position, transform.rotation, false);
                        obj5.GetComponent<TITAN>().isAlarm = true;
                        obj5.GetComponent<TITAN>().chaseDistance = 999999f;
                    }
                    Vector3 vector10 = transform.forward * 6f;
                    Vector3 vector11 = rigidbody.velocity;
                    Vector3 vector12 = vector10 - vector11;
                    vector12.x = Mathf.Clamp(vector12.x, -this.maxVelocityChange, this.maxVelocityChange);
                    vector12.z = Mathf.Clamp(vector12.z, -this.maxVelocityChange, this.maxVelocityChange);
                    vector12.y = 0f;
                    rigidbody.AddForce(vector12, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -this.gravity * rigidbody.mass, 0f));
                    Vector3 vector13 = this.targetCheckPt - transform.position;
                    float current = -Mathf.Atan2(vector13.z, vector13.x) * 57.29578f;
                    float num4 = -Mathf.DeltaAngle(current, gameObject.transform.rotation.eulerAngles.y - 90f);
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, gameObject.transform.rotation.eulerAngles.y + num4, 0f), 0.8f * Time.deltaTime);
                }
                else if (this.rockPhase == 6)
                {
                    rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -10f * rigidbody.mass, 0f));
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    this.rockPhase++;
                    this.crossFade("rock_fix_hole", 0.1f);
                    object[] objArray4 = new object[] { "set" };
                    photonView.RPC("rockPlayAnimation", PhotonTargets.All, objArray4);
                    photonView.RPC("endMovingRock", PhotonTargets.All, new object[0]);
                }
                else if (this.rockPhase == 7)
                {
                    rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, -10f * rigidbody.mass, 0f));
                    if (animation["rock_fix_hole"].normalizedTime >= 1.2f)
                    {
                        this.crossFade("die", 0.1f);
                        this.rockPhase++;
                        FengGameManagerMKII.instance.GameWin();
                    }
                    if (animation["rock_fix_hole"].normalizedTime >= 0.62f && !this.rockHitGround)
                    {
                        this.rockHitGround = true;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
                        {
                            PhotonNetwork.Instantiate("FX/boom1_CT_KICK", new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f), 0);
                        }
                        else
                        {
                            Instantiate(Resources.Load("FX/boom1_CT_KICK"), new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f));
                        }
                    }
                }
            }
        }
    }

    public void setRoute()
    {
        GameObject obj2 = GameObject.Find("routeTrost");
        this.checkPoints = new ArrayList();
        for (int i = 1; i <= 7; i++)
        {
            this.checkPoints.Add(obj2.transform.Find("r" + i).position);
        }
        this.checkPoints.Add("end");
    }

    private void showAimUI()
    {
        GameObject obj2 = GameObject.Find("cross1");
        GameObject obj3 = GameObject.Find("cross2");
        GameObject obj4 = GameObject.Find("crossL1");
        GameObject obj5 = GameObject.Find("crossL2");
        GameObject obj6 = GameObject.Find("crossR1");
        GameObject obj7 = GameObject.Find("crossR2");
        GameObject obj8 = GameObject.Find("LabelDistance");
        Vector3 vector = Vector3.up * 10000f;
        obj7.transform.localPosition = vector;
        obj6.transform.localPosition = vector;
        obj5.transform.localPosition = vector;
        obj4.transform.localPosition = vector;
        obj8.transform.localPosition = vector;
        obj3.transform.localPosition = vector;
        obj2.transform.localPosition = vector;
    }

    private void showSkillCD()
    {
        GameObject.Find("skill_cd_eren").GetComponent<UISprite>().fillAmount = this.lifeTime / this.lifeTimeMax;
    }

    private void Start()
    {
        this.loadskin();
        FengGameManagerMKII.instance.ErenTitans.Add(this);
        if (this.rockLift)
        {
            this.rock = GameObject.Find("rock");
            this.rock.animation["lift"].speed = 0f;
        }
        else
        {
            this.currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            this.inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
            this.oldCorePosition = transform.position - transform.Find("Amarture/Core").position;
            this.myR = this.sqrt2 * 6f;
            animation["hit_annie_1"].speed = 0.8f;
            animation["hit_annie_2"].speed = 0.7f;
            animation["hit_annie_3"].speed = 0.7f;
        }
        this.hasSpawn = true;
    }

    [RPC]
    private void StartMovingRock()
    {
        this.isROCKMOVE = true;
    }

    public void DoUpdate()
    {
        if ((!IN_GAME_MAIN_CAMERA.isPausing || IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer) && !this.rockLift)
        {
            if (animation.IsPlaying("run"))
            {
                if (animation["run"].normalizedTime % 1f > 0.3f && animation["run"].normalizedTime % 1f < 0.75f && this.stepSoundPhase == 2)
                {
                    this.stepSoundPhase = 1;
                    Transform transform = this.transform.Find("snd_eren_foot");
                    transform.GetComponent<AudioSource>().Stop();
                    transform.GetComponent<AudioSource>().Play();
                }
                if (animation["run"].normalizedTime % 1f > 0.75f && this.stepSoundPhase == 1)
                {
                    this.stepSoundPhase = 2;
                    Transform transform2 = transform.Find("snd_eren_foot");
                    transform2.GetComponent<AudioSource>().Stop();
                    transform2.GetComponent<AudioSource>().Play();
                }
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
            {
                if (this.hasDied)
                {
                    if (animation["die"].normalizedTime >= 1f || this.hitAnimation == "hit_annie_3")
                    {
                        if (this.realBody != null)
                        {
                            this.realBody.GetComponent<HERO>().backToHuman();
                            this.realBody.transform.position = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position + Vector3.up * 2f;
                            this.realBody = null;
                        }
                        this.dieTime += Time.deltaTime;
                        if (this.dieTime > 2f && !this.hasDieSteam)
                        {
                            this.hasDieSteam = true;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                GameObject obj2 = (GameObject)Instantiate(Resources.Load("FX/FXtitanDie1"));
                                obj2.transform.position = transform.Find("Amarture/Core/Controller_Body/hip").position;
                                obj2.transform.localScale = transform.localScale;
                            }
                            else if (photonView.isMine)
                            {
                                PhotonNetwork.Instantiate("FX/FXtitanDie1", transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = transform.localScale;
                            }
                        }
                        if (this.dieTime > 5f)
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                GameObject obj4 = (GameObject)Instantiate(Resources.Load("FX/FXtitanDie"));
                                obj4.transform.position = transform.Find("Amarture/Core/Controller_Body/hip").position;
                                obj4.transform.localScale = transform.localScale;
                                Destroy(gameObject);
                            }
                            else if (photonView.isMine)
                            {
                                PhotonNetwork.Instantiate("FX/FXtitanDie", transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = transform.localScale;
                                PhotonNetwork.Destroy(photonView);
                            }
                        }
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine)
                {
                    if (this.isHit)
                    {
                        if (animation[this.hitAnimation].normalizedTime >= 1f)
                        {
                            this.isHit = false;
                            this.falseAttack();
                            this.playAnimation("idle");
                        }
                    }
                    else
                    {
                        if (this.lifeTime > 0f)
                        {
                            this.lifeTime -= Time.deltaTime;
                            if (this.lifeTime <= 0f)
                            {
                                this.hasDied = true;
                                this.playAnimation("die");
                                return;
                            }
                        }
                        if (this.grounded && !this.isAttack && !animation.IsPlaying("jump_land") && !this.isAttack && !animation.IsPlaying("born"))
                        {
                            if (Shelter.InputManager.IsDown(InputAction.Attack) || Shelter.InputManager.IsDown(InputAction.Special))
                            {
                                bool flag = false;
                                if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.Stop && Shelter.InputManager.IsKeyPressed(InputAction.Back) || Shelter.InputManager.IsDown(InputAction.Special))
                                {
                                    if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.Stop && Shelter.InputManager.IsDown(InputAction.Special) && this.inputManager.inputKey[11] == KeyCode.Mouse1)
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        this.attackAnimation = "attack_kick";
                                    }
                                }
                                else
                                {
                                    this.attackAnimation = "attack_combo_001";
                                }
                                if (!flag)
                                {
                                    this.playAnimation(this.attackAnimation);
                                    animation[this.attackAnimation].time = 0f;
                                    this.isAttack = true;
                                    this.needFreshCorePosition = true;
                                    if (this.attackAnimation == "attack_combo_001" || this.attackAnimation == "attack_combo_001")
                                    {
                                        this.attackBox = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
                                    }
                                    else if (this.attackAnimation == "attack_combo_002")
                                    {
                                        this.attackBox = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
                                    }
                                    else if (this.attackAnimation == "attack_kick")
                                    {
                                        this.attackBox = transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
                                    }
                                    this.hitTargets = new ArrayList();
                                }
                            }
                            if (Shelter.InputManager.IsDown(InputAction.Salute))
                            {
                                this.crossFade("born", 0.1f);
                                animation["born"].normalizedTime = 0.28f;
                                this.isPlayRoar = false;
                            }
                        }
                        if (!this.isAttack)
                        {
                            if ((this.grounded || animation.IsPlaying("idle")) && (!animation.IsPlaying("jump_start") && !animation.IsPlaying("jump_air")) && !animation.IsPlaying("jump_land") && Shelter.InputManager.IsDown(InputAction.BothHooks))
                            {
                                this.crossFade("jump_start", 0.1f);
                            }
                        }
                        else
                        {
                            if (animation[this.attackAnimation].time >= 0.1f && Shelter.InputManager.IsDown(InputAction.Attack))
                            {
                                this.isNextAttack = true;
                            }
                            float num = 0f;
                            float num2 = 0f;
                            float num3 = 0f;
                            string str = string.Empty;
                            if (this.attackAnimation == "attack_combo_001")
                            {
                                num = 0.4f;
                                num2 = 0.5f;
                                num3 = 0.66f;
                                str = "attack_combo_002";
                            }
                            else if (this.attackAnimation == "attack_combo_002")
                            {
                                num = 0.15f;
                                num2 = 0.25f;
                                num3 = 0.43f;
                                str = "attack_combo_003";
                            }
                            else if (this.attackAnimation == "attack_combo_003")
                            {
                                num3 = 0f;
                                num = 0.31f;
                                num2 = 0.37f;
                            }
                            else if (this.attackAnimation == "attack_kick")
                            {
                                num3 = 0f;
                                num = 0.32f;
                                num2 = 0.38f;
                            }
                            else
                            {
                                num = 0.5f;
                                num2 = 0.85f;
                            }
                            if (this.hitPause > 0f)
                            {
                                this.hitPause -= Time.deltaTime;
                                if (this.hitPause <= 0f)
                                {
                                    animation[this.attackAnimation].speed = 1f;
                                    this.hitPause = 0f;
                                }
                            }
                            if (num3 > 0f && this.isNextAttack && animation[this.attackAnimation].normalizedTime >= num3)
                            {
                                if (this.hitTargets.Count > 0)
                                {
                                    Transform transform3 = (Transform)this.hitTargets[0];
                                    if (transform3 != null)
                                    {
                                        transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(transform3.position - transform.position).eulerAngles.y, 0f);
                                        this.facingDirection = transform.rotation.eulerAngles.y;
                                    }
                                }
                                this.falseAttack();
                                this.attackAnimation = str;
                                this.crossFade(this.attackAnimation, 0.1f);
                                animation[this.attackAnimation].time = 0f;
                                animation[this.attackAnimation].speed = 1f;
                                this.isAttack = true;
                                this.needFreshCorePosition = true;
                                if (this.attackAnimation == "attack_combo_002")
                                {
                                    this.attackBox = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
                                }
                                else if (this.attackAnimation == "attack_combo_003")
                                {
                                    this.attackBox = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
                                }
                                this.hitTargets = new ArrayList();
                            }
                            if (animation[this.attackAnimation].normalizedTime >= num && animation[this.attackAnimation].normalizedTime <= num2 || !this.attackChkOnce && animation[this.attackAnimation].normalizedTime >= num)
                            {
                                if (!this.attackChkOnce)
                                {
                                    if (this.attackAnimation == "attack_combo_002")
                                    {
                                        this.playSound("snd_eren_swing2");
                                    }
                                    else if (this.attackAnimation == "attack_combo_001")
                                    {
                                        this.playSound("snd_eren_swing1");
                                    }
                                    else if (this.attackAnimation == "attack_combo_003")
                                    {
                                        this.playSound("snd_eren_swing3");
                                    }
                                    this.attackChkOnce = true;
                                }
                                Collider[] colliderArray = Physics.OverlapSphere(this.attackBox.transform.position, 8f);
                                for (int i = 0; i < colliderArray.Length; i++)
                                {
                                    if (colliderArray[i].gameObject.transform.root.GetComponent<TITAN>() == null)
                                    {
                                        continue;
                                    }
                                    bool flag2 = false;
                                    for (int j = 0; j < this.hitTargets.Count; j++)
                                    {
                                        if (colliderArray[i].gameObject.transform.root == (Transform)this.hitTargets[j])
                                        {
                                            flag2 = true;
                                            break;
                                        }
                                    }
                                    if (!flag2 && !colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().hasDie)
                                    {
                                        animation[this.attackAnimation].speed = 0f;
                                        if (this.attackAnimation == "attack_combo_002")
                                        {
                                            this.hitPause = 0.05f;
                                            colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().HitLeft(transform.position, this.hitPause);
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(1f, 0.03f, 0.95f);
                                        }
                                        else if (this.attackAnimation == "attack_combo_001")
                                        {
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(1.2f, 0.04f, 0.95f);
                                            this.hitPause = 0.08f;
                                            colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().HitRight(transform.position, this.hitPause);
                                        }
                                        else if (this.attackAnimation == "attack_combo_003")
                                        {
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(3f, 0.1f, 0.95f);
                                            this.hitPause = 0.3f;
                                            colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().dieHeadBlow(transform.position, this.hitPause);
                                        }
                                        else if (this.attackAnimation == "attack_kick")
                                        {
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(3f, 0.1f, 0.95f);
                                            this.hitPause = 0.2f;
                                            if (colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().abnormalType == AbnormalType.TYPE_CRAWLER)
                                            {
                                                colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().dieBlow(transform.position, this.hitPause);
                                            }
                                            else if (colliderArray[i].gameObject.transform.root.transform.localScale.x < 2f)
                                            {
                                                colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().dieBlow(transform.position, this.hitPause);
                                            }
                                            else
                                            {
                                                colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().HitRight(transform.position, this.hitPause);
                                            }
                                        }
                                        this.hitTargets.Add(colliderArray[i].gameObject.transform.root);
                                        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
                                        {
                                            PhotonNetwork.Instantiate("hitMeatBIG", (colliderArray[i].transform.position + this.attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f), 0);
                                        }
                                        else
                                        {
                                            Instantiate(Resources.Load("hitMeatBIG"), (colliderArray[i].transform.position + this.attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f));
                                        }
                                    }
                                }
                            }
                            if (animation[this.attackAnimation].normalizedTime >= 1f)
                            {
                                this.falseAttack();
                                this.playAnimation("idle");
                            }
                        }
                        if (animation.IsPlaying("jump_land") && animation["jump_land"].normalizedTime >= 1f)
                        {
                            this.crossFade("idle", 0.1f);
                        }
                        if (animation.IsPlaying("born"))
                        {
                            if (animation["born"].normalizedTime >= 0.28f && !this.isPlayRoar)
                            {
                                this.isPlayRoar = true;
                                this.playSound("snd_eren_roar");
                            }
                            if (animation["born"].normalizedTime >= 0.5f && animation["born"].normalizedTime <= 0.7f)
                            {
                                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.5f, 1f, 0.95f);
                            }
                            if (animation["born"].normalizedTime >= 1f)
                            {
                                this.crossFade("idle", 0.1f);
                                if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
                                {
                                    if (PhotonNetwork.isMasterClient)
                                    {
                                        object[] parameters = new object[] { 10f, 500f };
                                        photonView.RPC("netTauntAttack", PhotonTargets.MasterClient, parameters);
                                    }
                                    else
                                    {
                                        this.NetTauntAttack(10f, 500f);
                                    }
                                }
                                else
                                {
                                    this.NetTauntAttack(10f, 500f);
                                }
                            }
                        }
                        this.showAimUI();
                        this.showSkillCD();
                    }
                }
            }
        }
    }
}