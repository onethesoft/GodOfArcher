using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/TaskAction/PositiveCount", fileName = "Positive Count")]
public class PositiveCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int SuccessCount)
    {
        return SuccessCount > 0 ? currentSuccess + SuccessCount : currentSuccess;
    }


}
