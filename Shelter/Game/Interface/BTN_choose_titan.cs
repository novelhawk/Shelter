using Game;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Mod;
using NGUI.Internal;
using Photon;
using Photon.Enums;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class BTN_choose_titan : MonoBehaviour
{
    [UsedImplicitly]
    private void OnClick()
    {
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpAHSS)
        {
            const string id = "AHSS";
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
            GameManager.instance.needChooseSide = false;
            if (!PhotonNetwork.isMasterClient && GameManager.instance.roundTime > 60f)
            {
                GameManager.instance.SpawnPlayerAfterGameEnd(id);
                GameManager.instance.photonView.RPC(Rpc.RestartByClient, PhotonTargets.MasterClient);
            }
            else
            {
                GameManager.instance.SpawnPlayer(id);
            }
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
            IN_GAME_MAIN_CAMERA.usingTitan = false;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            Player.Self.SetCustomProperties(new Hashtable
            {
                { PlayerProperty.Character, id }
            });
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
            {
                GameManager.instance.checkpoint = GameObject.Find("PVPchkPtT");
            }
            string selection = GameObject.Find("PopupListCharacterTITAN").GetComponent<UIPopupList>().selection;
            NGUITools.SetActive(transform.parent.gameObject, false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
            if (!PhotonNetwork.isMasterClient && GameManager.instance.roundTime > 60f || GameManager.instance.justSuicide)
            {
                GameManager.instance.justSuicide = false;
                GameManager.instance.SpawnPlayerTitanAfterGameEnd(selection);
            }
            else
            {
                GameManager.instance.SpawnPlayerTitan(selection);
            }
            GameManager.instance.needChooseSide = false;
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
            NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
            IN_GAME_MAIN_CAMERA.usingTitan = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
        }
    }

    private void Start()
    {
        if (!LevelInfoManager.Get(GameManager.Level).PlayerTitansNotAllowed)
        {
            gameObject.GetComponent<UIButton>().isEnabled = false;
        }
    }
}

