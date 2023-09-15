using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_RuneItem : UI_Base
{
    Rune _rune;
    public Rune Rune { set { _rune = value; } }

    public delegate void EquipHandler(Rune rune);
    public EquipHandler OnEquipRune;

    public delegate void UpgradeHandler();
    public UpgradeHandler OnUprade;

    UI_RuneItemData _itemData;
    enum Images
    {
        Background,
        EquipBackground ,
        DescriptionBackground,
        Icon , 
        IconBackground
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

    enum Blocker
    {
        OneUpgradeButtonBlocker,
        AllUpgradeButtonBlocker
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

    UI_RunePopup _parent;

    public void Setup(UI_RuneItemData itemData)
    {
        Rune FindRune = Managers.Item.Database.ItemList.Where(x => x.ItemId == itemData.ItemId).FirstOrDefault() as Rune;
        if (FindRune != null)
        {
            _itemData = itemData;

            EquipBackground.sprite = _itemData.EquipBackgroundSprite;
            DescriptionBackground.sprite = _itemData.DescriptionBackgroundSprite;

            string _itemDescription = "";
            foreach (StatModifier _stat in FindRune.StatModifiers)
                _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", _stat.Value.ToString()) + System.Environment.NewLine;
           
            DescriptionText.text = _itemDescription;


            UI_ItemPrefab.Item = FindRune;

            UpdateCountText();

            EquipButton.onClick.RemoveAllListeners();
            EquipButton.onClick.AddListener(() => {
                if (Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId) != null)
                    OnEquipRune?.Invoke(Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId) as Rune);
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
        Bind<GameObject>(typeof(Blocker));


    }

    public void UpdateCountText()
    {
          ItemCountText.text = $"º¸À¯ °¹¼ö : {(Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId) == null ? 0 : Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId).GetUsableCount())} / 3";

    }
    
    public void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
        OnEquipRune = null;
        OnUprade = null;
    }

    
}
