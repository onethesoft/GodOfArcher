using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class UI_Seasonpass : UI_Popup
{
    public enum Mode
    {
        PASS ,
        PASS2
    }
    enum Buttons
    {
        Exit,
        LeftButton,
        RightButton,
        PurchaseSeasonpassButton,
        RubyPurchaseButton,
        AllCompleteButton
    }
    enum GameObjects
    {
        LeftPanel,
        RightPanel,
        PurchasePanel,
        PurchasePanelBlocker
    }
    enum Texts
    {
        TitleText,
        SubTitleText_0,
        SubTitleText_1,
        SubTitleText_2,
        SubTitleText_3,
        PurchaseText,
        PriceText
    }

    public Mode mode = Mode.PASS;

    List<Quest> _freeQuest;
    List<Quest> _passQuest;

    List<UI_SeasonpassItem> _seasonpassItemList;
    const int PageSize = 20;
    int PageIndex = 0;
    
   
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.TitleText).text = mode.ToString() + " 이벤트";

       
        List<Quest> _SeasonPassQuests = Managers.Quest.FindQuestByCategory(Define.QuestType.Seasonpass.ToString());
     
        
        if (mode == Mode.PASS)
        {
            GetText((int)Texts.SubTitleText_0).text = "무료 보상";
            GetText((int)Texts.SubTitleText_1).text = "PASS 보상";
            GetText((int)Texts.SubTitleText_2).text = "무료 보상";
            GetText((int)Texts.SubTitleText_3).text = "PASS 보상";

            _freeQuest = _SeasonPassQuests.Where(x => x.CodeName.Contains("free")).ToList();
            _passQuest = _SeasonPassQuests.Where(x => x.CodeName.Contains("pass_")).ToList();
        }
        else
        {
            GetText((int)Texts.SubTitleText_0).text = "PASS 보상";
            GetText((int)Texts.SubTitleText_1).text = "PASS 보상";
            GetText((int)Texts.SubTitleText_2).text = "PASS 보상";
            GetText((int)Texts.SubTitleText_3).text = "PASS 보상";

            _freeQuest = _SeasonPassQuests.Where(x => x.CodeName.Contains("pass2_")).ToList();
            _passQuest = _SeasonPassQuests.Where(x => x.CodeName.Contains("pass3_")).ToList();
        }
     

        _seasonpassItemList = new List<UI_SeasonpassItem>();

        for (int i=0;i<_freeQuest.Count;i++)
        {
            UI_SeasonpassItem _item;
            if ( i % PageSize < 10 )
                _item = Util.GetOrAddComponent<UI_SeasonpassItem>(Managers.Resource.Instantiate($"UI/SubItem/SeasonPass/UI_SessonpassItem", Get<GameObject>((int)GameObjects.LeftPanel).transform));
            else
                _item = Util.GetOrAddComponent<UI_SeasonpassItem>(Managers.Resource.Instantiate($"UI/SubItem/SeasonPass/UI_SessonpassItem", Get<GameObject>((int)GameObjects.RightPanel).transform));

            _item.Setup(Int32.Parse(_freeQuest[i].CodeName.Substring(_freeQuest[i].CodeName.LastIndexOf("_") + 1)), _freeQuest[i], _passQuest[i]);
            
            _seasonpassItemList.Add(_item);

            if (i >= PageSize)
                _seasonpassItemList[i].gameObject.SetActive(false);
            
          
        }

        
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.LeftButton).gameObject, (data) => {
            if (PageIndex > 0)
            {
                PageIndex--;
                UpdateItems();
            }
        });
        AddUIEvent(GetButton((int)Buttons.RightButton).gameObject, (data) => {
            if (PageIndex < 2)
            {
                PageIndex++;
                UpdateItems();
            }
        });


        UpdatePurchasePanel();
        AddUIEvent(GetButton((int)Buttons.PurchaseSeasonpassButton).gameObject, (data) => {

            if (mode == Mode.PASS)
            {
             
                Managers.Shop.BuyProductId(Managers.Shop.Database.Seasonpass.Item.ItemId);
          
                UpdatePurchasePanel();
            }
            else
            {
                if (!Managers.Game.GetInventory().IsFindItem(Managers.Shop.Database.Seasonpass.Item.ItemId))
                {
                    UI_Messagebox _popup = Managers.UI.ShowPopupUI<UI_Messagebox>();
                    _popup.mode = UI_Messagebox.Mode.OK;
                    _popup.Title = "시즌패스";
                    _popup.Text = "Pass 이벤트를 먼저 구매해주세요";
                    return;
                }
                

                Managers.Shop.BuyProductId(Managers.Shop.Database.Seasonpass2.Item.ItemId);
           
                UpdatePurchasePanel();
            }

            GetButton((int)Buttons.PurchaseSeasonpassButton).gameObject.SetActive(false);


        });

        AddUIEvent(GetButton((int)Buttons.RubyPurchaseButton).gameObject, (data) =>
        {
            ClosePopupUI();
            Managers.UI.ShowPopupUI<UI_ShopPopup>().Setup(UI_ShopPopup.Panel.RubyView);
            
        });

        AddUIEvent(GetButton((int)Buttons.AllCompleteButton).gameObject, (data) =>
        {
            _freeQuest.ForEach(x => {if (x.IsCompletable)
                    x.Complete(); 
            });

            _passQuest.ForEach(x => {
                if (x.IsCompletable)
                    x.Complete();
            });

        });

    }

    public void UpdateItems()
    {
        for (int i = 0; i < _freeQuest.Count; i++)
        {
            if (i >= PageIndex * PageSize && i < (PageIndex * PageSize) + PageSize)
            {
                _seasonpassItemList[i].gameObject.SetActive(true);
            }
            else
            {
                _seasonpassItemList[i].gameObject.SetActive(false);
            }

        }
    }
    public void UpdatePurchasePanel()
    {
        string ItemId;
        if (mode == Mode.PASS)
        {
            
            GetText((int)Texts.PriceText).text = Managers.Shop.GetPrice(Managers.Shop.Database.Seasonpass.Item.ItemId);
            GetText((int)Texts.PurchaseText).text = "총 스테이지 클리어 시 약 100만 루비 획득";
            ItemId = Managers.Shop.Database.Seasonpass.Item.ItemId;
        }
        else
        {
            GetText((int)Texts.PriceText).text = Managers.Shop.GetPrice(Managers.Shop.Database.Seasonpass2.Item.ItemId);
            GetText((int)Texts.PurchaseText).text = "총 스테이지 클리어 시 약 650만 루비 획득";
            ItemId = Managers.Shop.Database.Seasonpass2.Item.ItemId;
        }


        if (Managers.Game.GetInventory().IsFindItem(x => x.ItemId == ItemId))
            SetActivePurchasePanel(false);
        else
            SetActivePurchasePanel(true);
    }

    public void SetActivePurchasePanel(bool isActive)
    {
        if (Get<GameObject>((int)GameObjects.PurchasePanelBlocker) != null)
            Get<GameObject>((int)GameObjects.PurchasePanelBlocker).SetActive(!isActive);


        if (Get<GameObject>((int)GameObjects.PurchasePanel) != null)
            Get<GameObject>((int)GameObjects.PurchasePanel).SetActive(isActive);
        
       
    }
    public override void ClosePopupUI()
    {
        base.ClosePopupUI();

    }

}
