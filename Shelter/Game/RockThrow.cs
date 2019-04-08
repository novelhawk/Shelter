using JetBrains.Annotations;
using Mod;
using Mod.Exceptions;
using Photon;
using Photon.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class RockThrow : Photon.MonoBehaviour
{
    private bool launched;
    
    /// <summary>
    /// Rock old position
    /// </summary>
    private Vector3 oldP;
    private Vector3 r;
    
    /// <summary>
    /// Rock position
    /// </summary>
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
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b);
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
        if (hero != null && !hero.GetComponent<HERO>().HasDied() && !hero.GetComponent<HERO>().IsInvincible)
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
    [UsedImplicitly]
    private void InitRPC(int viewID, Vector3 scale, Vector3 pos, float level, PhotonMessageInfo info)
    {
        if (!PhotonView.TryParse(viewID, out var view))
            throw new NotAllowedException(nameof(InitRPC), info);
        
        GameObject go = view.gameObject;
        Transform tr = go.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        this.transform.localScale = go.transform.localScale;
        this.transform.parent = tr;
        this.transform.localPosition = pos;
    }

    public void launch(Vector3 position)
    {
        this.launched = true;
        this.oldP = transform.position;
        this.v = position;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
        {
            photonView.RPC(Rpc.ThrowRock, PhotonTargets.Others, this.v, this.oldP);
        }
    }

    /// <summary>
    /// Launches the rock
    /// </summary>
    /// <param name="position">Position to launch the rock from</param>
    /// <param name="_">Old rock position; Unused.</param>
    /// <param name="info">RPC message info</param>
    [RPC]
    [UsedImplicitly]
    private void LaunchRPC(Vector3 position, Vector3 _, PhotonMessageInfo info)
    {
        if (launched)
            throw new NotAllowedException(nameof(LaunchRPC), info);
        launched = true;
        
        transform.position = position;
        transform.parent = null;
        
        oldP = position;
        launch(position);
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
                        GameObject go = hit.collider.gameObject.transform.root.gameObject;
                        if (go.GetComponent<TITAN>() != null && !go.GetComponent<TITAN>().hasDie)
                        {
                            go.GetComponent<TITAN>().hitAnkle();
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                go.GetComponent<TITAN>().hitAnkle();
                            }
                            else
                            {
                                go.GetComponent<HERO>().photonView.RPC(Rpc.HitAnkle, PhotonTargets.All);
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
                        else if (hero.GetComponent<HERO>() != null && !hero.GetComponent<HERO>().IsInvincible)
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

