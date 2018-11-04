using Mod;
using Photon;
using Photon.Enums;
using UnityEngine;
using Extensions = Photon.Extensions;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class RacingKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObject = other.gameObject;
        if (gameObject.layer == 8)
        {
            gameObject = gameObject.transform.root.gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && Extensions.GetPhotonView(gameObject) != null && Extensions.GetPhotonView(gameObject).isMine)
            {
                HERO component = gameObject.GetComponent<HERO>();
                if (component != null)
                {
                    component.markDie();
                    component.photonView.RPC(Rpc.DieRC, PhotonTargets.All, new object[] { -1, "Server" });
                }
            }
        }
    }
}

