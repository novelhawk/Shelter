using UnityEngine;

public class MovementUpdate1 : MonoBehaviour
{
    public bool disabled;
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    public Vector3 lastVelocity;

    private void Start()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
        {
            this.disabled = true;
            enabled = false;
        }
        else if (networkView.isMine)
        {
            object[] args = new object[] { transform.position, transform.rotation, transform.lossyScale };
            networkView.RPC("updateMovement1", RPCMode.OthersBuffered, args);
        }
        else
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!this.disabled)
        {
            object[] args = new object[] { transform.position, transform.rotation, transform.lossyScale };
            networkView.RPC("updateMovement1", RPCMode.Others, args);
        }
    }

    [RPC]
    private void UpdateMovement1(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    {
        transform.position = newPosition;
        transform.rotation = newRotation;
        transform.localScale = newScale;
    }
}

