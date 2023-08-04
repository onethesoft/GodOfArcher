using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Loading : UI_Popup
{
    public delegate void OnLoadingHandler();
    public event OnLoadingHandler OnLoading;
    enum Sliders
    {
        Slider
    }
    enum Texts
    {
        LoadingText
    }

    const float _time = 10.0f;
   
    public override void Init()
    {
        base.Init();
        Bind<Slider>(typeof(Sliders));
        Bind<Text>(typeof(Texts));

        Get<Slider>((int)Sliders.Slider).DOValue(1.0f, _time).OnComplete(()=> {
            if(Managers.Network.IS_ENABLE_NETWORK)
                StartCoroutine(WaitForGetPlayerInfoCompleted());
            else
                Managers.Scene.LoadScene(Define.Scene.Main);
        });

    }
    IEnumerator WaitForGetPlayerInfoCompleted()
    {
        while (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.AccountInfo == null)
        {
            yield return new WaitForSeconds(1.0f);
        }

        Managers.Scene.LoadScene(Define.Scene.Main);
    }

   


    
}
