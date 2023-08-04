using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStat : ScriptableObject
{
    [SerializeField]
    List<BaseStat> _stats;

    public IReadOnlyList<BaseStat> Stats => _stats;

    public BaseStat GetStat(StatModifier modifier)
    {
        return _stats.Find(x => x.IsExist(modifier));
    }
    
}
