using Mod;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class ChangeQuality : MonoBehaviour
{
    public bool init;

    public static void LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("GameQuality"))
            SetQuality(PlayerPrefs.GetInt("GameQuality"));
    }

    private static void SetQuality(int val) // val 0-5
    {
        QualitySettings.SetQualityLevel(val, true);
    }
}

