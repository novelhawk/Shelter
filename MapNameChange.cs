using UnityEngine;

public class MapNameChange : MonoBehaviour
{
    private void OnSelectionChange()
    {
        LevelInfo info = LevelInfoManager.GetInfo(base.GetComponent<UIPopupList>().selection);
        if (info != null)
        {
            GameObject.Find("LabelLevelInfo").GetComponent<UILabel>().text = info.Description;
        }
        if (!base.GetComponent<UIPopupList>().items.Contains("Custom"))
        {
            base.GetComponent<UIPopupList>().items.Add("Custom");
            UIPopupList component = base.GetComponent<UIPopupList>();
            component.textScale *= 0.8f;
        }
        if (!base.GetComponent<UIPopupList>().items.Contains("Custom (No PT)"))
        {
            base.GetComponent<UIPopupList>().items.Add("Custom (No PT)");
        }
    }
}

