using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OutlineData
{
    public Color OutlineColor;
    public Vector2 EffectDistance;

    public void AddOutline(GameObject go)
    {
        Outline outline = Util.GetOrAddComponent<Outline>(go);
        outline.effectColor = OutlineColor;
        outline.effectDistance = EffectDistance;
    }
    public OutlineData Copy()
    {
        OutlineData _ret = new OutlineData();
        _ret.OutlineColor = new Color(OutlineColor.r , OutlineColor.g , OutlineColor.b, OutlineColor.a);
        _ret.EffectDistance = new Vector2(EffectDistance.x, EffectDistance.y);
        return _ret;
    }

    
}

[CreateAssetMenu(menuName = "아이템/활", fileName = "활_")]
public class Bow : EquipableItem
{


    public override BaseItem Clone()
    {
        Bow clone = Instantiate(this);

        clone._statModifiers = new List<StatModifier>();
        foreach (StatModifier modifier in _statModifiers)
            clone._statModifiers.Add(new StatModifier(modifier));

        if (!clone.RemainingUses.HasValue)
            clone._RemainingUses = 1;

        return clone ;
    }



}
