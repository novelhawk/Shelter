using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class RockScript : MonoBehaviour
{
    private Vector3 desPt = new Vector3(-200f, 0f, -280f);
    private bool disable;
    private float g = 500f;
    private float speed = 800f;
    private Vector3 vh;
    private Vector3 vv;

    private void Start()
    {
        transform.position = new Vector3(0f, 0f, 676f);
        this.vh = this.desPt - transform.position;
        this.vv = new Vector3(0f, this.g * this.vh.magnitude / (2f * this.speed), 0f);
        this.vh.Normalize();
        this.vh = this.vh * this.speed;
    }

    private void Update()
    {
        if (!this.disable)
        {
            this.vv += -Vector3.up * this.g * Time.deltaTime;
            Transform transform = this.transform;
            transform.position += this.vv * Time.deltaTime;
            Transform transform2 = this.transform;
            transform2.position += this.vh * Time.deltaTime;
            if (Vector3.Distance(this.desPt, this.transform.position) < 20f || this.transform.position.y < 0f)
            {
                this.transform.position = this.desPt;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && PhotonNetwork.isMasterClient)
                {
                    if (GameManager.LAN)
                    {
                        Network.Instantiate(Resources.Load("FX/boom1_CT_KICK"), this.transform.position + Vector3.up * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
                    }
                    else
                    {
                        PhotonNetwork.Instantiate("FX/boom1_CT_KICK", this.transform.position + Vector3.up * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
                    }
                }
                else
                {
                    Instantiate(Resources.Load("FX/boom1_CT_KICK"), this.transform.position + Vector3.up * 30f, Quaternion.Euler(270f, 0f, 0f));
                }
                this.disable = true;
            }
        }
    }
}

