using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/ÀåÂø/ReviveLevelCondition", fileName = "ÄªÈ£ ÇØ±İ")]
public class ReviveLevelUnLockable : UnlockableCondition
{
    [SerializeField]
    int ReviveLevel;

    public override bool IsUnlockable(EquipmentSlot target, EquipmentSystem system)
    {
        if (system.Owner.ReviveLevel >= (int)ReviveLevel)
            return true;
        else
            return false;
    }
}
