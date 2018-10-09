using Mod;
using Mod.Exceptions;
using UnityEngine;

public class SmoothSyncMovement : Photon.MonoBehaviour
{
    public Vector3 correctCameraPos;
    public Quaternion correctCameraRot;
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Vector3 correctPlayerVelocity = Vector3.zero;
    public bool disabled;
    public bool noVelocity;
    public bool PhotonCamera;
    public float SmoothingDelay = 5f;

    public void Awake()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            enabled = false;
        }
        this.correctPlayerPos = transform.position;
        this.correctPlayerRot = transform.rotation;
        if (rigidbody == null)
        {
            this.noVelocity = true;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (!this.noVelocity)
            {
                stream.SendNext(rigidbody.velocity);
            }
            if (this.PhotonCamera)
            {
                stream.SendNext(Camera.main.transform.rotation);
            }

            return;
        }

        if (!info.sender.IsMasterClient && info.sender.ID != photonView.owner.ID)
            throw new NotAllowedException(PunEvent.SendSerializeReliable, info.sender);

        this.correctPlayerPos = (Vector3) stream.ReceiveNext();
        this.correctPlayerRot = (Quaternion) stream.ReceiveNext();
        if (!this.noVelocity)
            this.correctPlayerVelocity = (Vector3) stream.ReceiveNext();
        
        if (this.PhotonCamera)
            this.correctCameraRot = (Quaternion) stream.ReceiveNext();
    }

    public void Update()
    {
        if (!(this.disabled || photonView.isMine))
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            if (!this.noVelocity)
            {
                rigidbody.velocity = this.correctPlayerVelocity;
            }
        }
    }
}

