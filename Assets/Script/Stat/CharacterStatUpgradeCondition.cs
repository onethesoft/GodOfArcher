using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(menuName ="스텟업그레이드 조건")]
public class CharacterStatUpgradeCondition : ScriptableObject
{
    [System.Serializable]
    class CharacterStatUpgradeConditionData
    {
        public int minLevel;
        public int maxLevel;

        public Generator Generator;

        public bool IsContain(int targetLevel)
        {
            Debug.Log(minLevel);
            Debug.Log(maxLevel);
            if (targetLevel > minLevel && targetLevel <= maxLevel)
                return true;
            else
                return false;
        }

        public BigInteger GetValue(int targetLevel)
        {
            return Generator.GetSumValue(targetLevel );
        }
    }
    

    [SerializeField]
    List<CharacterStatUpgradeConditionData> _costCalucations;
    public bool CanUpgradable(int currentLevel , int targetLevel)
    {
        CharacterStatUpgradeConditionData _target = _costCalucations.Find(x => x.IsContain(targetLevel));
        CharacterStatUpgradeConditionData _current = _costCalucations.Find(x => x.IsContain(currentLevel));
        if (_target == _current)
        {
            BigInteger _targetCost = _target.GetValue(targetLevel);
            BigInteger _currentCost = _target.GetValue(currentLevel);

            Debug.Log(_targetCost - _currentCost);
         

        }
        else
        {
            BigInteger diff;
            foreach (CharacterStatUpgradeConditionData ite in _costCalucations.FindAll(x => x.minLevel > currentLevel && x.maxLevel < targetLevel))
            {
                if (ite.IsContain(currentLevel))
                {
                    //Debug.Log("Level : " + ite.minLevel);
                    //Debug.Log(ite.GetValue(ite.maxLevel) - ite.GetValue(currentLevel));
                    diff += (ite.GetValue(ite.maxLevel) - ite.GetValue(currentLevel));
                }
                else if (ite.IsContain(targetLevel))
                {

                    //Debug.Log("Level : " + ite.minLevel);
                    //Debug.Log(ite.GetValue(ite.maxLevel) - ite.GetValue(currentLevel));
                    diff += (ite.GetValue(targetLevel) - ite.GetValue(ite.minLevel));
                }
                else
                {
                    diff += ite.GetValue(ite.maxLevel) - ite.GetValue(ite.minLevel);
                }
                    

               // Debug.Log("Diff : " + diff.ToString());

            }

            diff += (_current.GetValue(_current.maxLevel) - _current.GetValue(currentLevel));

            if(_target.IsContain(targetLevel))
            {
                
                Debug.Log(_target.minLevel);
                diff += (_target.GetValue(targetLevel) - _target.GetValue(_target.minLevel));
            }
                

            Debug.Log(diff.ToString());
           


        }

        return true;
    }
    public bool CanUpgradable(CharacterStat stat, int targetLevel)
    {
        if (targetLevel > stat.MaxLevel)
            return false;
        if (targetLevel <= stat.Level)
            return true;

        BigInteger cost = GetUpgradeCost(stat, targetLevel);

        if (cost <= stat.Currency.Amount)
        {
            return true;
        }
        else
            return false;

        
        
    }

    public BigInteger GetUpgradeCost(CharacterStat stat, int targetLevel)
    {

        CharacterStatUpgradeConditionData _target = _costCalucations.Find(x => x.IsContain(targetLevel));
        CharacterStatUpgradeConditionData _current;
        if (stat.Level <= 1)
            _current = _costCalucations[0];
        else
            _current = _costCalucations.Find(x => x.IsContain(stat.Level));
        //CharacterStatUpgradeConditionData _current = _costCalucations.Find(x => x.IsContain(stat.Level));
        
        if (_target == _current)
        {
            BigInteger _targetCost;
            BigInteger _currentCost; 

            _targetCost = _target.GetValue(targetLevel);
            _currentCost = _target.GetValue(stat.Level);

           

            return _targetCost - _currentCost;
        }
        else
        {
            BigInteger diff;
            foreach (CharacterStatUpgradeConditionData ite in _costCalucations.FindAll(x => x.minLevel > stat.Level && x.maxLevel < targetLevel))
            {
                diff += ite.GetValue(ite.maxLevel) - ite.GetValue(ite.minLevel);
            }

           
            diff += (_current.GetValue(_current.maxLevel) - _current.GetValue(stat.Level));
        
           
            if (_target.IsContain(targetLevel))
            {
                diff += (_target.GetValue(targetLevel) - _target.GetValue(_target.minLevel));
            }



            return diff;
        }

        
    }
}
