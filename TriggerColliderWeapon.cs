using System.Collections;
using Mod;
using Mod.Interface;
using Mod.Modules;
using UnityEngine;

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
        Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Vector3 to = this.transform.position - transform.transform.position;
        return Vector3.Angle(-transform.transform.forward, to) < 70f;
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
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.1f, 0.1f, 0.95f);
            if (other.gameObject.transform.root.gameObject.CompareTag("titan"))
            {
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>().slashHit.Play();
                
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    Instantiate(Resources.Load("hitMeat"), transform.position, Quaternion.Euler(270f, 0f, 0f));
                else
                    PhotonNetwork.Instantiate("hitMeat", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);

                transform.root.GetComponent<HERO>().useBlade();
            }
        }

        if (other.gameObject.CompareTag("playerHitbox"))
        {
            if ((Shelter.ModuleManager.Enabled(nameof(ModulePVPEverywhere)) || LevelInfoManager.GetInfo(FengGameManagerMKII.Level).IsPvP) && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                float b = 1f - Vector3.Distance(other.gameObject.transform.position, transform.position) * 0.05f;
                b = Mathf.Min(1f, b);
                HitBox component = other.gameObject.GetComponent<HitBox>();

                if (component != null && component.transform.root != null)
                {
                    HERO hero = component.transform.root.GetComponent<HERO>();
                    
                    var view = transform.root.gameObject.GetPhotonView();
                    if (!component.transform.root.gameObject.GetPhotonView().isMine && (hero.myTeam != this.myTeam && !hero.isInvincible() || Shelter.ModuleManager.Enabled(nameof(ModulePVPEverywhere))) && !hero.IsGrabbed && !hero.HasDied())
                    {
                        hero.markDie();
                        Vector3 vector2 = component.transform.root.position - transform.position;
                        component.transform.root.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All,
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
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    if (item.transform.root.GetComponent<TITAN>() != null && !item.transform.root.GetComponent<TITAN>().hasDie)
                    {
                        Vector3 velocity = currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject, 0.02f);

                        item.transform.root.GetComponent<TITAN>().die();
                        napeMeat(currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity, item.transform.root);
                        FengGameManagerMKII.instance.NetShowDamage(damage, null);
                        FengGameManagerMKII.instance.PlayerKillInfoSingleplayerUpdate(damage);
                    }
                }
                else if (!PhotonNetwork.isMasterClient)
                {
                    if (item.transform.root.GetComponent<TITAN>() != null)
                    {
                        if (!item.transform.root.GetComponent<TITAN>().hasDie)
                        {
                            Vector3 velocity = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                            int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                            if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            {
                                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>()
                                    .TakeScreenshot(item.transform.position, damage, item.transform.root.gameObject,
                                        0.02f);
                                item.transform.root.GetComponent<TITAN>().asClientLookTarget = false;
                            }

                            item.transform.root.GetComponent<TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<TITAN>().photonView.owner, 
                                transform.root.gameObject.GetPhotonView().viewID, damage);
                        }
                    }
                    else if (item.transform.root.GetComponent<FEMALE_TITAN>() != null)
                    {
                        transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
                        Vector3 velocity = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                        if (!item.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            item.transform.root.GetComponent<FEMALE_TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<FEMALE_TITAN>().photonView.owner, 
                                transform.root.gameObject.GetPhotonView().viewID, damage);
                        }
                    }
                    else if (item.transform.root.GetComponent<COLOSSAL_TITAN>() != null)
                    {
                        transform.root.GetComponent<HERO>().useBlade(2147483647);
                        if (!item.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                        {
                            Vector3 velocity = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                            int damage = Mathf.Max(10, (int) (velocity.magnitude * 10f * this.scoreMulti));
                            item.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.RPC("titanGetHit",item.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.owner, 
                                transform.root.gameObject.GetPhotonView().viewID, damage);
                        }
                    }
                }
                else if (item.transform.root.GetComponent<TITAN>() != null)
                {
                    if (!item.transform.root.GetComponent<TITAN>().hasDie)
                    {
                        Vector3 vector7 =
                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity -
                            item.transform.root.rigidbody.velocity;
                        int num6 = (int) (vector7.magnitude * 10f * this.scoreMulti);
                        num6 = Mathf.Max(10, num6);
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                        {
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>()
                                .TakeScreenshot(item.transform.position, num6, item.transform.root.gameObject, 0.02f);
                        }

                        item.transform.root.GetComponent<TITAN>()
                            .TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num6);
                    }
                }
                else if (item.transform.root.GetComponent<FEMALE_TITAN>() != null)
                {
                    transform.root.GetComponent<HERO>().useBlade(2147483647);
                    if (!item.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        Vector3 vector8 =
                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity -
                            item.transform.root.rigidbody.velocity;
                        int num7 = (int) (vector8.magnitude * 10f * this.scoreMulti);
                        num7 = Mathf.Max(10, num7);
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                        {
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>()
                                .TakeScreenshot(item.transform.position, num7, null, 0.02f);
                        }

                        item.transform.root.GetComponent<FEMALE_TITAN>()
                            .TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num7);
                    }
                }
                else if (item.transform.root.GetComponent<COLOSSAL_TITAN>() != null)
                {
                    transform.root.GetComponent<HERO>().useBlade(2147483647);
                    if (!item.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                    {
                        Vector3 vector9 =
                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity -
                            item.transform.root.rigidbody.velocity;
                        int num8 = (int) (vector9.magnitude * 10f * this.scoreMulti);
                        num8 = Mathf.Max(10, num8);
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                        {
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>()
                                .TakeScreenshot(item.transform.position, num8, null, 0.02f);
                        }

                        item.transform.root.GetComponent<COLOSSAL_TITAN>()
                            .TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num8);
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
                GameObject gameObject = other.gameObject.transform.root.gameObject;
                if (gameObject.GetComponent<FEMALE_TITAN>() != null)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    {
                        if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject.GetComponent<FEMALE_TITAN>().hitEye();
                        }
                    }
                    else if (!PhotonNetwork.isMasterClient)
                    {
                        if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            object[] objArray5 = new object[] {transform.root.gameObject.GetPhotonView().viewID};
                            gameObject.GetComponent<FEMALE_TITAN>().photonView
                                .RPC("hitEyeRPC", PhotonTargets.MasterClient, objArray5);
                        }
                    }
                    else if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        gameObject.GetComponent<FEMALE_TITAN>()
                            .HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                    }
                }
                else if (gameObject.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                    {
                        if (!gameObject.GetComponent<TITAN>().hasDie)
                        {
                            gameObject.GetComponent<TITAN>().HitEye();
                        }
                    }
                    else if (!PhotonNetwork.isMasterClient)
                    {
                        if (!gameObject.GetComponent<TITAN>().hasDie)
                        {
                            object[] objArray6 = new object[] {transform.root.gameObject.GetPhotonView().viewID};
                            gameObject.GetComponent<TITAN>().photonView
                                .RPC("hitEyeRPC", PhotonTargets.MasterClient, objArray6);
                        }
                    }
                    else if (!gameObject.GetComponent<TITAN>().hasDie)
                    {
                        gameObject.GetComponent<TITAN>().HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                    }

                    this.showCriticalHitFX();
                }
            }
        }
        else if (other.gameObject.CompareTag("titanankle") && !this.currentHits.Contains(other.gameObject))
        {
            this.currentHits.Add(other.gameObject);
            GameObject obj4 = other.gameObject.transform.root.gameObject;
            Vector3 vector10 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity -
                               obj4.rigidbody.velocity;
            int num9 = (int) (vector10.magnitude * 10f * this.scoreMulti);
            num9 = Mathf.Max(10, num9);
            if (obj4.GetComponent<TITAN>() != null &&
                obj4.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    if (!obj4.GetComponent<TITAN>().hasDie)
                    {
                        obj4.GetComponent<TITAN>().hitAnkle();
                    }
                }
                else
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        if (!obj4.GetComponent<TITAN>().hasDie)
                        {
                            object[] objArray7 = new object[] {transform.root.gameObject.GetPhotonView().viewID};
                            obj4.GetComponent<TITAN>().photonView
                                .RPC("hitAnkleRPC", PhotonTargets.MasterClient, objArray7);
                        }
                    }
                    else if (!obj4.GetComponent<TITAN>().hasDie)
                    {
                        obj4.GetComponent<TITAN>().hitAnkle();
                    }

                    this.showCriticalHitFX();
                }
            }
            else if (obj4.GetComponent<FEMALE_TITAN>() != null)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    if (other.gameObject.name == "ankleR")
                    {
                        if (obj4.GetComponent<FEMALE_TITAN>() != null && !obj4.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            obj4.GetComponent<FEMALE_TITAN>().hitAnkleR(num9);
                        }
                    }
                    else if (obj4.GetComponent<FEMALE_TITAN>() != null && !obj4.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        obj4.GetComponent<FEMALE_TITAN>().hitAnkleL(num9);
                    }
                }
                else if (other.gameObject.name == "ankleR")
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            object[] objArray8 = new object[] {transform.root.gameObject.GetPhotonView().viewID, num9};
                            obj4.GetComponent<FEMALE_TITAN>().photonView
                                .RPC("hitAnkleRRPC", PhotonTargets.MasterClient, objArray8);
                        }
                    }
                    else if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        obj4.GetComponent<FEMALE_TITAN>()
                            .HitAnkleRRPC(transform.root.gameObject.GetPhotonView().viewID, num9);
                    }
                }
                else if (!PhotonNetwork.isMasterClient)
                {
                    if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        object[] objArray9 = new object[] {transform.root.gameObject.GetPhotonView().viewID, num9};
                        obj4.GetComponent<FEMALE_TITAN>().photonView
                            .RPC("hitAnkleLRPC", PhotonTargets.MasterClient, objArray9);
                    }
                }
                else if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                {
                    obj4.GetComponent<FEMALE_TITAN>()
                        .HitAnkleLRPC(transform.root.gameObject.GetPhotonView().viewID, num9);
                }

                this.showCriticalHitFX();
            }
        }
    }

    private void showCriticalHitFX()
    {
        GameObject obj2;
        this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f, 0.95f);
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

