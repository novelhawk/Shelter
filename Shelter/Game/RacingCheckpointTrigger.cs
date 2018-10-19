using Mod.Interface;
using Photon;
using UnityEngine;
using Extensions = Photon.Extensions;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class RacingCheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObject = other.gameObject;
        if (gameObject.layer == 8)
        {
            gameObject = gameObject.transform.root.gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && Extensions.GetPhotonView(gameObject) != null && Extensions.GetPhotonView(gameObject).isMine && gameObject.GetComponent<HERO>() != null)
            {
                Chat.System("<color=#00ff00>Checkpoint set.</color>");
                gameObject.GetComponent<HERO>().fillGas();
                FengGameManagerMKII.instance.racingSpawnPoint = this.gameObject.transform.position;
                FengGameManagerMKII.instance.racingSpawnPointSet = true;
            }
        }
    }
}

