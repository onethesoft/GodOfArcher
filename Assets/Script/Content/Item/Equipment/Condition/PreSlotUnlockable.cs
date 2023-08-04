using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "아이템/장착/PreSlotUnlockCondition", fileName = "")]
public class PreSlotUnlockable : UnlockableCondition
{
    public override bool IsUnlockable(EquipmentSlot target, EquipmentSystem system)
    {
        // 구매가능한 슬롯의 첫번째 슬롯이면 가격만 만족하면 구매가능.
       // if (target == system.SlotList.Where(x => x.category == "Buyable").OrderBy(x => x.Index).FirstOrDefault())
        if (system.SlotList.Where(x => x.category == target.category && x.Index < target.Index).All(x => x.IsLock == false) == true) // 이전의 슬롯들이 모두 구매한뒤 구매 가능
            return true;

        return false;
    }

    
}
