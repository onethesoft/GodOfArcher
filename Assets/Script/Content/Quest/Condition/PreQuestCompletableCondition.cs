using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/PreQuestCompletableCondition", fileName = "PreQuestCompletableCondition_")]
public class PreQuestCompletableCondition : Condition
{
    [SerializeField]
    Quest _targetQuest;
    public void Init(Quest _target)
    {
        _targetQuest = _target;
    }

    public override bool IsPass(Quest quest)
    {
        if (Managers.Quest.CompletedQuests.Any(x => x.CodeName == _targetQuest.CodeName))
            return true;

        Quest _findtargetQuest = Managers.Quest.ActiveQuests.Where(x => x.CodeName == _targetQuest.CodeName).FirstOrDefault();
        if (_findtargetQuest == null)
        {
            Debug.LogError("PreQuestCompletableCondition : TargetQuest not exist");
            return false;
        }

        return _findtargetQuest.IsCompletable;
    }
}