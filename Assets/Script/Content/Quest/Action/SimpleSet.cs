using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quest/Task/TaskAction/SimpleSet", fileName = "TaskAction_")]
public class SimpleSet : TaskAction
{
    public override int Run(Task task, int currentSuccess, int SuccessCount)
    {
        return SuccessCount;
    }

    
}
