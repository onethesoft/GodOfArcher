using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "æ∆¿Ã≈€/«Ô∏‰", fileName = "«Ô∏‰")]
public class Helmet : EquipableItem
{

  
    public override BaseItem Clone()
    {
        Helmet clone = Instantiate(this);

        clone._statModifiers = new List<StatModifier>();
        foreach (StatModifier modifier in _statModifiers)
            clone._statModifiers.Add(new StatModifier(modifier));

        if (!clone.RemainingUses.HasValue)
            clone._RemainingUses = 1;

        return clone;
    }




}
