using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipmentSlot 
{
    bool IsLock { get; }
    bool IsEquip { get; }

    bool UnLock();
    void Equip(EquipableItem item);
    EquipableItem UnEquip();
}
