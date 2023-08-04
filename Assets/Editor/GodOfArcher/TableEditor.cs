using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;



[CustomEditor(typeof(Table))]
public class TableEditor : Editor
{
    Table _table;
    public void OnEnable()
    {
        _table = (Table)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_table.Items == null)
            return;

        List<TableItem> _sorted = _table.Items.OrderByDescending(x => x.Count).ToList();


        decimal total = (decimal)_table.GetTotalCount; 
        foreach(TableItem item in _sorted)
        {
            if (item.Count <= 0)
                continue;

            decimal getCount = (decimal)item.Count;
            getCount /= total;
            EditorGUILayout.LabelField(item.GetItem.DisplayName, getCount.ToString());
            
            
        }

        
    }
}

#endif
