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

  

    List<UI_EquipItem> _bowItems;
    List<UI_EquipItem> _helmetItems;
    List<UI_EquipItem> _armorItems;
    List<UI_EquipItem> _cloakItems;

    List<UI_EquipItem> _selectItemList;

    UI_SelectButton[] _selectors;
    int _selectId;
    bool _isUpdate = false;

    string _isUpdateBow;
    string _isUpdateHelmet;
    string _isUpdateArmor;
    string _isUpdateCloak;

    [SerializeField]
    RecyclableScrollRect _scrollView;

    // 팝업창이 종료될때 각 장착창을 서버에 저장하기 위해 사용된다.
    List<PlayerInfo.UserDataKey> _updateKeys;

    string selectItem = "Bow";

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        _isUpdate = false;

        _bowItems = new List<UI_EquipItem>();
        _helmetItems = new List<UI_EquipItem>();
        _armorItems = new List<UI_EquipItem>();
        _cloakItems = new List<UI_EquipItem>();

        _updateKeys = new List<PlayerInfo.UserDataKey>();

        _isUpdateBow = JsonUtility.ToJson(Managers.Game.GetEquipment("Bow").ToSaveData());
        _isUpdateHelmet = JsonUtility.ToJson(Managers.Game.GetEquipment("Helmet").ToSaveData());
        _isUpdateArmor = JsonUtility.ToJson(Managers.Game.GetEquipment("Armor").ToSaveData());
        _isUpdateCloak = JsonUtility.ToJson(Managers.Game.GetEquipment("Cloak").ToSaveData());

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
                selectItem = "Bow";
                _selectItemList.Clear();
                _scrollView.ReloadData();

            });
        AddUIEvent(GetButton((int)Buttons.HelmetButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.HelmetButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                selectItem = "Helmet";
                _selectItemList.Clear();
                _scrollView.ReloadData();
            });
        AddUIEvent(GetButton((int)Buttons.ArmorButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.ArmorButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                selectItem = "Armor";
                _selectItemList.Clear();
                _scrollView.ReloadData();
            });
        AddUIEvent(GetButton((int)Buttons.CloakButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.CloakButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                selectItem = "Cloak";
                _selectItemList.Clear();
                _scrollView.ReloadData();
            });


        AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => {
            if (selectItem == "Bow")
            {
                if (Managers.Item.UpgradeAllItems(selectItem))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateBow, selectItem))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Bow);
                        _isUpdateBow = JsonUtility.ToJson(Managers.Game.GetEquipment("Bow").ToSaveData());
                    }
                }
            }
            else if (selectItem == "Armor")
            {
                if (Managers.Item.UpgradeAllItems(selectItem))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateArmor, selectItem))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Armor);
                        _isUpdateArmor = JsonUtility.ToJson(Managers.Game.GetEquipment("Armor").ToSaveData());
                    }
                }
            }
            else if (selectItem == "Helmet")
            {
                if (Managers.Item.UpgradeAllItems(selectItem))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateHelmet, selectItem))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Helmet);
                        _isUpdateHelmet = JsonUtility.ToJson(Managers.Game.GetEquipment("Helmet").ToSaveData());
                    }
                }
            }
            else if (selectItem == "Cloak")
            {
                if (Managers.Item.UpgradeAllItems(selectItem))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateCloak, selectItem))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Cloak);
                        _isUpdateCloak = JsonUtility.ToJson(Managers.Game.GetEquipment("Cloak").ToSaveData());
                    }
                }
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
        if (selectItem == "Bow")
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateBow, selectItem))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Bow);
                _isUpdateBow = JsonUtility.ToJson(Managers.Game.GetEquipment(selectItem).ToSaveData());
            }
        }
        else if (selectItem == "Helmet")
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateHelmet, selectItem))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Helmet);
                _isUpdateHelmet = JsonUtility.ToJson(Managers.Game.GetEquipment(selectItem).ToSaveData());
            }
        }
        else if (selectItem == "Armor")
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateArmor, selectItem))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Armor);
                _isUpdateArmor = JsonUtility.ToJson(Managers.Game.GetEquipment(selectItem).ToSaveData());
            }
        }
        else if (selectItem == "Cloak")
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateCloak, selectItem))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Cloak);
                _isUpdateCloak = JsonUtility.ToJson(Managers.Game.GetEquipment(selectItem).ToSaveData());
            }
        }
     
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
        if (Managers.Item.CanSavableEquipment(_isUpdateCloak, "Cloak"))
            _updateKeys.Add(PlayerInfo.UserDataKey.Cloak);
        if (Managers.Item.CanSavableEquipment(_isUpdateArmor, "Armor"))
            _updateKeys.Add(PlayerInfo.UserDataKey.Armor);
        if (Managers.Item.CanSavableEquipment(_isUpdateBow, "Bow"))
            _updateKeys.Add(PlayerInfo.UserDataKey.Bow);
        if (Managers.Item.CanSavableEquipment(_isUpdateHelmet, "Helmet"))
            _updateKeys.Add(PlayerInfo.UserDataKey.Helmet);

        if(_updateKeys.Count > 0)
        {
            Managers.Game.Save(_updateKeys.ToArray());
#if !ENABLE_LOG
            Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.ItemEquipment });
#endif
        }



        base.ClosePopupUI();

        _bowItems.Clear();
        _helmetItems.Clear();
        _armorItems.Clear();
        _cloakItems.Clear();



    }

    public int GetItemCount()
    {
        return Managers.Item.Database.ItemList.Where(x => x.ItemClass == selectItem).Count();
    }

    public void SetCell(ICell cell, int index)
    {
        UI_EquipItem item = cell as UI_EquipItem;
        if (selectItem == "Bow")
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == selectItem).OrderBy(x => (x as Bow).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(selectItem).SlotList[0]);
        else if (selectItem == "Helmet")
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == selectItem).OrderBy(x => (x as Helmet).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(selectItem).SlotList[0]);
        else if (selectItem == "Cloak")
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == selectItem).OrderBy(x => (x as Cloak).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(selectItem).SlotList[0]);
        else
        {
            item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemClass == selectItem).OrderBy(x => (x as Armor).Level).ElementAt(index) as EquipableItem, Managers.Game.GetEquipment(selectItem).SlotList[0]);

        }

        item.OnEquip -= OnEquipSlot;
        item.OnEquip += OnEquipSlot;
        item.OnUpgrade -= OnUpgradeItem;
        item.OnUpgrade += OnUpgradeItem;

        _selectItemList.Add(item);
        //item.Setup
        return;
    }
}
