using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System .Serializable]
public class RoulettePieceData 
{
    public enum RewardType
    {
        Currency,
        Item
    }
    public Sprite Icon;
    public string description;
    public RewardType Rewardtype;

    public string Id;
    public int Amount;


    [Range(1, 20000)]
    public int change = 20000;

    [HideInInspector]
    public int index;

    [HideInInspector]
    public int weight;


}
