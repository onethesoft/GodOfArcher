using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CurrencyData 
{
    public int ID;
    public string CodeName;
    public string Description;
    public string Icon;

    public Currency toCurrency()
    {
        Currency cur = new Currency();
        cur.Set(this);
        return cur;
    }
}



[System.Serializable]
public class CurrencyDataArray : ILoader<int, CurrencyData>
{
    public List<CurrencyData> data;

    public Dictionary<int, CurrencyData> MakeDict()
    {
        Dictionary<int, CurrencyData> dict = new Dictionary<int, CurrencyData>();
        foreach (CurrencyData item in data)
        {
            dict.Add(item.ID, item);
        }
        return dict;
    }
}
