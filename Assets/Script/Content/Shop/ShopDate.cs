using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopButtonData
{
    public string ButtonText;
    public Sprite Icon;
    public BaseItem PurchaseItem;
    public string Currency;
    public int Price;
}

[CreateAssetMenu(menuName = "Shop/ShopData", fileName = "ShopData_")]
public class ShopDate : ScriptableObject
{
    [SerializeField]
    string _displayName;
    public string DisplayName => _displayName;

    [SerializeField]
    string _itemClass;
    public string ItemClass => _itemClass;

    

    [SerializeField]
    string _codeName;
    public string CodeName => _codeName;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    List<ShopButtonData> _btnDatas;
    public IReadOnlyList<ShopButtonData> BtnLists => _btnDatas;

   

    
    
    
    
   

}
