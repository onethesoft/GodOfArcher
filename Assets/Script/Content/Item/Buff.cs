using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "아이템/버프", fileName = "버프_")]
public class Buff : BaseItem
{
    public enum Type
    {
        AutoAttack,
        AttackSpeed,
        Damage,
        Skill,
        GoldDropRate,
        CPDropRate
    }
    public enum Rating
    {
        AD = 0 ,
        Beginner,
        IAP
    }
    public enum Id
    {
        Buff_GoldDropRate_AD,
        Buff_Attack_AD,
        Buff_AttackSpeed_AD,
        Buff_Skill_AD,
        Buff_Auto_AD,
        Buff_CPDropRate_AD,
        Buff_GoldDropRate_IAP,
        Buff_Attack_IAP,
        Buff_AttackSpeed_IAP,
        Buff_Skill_IAP,
        Buff_Auto_IAP,
        Buff_CPDropRate_IAP,
        Buff_GoldDropRate_Beginner,
        Buff_Attack_Beginner,
        Buff_AttackSpeed_Beginner,
        Buff_Skill_Beginner,
        Buff_Auto_Beginner,
        Buff_CPDropRate_Beginner
    }

    public enum CustomDataKey
    {
        Expire
    }

    

    [SerializeField]
    Type _type;
    public Type type => _type;

    [SerializeField]
    Rating _rating;
    public Rating Level => _rating;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    Sprite _OnBackground;
    public Sprite OnBackground => _OnBackground;

    [SerializeField]
    Sprite _OffBackground;
    public Sprite OffBackground => _OffBackground;

    [SerializeField]
    OutlineData _OnOutline;
    public OutlineData OnOutline => _OnOutline;

    [SerializeField]
    OutlineData _OffOutline;
    public OutlineData OffOutline => _OffOutline;

    [SerializeField]
    int _fontSize;
    public int FontSize => _fontSize;

    [SerializeField]
    int _Minutes;

    public int Minutes => _Minutes;




    [SerializeField]
    List<StatModifier> _statModifier;
    public IReadOnlyList<StatModifier> StatModifier => _statModifier;


    
    public override BaseItem Clone()
    {
        Buff _ret = Instantiate(this);
        
        
        _ret._RemainingUses = 1;
        _ret._statModifier = new List<StatModifier>();
        foreach (StatModifier _stat in _statModifier)
        {
            StatModifier _added = new StatModifier(_stat);
            _ret._statModifier.Add(_added);
        }

        if (_Minutes <= 0)
            _ret._Expiration = null;
        else
            _ret._Expiration = GlobalTime.Now.AddMinutes(_Minutes);

        Debug.Log(ItemId + " : " + GlobalTime.Now);
        Debug.Log(ItemId + " : " + _ret._Expiration);
        return _ret;
        
    }
    

    public TimeSpan? RemainingTime()
    {
        if(!_Expiration.HasValue)
            return null;

        return _Expiration.Value.ToLocalTime() - GlobalTime.Now.ToLocalTime();
    }

    public override void Setup(ItemInstance item)
    {
        base.Setup(item);
        if (item.CustomData != null)
            if (item.CustomData.ContainsKey(CustomDataKey.Expire.ToString()))
            {
                _Expiration = DateTime.Parse(item.CustomData[CustomDataKey.Expire.ToString()] , null , System.Globalization.DateTimeStyles.RoundtripKind);

                Debug.Log("Buff Setup CustomData : " + item.CustomData[CustomDataKey.Expire.ToString()]);
                Debug.Log("Buff Setup : " + _Expiration);
            }
        
    }
    public override PlayFab.ServerModels.ItemGrant ToGrantItem()
    {
        PlayFab.ServerModels.ItemGrant _ret;
        if (RemainingTime() != null)
            _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId, PlayFabId = Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId, Data = new Dictionary<string, string>() { { CustomDataKey.Expire.ToString(), Util.GetTimeString(_Expiration.Value) } } };
        else
            _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId, PlayFabId = Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId };

        return _ret;
        //PlayFab.ServerModels.ItemGrant _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId , Data = new Dictionary<string, string>() { } }
    }

}
