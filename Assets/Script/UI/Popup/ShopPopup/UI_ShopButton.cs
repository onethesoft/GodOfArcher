using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class UI_ShopButton : UI_Base
{
    private Sprite _icon;
    public Sprite Icon
    {
        get
        {
            return _icon;
        }
        set
        {
            _icon = value;
            if (GetImage((int)Images.CurrencyIcon) != null)
                GetImage((int)Images.CurrencyIcon).sprite = _icon;
        }
    }

    private string _text;
    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
            if (GetText((int)Texts.Text) != null)
                GetText((int)Texts.Text).text = _text;
        }
    }

    private string _priceText;
    public string PriceText
    {
        get
        {
            return _priceText;
        }
        set
        {
            _priceText = value;
            if (GetText((int)Texts.PriceText) != null)
                GetText((int)Texts.PriceText).text = _priceText;
        }
    }
    private string _blockerText;
    public string BlockerText
    {
        get
        {
            return _blockerText;
        }
        set
        {
            _blockerText = value;
            if (GetText((int)Texts.BlockerText) != null)
                GetText((int)Texts.BlockerText).text = _blockerText;
        }
    }

    bool _isBlockerEnabled = false;
    public bool SetBlocker
    {
        get
        {
            return _isBlockerEnabled;
        }
        set
        {
            _isBlockerEnabled = value;
            SetEnableBlocker(_isBlockerEnabled);

        }
    }
    ShopButtonData _data;
    enum Images
    {
        CurrencyIcon,
        Blocker
    }
    enum Texts
    {
        Text,
        PriceText,
        BlockerText
    }
    enum Buttons
    {
        Purchase
    }

   
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        if(!string.IsNullOrEmpty(_priceText))
        {
            GetText((int)Texts.PriceText).text = _priceText;
        }
        if (!string.IsNullOrEmpty(_text))
        {
            GetText((int)Texts.Text).text = _text;
        }
        if(_icon != null)
        {
            GetImage((int)Images.CurrencyIcon).sprite = _icon;
        }

        GetText((int)Texts.Text).text = _data.ButtonText;
        GetImage((int)Images.CurrencyIcon).sprite = _data.Icon;
       
       

        if (_data.PurchaseItem != null)
        {
            if (_data.PurchaseItem.UnitPrice == 0)
            {
                GetText((int)Texts.PriceText).text = "FREE";

                UpdateStatus();

            }
            else
            {
                GetText((int)Texts.PriceText).text = _data.PurchaseItem.UnitPrice.ToString();
                UpdateStatus();
            }

            AddUIEvent(GetButton((int)Buttons.Purchase).gameObject, (data) =>
            {
                if (Managers.Game.GetCurrency(_data.Currency) < _data.Price)
                    return;

                if(Managers.Game.IsAdSkipped == false && _data.PurchaseItem.UnitPrice == 0)
                {
                    UI_AdConfirm _showmessage = Managers.UI.ShowPopupUI<UI_AdConfirm>();
                    _showmessage.TitleText = "뽑기 광고";
                    _showmessage.ContentText = "무료 뽑기는 4시간에 1번 가능합니다";
                    _showmessage.ContentTextFontSize = 32;
                    _showmessage.OnOK += () => {

                        if (Managers.Ad.ShowAd(_data.PurchaseItem.ItemId, () => { Managers.Shop.BuyRandomBox(_data.PurchaseItem.ItemId); }) == false)
                        {

                        }
                       
                    };
                }
                else
                    Managers.Shop.BuyRandomBox(_data.PurchaseItem.ItemId);

                //Managers.Game.SubstractCurrency(_data.Currency, _data.Price);
                //List<BaseItem> _grants = Managers.Item.GrantItemToUser(_data.PurchaseItem.ItemId);
                //Managers.UI.ShowPopupUI<UI_RandomboxPopup>().Setup(_grants, _shopdata);



            });

          
        }
    }
    public void Setup(ShopButtonData data)
    {
        _data = data;
    }
    void SetEnableBlocker(bool isEnabled = false)
    {
        if (GetImage((int)Images.Blocker) == null)
            return;

        GetImage((int)Images.Blocker).gameObject.SetActive(isEnabled);

    }
    private void Update()
    {
        UpdateStatus();
    }
    public void UpdateStatus()
    {
        if(_data != null)
        {
            if (_data.PurchaseItem.UnitPrice == 0)
            {
                BaseItem _item = Managers.Game.GetInventory().Find(x=>x.ItemId == _data.PurchaseItem.ItemId);
                if (_item != null)
                {
                    if(GetImage((int)Images.Blocker).gameObject.activeSelf == false)
                        SetEnableBlocker(true);
                    TimeSpan _diff = _item.Expiration.Value.ToLocalTime() - GlobalTime.Now.ToLocalTime();
                    GetText((int)Texts.BlockerText).text = _diff.ToString(@"hh\:mm\:ss");
                 
                }
                else
                {
                    SetEnableBlocker(false);
                }
            }
            else
            {
                SetEnableBlocker(false);
            }
        }

    }

    

}
