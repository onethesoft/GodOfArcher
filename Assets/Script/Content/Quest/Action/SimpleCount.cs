using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/TaskAction/SimpleCount", fileName = "TaskAction_")]
public class SimpleCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int SuccessCount)
    {
        return currentSuccess + SuccessCount;
    }

    
}
