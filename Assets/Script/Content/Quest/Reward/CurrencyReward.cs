using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Currency")]
public class CurrencyReward : Reward
{
    [SerializeField]
    Define.CurrencyID _currencyId;

    public override string GetId()
    {
        return _currencyId.ToString();
    }

    public override void Give(Quest quest)
    {
        Managers.Game.AddCurrency(_currencyId.ToString(), (System.Numerics.BigInteger)Quantity);
    }

    
}
