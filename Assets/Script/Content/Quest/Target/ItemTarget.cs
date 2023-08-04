using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/ShopData", fileName = "Target_")]
public class ItemTarget : TaskTarget
{
    [SerializeField]
    ShopDate _purchaseItemData;

    public override object Value => _purchaseItemData;

    public override bool IsEqual(object target)
    {
        ShopDate _target = target as ShopDate;
        if (_target == null)
            return false;

        return _target.CodeName == _purchaseItemData.CodeName;
    }

    
}
