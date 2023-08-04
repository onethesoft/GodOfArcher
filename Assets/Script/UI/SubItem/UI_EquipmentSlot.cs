using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentSlot : UI_Base
{
    public delegate void SelectHandler(UI_EquipmentSlot slot);
    public SelectHandler OnSelect;


    enum GameObjects
    {
        Select ,
        fx_background ,
        CurrencyImage ,
        CurrencyText ,
        AmountText,
        EquipPosition
    }
    enum Texts
    {
        Description
    }
    enum Buttons
    {
        SlotBackground
    }
    
    EquipmentSlot _slot;
    public EquipmentSlot Slot { set { _slot = value; } get { return _slot; } }

    UI_BaseItem _equipItem = null;

    public void Select()
    {
        OnSelect?.Invoke(this);
        Get<GameObject>((int)GameObjects.Select).SetActive(true);
    }
    public void UnSelect()
    {
        Get<GameObject>((int)GameObjects.Select).SetActive(false);
    }
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));


        Get<GameObject>((int)GameObjects.Select).SetActive(false);

        AddUIEvent(GetButton((int)Buttons.SlotBackground).gameObject, (data) => {
            if(_slot.IsLock == false )
                Select();
            else
            {
                if(_slot.category == "Buyable")
                {
                    Debug.Log(_slot.IsUnLockable);
                    if(_slot.IsUnLockable)
                    {
                        // 구매팝업 창 띄움
                        UI_Purchase _popup = Managers.UI.ShowPopupUI<UI_Purchase>();
                        _popup.Title = "인벤토리 추가 구매";
                        _popup.Currency = Define.CurrencyID.Ruby;
                        _popup.Amount = (int)(_slot.UnitPrice);
                        _popup.OnPurchase += () =>
                        {
                            _slot.UnLock();
                        };
                    }
                    else
                    {
                        // 구매팝업 창 띄움
                        UI_Messagebox _popup = Managers.UI.ShowPopupUI<UI_Messagebox>();
                        _popup.mode = UI_Messagebox.Mode.OK;
                        _popup.Title = "인벤토리 추가 구매";
                        _popup.Text = _slot.UnLockableReson;
                        _popup.TextSize = 40;


                    }
                }
               
                
            }
        });

        if (_slot != null)
        {
            GetText((int)Texts.Description).text = _slot.Description;
            UpdateSlot();

            _slot.EquipItem -= Equip;
            _slot.EquipItem += Equip;

            _slot.UnEquipItem -= UnEquip;
            _slot.UnEquipItem += UnEquip;


            _slot.OnUnlock -= UpdateSlot;
            _slot.OnUnlock += UpdateSlot;
           
        }
    }
    void UpdateSlot()
    {
        if(Slot.IsLock)
        {
            if (Slot.category == "Buyable")
            {
                Get<GameObject>((int)GameObjects.AmountText).SetActive(true);
                Get<GameObject>((int)GameObjects.CurrencyText).SetActive(true);
                Get<GameObject>((int)GameObjects.CurrencyImage).SetActive(true);
                Get<GameObject>((int)GameObjects.fx_background).SetActive(true);

                Get<GameObject>((int)GameObjects.AmountText).GetComponent<Text>().text = $"x{_slot.UnitPrice.ToString()}";
            }
            else if(Slot.category == "Rating")
            {
                Get<GameObject>((int)GameObjects.CurrencyText).SetActive(true);
                Get<GameObject>((int)GameObjects.CurrencyImage).SetActive(false);
                Get<GameObject>((int)GameObjects.fx_background).SetActive(false);

                Get<GameObject>((int)GameObjects.AmountText).SetActive(false);

                Get<GameObject>((int)GameObjects.CurrencyText).GetComponent<Text>().text = _slot.Description.Split(' ')[0];
            }
        }
        else
        {
            if(Get<GameObject>((int)GameObjects.AmountText).activeSelf)     Get<GameObject>((int)GameObjects.AmountText).SetActive(false);
            if (Get<GameObject>((int)GameObjects.CurrencyText).activeSelf)  Get<GameObject>((int)GameObjects.CurrencyText).SetActive(false);
            if (Get<GameObject>((int)GameObjects.CurrencyImage).activeSelf) Get<GameObject>((int)GameObjects.CurrencyImage).SetActive(false);
            if (Get<GameObject>((int)GameObjects.fx_background).activeSelf) Get<GameObject>((int)GameObjects.fx_background).SetActive(false);
          

            if (Slot.IsEquip)
            {
                if (Get<GameObject>((int)GameObjects.EquipPosition) != null)
                {
                    _equipItem = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_BaseItem", Get<GameObject>((int)GameObjects.EquipPosition).transform));
                    _equipItem.Item = Slot.GetItem;
                }
            }
            else
            {
                if (_equipItem != null)
                {
                    Managers.Resource.Destroy(_equipItem.gameObject);
                    _equipItem = null;
                }

            }
        }

       
    }
    void Equip(EquipableItem item)
    {

        UpdateSlot();


    }
    void UnEquip(EquipableItem item)
    {
        UpdateSlot();
    }

    private void OnDestroy()
    {
        if (_slot != null)
        {
            _slot.EquipItem -= Equip;
            _slot.UnEquipItem -= UnEquip;
            _slot.OnUnlock -= UpdateSlot;
        }
    }
       

}
