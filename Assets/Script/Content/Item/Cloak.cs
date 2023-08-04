using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "æ∆¿Ã≈€/∏¡≈‰", fileName = "∏¡≈‰")]
public class Cloak : EquipableItem
{
   
    public override BaseItem Clone()
    {
        Cloak clone = Instantiate(this);

        clone._statModifiers = new List<StatModifier>();
        foreach (StatModifier modifier in _statModifiers)
            clone._statModifiers.Add(new StatModifier(modifier));

        if (!clone.RemainingUses.HasValue)
            clone._RemainingUses = 1;

        return clone;
    }






}
