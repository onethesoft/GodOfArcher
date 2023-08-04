using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;

public class Inventory : ScriptableObject
{
    [SerializeField]
    List<BaseItem> _itemList;

    public delegate void OnChangeItemHandler();
    public OnChangeItemHandler OnItemChanged;
    public Inventory()
    {
        _itemList = new List<BaseItem>();
    }

    public List<BaseItem> AddItem(string ItemId , int Count, Dictionary<string,string> data = null)
    {
        BaseItem _item = Managers.Item.Database.ItemList.Where(x => x.ItemId == ItemId).FirstOrDefault();
        if (_item == null)
            throw new Exception("ItemId is not found");

        List<BaseItem> _addedItemList = new List<BaseItem>();


        if(_item.IsStackable)
        {
            BaseItem _findItem = _itemList.Where(x => x.ItemId == ItemId).FirstOrDefault();
            if(_findItem == null)
            {
                _findItem = Managers.Item.CreateItemInstance(ItemId);
                _findItem.SetIncrement(Count);
                _findItem.Add(Count - 1);
                _findItem.OnRevokeNotifier -= RevokeItem;
                _findItem.OnRevokeNotifier += RevokeItem;
                _itemList.Add(_findItem);
            }
            else
            {
                _findItem.Add(Count);
                _findItem.SetIncrement(Count);
               
            }

            if (_findItem is Buff)
            {
                Managers.Game.GiveBuff(_findItem as Buff);
            
                Debug.Log("Inventory Item is AddedBuff Exp : " + _findItem.Expiration);

            }

            _findItem.AddCustomdata(data);
            _addedItemList.Add(_findItem);
        }
        else
        {
            for(int i= 0; i< Count;i++)
            {
                BaseItem _addedItem = Managers.Item.CreateItemInstance(ItemId);
                _addedItem.SetIncrement(1);
                _addedItem.OnRevokeNotifier -= RevokeItem;
                _addedItem.OnRevokeNotifier += RevokeItem;
                
                _addedItem.AddCustomdata(data);
                _itemList.Add(_addedItem);
                _addedItemList.Add(_addedItem);
            }
                
        }

        

        OnItemChanged?.Invoke();

        return _addedItemList;

    }


    public void AddItem(BaseItem item)
    {
        if (item.IsStackable)
        {
            if (item is Buff)
                Managers.Game.GiveBuff(item as Buff);

            BaseItem findItem = _itemList.Find(x => (x.ItemId == item.ItemId) && (x.ItemClass == item.ItemClass));
            if (findItem != null)
            {
                if (item.RemainingUses.HasValue)
                    findItem.Add(item.RemainingUses.Value);
                else
                    findItem.Add(1);
            }
            else
            {
                item.OnRevokeNotifier -= RevokeItem;
                item.OnRevokeNotifier += RevokeItem;
                _itemList.Add(item);
            }

            
        }
        else
        {
            item.OnRevokeNotifier -= RevokeItem;
            item.OnRevokeNotifier += RevokeItem;
            _itemList.Add(item);
        }

        


        OnItemChanged?.Invoke();
    }
    public void RevokeItem(BaseItem item)
    {
        BaseItem _get = _itemList.Find(x => x == item);
        //Debug.Log("Inventory Revoke Item : " + _get.ItemId);
        if (_get != null)
            _itemList.Remove(_get);

        _get.OnRevokeNotifier -= RevokeItem;
        OnItemChanged?.Invoke();

    }
    public bool IsFindItem(string ItemId)
    {
        foreach(BaseItem item in _itemList)
        {
            if (item.ItemId == ItemId)
                return true;
        }
        return false;
        
    }
    public bool IsFindItem(Predicate<BaseItem> predicate)
    {
        var array = _itemList.ToArray();
        bool _ret;
        try
        {
            _ret = Array.Exists(array, predicate);
        }
        finally
        {
            Array.Clear(array, 0, array.Length);
        }

        return _ret;
            //return Array.Exists(_itemList.ToArray(), predicate);
    }
    
    public BaseItem GetItem(string ItemId)
    {
        return _itemList.Find(x => x.ItemId == ItemId);
    }
    public List<BaseItem> FindAll(List<BaseItem> FindList)
    {
        return _itemList.Where(x => FindList.Any(y => y.ItemId == x.ItemId)).ToList();
    }
    public List<BaseItem> FindAll(Predicate<BaseItem> predicate)
    {
        return Array.FindAll(_itemList.ToArray(), predicate).ToList();
    }
    public BaseItem Find(Predicate<BaseItem> predicate)
    {
        return _itemList.Find(predicate);
    }
    public BaseItem Find(BaseItem FindItem)
    {
        return _itemList.Find(x => FindItem.ItemId == x.ItemId);
    }
    public void RemoveItem(string ItemId)
    {
        BaseItem _item = _itemList.Find(x => x.ItemId == ItemId);
        if (_item != null && _item is Buff)
            Managers.Game.RemoveBuff(_item as Buff);

        _itemList.RemoveAll(x => x.ItemId == ItemId);
        OnItemChanged?.Invoke();
    }
    public void ConsumeItem(string ItemId , int Count)
    {
        _itemList.FindAll(x => x.ItemId == ItemId).First().Consume(Count);
        OnItemChanged?.Invoke();
    }
    public List<Mail> GetMail()
    {

        return _itemList.Where(x => x is Mail).Select(x => x as Mail).ToList();
    }
    public List<BaseItem> ToList()
    {
        return _itemList;
    }

    public InventorySaveData ToSaveData()
    {
        return new InventorySaveData { ItemList = _itemList.Select(x=>x.ToSaveData()).ToList()};
    }
    public void Load(InventorySaveData saveData)
    {
        foreach(var data in saveData.ItemList)
        {
            BaseItem item = Managers.Item.LoadFrom(data);
            item.OnRevokeNotifier -= RevokeItem;
            item.OnRevokeNotifier += RevokeItem;
            _itemList.Add(item);

            
            if(item.Expiration.HasValue)
            {

                if (DateTime.Compare(item.Expiration.Value , GlobalTime.Now) <= 0)
                    RemoveItem(item.ItemId);
                else
                {
                    /*
                    Managers.Job.ReserveJob(item.Expiration.Value - GlobalTime.Now, () =>
                    {
                        Managers.Game.GetInventory().RemoveItem(item.ItemId);
                        //RemoveItem(item.ItemId);
                    });
                    */
                }
                
            }
        }
    }

    public void Load(List<PlayFab.ClientModels.ItemInstance> itemList)
    {
        foreach (var data in itemList)
        {
            BaseItem item = Managers.Item.LoadFrom(data);
            item.OnRevokeNotifier -= RevokeItem;
            item.OnRevokeNotifier += RevokeItem;
            _itemList.Add(item);
            if (item.Expiration.HasValue)
            {
                if (item.Expiration.Value.ToLocalTime() <= GlobalTime.Now.ToLocalTime())
                {
                    if(item is Buff)
                    {
                        
                    }
                    else
                    {
                        RemoveItem(item.ItemId);
                    }
                }
                    
                
            }
        }
    }



}
