using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetItem : UI_Base
{
    Pet _pet;
    public Pet Pet { set { _pet = value; } }

    public delegate void EquipHandler(Pet pet);
    public EquipHandler OnEquipPet;

    public delegate void UpgradeHandler();
    public UpgradeHandler OnUprade;

    enum Images
    {
        Background ,
        IconBackground ,
        Icon ,
        EquipBackground ,
        DescriptionBackground
    }
    enum Texts
    {
        DisplayName,
        CountText,
        Description
    }
    enum Buttons
    {
        EquipButton,
        OneUpgradeButton,
        AllUpgradeButton
    }
    [SerializeField]
    UI_ButtonBlocker _oneUpgradeButtonBlocker;

    [SerializeField]
    UI_ButtonBlocker _allUpgradeButtonBlocker;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        if (_pet != null)
        {
            
            GetImage((int)Images.Background).sprite = _pet.Background;
            GetImage((int)Images.DescriptionBackground).sprite = _pet.DescriptionBackground;
            GetImage((int)Images.EquipBackground).sprite = _pet.DescriptionBackground;

            string _itemDescription = "";
            foreach (StatModifier _stat in _pet.StatModifiers)
            {
                if (_stat.CodeName == "CriticalHitRate")
                {
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", (_stat.Value / 10).ToString()) + System.Environment.NewLine;
                }
                else
                {
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", _stat.Value.ToString()) + System.Environment.NewLine;
                }
            }
            

            GetText((int)Texts.Description).text = _itemDescription;

            if (_pet.type == Pet.Type.Drop)
                GetText((int)Texts.Description).fontSize = 20;

            GetImage((int)Images.Icon).sprite = _pet.Icon;
            GetImage((int)Images.IconBackground).sprite = _pet.IconBackground;

            if (_pet.Level > 6)
            {
                GetImage((int)Images.Icon).gameObject.SetActive(false);
            }

            GetText((int)Texts.DisplayName).text = _pet.DisplayName;
            GetText((int)Texts.CountText).text = $"º¸À¯ °¹¼ö : {(Managers.Game.GetInventory().Find(_pet) == null ? 0 : Managers.Game.GetInventory().Find(_pet).GetUsableCount())} / 3";

            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            AddUIEvent(GetButton((int)Buttons.EquipButton).gameObject, (data) => {
                if (Managers.Game.GetInventory().Find(_pet) != null)
                    OnEquipPet?.Invoke(Managers.Game.GetInventory().Find(_pet) as Pet);
                UpdateCountText();
            });
            AddUIEvent(GetButton((int)Buttons.OneUpgradeButton).gameObject, (data) => {
                if (Managers.Item.UpgradeOneItem(_pet, true) == true)
                {
                    OnUprade?.Invoke();
                    _oneUpgradeButtonBlocker.StartBlocker();
                }
                UpdateCountText();


            });

            AddUIEvent(GetButton((int)Buttons.AllUpgradeButton).gameObject, (data) => {
                if (Managers.Item.UpgradeOneItem(_pet) == true)
                {
                    OnUprade?.Invoke();
                    _allUpgradeButtonBlocker.StartBlocker();
                }
                UpdateCountText();
            });

        }

    }

    public void UpdateCountText()
    {
        //Debug.Log(Managers.Game.GetInventory().Find(_rune) != null ? Managers.Game.GetInventory().Find(_rune).RemainingUses : -1);
        // Debug.Log($"{_rune.ItemId} : {(Managers.Game.GetInventory().Find(x => x.ItemId == _rune.ItemId) == null ? -1 : (Managers.Game.GetInventory().Find(_rune) as EquipableItem).EquipCount)}");
        //Debug.Log($"{_pet.ItemId} : {(Managers.Game.GetInventory().Find(x => x.ItemId == _pet.ItemId) == null ? -1 : (Managers.Game.GetInventory().Find(_pet) as EquipableItem).RemainingUses)}");
        GetText((int)Texts.CountText).text = $"º¸À¯ °¹¼ö : {(Managers.Game.GetInventory().Find(_pet) == null ? 0 : Managers.Game.GetInventory().Find(_pet).GetUsableCount())} / 3";
    }

    public void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
        OnUprade = null;
    }
}
