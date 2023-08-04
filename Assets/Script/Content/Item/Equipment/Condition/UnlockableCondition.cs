using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnlockableCondition : ScriptableObject
{
    [SerializeField]
    string _description;

    public string Description => _description;
    public abstract bool IsUnlockable(EquipmentSlot target , EquipmentSystem system);
}
