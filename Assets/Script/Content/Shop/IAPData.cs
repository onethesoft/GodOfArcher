using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/IAPData", fileName = "IAPData_")]
public class IAPData : ScriptableObject
{
    public enum Type
    {
        Package,
        Ruby
    }
    [SerializeField]
    Type _type;

    public Type type => _type;

    [SerializeField]
    string _displayName;
    public string DisplayName => _displayName;

    [SerializeField]
    Sprite _background;
    public Sprite Background => _background;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    Sprite _subicon;
    public Sprite SubIcon => _subicon;

    [SerializeField]
    List<BaseItem> _itemIcon;
    public IReadOnlyList<BaseItem> ItemIcons => _itemIcon;




    [SerializeField]
    Sprite _backcurrencyIcon;
    public Sprite BackgroundCurrencyIcon => _backcurrencyIcon;

    [SerializeField]
    Sprite _currencyIcon;
    public Sprite CurrencyIcon => _currencyIcon;


    [SerializeField]
    Bundle _item;
    public Bundle Item => _item;


    [SerializeField]
    string _description;
    public string Description => _description;


    [SerializeField]
    Color _descriptionColor;
    public Color DescriptionColor => _descriptionColor;

    [SerializeField]
    string _PriceText;
    public string Price => _PriceText;

    [SerializeField]
    bool _isBlock;
    public bool IsBlock => _isBlock;


}
