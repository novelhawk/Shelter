using Mod;
using Photon.Enums;
using UnityEngine;
using Extensions = Photon.Extensions;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class RacingKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.layer == 8)
        {
            go = go.transform.root.gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && Extensions.GetPhotonView(go) != null && Extensions.GetPhotonView(go).isMine)
            {
                HERO component = go.GetComponent<HERO>();
                if (component != null)
                {
                    component.markDie();
                    component.photonView.RPC(Rpc.DieRC, PhotonTargets.All, -1, "Server");
                }
            }
        }
    }
}

