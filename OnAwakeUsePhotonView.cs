using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakeUsePhotonView : Photon.MonoBehaviour
{
    private void Awake()
    {
        if (photonView.isMine)
        {
            photonView.RPC("OnAwakeRPC", PhotonTargets.All, new object[0]);
        }
    }

    [RPC]
    public void OnAwakeRPC()
    {
        Debug.Log("RPC: 'OnAwakeRPC' PhotonView: " + photonView);
    }

    [RPC]
    public void OnAwakeRPC(byte myParameter)
    {
        Debug.Log(string.Concat(new object[] { "RPC: 'OnAwakeRPC' Parameter: ", myParameter, " PhotonView: ", photonView }));
    }

    private void Start()
    {
        if (photonView.isMine)
        {
            object[] parameters = new object[] { (byte) 1 };
            photonView.RPC("OnAwakeRPC", PhotonTargets.All, parameters);
        }
    }
}

