using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestComplete : UI_Popup
{
    public delegate void ReceiveHandler();
    public ReceiveHandler OnReceive = null;
    enum Buttons
    {
        Button
    }
    enum GameObjects
    {
        Content
    }
    List<Reward> _rewards; 
    public void SetRewardList(List<Reward> RewardItems)
    {
        _rewards = RewardItems;
    }
    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        if(_rewards != null && _rewards.Count != 0)
        {
            foreach(Reward item in _rewards)
                Util.GetOrAddComponent<UI_RewardItem>(Managers.Resource.Instantiate("UI/SubItem/QuestPopup/UI_RewardItem", Get<GameObject>((int)GameObjects.Content).transform)).SetReward(item);
        }

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            OnReceive?.Invoke();
            ClosePopupUI();
            
        });

    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
        if (_rewards != null)
            _rewards.Clear();
        OnReceive = null;
    }


}
