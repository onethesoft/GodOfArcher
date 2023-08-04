using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Helmet")]
public class HelmetReward : Reward
{
    [SerializeField]
    Helmet _helmet;
    public override string GetId()
    {
        return _helmet.ItemId;
    }

    public override void Give(Quest quest)
    {
        List<BaseItem> _listHelmet = Managers.Item.GrantItemToUser(_helmet.ItemId);
        UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
        ui_randombox.Setup(_listHelmet);
    }
}
