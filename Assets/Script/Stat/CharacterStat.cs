using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "플레이어 스텟")]
public class CharacterStat : BaseStat
{

    public delegate void LevelChangedHandler(CharacterStat stat, int Count);
    public event LevelChangedHandler OnLevelChanged;

    public delegate void UnLockHandler(bool isUnlock);
    public event UnLockHandler OnUnLock;

    public delegate void MaxLevelChangedHandler(CharacterStat stat, int Count);
    public event MaxLevelChangedHandler OnMaxLevelChanged;

    [Header("타입")]
    [SerializeField]
    Define.StatType _Type;

    public Define.StatType type => _Type;

    CharacterStatSystem _parent;
    public CharacterStatSystem Parent { set { _parent = value; } get { return _parent; } }

    [Header("최고레벨")]
    [SerializeField]
    protected int _maxLevel;
    public int MaxLevel => _maxLevel;

    [Header("최소레벨")]
    [SerializeField]
    protected int _minLevel;

    public int MinLevel => _minLevel;

    [Header("레벨")]
    [SerializeField]
    protected int _Level;

    public int Level => _Level;

    [Header("Unlock")] 
    [SerializeField]
    bool _isUnlock;
    public bool IsUnLock => _isUnlock;

    [Header("UnlockCondition")]
    [SerializeField]
    UnlockStatCondition _unlockCondition;
    public UnlockStatCondition UnLockCondition => _unlockCondition;

    [Header("Unlock 상태일때 Text")]
    [SerializeField]
    public string UnLockString;





    [Header("아이콘 배경")]
    [SerializeField]
    Sprite _iconBackground;

    public Sprite IconBackground => _iconBackground;

    [Header("아이콘")]
    [SerializeField]
    Sprite _icon;

    public Sprite Icon => _icon;

    [Header("커런시 코드")]
    [SerializeField]
    string _currencyCodeName;
    public string CurrencyCode => _currencyCodeName;

    [Header("레벨당 증가량")]
    [SerializeField]
    int _incrementValuePerLevel;

    public int IncrementValuePerLevel => (int)_valueGenerator.GetIncrementStatValuePerLevel(_Level);

    [SerializeField]
    CharacterStatUpgradeCondition _condition;
    public CharacterStatUpgradeCondition Condition => _condition;

    [SerializeField]
    StatValueGenerator _valueGenerator;
    public StatValueGenerator Generator => _valueGenerator;

    


    public void Upgrade(int targetLevel)
    {
        if(_condition.CanUpgradable(this, targetLevel))
        {
            Managers.Game.SubstractCurrency(((Define.CurrencyID)Currency.ID).ToString(), GetUpgradeCost(targetLevel));
           // Currency.Substract();
            this.Value = CalculateValue(targetLevel);

            
            int _count = targetLevel - _Level;
            _Level = targetLevel;

            

            OnLevelChanged?.Invoke(this, _count);
        }

        UpdateIncrementValuePerLevel();

    }
    public void Unlock(object data)
    {
        if (_unlockCondition == null)
            return;
        if (_isUnlock == true)
            return;
        
        _isUnlock = _unlockCondition.IsUnlock(data) ? true : false;
       
        OnUnLock?.Invoke(_isUnlock);
    }
    public void Reset()
    {
        _Level = _minLevel;


        UpdateIncrementValuePerLevel();

        this.Value = CalculateValue(_Level);
    }
    public int CalculateValue(int level)
    {
        return Generator.GetStatValue(level);
    }
    public void OnChangedMaxLevel(CharacterStat stat, int count)
    {
        _maxLevel = stat.Value;
        OnMaxLevelChanged?.Invoke(this, count);
    }
    public System.Numerics.BigInteger GetUpgradeCost(int targetLevel)
    {
        return targetLevel > MaxLevel ? _condition.GetUpgradeCost(this, MaxLevel) : _condition.GetUpgradeCost(this, targetLevel);
    }
    public int SearchUpgradableMaxLevel()
    {
        if (_Level == _maxLevel)
            return _maxLevel;
        else if (_Level + 1 == _maxLevel)
            return _maxLevel;

        return SearchLevel(_Level + 1 , _maxLevel);


    }
    int SearchLevel(int minLevel, int maxLevel)
    {
        if (Currency.Amount <= _condition.GetUpgradeCost(this, minLevel))
            return minLevel;
        else if (Currency.Amount >= _condition.GetUpgradeCost(this, maxLevel))
            return maxLevel;
        else
        {
            if (Currency.Amount > _condition.GetUpgradeCost(this, minLevel) && Currency.Amount <= _condition.GetUpgradeCost(this, minLevel + 1))
                return minLevel;

            int centerLevel = ((int)minLevel + (int)maxLevel) / 2;

            if (_condition.GetUpgradeCost(this, centerLevel) < Currency.Amount)
                return SearchLevel(centerLevel, maxLevel);
            else
                return SearchLevel(minLevel, centerLevel);
          
        }
    }

    public Currency Currency { get; set; }

    public CharacterStat clone()
    {
        CharacterStat _ret = Instantiate(this);
        _ret.ClearModifier();
        return _ret;
    }
    public CharacterStatSaveData ToSaveData()
    {
        return new CharacterStatSaveData { Key = CodeName.ToString(), Level = _Level, MaxLevel = _maxLevel, IsUnLock = _isUnlock };
    }

    public void Load(CharacterStatSaveData data)
    {
        if (data.Key != CodeName)
            return;

        _Level = data.Level;
        Value = CalculateValue(_Level);

        // 20230902 스테이지 상승 및 스텟레벨 상승으로 인한 변경
        if (CodeName != Define.StatID.Attack.ToString() &&
            CodeName != Define.StatID.CriticalHit.ToString() &&
            CodeName != Define.StatID.CraftAttack.ToString() &&
            CodeName != Define.StatID.CraftCriticalHit.ToString())
                _maxLevel = data.MaxLevel;
        
        
        _isUnlock = data.IsUnLock;

        UpdateIncrementValuePerLevel();



    }
    public void UpdateIncrementValuePerLevel()
    {
        if (_condition.CanUpgradable(this, _Level + 1))
        {
           _incrementValuePerLevel = Generator.GetStatValue(_Level + 1) - Generator.GetStatValue(_Level );
          
        }
           
    }
}
