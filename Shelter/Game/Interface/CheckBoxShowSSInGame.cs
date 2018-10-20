using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CheckBoxShowSSInGame : MonoBehaviour
{
    private bool init;

    [UsedImplicitly]
    private void OnActivate(bool yes)
    {
        if (this.init)
        {
            if (yes)
            {
                PlayerPrefs.SetInt("showSSInGame", 1);
            }
            else
            {
                PlayerPrefs.SetInt("showSSInGame", 0);
            }
        }
    }

    private void Start()
    {
        this.init = true;
        if (PlayerPrefs.HasKey("showSSInGame"))
        {
            if (PlayerPrefs.GetInt("showSSInGame") == 1)
            {
                GetComponent<UICheckbox>().isChecked = true;
            }
            else
            {
                GetComponent<UICheckbox>().isChecked = false;
            }
        }
        else
        {
            GetComponent<UICheckbox>().isChecked = true;
            PlayerPrefs.SetInt("showSSInGame", 1);
        }
    }
}

