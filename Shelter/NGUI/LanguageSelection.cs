using JetBrains.Annotations;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Language Selection"), RequireComponent(typeof(UIPopupList))]
// ReSharper disable once CheckNamespace
public class LanguageSelection : MonoBehaviour
{
    private UIPopupList mList;
    
    [UsedImplicitly]
    private void OnLanguageSelection(string language)
    {
        if (Localization.instance != null)
        {
            Localization.instance.currentLanguage = language;
        }
    }

    private void Start()
    {
        this.mList = GetComponent<UIPopupList>();
        this.UpdateList();
        this.mList.eventReceiver = gameObject;
        this.mList.functionName = "OnLanguageSelection";
    }

    private void UpdateList()
    {
        if (Localization.instance != null && Localization.instance.languages != null)
        {
            this.mList.items.Clear();
            int index = 0;
            int length = Localization.instance.languages.Length;
            while (index < length)
            {
                TextAsset asset = Localization.instance.languages[index];
                if (asset != null)
                {
                    this.mList.items.Add(asset.name);
                }
                index++;
            }
            this.mList.selection = Localization.instance.currentLanguage;
        }
    }
}

