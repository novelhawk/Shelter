using Mod;
using Photon;
using UnityEngine;

public class EnemyCheckCollider : Photon.MonoBehaviour
{
    public bool active_me;
    private int count;
    public int dmg = 1;
    public bool isThisBite;

    private void FixedUpdate()
    {
        if (this.count > 1)
        {
            this.active_me = false;
        }
        else
        {
            this.count++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && !transform.root.gameObject.GetPhotonView().isMine || !active_me) 
            return;
        
        if (other.gameObject.CompareTag("playerHitbox"))
        {
            float dist = 1f - Vector3.Distance(other.gameObject.transform.position, transform.position) * 0.05f;
            dist = Mathf.Min(1f, dist);
            HitBox component = other.gameObject.GetComponent<HitBox>();
            if (component != null && component.transform.root != null)
            {
                if (this.dmg == 0)
                {
                    Vector3 vector = component.transform.root.transform.position - transform.position;
                    float multiplier = 0f;
                    if (gameObject.GetComponent<SphereCollider>() != null)
                        multiplier = transform.localScale.x * gameObject.GetComponent<SphereCollider>().radius;
                    if (gameObject.GetComponent<CapsuleCollider>() != null)
                        multiplier = transform.localScale.x * gameObject.GetComponent<CapsuleCollider>().height;
                    if (multiplier > 0f)
                        multiplier = Mathf.Max(5f, 5f - vector.magnitude);
                    switch (IN_GAME_MAIN_CAMERA.GameType)
                    {
                        case GameType.Singleplayer:
                            component.transform.root.GetComponent<HERO>().BlowAway(vector.normalized * multiplier + Vector3.up * 1f);
                            break;
                        case GameType.Multiplayer:
                            component.transform.root.GetComponent<HERO>().photonView.RPC(Rpc.BlowAway, PhotonTargets.All, 
                                vector.normalized * multiplier + Vector3.up * 1f);
                            break;
                    }
                }
                else if (!component.transform.root.GetComponent<HERO>().isInvincible() && 
                         !component.transform.root.GetComponent<HERO>().IsGrabbed && 
                         !component.transform.root.GetComponent<HERO>().HasDied())
                {
                    component.transform.root.GetComponent<HERO>().markDie();
                    string titanName = string.Empty;
                    int myOwnerViewID = -1;
                    if (transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
                    {
                        myOwnerViewID = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                        titanName = transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                    }
                    Vector3 distance = component.transform.root.position - transform.position;
                    component.transform.root.GetComponent<HERO>().photonView.RPC(Rpc.Die, PhotonTargets.All, 
                        distance.normalized * dist * 1000f + Vector3.up * 50f,
                        this.isThisBite,
                        myOwnerViewID,
                        titanName,
                        true);
                }
            }
        }
        else if (other.gameObject.CompareTag("erenHitbox") && this.dmg > 0 && !other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().isHit)
        {
            other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByTitan();
        }
    }

    private void Start()
    {
        this.active_me = true;
        this.count = 0;
    }
}

