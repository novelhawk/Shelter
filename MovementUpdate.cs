using UnityEngine;

public class MovementUpdate : MonoBehaviour
{
    public bool disabled;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 lastVelocity;
    private Vector3 targetPosition;

    private void Start()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            this.disabled = true;
            enabled = false;
        }
        else if (networkView.isMine)
        {
            object[] args = new object[] { transform.position, transform.rotation, transform.localScale, Vector3.zero };
            networkView.RPC("updateMovement", RPCMode.OthersBuffered, args);
        }
        else
        {
            this.targetPosition = transform.position;
        }
    }

    private void Update()
    {
        if (!this.disabled && Network.peerType != NetworkPeerType.Disconnected && Network.peerType != NetworkPeerType.Connecting)
        {
            if (networkView.isMine)
            {
                if (Vector3.Distance(transform.position, this.lastPosition) >= 0.5f)
                {
                    this.lastPosition = transform.position;
                    object[] args = new object[] { transform.position, transform.rotation, transform.localScale, rigidbody.velocity };
                    networkView.RPC("updateMovement", RPCMode.Others, args);
                }
                else if (Vector3.Distance(transform.rigidbody.velocity, this.lastVelocity) >= 0.1f)
                {
                    this.lastVelocity = transform.rigidbody.velocity;
                    object[] objArray2 = new object[] { transform.position, transform.rotation, transform.localScale, rigidbody.velocity };
                    networkView.RPC("updateMovement", RPCMode.Others, objArray2);
                }
                else if (Quaternion.Angle(transform.rotation, this.lastRotation) >= 1f)
                {
                    this.lastRotation = transform.rotation;
                    object[] objArray3 = new object[] { transform.position, transform.rotation, transform.localScale, rigidbody.velocity };
                    networkView.RPC("updateMovement", RPCMode.Others, objArray3);
                }
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, this.targetPosition, Time.deltaTime * 2f);
            }
        }
    }

    [RPC]
    private void UpdateMovement(Vector3 newPosition, Quaternion newRotation, Vector3 newScale, Vector3 veloctiy)
    {
        this.targetPosition = newPosition;
        transform.rotation = newRotation;
        transform.localScale = newScale;
        rigidbody.velocity = veloctiy;
    }
}

