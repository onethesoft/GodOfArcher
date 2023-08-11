using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetPopup : UI_Popup
{
    enum GameObjects
    {
        SlotLayout,
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
    UI_EquipmentSlot _selected;
    List<UI_EquipmentSlot> _slotItems;
    List<UI_PetItem> _petItems;
    BlockerGroup[] _blockerGroups;

    string _isUpdateKey; // Key 를 비교하여 다르면 서버에 업데이트한다.
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _slotItems = new List<UI_EquipmentSlot>();
        _petItems = new List<UI_PetItem>();

        _isUpdateKey = JsonUtility.ToJson(Managers.Game.GetEquipment("Pet").ToSaveData());

        foreach (EquipmentSlot slot in Managers.Game.GetEquipment("Pet").SlotList) 
        {

            UI_EquipmentSlot _slot = Util.GetOrAddComponent<UI_EquipmentSlot>(Managers.Resource.Instantiate($"UI/SubItem/ItemPopup/UI_PetSlotItem", Get<GameObject>((int)GameObjects.SlotLayout).transform));
            _slot.Slot = slot;
            _slot.OnSelect -= OnSelectSlot;
            _slot.OnSelect += OnSelectSlot;
            _slotItems.Add(_slot);

            slot.OnUnlock += SaveEquipment;
        }

        int ItemCount = 0;
        GameObject ItemPanel = null;
       
        foreach (Pet pet in Managers.Item.Database.PetList)
        {
            UI_PetItem _item;
            if (pet.Level <= 6)
            {
                if (ItemCount % 2 == 0)
                {
                    ItemPanel = Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_PetItemPanel", Get<GameObject>((int)GameObjects.Content).transform);
                    _item = Util.GetOrAddComponent<UI_PetItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_PetItem_normal", ItemPanel.transform));

                }
                else
                    _item = Util.GetOrAddComponent<UI_PetItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_PetItem_normal", ItemPanel.transform));

            }
            else
            {
                ItemPanel = Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_PetItemPanel", Get<GameObject>((int)GameObjects.Content).transform);
                _item = Util.GetOrAddComponent<UI_PetItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_PetItem_Big", ItemPanel.transform));

            }


            _petItems.Add(_item);

            _item.Pet = pet;
            _item.OnEquipPet -= OnEquipHandler;
            _item.OnEquipPet += OnEquipHandler;

            _item.OnUprade -= SaveEquipment;
            _item.OnUprade += SaveEquipment;

            _item.OnUprade -= Block;
            _item.OnUprade += Block;
            ItemCount++;

        }

        _blockerGroups = GetComponentsInChildren<BlockerGroup>();

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => {

            if (Managers.Item.UpgradeAllItems("Pet") == true )
            {
                SaveEquipment();
                Block();

            }
        });
        AddUIEvent(GetButton((int)Buttons.RandomboxButton).gameObject, (data) => 
        { 
            ClosePopupUI();
            Managers.UI.ShowPopupUI<UI_ShopPopup>().Setup(UI_ShopPopup.Panel.ShopView);
        });

        AddUIEvent(GetButton((int)Buttons.HelpButton).gameObject, (data) =>
        {
            Managers.UI.ShowPopupUI<UI_Help>();
        });
        AddUIEvent(GetButton((int)Buttons.ConvertButton).gameObject, (data) =>
        {
            Managers.UI.ShowPopupUI<UI_Convert>().ConvertItemType = UI_Convert.ItemType.Pet;
        });
    }
    private void Start()
    {
        Init();
    }
    void OnSelectSlot(UI_EquipmentSlot selected)
    {
        foreach (UI_EquipmentSlot _slot in _slotItems)
        {
            if (_slot != selected)
                _slot.UnSelect();
        }

        _selected = selected;
    }

    void Block()
    {
        foreach (BlockerGroup blockgroup in _blockerGroups)
            blockgroup.Block();
    }

    void OnEquipHandler(Pet pet)
    {
        if (_selected != null)
        {
            if (!_selected.Slot.IsEquip)
                _selected.Slot.Equip(pet);
            else
            {
                if (_selected.Slot.UnEquip().ItemId != pet.ItemId)
                {
                    _selected.Slot.Equip(pet);
                }
            }

        }

        foreach (UI_PetItem _petItem in _petItems)
            _petItem.UpdateCountText();
        
    }

    void SaveEquipment()
    {
        if (Managers.Item.CanSavableEquipment(_isUpdateKey, "Pet"))
        {
            Managers.Game.Save(PlayerInfo.UserDataKey.Pet);
            _isUpdateKey = JsonUtility.ToJson(Managers.Game.GetEquipment("Pet").ToSaveData());
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

       
        _petItems.Clear();

       
        

    }
}
