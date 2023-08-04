using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/ÀåÂø/RatingRuneCondition", fileName = "Rune_")]
public class RatingRuneUnLockable : UnlockableCondition
{
    [SerializeField]
    PlayerRank.Rank _rank;



    public override bool IsUnlockable(EquipmentSlot target, EquipmentSystem system)
    {
        if (system.Owner.Level >= (int)_rank)
            return true;
        else
            return false;
    }
}
