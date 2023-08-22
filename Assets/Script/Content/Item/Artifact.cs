using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;

[CreateAssetMenu(menuName = "아이템/유물")]
public class Artifact : BaseItem
{
    public enum type{
        Red,
        Green,
        Blue,
        Gold,
        White
    }
    public int Level { get { return _RemainingUses == null ? 0 : _RemainingUses.Value; } }

    [SerializeField]
    type _type;

    public type Type => _type;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    Sprite _Background;
    public Sprite Background => _Background;

    [SerializeField]
    Color _BackgroundColor;
    public Color BackgroundColor => _BackgroundColor;

    [SerializeField]
    int _incrementstatPerLevel;
    public int IncrementstatPerLevel => CalculateIncrement();


    [SerializeField]
    int _maxLevel;
    public int MaxLevel => _maxLevel;

    [SerializeField]
    StatModifier _statModifier;
    public StatModifier StatModifier => _statModifier;

    public override BaseItem Clone()
    {
        Artifact _ret = Instantiate(this);
        if (!_ret._RemainingUses.HasValue)
        {
            _ret._RemainingUses = 1;
            _ret._statModifier = new StatModifier(_statModifier.CodeName , IncrementstatPerLevel, _statModifier.Mode);
        }
        else
        {
            _ret._RemainingUses = this._RemainingUses;
            _ret._statModifier = new StatModifier(_statModifier);
        }

        
        return _ret;
    }
   

    public override void Add(int Count)
    {
        base.Add(Count);
        if (Count > 0)
        {
            StatModifier _current = _statModifier;
            _statModifier = new StatModifier(_current.CodeName, CalculateStatValue(), _current.Mode);
        }

    }
    public override void Setup(ItemSaveData data)
    {
        base.Setup(data);
        if(_RemainingUses.GetValueOrDefault() > 0)
        {
          
            StatModifier _current = _statModifier;
            _statModifier = new StatModifier(_current.CodeName, CalculateStatValue(), _current.Mode);
        }
        

    }
    public override void Setup(ItemInstance item)
    {
        base.Setup(item);
        if (_RemainingUses.GetValueOrDefault() > 0)
        {

            StatModifier _current = _statModifier;
            _statModifier = new StatModifier(_current.CodeName, CalculateStatValue(), _current.Mode);
        }

    }
    public int CalculateIncrement()
    {
        if (ItemClass == "Heart")
        {
            if (Level >= 1 && Level <= 500)
            {
                return _incrementstatPerLevel;
            }
            else
            {
                return _incrementstatPerLevel * 2;
            }
        }
        else if (ItemClass == "Essence")
        {
            if (Level >= 1 && Level <= 100)
            {
                return _incrementstatPerLevel;
            }
            else
            {
                return _incrementstatPerLevel * 2;
            }
        }
        else
            return _incrementstatPerLevel;
    }

    public int CalculateStatValue()
    {
        int newValue = (_incrementstatPerLevel * _RemainingUses.GetValueOrDefault());
      

        if (ItemClass == "Heart")
        {
            if (_RemainingUses >= 1 && _RemainingUses <= 500)
            {
                newValue = (_incrementstatPerLevel * _RemainingUses.GetValueOrDefault());
            }
            else
            {
                newValue = (_incrementstatPerLevel * 500) + (_incrementstatPerLevel * 2 * (_RemainingUses.GetValueOrDefault() - 500));
                
                
            }
        }
        else if (ItemClass == "Essence")
        {
            if (_RemainingUses >= 1 && _RemainingUses <= 100)
            {
                newValue = (_incrementstatPerLevel * _RemainingUses.GetValueOrDefault());
            }
            else
            {
                newValue = (_incrementstatPerLevel * 100) + (_incrementstatPerLevel * 2 * (_RemainingUses.GetValueOrDefault() - 100));
               
            }
        }
        

        return newValue;
    }



}
