using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StatData
{
    public int ID;
    public string CodeName;
    public int InitLevel;
    public int MaxLevel;
    public int InitStat;
    public int IncrementStatPerLevel;
    public int InitCost;
    public string IncrementCostOperator;
    public double IncrementCostPerLevel;
    public string CurrencyID;
    public bool IsOpenned;

    public int LevelText;
    public int IncrementStatText;
    public int MaxLevelText;
    public int StatText;

    public string Icon;
    public string BackIcon;

    //public string TotalLevelup
}

[System.Serializable]
public class StatDataArray : ILoader<int, StatData>
{
    public List<StatData> data;

    public Dictionary<int, StatData> MakeDict()
    {
        Dictionary<int, StatData> dict = new Dictionary<int, StatData>();
        foreach (StatData item in data)
        {
            dict.Add(item.ID, item);
        }
        return dict;
    }

}

