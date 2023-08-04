using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Cloak")]
public class CloakReward : Reward
{
    [SerializeField]
    Cloak _cloak;
    public override string GetId()
    {
        return _cloak.ItemId;
    }

    public override void Give(Quest quest)
    {
        List<BaseItem> _listCloak = Managers.Item.GrantItemToUser(_cloak.ItemId);
        UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
        ui_randombox.Setup(_listCloak);
    }
}
