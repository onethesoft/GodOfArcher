using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardItem : UI_Base
{
    Reward _reward;
    public void SetReward(Reward reward)
    {
        _reward = reward;
    }
    enum Texts
    {
        Description
    }

    enum Images
    {
        Icon
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        if(_reward != null)
        {
            GetImage((int)Images.Icon).sprite = _reward.Icon;
            GetText((int)Texts.Description).text = $"{_reward.Description}";
        }
    }

    private void Start()
    {
        Init();
    }

    


}
