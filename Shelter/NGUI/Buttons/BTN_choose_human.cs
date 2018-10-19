using System.Linq;
using ExitGames.Client.Photon;
using Mod;
using Mod.Interface;
using NGUI;
using NGUI.Internal;
using Photon;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class BTN_choose_human : MonoBehaviour
{
    private static bool IsEveryoneDead()
    {
        return !PhotonNetwork.PlayerList.Any(player => player.Properties.PlayerType == PlayerType.Human && player.Properties.Alive == true);
    }

    private void OnClick()
    {
        string selection = GameObject.Find("PopupListCharacterHUMAN").GetComponent<UIPopupList>().selection;
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
        FengGameManagerMKII.instance.needChooseSide = false;
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
        {
            FengGameManagerMKII.instance.checkpoint = GameObject.Find("PVPchkPtH");
        }
        if (!PhotonNetwork.isMasterClient && FengGameManagerMKII.instance.roundTime > 60f)
        {
            if (IsEveryoneDead())
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(selection);
            }
            else
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(selection);
                FengGameManagerMKII.instance.photonView.RPC(Rpc.RestartByClient, PhotonTargets.MasterClient, new object[0]);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight || IN_GAME_MAIN_CAMERA.GameMode == GameMode.Trost || IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpCapture)
        {
            if (!IsEveryoneDead())
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(selection);
                FengGameManagerMKII.instance.photonView.RPC(Rpc.RestartByClient, PhotonTargets.MasterClient, new object[0]);
            }
            else
            {
                FengGameManagerMKII.instance.SpawnPlayer(selection);
            }
        }
        else
        {
            FengGameManagerMKII.instance.SpawnPlayer(selection);
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

