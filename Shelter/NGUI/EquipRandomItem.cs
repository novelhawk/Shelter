using System.Collections.Generic;
using JetBrains.Annotations;
using NGUI;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Equip Random Item")]
// ReSharper disable once CheckNamespace
public class EquipRandomItem : MonoBehaviour
{
    public InvEquipment equipment;

    [UsedImplicitly]
    private void OnClick()
    {
        if (this.equipment != null)
        {
            List<InvBaseItem> items = InvDatabase.list[0].items;
            if (items.Count != 0)
            {
                int id = Random.Range(0, items.Count);
                InvBaseItem bi = items[id];
                InvGameItem item = new InvGameItem(id, bi) {
                    quality = (InvGameItem.Quality) Random.Range(0, 12),
                    itemLevel = NGUITools.RandomRange(bi.minItemLevel, bi.maxItemLevel)
                };
                this.equipment.Equip(item);
            }
        }
    }
}

