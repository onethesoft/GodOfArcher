using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_RandomboxPopup : UI_Popup
{
    List<BaseItem> _itemList;
    List<UI_BaseItem> _ui;
    ShopDate _shopData;

    public enum Status
    {
        Init,
        Animation,
        AnimationEnd,
        Touch

    }

    enum Buttons
    {
        Close,
        Exit,
        PurchaseButton_1,
        PurchaseButton_11,
        PurchaseButton_33,
        TouchButton
    }
    enum GameObjects
    {
        ItemPanel
    }
    enum Texts
    {
        PurchaseButton_1_PriceText,
        PurchaseButton_11_PriceText,
        PurchaseButton_33_PriceText
    }

    float _delayTime = 0.025f;
    float _startDelayTime = 0.15f;

    Status _status;
    public Status status 
    { 
        private set
        {
            _status = value;
            switch(_status)
            {
                case Status.Init:
                    if (GetButton((int)Buttons.TouchButton) != null)
                        if (GetButton((int)Buttons.TouchButton).gameObject.activeSelf == true)
                            GetButton((int)Buttons.TouchButton).gameObject.SetActive(false);
                    break;
                case Status.Animation:
                    if(GetButton((int)Buttons.TouchButton) != null)
                        if(GetButton((int)Buttons.TouchButton).gameObject.activeSelf == false)
                            GetButton((int)Buttons.TouchButton).gameObject.SetActive(true);
                        
                    break;
                case Status.AnimationEnd:
                    if (GetButton((int)Buttons.TouchButton) != null)
                        if (GetButton((int)Buttons.TouchButton).gameObject.activeSelf == true)
                            GetButton((int)Buttons.TouchButton).gameObject.SetActive(false);
                    break;
                case Status.Touch:
                    if (GetButton((int)Buttons.TouchButton) != null)
                        if (GetButton((int)Buttons.TouchButton).gameObject.activeSelf == true)
                            GetButton((int)Buttons.TouchButton).gameObject.SetActive(false);
                    break;
            }
        }
        get
        {
            return _status;
        }
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));


        status = Status.Init;

        

        UpdatePopup();
        /*
        _ui = new List<UI_BaseItem>();

        foreach (BaseItem item in _itemList)
        {
            if (item is Bundle)
                continue;

            for(int i=0;i<item.UsesIncrementedBy;i++)
            {
                UI_BaseItem _ui_base = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_BaseItem", Get<GameObject>((int)GameObjects.ItemPanel).transform));
                _ui_base.Item = item;

                _ui.Add(_ui_base);
            }
        }
        */



        if (_shopData != null)
        {
            GetText((int)Texts.PurchaseButton_1_PriceText).text = _shopData.BtnLists[1].Price.ToString();
            GetText((int)Texts.PurchaseButton_11_PriceText).text = _shopData.BtnLists[2].Price.ToString();
            GetText((int)Texts.PurchaseButton_33_PriceText).text = _shopData.BtnLists[3].Price.ToString();
        }

        SetButtonEnable(false);



        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            
            ClosePopupUI();
            UpdateGambleQuest();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            
            ClosePopupUI();
            UpdateGambleQuest();
        });

        AddUIEvent(GetButton((int)Buttons.PurchaseButton_1).gameObject, (data) => {
            if (_shopData.BtnLists[1].Price > Managers.Game.GetCurrency(_shopData.BtnLists[1].Currency))
                return;

            ClearItems();
            SetButtonEnable(false);
            Managers.Shop.BuyRandomBox(_shopData.BtnLists[1].PurchaseItem.ItemId);
        });
        AddUIEvent(GetButton((int)Buttons.PurchaseButton_11).gameObject, (data) => {
            if (_shopData.BtnLists[2].Price > Managers.Game.GetCurrency(_shopData.BtnLists[2].Currency))
                return;
            ClearItems();
            SetButtonEnable(false);
            Managers.Shop.BuyRandomBox(_shopData.BtnLists[2].PurchaseItem.ItemId);
        });
        AddUIEvent(GetButton((int)Buttons.PurchaseButton_33).gameObject, (data) => {
            if (_shopData.BtnLists[3].Price > Managers.Game.GetCurrency(_shopData.BtnLists[3].Currency))
                return;
            ClearItems();
            SetButtonEnable(false);
            Managers.Shop.BuyRandomBox(_shopData.BtnLists[3].PurchaseItem.ItemId);
        });

        AddUIEvent(GetButton((int)Buttons.TouchButton).gameObject, (data) => {
            if (status != Status.Touch)
                status = Status.Touch;
        });

    }
    


    public void UpdateGambleQuest()
    {
        // 구매한 아이템에 유물이면 업데이트할 퀘스트 x
        // 퀘스트 보상으로 주어진 아이템이면 _shopData 가 null 따라서 업데이트할 퀘스트 없음
        // 네트워크 연동확인
        if (_shopData == null)
            return;
        if (_shopData.ItemClass == "Heart")
            return;
        if (Managers.Network.IS_ENABLE_NETWORK == false)
            return;

        Managers.Quest.Save(PlayerInfo.UserDataKey.GambleQuest);
    }
    private int SortItem(BaseItem a , BaseItem b)
    {
        if (!(a is EquipableItem) || !(b is EquipableItem))
            throw new System.Exception();

        return (a as EquipableItem).Level.CompareTo((b as EquipableItem).Level);
    }
    public void Setup(List<BaseItem> itemList , ShopDate shopData)
    {
        _itemList = itemList;
        _shopData = shopData;
        _itemList.RemoveAll(x => x is Bundle);

        if(_itemList.First() is EquipableItem)
            _itemList.Sort(SortItem);

        
    }
    public void Setup(List<BaseItem> itemList)
    {
        _itemList = itemList;
        _shopData = null;

        _itemList.RemoveAll(x => x is Bundle);

        if (_itemList.First() is EquipableItem)
            _itemList.Sort(SortItem);


    }
    public void SetButtonEnable(bool active)
    {
        GetButton((int)Buttons.PurchaseButton_1).gameObject.SetActive(active);
        GetButton((int)Buttons.PurchaseButton_11).gameObject.SetActive(active);
        GetButton((int)Buttons.PurchaseButton_33).gameObject.SetActive(active);
    }
    public void ClearItems()
    {
        if(_ui != null)
        {
            foreach (UI_BaseItem _baseItem in _ui)
                Managers.Resource.Destroy(_baseItem.gameObject);
            _ui.Clear();
        }
    }
    public void UpdatePopup()
    {
        _status = Status.Animation;
        GetButton((int)Buttons.TouchButton).gameObject.SetActive(true);
        StartCoroutine(UpdateItems());
    }
    private IEnumerator UpdateItems()
    {
        if (_ui == null)
            _ui = new List<UI_BaseItem>();
        else
            ClearItems();

        

        // 아이템 ( 룬 ,펫 , 정수 , 심장 , 활 , 투구 , 헬멧 , 장갑) 만 출력한다.
        _itemList.RemoveAll(x => x is Bundle);
        if(status != Status.Touch)
            yield return new WaitForSeconds(_startDelayTime);

        

        foreach (BaseItem item in _itemList)
        {
            if (item is Bundle)
                continue;

            for (int i = 0; i < item.UsesIncrementedBy; i++)
            {
                UI_BaseItem _ui_base = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_BaseItem", Get<GameObject>((int)GameObjects.ItemPanel).transform));
                _ui_base.Item = item;

                _ui.Add(_ui_base);
                if (status != Status.Touch)
                    yield return new WaitForSeconds(_delayTime);
            }

           
        }

        status = Status.AnimationEnd;
       
        if (_shopData != null)
            SetButtonEnable(true);


    }
}
