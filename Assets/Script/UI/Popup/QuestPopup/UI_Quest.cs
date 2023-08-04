using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UI_Quest : UI_Popup
{
    enum Texts
    {
        HeaderText ,
        SubHeaderText
    }
    enum Buttons
    {
        Close ,
        Exit ,
        AllReceiveButton
    }
    enum GameObjects
    {
        QuestItemPanel
    }

    List<UI_QuestItem> _items;
    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        _items = new List<UI_QuestItem>();
        UI_QuestItem _item = Util.GetOrAddComponent<UI_QuestItem>(Managers.Resource.Instantiate("UI/SubItem/QuestPopup/UI_QuestItem", Get<GameObject>((int)GameObjects.QuestItemPanel).transform));
        _item.Setup(Define.QuestType.StageClear);
        _items.Add(_item);

        _item = Util.GetOrAddComponent<UI_QuestItem>(Managers.Resource.Instantiate("UI/SubItem/QuestPopup/UI_QuestItem", Get<GameObject>((int)GameObjects.QuestItemPanel).transform));
        _item.Setup(Define.QuestType.BossMonsterKill);
        _items.Add(_item);

        _item = Util.GetOrAddComponent<UI_QuestItem>(Managers.Resource.Instantiate("UI/SubItem/QuestPopup/UI_QuestItem", Get<GameObject>((int)GameObjects.QuestItemPanel).transform));
        _item.Setup(Define.QuestType.MonsterKill);
        _items.Add(_item);

        _item = Util.GetOrAddComponent<UI_QuestItem>(Managers.Resource.Instantiate("UI/SubItem/QuestPopup/UI_QuestItem", Get<GameObject>((int)GameObjects.QuestItemPanel).transform));
        _item.Setup(Define.QuestType.PlayTime);
        _items.Add(_item);

        _item = Util.GetOrAddComponent<UI_QuestItem>(Managers.Resource.Instantiate("UI/SubItem/QuestPopup/UI_QuestItem", Get<GameObject>((int)GameObjects.QuestItemPanel).transform));
        _item.Setup(Define.QuestType.TrollStageClear);
        _items.Add(_item);




        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
           

        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
           
        });
        AddUIEvent(GetButton((int)Buttons.AllReceiveButton).gameObject, (data) => {
            List<Reward> _rewards = new List<Reward>();
            foreach (UI_QuestItem _item in _items)
            {
                if(_item.IsCompletable())
                {
                    List<Quest> _completedQuests = _item.ProcessCompleteQuests();
                    foreach (IReadOnlyList<Reward> Reward in _completedQuests.Select(x => x.Rewards))
                        _rewards.AddRange(Reward);
                }
            }

            if (_rewards.Count <= 0)
                return;

            UI_QuestComplete _rewardPopup = Managers.UI.ShowPopupUI<UI_QuestComplete>();
            _rewardPopup.SetRewardList(_rewards);
            
        });



    }
    public void LoadQuestItem()
    {
       


    }
}
