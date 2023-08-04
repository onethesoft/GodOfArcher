using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UI_Item : UI_Popup
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

    UI_SelectButton[] _selectors;
    int _selectId;
    bool _isUpdate = false;

    string _isUpdateBow;
    string _isUpdateHelmet;
    string _isUpdateArmor;
    string _isUpdateCloak;

    // 팝업창이 종료될때 각 장착창을 서버에 저장하기 위해 사용된다.
    List<PlayerInfo.UserDataKey> _updateKeys;

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
        

        Get<GameObject>((int)GameObjects.ArmorView).SetActive(false);
        Get<GameObject>((int)GameObjects.HelmetView).SetActive(false);
        Get<GameObject>((int)GameObjects.CloakView).SetActive(false);
        
        foreach (Bow bow in Managers.Item.Database.BowList.OrderBy(x=>x.Level))
        {
            UI_EquipItem _bowItem = Util.GetOrAddComponent<UI_EquipItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_EquipItem", Get<GameObject>((int)GameObjects.BowContent).transform));
            
            _bowItem.Setup(bow);
            _bowItem.Slot = Managers.Game.GetEquipment("Bow").SlotList[0];
            _bowItem.OnEquip += OnEquipSlot;
            _bowItem.OnUpgrade += OnUpgradeItem;



            _bowItems.Add(_bowItem);
        }
        foreach (Helmet helmet in Managers.Item.Database.HelmetList.OrderBy(x => x.Level))
        {
            UI_EquipItem _helmetItem = Util.GetOrAddComponent<UI_EquipItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_EquipItem", Get<GameObject>((int)GameObjects.HelmetContent).transform));
            _helmetItem.Setup(helmet);
            _helmetItem.Slot = Managers.Game.GetEquipment("Helmet").SlotList[0];
            _helmetItem.OnEquip += OnEquipSlot;
            _helmetItem.OnUpgrade += OnUpgradeItem;

            _helmetItems.Add(_helmetItem);
        }
        foreach (Armor armor in Managers.Item.Database.ArmorList.OrderBy(x => x.Level))
        {
            UI_EquipItem _armorItem = Util.GetOrAddComponent<UI_EquipItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_EquipItem", Get<GameObject>((int)GameObjects.ArmorContent).transform));
            _armorItem.Setup(armor);
            _armorItem.Slot = Managers.Game.GetEquipment("Armor").SlotList[0];
            _armorItem.OnEquip += OnEquipSlot;
            _armorItem.OnUpgrade += OnUpgradeItem;

            _armorItems.Add(_armorItem);
        }
        foreach (Cloak cloak in Managers.Item.Database.CloakList.OrderBy(x => x.Level))
        {
            UI_EquipItem _cloakItem = Util.GetOrAddComponent<UI_EquipItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_EquipItem", Get<GameObject>((int)GameObjects.CloakContent).transform));
            _cloakItem.Setup(cloak);
            _cloakItem.Slot = Managers.Game.GetEquipment("Cloak").SlotList[0];
            _cloakItem.OnEquip += OnEquipSlot;
            _cloakItem.OnUpgrade += OnUpgradeItem;

            _cloakItems.Add(_cloakItem);
        }
        
        SetView((int)GameObjects.BowView);
       


        AddUIEvent(GetButton((int)Buttons.BowButton).gameObject, 
            (data) => 
            {

                GetButton((int)Buttons.BowButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                SetView((int)GameObjects.BowView);

            });
        AddUIEvent(GetButton((int)Buttons.HelmetButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.HelmetButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                SetView((int)GameObjects.HelmetView);
            });
        AddUIEvent(GetButton((int)Buttons.ArmorButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.ArmorButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                SetView((int)GameObjects.ArmorView);
            });
        AddUIEvent(GetButton((int)Buttons.CloakButton).gameObject, 
            (data) => 
            {
                GetButton((int)Buttons.CloakButton).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
                SetView((int)GameObjects.CloakView);
            });


        AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => { 
            if(_selectId == (int)GameObjects.BowView)
            {
                if(Managers.Item.UpgradeAllItems("Bow"))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateBow, "Bow"))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Bow);
                        _isUpdateBow = JsonUtility.ToJson(Managers.Game.GetEquipment("Bow").ToSaveData());
                    }
                }
            }
            else if(_selectId == (int)GameObjects.ArmorView)
            {
                if (Managers.Item.UpgradeAllItems("Armor"))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateArmor, "Armor"))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Armor);
                        _isUpdateArmor = JsonUtility.ToJson(Managers.Game.GetEquipment("Armor").ToSaveData());
                    }
                }
            }
            else if (_selectId == (int)GameObjects.HelmetView)
            {
                if (Managers.Item.UpgradeAllItems("Helmet"))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateHelmet, "Helmet"))
                    {
                        Managers.Game.Save(PlayerInfo.UserDataKey.Helmet);
                        _isUpdateHelmet = JsonUtility.ToJson(Managers.Game.GetEquipment("Helmet").ToSaveData());
                    }
                }
            }
            else if (_selectId == (int)GameObjects.CloakView)
            {
                if (Managers.Item.UpgradeAllItems("Cloak"))
                {
                    if (Managers.Item.CanSavableEquipment(_isUpdateCloak, "Cloak"))
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
        if(_bowItems.Any(x=> x == sender))
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateBow, "Bow"))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Bow);
                _isUpdateBow = JsonUtility.ToJson(Managers.Game.GetEquipment("Bow").ToSaveData());
            }
        }
        else if(_helmetItems.Any(x => x == sender))
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateHelmet, "Helmet"))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Bow);
                _isUpdateHelmet = JsonUtility.ToJson(Managers.Game.GetEquipment("Helmet").ToSaveData());
            }
        }
        else if (_armorItems.Any(x => x == sender))
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateArmor, "Armor"))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Armor);
                _isUpdateArmor = JsonUtility.ToJson(Managers.Game.GetEquipment("Armor").ToSaveData());
            }
        }
        else if (_cloakItems.Any(x => x == sender))
        {
            if (Managers.Item.CanSavableEquipment(_isUpdateCloak, "Cloak"))
            {
                Managers.Game.Save(PlayerInfo.UserDataKey.Cloak);
                _isUpdateCloak = JsonUtility.ToJson(Managers.Game.GetEquipment("Cloak").ToSaveData());
            }
        }
    }
    void OnEquipSlot(UI_EquipItem slot)
    {
        if(_bowItems.Any(x=> x == slot))
        {
            foreach (UI_EquipItem _item in _bowItems)
                if (_item != slot)
                    _item.UpdateSlot();


        }
        else if (_helmetItems.Any(x => x == slot))
        {
            foreach (UI_EquipItem _item in _helmetItems)
                if (_item != slot)
                    _item.UpdateSlot();

        }
        else if (_armorItems.Any(x => x == slot))
        {
            foreach (UI_EquipItem _item in _armorItems)
                if (_item != slot)
                    _item.UpdateSlot();

        }
        else if (_cloakItems.Any(x => x == slot))
        {
            foreach (UI_EquipItem _item in _cloakItems)
                if (_item != slot)
                    _item.UpdateSlot();
        }
    }

    void SetView(int GameObjectsId)
    {
        _selectId = GameObjectsId;
        if (GameObjectsId == (int)GameObjects.BowView)
        {
            
            Get<GameObject>((int)GameObjects.BowView).SetActive(true);
            Get<GameObject>((int)GameObjects.ArmorView).SetActive(false);
            Get<GameObject>((int)GameObjects.HelmetView).SetActive(false);
            Get<GameObject>((int)GameObjects.CloakView).SetActive(false);
        }
        else if(GameObjectsId == (int)GameObjects.ArmorView)
        {
            Get<GameObject>((int)GameObjects.BowView).SetActive(false);
            Get<GameObject>((int)GameObjects.ArmorView).SetActive(true);
            Get<GameObject>((int)GameObjects.HelmetView).SetActive(false);
            Get<GameObject>((int)GameObjects.CloakView).SetActive(false);
        }
        else if (GameObjectsId == (int)GameObjects.HelmetView)
        {
            Get<GameObject>((int)GameObjects.BowView).SetActive(false);
            Get<GameObject>((int)GameObjects.ArmorView).SetActive(false);
            Get<GameObject>((int)GameObjects.HelmetView).SetActive(true);
            Get<GameObject>((int)GameObjects.CloakView).SetActive(false);
        }
        else if (GameObjectsId == (int)GameObjects.CloakView)
        {
            Get<GameObject>((int)GameObjects.BowView).SetActive(false);
            Get<GameObject>((int)GameObjects.ArmorView).SetActive(false);
            Get<GameObject>((int)GameObjects.HelmetView).SetActive(false);
            Get<GameObject>((int)GameObjects.CloakView).SetActive(true);
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
            Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.ItemEquipment });
        }



        base.ClosePopupUI();

        _bowItems.Clear();
        _helmetItems.Clear();
        _armorItems.Clear();
        _cloakItems.Clear();



    }

}
