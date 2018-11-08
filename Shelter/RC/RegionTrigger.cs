using RC;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class RegionTrigger : MonoBehaviour
{
    public string myName;
    public RCEvent playerEventEnter;
    public RCEvent playerEventExit;
    public RCEvent titanEventEnter;
    public RCEvent titanEventExit;

    public void CopyTrigger(RegionTrigger copyTrigger)
    {
        this.playerEventEnter = copyTrigger.playerEventEnter;
        this.titanEventEnter = copyTrigger.titanEventEnter;
        this.playerEventExit = copyTrigger.playerEventExit;
        this.titanEventExit = copyTrigger.titanEventExit;
        this.myName = copyTrigger.myName;
    }

    private void OnTriggerEnter(Collider other)
    {
        string str;
        GameObject gameObject = other.transform.gameObject;
        if (gameObject.layer == 8)
        {
            if (this.playerEventEnter != null)
            {
                HERO component = gameObject.GetComponent<HERO>();
                if (component != null)
                {
                    str = (string) GameManager.RCVariableNames["OnPlayerEnterRegion[" + this.myName + "]"];
                    if (GameManager.playerVariables.ContainsKey(str))
                    {
                        GameManager.playerVariables[str] = component.photonView.owner;
                    }
                    else
                    {
                        GameManager.playerVariables.Add(str, component.photonView.owner);
                    }
                    this.playerEventEnter.checkEvent();
                }
            }
        }
        else if (gameObject.layer == 11 && this.titanEventEnter != null)
        {
            TITAN titan = gameObject.transform.root.gameObject.GetComponent<TITAN>();
            if (titan != null)
            {
                str = (string) GameManager.RCVariableNames["OnTitanEnterRegion[" + this.myName + "]"];
                if (GameManager.titanVariables.ContainsKey(str))
                {
                    GameManager.titanVariables[str] = titan;
                }
                else
                {
                    GameManager.titanVariables.Add(str, titan);
                }
                this.titanEventEnter.checkEvent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string str;
        GameObject gameObject = other.transform.root.gameObject;
        if (gameObject.layer == 8)
        {
            if (this.playerEventExit != null)
            {
                HERO component = gameObject.GetComponent<HERO>();
                if (component != null)
                {
                    str = (string) GameManager.RCVariableNames["OnPlayerLeaveRegion[" + this.myName + "]"];
                    if (GameManager.playerVariables.ContainsKey(str))
                    {
                        GameManager.playerVariables[str] = component.photonView.owner;
                    }
                    else
                    {
                        GameManager.playerVariables.Add(str, component.photonView.owner);
                    }
                }
            }
        }
        else if (gameObject.layer == 11 && this.titanEventExit != null)
        {
            TITAN titan = gameObject.GetComponent<TITAN>();
            if (titan != null)
            {
                str = (string) GameManager.RCVariableNames["OnTitanLeaveRegion[" + this.myName + "]"];
                if (GameManager.titanVariables.ContainsKey(str))
                {
                    GameManager.titanVariables[str] = titan;
                }
                else
                {
                    GameManager.titanVariables.Add(str, titan);
                }
                this.titanEventExit.checkEvent();
            }
        }
    }
}

