using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
[CreateAssetMenu(menuName ="±âº»½ºÅÝ" , fileName ="½ºÅÝ")]
public class BaseStat : ScriptableObject
{
   

    [Header("½ºÅÝÀÌ¸§")]
    public string CodeName;

    [Header("½ºÅÝ¼³¸í")]
    public string Description;

    [Header("½ºÅÝ¼öÄ¡")]
    public int Value = 0;

    
    public int GetValue => CalculateValue();

    protected List<StatModifier> _statModifiers;
    protected virtual int CalculateValue()
    {
        int calculate = Value;
        
        if (_statModifiers == null)
            return calculate;

        foreach (StatModifier modifier in _statModifiers)
        {
            if (modifier.Mode == StatModifier.StatModeType.Add)
                calculate += modifier.Value;
            else if(modifier.Mode == StatModifier.StatModeType.Mul)
                calculate *= modifier.Value;
        }

     
        return calculate;

    }

    public void AddModifier(StatModifier modifier)
    {
        if (modifier.CodeName != CodeName)
            return;
        
        if (_statModifiers == null)
            _statModifiers = new List<StatModifier>();
    

        _statModifiers.Add(modifier);

    }
    public void ClearModifier()
    {
        if(_statModifiers != null)
            _statModifiers.Clear();
    }
    public bool RemoveModifier(StatModifier modifier)
    {
        if (_statModifiers == null || (modifier.CodeName != CodeName))
        {
            return false;
        }



        return _statModifiers.Remove(modifier);

    }
    public bool IsExist(StatModifier modifier)
    {
        return _statModifiers.Find(x => x == modifier) == null ? false : true;
    }

    public BaseStat Clone()
    {
        BaseStat _ret = new BaseStat();
        _ret.CodeName = CodeName;
        _ret.Description = Description;
        _ret.Value = Value;
        _ret._statModifiers = new List<StatModifier>();
        return _ret;

    }
    public void PritModfiers()
    {
        Debug.Log("PritModfiers");
        foreach (StatModifier d in _statModifiers)
        {
            Debug.Log(d.CodeName);
        }
    }
}
