using com.onethesoft.GodOfArcher;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface ILoader < key , value >
{
    Dictionary<key, value> MakeDict();
}
public class DataManager
{
    public Dictionary<int, StatItemData> StatItemData { get; private set; } = new Dictionary<int, StatItemData>();
    public Dictionary<int, StatData> StatData { get; private set; } = new Dictionary<int, StatData>();
    public Dictionary<int, TextData> TextData { get; private set; } = new Dictionary<int, TextData>();
    public Dictionary<int, StatData> AttackStatData { get; private set; } = new Dictionary<int, StatData>();
    public Dictionary<int, CurrencyData> CurrencyData { get; private set; } = new Dictionary<int, CurrencyData>();

    public Dictionary<int, QuestData> QuestData { get; private set; } = new Dictionary<int, QuestData>();
    public void Init()
    {
        StatData = LoadJson<StatDataArray, int, StatData>(Define.DataFileList.StatData.ToString()).MakeDict();
        StatItemData = LoadJson<StatItemDataArray , int, StatItemData>(Define.DataFileList.StatItemData.ToString()).MakeDict();
        TextData = LoadJson<TextDataArray, int, TextData>(Define.DataFileList.TextData.ToString()).MakeDict();

        CurrencyData = LoadJson<CurrencyDataArray, int, CurrencyData>(Define.DataFileList.CurrencyData.ToString()).MakeDict();
        #region Stat
        //AttackStatData = LoadJson<StatDataArray, int, StatData>(Define.DataFileList.AttackStatData.ToString()).MakeDict();
        #endregion

        #region Quest
        QuestData = LoadJson<QuestDataArray, int, QuestData>(Define.DataFileList.QuestTable.ToString()).MakeDict();
        #endregion
    }


    Loader LoadJson< Loader , key , value >(string path) where Loader : ILoader<key , value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        Loader data = JsonUtility.FromJson<Loader>(textAsset.text);
        return data;
    }
    public string [] LoadProhibitedText()
    {
        TextAsset text = Managers.Resource.Load<TextAsset>($"Data/{Define.DataFileList.fword_list.ToString()}");
        return text.text.Split('\n');
    }
}
