using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipItem : UI_Base, ICell
{
    public delegate void SelectHandler(UI_EquipItem item);
    public event SelectHandler onSelect = null;

    public delegate void EquipHandler(UI_EquipItem item);
    public event EquipHandler OnEquip = null;

    public delegate void UpgradeHandler(UI_EquipItem item);
    public UpgradeHandler OnUpgrade;




    EquipableItem _item;

    UI_EquipItemData _itemData;

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

    [SerializeField]
    private Text DisplayName;

    [SerializeField]
    private Text Level;

    [SerializeField]
    private Text Count;

    [SerializeField]
    private Text EquipButtonText;

    [SerializeField]
    private Text Description;


    [SerializeField]
    private Image Icon;

    [SerializeField]
    private Image IconBackground;

    [SerializeField]
    private Image IconBackgroundSprite;

    [SerializeField]
    private Button UpgradeButton;

    [SerializeField]
    private Button EquipButton;

    [SerializeField]
    private GameObject SelectedIcon;


    public int GetLevel()
    {
        if (_item != null)
            return _item.Level;
        return 0;
    }
    public string GetItemClass()
    {
        if (_item != null)
            return _item.ItemClass;
        return string.Empty;
    }
    public void Setup(UI_EquipItemData itemData, EquipmentSlot slot)
    {
        this._itemData = itemData;
        this._slot = slot;

        EquipableItem FindItem = Managers.Item.Database.ItemList.Where(x => x.ItemId == _itemData.ItemId).FirstOrDefault() as EquipableItem;
        if (FindItem != null)
        {
            DisplayName.text = FindItem.DisplayName;

            Outline DisplayNameOutline = Util.GetOrAddComponent<Outline>(DisplayName.gameObject);
            DisplayNameOutline.effectColor = itemData.DisplayNameOutline.OutlineColor;
            DisplayNameOutline.effectDistance = itemData.DisplayNameOutline.EffectDistance;

            string _itemDescription = "ÀåÂøÈ¿°ú : ";
            foreach (StatModifier _stat in FindItem.StatModifiers)
            {
                if (_stat.CodeName == "CriticalHitRate")
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", (_stat.Value / 10).ToString()) + System.Environment.NewLine;
                else
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", _stat.Value.ToString()) + System.Environment.NewLine;
            }

            Description.text = _itemDescription;


            Level.text = $"{((EquipableItem.Rank)FindItem.Level).ToString()} µî±Þ";

            gameObject.GetComponent<Image>().sprite = itemData.BackgroundSprite;
            gameObject.GetComponent<Image>().color = itemData.BackgroundColor;

            Outline gameObjectOutline = Util.GetOrAddComponent<Outline>(gameObject);
            gameObjectOutline.effectColor = itemData.BackgroundOutline.OutlineColor;
            gameObjectOutline.effectDistance = itemData.BackgroundOutline.EffectDistance;

            Icon.sprite = itemData.IconSprite;
            IconBackgroundSprite.sprite = itemData.IconWrapperSprite;

            IconBackground.sprite = itemData.ItemPanelSprite;
            Outline IconBackgroundOutline = Util.GetOrAddComponent<Outline>(IconBackground.gameObject);
            IconBackgroundOutline.effectColor = itemData.ItemPanelOutline.OutlineColor;
            IconBackgroundOutline.effectDistance = itemData.ItemPanelOutline.EffectDistance;

            UpdateSlot();

            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            EquipButton.onClick.RemoveAllListeners();
            EquipButton.onClick.AddListener(() =>
            {
                EquipableItem _getItem = Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId) as EquipableItem;
                if (_getItem == null)
                    return;

                if (_slot.IsEquip)
                {
                    if (_slot.GetItem.ItemId == _itemData.ItemId)
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

            UpgradeButton.onClick.RemoveAllListeners();
            UpgradeButton.onClick.AddListener(() =>
            {
                if (Managers.Item.UpgradeOneItem(Managers.Game.GetInventory().Find(x => x.ItemId == _itemData.ItemId), false))
                    OnUpgrade?.Invoke(this);
                UpdateCountText();
            });
        }
        else
            Debug.LogError("FindItem is null");
      

    }
    public void Setup(EquipableItem item, EquipmentSlot slot)
    {
        _item = item;
        _slot = slot;

        if (_item != null && _slot != null)
        {
            DisplayName.text = _item.DisplayName;
            _item.DisplayNameOutLine.AddOutline(DisplayName.gameObject);

            string _itemDescription = "ÀåÂøÈ¿°ú : ";
            foreach (StatModifier _stat in _item.StatModifiers)
            {
                if (_stat.CodeName == "CriticalHitRate")
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", (_stat.Value / 10).ToString()) + System.Environment.NewLine;
                else
                    _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", _stat.Value.ToString()) + System.Environment.NewLine;
            }

            Description.text = _itemDescription;

            Level.text = $"{((EquipableItem.Rank)_item.Level).ToString()} µî±Þ";

            
            gameObject.GetComponent<Image>().sprite = _item.DescriptionBackgroundTexture;
            gameObject.GetComponent<Image>().color = _item.DescriptionBackgroundColor;

            _item.DescriptionBackgroundOutline.AddOutline(gameObject);


            Icon.sprite = _item.Icon;
            IconBackground.sprite = _item.IconBackground;
            IconBackgroundSprite.sprite = _item.DescriptionBackground;

            _item.IconBackgroundOutline.AddOutline(IconBackground.gameObject);

            UpdateSlot();

            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            EquipButton.onClick.RemoveAllListeners();
            EquipButton.onClick.AddListener(() => {
                EquipableItem _getItem = Managers.Game.GetInventory().Find(x => x.ItemId == _item.ItemId) as EquipableItem;
                if (_getItem == null)
                    return;

                if (_slot.IsEquip)
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

            UpgradeButton.onClick.RemoveAllListeners();
            UpgradeButton.onClick.AddListener(() => {
                if (Managers.Item.UpgradeOneItem(_item, false))
                    OnUpgrade?.Invoke(this);
                UpdateCountText();
            });
        }
    }
    private void OnEnable()
    {
        if (_itemData != null && _slot != null)
            UpdateSlot();
    }
    public override void Init()
    {
        /*
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
        */

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
        EquipableItem _GetItem = Managers.Game.GetInventory().GetItem(_itemData.ItemId) as EquipableItem;

        if (_GetItem == null)
            Count.text = $"º¸À¯ : {0}";
        else
            Count.text = $"º¸À¯ : {_GetItem.GetUsableCount()}";
    }

    public void UpdateEquipText()
    {
        if (_slot.IsEquip)
        {
            if (_slot.GetItem.ItemId == _itemData.ItemId)
                EquipButtonText.text = UnEquipText;
            else
                EquipButtonText.text = EquipText;
        }
        else
        {
            EquipButtonText.text = EquipText;
        }

    }

    public void UpdateEquipIcon()
    {
        if (_slot.IsEquip)
        {
            if (_slot.GetItem.ItemId == _itemData.ItemId)
                SelectedIcon.SetActive(true);
            else
                SelectedIcon.SetActive(false);
        }
        else
        {
            SelectedIcon.SetActive(false);
        }


    }

}
