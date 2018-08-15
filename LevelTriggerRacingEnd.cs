using UnityEngine;

public class LevelTriggerRacingEnd : MonoBehaviour
{
    private bool disable;

    private void OnTriggerStay(Collider other)
    {
        if (!this.disable && other.gameObject.tag == "Player")
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                FengGameManagerMKII.instance.GameWin();
                this.disable = true;
            }
            else if (other.gameObject.GetComponent<HERO>().photonView.isMine)
            {
                FengGameManagerMKII.instance.MultiplayerRacingFinish();
                this.disable = true;
            }
        }
    }

    private void Start()
    {
        this.disable = false;
    }
}

