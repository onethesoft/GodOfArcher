using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_GambleQuestItem : UI_Base
{
    enum Images
    {
        Icon,
        Blocker,
        RewardImage
    }
    enum Texts
    {
        DisplayName,
        DescriptionText,
        RewardAmount,
        RewardDescription
    }
    enum Buttons
    {
        RewardButton
    }

    Quest _quest;
    public Quest Quest => _quest;
    public void SetQuest(Quest quest)
    {
        _quest = quest;
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Debug.Assert(_quest != null, $"UI_GambleQuestItem : _quest is null");

        

        GetImage((int)Images.Icon).sprite = _quest.Icon;
        GetText((int)Texts.DisplayName).text = _quest.DisplayName;
        GetText((int)Texts.DescriptionText).text = $"({_quest.TaskGroups.First().Tasks.First().CurrentSuccess}/{_quest.Description})";

        GetButton((int)Buttons.RewardButton).GetComponent<Image>().sprite = _quest.Rewards.First().BackgroundIcon;
        GetImage((int)Images.Blocker).sprite = _quest.Rewards.First().BackgroundIcon;


        if (_quest.Rewards.First().Icon != null)
            GetImage((int)Images.RewardImage).sprite = _quest.Rewards.First().Icon;
        else
            GetImage((int)Images.RewardImage).gameObject.SetActive(false);

        if(_quest.Rewards.First() is CurrencyReward)
        {
            GetText((int)Texts.RewardAmount).text = _quest.Rewards.First().Quantity.ToString();
            GetText((int)Texts.RewardDescription).gameObject.SetActive(false);
        }
        else
        {
            GetText((int)Texts.RewardAmount).gameObject.SetActive(false);
            GetText((int)Texts.RewardDescription).text = _quest.Rewards.First().Description;
        }

        if (_quest.IsComplete)
            GetImage((int)Images.Blocker).gameObject.SetActive(true);
        else
            GetImage((int)Images.Blocker).gameObject.SetActive(false);

        _quest.onCompleted -= OnComplete;
        _quest.onCompleted += OnComplete;

        AddUIEvent(GetButton((int)Buttons.RewardButton).gameObject, (data) => {
            if(_quest.IsCompletable)
                _quest.Complete();
        });


    }
    public void OnComplete(Quest quest)
    {
        if(quest == _quest)
            GetImage((int)Images.Blocker).gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        _quest.onCompleted -= OnComplete;
    }

}
