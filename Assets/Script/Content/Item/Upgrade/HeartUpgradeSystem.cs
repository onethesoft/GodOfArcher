using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "������/���׷��̵����̺�/Heart", fileName = "HeartUpgradeTable")]
public class HeartUpgradeSystem : UpgradeSystem
{
    public override string GetNextLevelItem(BaseItem item)
    {
        if(item.ItemClass == "Heart")
        {
            Artifact _heart = item as Artifact;
            if (_heart.Level < _heart.MaxLevel)
                return item.ItemId;
        }
        else if(item.ItemClass == "Essence")
        {
            Artifact _heart = item as Artifact;
            if (_heart.Level < _heart.MaxLevel)
                return item.ItemId;
        }
        return null;
    }
}
