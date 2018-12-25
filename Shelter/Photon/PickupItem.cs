using System.Collections.Generic;
using JetBrains.Annotations;
using Mod;
using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

[RequireComponent(typeof(PhotonView))]
// ReSharper disable once CheckNamespace
public class PickupItem : Photon.MonoBehaviour, IPunObservable
{
    public static HashSet<PickupItem> DisabledPickupItems = new HashSet<PickupItem>();
    public MonoBehaviour OnPickedUpCall;
    public bool PickupIsMine;
    public bool PickupOnTrigger;
    public float SecondsBeforeRespawn = 2f;
    public bool SentPickup;
    public double TimeOfRespawn;

    public void Drop()
    {
        if (this.PickupIsMine)
        {
            photonView.RPC(Rpc.PunRespawn, PhotonTargets.AllViaServer);
        }
    }

    public void Drop(Vector3 newPosition)
    {
        if (this.PickupIsMine)
        {
            photonView.RPC(Rpc.PunRespawn, PhotonTargets.AllViaServer, newPosition);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting && this.SecondsBeforeRespawn <= 0f)
        {
            stream.SendNext(gameObject.transform.position);
        }
        else
        {
            Vector3 vector = (Vector3) stream.ReceiveNext();
            gameObject.transform.position = vector;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        PhotonView component = other.GetComponent<PhotonView>();
        if (this.PickupOnTrigger && component != null && component.isMine)
        {
            this.Pickup();
        }
    }

    internal void PickedUp(float timeUntilRespawn)
    {
        gameObject.SetActive(false);
        DisabledPickupItems.Add(this);
        this.TimeOfRespawn = 0.0;
        if (timeUntilRespawn > 0f)
        {
            this.TimeOfRespawn = PhotonNetwork.time + timeUntilRespawn;
            Invoke(nameof(PunRespawn), timeUntilRespawn);
        }
    }

    public void Pickup()
    {
        if (!this.SentPickup)
        {
            this.SentPickup = true;
            photonView.RPC(Rpc.PunPickup, PhotonTargets.AllViaServer);
        }
    }

    [RPC]
    [UsedImplicitly]
    public void PunPickup(PhotonMessageInfo msgInfo)
    {
        if (msgInfo.sender.IsLocal)
        {
            this.SentPickup = false;
        }
        if (!gameObject.GetActive())
        {
            Debug.Log(string.Concat("Ignored PU RPC, cause item is inactive. ", gameObject, " SecondsBeforeRespawn: ", this.SecondsBeforeRespawn, " TimeOfRespawn: ", this.TimeOfRespawn, " respawn in future: ", this.TimeOfRespawn > PhotonNetwork.time));
        }
        else
        {
            this.PickupIsMine = msgInfo.sender.IsLocal;
            if (this.OnPickedUpCall != null)
            {
                this.OnPickedUpCall.SendMessage("OnPickedUp", this);
            }
            if (this.SecondsBeforeRespawn <= 0f)
            {
                this.PickedUp(0f);
            }
            else
            {
                double num = PhotonNetwork.time - msgInfo.timestamp;
                double num2 = this.SecondsBeforeRespawn - num;
                if (num2 > 0.0)
                {
                    this.PickedUp((float) num2);
                }
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    internal void PunRespawn()
    {
        DisabledPickupItems.Remove(this);
        this.TimeOfRespawn = 0.0;
        this.PickupIsMine = false;
        if (gameObject != null)
        {
            gameObject.SetActive(true);
        }
    }

    [RPC]
    [UsedImplicitly]
    internal void PunRespawn(Vector3 pos)
    {
        Debug.Log("PunRespawn with Position.");
        this.PunRespawn();
        gameObject.transform.position = pos;
    }

    public int ViewID
    {
        get
        {
            return photonView.viewID;
        }
    }
}

