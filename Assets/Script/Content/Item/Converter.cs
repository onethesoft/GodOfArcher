using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Converter : MonoBehaviour
{
    UI_Convert.ItemType ItemType;

    [HideInInspector]
    public string ConsumeItem;

    [HideInInspector]
    public string TargetItem;

    [SerializeField]
    int _requireCount = 2;

    public int RequireCount => _requireCount;

    private void Start()
    {
        ItemType = GetComponent<UI_Convert>().ConvertItemType;
        ConsumeItem = string.Empty;
        TargetItem = string.Empty;

    }
    public int GetMaxConvertCount()
    {
        if (string.IsNullOrEmpty(ConsumeItem))
            return 0;
        if (Managers.Game.GetInventory().IsFindItem(ConsumeItem) == false)
            return 0;

        return Managers.Game.GetInventory().Find(x => x.ItemId == ConsumeItem).GetUsableCount().GetValueOrDefault() / _requireCount;

    }
    public bool IsConvertable(int Count)
    {
        if (string.IsNullOrEmpty(ConsumeItem) || string.IsNullOrEmpty(TargetItem))
            return false;
        if (Managers.Game.GetInventory().IsFindItem(ConsumeItem) == false)
            return false;
        if (Managers.Game.GetInventory().Find(x => x.ItemId == ConsumeItem).GetUsableCount() < Count * _requireCount)
            return false;

        if (ItemType == UI_Convert.ItemType.Rune)
        {
            Rune _consume = Managers.Item.Database.ItemList.Where(x => x.ItemId == ConsumeItem).FirstOrDefault() as Rune;
            Rune _target = Managers.Item.Database.ItemList.Where(x => x.ItemId == TargetItem).FirstOrDefault() as Rune;

            if (_consume == null || _target == null)
                return false;
            if (_consume.Level != _target.Level)
                return false;

        }
        else
        {
            Pet _consume = Managers.Item.Database.ItemList.Where(x => x.ItemId == ConsumeItem).FirstOrDefault() as Pet;
            Pet _target = Managers.Item.Database.ItemList.Where(x => x.ItemId == TargetItem).FirstOrDefault() as Pet;

            if (_consume == null || _target == null)
                return false;
            if (_consume.Level != _target.Level)
                return false;
        }

        return true;
    }
    public void Convert(int Count)
    {
        if (!IsConvertable(Count))
            return;
        

        Managers.Game.GetInventory().ConsumeItem(ConsumeItem, Count * _requireCount);
        Managers.Game.GetInventory().AddItem(TargetItem, Count);

        List<ModifyItem> _modify = new List<ModifyItem>();
        _modify.Add(new ModifyItem { ItemId = ConsumeItem, UsesToadds = -Count * _requireCount });
        _modify.Add(new ModifyItem { ItemId = TargetItem, UsesToadds = Count });

        Managers.Network.UpgradeItems(_modify);
    }

  
}
