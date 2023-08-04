using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipItem : UI_Base 
{
    public delegate void SelectHandler(UI_EquipItem item);
    public event SelectHandler onSelect = null;

    public delegate void EquipHandler(UI_EquipItem item);
    public event EquipHandler OnEquip = null;

    public delegate void UpgradeHandler(UI_EquipItem item);
    public UpgradeHandler OnUpgrade;



    [SerializeField]
    EquipableItem _item;

    EquipmentSlot _slot;
    public EquipmentSlot Slot { set { _slot = value; } }

    readonly string EquipText = "Àå Âø";
    readonly string UnEquipText = "ÀåÂøÇØÁ¦";
    enum GameObjects
    {
        Selected,
        SelectedIcon
    }
    enum Texts
    {
        DisplayName,
        Count,
        Level,
        EquipButtonText,
        Description
    }

    enum Images
    {
        Icon,
        IconBackground,
        IconBackgroundSprite
    }
    enum Buttons
    {
        UpgradeButton ,
        EquipButton
    }
    public void Setup(EquipableItem item)
    {
        _item = item;
    }
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        if (_item != null && _slot != null)
        {
            Get<GameObject>((int)GameObjects.Selected).SetActive(false);
            
            GetText((int)Texts.DisplayName).text = _item.DisplayName;
            _item.DisplayNameOutLine.AddOutline(GetText((int)Texts.DisplayName).gameObject);

            string _itemDescription = "ÀåÂøÈ¿°ú : ";
            foreach(StatModifier _stat in _item.StatModifiers)
            {
                if (_stat.CodeName == "CriticalHitRate")
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", (_stat.Value / 10).ToString()) + System.Environment.NewLine;
                else
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", _stat.Value.ToString()) + System.Environment.NewLine;
            }
                
            
            //GetText((int)Texts.Description).text = $"ÀåÂøÈ¿°ú : " + _item.Description.Replace("\\n", System.Environment.NewLine);
            GetText((int)Texts.Description).text = _itemDescription;



            if (_item.ItemClass == "Bow")
                GetText((int)Texts.Level).text = $"{((Bow.Rank)_item.Level).ToString()} µî±Þ";
            if (_item.ItemClass == "Armor")
                GetText((int)Texts.Level).text = $"{((Armor.Rank)_item.Level).ToString()} µî±Þ";
            if (_item.ItemClass == "Helmet")
                GetText((int)Texts.Level).text = $"{((Helmet.Rank)_item.Level).ToString()} µî±Þ";
            if (_item.ItemClass == "Cloak")
                GetText((int)Texts.Level).text = $"{((Cloak.Rank)_item.Level).ToString()} µî±Þ";

            gameObject.GetComponent<Image>().sprite = _item.DescriptionBackgroundTexture;
            gameObject.GetComponent<Image>().color = _item.DescriptionBackgroundColor;
        
            _item.DescriptionBackgroundOutline.AddOutline(gameObject);

            GetImage((int)Images.Icon).sprite = _item.Icon;
            GetImage((int)Images.IconBackground).sprite = _item.IconBackground;
            GetImage((int)Images.IconBackgroundSprite).sprite = _item.DescriptionBackground;

            _item.IconBackgroundOutline.AddOutline(GetImage((int)Images.IconBackground).gameObject);



            UpdateSlot();



            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            //UpdateCountText();

            AddUIEvent(GetButton((int)Buttons.EquipButton).gameObject, (data) => {

                EquipableItem _getItem = Managers.Game.GetInventory().Find(x=>x.ItemId == _item.ItemId) as EquipableItem;
                if (_getItem == null)
                    return;

                if (_slot.IsEquip )
                {
                    if (_slot.GetItem.ItemId == _item.ItemId)
                    {
                        _slot.UnEquip();
                    }
                    else
                    {
                        _slot.UnEquip();
                        _slot.Equip(_getItem);
                    }
                }
                else
                    _slot.Equip(_getItem);

                UpdateSlot();
                OnEquip?.Invoke(this);
            });

            AddUIEvent(GetButton((int)Buttons.UpgradeButton).gameObject, (data) => {

                if (Managers.Item.UpgradeOneItem(_item, false))
                    OnUpgrade?.Invoke(this);
                UpdateCountText();



            });


        }

    }

    public void UpdateSlot()
    {
        UpdateCountText();
        UpdateEquipText();
        UpdateEquipIcon();
    }
   
    
    private void Start()
    {
        Init();
    }
    public void OnDestroy()
    {
        OnUpgrade = null;
        OnEquip = null;
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
    }

    public void UpdateCountText()
    {
        EquipableItem _GetItem = Managers.Game.GetInventory().GetItem(_item.ItemId) as EquipableItem;
        if(_GetItem == null)
            GetText((int)Texts.Count).text = $"º¸À¯ : {0}";
        else
            GetText((int)Texts.Count).text = $"º¸À¯ : {_GetItem.GetUsableCount()}";
    }

    public void UpdateEquipText()
    {
        if (GetText((int)Texts.EquipButtonText) == null)
            return;

        if(_slot.IsEquip)
        {
            if (_slot.GetItem.ItemId == _item.ItemId)
                GetText((int)Texts.EquipButtonText).text = UnEquipText;
            else
                GetText((int)Texts.EquipButtonText).text = EquipText;
        }
        else
        {
            GetText((int)Texts.EquipButtonText).text = EquipText;
        }
        
    }

    public void UpdateEquipIcon()
    {
        if (Get<GameObject>((int)GameObjects.SelectedIcon) == null)
            return;

        if (_slot.IsEquip)
        {
            if(_slot.GetItem.ItemId == _item.ItemId)
                Get<GameObject>((int)GameObjects.SelectedIcon).SetActive(true);
            else
                Get<GameObject>((int)GameObjects.SelectedIcon).SetActive(false);
        }
        else
        {
            Get<GameObject>((int)GameObjects.SelectedIcon).SetActive(false);
        }

       
    }

}
