using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class GameSaveData 
{
    public int Level;
    public int Stage;
    public int ClearStage;
    public int MaxClearStage;
    public int TrollStage;
    public int PlayTime;

    public InventorySaveData Inventory;
    public List<CurrencySaveData> Currency;

    public CharacterStatSystemSaveData PlayerStat;

    public EquipmentSaveData Rune;
    public EquipmentSaveData Pet;
    public EquipmentSaveData Bow;
    public EquipmentSaveData Helmet;
    public EquipmentSaveData Armor;
    public EquipmentSaveData Cloak;


    public DateTime? DailyCheckoutQuestLastClearTime;
    public DateTime? DailyQuestLastClearTime;
    public bool IsBeginner;

    public StageDataList StageInfoList;

    public GameSaveData()
    {

    }

    public GameSaveData(GameData data)
    {
        Level = data.Level;
        Stage = data.Stage;
        ClearStage = data.ClearStage;
        MaxClearStage = data.MaxClearStage;
        TrollStage = data.TrollStage;
        PlayTime = data.PlayTime;

        Inventory = data.Inventory.ToSaveData();
        PlayerStat = data.PlayerStat.ToSaveData();

        Rune = data.Rune.ToSaveData();
        Pet = data.Pet.ToSaveData();
        Bow = data.Bow.ToSaveData();
        Helmet = data.Helmet.ToSaveData();
        Armor = data.Armor.ToSaveData();
        Cloak = data.Cloak.ToSaveData();


        Currency = data.Currency.Select(x => x.Value.ToSaveData()).ToList();

        DailyQuestLastClearTime = data.DailyQuestLastClearTime;
        DailyCheckoutQuestLastClearTime = data.DailyCheckoutQuestLastClearTime;
        IsBeginner = data.IsBeginner;

        StageInfoList = data.StageInfoList;
    }

    public static List<CurrencySaveData> ParseCurrency(string CurrencyArrayString)
    {
        var CurrencyArr = JArray.Parse(CurrencyArrayString);
        return CurrencyArr.ToObject <List<CurrencySaveData>>();
    }

    public string ToString()
    {
        JObject saveObject = new JObject();
        saveObject.Add("Level", Level);
        saveObject.Add("Stage", Stage);
        saveObject.Add("ClearStage", ClearStage);
        saveObject.Add("MaxClearStage", MaxClearStage);
        saveObject.Add("TrollStage", TrollStage);
        saveObject.Add("PlayTime", PlayTime);

        saveObject.Add("Inventory", JObject.FromObject(Inventory));
        saveObject.Add("Currency", JArray.FromObject(Currency));
        saveObject.Add("PlayerStat", JObject.FromObject(PlayerStat));

        saveObject.Add("Rune", JObject.FromObject(Rune));
        saveObject.Add("Pet", JObject.FromObject(Pet));
        saveObject.Add("Bow", JObject.FromObject(Bow));
        saveObject.Add("Helmet", JObject.FromObject(Helmet));
        saveObject.Add("Armor", JObject.FromObject(Armor));
        saveObject.Add("Cloak", JObject.FromObject(Cloak));


        saveObject.Add("DailyCheckoutQuestLastClearTime", DailyCheckoutQuestLastClearTime);
        saveObject.Add("DailyQuestLastClearTime", DailyQuestLastClearTime);
        saveObject.Add("IsBeginner", IsBeginner);

        saveObject.Add("StageInfoList", JObject.FromObject(StageInfoList));

        return saveObject.ToString();


    }
    public static GameSaveData Parse(string data)
    {
        var loadObject = JObject.Parse(data);
        GameSaveData _ret = loadObject.ToObject<GameSaveData>();

        if (loadObject["DailyQuestLastClearTime"] as JToken != null)
        {
            if (!string.IsNullOrEmpty(loadObject["DailyQuestLastClearTime"].ToString()))
            {
                _ret.DailyQuestLastClearTime = DateTime.Parse(loadObject["DailyQuestLastClearTime"].ToString());
            }
        }

        if (loadObject["DailyCheckoutQuestLastClearTime"] as JToken != null)
        {
            if (!string.IsNullOrEmpty(loadObject["DailyCheckoutQuestLastClearTime"].ToString()))
            {
                _ret.DailyCheckoutQuestLastClearTime = DateTime.Parse(loadObject["DailyCheckoutQuestLastClearTime"].ToString());
            }
        }

        return _ret;

    }

}
#region StageInfo

