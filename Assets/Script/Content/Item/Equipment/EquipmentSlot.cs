using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/ÀåÂø/ÀåÂø½½·Ô", fileName = "½½·Ô_")]
public class EquipmentSlot : BaseItem, IEquipmentSlot
{
    public delegate void OnEquipItemHandler(EquipableItem item);
    public delegate void OnUnEquipItemHandler(EquipableItem item);

    public delegate void OnUnlockHandler();
    

    public OnEquipItemHandler EquipItem;
    public OnUnEquipItemHandler UnEquipItem;

    public OnUnlockHandler OnUnlock;

    [SerializeField]
    private int _index;
    public int Index => _index;

    [SerializeField]
    private Category _category;
    public Category category => _category;

    [SerializeField]
    private EquipmentSystem _parent;
    public EquipmentSystem Parent => _parent;
    public void SetParent(EquipmentSystem parent)
    {
        _parent = parent;
    }

    [SerializeField]
    int _DescriptionFontSize = 20;
    public int DescriptionFontSize => _DescriptionFontSize;

    [SerializeField]
    bool _IsLock;
    public bool IsLock => _IsLock;

    [SerializeField]
    bool _isEquip;
    public bool IsEquip => _isEquip;

    [SerializeField]
    EquipableItem _item = null;
    public EquipableItem GetItem => _item;

    [SerializeField]
    List<UnlockableCondition> _condition;


    private GameObject _itemprefab;

    [SerializeField]
    Vector3 _itemprefabPos;

    public bool IsUnLockable
    {
        get
        {
            if (_condition != null)
                return _condition.All(x=>x.IsUnlockable(this, _parent));
            else
                return true;
        }
    }

    public string UnLockableReson { 
        get {
            if (IsUnLockable == true)
                return string.Empty;

            return _condition.Where(x => x.IsUnlockable(this, _parent) == false).FirstOrDefault().Description;
        } 
    }

    public override BaseItem Clone()
    {
        return Instantiate(this);
    }

    public void Equip(EquipableItem item)
    {
        if (!_isEquip && item.GetUsableCount() > 0)
        {
            _isEquip = true;
            _item = item;
            _item.Equipped();
      
            if (item is Pet)
            {
                Pet pet = _item as Pet;
                _itemprefab = Managers.Game.Spawn(Define.WorldObject.Pet, pet.Prefab);
                _itemprefab.transform.position = _itemprefabPos;
            }
            EquipItem?.Invoke(_item);
        }

    }

    public EquipableItem UnEquip()
    {
        if (_isEquip)
        {
            _item.UnEquipped();

            EquipableItem _ret = _item;
            _isEquip = false;
            _item = null;

            if (_ret is Pet)
            {
                if (_itemprefab != null)
                    Managers.Game.Despawn(_itemprefab);
                _itemprefab = null;
            }
            UnEquipItem?.Invoke(_ret);
            return _ret;
        }
        else
            return null;


    }

    public bool UnLock()
    {
        if (_IsLock == false)
            return true;

        if (IsUnLockable)
        {
            if (_category == "Buyable")
            {
                Managers.Game.SubstractCurrency(Define.CurrencyID.Ruby.ToString(), System.Numerics.BigInteger.Parse(UnitPrice.ToString()), Managers.Network.IS_ENABLE_NETWORK);
                _IsLock = false;
                OnUnlock?.Invoke();

                /*
                
                */
            }
            else if (_category == "Rating" || _category == "ReviveLevel")
            {
                _IsLock = false;
                OnUnlock?.Invoke();
            }
        }
        

        return !_IsLock;
        

        
    }

    public void UpdateUnlock()
    {
        if (category == "Rating" || category == "ReviveLevel")
            UnLock();
    }
    
    public void UpdateSlot()
    {
        if (_item != null)
            if (_item is Pet)
            {
                Pet pet = _item as Pet;
                _itemprefab = Managers.Game.Spawn(Define.WorldObject.Pet, pet.Prefab);
                _itemprefab.transform.position = _itemprefabPos;
            }
    }

    public EquipmentSlotSaveData ToSaveSlotData()
    {
        return new EquipmentSlotSaveData { index = _index, IsEquip = _isEquip, IsLock = _IsLock, ItemId = _item == null ? string.Empty : _item.ItemId };
    }
    public void Load(EquipmentSlotSaveData data)
    {
        if (_index == data.index)
        {
            _isEquip = data.IsEquip;
            _IsLock = data.IsLock;
            if (_isEquip )
            {
                _item = Managers.Game.GetInventory().Find(x => x.ItemId == data.ItemId) as EquipableItem;
                if (_item != null && _item.GetUsableCount() > 0)
                {
                    _item.Equipped();
                }
                else
                {
                    _isEquip = false;

                }
            }
        }
       

    }


}
