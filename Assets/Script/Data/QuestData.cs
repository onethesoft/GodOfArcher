using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData 
{
    public int Id;
    public string Category;
    public string Icon;
    public string CodeName;
    public string Description;
    public string GroupId;
    public string Target;
    public int NeedSuccessToComplete;
    public string RewardId;
    public string RewardText;
}


[System.Serializable]
public class QuestDataArray : ILoader<int, QuestData>
{
    public List<QuestData> data;

    public Dictionary<int, QuestData> MakeDict()
    {
        Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();
        foreach (QuestData item in data)
        {
            dict.Add(item.Id, item);
        }
        return dict;
    }
}
