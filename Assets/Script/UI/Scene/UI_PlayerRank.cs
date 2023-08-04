using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Play.Review;

public class UI_PlayerRank : UI_Base
{
    enum Texts
    {
        RankText,
        DisplayNameText
    }

    GameObject _iconObject = null;
    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        _iconObject = Managers.Game.GetRankIcon(gameObject.transform);
        GetText((int)Texts.RankText).text = Managers.Game.PlyaerDataBase.RankDic[Managers.Game.Level].DisplayName;
        if(Managers.Network.IS_ENABLE_NETWORK == true)
            GetText((int)Texts.DisplayNameText).text = Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName;
        

        Managers.Game.OnLevelChanged -= UpdateRank;
        Managers.Game.OnLevelChanged += UpdateRank;

        
    }
    void UpdateRank()
    {
        _iconObject = Managers.Game.GetRankIcon(gameObject.transform);
        GetText((int)Texts.RankText).text = Managers.Game.PlyaerDataBase.RankDic[Managers.Game.Level].DisplayName;

        if (Managers.Network.IS_ENABLE_NETWORK == true)
            GetText((int)Texts.DisplayNameText).text = Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName;
        
    }
    
    private void Start()
    {
        Init();
    }
    private void OnDestroy()
    {
        if (_iconObject != null)
            Managers.Resource.Destroy(_iconObject);
        _iconObject = null;
        Managers.Game.OnLevelChanged -= UpdateRank;
    }

}
