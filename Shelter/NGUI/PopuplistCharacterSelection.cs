using Game;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class PopuplistCharacterSelection : MonoBehaviour
{
    //TODO: Use Unity's UI intead
    
    // Progress bars (kinda)
    public GameObject ACL;
    public GameObject BLA;
    public GameObject GAS;
    public GameObject SPD;

    private void Start()
    {
        return;
        // destroy the Camera Typesadasdasdasdadsasdasdasdasdasdasdasdasdasdasasd label
        // use instance id instead?
        foreach (Object obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj is GameObject go)
            {
                if (go.name == "Label")
                {
                    UILabel label = go.GetComponent<UILabel>();
                    if (label == null)
                        continue;

                    if (label.text.StartsWith("Camera Types")) 
                    {
                        Destroy(label);
                        return;
                    }
                }
            }
        }
    }
    
    // ReSharper disable once UnusedMember.Local
    private void onCharacterChange() // Called by Unity.
    {
        HeroStat stat;
        var list = GetComponent<UIPopupList>();
        string selection = list.selection;
        
        if (!list.items.Contains("AHSS"))
            list.items.Insert(list.items.Count - 3, "AHSS");
        
        if (selection != "Set 1" && selection != "Set 2" && selection != "Set 3")
        {
            stat = HeroStat.GetInfo(selection);
        }
        else
        {
            HeroCostume costume = CostumeConverter.LocalDataToHeroCostume(selection.ToUpperInvariant());
            if (costume == null)
                stat = new HeroStat();
            else
                stat = costume.stat;
        }
        this.SPD.transform.localScale = new Vector3(stat.Speed, 20f, 0f);
        this.GAS.transform.localScale = new Vector3(stat.Gas, 20f, 0f);
        this.BLA.transform.localScale = new Vector3(stat.Blade, 20f, 0f);
        this.ACL.transform.localScale = new Vector3(stat.Acceleration, 20f, 0f);
    }
}

