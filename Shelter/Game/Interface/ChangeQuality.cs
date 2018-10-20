using JetBrains.Annotations;
using Mod;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class ChangeQuality : MonoBehaviour
{
    public bool init;

    public static void setCurrentQuality()
    {
        if (PlayerPrefs.HasKey("GameQuality"))
            setQuality(PlayerPrefs.GetInt("GameQuality"));
    }

    private static void setQuality(int val) // val 0-5
    {
        QualitySettings.SetQualityLevel(val, true);
        
        if (val < 5)
            turnOffTiltShift();
        else
            turnOnTiltShift();
    }

    private static void turnOffTiltShift()
    {
        if (Shelter.TryFind("MainCamera", out var mainCamera))
            mainCamera.GetComponent<TiltShift>().enabled = false;
    }

    private static void turnOnTiltShift()
    {
        if (Shelter.TryFind("MainCamera", out var mainCamera))
            mainCamera.GetComponent<TiltShift>().enabled = true;
    }
}

