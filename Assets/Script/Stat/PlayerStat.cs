using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class StatUpgrader
{
    int _initlevel;
    int _initValue;
    int _initCost;

    int _IncrementValuePerLevel;
    double _incrementCostPerLevel;
    string _dfOperator;
    public StatUpgrader(int InitLevel , int InitValue , int InitCost , int IncrementValuePerLevel , double InCrementCostPerLevel , string dfOperator)
    {
        _initlevel = InitLevel;
        _initValue = InitValue;

        _initCost = InitCost;
        _IncrementValuePerLevel = IncrementValuePerLevel;

        _incrementCostPerLevel = InCrementCostPerLevel;
        _dfOperator = dfOperator;

    }

    public BigInteger GetCost(PlayerStat Stat , int TargetLevel , int CurrentLevel)
    {
        if (TargetLevel <= CurrentLevel)
            return 0;
        if (TargetLevel < _initlevel)
            return 0;


        if(Stat.ID == (int)Define.StatID.SkillAttack)
        {
            return 0;
        }
        else
        {
            BigInteger TargetLevel_n = TargetLevel - 1;
            BigInteger CurrentLevel_n = CurrentLevel - 1;
            BigInteger _cost = _initCost;
            if (_dfOperator == "sum")
            {
                BigInteger _TargetLevelTotalCost = TargetLevel_n * (2 * _cost + ((BigInteger)_incrementCostPerLevel * (TargetLevel_n - 1))) / 2;
                BigInteger _CurrentLevelTotalCost = CurrentLevel_n * (2 * _cost + ((BigInteger)_incrementCostPerLevel * (CurrentLevel_n - 1))) / 2;

                return _TargetLevelTotalCost - _CurrentLevelTotalCost;
            }
            else if (_dfOperator == "mul")
            {
                double double_r = _incrementCostPerLevel * 1000.0;

                BigInteger r = (BigInteger)double_r;
                BigInteger _TargetLevelTotalCost = _cost * (BigInteger.Pow(r, TargetLevel - 1) - 1) / (r - 1);
                BigInteger _CurrentLevelTotalCost = _cost * (BigInteger.Pow(r, CurrentLevel - 1) - 1) / (r - 1);
                BigInteger _diff = (_TargetLevelTotalCost - _CurrentLevelTotalCost) / 1000;
                return _diff;
            }
            else
                return 0;
        }
        
    }

    public int GetValue(int TargetLevel)
    {
        int dfValue = TargetLevel - 1;
        return dfValue * _IncrementValuePerLevel + _initValue;
    }

}

public class PlayerStat : ScriptableObject
{
    public delegate void LevelChangedHandler(PlayerStat Stat, int PrevLevel, int CurrentLevel);
    public LevelChangedHandler onLevelChanged;

    int _id;
    string _codeName;

    int _Level;
    int _MaxLevel;

    int _IncrementValuePerLevel;

    BigInteger _Value;
    bool _IsOpenned;
    int _currencyId;

    Sprite _backIcon;
    Sprite _Icon;

    string _LevelDescription;
    string _IncrementValueDescription;
    string _maxLevelDescription;
    string _valueDescription;

    public int ID => _id;
    public string CodeName => _codeName;

    public int Level => _Level;
    public int MaxLevel => _MaxLevel;
    public int IncrementValuePerLevel => _IncrementValuePerLevel;
    public BigInteger Value => _Value;

    StatUpgrader _Upgrader;


    public int CurrencyId => _currencyId;

    public bool IsOpenned => _IsOpenned;

    public Sprite Icon => _Icon;
    public Sprite BackIcon => _backIcon;

    

    public string LevelText { 
        get {
            if (_id == (int)Define.StatID.SkillMaxLevelLimit)
                return _LevelDescription;
            else
                return string.Format(_LevelDescription, _Level);
        }
    }
    public string IncrementValueText
    {
        get
        {
            return string.Format(_IncrementValueDescription, _IncrementValuePerLevel);
        }
    }
    public string MaxLevelText
    {
        get
        {
            return string.Format(_maxLevelDescription, _MaxLevel);
        }
    }
    public string ValueText
    {
        get
        {
            if (ID == (int)Define.StatID.SkillAttack)
                return string.Format(_valueDescription, Managers.Currency.GetCurrency((Define.CurrencyID)_currencyId).Amount);
            else if(ID == (int)Define.StatID.SkillMaxLevelLimit)
            {
                return string.Format(_valueDescription, Value , Value + IncrementValuePerLevel);
            }
            else
                return string.Format(_valueDescription, _Value);
        }
    }

    public void Set(StatData data)
    {
        _id = data.ID;
        _codeName = data.CodeName;
        _MaxLevel = data.MaxLevel;
        _IncrementValuePerLevel = data.IncrementStatPerLevel;

        Define.CurrencyID currencyId = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), data.CurrencyID);
        _currencyId = (int)currencyId;


        _Upgrader = new StatUpgrader(data.InitLevel, data.InitStat, data.InitCost, data.IncrementStatPerLevel, data.IncrementCostPerLevel, data.IncrementCostOperator);

        _Icon = Managers.Resource.Load<Sprite>($"Sprites/{data.Icon}");
        _backIcon = Managers.Resource.Load<Sprite>($"Sprites/{data.BackIcon}");


        _LevelDescription = Managers.Data.TextData[data.LevelText].Kor;
        _IncrementValueDescription = Managers.Data.TextData[data.IncrementStatText].Kor;
        _maxLevelDescription = Managers.Data.TextData[data.MaxLevelText].Kor;
        _valueDescription = Managers.Data.TextData[data.StatText].Kor;

    }

    // PlayerLevel 1 ~ 10
    public void UpdateMaxLevel(int PlayerLevel)
    {
        if(ID != (int)Define.StatID.SkillAttack)
            return;

        if (_MaxLevel < 100)
            _MaxLevel = PlayerLevel * 10;

    }
    public void UpdateMaxLevel(PlayerStat stat)
    {
        if (ID != (int)Define.StatID.SkillAttack)
            return;
        if (stat.ID != (int)Define.StatID.SkillMaxLevelLimit)
            return;
        if (_MaxLevel < 100)
            return;

        _MaxLevel = (int)stat.Value;

    }

    public void Open(PlayerStat stat)
    {
        if (stat.ID != (int)Define.StatID.SkillAttack)
            return;
        if (ID != (int)Define.StatID.SkillMaxLevelLimit)
            return;

        if (stat.Level >= 100 && _IsOpenned == false)
            _IsOpenned = true;

    }

    public BigInteger GetUpgradeCost(int TargetLevel)
    {
        return _Upgrader.GetCost(this, TargetLevel, _Level);
    }


    public void Upgrade(int TargetLevel)
    {
        int _target = TargetLevel >= _MaxLevel ? _MaxLevel : TargetLevel;
        
        if( _Upgrader.GetCost(this , _target, _Level) <= 0 )
        {
            _Level = _target;
            _Value = _Upgrader.GetValue(_Level);
        }
    }


    


    
    
    
}
