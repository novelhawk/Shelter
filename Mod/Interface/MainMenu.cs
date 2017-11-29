using UnityEngine;

namespace Mod.Interface
{
    public class MainMenu : Gui
    {
        private Texture2D btnNormal;
        private Texture2D btnHover;
        private Texture2D btnActive;
        private GUIStyle serverSelect;
        private GUIStyle text;
        private GUIStyle selected;

        protected override void OnShow()
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectToMaster(PlayerPrefs.GetString("ShelterServer", "app-eu.exitgamescloud.com"), 5055, FengGameManagerMKII.applicationId, UIMainReferences.Version);
//            TODO: Work in progress
//            DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();
//            DiscordRpc.Initialize("378900623875244042", ref handlers, true, null);
//            DiscordRpc.RichPresence r = new DiscordRpc.RichPresence();
//            r.endTimestamp = long.MaxValue;
//            r.state = "In game";
//            r.partySize = 1;
//            r.partyMax = 5;
//            r.details = "Hellothere11!";
//            DiscordRpc.UpdatePresence(ref r);

            btnNormal = Texture(169, 169, 169, 100);
            btnHover = Texture(169, 169, 169, 255);
            btnActive = Texture(134, 134, 134, 255);
            serverSelect = new GUIStyle(GUIStyle.none)
            {
                normal = {background = btnNormal},
                hover = {background = btnHover},
                active = {background = btnActive},
                alignment = TextAnchor.MiddleCenter
            };
            text = new GUIStyle
            {
                fontSize = 20,
                normal = { textColor = Color(178, 102, 106) },
                alignment = TextAnchor.MiddleCenter,
            };
            selected = new GUIStyle(text)
            {
                fontStyle = FontStyle.Bold
            };
        }

        protected override void Render()
        {
            Rect rect;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1)); // Scale the gui for any resoultion
            GUI.Label(rect = new Rect(145, 347, 166, 40), "Create", IsVisible("CreateRoom") ? selected : text);
            if (GUI.Button(new Rect(rect.x, rect.y - 10f, rect.width, rect.height), string.Empty, GUIStyle.none))
            {
                Enable("CreateRoom");
                Disable("ServerList");
                Disable("ProfileManager");
            }
            GUI.Label(rect = new Rect(143, 370, 167, 40), "Server list", IsVisible("ServerList") ? selected : text);
            if (GUI.Button(new Rect(rect.x, rect.y - 10f, rect.width, rect.height), string.Empty, GUIStyle.none))
            {
                Enable("ServerList");
                Disable("CreateRoom");
                Disable("ProfileManager");
            }
            GUI.Label(rect = new Rect(143, 393, 167, 40), "Profile Manager", IsVisible("ProfileManager") ? selected : text);
            if (GUI.Button(new Rect(rect.x, rect.y - 10f, rect.width, rect.height), string.Empty, GUIStyle.none))
            {
                Enable("ProfileManager");
                Disable("CreateRoom");
                Disable("ServerList");
            }

            GUI.matrix = Matrix4x4.identity;
            GUILayout.BeginArea(new Rect(Screen.width - 250f, 0, 150f, 40f));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("EU", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.Version);
                PlayerPrefs.SetString("ShelterServer", "app-eu.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            if (GUILayout.Button("US", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-us.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.Version);
                PlayerPrefs.SetString("ShelterServer", "app-us.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            if (GUILayout.Button("JPN", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-jp.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.Version);
                PlayerPrefs.SetString("ShelterServer", "app-jp.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            if (GUILayout.Button("ASIA", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-asia.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.Version);
                PlayerPrefs.SetString("ShelterServer", "app-asia.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }

        protected override void OnHide()
        {
            Destroy(btnActive);
            Destroy(btnHover);
            Destroy(btnNormal);
        }
    }
}
