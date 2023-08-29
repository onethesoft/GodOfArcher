using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_RunePopup : UI_Popup
{

    enum GameObjects
    {
        SlotLayout ,
        Content
    }

    enum Buttons
    {
        Close,
        Exit,
        RandomboxButton,
        TotalUpgradeButton,
        HelpButton,
        ConvertButton
    }

    List<UI_RuneItem> _runeItems;
    List<UI_EquipmentSlot> _slotItems;
    UI_EquipmentSlot _selected;
    BlockerGroup[] _blockerGroups;

    string _isUpdateKey; // Key 를 비교하여 다르면 서버에 업데이트한다.

    [SerializeField]
    List<UI_RuneItemData> _runeItemViewDataList;
    public void SetupItemViewData(List<UI_RuneItemData> itemList)
    {
        _runeItemViewDataList = itemList;
    }
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _runeItems = new List<UI_RuneItem>();
        _slotItems = new List<UI_EquipmentSlot>();

      
        _isUpdateKey = JsonUtility.ToJson(Managers.Game.GetEquipment("Rune").ToSaveData());

        foreach (EquipmentSlot slot in Managers.Game.GetEquipment("Rune").SlotList)
        {

            UI_EquipmentSlot _slot = Util.GetOrAddComponent<UI_EquipmentSlot>(Managers.Resource.Instantiate($"UI/SubItem/ItemPopup/UI_RuneSlotItem", Get<GameObject>((int)GameObjects.SlotLayout).transform));
            _slot.Slot = slot;
            _slot.OnSelect -= OnSelectSlot;
            _slot.OnSelect += OnSelectSlot;
            _slotItems.Add(_slot);

            slot.OnUnlock += SaveEquipment;
            
        }

        int ItemCount = 0;
        GameObject ItemPanel = null;
        foreach(UI_RuneItemData runeItemData in _runeItemViewDataList)
        {
            if (ItemCount % 3 == 0)
            {
                ItemPanel = Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_RuneItemPanel", Get<GameObject>((int)GameObjects.Content).transform);
                if (runeItemData.Level <= 6)
                    ItemPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1900, 300);
                else
                    ItemPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1900, 420);


            }

            UI_RuneItem _item;
          
            _item = Util.GetOrAddComponent<UI_RuneItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_RuneItem_normal", ItemPanel.transform));
            _item.Setup(runeItemData);

            _item.OnEquipRune -= OnEquipHandler;
            _item.OnEquipRune += OnEquipHandler;

            _item.OnUprade -= SaveEquipment;
            _item.OnUprade += SaveEquipment;

            _item.OnUprade -= Block;
            _item.OnUprade += Block;
            _runeItems.Add(_item);

            
            ItemCount++;
        }


        _blockerGroups = GetComponentsInChildren<BlockerGroup>();

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { 
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { 
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => {
     
            if (Managers.Item.UpgradeAllItems("Rune") == true )
            {
                SaveEquipment();
                Block();
            }
        });
        AddUIEvent(GetButton((int)Buttons.RandomboxButton).gameObject, (data) => 
        { 
            ClosePopupUI();
            Managers.UI.ShowPopupUI<UI_ShopPopup>();
        });

        AddUIEvent(GetButton((int)Buttons.HelpButton).gameObject, (data) =>
        {
            Managers.UI.ShowPopupUI<UI_Help>();
        });
        AddUIEvent(GetButton((int)Buttons.ConvertButton).gameObject, (data) =>
        {
            Managers.UI.ShowPopupUI<UI_Convert>().ConvertItemType = UI_Convert.ItemType.Rune;
        });
    }

    public void ShowText(string text)
    {
        if (UnityEngine.Object.FindObjectOfType<UI_FadeText>() == null)
        {
            UI_FadeText _info = Util.GetOrAddComponent<UI_FadeText>(Managers.Resource.Instantiate("UI/SubItem/UI_FadeText", gameObject.transform));
            _info.text = text;

        }
        else
        {
            UI_FadeText _info = UnityEngine.Object.FindObjectOfType<UI_FadeText>();
            _info.text = text;
            _info.RePlay();
        }
          
    }


    private void Start()
    {
        Init();
    }
    void OnUpradeRune()
    {
        if (Managers.Item.CanSavableEquipment(_isUpdateKey, "Rune"))
        {
            Managers.Game.Save(PlayerInfo.UserDataKey.Rune);
            _isUpdateKey = JsonUtility.ToJson(Managers.Game.GetEquipment("Rune").ToSaveData());
        }
    }
    void Block()
    {
        foreach (BlockerGroup blockgroup in _blockerGroups)
            blockgroup.Block();
    }
    void OnEquipHandler(Rune rune)
    {
        if(_selected != null)
        {
            if (!_selected.Slot.IsEquip)
            {
                _selected.Slot.Equip(rune);
            }
            else
            {

                if (_selected.Slot.UnEquip().ItemId != rune.ItemId)
                {
                    _selected.Slot.Equip(rune);
                }


            }
          
        }
        foreach (UI_RuneItem _runeItem in _runeItems)
            _runeItem.UpdateCountText();
    }
    void OnSelectSlot(UI_EquipmentSlot selected)
    {
        foreach(UI_EquipmentSlot _slot in _slotItems)
        {
            if (_slot != selected)
                _slot.UnSelect();
        }

        _selected = selected;
    }
    void SaveEquipment()
    {
        if (Managers.Item.CanSavableEquipment(_isUpdateKey, "Rune"))
        {
            Managers.Game.Save(PlayerInfo.UserDataKey.Rune);
            _isUpdateKey = JsonUtility.ToJson(Managers.Game.GetEquipment("Rune").ToSaveData());
        }
    }
    public override void ClosePopupUI()
    {
        SaveEquipment();

        base.ClosePopupUI();
        foreach (UI_EquipmentSlot _slot in _slotItems)
        {
            _slot.Slot.OnUnlock -= SaveEquipment;
        }
        _slotItems.Clear();

      
        _runeItems.Clear();

        
        
       
    }
}
