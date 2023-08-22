using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_GambleQuest : UI_Popup
{
    Define.QuestType _type;
    List<UI_GambleQuestItem> _itemList;
    string[] TitleTexts = new string[] { "∑È,∆Í ªÃ±‚ ∑π∫ß ∫∏ªÛ", "¿Â∫Ò ªÃ±‚ ∑π∫ß ∫∏ªÛ"};
    enum GameObjects
    {
        RuneAndPet,
        Other,
        QuestPanel
    }
    
    enum Buttons
    {
        RuneButton,
        PetButton,
        BowButton,
        HelmetButton,
        ArmorButton,
        CloakButton,
        Close,
        Exit

    }
    enum Texts
    {
        TitleText
    }
    public void SetGambleQuestType(Define.QuestType type)
    {
        _type = type;
    }
   
    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        if (_type == Define.QuestType.GambleRune || _type == Define.QuestType.GamblePet)
        {
            Get<GameObject>((int)GameObjects.Other).SetActive(false);
            GetText((int)Texts.TitleText).text = TitleTexts[(int)(int)GameObjects.RuneAndPet];
        }
        else if (_type == Define.QuestType.GambleBow || _type == Define.QuestType.GambleArmor || _type == Define.QuestType.GambleHelmet || _type == Define.QuestType.GambleCloak)
        {
            Get<GameObject>((int)GameObjects.RuneAndPet).SetActive(false);
            GetText((int)Texts.TitleText).text = TitleTexts[(int)(int)GameObjects.Other];
        }
        else
            Debug.LogError("QuestType is not GambleQuest");

        _itemList = new List<UI_GambleQuestItem>();
        foreach(Define.QuestType questType in Enum.GetValues(typeof(Define.QuestType)))
        {
            if (!questType.ToString().Contains("Gamble"))
                continue;

            CreateItems(questType);
        }

        Managers.Quest.onQuestCompleted -= OnQuestComplete;
        Managers.Quest.onQuestCompleted += OnQuestComplete;

        AddUIEvent(GetButton((int)Buttons.RuneButton).gameObject, (data) => {
            _type = Define.QuestType.GambleRune;
            SetActiveItems(_type);
        });
        AddUIEvent(GetButton((int)Buttons.PetButton).gameObject, (data) => {
            _type = Define.QuestType.GamblePet;
            SetActiveItems(_type);
        });
        AddUIEvent(GetButton((int)Buttons.BowButton).gameObject, (data) => {
            _type = Define.QuestType.GambleBow;
            SetActiveItems(_type);
        });
        AddUIEvent(GetButton((int)Buttons.HelmetButton).gameObject, (data) => {
            _type = Define.QuestType.GambleHelmet;
            SetActiveItems(_type);
        });
        AddUIEvent(GetButton((int)Buttons.ArmorButton).gameObject, (data) => {
            _type = Define.QuestType.GambleArmor;
            SetActiveItems(_type);
        });
        AddUIEvent(GetButton((int)Buttons.CloakButton).gameObject, (data) => {
            _type = Define.QuestType.GambleCloak;
            SetActiveItems(_type);
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
         
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
           
            ClosePopupUI();
        });



    }
    void OnQuestComplete(Quest quest)
    {
        if(quest.Category == _type.ToString())
        {
            if (!Managers.Quest.ActiveQuests.Any(x => x.Category == _type.ToString()))
            {
                Invoke("ResetQuest", 0.1f);
            }
        }
        
    }
    void ResetQuest()
    {
        Managers.Quest.ResetQuests(new string[] { _type.ToString() });
        
    }
    public void CreateItems(Define.QuestType category)
    {
        foreach (Quest quest in Managers.Quest.FindQuestByCategory(category.ToString()))
        {
            UI_GambleQuestItem _item = Util.GetOrAddComponent<UI_GambleQuestItem>(Managers.Resource.Instantiate("UI/SubItem/GambleQuestPopup/UI_GambleQuestItem", Get<GameObject>((int)GameObjects.QuestPanel).transform));
            _item.SetQuest(quest);
            _itemList.Add(_item);

            if (quest.Category != _type.ToString())
                _item.gameObject.SetActive(false);
        }
    }
    public void SetActiveItems(Define.QuestType category)
    {
        foreach(UI_GambleQuestItem item in _itemList)
        {
            if (item.Quest.Category == category.ToString())
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }

    public override void ClosePopupUI()
    {
        Managers.Quest.onQuestCompleted -= OnQuestComplete;
        base.ClosePopupUI();

    }

}
