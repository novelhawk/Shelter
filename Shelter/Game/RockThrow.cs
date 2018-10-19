using Mod;
using Photon;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class RockThrow : Photon.MonoBehaviour
{
    private bool launched;
    private Vector3 oldP;
    private Vector3 r;
    private Vector3 v;

    private void explore()
    {
        GameObject obj2;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            obj2 = PhotonNetwork.Instantiate("FX/boom6", transform.position, transform.rotation, 0);
            if (transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
            {
                obj2.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                obj2.GetComponent<EnemyfxIDcontainer>().titanName = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
            }
        }
        else
        {
            obj2 = (GameObject) Instantiate(Resources.Load("FX/boom6"), transform.position, transform.rotation);
        }
        obj2.transform.localScale = transform.localScale;
        float b = 1f - Vector3.Distance(GameObject.Find("MainCamera").transform.position, obj2.transform.position) * 0.05f;
        b = Mathf.Min(1f, b);
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b, 0.95f);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            Destroy(gameObject);
        }
        else
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    private void hitPlayer(GameObject hero)
    {
        if (hero != null && !hero.GetComponent<HERO>().HasDied() && !hero.GetComponent<HERO>().isInvincible())
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                if (!hero.GetComponent<HERO>().IsGrabbed)
                {
                    hero.GetComponent<HERO>().die(this.v.normalized * 1000f + Vector3.up * 50f, false);
                }
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && !hero.GetComponent<HERO>().HasDied() && !hero.GetComponent<HERO>().IsGrabbed)
            {
                hero.GetComponent<HERO>().markDie();
                int myOwnerViewID = -1;
                string titanName = string.Empty;
                if (transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
                {
                    myOwnerViewID = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                    titanName = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                }
                Debug.Log("rock hit player " + titanName);
                hero.GetComponent<HERO>().photonView.RPC(Rpc.Die, PhotonTargets.All, this.v.normalized * 1000f + Vector3.up * 50f, false, myOwnerViewID, titanName, true);
            }
        }
    }

    [RPC]
    private void InitRPC(int viewID, Vector3 scale, Vector3 pos, float level)
    {
        GameObject gameObject = PhotonView.Find(viewID).gameObject;
        Transform transform = gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        this.transform.localScale = gameObject.transform.localScale;
        this.transform.parent = transform;
        this.transform.localPosition = pos;
    }

    public void launch(Vector3 v1)
    {
        this.launched = true;
        this.oldP = transform.position;
        this.v = v1;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            photonView.RPC(Rpc.ThrowRock, PhotonTargets.Others, this.v, this.oldP);
        }
    }

    [RPC]
    private void LaunchRPC(Vector3 v, Vector3 p)
    {
        this.launched = true;
        Vector3 vector = p;
        transform.position = vector;
        this.oldP = vector;
        transform.parent = null;
        this.launch(v);
    }

    private void Start()
    {
        this.r = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }

    private void Update()
    {
        if (this.launched)
        {
            this.transform.Rotate(this.r);
            this.v -= 20f * Vector3.up * Time.deltaTime;
            Transform transform = this.transform;
            transform.position += this.v * Time.deltaTime;
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer || PhotonNetwork.isMasterClient)
            {
                LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
                LayerMask mask2 = 1 << LayerMask.NameToLayer("Players");
                LayerMask mask3 = 1 << LayerMask.NameToLayer("EnemyAABB");
                LayerMask mask4 = mask2 | mask | mask3;
                foreach (RaycastHit hit in Physics.SphereCastAll(this.transform.position, 2.5f * this.transform.lossyScale.x, this.transform.position - this.oldP, Vector3.Distance(this.transform.position, this.oldP), mask4))
                {
                    if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "EnemyAABB")
                    {
                        GameObject gameObject = hit.collider.gameObject.transform.root.gameObject;
                        if (gameObject.GetComponent<TITAN>() != null && !gameObject.GetComponent<TITAN>().hasDie)
                        {
                            gameObject.GetComponent<TITAN>().hitAnkle();
                            Vector3 position = this.transform.position;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                gameObject.GetComponent<TITAN>().hitAnkle();
                            }
                            else
                            {
                                if (this.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null && PhotonView.Find(this.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID) != null)
                                {
                                    Vector3 vector3 = PhotonView.Find(this.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID).transform.position;
                                }
                                gameObject.GetComponent<HERO>().photonView.RPC(Rpc.HitAnkle, PhotonTargets.All, new object[0]);
                            }
                        }
                        this.explore();
                    }
                    else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Players")
                    {
                        GameObject hero = hit.collider.gameObject.transform.root.gameObject;
                        if (hero.GetComponent<TITAN_EREN>() != null)
                        {
                            if (!hero.GetComponent<TITAN_EREN>().isHit)
                            {
                                hero.GetComponent<TITAN_EREN>().hitByTitan();
                            }
                        }
                        else if (hero.GetComponent<HERO>() != null && !hero.GetComponent<HERO>().isInvincible())
                        {
                            this.hitPlayer(hero);
                        }
                    }
                    else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Ground")
                    {
                        this.explore();
                    }
                }
                this.oldP = this.transform.position;
            }
        }
    }
}

