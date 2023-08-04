using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Title : UI_Scene
{
    enum Buttons
    {
        RegisterButton
    }
    enum Texts
    {
        LoadingText,
        VersionText
    }
    enum GameObjects
    {
        Blocker,
        Animation_0,
        Animation_1,
        Animation_2,
        Animation_3,
        Animation_4,
        Animation_5,
        Animation_6,
        Animation_7,
        Animation_8
    }

    enum Animations
    {
        Animation_0 = 1,
        Animation_1,
        Animation_2,
        Animation_3,
        Animation_4,
        Animation_5,
        Animation_6,
        Animation_7,
        Animation_8

    }
        
   



    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetText((int)Texts.VersionText).text = $"Onethesoft. All Rights Reserved V{Application.version}";

        ShowLoadingText();
        AnimationInitialize();


        Get<GameObject>((int)GameObjects.Blocker).GetComponent<Image>().DOFade(0.0f, 1.5f).OnComplete(()=> {

            StartCoroutine(ShowAnimation());
            ShowBlocker(false);
        });
       
        
        AddUIEvent(GetButton((int)Buttons.RegisterButton).gameObject, (data) => {
            ShowLoadingText();

            Managers.UI.ShowPopupUI<UI_Agreement>().OnExit += () => {
                ProcessRegister();
            };

            
        });
      
        

    }
    public void ShowRegisterButton()
    {
        GetButton((int)Buttons.RegisterButton).gameObject.SetActive(true);
        GetText((int)Texts.LoadingText).gameObject.SetActive(false);
    }
    void ShowLoadingText()
    {
        GetButton((int)Buttons.RegisterButton).gameObject.SetActive(false);
        GetText((int)Texts.LoadingText).gameObject.SetActive(true);
    }
    void ShowBlocker(bool IsShow)
    {
        Get<GameObject>((int)GameObjects.Blocker).SetActive(IsShow);
    }

    void AnimationInitialize()
    {
        foreach (Animations anim in System.Enum.GetValues(typeof(Animations)))
            Get<GameObject>((int)anim).SetActive(false);
    }
    IEnumerator ShowAnimation()
    {
        yield return new WaitForSeconds(0.4f);

        foreach(Animations anim in System.Enum.GetValues(typeof(Animations)))
        {
            Get<GameObject>((int)anim).SetActive(true);
            if (anim == Animations.Animation_0)
            {
                Get<GameObject>((int)anim).transform.DOLocalMoveY(0.0f, 0.25f).From(Vector3.up * 250.0f).SetEase(Ease.InQuad).OnComplete(()=> {
                    Get<GameObject>((int)Animations.Animation_0).transform.DOShakePosition(2.0f, strength: new Vector3(0, 15, 0), vibrato: 5, randomness: 5, snapping: false, fadeOut: true);
                    Get<GameObject>((int)Animations.Animation_0).GetComponent<DOTweenAnimation>().DORestartById("Scale");
                });
               
            }
            else
                Get<GameObject>((int)anim).GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            yield return new WaitForSeconds(0.5f);
            
        }

        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            Managers.Network.OnErrorCallback += (error) =>
            {
                if (error == PlayFab.PlayFabErrorCode.AccountNotFound)
                    ShowRegisterButton();
            };
            Managers.Network.RequestLogin();
        }
        else
        {
            ShowRegisterButton();
        }


    }
    


    void ProcessRegister()
    {
        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            Managers.Network.RequestRegister();
        }
        else
        {
            Managers.UI.ShowPopupUI<UI_Loading>();
        }
       
    }

    

    
}
