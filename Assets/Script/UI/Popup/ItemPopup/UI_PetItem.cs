using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetItem : UI_Base
{
    Pet _pet;
    public Pet Pet { set { _pet = value; } }

    UI_PetItemData _itemData;

    

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
    Image EquipBackground;

    [SerializeField]
    Image DescriptionBackground;

    [SerializeField]
    Text ItemCountText;

    [SerializeField]
    Text DescriptionText;

    [SerializeField]
    UI_BaseItem UI_ItemPrefab;

    [SerializeField]
    Button EquipButton;

    [SerializeField]
    Button OneUpgradeButton;

    [SerializeField]
    Button AllUpgradeButton;

    public void Setup(UI_PetItemData itemData)
    {
        Pet FindPet = Managers.Item.Database.ItemList.Where(x => x.ItemId == itemData.ItemId).FirstOrDefault() as Pet;
        if(FindPet != null)
        {
            _itemData = itemData;

            EquipBackground.sprite = _itemData.EquipBackgroundSprite;
            DescriptionBackground.sprite = _itemData.DescriptionBackgroundSprite;

            string _itemDescription = "";
            foreach (StatModifier _stat in FindPet.StatModifiers)
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
            DescriptionText.text = _itemDescription;
            if (FindPet.type == Pet.Type.Drop)
                DescriptionText.fontSize = 20;

            UI_ItemPrefab.Item = FindPet;

            UpdateCountText();

            EquipButton.onClick.RemoveAllListeners();
            EquipButton.onClick.AddListener(() => {
                if (Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId) != null)
                    OnEquipPet?.Invoke(Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId) as Pet);
                UpdateCountText();
            });

            OneUpgradeButton.onClick.RemoveAllListeners();
            OneUpgradeButton.onClick.AddListener(() => {
                if (Managers.Item.UpgradeOneItem(Managers.Item.Database.ItemList.Where(x => x.ItemId == _itemData.ItemId).FirstOrDefault(), true) == true)
                {
                    OnUprade?.Invoke();

                }
                UpdateCountText();
            });

            AllUpgradeButton.onClick.RemoveAllListeners();
            AllUpgradeButton.onClick.AddListener(() => {
                if (Managers.Item.UpgradeOneItem(Managers.Item.Database.ItemList.Where(x => x.ItemId == _itemData.ItemId).FirstOrDefault()) == true)
                {
                    OnUprade?.Invoke();

                }
                UpdateCountText();
            });

            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;
        }
        
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        if (_pet != null)
        {
           

        }

    }

    public void UpdateCountText()
    {
        
        ItemCountText.text = $"º¸À¯ °¹¼ö : {(Managers.Game.GetInventory().Find(x=>x.ItemId == _itemData.ItemId) == null ? 0 : Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId).GetUsableCount())} / 3";
    }

   

    

    public void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
        OnUprade = null;
        OnEquipPet = null;
    }
}
