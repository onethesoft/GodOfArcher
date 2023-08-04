using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Rune")]
public class RuneReward : Reward
{
    [SerializeField]
    Rune _rune;
    public override string GetId()
    {
        return _rune.ItemId;
    }

    public override void Give(Quest quest)
    {
        List<BaseItem> _listRune = Managers.Item.GrantItemToUser(_rune.ItemId);
        UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
        ui_randombox.Setup(_listRune);
    }

    
}
