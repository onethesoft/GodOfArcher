using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestReporter 
{
    [SerializeField]
    Category _category;

    [SerializeField]
    TaskTarget _target;
    

    [SerializeField]
    int _successCount;
    public void Set(Category category, TaskTarget target, int successCount)
    {
        _category = category;
        _target = target;
        _successCount = successCount;
    }
    public void SetSuccessCount(int count = 1)
    {
        _successCount = count;
    }
    public void DoReport()
    {
        if (_category != null && _target != null)
        {
            Managers.Quest.ReceiveReport(_category, _target, _successCount);
        }
    }
}
