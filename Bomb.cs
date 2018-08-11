using System.Collections;
using UnityEngine;

public class Bomb : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Vector3 correctPlayerVelocity = Vector3.zero;
    public bool disabled;
    public GameObject myExplosion;
    public float SmoothingDelay = 10f;

    public void Awake()
    {
        if (photonView != null)
        {
            float num2;
            float num3;
            float num4;
            photonView.observed = this;
            this.correctPlayerPos = transform.position;
            this.correctPlayerRot = Quaternion.identity;
            Player owner = photonView.owner;
            if (RCSettings.teamMode > 0)
            {
                int num = RCextensions.returnIntFromObject(owner.Properties[PhotonPlayerProperty.RCteam]);
                if (num == 1)
                {
                    GetComponent<ParticleSystem>().startColor = Color.cyan;
                }
                else if (num == 2)
                {
                    GetComponent<ParticleSystem>().startColor = Color.magenta;
                }
                else
                {
                    num2 = RCextensions.returnFloatFromObject(owner.Properties[PhotonPlayerProperty.RCBombR]);
                    num3 = RCextensions.returnFloatFromObject(owner.Properties[PhotonPlayerProperty.RCBombG]);
                    num4 = RCextensions.returnFloatFromObject(owner.Properties[PhotonPlayerProperty.RCBombB]);
                    GetComponent<ParticleSystem>().startColor = new Color(num2, num3, num4, 1f);
                }
            }
            else
            {
                num2 = RCextensions.returnFloatFromObject(owner.Properties[PhotonPlayerProperty.RCBombR]);
                num3 = RCextensions.returnFloatFromObject(owner.Properties[PhotonPlayerProperty.RCBombG]);
                num4 = RCextensions.returnFloatFromObject(owner.Properties[PhotonPlayerProperty.RCBombB]);
                GetComponent<ParticleSystem>().startColor = new Color(num2, num3, num4, 1f);
            }
        }
    }

    public void destroyMe()
    {
        if (photonView.isMine)
        {
            if (this.myExplosion != null)
            {
                PhotonNetwork.Destroy(this.myExplosion);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Explode(float radius)
    {
        this.disabled = true;
        rigidbody.velocity = Vector3.zero;
        Vector3 position = transform.position;
        this.myExplosion = PhotonNetwork.Instantiate("RCAsset/BombExplodeMain", position, Quaternion.Euler(0f, 0f, 0f), 0);
        foreach (HERO hero in FengGameManagerMKII.instance.GetPlayers())
        {
            GameObject gameObject = hero.gameObject;
            if (Vector3.Distance(gameObject.transform.position, position) < radius && !gameObject.GetPhotonView().isMine && !hero.bombImmune)
            {
                Player owner = gameObject.GetPhotonView().owner;
                if (RCSettings.teamMode > 0 && Player.Self.Properties[PhotonPlayerProperty.RCteam] != null && owner.Properties[PhotonPlayerProperty.RCteam] != null)
                {
                    int num = RCextensions.returnIntFromObject(Player.Self.Properties[PhotonPlayerProperty.RCteam]);
                    int num2 = RCextensions.returnIntFromObject(owner.Properties[PhotonPlayerProperty.RCteam]);
                    if (num == 0 || num != num2)
                    {
                        gameObject.GetComponent<HERO>().markDie();
                        gameObject.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, RCextensions.returnStringFromObject(Player.Self.Properties[PhotonPlayerProperty.name]) + " " });
                        FengGameManagerMKII.instance.PlayerKillInfoUpdate(Player.Self, 0);
                    }
                }
                else
                {
                    gameObject.GetComponent<HERO>().markDie();
                    gameObject.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, RCextensions.returnStringFromObject(Player.Self.Properties[PhotonPlayerProperty.name]) + " " });
                    FengGameManagerMKII.instance.PlayerKillInfoUpdate(Player.Self, 0);
                }
            }
        }
        StartCoroutine(this.WaitAndFade(1.5f));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigidbody.velocity);
        }
        else
        {
            this.correctPlayerPos = (Vector3) stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion) stream.ReceiveNext();
            this.correctPlayerVelocity = (Vector3) stream.ReceiveNext();
        }
    }

    public void Update()
    {
        if (!(this.disabled || photonView.isMine))
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            rigidbody.velocity = this.correctPlayerVelocity;
        }
    }

    private IEnumerator WaitAndFade(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(this.myExplosion);
        PhotonNetwork.Destroy(this.gameObject);
    }
}