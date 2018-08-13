using ExitGames.Client.Photon;
using UnityEngine;

public class BTN_choose_titan : MonoBehaviour
{
    private void OnClick()
    {
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
        {
            string id = "AHSS";
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
            FengGameManagerMKII.instance.needChooseSide = false;
            if (!PhotonNetwork.isMasterClient && FengGameManagerMKII.instance.roundTime > 60f)
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(id);
                FengGameManagerMKII.instance.photonView.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
            else
            {
                FengGameManagerMKII.instance.SpawnPlayer(id);
            }
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
            IN_GAME_MAIN_CAMERA.usingTitan = false;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            Hashtable hashtable = new Hashtable
            {
                { PlayerProperty.Character, id }
            };
            Hashtable propertiesToSet = hashtable;
            Player.Self.SetCustomProperties(propertiesToSet);
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                FengGameManagerMKII.instance.checkpoint = GameObject.Find("PVPchkPtT");
            }
            string selection = GameObject.Find("PopupListCharacterTITAN").GetComponent<UIPopupList>().selection;
            NGUITools.SetActive(transform.parent.gameObject, false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
            if (!PhotonNetwork.isMasterClient && FengGameManagerMKII.instance.roundTime > 60f || FengGameManagerMKII.instance.justSuicide)
            {
                FengGameManagerMKII.instance.justSuicide = false;
                FengGameManagerMKII.instance.SpawnPlayerTitanAfterGameEnd(selection);
            }
            else
            {
                FengGameManagerMKII.instance.SpawnPlayerTitan(selection);
            }
            FengGameManagerMKII.instance.needChooseSide = false;
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
            IN_GAME_MAIN_CAMERA.usingTitan = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
        }
    }

    private void Start()
    {
        if (!LevelInfoManager.GetInfo(FengGameManagerMKII.Level).PlayerTitansAllowed)
        {
            gameObject.GetComponent<UIButton>().isEnabled = false;
        }
    }
}

