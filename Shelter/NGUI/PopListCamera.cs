using System;
using Game.Enums;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class PopListCamera : MonoBehaviour
{
    private void Start()
    {
        var list = GetComponent<UIPopupList>();
        list.items.Clear();
        
        for (var cam = CameraType.Original; cam < CameraType.TPS; cam++)
            list.items.Add(cam.ToString());

        list.selection = IN_GAME_MAIN_CAMERA.cameraMode.ToString();
    }

    [UsedImplicitly]
    private void OnSelectionChange()
    {
        var selection = GetComponent<UIPopupList>().selection;
        var cameraMode = (CameraType) Enum.Parse(typeof(CameraType), selection);
        IN_GAME_MAIN_CAMERA.cameraMode = cameraMode;

        PlayerPrefs.SetString("cameraType", selection);
    }
}

