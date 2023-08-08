using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankingReward : UI_Popup
{
    enum Buttons
    {
        Close,
        Exit
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });

    }
   
}
