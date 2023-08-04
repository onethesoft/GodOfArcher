using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "������/����/PreSlotUnlockCondition", fileName = "")]
public class PreSlotUnlockable : UnlockableCondition
{
    public override bool IsUnlockable(EquipmentSlot target, EquipmentSystem system)
    {
        // ���Ű����� ������ ù��° �����̸� ���ݸ� �����ϸ� ���Ű���.
       // if (target == system.SlotList.Where(x => x.category == "Buyable").OrderBy(x => x.Index).FirstOrDefault())
        if (system.SlotList.Where(x => x.category == target.category && x.Index < target.Index).All(x => x.IsLock == false) == true) // ������ ���Ե��� ��� �����ѵ� ���� ����
            return true;

        return false;
    }

    
}
