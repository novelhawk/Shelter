using System.Linq;
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
public class BTN_choose_human : MonoBehaviour
{
    private static bool IsEveryoneDead()
    {
        return !PhotonNetwork.PlayerList.Any(player => player.Properties.PlayerType == PlayerType.Human && player.Properties.Alive == true);
    }

    [UsedImplicitly]
    private void OnClick()
    {
        string selection = GameObject.Find("PopupListCharacterHUMAN").GetComponent<UIPopupList>().selection;
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
        GameManager.instance.needChooseSide = false;
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
        {
            GameManager.instance.checkpoint = GameObject.Find("PVPchkPtH");
        }
        if (!PhotonNetwork.isMasterClient && GameManager.instance.roundTime > 60f)
        {
            if (IsEveryoneDead())
            {
                GameManager.instance.SpawnPlayerAfterGameEnd(selection);
            }
            else
            {
                GameManager.instance.SpawnPlayerAfterGameEnd(selection);
                GameManager.instance.photonView.RPC(Rpc.RestartByClient, PhotonTargets.MasterClient);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight || IN_GAME_MAIN_CAMERA.GameMode == GameMode.Trost || IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
        {
            if (!IsEveryoneDead())
            {
                GameManager.instance.SpawnPlayerAfterGameEnd(selection);
                GameManager.instance.photonView.RPC(Rpc.RestartByClient, PhotonTargets.MasterClient);
            }
            else
            {
                GameManager.instance.SpawnPlayer(selection);
            }
        }
        else
        {
            GameManager.instance.SpawnPlayer(selection);
        }
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
        IN_GAME_MAIN_CAMERA.usingTitan = false;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
        Player.Self.SetCustomProperties(new Hashtable
        {
            { PlayerProperty.Character, selection }
        });
    }
}

