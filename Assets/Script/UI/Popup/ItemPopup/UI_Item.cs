using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using PolyAndCode.UI;

public class UI_Item : UI_Popup, IRecyclableScrollRectDataSource
{
    enum Buttons
    {
        Close ,
        Exit ,
        TotalUpgradeButton,
        ShopButton,
        HelpButton,
        BowButton ,
        HelmetButton ,
        ArmorButton,
        CloakButton,
        
    }

    enum GameObjects
    {
        BowView,
        ArmorView,
        HelmetView,
        CloakView ,
        BowContent,
        ArmorContent,
        HelmetContent,
        CloakContent
    }

    enum ItemType
    {
        Bow,
        Helmet,
        Armor,
        Cloak
    }
    [SerializeField]
    List<UI_EquipItemData> _itemViewDataList;

    public void SetupItemViewData(List<UI_EquipItemData> list)
    {
        _itemViewDataList = list;
    }
    


    List<UI_EquipItem> _bowItems;
    List<UI_EquipItem> _helmetItems;
    List<UI_EquipItem> _armorItems;
    List<UI_EquipItem> _cloakItems;

    List<UI_EquipItem> _selectItemList;

    UI_SelectButton[] _selectors;
   

    [SerializeField]
    RecyclableScrollRect _scrollView;

    // 팝업창이 종료될때 각 장착창을 서버에 저장하기 위해 사용된다.
    List<PlayerInfo.UserDataKey> _updateKeys;

    [SerializeField]
    ItemType _selectType = ItemType.Bow;

