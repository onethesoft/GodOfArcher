using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifier 
{
    public enum StatModeType
    {
        Add ,
        Mul
    }
    [SerializeField]
    string _codeName;

    [SerializeField]
    string _description;

    [SerializeField]
    int _value;

    [SerializeField]
    StatModeType _type;

   

    public string CodeName => _codeName;

    public string Description => _description;
    public int Value => _value;
    public StatModeType Mode => _type;


    public StatModifier(string codeName , int value , StatModeType type)
    {
        _codeName = codeName;
        _value = value;
        _type = type;
    }
    public StatModifier(StatModifier moddifier)
    {
        _codeName = moddifier._codeName;
        _value = moddifier._value;
        _type = moddifier._type;
        _description = moddifier._description;
    }


}
