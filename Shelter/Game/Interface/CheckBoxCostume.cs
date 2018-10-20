using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CheckBoxCostume : MonoBehaviour
{
    public static int costumeSet;
    public int set = 1;

    [UsedImplicitly]
    private void OnActivate(bool yes)
    {
        if (yes)
        {
            costumeSet = this.set;
        }
    }
}

