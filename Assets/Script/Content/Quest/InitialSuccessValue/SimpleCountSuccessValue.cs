using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/InitialSuccess/SimpleCountSucess", fileName = "InitialSimpleSuccess")]
public class SimpleCountSuccessValue : InitialSuccessValue
{
    [SerializeField]
    int _value;
    public override int GetValue(Task task)
    {
        return _value;
    }

    
}
