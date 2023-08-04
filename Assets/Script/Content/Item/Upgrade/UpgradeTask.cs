using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ConsumeItem
{
    public BaseItem Consume;

    public int ConsumeCount;

    public ModifyItem ToModifyItem()
    {
        return new ModifyItem { ItemId = Consume.ItemId, UsesToadds = -ConsumeCount  };
    }
}

[CreateAssetMenu(menuName = "아이템/UpgradeTask", fileName = "UpgradeTask")]
public class UpgradeTask : ScriptableObject
{
    public delegate void OnUpgradeEventHandler(List<BaseItem> counsumeItems, BaseItem grantItem);
    public OnUpgradeEventHandler OnUpgradeResult = null;

    public delegate void OnUpgradeEvent(List<ModifyItem> ModifyItems);
    public OnUpgradeEvent OnChangeItems = null;

    [SerializeField]
    protected List<ConsumeItem> _consumeItems;

    [SerializeField]
    protected BaseItem _targetItem;

    [SerializeField]
    bool _isAuto ;
    public bool IsAuto => _isAuto;

    public IReadOnlyList<ConsumeItem> Materials => _consumeItems;
    public BaseItem TargetItem => _targetItem;
    public int GetMaxUpgradeCount(List<BaseItem> Lists)
    {
        if (!_consumeItems.All(x => Lists.Find(y=>y.ItemId == x.Consume.ItemId) != null))
            return 0;

        int minCount = System.Int32.MaxValue;
        foreach (ConsumeItem consumeItem in _consumeItems)
        {
            BaseItem findConsume = Lists.Find(x => x.ItemId == consumeItem.Consume.ItemId);
            if (findConsume.GetUsableCount() < consumeItem.ConsumeCount)
                return 0;

            if (findConsume.GetUsableCount() / consumeItem.ConsumeCount < minCount)
                minCount = (int)findConsume.GetUsableCount() / consumeItem.ConsumeCount;
        }

        return minCount;

    }

    public List<BaseItem> GetConsumeItem()
    {
        return _consumeItems.Select(x=>x.Consume).ToList();
    }
    public virtual bool Upgrade(Inventory inventory, bool once = true)
    {

        int UpgradeCount = GetMaxUpgradeCount(inventory.ToList());
        if (UpgradeCount == 0)
            return false;


        List<BaseItem> _consumes = new List<BaseItem>();
        BaseItem _grant;

        List<ModifyItem> _itemChanges = new List<ModifyItem>();


        foreach (ConsumeItem consume in _consumeItems)
        {
            BaseItem _consume = consume.Consume.Clone();
            if (once)
            {
                inventory.ConsumeItem(consume.Consume.ItemId, consume.ConsumeCount);
                _consumes.Add(_consume);
            }
            else
            {
                inventory.ConsumeItem(consume.Consume.ItemId, UpgradeCount * consume.ConsumeCount);
                _consume.Add(UpgradeCount * consume.ConsumeCount - 1);
            }


        }

        if (once)
        {
            // 서버에 저장되는 정보 연산
            foreach (ConsumeItem item in _consumeItems)
            {
                ModifyItem _info = item.ToModifyItem();
                _info.CalculateCallsAPICount(inventory, item.Consume.ItemId);
                _itemChanges.Add(_info);
            }

            ModifyItem _targetinfo = new ModifyItem { ItemId = _targetItem.ItemId, UsesToadds = 1 };
            _targetinfo.CalculateCallsAPICount(inventory, _targetItem.ItemId);
            _itemChanges.Add(_targetinfo);

            // 업그레이드 결과 인벤에 반영
            _grant = _targetItem.Clone();
            inventory.AddItem(_grant.ItemId, 1);

        }
        else
        {
            
            foreach (ConsumeItem item in _consumeItems)
            {
                ModifyItem _cousume = item.ToModifyItem();
                _cousume.UsesToadds *= UpgradeCount;
                _cousume.CalculateCallsAPICount(inventory, item.Consume.ItemId);
                _itemChanges.Add(_cousume);
            }

            ModifyItem _targetinfo = new ModifyItem { ItemId = _targetItem.ItemId, UsesToadds = UpgradeCount };
            _targetinfo.CalculateCallsAPICount(inventory, _targetItem.ItemId);
            _itemChanges.Add(_targetinfo);

            _grant = _targetItem.Clone();
            _grant.Add(UpgradeCount - 1);
            inventory.AddItem(_grant.ItemId, UpgradeCount);

        }


        //Debug.Log(_itemChanges[0].ItemId);
        OnUpgradeResult?.Invoke(_consumes, _grant);
        OnChangeItems?.Invoke(_itemChanges);

        return true;
    }



}
