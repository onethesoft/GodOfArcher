using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/테이블", fileName = "테이블")]
public class Table : BaseItem
{
    [SerializeField]
    List<TableItem> _items;

   

    public IReadOnlyList<TableItem> Items => _items;

    public int GetTotalCount
    {
        get
        {
            int total = 0;
            _items.ForEach(x => total += x.Count );
            return total;
        }
    }
   

    public string EvaluteTable()
    {
        List<TableItem> _copy = _items.OrderByDescending(x => x.Count).ToList();
        int RandomNum = Random.Range(0, GetTotalCount);
        int iteCount = 0;
        TableItem getItem = _copy.First();
        foreach(TableItem item in _items)
        {
            getItem = item;
            iteCount += item.Count;
            if(RandomNum < iteCount)
                break;
        }

        if(getItem.GetItem is Table)
        {
            Table _ret = getItem.GetItem as Table;
            return _ret.EvaluteTable();
        }
        else
        {
            return getItem.GetItem.ItemId;
        }
    }
}
