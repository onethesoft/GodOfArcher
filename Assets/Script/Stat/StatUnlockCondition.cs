using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "플레이어 스텟 해금 조건(플레이어 스텟)")]
public class StatUnlockCondition : UnlockStatCondition
{
    [SerializeField]
    Define.StatID _statCodeName;

    [SerializeField]
    int _statLevel;

    public override bool IsUnlock(object Playerdata)
    {
        if (Playerdata is CharacterStat == false)
            return false;
        
        CharacterStat _stat = Playerdata as CharacterStat;

        Debug.Log(_stat.CodeName);
        if (_stat.CodeName != _statCodeName.ToString())
            return false;
        if (_stat.Level < _statLevel)
            return false;

        return true;

    }

   
}
