using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Bow")]
public class BowReward : Reward
{
    [SerializeField]
    Bow _bow;
    public override string GetId()
    {
        return _bow.ItemId;
    }

    public override void Give(Quest quest)
    {
        List<BaseItem> _listBow = Managers.Item.GrantItemToUser(_bow.ItemId);
        UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
        ui_randombox.Setup(_listBow);
    }
}
