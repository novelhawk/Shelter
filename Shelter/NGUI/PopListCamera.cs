using Game;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class PopListCamera : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.HasKey("cameraType"))
        {
            GetComponent<UIPopupList>().selection = PlayerPrefs.GetString("cameraType");
        }
    }

    private void Start()
    {
        UIPopupList list = GetComponent<UIPopupList>();
        if (list != null)
        {
            list.items.Clear();
            list.items.Add("Original");
            list.items.Add("TPS");
            list.items.Add("Stop");
        }
    }

    private void OnSelectionChange()
    {
        if (GetComponent<UIPopupList>().selection == "Original")
        {
            IN_GAME_MAIN_CAMERA.cameraMode = CameraType.Original;
        }
        if (GetComponent<UIPopupList>().selection == "TPS")
        {
            IN_GAME_MAIN_CAMERA.cameraMode = CameraType.TPS;
        }
        if (GetComponent<UIPopupList>().selection == "Stop")
        {
            IN_GAME_MAIN_CAMERA.cameraMode = CameraType.Stop;
        }

        PlayerPrefs.SetString("cameraType", GetComponent<UIPopupList>().selection);
    }
}

