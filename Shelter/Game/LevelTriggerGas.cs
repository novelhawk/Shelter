using Photon;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class LevelTriggerGas : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                other.gameObject.GetComponent<HERO>().fillGas();
                Destroy(gameObject);
            }
            else if (other.gameObject.GetComponent<HERO>().photonView.isMine)
            {
                other.gameObject.GetComponent<HERO>().fillGas();
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
    }
}

