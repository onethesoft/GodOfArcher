using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/TaskAction/ContinuosCount", fileName = "TaskAction_")]
public class ContinuosCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int SuccessCount)
    {
        return SuccessCount > 0 ? currentSuccess + SuccessCount : 0;
    }

    
}
