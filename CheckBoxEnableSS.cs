using UnityEngine;

public class CheckBoxEnableSS : MonoBehaviour
{
    private bool init;

    private void OnActivate(bool yes)
    {
        if (this.init)
        {
            if (yes)
            {
                PlayerPrefs.SetInt("EnableSS", 1);
            }
            else
            {
                PlayerPrefs.SetInt("EnableSS", 0);
            }
        }
    }

    private void Start()
    {
        this.init = true;
        if (PlayerPrefs.HasKey("EnableSS"))
        {
            if (PlayerPrefs.GetInt("EnableSS") == 1)
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
            PlayerPrefs.SetInt("EnableSS", 1);
        }
    }
}

