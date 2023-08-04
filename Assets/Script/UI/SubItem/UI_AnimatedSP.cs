using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AnimatedSP : UI_Base
{
    enum Images{
        Image
    }
    enum Texts
    {
        Text
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        Managers.Game.OnCurrencyChanged -= UpdateCurrency;
        Managers.Game.OnCurrencyChanged += UpdateCurrency;

        GetImage((int)Images.Image).color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        GetText((int)Texts.Text).color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    }
   
    public void UpdateCurrency(Define.CurrencyID id , string amount)
    {
        if(id == Define.CurrencyID.SP)
        {
            GetImage((int)Images.Image).DOFade(1.0f, 0.5f).OnComplete(() =>
            {
                GetImage((int)Images.Image).DOFade(0.0f, 0.5f).OnComplete(() =>
                {

                });
            }).Restart() ;

            GetText((int)Texts.Text).DOFade(1.0f, 0.5f).OnComplete(() =>
            {
                GetText((int)Texts.Text).DOFade(0.0f, 0.5f).OnComplete(() =>
                {

                });
            }).Restart();
        }
    }
    private void OnDestroy()
    {
        Managers.Game.OnCurrencyChanged -= UpdateCurrency;
    }



}
