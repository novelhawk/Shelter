using System;
using Mod;
using UnityEngine;

public class CheckBoxCamera : MonoBehaviour
{
    public new CameraType camera;

    private void OnSelectionChange(bool yes)
    {
        if (!yes)
            return;
        
        IN_GAME_MAIN_CAMERA.cameraMode = this.camera;
        PlayerPrefs.SetString("cameraType", this.camera.ToString().ToUpper());
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("cameraType")) 
            return;
        
        GetComponent<UICheckbox>().isChecked = camera.ToString().EqualsIgnoreCase(PlayerPrefs.GetString("cameraType"));
    }
}