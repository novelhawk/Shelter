using System.Collections;
using UnityEngine;

public class AHSSShotGunCollider : MonoBehaviour
{
    public bool active_me;
    private int count;
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

    private void FixedUpdate()
    {
        if (!active_me)
            return;
        
        active_me = count++ > 1;
    }


    public bool BypassPVP = true;
    private void OnTriggerStay(Collider other)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && !transform.root.gameObject.GetPhotonView().isMine || !active_me) 
            return;
        
        
        if (other.gameObject.CompareTag("playerHitbox"))
        {
            if (BypassPVP || LevelInfoManager.GetInfo(FengGameManagerMKII.Level).IsPvP)
            {
                float b = 1f - Vector3.Distance(other.gameObject.transform.position, transform.position) * 0.05f;
                b = Mathf.Min(1f, b);
                HitBox component = other.gameObject.GetComponent<HitBox>();
                if (component != null && component.transform.root != null && component.transform.root.GetComponent<HERO>().myTeam != this.myTeam && !component.transform.root.GetComponent<HERO>().isInvincible())
                {
                    switch (IN_GAME_MAIN_CAMERA.gametype)
                    {
                        case GAMETYPE.SINGLE:
                            if (!component.transform.root.GetComponent<HERO>().IsGrabbed)
                            {
                                Vector3 vector = component.transform.root.transform.position - transform.position;
                                component.transform.root.GetComponent<HERO>().die(vector.normalized * b * 1000f + Vector3.up * 50f, false);
                            }

                            break;
                        case GAMETYPE.MULTIPLAYER when !component.transform.root.GetComponent<HERO>().HasDied() && !component.transform.root.GetComponent<HERO>().IsGrabbed:
                            component.transform.root.GetComponent<HERO>().markDie();
                            Vector3 distance = component.transform.root.position - transform.position;
                            component.transform.root.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, 
                                distance.normalized * b * 1000f + Vector3.up * 50f, // force : Vector3
                                false,                                              // isBite : bool
                                this.viewID,                                        // viewID : int
                                this.ownerName,                                     // titanName : string
                                false);                                             // killByTitan : bool
                            break;
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
            if (item != null && this.CheckIfBehind(item.transform.root.gameObject) && !this.currentHits.Contains(item))
            {
                item.hitPosition = (transform.position + item.transform.position) * 0.5f;
                this.currentHits.Add(item);
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                {
                    if (item.transform.root.GetComponent<TITAN>() != null && !item.transform.root.GetComponent<TITAN>().hasDie)
                    {
                        Vector3 vector3 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int num2 = (int) (vector3.magnitude * 10f * this.scoreMulti);
                        num2 = Mathf.Max(10, num2);
                        FengGameManagerMKII.instance.NetShowDamage(num2);
                        if (num2 > item.transform.root.GetComponent<TITAN>().myLevel * 100f)
                        {
                            item.transform.root.GetComponent<TITAN>().die();
                            if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            {
                                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(item.transform.position, num2, item.transform.root.gameObject, 0.02f);
                            }
                            FengGameManagerMKII.instance.PlayerKillInfoSingleplayerUpdate(num2);
                        }
                    }
                }
                else if (!PhotonNetwork.isMasterClient)
                {
                    if (item.transform.root.GetComponent<TITAN>() != null)
                    {
                        if (!item.transform.root.GetComponent<TITAN>().hasDie)
                        {
                            Vector3 vector4 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                            int num3 = (int) (vector4.magnitude * 10f * this.scoreMulti);
                            num3 = Mathf.Max(10, num3);
                            if (num3 > item.transform.root.GetComponent<TITAN>().myLevel * 100f)
                            {
                                if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                                {
                                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(item.transform.position, num3, item.transform.root.gameObject, 0.02f);
                                    item.transform.root.GetComponent<TITAN>().asClientLookTarget = false;
                                }
                                object[] objArray2 = new object[] { transform.root.gameObject.GetPhotonView().viewID, num3 };
                                item.transform.root.GetComponent<TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<TITAN>().photonView.owner, objArray2);
                            }
                        }
                    }
                    else if (item.transform.root.GetComponent<FEMALE_TITAN>() != null)
                    {
                        Vector3 vector5 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int num4 = (int) (vector5.magnitude * 10f * this.scoreMulti);
                        num4 = Mathf.Max(10, num4);
                        if (!item.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            object[] objArray3 = new object[] { transform.root.gameObject.GetPhotonView().viewID, num4 };
                            item.transform.root.GetComponent<FEMALE_TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<FEMALE_TITAN>().photonView.owner, objArray3);
                        }
                    }
                    else if (item.transform.root.GetComponent<COLOSSAL_TITAN>() != null && !item.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                    {
                        Vector3 vector6 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int num5 = (int) (vector6.magnitude * 10f * this.scoreMulti);
                        num5 = Mathf.Max(10, num5);
                        object[] objArray4 = new object[] { transform.root.gameObject.GetPhotonView().viewID, num5 };
                        item.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.RPC("titanGetHit", item.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.owner, objArray4);
                    }
                }
                else if (item.transform.root.GetComponent<TITAN>() != null)
                {
                    if (!item.transform.root.GetComponent<TITAN>().hasDie)
                    {
                        Vector3 vector7 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int num6 = (int) (vector7.magnitude * 10f * this.scoreMulti);
                        num6 = Mathf.Max(10, num6);
                        if (num6 > item.transform.root.GetComponent<TITAN>().myLevel * 100f)
                        {
                            if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                            {
                                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(item.transform.position, num6, item.transform.root.gameObject, 0.02f);
                            }
                            item.transform.root.GetComponent<TITAN>().TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num6);
                        }
                    }
                }
                else if (item.transform.root.GetComponent<FEMALE_TITAN>() != null)
                {
                    if (!item.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        Vector3 vector8 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                        int num7 = (int) (vector8.magnitude * 10f * this.scoreMulti);
                        num7 = Mathf.Max(10, num7);
                        if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                        {
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(item.transform.position, num7, null, 0.02f);
                        }
                        item.transform.root.GetComponent<FEMALE_TITAN>().TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num7);
                    }
                }
                else if (item.transform.root.GetComponent<COLOSSAL_TITAN>() != null && !item.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                {
                    Vector3 vector9 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - item.transform.root.rigidbody.velocity;
                    int num8 = (int) (vector9.magnitude * 10f * this.scoreMulti);
                    num8 = Mathf.Max(10, num8);
                    if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
                    {
                        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(item.transform.position, num8, null, 0.02f);
                    }
                    item.transform.root.GetComponent<COLOSSAL_TITAN>().TitanGetHit(transform.root.gameObject.GetPhotonView().viewID, num8);
                }
                this.ShowCriticalHitFX(other.gameObject.transform.position);
            }
        }
        else if (other.gameObject.CompareTag("titaneye"))
        {
            if (!this.currentHits.Contains(other.gameObject))
            {
                this.currentHits.Add(other.gameObject);
                GameObject gameObject = other.gameObject.transform.root.gameObject;
                if (gameObject.GetComponent<FEMALE_TITAN>() != null && !gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                        gameObject.GetComponent<FEMALE_TITAN>().hitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        gameObject.GetComponent<FEMALE_TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    else
                        gameObject.GetComponent<FEMALE_TITAN>().HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                }
                else if (gameObject.GetComponent<TITAN>() != null && gameObject.GetComponent<TITAN>().abnormalType != AbnormalType.TYPE_CRAWLER && !gameObject.GetComponent<TITAN>().hasDie)
                {
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                        gameObject.GetComponent<TITAN>().hitEye();
                    else if (!PhotonNetwork.isMasterClient)
                        gameObject.GetComponent<TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                    else if (!gameObject.GetComponent<TITAN>().hasDie)
                        gameObject.GetComponent<TITAN>().HitEyeRPC(transform.root.gameObject.GetPhotonView().viewID);
                    
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
            if (obj.GetComponent<TITAN>() != null && obj.GetComponent<TITAN>().abnormalType != AbnormalType.TYPE_CRAWLER && !obj.GetComponent<TITAN>().hasDie)
            {
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    obj.GetComponent<TITAN>().hitAnkle();
                else if (!PhotonNetwork.isMasterClient)
                    obj.GetComponent<TITAN>().photonView.RPC("hitAnkleRPC", PhotonTargets.MasterClient, transform.root.gameObject.GetPhotonView().viewID);
                else
                    obj.GetComponent<TITAN>().hitAnkle();
                
                ShowCriticalHitFX(other.gameObject.transform.position);
            }
            else if (obj.GetComponent<FEMALE_TITAN>() != null && !obj.GetComponent<FEMALE_TITAN>().hasDie)
            {
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
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
        
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            Instantiate(Resources.Load("redCross1"), position, Quaternion.Euler(270f, 0f, 0f));
        else
            PhotonNetwork.Instantiate("redCross1", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
    }

    private void Start()
    {
        switch (IN_GAME_MAIN_CAMERA.gametype)
        {
            case GAMETYPE.SINGLE:
                this.myTeam = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>().myTeam;
                break;
            
            case GAMETYPE.MULTIPLAYER when !transform.root.gameObject.GetPhotonView().isMine:
                enabled = false;
                return;
            
            case GAMETYPE.MULTIPLAYER:
                if (transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
                {
                    this.viewID = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                    this.ownerName = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                    this.myTeam = PhotonView.Find(this.viewID).gameObject.GetComponent<HERO>().myTeam;
                }
                break;
        }

        this.active_me = true;
        this.count = 0;
        this.currentCamera = GameObject.Find("MainCamera");
    }
}

