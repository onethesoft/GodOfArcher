using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_NoticeBoard : UI_Popup
{
    enum Buttons
    {
        Close,
        Exit,
        NoticeButton,
        EventButton,
        CouponButton,
        TistoryButton,
        ServiceButton
    }

    enum Texts
    {
        NoticeText
    }

    NoticeData _notice;
    NoticeData _eventNotice;

    int headerFontSize = 50;
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        string noticeJsonData;
        string eventNoticeJsonData;

        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.TitleData.TryGetValue(PlayerInfo.TitleDataKey.Notice.ToString(), out noticeJsonData))
        {
            _notice = NoticeData.FromJson(noticeJsonData);
        }

        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.TitleData.TryGetValue(PlayerInfo.TitleDataKey.EventNotice.ToString(), out eventNoticeJsonData))
        {
            _eventNotice = NoticeData.FromJson(eventNoticeJsonData);
        }

        ShowNoticeText(false);
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });


        AddUIEvent(GetButton((int)Buttons.NoticeButton).gameObject, (data) => {
            ShowNoticeText(false);
        });
        AddUIEvent(GetButton((int)Buttons.EventButton).gameObject, (data) => {
            ShowNoticeText(true);
        });
        AddUIEvent(GetButton((int)Buttons.CouponButton).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_Coupon>();
        });
        AddUIEvent(GetButton((int)Buttons.TistoryButton).gameObject, (data) => {
            Application.OpenURL("https://onethesoft.tistory.com/3");
        });
        AddUIEvent(GetButton((int)Buttons.ServiceButton).gameObject, (data) => {
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

    }
    void ShowNoticeText(bool isEventNotice)
    {
        if(!isEventNotice)
            GetText((int)Texts.NoticeText).text = $"{System.Environment.NewLine}<size={headerFontSize}>{"-공지 사항-"}</size>{System.Environment.NewLine}" + _notice.GetNoticeText();
        else
            GetText((int)Texts.NoticeText).text = $"{System.Environment.NewLine}<size={headerFontSize}>{"-이벤트 공지 사항-"}</size>{System.Environment.NewLine}" + _eventNotice.GetNoticeText();

    }
    
    string EscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
}
