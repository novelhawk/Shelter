using System.Collections;
using System.Collections.Generic;
using Mod;
using Mod.GameSettings;
using Photon;
using UnityEngine;

public class CannonBall : Photon.MonoBehaviour
{
    private Vector3 correctPos;
    private Vector3 correctVelocity;
    public bool disabled;
    public Transform firingPoint;
    public bool isCollider;
    public HERO myHero;
    public List<TitanTrigger> myTitanTriggers;
    public float SmoothingDelay = 10f;

    private void Awake()
    {
        if (photonView != null)
        {
            photonView.observed = this;
            this.correctPos = transform.position;
            this.correctVelocity = Vector3.zero;
            GetComponent<SphereCollider>().enabled = false;
            if (photonView.isMine)
            {
                StartCoroutine(this.WaitAndDestroy(10f));
                this.myTitanTriggers = new List<TitanTrigger>();
            }
        }
    }

    private void destroyMe()
    {
        if (!this.disabled)
        {
            this.disabled = true;
            foreach (EnemyCheckCollider collider in PhotonNetwork.Instantiate("FX/boom4", transform.position, transform.rotation, 0).GetComponentsInChildren<EnemyCheckCollider>())
            {
                collider.dmg = 0;
            }
            if (FengGameManagerMKII.settings.AllowCannonHumanKills)
            {
                foreach (HERO hero in FengGameManagerMKII.instance.GetPlayers())
                {
                    if (hero != null && Vector3.Distance(hero.transform.position, transform.position) <= 20f && !hero.photonView.isMine)
                    {
                        GameObject gameObject = hero.gameObject;
                        Player owner = gameObject.GetPhotonView().owner;
                        if (FengGameManagerMKII.settings.TeamSort > TeamSort.Off)
                        {
                            if (Player.Self.Properties.RCTeam == 0 || Player.Self.Properties.RCTeam != owner.Properties.RCTeam)
                            {
                                gameObject.GetComponent<HERO>().markDie();
                                gameObject.GetComponent<HERO>().photonView.RPC(Rpc.DieRC, PhotonTargets.All, -1, Player.Self.Properties.Name + " ");
                                FengGameManagerMKII.instance.PlayerKillInfoUpdate(Player.Self, 0);
                            }
                        }
                        else
                        {
                            gameObject.GetComponent<HERO>().markDie();
                            gameObject.GetComponent<HERO>().photonView.RPC(Rpc.DieRC, PhotonTargets.All, -1, Player.Self.Properties.Name + " ");
                            FengGameManagerMKII.instance.PlayerKillInfoUpdate(Player.Self, 0);
                        }
                    }
                }
            }
            if (this.myTitanTriggers != null)
            {
                foreach (var triggers in this.myTitanTriggers)
                {
                    if (triggers != null)
                    {
                        triggers.isCollide = false;
                    }
                }
            }
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public void FixedUpdate()
    {
        if (photonView.isMine && !this.disabled)
        {
            LayerMask mask = 1 << LayerMask.NameToLayer("PlayerAttackBox");
            LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
            LayerMask mask3 = mask | mask2;
            if (!this.isCollider)
            {
                LayerMask mask4 = 1 << LayerMask.NameToLayer("Ground");
                mask3 |= mask4;
            }
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, 0.6f, mask3.value);
            bool flag2 = false;
            for (int i = 0; i < colliderArray.Length; i++)
            {
                GameObject gameObject = colliderArray[i].gameObject;
                if (gameObject.layer == 16)
                {
                    TitanTrigger component = gameObject.GetComponent<TitanTrigger>();
                    if (!(component == null || this.myTitanTriggers.Contains(component)))
                    {
                        component.isCollide = true;
                        this.myTitanTriggers.Add(component);
                    }
                }
                else if (gameObject.layer == 10)
                {
                    TITAN titan = gameObject.transform.root.gameObject.GetComponent<TITAN>();
                    if (titan != null)
                    {
                        if (titan.abnormalType == AbnormalType.Crawler)
                        {
                            if (gameObject.name == "head")
                            {
                                titan.photonView.RPC(Rpc.DieByCannon, titan.photonView.owner, new object[] { this.myHero.photonView.viewID });
                                titan.dieBlow(transform.position, 0.2f);
                                i = colliderArray.Length;
                            }
                        }
                        else if (gameObject.name == "head")
                        {
                            titan.photonView.RPC(Rpc.DieByCannon, titan.photonView.owner, new object[] { this.myHero.photonView.viewID });
                            titan.dieHeadBlow(transform.position, 0.2f);
                            i = colliderArray.Length;
                        }
                        else if (Random.Range(0f, 1f) < 0.5f)
                        {
                            titan.HitLeft(transform.position, 0.05f);
                        }
                        else
                        {
                            titan.HitRight(transform.position, 0.05f);
                        }
                        this.destroyMe();
                    }
                }
                else if (gameObject.layer == 9 && (gameObject.transform.root.name.Contains("CannonWall") || gameObject.transform.root.name.Contains("CannonGround")))
                {
                    flag2 = true;
                }
            }
            if (!(this.isCollider || flag2))
            {
                this.isCollider = true;
                GetComponent<SphereCollider>().enabled = true;
            }
        }
    }

    public void OnCollisionEnter(Collision myCollision)
    {
        if (photonView.isMine)
        {
            this.destroyMe();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rigidbody.velocity);
        }
        else
        {
            this.correctPos = (Vector3) stream.ReceiveNext();
            this.correctVelocity = (Vector3) stream.ReceiveNext();
        }
    }

    public void Update()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * this.SmoothingDelay);
            rigidbody.velocity = this.correctVelocity;
        }
    }

    public IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        this.destroyMe();
    }
}