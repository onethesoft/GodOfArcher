using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="พฦภฬลÛ/ท้",fileName ="ท้_")]
public class Rune : EquipableItem
{
    public enum Type
    {
        Drop = 0,
        Speed ,
        Cri ,
        All
    }
  


    [SerializeField]
    private Type _Type = 0;
    public Type type => _Type;


    public override BaseItem Clone()
    {
        Rune clone = Instantiate(this);

        clone._statModifiers = new List<StatModifier>();
        foreach (StatModifier modifier in _statModifiers)
            clone._statModifiers.Add(new StatModifier(modifier));

        if (!clone.RemainingUses.HasValue)
            clone._RemainingUses = 1;

        return clone;
    }
}
