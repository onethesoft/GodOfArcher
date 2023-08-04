using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Armor")]
public class ArmorReward : Reward
{
    [SerializeField]
    Armor _armor;

    public override string GetId()
    {
        return _armor.ItemId;
    }

    public override void Give(Quest quest)
    {
        List<BaseItem> _listArmor = Managers.Item.GrantItemToUser(_armor.ItemId);
        UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
        ui_randombox.Setup(_listArmor);
    }
}
