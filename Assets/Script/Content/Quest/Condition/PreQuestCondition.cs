using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/PreQuestCondition", fileName = "PreQuestCondition_")]
public class PreQuestCondition : Condition
{
    [SerializeField]
    Quest _targetQuest;
    public void Init(Quest _target)
    {
        _targetQuest = _target;
    }
    public override bool IsPass(Quest quest)
    {
        return Managers.Quest.CompletedQuests.Any(x => x.CodeName == _targetQuest.CodeName);
    }
}

