using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/ArtifactUpgradeTask", fileName = "UpgradeTask")]
public class ArtifactUpgradeTask : UpgradeTask
{
    

    public override bool Upgrade(Inventory inventory, bool once = true)
    {
        int UpgradeCount = GetMaxUpgradeCount(inventory.ToList());
        if (UpgradeCount == 0)
            return false;

        Artifact _target = inventory.Find(_targetItem) as Artifact;
        if (_target != null)
        {
            if (_target.Level >= _target.MaxLevel)
                return false;

            if (_target.Level + UpgradeCount > _target.MaxLevel)
                UpgradeCount = _target.MaxLevel - _target.Level;


        }
        else
        {
            
            if(UpgradeCount > (_targetItem as Artifact).MaxLevel)
                UpgradeCount = (_targetItem as Artifact).MaxLevel;
            
        }

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

            foreach (ConsumeItem item in _consumeItems)
            {
                ModifyItem _info = item.ToModifyItem();
                _info.CalculateCallsAPICount(inventory, item.Consume.ItemId);
                _itemChanges.Add(_info);
            }

            ModifyItem _targetinfo = new ModifyItem { ItemId = _targetItem.ItemId, UsesToadds = 1 };
            _targetinfo.CalculateCallsAPICount(inventory, _targetItem.ItemId);
            _itemChanges.Add(_targetinfo);
          

            Artifact _getTarget = _targetItem as Artifact; 
            _grant = _getTarget.Clone();
            //_grant.Add(1);
            inventory.AddItem(_grant.ItemId , 1);


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

        OnUpgradeResult?.Invoke(_consumes, _grant);
        OnChangeItems?.Invoke(_itemChanges);

        return true;
    }
}
