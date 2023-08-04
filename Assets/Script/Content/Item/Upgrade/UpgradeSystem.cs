using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(menuName = "아이템/업그레이드테이블/아이템", fileName = "UpgradeTable")]
public class UpgradeSystem : ScriptableObject
{
    public delegate void ItemUpgradeHandler(List < BaseItem > consumes, BaseItem target);
    public event ItemUpgradeHandler OnItemUpgraded;
    [SerializeField]
    protected List<UpgradeTask> _tasks;

    List<BaseItem> _consumeItems = new List<BaseItem>();
    List<BaseItem> _grantItems = new List<BaseItem>();
    List<ModifyItem> _updateItems = new List<ModifyItem>();



    public void OnEnable()
    {
        if (_tasks == null) return;

        foreach (UpgradeTask task in _tasks)
        {
            task.OnUpgradeResult -= OnUpgradeResult;
            task.OnUpgradeResult += OnUpgradeResult;
            task.OnChangeItems -= OnUpgraded;
            task.OnChangeItems += OnUpgraded;
        }
    }
    
    public List<BaseItem> GetConsumeItem(BaseItem targetItem)
    {
        UpgradeTask _findTask = _tasks.Find(x => x.TargetItem.ItemId == targetItem.ItemId);
        if (_findTask == null)
            return null;
        else
            return _findTask.GetConsumeItem();
    }
    public bool UpgradeItem(Inventory inven, string targetId, bool once = false)
    {
       
        UpgradeTask find = _tasks.Select(x => x).Where(x => x.TargetItem.ItemId == targetId).First();
        if (find == null) return false;

        bool Success = false;

        _consumeItems.Clear();
        _grantItems.Clear();
        _updateItems.Clear();

        Success = find.Upgrade(inven , once);


        UpdateUpgraded();

        return Success;


    }

    public bool UpgradeAllItems(Inventory inven)
    {
        bool Success = false;

        _consumeItems.Clear();
        _grantItems.Clear();
        _updateItems.Clear();

        foreach (UpgradeTask task in _tasks)
        {
            if(task.IsAuto)
                Success |= task.Upgrade(inven, false);
        }

        UpdateUpgraded();

        return Success;
    }

    public void OnUpgradeResult(List<BaseItem> consumes , BaseItem target)
    {
        foreach(BaseItem consume in consumes)
        {
            if(_consumeItems.Find(x=>x.ItemId == consume.ItemId) != null)
                _consumeItems.Find(x => x.ItemId == consume.ItemId).Add(consume.RemainingUses.Value);
            else
                _consumeItems.Add(consume);
        }

        if (_grantItems.Find(x => x.ItemId == target.ItemId) != null)
            _grantItems.Find(x => x.ItemId == target.ItemId).Add(target.RemainingUses.Value);
        else
            _grantItems.Add(target);

        OnItemUpgraded?.Invoke(consumes, target);
    }

    public void OnUpgraded(List<ModifyItem> items)
    {
        _updateItems.AddRange(items);
    }

    public void UpdateUpgraded()
    {
        if (Managers.Network.IS_ENABLE_NETWORK == false)
            return;

        Debug.Log("Item UpdateUpgraded");
        List<ModifyItem> _itemList = new List<ModifyItem>();
        foreach(ModifyItem changeItem in _updateItems)
        {
            ModifyItem _find = _itemList.Where(x => x.ItemId == changeItem.ItemId).FirstOrDefault();
            if (_find != null)
                _find.UsesToadds += changeItem.UsesToadds;
            else
                _itemList.Add(changeItem);
            
        }

        _itemList.RemoveAll(x => x.UsesToadds == 0);
        if (_itemList.Count > 0)
        {
            int CallsAPI = _itemList.Select(x => x.CallsAPICount).Sum();
            if(CallsAPI <= Managers.Network.MaxCallsAPICount)
                Managers.Network.UpgradeItems(_itemList);
            else
            {
                Debug.Log("Item UpdateUpgraded MaxCallsAPICount Exceed"  );
                // 1.Devide Chunk for list // count 23
                // 2. calls networkmanetet.upgradeitems loop
                // 3.
                int CallAPICountSum = 0;
                int index = 0;
                Dictionary<int, List<ModifyItem>> _queue = new Dictionary<int, List<ModifyItem>>();
                foreach(ModifyItem it in _itemList)
                {
                    if(CallAPICountSum > Managers.Network.MaxCallsAPICount - 2)
                    {
                        index++;
                        CallAPICountSum = 0;
                    }

                    if (_queue.ContainsKey(index) == false)
                        _queue.Add(index, new List<ModifyItem>());

                    _queue[index].Add(it);
                    CallAPICountSum += it.CallsAPICount;

                }

                foreach(KeyValuePair<int , List<ModifyItem>> pair in _queue)
                    Managers.Network.UpgradeItems(pair.Value);
                



            }
        }
        _itemList.Clear();
        _updateItems.Clear();
    }

