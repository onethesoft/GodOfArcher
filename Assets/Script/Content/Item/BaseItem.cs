using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Playfab ItemInstance Model

[System.Serializable]
[CreateAssetMenu(menuName ="¾ÆÀÌÅÛ/Default")]
public class BaseItem : ScriptableObject 
{
    public delegate void OnRevokeHandler(BaseItem item);
    public OnRevokeHandler OnRevokeNotifier = null;
    public enum ItemType
    {
        Item,
        Rune , 
        Pet ,
        Essence ,
        Heart,

    }

    [SerializeField]
    protected string _DisplayName;
    public string DisplayName => _DisplayName;

    [SerializeField]
    protected string _Description;
    public string Description => _Description;

    [SerializeField]
    protected DateTime? _Expiration = null;
    public DateTime? Expiration => _Expiration;
    

    [SerializeField]
    protected string _ItemId;
    public string ItemId => _ItemId;

    [SerializeField]
    protected string _ItemClass;
    public string ItemClass => _ItemClass;

    [SerializeField] 
    protected string  _ItemInstanceId;
    public string ItemInstanceId => _ItemInstanceId;

    [SerializeField]
    protected int? _RemainingUses;
    public int? RemainingUses => _RemainingUses;

    [SerializeField]
    protected string _unitCurrency;
    public string UnitCurrency => _unitCurrency;

    [SerializeField]
    protected uint _unitPrice;
    public uint UnitPrice => _unitPrice;

    [SerializeField]
    protected int? _usesIncrementedBy = 0;
    public int? UsesIncrementedBy => _usesIncrementedBy;

    [SerializeField]
    protected bool _isStackable = true;
    public bool IsStackable => _isStackable;

    [SerializeField]
    protected Dictionary<string, string> _customdata = null;

    protected DateTime? _purchaseDate;
    public DateTime? PurchaseDate => _purchaseDate;

    public void AddCustomdata(Dictionary<string,string> data)
    {
        if (data == null)
            return;
        if (data.Count <= 0)
            return;

        _customdata = data;
        
    }
    public string GetCustomData(string key)
    {
        string _data;
        if (_customdata == null)
        {
            return string.Empty;
        }

        if (_customdata.TryGetValue(key, out _data))
            return _data;
        else
        {
            return string.Empty;
        }
        
    }
    public Dictionary<string, string> GetCustomData() => _customdata;

    public void SetIncrement(int value)
    {
        _usesIncrementedBy = value;
    }
    public virtual void Add(int Count)
    {
        if (Count <= 0)
            return;

        if (!_RemainingUses.HasValue)
            _RemainingUses = 0;

        _RemainingUses += Count;
        
    }

    public virtual void Consume(int Count)
    {
        if (Count <= 0)
            return;

        _RemainingUses -= Count;
        _usesIncrementedBy = -Count;
        if (_RemainingUses.GetValueOrDefault() <= 0)
            OnRevokeNotifier?.Invoke(this);
    }

    public virtual int? GetUsableCount()
    {
        return _RemainingUses;
    }
    public void SetExpiration(DateTime expirationData)
    {
        _Expiration = expirationData;
    }
    public void SetItemInstanceId(string instanceId)
    {
        _ItemInstanceId = instanceId;
    }

    public virtual PlayFab.ServerModels.ItemGrant ToGrantItem()
    {
        PlayFab.ServerModels.ItemGrant _ret;
        _ret = new PlayFab.ServerModels.ItemGrant { ItemId = ItemId, PlayFabId = Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId };

        if (_customdata == null)
            return _ret;
       
        _ret.Data = _customdata;
        return _ret;


    }

    public virtual PlayFab.ClientModels.ConsumeItemRequest ToConsumeItem(int ConsumeCount = 1)
    {
        PlayFab.ClientModels.ConsumeItemRequest _ret;
        _ret = new PlayFab.ClientModels.ConsumeItemRequest { ConsumeCount = ConsumeCount > 0 ? ConsumeCount : RemainingUses.Value, ItemInstanceId = ItemInstanceId };
        return _ret;
    }



    public virtual void Setup(PlayFab.ClientModels.ItemInstance item)
    {
        _DisplayName = item.DisplayName;
        _Expiration = item.Expiration;
        _ItemId = item.ItemId;
        _ItemClass = item.ItemClass;
        _ItemInstanceId = item.ItemInstanceId;
        _RemainingUses = item.RemainingUses;
        _unitCurrency = item.UnitCurrency;
        _unitPrice = item.UnitPrice;
        _usesIncrementedBy = item.UsesIncrementedBy;
        _purchaseDate = item.PurchaseDate;

        if(item.CustomData != null)
            if(item.CustomData.Count > 0)
                _customdata = item.CustomData;
    }
    
    public virtual BaseItem Clone()
    {
        BaseItem _ret = Instantiate(this);
        if (!_ret.RemainingUses.HasValue)
            _ret._RemainingUses = 1;
        return _ret;
    }
    

    public ItemSaveData ToSaveData()
    {
        return new ItemSaveData { Description = _Description, ItemId = _ItemId, ItemClass = _ItemClass, DisplayName = _DisplayName, Expiration = _Expiration, ItemInstanceId = _ItemInstanceId, RemainingUses = _RemainingUses, UsesIncrementedBy = _usesIncrementedBy, IsStackable = _isStackable, UnitCurrency = _unitCurrency, UnitPrice = _unitPrice };
    }
    public virtual void Setup(ItemSaveData data)
    {
        _DisplayName = data.DisplayName;
        _ItemId = data.ItemId;
        _ItemClass = data.ItemClass;
        _Description = data.Description;
        _Expiration = data.Expiration;
        _isStackable = data.IsStackable;
        _ItemInstanceId = data.ItemInstanceId;
        _RemainingUses = data.RemainingUses;
        _usesIncrementedBy = data.UsesIncrementedBy;
        _unitCurrency = data.UnitCurrency;
        _unitPrice = data.UnitPrice;
    }
    
}
