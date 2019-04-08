using System.Collections;
using Mod;
using Mod.GameSettings;
using Photon;
using Photon.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
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
            photonView.observed = this;
            this.correctPlayerPos = transform.position;
            this.correctPlayerRot = Quaternion.identity;
            Player owner = photonView.owner;
            if (GameManager.settings.TeamSort > TeamSort.Off)
            {
                var num = owner.Properties.RCTeam;
                switch (num)
                {
                    case 1:
                        GetComponent<ParticleSystem>().startColor = Color.cyan;
                        break;
                    case 2:
                        GetComponent<ParticleSystem>().startColor = Color.magenta;
                        break;
                    default:
                        GetComponent<ParticleSystem>().startColor = owner.Properties.RCBombColor;
                        break;
                }
            }
            else
            {
                GetComponent<ParticleSystem>().startColor = owner.Properties.RCBombColor;
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
        foreach (HERO hero in GameManager.instance.GetPlayers())
        {
            GameObject go = hero.gameObject;
            if (!hero.RCBombImmune && Vector3.Distance(go.transform.position, position) < radius && !go.GetPhotonView().isMine)
            {
                Player owner = go.GetPhotonView().owner;
                if (GameManager.settings.TeamSort > TeamSort.Off)
                {
                    if (Player.Self.Properties.RCTeam == 0 || Player.Self.Properties.RCTeam != owner.Properties.RCTeam)
                    {
                        go.GetComponent<HERO>().markDie();
                        go.GetComponent<HERO>().photonView.RPC(Rpc.DieRC, PhotonTargets.All, -1, Player.Self.Properties.Name + " ");
                        GameManager.instance.PlayerKillInfoUpdate(Player.Self, 0);
                    }
                }
                else
                {
                    go.GetComponent<HERO>().markDie();
                    go.GetComponent<HERO>().photonView.RPC(Rpc.DieRC, PhotonTargets.All, -1, Player.Self.Properties.Name + " ");
                    GameManager.instance.PlayerKillInfoUpdate(Player.Self, 0);
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