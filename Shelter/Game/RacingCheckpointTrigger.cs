using Mod;
using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class RacingCheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.layer == 8)
        {
            go = go.transform.root.gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && go.GetPhotonView() != null && go.GetPhotonView().isMine && go.GetComponent<HERO>() != null)
            {
                Shelter.Chat.System("<color=#00ff00>Checkpoint set.</color>");
                go.GetComponent<HERO>().fillGas();
                GameManager.instance.racingSpawnPoint = gameObject.transform.position;
                GameManager.instance.racingSpawnPointSet = true;
            }
        }
    }
}

