using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ranking : UI_Popup , IRecyclableScrollRectDataSource
{
    enum Buttons
    {
        Exit,
        Close,
        RankingRewardButton
    }

    enum GameObjects
    {
        Content,
        PlayerRankingItem
    }

    [SerializeField]
    RecyclableScrollRect _scrollview;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        FindObjectOfType<GameData>().OpenPopup(this);
        List<PlayFab.ClientModels.PlayerLeaderboardEntry> _entryList = Managers.Ranking.GetLeaderboardList();
        _scrollview.DataSource = this;
        /*
        List<PlayFab.ClientModels.PlayerLeaderboardEntry> _entryList = Managers.Ranking.GetLeaderboardList();
        for(int i=0; i< Get<GameObject>((int)GameObjects.Content).transform.childCount; i++)
        {
            if (i >= _entryList.Count)
            {
                 Get<GameObject>((int)GameObjects.Content).transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject rankItem = Get<GameObject>((int)GameObjects.Content).transform.GetChild(i).gameObject;
            UI_RankingItem _item = Util.GetOrAddComponent<UI_RankingItem>(rankItem);
           
            _item.Setup(_entryList[i]);
            Debug.Log(_entryList[i].DisplayName);

        }
        */
        /*
        foreach (PlayFab.ClientModels.PlayerLeaderboardEntry _entry in Managers.Ranking.GetLeaderboardList())
        {
            UI_RankingItem _item = Util.GetOrAddComponent<UI_RankingItem>(Managers.Resource.Instantiate("UI/SubItem/RankingPopup/UI_RankingItem", Get<GameObject>((int)GameObjects.Content).transform));
            _item.Setup(_entry);
        }
        */


        Util.GetOrAddComponent<UI_RankingItem>(Get<GameObject>((int)GameObjects.PlayerRankingItem)).Setup(Managers.Player.GetPlayer(Managers.Game.PlayerId));

        AddUIEvent(GetButton((int)Buttons.RankingRewardButton).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_RankingReward>();
        });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });
    }
    public int GetItemCount()
    {
        return Managers.Ranking.GetLeaderboardList().Count;
    }
    public void SetCell(ICell cell, int index)
    {
        UI_RankingItem _item = cell as UI_RankingItem;
        if (_item != null)
            _item.Setup(Managers.Ranking.GetLeaderboardList()[index]);
        //UI_RankingItem _item = Util.GetOrAddComponent<UI_RankingItem>(cell);
    }
}
