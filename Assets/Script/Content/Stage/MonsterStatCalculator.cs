using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(menuName ="����/HP������")]
public class MonsterStatCalculator : ScriptableObject
{
    [System.Serializable]
    public class IntervalGenerator
    {
        [Header(header: "ù��°��")]
        public int min;

        [Header(header: "��������")]
        public int max;
        public Generator generator;
        public int GetFirst()
        {
            return (int)generator.GetValue(1);
        }
        public int GetLast()
        {

            return (int)generator.GetValue(max - 1); ;

        }

        public bool IsContain(int targetLevel)
        {
            if (targetLevel > min && targetLevel <= max)
                return true;
            else
                return false;
        }
        public BigInteger GetValue(int targetLevel)
        {
            int diffLevel = targetLevel - min;
            return generator.GetValue(diffLevel);
        }
    }

    [SerializeField]
    List<IntervalGenerator> _intervalGen;

    public virtual BigInteger GetHP(int targetLevel)
    {
        foreach (IntervalGenerator _statGen in _intervalGen)
        {
            if (_statGen.IsContain(targetLevel))
                return _statGen.GetValue(targetLevel);

        }
        return _intervalGen[0].GetFirst();
    }
}
