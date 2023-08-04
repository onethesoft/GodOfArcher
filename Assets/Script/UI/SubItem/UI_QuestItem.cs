using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestItem : UI_Base
{
    public string DescriptionText
    {
        set
        {
            if(GetText((int)Texts.DescriptionText) != null)
                GetText((int)Texts.DescriptionText).text = value;
            
        }
    }
    public string CurrentSuccessText
    {
        set
        {
            if (GetText((int)Texts.CurrentSuccessText) != null)
                GetText((int)Texts.CurrentSuccessText).text = value;
            
        }
    }
    public string RewardText
    {
        set
        {
            if (GetText((int)Texts.RewardText) != null)
                GetText((int)Texts.RewardText).text = value;
            
        }
    }

    public Sprite Icon
    {
        set
        {
            if (GetImage((int)Images.Icon) != null)
                if (GetImage((int)Images.Icon).sprite = value) ;
        }
    }
    enum Texts
    {
        DescriptionText,
        CurrentSuccessText,
        RewardText
    }
    enum Images
    {
        Icon,
        Blocker
    }
    enum Buttons
    {
        ReceiveButton
    }


  
    Define.QuestType _type;
    
    public void Setup(Define.QuestType type)
    {
        _type = type;

    }
    Quest _targetQuest;
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        
        _targetQuest = Managers.Quest.ActiveQuests.Where(x => x.Category == _type.ToString()).FirstOrDefault();
        if(_targetQuest == null)
        {
            GetImage((int)Images.Blocker).gameObject.SetActive(true);
            _targetQuest = Managers.Quest.CompletedQuests.Where(x => x.Category == _type.ToString()).Last();
            Setup(_targetQuest);
        }
        else
        {
            GetImage((int)Images.Blocker).gameObject.SetActive(false);
            Setup(_targetQuest);
        }

        Managers.Quest.onQuestCompleted -= UpdateQuest;
        Managers.Quest.onQuestCompleted += UpdateQuest;


        AddUIEvent(GetButton((int)Buttons.ReceiveButton).gameObject, (data) => {
            if (!_targetQuest.IsCompletable)
                return;

            List<Quest> _listCompletedQuest = ProcessCompleteQuests();
            if(_listCompletedQuest != null)
            {
                List<Reward> _rewards = new List<Reward>();
                
                foreach (IReadOnlyList<Reward> Reward in _listCompletedQuest.Select(x=>x.Rewards))
                    _rewards.AddRange(Reward);
                
                UI_QuestComplete _rewardPopup = Managers.UI.ShowPopupUI<UI_QuestComplete>();
                _rewardPopup.SetRewardList(_rewards);
                
            }


        });

    }
    public bool IsCompletable()
    {
        Debug.Assert(_targetQuest != null, "TargetQuest is Null");
        return _targetQuest.IsCompletable;
    }
    public void Setup(Quest quest)
    {
        GetImage((int)Images.Icon).sprite = quest.Icon;
        GetText((int)Texts.DescriptionText).text = quest.Description;
        GetText((int)Texts.CurrentSuccessText).text = $"{quest.CurrentTaskGroup.Tasks.First().CurrentSuccess}/{quest.CurrentTaskGroup.Tasks.First().NeedSuccessToComplete}";
        GetText((int)Texts.RewardText).text = $"{quest.Rewards[0].Description}";

    }
    
    public List<Quest> ProcessCompleteQuests()
    {
        if (!_targetQuest.IsCompletable)
            return null;

        List<Quest> _listCompleteQuests = new List<Quest>();
        while (_targetQuest.IsCompletable)
        {
            _listCompleteQuests.Add(_targetQuest);
            _targetQuest.Complete();
          
        }
        return _listCompleteQuests;

        /*
        List<Reward> _listReward = new List<Reward>();
        while (_targetQuest.IsCompletable)
        {
            _listReward.AddRange(_targetQuest.Rewards.ToList());
            _targetQuest.Complete();
        }

        return _listReward;
        */

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void UpdateQuest(Quest quest)
    {
       
        if(_targetQuest == quest)
        {

            if (Managers.Quest.ActiveQuests.Any(x => x.Category == _type.ToString()))
            {
                _targetQuest = Managers.Quest.ActiveQuests.Where(x => x.Category == _type.ToString()).First();
                Setup(_targetQuest);
            }
            else
                GetImage((int)Images.Blocker).gameObject.SetActive(true);
        }
    }
    
    private void Update()
    {
        GetText((int)Texts.CurrentSuccessText).text = $"{_targetQuest.CurrentTaskGroup.Tasks.First().CurrentSuccess}/{_targetQuest.CurrentTaskGroup.Tasks.First().NeedSuccessToComplete}";
    }
    private void OnDestroy()
    {
        Managers.Quest.onQuestCompleted -= UpdateQuest;
    }


}
