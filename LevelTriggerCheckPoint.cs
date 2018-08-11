using UnityEngine;

public class LevelTriggerCheckPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                FengGameManagerMKII.instance.checkpoint = gameObject;
            }
            else if (other.gameObject.GetComponent<HERO>().photonView.isMine)
            {
                FengGameManagerMKII.instance.checkpoint = gameObject;
            }
        }
    }

    private void Start()
    {
    }
}

