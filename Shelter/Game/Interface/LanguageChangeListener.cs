using Game;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class LanguageChangeListener : MonoBehaviour
{
    [UsedImplicitly]
    private void OnSelectionChange()
    {
        Language.type = Language.GetLangIndex(GetComponent<UIPopupList>().selection);
        PlayerPrefs.SetInt("language", Language.type);
    }

    private void Start()
    {
        if (Language.type == -1)
        {
            if (PlayerPrefs.HasKey("language"))
            {
                Language.type = PlayerPrefs.GetInt("language");
            }
            else
            {
                PlayerPrefs.SetInt("language", 0);
                Language.type = 0;
            }
            Language.init();
            GetComponent<UIPopupList>().selection = Language.GetLang(Language.type);
        }
        else
        {
            GetComponent<UIPopupList>().selection = Language.GetLang(Language.type);
        }
    }
}

