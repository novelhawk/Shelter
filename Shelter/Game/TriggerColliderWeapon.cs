using System.Collections;
using Game;
using Mod;
using Mod.Managers;
using Mod.Modules;
using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class TriggerColliderWeapon : MonoBehaviour
{
    public bool active_me;
    public GameObject currentCamera;
    public ArrayList currentHits = new ArrayList();
    public ArrayList currentHitsII = new ArrayList();
    public AudioSource meatDie;
    public int myTeam = 1;
    public float scoreMulti = 1f;

    private bool CheckIfBehind(GameObject titan)
    {
        Transform head = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Vector3 to = this.transform.position - head.transform.position;
        return Vector3.Angle(-head.transform.forward, to) < 70f;
    }

    public void clearHits()
    {
        this.currentHitsII = new ArrayList();
        this.currentHits = new ArrayList();
    }

    private void napeMeat(Vector3 velocity, Transform titan)
    {
        Transform neck = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        GameObject obj2 = (GameObject) Instantiate(Resources.Load("titanNapeMeat"), neck.position, neck.rotation);
        obj2.transform.localScale = titan.localScale;
        obj2.rigidbody.AddForce(velocity.normalized * 15f, ForceMode.Impulse);
        obj2.rigidbody.AddForce(-titan.forward * 10f, ForceMode.Impulse);
        obj2.rigidbody.AddTorque(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)), ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active_me)
            return;
        
        if (!this.currentHitsII.Contains(other.gameObject))
        {
            this.currentHitsII.Add(other.gameObject);
            IN_GAME_MAIN_CAMERA.instance.startShake(0.1f, 0.1f);
            if (other.gameObject.transform.root.gameObject.CompareTag("titan"))
            {
                IN_GAME_MAIN_CAMERA.instance.main_object.GetComponent<HERO>().slashHit.Play();
                
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    Instantiate(Resources.Load("hitMeat"), transform.position, Quaternion.Euler(270f, 0f, 0f));
                else
                    PhotonNetwork.Instantiate("hitMeat", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);

                transform.root.GetComponent<HERO>().useBlade();
            }
        }

        if (other.gameObject.CompareTag("playerHitbox"))
        {
            if ((ModuleManager.Enabled(nameof(ModulePVPEverywhere)) || LevelInfoManager.Get(FengGameManagerMKII.Level).IsPvP) && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                float b = Mathf.Min(1f - Vector3.Distance(other.gameObject.transform.position, transform.position) * 0.05f, 1f);
                HitBox component = other.gameObject.GetComponent<HitBox>();

                if (component != null && component.transform.root != null)
                {
                    HERO hero = component.transform.root.GetComponent<HERO>();
                    
                    var view = transform.root.gameObject.GetPhotonView();
                    if (!component.transform.root.gameObject.GetPhotonView().isMine && (hero.myTeam != this.myTeam && !hero.isInvincible() || ModuleManager.Enabled(nameof(ModulePVPEverywhere))) && !hero.IsGrabbed && !hero.HasDied())
                    {
                        hero.markDie();
                        Vector3 vector2 = component.transform.root.position - transform.position;
                        component.transform.root.GetComponent<HERO>().photonView.RPC(Rpc.Die, PhotonTargets.All,
                            vector2.normalized * b * 1000f + Vector3.up * 50f,  // force : Vector3
                            false,                                              // isBite : bool
                            view.viewID,                                        // viewID : int
                            view.owner.Properties.Name,                         // titanName : string
                            false);                                             // killByTitan : bool
                    }
                }
            }
        }
        else if (other.gameObject.CompareTag("titanneck"))
        {
            HitBox item = other.gameObject.GetComponent<HitBox>();
            if (item != null && this.CheckIfBehind(item.transform.root.gameObject) && !this.currentHits.Contains(item))
            {
                item.hitPosition = (transform.position + item.transform.position) * 0.5f;
                this.currentHits.Add(item);
                this.meatDie.Play();
                
                var titan = item.transform.root.GetComponent<TITAN>();
                var femaleTitan = item.transform.root.GetComponent<FEMALE_TITAN>();
                var colossalTitan = item.transform.root.GetComponent<COLOSSAL_TITAN>();
                
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    if (titan != null && !titan.hasDie)
                    {
                        Vector3 velocity = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);

                        titan.die();
                        napeMeat(IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity, item.transform.root);
                        FengGameManagerMKII.instance.NetShowDamage(damage, null);
                        FengGameManagerMKII.instance.PlayerKillInfoSingleplayerUpdate(damage);
                    }
                }
                else if (!PhotonNetwork.isMasterClient)
                {
                    if (titan != null)
                    {
                        if (!titan.hasDie)
                        {
                            Vector3 velocity = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                            int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                            if (ModuleManager.Enabled(nameof(ModuleSnapshot)))
                            {
                                IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);
                                titan.asClientLookTarget = false;
                            }

                            titan.photonView.RPC(Rpc.TitanGetHit, titan.photonView.owner, transform.root.gameObject.GetPhotonView().viewID, damage);
                        }
                    }
                    else if (femaleTitan != null)
                    {
                        transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
                        Vector3 velocity = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                        if (!femaleTitan.hasDie)
                        {
                            femaleTitan.photonView.RPC(Rpc.TitanGetHit, 
                                femaleTitan.photonView.owner, 
                                transform.root.gameObject.GetPhotonView().viewID, 
                                damage);
                        }
                    }
                    else if (colossalTitan != null)
                    {
                        transform.root.GetComponent<HERO>().useBlade(2147483647);
                        if (!colossalTitan.hasDie)
                        {
                            Vector3 velocity = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                            int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                            colossalTitan.photonView.RPC(Rpc.TitanGetHit,
                                colossalTitan.photonView.owner, 
                                transform.root.gameObject.GetPhotonView().viewID, 
                                damage);
                        }
                    }
                }
                else if (titan != null)
                {
                    if (!titan.hasDie)
                    {
                        Vector3 vector7 =
                            IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity -
                            item.transform.root.rigidbody.velocity;
                        int num6 = (int) (vector7.magnitude * 10f * this.scoreMulti);
                        num6 = Mathf.Max(10, num6);
                        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, num6, item.transform.root.gameObject, 0.02f);

                        titan.TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num6);
                    }
                }
                else if (femaleTitan != null)
                {
                    transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
                    if (!femaleTitan.hasDie)
                    {
                        Vector3 vector8 = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int num7 = (int) (vector8.magnitude * 10f * this.scoreMulti);
                        num7 = Mathf.Max(10, num7);
                        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, num7, null, 0.02f);

                        femaleTitan.TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num7);
                    }
                }
                else if (colossalTitan != null)
                {
                    transform.root.GetComponent<HERO>().useBlade(2147483647);
                    if (!colossalTitan.hasDie)
                    {
                        Vector3 vector9 =
                            IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity -
                            item.transform.root.rigidbody.velocity;
                        int num8 = (int) (vector9.magnitude * 10f * this.scoreMulti);
                        num8 = Mathf.Max(10, num8);
                        IN_GAME_MAIN_CAMERA.instance.TakeScreenshot(item.transform.position, num8, null, 0.02f);

                        colossalTitan.TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num8);
                    }
                }

                this.showCriticalHitFX();
            }
        }
        else if (other.gameObject.CompareTag("titaneye"))
        {
            if (!this.currentHits.Contains(other.gameObject))
            {
                this.currentHits.Add(other.gameObject);

                var titan = other.gameObject.transform.root.gameObject.GetComponent<TITAN>();
                var femaleTitan = other.gameObject.transform.root.gameObject.GetComponent<FEMALE_TITAN>();
                
                if (femaleTitan != null && !femaleTitan.hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        femaleTitan.hitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        femaleTitan.photonView.RPC(Rpc.HitEye, PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    else
                        femaleTitan.HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                }
                else if (titan != null && titan.abnormalType != AbnormalType.Crawler && !titan.hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                        titan.HitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        titan.photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    else
                        titan.HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                }
                
                this.showCriticalHitFX();
            }
        }
        else if (other.gameObject.CompareTag("titanankle") && !this.currentHits.Contains(other.gameObject))
        {
            this.currentHits.Add(other.gameObject);
            GameObject obj4 = other.gameObject.transform.root.gameObject;
            Vector3 vector10 = IN_GAME_MAIN_CAMERA.instance.main_object.rigidbody.velocity -
                               obj4.rigidbody.velocity;
            int num9 = (int) (vector10.magnitude * 10f * this.scoreMulti);
            num9 = Mathf.Max(10, num9);
            
            var titan = obj4.GetComponent<TITAN>();
            var femaleTitan = obj4.GetComponent<FEMALE_TITAN>();
            
            if (titan != null && titan.abnormalType != AbnormalType.Crawler)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    if (!titan.hasDie)
                        titan.hitAnkle();
                }
                else
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        if (!titan.hasDie)
                            titan.photonView.RPC(Rpc.HitAnkle, PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    }
                    else if (!titan.hasDie)
                    {
                        titan.hitAnkle();
                    }

                    this.showCriticalHitFX();
                }
            }
            else if (femaleTitan != null && !femaleTitan.hasDie)
            {
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    case GameType.Singleplayer:
                        if (other.gameObject.name == "ankleR")
                            femaleTitan.hitAnkleR(num9);
                        else
                            femaleTitan.hitAnkleL(num9);
                        break;
                    
                    case GameType.Multiplayer:
                        if (PhotonNetwork.isMasterClient)
                        {
                            if (other.gameObject.name == "ankleR")
                                femaleTitan.HitAnkleRRPC(transform.root.gameObject.GetPhotonView().viewID, num9);
                            else
                                femaleTitan.HitAnkleLRPC(transform.root.gameObject.GetPhotonView().viewID, num9);
                        }
                        else
                        {
                            if (other.gameObject.name == "ankleR")
                                femaleTitan.photonView.RPC("hitAnkleRRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID, num9);
                            else
                                femaleTitan.photonView.RPC(Rpc.HitLeftAnkle, PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID, num9);
                        }
                        break;
                }

                this.showCriticalHitFX();
            }
        }
    }

    private void showCriticalHitFX()
    {
        GameObject obj2;
        IN_GAME_MAIN_CAMERA.instance.startShake(0.2f, 0.3f);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Singleplayer)
        {
            obj2 = PhotonNetwork.Instantiate("redCross", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            obj2 = (GameObject) Instantiate(Resources.Load("redCross"));
        }
        obj2.transform.position = transform.position;
    }

    private void Start()
    {
        this.currentCamera = GameObject.Find("MainCamera");
    }
}

