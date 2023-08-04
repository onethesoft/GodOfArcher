using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_DailyCheckout : UI_Popup
{
    enum Buttons
    {
        Close,
        Exit,
        Button
    }
    enum GameObjects
    {
        QuestItemPanel
    }


    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        

        foreach (Quest _dailyQuest in Managers.Quest.FindQuestByCategory(Define.QuestType.DailyCheckout.ToString()))
            Util.GetOrAddComponent<UI_CheckoutItem>(Managers.Resource.Instantiate($"UI/SubItem/DailyCheckoutPopup/UI_CheckoutItem", Get<GameObject>((int)GameObjects.QuestItemPanel).transform)).Setup(_dailyQuest);
            
        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            int progress =Managers.Quest.CompletedQuests.Where(x => x.Category == Define.QuestType.DailyCheckout.ToString()).Count();
            int Active = Managers.Quest.ActiveQuests.Where(x => x.Category == Define.QuestType.DailyCheckout.ToString()).Count();

            int total = progress + Active;


            GameData _gameData = FindObjectOfType<GameData>();
            if(_gameData.IsDayPassedQuest(Define.QuestType.DailyCheckout) && (_gameData.IsAllCompletedQuest(Define.QuestType.DailyCheckout) == false))
                _gameData.ProcessReportDailyCheckoutQuest();
         
                
            
        });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
           
           
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
           
            
            ClosePopupUI();
        });
    }
    private void Start()
    {
        Init();
    }
}
