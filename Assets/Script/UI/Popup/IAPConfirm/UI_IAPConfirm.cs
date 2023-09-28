using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_IAPConfirm : UI_Popup
{
    public Action OnOK;
    enum Buttons
    {
        OK,
    }
    enum Texts
    {
        DescriptionText,
        TitleText
    }
    enum GameObjects
    {
        ItemIconPanel
    }
    string _titleText;
    public string TitleText
    {
        get
        {
            return _titleText;
        }
        set
        {
            _titleText = value;
        }
    }
    string _descriptionText;
    public string DescriptionText
    {
        get
        {
            return _descriptionText;
        }
        set
        {
            _descriptionText = value;
        }
    }

    IAPData _data;
    List<BaseItem> _itemIds;
    public void SetIAPData(IAPData data)
    {
        _data = data;
    }
    public void SetItems(List<BaseItem> itemIds )
    {
        _itemIds = itemIds;
    }
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetText((int)Texts.TitleText).text = _titleText;
        GetText((int)Texts.DescriptionText).text = _descriptionText;


        if (_data != null)
        {
            if (_data.ItemIcons.Count == 2)
            {
                Get<GameObject>((int)GameObjects.ItemIconPanel).GetComponent<GridLayoutGroup>().cellSize = new Vector2(150, 150);
                foreach (BaseItem item in _data.ItemIcons)
                {
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopBaseItem_Big", Get<GameObject>((int)GameObjects.ItemIconPanel).transform));
                    _item.Setup(item, UI_BaseItem.Mode.ShopPopup);

                }
            }
            else if (_data.ItemIcons.Count == 4)
            {
                Get<GameObject>((int)GameObjects.ItemIconPanel).GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
                foreach (BaseItem item in _data.ItemIcons)
                {
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopBaseItem_Middle", Get<GameObject>((int)GameObjects.ItemIconPanel).transform));
                    _item.Setup(item, UI_BaseItem.Mode.ShopPopup);
                }
            }
        }
        else if (_itemIds != null)
        {
            if (_itemIds.Count <= 2)
            {
                Get<GameObject>((int)GameObjects.ItemIconPanel).GetComponent<GridLayoutGroup>().cellSize = new Vector2(150, 150);
                foreach (BaseItem item in _itemIds)
                {
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopBaseItem_Big", Get<GameObject>((int)GameObjects.ItemIconPanel).transform));
                    _item.Setup(item, UI_BaseItem.Mode.ShopPopup);
                }
            }
            else if (_itemIds.Count <= 4)
            {
                Get<GameObject>((int)GameObjects.ItemIconPanel).GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
                foreach (BaseItem item in _itemIds)
                {
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopBaseItem_Middle", Get<GameObject>((int)GameObjects.ItemIconPanel).transform));
                    _item.Setup(item, UI_BaseItem.Mode.ShopPopup);
                }
            }
            
        }
       

        AddUIEvent(GetButton((int)Buttons.OK).gameObject, (data) => {
            OnOK?.Invoke();
            ClosePopupUI();
        });

       



    }

}
