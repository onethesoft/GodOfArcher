using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/Currency" , fileName ="Currency_")]
public class Currency : ScriptableObject
{
    [SerializeField]
    int _Id;
    public int ID => _Id;

    [SerializeField]
    string _codeName;
    public string CodeName => _codeName;

    [SerializeField]
    string _shortCodeName;
    public string ShortCodeName => _shortCodeName;

    [SerializeField]
    string _description;
    public string Description => _description;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    BigInteger _Amount;
    public BigInteger Amount => _Amount;

    public void Set(CurrencyData data)
    {
        _Id = data.ID;
        _codeName = data.CodeName;
        _icon = Managers.Resource.Load<Sprite>($"Sprites/{data.Icon}");

        Define.TextData GetText = (Define.TextData)Enum.Parse(typeof(Define.TextData), data.Description);

        _description = Managers.Data.TextData[(int)GetText].Kor;

    }

    public void SetAmount(BigInteger Amount)
    {
        _Amount = Amount;
    }

    public void Add(BigInteger Amount)
    {
        
        if (Amount > 0)
        {
            _Amount += Amount;
        }
        else
        {
            if (_Amount < BigInteger.Abs(Amount))
                return;

            _Amount += Amount;

        }
       
    }
    public void Substract(BigInteger Amount)
    {
        if (Amount < 0)
            _Amount -= Amount;
        else
        {
            if (_Amount < BigInteger.Abs(Amount))
                return;

            _Amount -= Amount;
        }
    }

    public void Reset()
    {
        _Amount = 0;
    }
    public Currency Clone()
    {
        Currency _ret = Instantiate(this);
        _ret._Amount = _Amount;
        return _ret;
    }
    public CurrencySaveData ToSaveData()
    {
        return new CurrencySaveData { Key = CodeName, Amount = _Amount.ToString() };
    }
    public void Load(CurrencySaveData data)
    {
        if (data.Key == CodeName)
            _Amount = BigInteger.Parse(data.Amount);
    }



}