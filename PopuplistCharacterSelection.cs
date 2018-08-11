using UnityEngine;

public class PopuplistCharacterSelection : MonoBehaviour
{
    //TODO: Use Unity's UI intead
    
    public GameObject ACL;
    public GameObject BLA;
    public GameObject GAS;
    public GameObject SPD;

    // ReSharper disable once UnusedMember.Local
    private void onCharacterChange() // Called by Unity.
    {
        HeroStat stat;
        string selection = GetComponent<UIPopupList>().selection;
        if (selection != "Set 1" && selection != "Set 2" && selection != "Set 3")
        {
            stat = HeroStat.GetInfo(GetComponent<UIPopupList>().selection);
        }
        else
        {
            HeroCostume costume = CostumeConeveter.LocalDataToHeroCostume(selection.ToUpper());
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