    string[] _equipmentSaveData;

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));



        _bowItems = new List<UI_EquipItem>();
        _helmetItems = new List<UI_EquipItem>();
        _armorItems = new List<UI_EquipItem>();
        _cloakItems = new List<UI_EquipItem>();

        _updateKeys = new List<PlayerInfo.UserDataKey>();

        _equipmentSaveData = new string[Enum.GetValues(typeof(ItemType)).Length];
        foreach (ItemType itemtype in Enum.GetValues(typeof(ItemType)))
            _equipmentSaveData[(int)itemtype] = JsonUtility.ToJson(Managers.Game.GetEquipment(itemtype.ToString()).ToSaveData());
        
        _selectItemList = new List<UI_EquipItem>();

        _selectors = new UI_SelectButton[4];
        int _btnIndex = 0;
        for(int i= (int)Buttons.BowButton; i < Enum.GetNames(typeof(Buttons)).Length;i++)
        {
            _selectors[_btnIndex] = Util.GetOrAddComponent<UI_SelectButton>(GetButton(i).gameObject);
            _selectors[_btnIndex].Select = false;
            _selectors[_btnIndex].onSelect -= OnSelectButton;
            _selectors[_btnIndex].onSelect += OnSelectButton;
            _btnIndex++;
        }

        _scrollView.DataSource = this;
        


        AddUIEvent(GetButton((int)Buttons.BowButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.BowButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                
                _selectType = ItemType.Bow;
                _selectItemList.Clear();
                _scrollView.ReloadData();

            });
        AddUIEvent(GetButton((int)Buttons.HelmetButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.HelmetButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();

                _selectType = ItemType.Helmet;
                _selectItemList.Clear();
                _scrollView.ReloadData();
            });
        AddUIEvent(GetButton((int)Buttons.ArmorButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.ArmorButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
          
                _selectType = ItemType.Armor;
                _selectItemList.Clear();
                _scrollView.ReloadData();
            });
        AddUIEvent(GetButton((int)Buttons.CloakButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.CloakButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();

                _selectType = ItemType.Cloak;
                _selectItemList.Clear();
                _scrollView.ReloadData();
            });


        AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => {
            if(Managers.Item.UpgradeAllItems(_selectType.ToString()))
            {
                SaveEquipment(_selectType);
            }

            

        });
        AddUIEvent(GetButton((int)Buttons.ShopButton).gameObject, (data) => {
            ClosePopupUI();
            Managers.UI.ShowPopupUI<UI_ShopPopup>();
        });
        AddUIEvent(GetButton((int)Buttons.HelpButton).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_Help>();
        });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });

    }

    void OnUpgradeItem(UI_EquipItem sender)
    {
        SaveEquipment(_selectType);
        
     
    }
    void OnEquipSlot(UI_EquipItem slot)
    {
        foreach (UI_EquipItem _item in _selectItemList)
        {
            if (_item != slot)
                _item.UpdateSlot();
        }
      
    }

   
    void OnSelectButton(UI_SelectButton selected)
    {
        foreach(UI_SelectButton _btn in _selectors)
        {
            if (selected == _btn) continue;
            _btn.Select = false;
        }
        
    }
    
    private void Start()
    {
        Init();
    }

    public override void ClosePopupUI()
    {
        _updateKeys.Clear();
        if (Managers.Item.CanSavableEquipment(_equipmentSaveData[(int)ItemType.Cloak], ItemType.Cloak.ToString()))
            _updateKeys.Add(PlayerInfo.UserDataKey.Cloak);
        if (Managers.Item.CanSavableEquipment(_equipmentSaveData[(int)ItemType.Armor], ItemType.Armor.ToString()))
            _updateKeys.Add(PlayerInfo.UserDataKey.Armor);
        if (Managers.Item.CanSavableEquipment(_equipmentSaveData[(int)ItemType.Bow], ItemType.Bow.ToString()))
            _updateKeys.Add(PlayerInfo.UserDataKey.Bow);
        if (Managers.Item.CanSavableEquipment(_equipmentSaveData[(int)ItemType.Helmet], ItemType.Helmet.ToString()))
            _updateKeys.Add(PlayerInfo.UserDataKey.Helmet);

        if(_updateKeys.Count > 0)
        {
            Managers.Game.Save(_updateKeys.ToArray());

            Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.ItemEquipment });


        }



        base.ClosePopupUI();
        
        _bowItems.Clear();
        _helmetItems.Clear();
        _armorItems.Clear();
        _cloakItems.Clear();



    }

    public int GetItemCount()
    {
        return Managers.Item.Database.ItemList.Where(x => x.ItemClass == _selectType.ToString()).Count();
    }

    public void SetCell(ICell cell, int index)
    {
        UI_EquipItem item = cell as UI_EquipItem;
        
      
        
        item.Setup(_itemViewDataList.Where(x => x.ItemClass == _selectType.ToString()).ElementAt(index), Managers.Game.GetEquipment(_selectType.ToString()).SlotList[0]);
        
        
        /*
        if (_selectType == ItemType.Bow)
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == _selectType.ToString()).OrderBy(x => (x as Bow).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(_selectType.ToString()).SlotList[0]);
        else if (_selectType == ItemType.Helmet)
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == _selectType.ToString()).OrderBy(x => (x as Helmet).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(_selectType.ToString()).SlotList[0]);
        else if (_selectType == ItemType.Cloak)
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == _selectType.ToString()).OrderBy(x => (x as Cloak).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(_selectType.ToString()).SlotList[0]);
        else
        {
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == _selectType.ToString()).OrderBy(x => (x as Armor).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(_selectType.ToString()).SlotList[0]);

        }
        */

        item.OnEquip -= OnEquipSlot;
        item.OnEquip += OnEquipSlot;
        item.OnUpgrade -= OnUpgradeItem;
        item.OnUpgrade += OnUpgradeItem;

        _selectItemList.Add(item);
        //item.Setup
        return;
    }

    void SaveEquipment(ItemType itemtype)
    {
        if (Managers.Item.CanSavableEquipment(_equipmentSaveData[(int)itemtype], itemtype.ToString()))
        {
            PlayerInfo.UserDataKey _saveKey;
            if (Enum.TryParse(itemtype.ToString(), out _saveKey))
            {
                Managers.Game.Save(_saveKey);
                _equipmentSaveData[(int)itemtype] = JsonUtility.ToJson(Managers.Game.GetEquipment(itemtype.ToString()).ToSaveData());
            }
        }
    }
}
