using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class TitanTrigger : MonoBehaviour
{
    public bool isCollide;

    private void OnTriggerEnter(Collider other)
    {
        if (!this.isCollide)
        {
            GameObject go = other.transform.root.gameObject;
            if (go.layer == 8)
            {
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    case GameType.Singleplayer:
                        GameObject obj3 = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
                        if (obj3 != null && obj3 == go)
                            this.isCollide = true;
                        break;
                    
                    case GameType.Multiplayer:
                        if (go.GetPhotonView().isMine)
                            this.isCollide = true;
                        break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.isCollide)
        {
            GameObject go = other.transform.root.gameObject;
            if (go.layer == 8)
            {
                switch (IN_GAME_MAIN_CAMERA.GameType)
                {
                    case GameType.Singleplayer:
                        GameObject obj3 = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
                        if (obj3 != null && obj3 == go)
                            this.isCollide = false;
                        break;
                    
                    case GameType.Multiplayer:
                        if (go.GetPhotonView().isMine)
                            this.isCollide = false;
                        break;
                }
            }
        }
    }
}