using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_SeasonpassQuestItem : UI_Base
{
    enum Buttons
    {
        Button
    }
    enum Images
    {
        Blocker,
        LockImage,
        CompleteImage
    }
    enum Texts
    {
        Text
    }

    Quest _quest;
    public void Setup(Quest quest)
    {
        _quest = quest;
    }
    void Complete()
    {
        GetImage((int)Images.Blocker).gameObject.SetActive(true);
        GetImage((int)Images.CompleteImage).gameObject.SetActive(true);
        GetImage((int)Images.LockImage).gameObject.SetActive(false);
    }
    void Completable()
    {
        GetImage((int)Images.Blocker).gameObject.SetActive(false);
    }
    void Running()
    {
        if (_quest.CodeName.Contains("free"))
            GetImage((int)Images.Blocker).gameObject.SetActive(false);
        else 
        {
            if(_quest.IsCompletable)
            {
                GetImage((int)Images.Blocker).gameObject.SetActive(false);
            }
            else
            {
                GetImage((int)Images.Blocker).gameObject.SetActive(true);
                GetImage((int)Images.CompleteImage).gameObject.SetActive(false);
                GetImage((int)Images.LockImage).gameObject.SetActive(true);
            }
            /*
            if(!Managers.Game.GetInventory().IsFindItem("Seasonpass"))
            {
                GetImage((int)Images.Blocker).gameObject.SetActive(true);
                GetImage((int)Images.CompleteImage).gameObject.SetActive(false);
                GetImage((int)Images.LockImage).gameObject.SetActive(true);
            }
            else
                GetImage((int)Images.Blocker).gameObject.SetActive(false);
            */
        }
    }
    
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        Debug.Assert(_quest != null , "UI_SeasonpassQuestItem Quest item not Setup");
        
        if (_quest.IsComplete)
        {
            Complete();
        }
        else if(_quest.IsCompletable)
        {
            Completable();
        }
        else
        {
            Running();
        }
        
        GetText((int)Texts.Text).text = _quest.Description;

        _quest.onTaskSuccessChanged -= UpdateStatus;
        _quest.onTaskSuccessChanged += UpdateStatus;

        _quest.onCompleted -= UpdateStatus;
        _quest.onCompleted += UpdateStatus;

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            if (_quest.IsCompletable)
                _quest.Complete();
        });
    }
    void UpdateStatus(Quest quest , Task task, int currentSuccess, int prevSuccess)
    {
        if (_quest.IsComplete)
        {
            Complete();
        }
        else if (_quest.IsCompletable || quest.TaskGroups.All(x => x.Tasks.All(x => x.IsComplete))) // Quest 의 State 가 WaitingForCompletion 으로 반영되기 전에 호출되므로 일일이 검사
        {
            Completable();
        }
        else
        {
            Running();
        }
    }
    void UpdateStatus(Quest quest)
    {
        Complete();
    }
    private void OnDestroy()
    {
        _quest.onTaskSuccessChanged -= UpdateStatus;
        _quest.onCompleted -= UpdateStatus;
    }
}
