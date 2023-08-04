using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Coffee.UIExtensions;

public class UI_RankupNotice : UI_Popup
{
    enum Buttons
    {
        Close,
        Button
    }
    enum GameObjects
    {
        RankPanel,
        Background,
        RankRow,
        RankHigh
    }
    enum Texts
    {
        NoticeText
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));


        Managers.Game.GetRankIcon(Get<GameObject>((int)GameObjects.RankPanel).transform);
        GetText((int)Texts.NoticeText).text = $"{Managers.Game.PlyaerDataBase.RankDic[Managers.Game.Level].RankText} 등급 달성을 축하드립니다.";

        StartCoroutine(StartAnimation());
       

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            ClosePopupUI();
        });

    }


    public void PlayParticle()
    {
        if (Managers.Game.Level >= (int)PlayerRank.Rank.A)
            Get<GameObject>((int)GameObjects.RankHigh).GetComponent<UIParticle>().Play();
        else
            Get<GameObject>((int)GameObjects.RankRow).GetComponent<UIParticle>().Play();
    }
    IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        Get<GameObject>((int)GameObjects.Background).GetComponent<DOTweenAnimation>().DORestartById("ScaleX");
    }
    
    private void OnDestroy()
    {
        Get<GameObject>((int)GameObjects.Background).GetComponent<DOTweenAnimation>().DOKillById("ScaleX");
    }
}
