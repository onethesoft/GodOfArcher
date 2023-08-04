using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Pet")]
public class PetReward : Reward
{
    [SerializeField]
    Pet _pet;
    public override string GetId()
    {
        return _pet.ItemId;
    }

    public override void Give(Quest quest)
    {
        List<BaseItem> _listPet = Managers.Item.GrantItemToUser(_pet.ItemId);
        UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
        ui_randombox.Setup(_listPet);
    }
}
