using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestProgressBar : UI_Base
{
    Quest _quest = null;
    enum Images
    {
        LeftImage
    }
    enum Texts
    {
        Text
    }
    enum Buttons
    {
        Button
    }
    enum Sliders
    {
        Slider
    }
    public void SetQuest(Quest quest)
    {
      
        if(_quest != null)
            _quest.onTaskSuccessChanged -= UpdateProgress;

        _quest = quest;
        UpdateText();
        if(Get<Slider>((int)Sliders.Slider) != null)
            Get<Slider>((int)Sliders.Slider).value = CalculateProgress();
        UpdateImage();

        _quest.onTaskSuccessChanged -= UpdateProgress;
        _quest.onTaskSuccessChanged += UpdateProgress;

    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));

        Debug.Assert(_quest != null, "UI_QuestProgressBar : _quest is not set");

        Get<Slider>((int)Sliders.Slider).value = CalculateProgress();
        UpdateImage();
        UpdateText();

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
           
            Managers.UI.ShowPopupUI<UI_GambleQuest>().SetGambleQuestType((Define.QuestType)System.Enum.Parse(typeof(Define.QuestType), _quest.Category.CodeName));
        });

        //float progress = _quest.CurrentTaskGroup.Tasks.First()

    }
    float CalculateProgress()
    {
        if (_quest.IsCompletable || _quest.IsComplete)
            return 1.0f;

        float progress = (float)_quest.CurrentTaskGroup.Tasks.First().CurrentSuccess / (float)_quest.CurrentTaskGroup.Tasks.First().NeedSuccessToComplete;
        return progress;
    }

    public void UpdateText()
    {
        if (_quest == null)
            return;

        if(GetText((int)Texts.Text) != null)
            GetText((int)Texts.Text).text = _quest.DisplayName;
        
    }
    public void UpdateImage()
    {
        if (_quest == null)
        {
         
            return;
        }

        if (GetImage((int)Images.LeftImage) != null)
            GetImage((int)Images.LeftImage).sprite = _quest.Icon;

        
        

    }

    void UpdateProgress(Quest quest, Task task, int currentSuccess, int prevSuccess)
    {
        if(_quest == quest)
        {
            if (Get<Slider>((int)Sliders.Slider) != null)
                Get<Slider>((int)Sliders.Slider).value = CalculateProgress();
        }
    }

    private void OnDestroy()
    {
        if (_quest != null)
            _quest.onTaskSuccessChanged -= UpdateProgress;
    }


}
