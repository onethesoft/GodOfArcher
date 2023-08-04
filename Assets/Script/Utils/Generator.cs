using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


[CreateAssetMenu(menuName = "Generator/수열생성기")]
[System.Serializable]
public class Generator : ScriptableObject
{
    public enum Mode
    {
        Linear,
        Exponent
    }


    public Mode mode;

 
    public double r;

    
    public int InitialValue;


    // n 항
    // 0 부터 시작
    // 
    public BigInteger GetValue(int n)
    {
        if (n == 0)
            return InitialValue;

        if (mode == Mode.Linear)
        {
            

            BigInteger _ret = (BigInteger)InitialValue;
            double _diffvalue = (n) * r;

            _ret += (BigInteger)_diffvalue;
            return _ret;
        }
        else
        {
            BigInteger initvalue = (BigInteger)InitialValue;
            double _diffvalue = System.Math.Pow(r, n);
            BigInteger _ret = (BigInteger)_diffvalue;
            _ret *= initvalue;


            return _ret;
        }
    }

    // n 항까지의 합
    // n 이 0이면 0을 리턴한다.
    // n 이 1이면 첫항을 리턴한다.
    // n 이 2이면 첫항과 두번째항까지의 합을 계산하여 리턴한다.
    public BigInteger GetSumValue(int n)
    {
        if (n <= 0)
            return 0;
        if (mode == Mode.Linear)
        {
            

            BigInteger _ret = GetValue(n -1);
        
            _ret += (BigInteger)InitialValue;
        
            _ret *= (BigInteger)(n);
          
            return _ret / 2;
        }
        else
        {

            double r_1 = r - 1;
            double numerator = (System.Math.Pow(r, n) - 1) * InitialValue;
            return (BigInteger)(numerator / r_1);
        }
    }

}
