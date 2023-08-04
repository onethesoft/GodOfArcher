using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "¾ÆÀÌÅÛ/ÀåÂø/BuyablePetCondition", fileName = "Pet_")]
public class BuyablePetUnlockable : UnlockableCondition
{
    [SerializeField]
    int _Price;

    [SerializeField]
    Define.CurrencyID _currency;

    public override bool IsUnlockable(EquipmentSlot target, EquipmentSystem system)
    {
       

        if (system.Owner.Currency[_currency.ToString()].Amount >= _Price)
        {
                return true;

        }

        return false;


    }
}
