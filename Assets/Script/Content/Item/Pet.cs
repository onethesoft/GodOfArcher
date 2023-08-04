using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/Æê", fileName = "Æê_")]
public class Pet : EquipableItem 
{
    public enum Type
    {
        Buff = 0,
        Drop,
        All,
    }
  

    [SerializeField]
    GameObject _prefab;
    public GameObject Prefab => _prefab;

    [SerializeField]
    private Type _Type = 0;
    public Type type => _Type;

    



    public override BaseItem Clone()
    {
        Pet clone = Instantiate(this);

        clone._statModifiers = new List<StatModifier>();
        foreach (StatModifier modifier in _statModifiers)
            clone._statModifiers.Add(new StatModifier(modifier));

        if (!clone.RemainingUses.HasValue)
            clone._RemainingUses = 1;

        return clone;
    }
}
