using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CurrencyManager 
{
    Dictionary<Define.CurrencyID, Currency> _Currency;
    public void Init()
    {
        _Currency = new Dictionary<Define.CurrencyID, Currency>();
        foreach (Define.CurrencyID currency in Enum.GetValues(typeof(Define.CurrencyID)))
        {
            _Currency.Add(currency, Managers.Data.CurrencyData[(int)currency].toCurrency());
        }
    }

    public void AddCurrency(Define.CurrencyID Id, BigInteger Amount)
    {
        _Currency[Id].Add(Amount);
    }
    public Currency GetCurrency(Define.CurrencyID Id)
    {
        return _Currency[Id];
    }
}
