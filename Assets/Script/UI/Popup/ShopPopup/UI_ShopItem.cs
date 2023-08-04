using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UI_ShopItem : UI_Base
{
    ShopDate _shopdata;
    public ShopDate Data { set { _shopdata = value; } }

    Quest _quest;
    enum GameObjects
    {
        ShopButtonParent,
        UI_QuestProgressBar
    }
    enum Texts
    {
        DisplayName
    }

    enum Images
    {
        Icon
    }
    enum Buttons
    {
        ShowRandomboxTableButton
    }
   
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
       

        if (_shopdata != null)
        {
            GetText((int)Texts.DisplayName).text = _shopdata.DisplayName;
            GetImage((int)Images.Icon).sprite = _shopdata.Icon;

            if (_shopdata.CodeName == "Shop_Artifact")
            {
                GetButton((int)Buttons.ShowRandomboxTableButton).gameObject.SetActive(false);
                Get<GameObject>((int)GameObjects.UI_QuestProgressBar).SetActive(false);
            }
            else
            {
                Define.QuestType _questType = (Define.QuestType)System.Enum.Parse(typeof(Define.QuestType), $"Gamble{_shopdata.ItemClass}");
                _quest = Managers.Quest.ActiveQuests.Where(x => x.Category == _questType.ToString()).FirstOrDefault();
                if(_quest == null)
                {
                    Util.GetOrAddComponent<UI_QuestProgressBar>(Get<GameObject>((int)GameObjects.UI_QuestProgressBar)).SetQuest(Managers.Quest.FindQuestByCategory(_questType.ToString()).Last());
                    //Debug.Log(Managers.Quest.FindQuestByCategory(_questType.ToString()).Last().CodeName);
                }
                else
                {
                    Util.GetOrAddComponent<UI_QuestProgressBar>(Get<GameObject>((int)GameObjects.UI_QuestProgressBar)).SetQuest(_quest);
                    _quest.onCompleted -= UpdateProgress;
                    _quest.onCompleted += UpdateProgress;

                }

                AddUIEvent(GetButton((int)Buttons.ShowRandomboxTableButton).gameObject, (data) => {
                    if(_shopdata.ItemClass == "Rune" || _shopdata.ItemClass == "Pet")
                        Managers.UI.ShowPopupUI<UI_RandomboxTable>().SetMode(UI_RandomboxTable.Mode.RuneAndPet);
                    else
                        Managers.UI.ShowPopupUI<UI_RandomboxTable>().SetMode(UI_RandomboxTable.Mode.Other);
                });


                //Managers.Quest.ActiveQuests.Where(x=>x.Category == QuestCategory).First().TaskGroups.First().Tasks.First().CurrentSuccess

            }


            foreach(ShopButtonData _data in _shopdata.BtnLists)
            {
                UI_ShopButton _btn = Util.GetOrAddComponent<UI_ShopButton>(Managers.Resource.Instantiate("UI/SubItem/ShopPopup/UI_ShopButton" , Get<GameObject>((int)GameObjects.ShopButtonParent).transform));
                _btn.Setup(_data);
                /*
                _btn.Text = _data.ButtonText;
                _btn.Icon = _data.Icon;
                if (_data.PurchaseItem != null)
                {

                    if (_data.PurchaseItem.UnitPrice == 0)
                    {
                        _btn.PriceText = "FREE";
                        if (Managers.Game.GetInventory().IsFindItem(_data.PurchaseItem.ItemId))
                        {
                            _btn.SetBlocker = true;
                        }
                        else
                            _btn.SetBlocker = false;
                        
                    }
                    else
                    {
                        _btn.PriceText = _data.PurchaseItem.UnitPrice.ToString();
                        _btn.SetBlocker = false;
                    }

                    AddUIEvent(_btn.gameObject.GetComponent<Button>().gameObject, (data) => 
                    {
                        if (Managers.Game.GetCurrency(_data.Currency) < _data.Price)
                            return;

                        Managers.Shop.BuyRandomBox(_data.PurchaseItem.ItemId);

                        //Managers.Game.SubstractCurrency(_data.Currency, _data.Price);
                        //List<BaseItem> _grants = Managers.Item.GrantItemToUser(_data.PurchaseItem.ItemId);
                        //Managers.UI.ShowPopupUI<UI_RandomboxPopup>().Setup(_grants, _shopdata);

                     
                        
                    });
                }
              */


            }

            
        }
    }

    void UpdateProgress(Quest quest)
    {
        if(_quest == quest)
        {
            _quest.onCompleted -= UpdateProgress;
            Define.QuestType _questType = (Define.QuestType)System.Enum.Parse(typeof(Define.QuestType), $"Gamble{_shopdata.ItemClass}");
            _quest = Managers.Quest.ActiveQuests.Where(x => x.Category == _questType.ToString()).FirstOrDefault();
            _quest.onCompleted -= UpdateProgress;
            _quest.onCompleted += UpdateProgress;

            if (_quest == null)
            {
                Util.GetOrAddComponent<UI_QuestProgressBar>(Get<GameObject>((int)GameObjects.UI_QuestProgressBar)).SetQuest(Managers.Quest.CompletedQuests.Last(x => x.Category == _questType.ToString()));

            }
            else
            {
                Util.GetOrAddComponent<UI_QuestProgressBar>(Get<GameObject>((int)GameObjects.UI_QuestProgressBar)).SetQuest(_quest);

            }
        }

    }

    private void OnDestroy()
    {
        if (_quest != null)
            _quest.onCompleted -= UpdateProgress;
    }
}
