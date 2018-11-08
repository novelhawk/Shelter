using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class LevelTriggerCheckPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                GameManager.instance.checkpoint = gameObject;
            }
            else if (other.gameObject.GetComponent<HERO>().photonView.isMine)
            {
                GameManager.instance.checkpoint = gameObject;
            }
        }
    }

    private void Start()
    {
    }
}

