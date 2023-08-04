using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BundleCurrencyContent
{
    public Currency Currency;
    public int Amount;
}
[System.Serializable]
public class BundleItemContent
{
    public BaseItem Item;
    public int Amount;
}




[CreateAssetMenu(menuName = "아이템/번들" , fileName ="Bundle_")]
public class Bundle : BaseItem
{
    [SerializeField]
    List<BundleItemContent> _contents;
    public IReadOnlyList<BundleItemContent> Content => _contents;

    [SerializeField]
    List<BundleCurrencyContent> _currencyList;
    public IReadOnlyList<BundleCurrencyContent> Currencies => _currencyList;

    [SerializeField]
    List<BundleItemContent> _dailyContents;
    public IReadOnlyList<BundleItemContent> DailyContents => _dailyContents;

    [SerializeField]
    List<BundleCurrencyContent> _daillyCurrencyList;
    public IReadOnlyList<BundleCurrencyContent> DailyCurrencies => _daillyCurrencyList;

    public System.DateTime? GetDailyRewardDate()
    {
        if (_customdata == null || _customdata.ContainsKey("Daily") == false)
            return null;
       
        return System.DateTime.Parse(_customdata["Daily"], null, System.Globalization.DateTimeStyles.RoundtripKind);
         
    }

    public bool IsExpireDailyDate()
    {
        System.DateTime? _DailyDate = GetDailyRewardDate();
        if (_DailyDate.HasValue == false)
            return false;

        //return (GlobalTime.Now.ToLocalTime() - _DailyDate.Value).TotalMinutes > 1.0 ? true : false;
        return GlobalTime.Now.ToLocalTime().Date > _DailyDate.Value.ToLocalTime().Date ? true : false;


    }
    public void UpdateDailyDate()
    {
        if (_customdata == null )
        {
            _customdata = new Dictionary<string, string>();
            _customdata.Add("Daily", Util.GetTimeString(GlobalTime.Now));
        }
        else if(_customdata.ContainsKey("Daily"))
            _customdata["Daily"] = Util.GetTimeString(GlobalTime.Now);
        else
            _customdata.Add("Daily", Util.GetTimeString(GlobalTime.Now));

        
    }
    


    public override BaseItem Clone()
    {
        Bundle _ret = Instantiate(this);
        _ret._RemainingUses = 1;
        return _ret;
    }

    public PlayFab.ServerModels.ItemGrant ToGrantItem()
    {

        PlayFab.ServerModels.ItemGrant _ret;
        if(_dailyContents != null && _dailyContents.Count > 0)
            _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId, PlayFabId = Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId , Data = new Dictionary<string, string>() { { "Daily", Util.GetTimeString(GlobalTime.Now)} } };
        else
            _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId, PlayFabId = Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId };

        return _ret;
        //PlayFab.ServerModels.ItemGrant _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId , Data = new Dictionary<string, string>() { } }
    }

    public List<PlayFab.ServerModels.ItemGrant> ToGrantDailyReward()
    {
        if (GetDailyRewardDate() == null)
            return null;

        List<PlayFab.ServerModels.ItemGrant> _ret = new List<PlayFab.ServerModels.ItemGrant>();

        foreach (var dailyReward in _dailyContents)
        {
            for(int i=0;i<dailyReward.Amount;i++)
                _ret.Add(new PlayFab.ServerModels.ItemGrant { ItemId = dailyReward.Item.ItemId, PlayFabId = Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId });
        }
        
       
        return _ret;
        //PlayFab.ServerModels.ItemGrant _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId , Data = new Dictionary<string, string>() { } }
    }
   

    

}
