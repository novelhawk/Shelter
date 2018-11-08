using Mod;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class LevelTeleport : MonoBehaviour
{
    public string levelname = string.Empty;
    public GameObject link;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && GameManager.Level != "[S]Tutorial")
            {
                if (this.levelname != string.Empty)
                    Shelter.Log("Someone tried to make you load Level({0})", levelname);
                else
                    Shelter.Log("Someone tried to teleport you to {0}", link.transform.position);
                return;
            }

            if (this.levelname != string.Empty)
                Application.LoadLevel(this.levelname);
            else
                other.gameObject.transform.position = this.link.transform.position;
        }
    }
}

