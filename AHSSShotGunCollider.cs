using System;
using System.Collections;
using Mod;
using Mod.Managers;
using Mod.Modules;
using UnityEngine;

public class AHSSShotGunCollider : MonoBehaviour
{
    public bool active_me;
    public GameObject currentCamera;
    public ArrayList currentHits = new ArrayList();
    public int dmg;
    private int myTeam = 1;
    private string ownerName = string.Empty;
    public float scoreMulti;
    private int viewID = -1;

    private bool CheckIfBehind(GameObject titan)
    {
        Transform neck = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Vector3 to = this.transform.position - neck.transform.position;
        return Vector3.Angle(-neck.transform.forward, to) < 100f;
    }

    private int _times;
    private void FixedUpdate()
    {
        if (!active_me)
            return;
        
        if (_times > 1)
            active_me = false;
        _times++;
    }


    private void OnTriggerStay(Collider other)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && !transform.root.gameObject.GetPhotonView().isMine || !active_me) 
            return;
        
        
        if (other.gameObject.CompareTag("playerHitbox"))
        {
            if (ModuleManager.Enabled(nameof(ModulePVPEverywhere)) || LevelInfoManager.Get(FengGameManagerMKII.Level).IsPvP)
            {
                float b = 1f - Vector3.Distance(other.gameObject.transform.position, transform.position) * 0.05f;
                b = Mathf.Min(1f, b);
                HitBox component = other.gameObject.GetComponent<HitBox>();
                if (component != null &&
                    component.transform.root != null)
                {
                    var hero = component.transform.root.GetComponent<HERO>();
                    if (!component.transform.root.gameObject.GetPhotonView().isMine && (hero.myTeam != this.myTeam || ModuleManager.Enabled(nameof(ModulePVPEverywhere))) && 
                        !hero.isInvincible() && !hero.HasDied() && !hero.IsGrabbed)
                    {
                        hero.markDie();
                        Vector3 distance = component.transform.root.position - transform.position;
                        component.transform.root.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All,
                            distance.normalized * b * 1000f + Vector3.up * 50f, // force : Vector3
                            false, // isBite : bool
                            this.viewID, // viewID : int
                            this.ownerName, // titanName : string
                            false); // killByTitan : bool
                    }
                }
            }
        }
        else if (other.gameObject.CompareTag("erenHitbox"))
        {
            if (dmg > 0 && !other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().isHit)
            {
                other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByTitan();
            }
        }
        else if (other.gameObject.CompareTag("titanneck"))
        {
            HitBox item = other.gameObject.GetComponent<HitBox>();
            if (item != null && CheckIfBehind(item.transform.root.gameObject) && !this.currentHits.Contains(item))
            {
                item.hitPosition = (transform.position + item.transform.position) * 0.5f;
                this.currentHits.Add(item);

                TITAN titan = item.transform.root.GetComponent<TITAN>();
                FEMALE_TITAN femaleTitan = item.transform.root.GetComponent<FEMALE_TITAN>();
                COLOSSAL_TITAN colossalTitan = item.transform.root.GetComponent<COLOSSAL_TITAN>();

                Vector3 velocity = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    // TITAN
                    // Single
                    case GameType.Singleplayer when titan != null && !titan.hasDie:
                        
                        FengGameManagerMKII.instance.NetShowDamage(damage, null);
                        if (damage > item.transform.root.GetComponent<TITAN>().myLevel * 100f)
                        {
                            item.transform.root.GetComponent<TITAN>().die();
                            if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                            FengGameManagerMKII.instance.PlayerKillInfoSingleplayerUpdate(damage);
                        }
                        break;
                    
                    // Multi as MC
                    case GameType.Multiplayer when PhotonNetwork.isMasterClient && titan != null && !titan.hasDie:
                        if (damage > item.transform.root.GetComponent<TITAN>().myLevel * 100f)
                        {
                            if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                        
                            item.transform.root.GetComponent<TITAN>().TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, damage);
                        }
                        break;
                    
                    // Multi as user
                    case GameType.Multiplayer when titan != null && !titan.hasDie:
                        if (damage > item.transform.root.GetComponent<TITAN>().myLevel * 100f)
                        {
                            if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            {
                                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                                item.transform.root.GetComponent<TITAN>().asClientLookTarget = false;
                            }
                            object[] objArray2 = new object[] { transform.root.gameObject.GetPhotonView().viewID, damage };
                            item.transform.root.GetComponent<TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<TITAN>().photonView.owner, objArray2);
                        }
                        break;
                    
                    // FEMALE TITAN
                    // Multi as MC
                    case GameType.Multiplayer when PhotonNetwork.isMasterClient && femaleTitan != null && !femaleTitan.hasDie:
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().TakeScreenshot(item.transform.position, damage, null, 0.02f);
                        
                        item.transform.root.GetComponent<FEMALE_TITAN>().TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, damage);
                        break;
                    
                    // Multi as user
                    case GameType.Multiplayer when femaleTitan != null && !femaleTitan.hasDie:
                        item.transform.root.GetComponent<FEMALE_TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<FEMALE_TITAN>().photonView.owner, 
                            transform.root.gameObject.GetPhotonView().viewID, damage);
                        break;
                    
                    // COLOSSAL TITAN
                    // Multi as MC
                    case GameType.Multiplayer when PhotonNetwork.isMasterClient && colossalTitan != null && !colossalTitan.hasDie:
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().TakeScreenshot(item.transform.position, damage, null, 0.02f);
                        
                        item.transform.root.GetComponent<COLOSSAL_TITAN>().TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, damage);
                        break;
                    
                    // Multi as user
                    case GameType.Multiplayer when colossalTitan != null && !colossalTitan.hasDie:
                        item.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.owner, 
                            transform.root.gameObject.GetPhotonView().viewID, damage);
                        break;
                        
                }
                    
                this.ShowCriticalHitFX(other.gameObject.transform.position);
            }
        }
        else if (other.gameObject.CompareTag("titaneye"))
        {
            if (!this.currentHits.Contains(other.gameObject))
            {
                this.currentHits.Add(other.gameObject);
                
                GameObject eye = other.gameObject.transform.root.gameObject;
                if (eye.GetComponent<FEMALE_TITAN>() != null && !eye.GetComponent<FEMALE_TITAN>().hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        eye.GetComponent<FEMALE_TITAN>().hitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        eye.GetComponent<FEMALE_TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    else
                        eye.GetComponent<FEMALE_TITAN>().HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                }
                else if (eye.GetComponent<TITAN>() != null && eye.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler && !eye.GetComponent<TITAN>().hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        eye.GetComponent<TITAN>().HitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        eye.GetComponent<TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    else if (!eye.GetComponent<TITAN>().hasDie)
                        eye.GetComponent<TITAN>().HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                    
                    this.ShowCriticalHitFX(other.gameObject.transform.position);
                }
            }
        }
        else if (other.gameObject.CompareTag("titanankle") && !this.currentHits.Contains(other.gameObject))
        {
            currentHits.Add(other.gameObject);
            GameObject obj = other.gameObject.transform.root.gameObject;
            Vector3 velocity = currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - obj.rigidbody.velocity;
            int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
            if (obj.GetComponent<TITAN>() != null && obj.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler && !obj.GetComponent<TITAN>().hasDie)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    obj.GetComponent<TITAN>().hitAnkle();
                else if (!PhotonNetwork.isMasterClient)
                    obj.GetComponent<TITAN>().photonView.RPC("hitAnkleRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                else
                    obj.GetComponent<TITAN>().hitAnkle();
                
                ShowCriticalHitFX(other.gameObject.transform.position);
            }
            else if (obj.GetComponent<FEMALE_TITAN>() != null && !obj.GetComponent<FEMALE_TITAN>().hasDie)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    if (obj.GetComponent<FEMALE_TITAN>() != null)
                    {
                        if (other.gameObject.name == "ankleR")
                            obj.GetComponent<FEMALE_TITAN>().hitAnkleR(damage);
                        else
                            obj.GetComponent<FEMALE_TITAN>().hitAnkleL(damage);
                    }
                }
                else
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        if (other.gameObject.name == "ankleR")
                            obj.GetComponent<FEMALE_TITAN>().HitAnkleRRPC(transform.root.gameObject.GetPhotonView().viewID, damage);
                        else
                            obj.GetComponent<FEMALE_TITAN>().HitAnkleLRPC(transform.root.gameObject.GetPhotonView().viewID, damage);    
                    }
                    else
                    {
                        if (other.gameObject.name == "ankleR")
                            obj.GetComponent<FEMALE_TITAN>().photonView.RPC("hitAnkleRRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID, damage);
                        else
                            obj.GetComponent<FEMALE_TITAN>().photonView.RPC("hitAnkleLRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID, damage);
                    }
                }

                this.ShowCriticalHitFX(other.gameObject.transform.position);
            }
        }
    }

    private void ShowCriticalHitFX(Vector3 position)
    {
        this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f);
        
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            Instantiate(Resources.Load("redCross1"), position, Quaternion.Euler(270f, 0f, 0f));
        else
            PhotonNetwork.Instantiate("redCross1", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
    }

    private void Start()
    {
        switch (IN_GAME_MAIN_CAMERA.GameType)
        {
            case GameType.Singleplayer:
                this.myTeam = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>().myTeam;
                break;
            
            case GameType.Multiplayer when !transform.root.gameObject.GetPhotonView().isMine:
                enabled = false;
                return;
            
            case GameType.Multiplayer:
                if (transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
                {
                    this.viewID = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                    this.ownerName = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                    this.myTeam = PhotonView.Find(this.viewID).gameObject.GetComponent<HERO>().myTeam;
                }
                break;
        }

        this.active_me = true;
        this.currentCamera = GameObject.Find("MainCamera");
    }
}

