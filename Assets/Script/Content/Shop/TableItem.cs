using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TableItem 
{
    [SerializeField]
    BaseItem _item;
    public BaseItem GetItem => _item;

    [SerializeField]
    int _count;
    public int Count => _count;
}
