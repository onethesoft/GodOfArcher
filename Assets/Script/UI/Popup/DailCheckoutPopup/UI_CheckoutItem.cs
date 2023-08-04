using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_CheckoutItem : UI_Base
{
    enum Texts
    {
        Description,
        AmountText
    }

    enum Images
    {
        Blocker,
        BlockerImage
    }
    Quest _quest;
    public void Setup(Quest quest)
    {
        _quest = quest;
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        Debug.Assert(_quest != null, "UI_CheckoutItem quest not setup");
        GetText((int)Texts.Description).text = _quest.Description;
        GetText((int)Texts.AmountText).text = _quest.Rewards.First().Quantity.ToString();

        
        if (_quest.IsComplete)
            GetImage((int)Images.Blocker).gameObject.SetActive(true);
        else
            GetImage((int)Images.Blocker).gameObject.SetActive(false);

        _quest.onCompleted -= UpdateCompleteState;
        _quest.onCompleted += UpdateCompleteState;

        


    }
    public void UpdateCompleteState(Quest quest )
    {
        Debug.Log("QuestComplete");
        if(quest == _quest && quest.IsComplete)
        {
            GetImage((int)Images.Blocker).gameObject.SetActive(true);
        }
    }
   
    void Start()
    {
        Init();
    }
    private void OnDestroy()
    {
        _quest.onCompleted -= UpdateCompleteState;
    }



}
