using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/CharacterStat", fileName = "Target_")]
public class CharacterStatTarget : TaskTarget
{
    [SerializeField]
    CharacterStat _targetStat;
    public override object Value => _targetStat;

    public override bool IsEqual(object target)
    {
        CharacterStat _target = target as CharacterStat;
        if (_target == null)
            return false;
        return _target.CodeName == _targetStat.CodeName ? true : false;
    }

   
}
