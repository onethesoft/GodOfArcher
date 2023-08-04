using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankHelp : UI_Popup
{
    enum Buttons
    {
        Close,
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

        foreach(PlayerRank rank in Managers.Game.PlyaerDataBase.RankList)
            Util.GetOrAddComponent<UI_RankItem>(Managers.Resource.Instantiate("UI/SubItem/RankHelpPopup/UI_RankItem", Get<GameObject>((int)GameObjects.ItemPanel).transform)).Setup(rank);
        


        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });
    }
}
