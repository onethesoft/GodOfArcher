using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public int ID;
    public int Level;
    public int LevelText;
    public string Icon;
    public int Condition;
    public int StatType;
    public int StatMaxLevel;
    public int JumpingCount;
    public int JumpingRate;
    public int AdditionalAttackCount;
    public int RuneSlotCount;
    public int PetSlotCount;
}


[System.Serializable]
public class PlayerDataArray : ILoader<int, PlayerData>
{
    public List<PlayerData> data;

    public Dictionary<int, PlayerData> MakeDict()
    {
        Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
        foreach (PlayerData item in data)
        {
            dict.Add(item.ID, item);
        }
        return dict;
    }
}

[System.Serializable]
public class PlayerResourceData
{
    public BigInteger Gold =0;
    public BigInteger CP = 0;
    public int Diamond = 0;
    public int SkillPoint = 0;
}




