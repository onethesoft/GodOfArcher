using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="아이템/던전키")]
public class DongeonKey : BaseItem
{
    [SerializeField]
    Sprite _icon;

    public Sprite Icon => _icon;

    
}
