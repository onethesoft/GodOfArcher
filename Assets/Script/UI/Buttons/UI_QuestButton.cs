using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UI_QuestButton : UI_Base
{

    [SerializeField]
    Define.QuestType _questType;

    enum Texts
    {
        Description,
        RewardText
    }
    enum Images
    {
        RewardImage
    }

    Quest _quest = null;

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));


      
        Managers.Quest.onQuestReset -= OnResetQuest;
        Managers.Quest.onQuestReset += OnResetQuest;

      
        if (FindObjectOfType<GameData>().IsAllCompletedQuest(_questType))
        {
            gameObject.SetActive(false);
        }
        else
        {
            _quest = Managers.Quest.ActiveQuests.Where(x => x.Category == _questType.ToString()).FirstOrDefault();
            UpdateText(_quest);
            UpdateImage(_quest);

            _quest.onTaskSuccessChanged -= UpdateProgressText;
            _quest.onTaskSuccessChanged += UpdateProgressText;
        }
        

        Managers.Quest.onQuestCompleted -= CompleteQuest;
        Managers.Quest.onQuestCompleted += CompleteQuest;


      


        AddUIEvent(gameObject, (data) => {

            if (_quest == null)
                return;

            if (_quest.IsCompletable)
            {
                _quest.Complete();
            }
            else
            {
                _quest.DoHelp();
            }


            
        });
    }

    void OnResetQuest(string category)
    {
        if(_questType.ToString() == category)
        {
            if(_quest != null)
                _quest.onTaskSuccessChanged -= UpdateProgressText;

            _quest = Managers.Quest.ActiveQuests.Where(x => x.Category == _questType.ToString()).FirstOrDefault();

            UpdateText(_quest);
            UpdateImage(_quest);

            _quest.onTaskSuccessChanged -= UpdateProgressText;
            _quest.onTaskSuccessChanged += UpdateProgressText;

            Managers.Quest.onQuestCompleted -= CompleteQuest;
            Managers.Quest.onQuestCompleted += CompleteQuest;


            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);
        }
    }
    void UpdateProgressText(Quest quest , Task task , int currentSuccess , int prevSuccess)
    {
        if(_quest == quest)
            GetText((int)Texts.Description).text = $"{quest.Description}[{quest.CurrentTaskGroup.Tasks.First().CurrentSuccess}/{quest.CurrentTaskGroup.Tasks.First().NeedSuccessToComplete}]";

       
    }
    void UpdateText(Quest quest)
    {
        GetText((int)Texts.Description).text = $"{quest.Description}[{quest.CurrentTaskGroup.Tasks.First().CurrentSuccess}/{quest.CurrentTaskGroup.Tasks.First().NeedSuccessToComplete}]";
        GetText((int)Texts.RewardText).text = $"x{quest.Rewards.First().Quantity}";
    }
    void UpdateImage(Quest quest)
    {
        GetImage((int)Images.RewardImage).sprite = quest.Rewards.First().Icon;
    }
    void CompleteQuest(Quest quest)
    {

        if (_quest != quest)
        {
            return;
        }

        if (_questType == Define.QuestType.Daily)
            FindObjectOfType<GameData>().DailyQuestLastClearTime = GlobalTime.Now;

        _quest.onTaskSuccessChanged -= UpdateProgressText;
        _quest = Managers.Quest.ActiveQuests.Where(x => x.Category == _questType.ToString()).FirstOrDefault();
        if (_quest == null)
        {
            Managers.Quest.onQuestCompleted -= CompleteQuest;
            gameObject.SetActive(false);
            return;
        }
        else
        {
            _quest.onTaskSuccessChanged += UpdateProgressText;
            UpdateText(_quest);
            UpdateImage(_quest);
        }
        
    }
    
    
    private void OnDestroy()
    {
        Managers.Quest.onQuestReset -= OnResetQuest;
        Managers.Quest.onQuestCompleted -= CompleteQuest;
        if(_quest != null)
            _quest.onTaskSuccessChanged -= UpdateProgressText;
    }




}