[System.Serializable]
public class StageDataList
{
    public List<StageData> List;

    public static StageDataList FromJson(string json)
    {
        return JsonUtility.FromJson<StageDataList>(json);
    }
   
}

[System.Serializable]
public class StageData
{
    public string type;
    public int FreePassCount;
    public string time;

    public void ResetStageData(StageTask task)
    {
        if (type != task.type.ToString())
            return;
        
        if(GlobalTime.Now.ToLocalTime().Date > DateTime.Parse(time , null , System.Globalization.DateTimeStyles.RoundtripKind).ToLocalTime().Date)
        {
            FreePassCount = task.DailyFreePassCount;
            time = Util.GetTimeString( GlobalTime.Now);
        }

        
    }

    
}

#endregion
#region Revive
[System.Serializable]
public class ReviveInfo
{
    public int ReviveCount;
    public DateTime ReviveTime;

    public static ReviveInfo Create()
    {
        return new ReviveInfo { ReviveCount = 0, ReviveTime = GlobalTime.Now };
    }
    public static ReviveInfo FromJson(string json)
    {
        var obj = JObject.Parse(json);
        ReviveInfo _info = new ReviveInfo { ReviveCount = obj["ReviveCount"].Value<int>(), ReviveTime = DateTime.Parse(obj["ReviveTime"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind) };
        return _info;
    }

    public string ToJson()
    {
        JObject obj = new JObject();
        obj.Add("ReviveCount", ReviveCount);
        obj.Add("ReviveTime", Util.GetTimeString(ReviveTime));
        return obj.ToString();
    }

    public bool DoReset()
    {
        if(ReviveTime.ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
        {
            ReviveCount = 0;
            ReviveTime = GlobalTime.Now;
            return true;
        }
        return false;
    }
    
    public void DoRevive()
    {
        ReviveCount++;
        ReviveTime = GlobalTime.Now;
    }

    public bool CanRevive()
    {
        return ReviveCount < 5 ? true : false;
    }
}

#endregion
#region Equipment
[System.Serializable]
public class EquipmentSlotSaveData
{
    public int index;
    public bool IsLock;
    public bool IsEquip;
    public string ItemId;
}

[System.Serializable]
public class EquipmentSaveData
{
    public List<EquipmentSlotSaveData> SlotList;

    public static EquipmentSaveData FromJson(string data)
    {
        return JsonUtility.FromJson<EquipmentSaveData>(data);
    }
}
#endregion

#region Currency
[System.Serializable]
public class CurrencySaveData
{
    public string Key;
    public string Amount;

    public static CurrencySaveData FromJson(string data)
    {
        return JsonUtility.FromJson<CurrencySaveData>(data);
    }
}
#endregion

#region Stat
[System.Serializable]
public class CharacterStatSystemSaveData
{
    public List<CharacterStatSaveData> StatList;

    public static CharacterStatSystemSaveData FromJson(string data)
    {
        return JsonUtility.FromJson<CharacterStatSystemSaveData>(data);
    }
}


[System.Serializable]
public class CharacterStatSaveData
{
    public string Key;
    public int Level;
    public int MaxLevel;
    public bool IsUnLock;
}
#endregion

#region Inventory
[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> ItemList;
}


[System.Serializable]
public class ItemSaveData
{
    public string DisplayName;
    public string Description;
    public System.DateTime? Expiration;
    public string ItemId;
    public string ItemClass;
    public string ItemInstanceId;
    public int? RemainingUses;
    public string UnitCurrency;
    public uint UnitPrice;
    public int? UsesIncrementedBy;
    public bool IsStackable;
}

#endregion 

[System.Serializable]
public class RouletteInfo
{
    public int PlayRouletteCount = 0;

    public int GetCoinToPlayRoulette => (PlayRouletteCount / 10) + 1;
    public string ToJson()
    {
        JObject obj = new JObject();
        obj.Add("PlayRouletteCount", PlayRouletteCount);
        return obj.ToString();
    }

    public static RouletteInfo FromJson(string json)
    {
        var obj = JObject.Parse(json);
        RouletteInfo _info = new RouletteInfo { PlayRouletteCount = obj["PlayRouletteCount"].Value<int>() };
        return _info;
    }
}