using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="플레이어 스텟 해금 조건(플레이어 레벨)")]
public class LevelUnlockCondition : UnlockStatCondition
{
    [SerializeField]
    PlayerRank.Rank _level;
    public override bool IsUnlock(object Playerdata)
    {
        if (Playerdata is int == false)
            return false;

        return (int)_level <= (int)Playerdata;
    }

}
