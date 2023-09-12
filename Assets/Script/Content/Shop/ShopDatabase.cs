using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/ShopDatabase")]
public class ShopDatabase : ScriptableObject
{
    [SerializeField]
    List<ShopDate> _randomBoxLists;
    public IReadOnlyList<ShopDate> RandomBoxItems => _randomBoxLists;

    [SerializeField]
    List<IAPData> _iapLists;
    public IReadOnlyList<IAPData> IAPItems => _iapLists;


    [SerializeField]
    List<IAPData> _rubyLists;
    public IReadOnlyList<IAPData> RubyPackageItems => _rubyLists;

    [SerializeField]
    IAPData _seasonpass;
    public IAPData Seasonpass => _seasonpass;

    [SerializeField]
    IAPData _seasonpass2;
    public IAPData Seasonpass2 => _seasonpass2;

    [SerializeField]
    IAPData _seasonpass3;
    public IAPData Seasonpass3 => _seasonpass3;
}
