using UnityEngine;

public class SmoothSyncMovement2 : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    public bool disabled;
    public float SmoothingDelay = 5f;

    public void Awake()
    {
        this.SmoothingDelay = 10f;
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            enabled = false;
        }
        this.correctPlayerPos = transform.position;
        this.correctPlayerRot = transform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.correctPlayerPos = (Vector3) stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion) stream.ReceiveNext();
        }
    }

    public void Update()
    {
        if (!this.disabled && !photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
        }
    }
}

