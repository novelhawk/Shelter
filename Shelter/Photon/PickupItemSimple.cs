using JetBrains.Annotations;
using Mod;
using Photon;
using Photon.Enums;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
// ReSharper disable once CheckNamespace
public class PickupItemSimple : Photon.MonoBehaviour
{
    public bool PickupOnCollide;
    public float SecondsBeforeRespawn = 2f;
    public bool SentPickup;

    public void OnTriggerEnter(Collider other)
    {
        PhotonView component = other.GetComponent<PhotonView>();
        if (this.PickupOnCollide && component != null && component.isMine)
        {
            this.Pickup();
        }
    }

    public void Pickup()
    {
        if (!this.SentPickup)
        {
            this.SentPickup = true;
            photonView.RPC(Rpc.PunPickupSimple, PhotonTargets.AllViaServer);
        }
    }

    [RPC]
    [UsedImplicitly]
    public void PunPickupSimple(PhotonMessageInfo msgInfo)
    {
        if (!this.SentPickup || !msgInfo.sender.IsLocal || !gameObject.GetActive())
        {
        }
        this.SentPickup = false;
        if (!gameObject.GetActive())
        {
            Debug.Log("Ignored PU RPC, cause item is inactive. " + gameObject);
        }
        else
        {
            double num = PhotonNetwork.time - msgInfo.timestamp;
            float time = this.SecondsBeforeRespawn - (float) num;
            if (time > 0f)
            {
                gameObject.SetActive(false);
                Invoke(nameof(RespawnAfter), time);
            }
        }
    }

    public void RespawnAfter()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(true);
        }
    }
}

