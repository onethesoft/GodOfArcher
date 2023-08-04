using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dongeon : UI_Popup
{
    enum Buttons
    {
        Close ,
        Exit
    }
    enum GameObjects
    {
        ItemPanel
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        foreach (StageTask task in Managers.Game.StageDataBase.StageList)
        {
            if (task.type == Define.Dongeon.Main) continue;
            UI_DongeonItem item = Util.GetOrAddComponent<UI_DongeonItem>(Managers.Resource.Instantiate("UI/SubItem/DongeonPopup/UI_DongeonItem", Get<GameObject>((int)GameObjects.ItemPanel).transform));
            item.Task = task;

            
        }
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });

    }
}
