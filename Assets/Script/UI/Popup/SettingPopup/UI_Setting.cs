using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class UI_Setting : UI_Popup
{
    enum Buttons
    {
        Close,
        Exit,
        CouponButton,
        HomepageButton,
        QuestionButton,
        EffectOptionButton,
        DamageOptionButton,
        ShakeOptionButton,
        DeletePlayerButton,
        SaveButton
    }
    enum Texts
    {
        NickName,
        Email,
        EffectOptionButtonText,
        DamageOptionButtonText,
        ShakeOptionButtonText
    }
    enum Sliders
    {
        BGM,
        EffectSound
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        Get<Slider>((int)Sliders.BGM).value = Managers.Setting.BGM;
        Get<Slider>((int)Sliders.EffectSound).value = Managers.Setting.SoundEffect;

        Get<Slider>((int)Sliders.BGM).onValueChanged.AddListener((sound) => { Managers.Setting.BGM = sound; });
        Get<Slider>((int)Sliders.EffectSound).onValueChanged.AddListener((sound) => { Managers.Setting.SoundEffect = sound; });

        UpdateText();

        AddUIEvent(GetButton((int)Buttons.EffectOptionButton).gameObject, (data) => {
            bool option = Managers.Setting.ShowEffect;
            Managers.Setting.ShowEffect = !option;

            UpdateText();
        });
        AddUIEvent(GetButton((int)Buttons.DamageOptionButton).gameObject, (data) => {

            bool option = Managers.Setting.ShowDamage;
            Managers.Setting.ShowDamage = !option;

            UpdateText();

        });
        AddUIEvent(GetButton((int)Buttons.ShakeOptionButton).gameObject, (data) => {
            bool option = Managers.Setting.EnableShake;
            Managers.Setting.EnableShake = !option;

            UpdateText();
        });

        /*
        AddUIEvent(GetButton((int)Buttons.DeletePlayerButton).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_DeletePlayer>();
           
        });
        */


        AddUIEvent(GetButton((int)Buttons.QuestionButton).gameObject, (data) => {
            string to = "amelee.hud@gmail.com";
            string subject = EscapeURL("문의사항");
            string body = EscapeURL
               (
                "이 곳에 내용을 작성해주세요.\n\n\n\n" +
                "________" +
                "Device Model : " + SystemInfo.deviceModel + "\n\n" +
                "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
                "Title Name : " + "활과 함께" + "\n\n" +
                "________"
               );


            Application.OpenURL($"mailto:{to}?subject={subject}&body={body}");
        });
        AddUIEvent(GetButton((int)Buttons.CouponButton).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_Coupon>();
        });
        AddUIEvent(GetButton((int)Buttons.HomepageButton).gameObject, (data) => {
            Application.OpenURL("https://onethesoft.tistory.com/3");
        });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            Managers.Setting.Save();
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            Managers.Setting.Save();
            ClosePopupUI();
        });

        AddUIEvent(GetButton((int)Buttons.SaveButton).gameObject, (data) => {
            UI_SimpleMessageBox _savePopup = Managers.UI.ShowPopupUI<UI_SimpleMessageBox>();
            _savePopup.Text = "서버에 데이터를 수동으로 저장합니다";
      
            _savePopup.OK += () => {
                FindObjectOfType<GameDataUpdater>().ManualUpdateUser();
            };
           

        });
    }

    string EscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
    void UpdateText()
    {
        GetText((int)Texts.DamageOptionButtonText).text = Managers.Setting.ShowDamage ? "ON" : "OFF";
        GetText((int)Texts.EffectOptionButtonText).text = Managers.Setting.ShowEffect ? "ON" : "OFF";
        GetText((int)Texts.ShakeOptionButtonText).text = Managers.Setting.EnableShake ? "ON" : "OFF";

        if(Managers.Network.IS_ENABLE_NETWORK == true)
        {
            GetText((int)Texts.NickName).text = Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName;

            if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.AccountInfo.GooglePlayGamesInfo != null)
            {
                GetText((int)Texts.Email).text = Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.AccountInfo.GooglePlayGamesInfo.GooglePlayGamesPlayerDisplayName;
            }
                
        }

       
    }
    
}
