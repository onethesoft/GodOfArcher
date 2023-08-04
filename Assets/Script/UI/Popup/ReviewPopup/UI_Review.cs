using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Review : UI_Popup
{
    enum Buttons
    {
        OK
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        AddUIEvent(GetButton((int)Buttons.OK).gameObject, (data) => {

            GetButton((int)Buttons.OK).gameObject.SetActive(false);
            StartCoroutine(RequestReviewFlow());
        });
    }

    IEnumerator RequestReviewFlow()
    {
#if UNITY_ANDROID
        var reviewManager = new ReviewManager();
        var reviewAsyncOperation = reviewManager.RequestReviewFlow();

        yield return reviewAsyncOperation;
        if (reviewAsyncOperation.Error != ReviewErrorCode.NoError)
        {
            Managers.UI.ShowPopupUI<UI_LogTest>().AddLog(reviewAsyncOperation.Error.ToString());
            yield break;
        }

        var playReviewInfo = reviewAsyncOperation.GetResult();
        var reviewFlow = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return reviewFlow;
        if(reviewFlow.Error != ReviewErrorCode.NoError)
        {
            Managers.UI.ShowPopupUI<UI_LogTest>().AddLog(reviewFlow.Error.ToString());
            yield break;
        }
        Managers.Game.AddCurrency(Define.CurrencyID.Ruby.ToString(), 30000, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            


        ClosePopupUI();
#else
        ClosePopupUI();
#endif
    }
}
