using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopPopup : UI_Popup
{
    enum Buttons
    {
        Close ,
        Exit,
        RandomBoxButton,
        PackageButton,
        RubyButton
    }
    enum GameObjects
    {
        IAPView ,
        IAPContent,
        ShopView,
        ShopContent,
        RubyView,
        RubyContent
    }
    public enum Panel
    {
        IAPView = 0,
        ShopView = 2,
        RubyView = 4
    }
    Panel _panel = Panel.ShopView;
    public void Setup(Panel panel)
    {
        _panel = panel;
    }
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));


        //20230803 프리팹에서 직접 입력하는것으로 수정
        /*
        foreach (ShopDate _item in Managers.Shop.Database.RandomBoxItems)
        {
            UI_ShopItem _shopItem = Util.GetOrAddComponent<UI_ShopItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopItem", Get<GameObject>((int)GameObjects.ShopContent).transform));
            _shopItem.Data = _item;
        }
        */

        foreach (IAPData _item in Managers.Shop.Database.IAPItems)
        {
            UI_IAPItem _iapItem = Util.GetOrAddComponent<UI_IAPItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_IAPItem", Get<GameObject>((int)GameObjects.IAPContent).transform));
            _iapItem.Data = _item;
        }

        foreach (IAPData _item in Managers.Shop.Database.RubyPackageItems)
        {
            UI_IAPItem _iapItem = Util.GetOrAddComponent<UI_IAPItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_IAPItem", Get<GameObject>((int)GameObjects.RubyContent).transform));
            _iapItem.Data = _item;
        }

        if (_panel == Panel.IAPView)
        {
            Get<GameObject>((int)GameObjects.RubyView).SetActive(false);
            Get<GameObject>((int)GameObjects.ShopView).SetActive(false);
        }
        else if (_panel == Panel.RubyView)
        {
            Get<GameObject>((int)GameObjects.ShopView).SetActive(false);
            Get<GameObject>((int)GameObjects.IAPView).SetActive(false);
        }
        else
        {
            Get<GameObject>((int)GameObjects.IAPView).SetActive(false);
            Get<GameObject>((int)GameObjects.RubyView).SetActive(false);
        }
           


        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });

        AddUIEvent(GetButton((int)Buttons.RandomBoxButton).gameObject, (data) => 
        { 
            Get<GameObject>((int)GameObjects.ShopView).SetActive(true);
            Get<GameObject>((int)GameObjects.IAPView).SetActive(false);
            Get<GameObject>((int)GameObjects.RubyView).SetActive(false);
            _panel = Panel.ShopView;
        });
        AddUIEvent(GetButton((int)Buttons.PackageButton).gameObject, (data) => 
        {
            Get<GameObject>((int)GameObjects.ShopView).SetActive(false);
            Get<GameObject>((int)GameObjects.IAPView).SetActive(true);
            Get<GameObject>((int)GameObjects.RubyView).SetActive(false);
            _panel = Panel.IAPView;
        });
        AddUIEvent(GetButton((int)Buttons.RubyButton).gameObject, (data) =>
        {
            Get<GameObject>((int)GameObjects.RubyView).SetActive(true);
            Get<GameObject>((int)GameObjects.IAPView).SetActive(false);
            Get<GameObject>((int)GameObjects.ShopView).SetActive(false);
            _panel = Panel.RubyView;
        });

    }
}
