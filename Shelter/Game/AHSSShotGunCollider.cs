using System.Collections;
using Game;
using Mod;
using Mod.Managers;
using Mod.Modules;
using Photon;
using UnityEngine;
using Extensions = Photon.Extensions;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
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
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && !Extensions.GetPhotonView(transform.root.gameObject).isMine || !active_me) 
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
                    if (!Extensions.GetPhotonView(component.transform.root.gameObject).isMine && (hero.myTeam != this.myTeam || ModuleManager.Enabled(nameof(ModulePVPEverywhere))) && 
                        !hero.isInvincible() && !hero.HasDied() && !hero.IsGrabbed)
                    {
                        hero.markDie();
                        Vector3 distance = component.transform.root.position - transform.position;
                        component.transform.root.GetComponent<HERO>().photonView.RPC(Rpc.Die, PhotonTargets.All,
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

                Vector3 velocity = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    // TITAN
                    // Single
                    case GameType.Singleplayer when titan != null && !titan.hasDie:
                        
                        FengGameManagerMKII.instance.NetShowDamage(damage, null);
                        if (damage > titan.myLevel * 100f)
                        {
                            titan.die();
                            IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                            FengGameManagerMKII.instance.PlayerKillInfoSingleplayerUpdate(damage);
                        }
                        break;
                    
                    // Multi as MC
                    case GameType.Multiplayer when PhotonNetwork.isMasterClient && titan != null && !titan.hasDie:
                        if (damage > titan.myLevel * 100f)
                        {
                            IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                        
                            titan.TitanGetHit(Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                        }
                        break;
                    
                    // Multi as user
                    case GameType.Multiplayer when titan != null && !titan.hasDie:
                        if (damage > titan.myLevel * 100f)
                        {
                            if (ModuleManager.Enabled(nameof(ModuleSnapshot)))
                            {
                                IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                                titan.asClientLookTarget = false;
                            }
                            object[] objArray2 = new object[] { Extensions.GetPhotonView(transform.root.gameObject).viewID, damage };
                            titan.photonView.RPC(Rpc.TitanGetHit, titan.photonView.owner, objArray2);
                        }
                        break;
                    
                    // FEMALE TITAN
                    // Multi as MC
                    case GameType.Multiplayer when PhotonNetwork.isMasterClient && femaleTitan != null && !femaleTitan.hasDie:
                        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, null, 0.02f);
                        
                        femaleTitan.TitanGetHit(Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                        break;
                    
                    // Multi as user
                    case GameType.Multiplayer when femaleTitan != null && !femaleTitan.hasDie:
                        femaleTitan.photonView.RPC(Rpc.TitanGetHit, femaleTitan.photonView.owner, 
                            Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                        break;
                    
                    // COLOSSAL TITAN
                    // Multi as MC
                    case GameType.Multiplayer when PhotonNetwork.isMasterClient && colossalTitan != null && !colossalTitan.hasDie:
                        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, null, 0.02f);
                        
                        colossalTitan.TitanGetHit(Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                        break;
                    
                    // Multi as user
                    case GameType.Multiplayer when colossalTitan != null && !colossalTitan.hasDie:
                        colossalTitan.photonView.RPC(Rpc.TitanGetHit, colossalTitan.photonView.owner, 
                            Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
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

                var femaleTitan = eye.GetComponent<FEMALE_TITAN>();
                if (femaleTitan != null && !femaleTitan.hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        femaleTitan.hitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        femaleTitan.photonView.RPC(Rpc.HitEye, PhotonTargets.MasterClient, Extensions.GetPhotonView(transform.root.gameObject).viewID);
                    else
                        femaleTitan.HitEyeRPC(Extensions.GetPhotonView(transform.root.gameObject).viewID);
                    return;
                }
                
                var titan = eye.GetComponent<TITAN>();
                if (titan != null && titan.abnormalType != AbnormalType.Crawler && !titan.hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        titan.HitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        titan.photonView.RPC(Rpc.HitEye, PhotonTargets.MasterClient, Extensions.GetPhotonView(transform.root.gameObject).viewID);
                    else if (!titan.hasDie)
                        titan.HitEyeRPC(Extensions.GetPhotonView(transform.root.gameObject).viewID);
                    
                    this.ShowCriticalHitFX(other.gameObject.transform.position);
                }
            }
        }
        else if (other.gameObject.CompareTag("titanankle") && !this.currentHits.Contains(other.gameObject))
        {
            currentHits.Add(other.gameObject);
            GameObject obj = other.gameObject.transform.root.gameObject;
            Vector3 velocity = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - obj.rigidbody.velocity;
            int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));

            var titan = obj.GetComponent<TITAN>();
            var femaleTitan = obj.GetComponent<FEMALE_TITAN>();
            
            if (titan != null && titan.abnormalType != AbnormalType.Crawler && !titan.hasDie)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    titan.hitAnkle();
                else if (!PhotonNetwork.isMasterClient)
                    titan.photonView.RPC(Rpc.HitEye, PhotonTargets.MasterClient, Extensions.GetPhotonView(transform.root.gameObject).viewID);
                else
                    titan.hitAnkle();
                
                ShowCriticalHitFX(other.gameObject.transform.position);
            }
            else if (femaleTitan != null && !femaleTitan.hasDie)
            {
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    case GameType.Singleplayer:
                        if (other.gameObject.name == "ankleR")
                            femaleTitan.hitAnkleR(damage);
                        else
                            femaleTitan.hitAnkleL(damage);
                        break;
                    
                    case GameType.Multiplayer:
                        if (PhotonNetwork.isMasterClient)
                        {
                            if (other.gameObject.name == "ankleR")
                                femaleTitan.HitAnkleRRPC(Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                            else
                                femaleTitan.HitAnkleLRPC(Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);    
                        }
                        else
                        {
                            if (other.gameObject.name == "ankleR")
                                femaleTitan.photonView.RPC(Rpc.HitRightAnkle, PhotonTargets.MasterClient, Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                            else
                                femaleTitan.photonView.RPC(Rpc.HitLeftAnkle, PhotonTargets.MasterClient, Extensions.GetPhotonView(transform.root.gameObject).viewID, damage);
                        }

                        break;
                }

                this.ShowCriticalHitFX(other.gameObject.transform.position);
            }
        }
    }

    private void ShowCriticalHitFX(Vector3 position)
    {
        IN_GAME_MAIN_CAMERA.instance.startShake(0.2f, 0.3f);
        
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
                this.myTeam = IN_GAME_MAIN_CAMERA.instance.main_object.GetComponent<HERO>().myTeam;
                break;
            
            case GameType.Multiplayer when !Extensions.GetPhotonView(transform.root.gameObject).isMine:
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

