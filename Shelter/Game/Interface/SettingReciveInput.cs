using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class SettingReciveInput : MonoBehaviour
{
    public int id;

    [UsedImplicitly]
    private void OnClick()
    {
        transform.Find("Label").gameObject.GetComponent<UILabel>().text = "*wait for input";
    }
}

