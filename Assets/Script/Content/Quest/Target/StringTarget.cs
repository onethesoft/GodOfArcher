using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/String", fileName = "Target_")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string _value;
    public override object Value => _value;
    public StringTarget(string value)
    {
        _value = value;
    }
    public void SetValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        _value = value;
    }

    public override bool IsEqual(object target)
    {
        string _target = target as string;
        if (_target == null)
            return false;

        return _target == _value;
    }

    
}
