using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="statvalueGenerator")]
public class StatValueGenerator : ScriptableObject
{
    [System.Serializable]
    public class IntervalGenerator
    {
        [Header(header:"첫번째항")]
        public int min;

        [Header(header: "마지막항")]
        public int max;
        public Generator generator;
        public int GetFirst()
        {
            return (int)generator.GetValue(0);
        }
        public int GetLast()
        {
            return (int)generator.GetValue(max - 1); ;
        }

        public bool IsContain(int targetLevel)
        {
            if (targetLevel >= min && targetLevel <= max)
                return true;
            else
                return false;
        }
        public int GetValue(int targetLevel)
        {
            return (int)generator.GetValue(targetLevel - min);
        }
        public double GetR()
        {
            return generator.r;
        }
    }

    [SerializeField]
    List<IntervalGenerator> _intervalGen;

    public int GetStatValue(int target)
    {
        foreach(IntervalGenerator _statGen in _intervalGen)
        {
            if (_statGen.IsContain(target))
            {
                //int re_targetLevel = target - _statGen.min + 1;
                //return _statGen.GetValue(re_targetLevel);
              
                return _statGen.GetValue(target);
            }

        }
        return _intervalGen[0].GetFirst();
    }

    public double GetIncrementStatValuePerLevel(int target)
    {
        foreach (IntervalGenerator _statGen in _intervalGen)
        {
            if (_statGen.IsContain(target))
            {
                //int re_targetLevel = target - _statGen.min + 1;
                //return _statGen.GetValue(re_targetLevel);

                return _statGen.GetR();
            }

        }
        return _intervalGen[0].GetR();
    }

    


}
