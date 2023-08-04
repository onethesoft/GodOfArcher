using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_IAPItem : UI_Base
{
    IAPData _iapdata;
    public IAPData Data { set { _iapdata = value; } }

    enum Texts
    {
        DisplayName,
        CurrencyText,
        Description,
        PriceText
    }
    enum Images
    {
        CurrencyIcon,
        Icon,
        SubIcon,
        Blocker

    }
    enum Buttons
    {
        Button
    }

    enum GameObjects
    {
        ItemIconPos_0,
        ItemIconPos_1,
        ItemIconPos_2,
        ItemIconPos_3,
        RuneAndPetIconPos_0,
        RuneAndPetIconPos_1,
        ResourceIconPanel
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        if(_iapdata.Item.DailyContents.Count <= 0 && _iapdata.Item.DailyCurrencies.Count <= 0)
            Get<GameObject>((int)GameObjects.ResourceIconPanel).SetActive(false);

        GetText((int)Texts.DisplayName).text = _iapdata.DisplayName;
        GetText((int)Texts.Description).text = _iapdata.Description.Replace("\\n",System.Environment.NewLine);
        GetText((int)Texts.Description).color = _iapdata.DescriptionColor;
        if (_iapdata.Icon != null)
            GetImage((int)Images.Icon).sprite = _iapdata.Icon;
        else
            GetImage((int)Images.Icon).gameObject.SetActive(false);


        if (_iapdata.SubIcon != null)
            GetImage((int)Images.SubIcon).sprite = _iapdata.SubIcon;
        else
            GetImage((int)Images.SubIcon).gameObject.SetActive(false);

        if(_iapdata.Item != null)
        {
            GetText((int)Texts.CurrencyText).text = $"·çºñ x {_iapdata.Item.Currencies[0].Amount}";
        }

        if(_iapdata.ItemIcons != null)
        {
            if(_iapdata.ItemIcons.Count > 2)
            {
                for(int i=0; i< _iapdata.ItemIcons.Count;i++)
                {
                    string GameObjectId = $"ItemIconPos_{i}";
                    GameObjects Pos = (GameObjects)System.Enum.Parse(typeof(GameObjects), GameObjectId);
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopBaseItem_Small", Get<GameObject>((int)Pos).transform));
                    _item.Item = _iapdata.ItemIcons[i];
                    _item.Setup(_iapdata.ItemIcons[i], UI_BaseItem.Mode.Shop);
                }
            }
            else
            {
                for (int i = 0; i < _iapdata.ItemIcons.Count; i++)
                {
                    string GameObjectId = $"RuneAndPetIconPos_{i}";
                    GameObjects Pos = (GameObjects)System.Enum.Parse(typeof(GameObjects), GameObjectId);
                    GameObject _itemIcon = Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopBaseItem_Middle", Get<GameObject>((int)Pos).transform);
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(_itemIcon);
                    _item.Item = _iapdata.ItemIcons[i];
                    _item.Setup(_iapdata.ItemIcons[i], UI_BaseItem.Mode.Shop);
                    _itemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }
            }
        }


        Managers.Game.GetInventory().OnItemChanged -= UpdateStatus;
        Managers.Game.GetInventory().OnItemChanged += UpdateStatus;


        UpdateStatus();
        if (string.IsNullOrEmpty(Managers.Shop.GetPrice(_iapdata.Item.ItemId)) == false)
            GetText((int)Texts.PriceText).text = Managers.Shop.GetPrice(_iapdata.Item.ItemId);


        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            Managers.Shop.BuyProductId(_iapdata.Item.ItemId);
        });
        

    }
    IEnumerator PurchaseIAPItem()
    {
        yield return new WaitForSeconds(0.2f);
        Managers.Item.GrantItemToUser(_iapdata.Item.ItemId);
        
    }
    void UpdateStatus()
    {
        if(_iapdata.type == IAPData.Type.Ruby)
        {
            if (Managers.Game.GetInventory().IsFindItem(_iapdata.Item.ItemId))
            {
                GetText((int)Texts.Description).gameObject.SetActive(false);
            }

        }

        if (_iapdata.IsBlock == true && Managers.Game.GetInventory().IsFindItem(x => x.ItemId == _iapdata.Item.ItemId) == true)
        {
            if (GetImage((int)Images.Blocker).gameObject.activeSelf == false)
                GetImage((int)Images.Blocker).gameObject.SetActive(true);
        }
        else
            GetImage((int)Images.Blocker).gameObject.SetActive(false);



    }
    private void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateStatus;
    }
}
