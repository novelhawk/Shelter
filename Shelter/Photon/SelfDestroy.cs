using Photon;
using Photon.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class SelfDestroy : Photon.MonoBehaviour // This class is used
{
    public float CountDown = 5f;
    
    private void Update()
    {
        this.CountDown -= Time.deltaTime;
        if (this.CountDown <= 0f)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView != null && photonView.isMine)
                PhotonNetwork.Destroy(gameObject);
            else
                Destroy(gameObject);
        }
    }
}