    public virtual string GetNextLevelItem(BaseItem item)
    {
        if(item.ItemClass == "Bow")
        {
            Bow _consumeitem = item as Bow;
            UpgradeTask _findTask;

            _findTask = _tasks.Find(x => ((x.TargetItem as Bow).Level == (_consumeitem.Level + 1)));

            if (_findTask == null)
                return null;
            else
                return _findTask.TargetItem.ItemId;
        }
        else if(item.ItemClass == "Helmet")
        {
            Helmet _consumeitem = item as Helmet;
            UpgradeTask _findTask;

            _findTask = _tasks.Find(x => ((x.TargetItem as Helmet).Level == (_consumeitem.Level + 1)));

            if (_findTask == null)
                return null;
            else
                return _findTask.TargetItem.ItemId;
        }
        else if (item.ItemClass == "Armor")
        {
            Armor _consumeitem = item as Armor;
            UpgradeTask _findTask;

            _findTask = _tasks.Find(x => ((x.TargetItem as Armor).Level == (_consumeitem.Level + 1)));

            if (_findTask == null)
                return null;
            else
                return _findTask.TargetItem.ItemId;
        }
        else if (item.ItemClass == "Cloak")
        {
            Cloak _consumeitem = item as Cloak;
            UpgradeTask _findTask;

            _findTask = _tasks.Find(x => ((x.TargetItem as Cloak).Level == (_consumeitem.Level + 1)));

            if (_findTask == null)
                return null;
            else
                return _findTask.TargetItem.ItemId;
        }
        else if(item.ItemClass == "Rune")
        {
            Rune _consumeRune = item as Rune;
            UpgradeTask _findTask;
            if (_consumeRune.Level < (int)Rune.Rank.S)
                _findTask = _tasks.Find(x => ((x.TargetItem as Rune).Level == (_consumeRune.Level + 1)) && ((x.TargetItem as Rune).type == (_consumeRune.type)));
            else
                _findTask = _tasks.Find(x => ((x.TargetItem as Rune).Level == (_consumeRune.Level + 1)) && ((x.TargetItem as Rune).type == Rune.Type.All));

            if (_findTask == null)
                return null;
            else
                return _findTask.TargetItem.ItemId;
        }
        else if(item.ItemClass == "Pet")
        {
            Pet _consumePet = item as Pet;
            UpgradeTask _findTask;
            if (_consumePet.Level < (int)Pet.Rank.S)
                _findTask = _tasks.Find(x => ((x.TargetItem as Pet).Level == (_consumePet.Level + 1)) && ((x.TargetItem as Pet).type == (_consumePet.type)));
            else
                _findTask = _tasks.Find(x => ((x.TargetItem as Pet).Level == (_consumePet.Level + 1)) && ((x.TargetItem as Pet).type == Pet.Type.All));

            if (_findTask == null)
                return null;
            else
                return _findTask.TargetItem.ItemId;
        }
        else if (item.ItemClass == "Heart" || item.ItemClass == "Essence")
        {
            Artifact _artifact = item as Artifact;
            if (_artifact.Level < _artifact.MaxLevel)
                return item.ItemId;
        }
       
        return null;

    }
    



}
