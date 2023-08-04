using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AdConfirm : UI_Popup
{
    public Action OnOK;
    public Action OnCancle;

    enum Buttons
    {
        OK,
        Cancle,
        Close
    }
    enum Texts
    {
        TitleText,
        ContentText
    }
    string _titleText;
    public string TitleText
    {
        get
        {
            return _titleText;
        }
        set
        {
            _titleText = value;
        }
    }

    string _content;
    public string ContentText
    {
        get
        {
            return _content;
        }
        set
        {
            _content = value;

        }
    }

    public int ContentTextFontSize = 40;
    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetText((int)Texts.TitleText).text = _titleText;
        GetText((int)Texts.ContentText).text = _content;
        GetText((int)Texts.ContentText).fontSize = ContentTextFontSize;


        AddUIEvent(GetButton((int)Buttons.OK).gameObject, (data) => {
            ClosePopupUI();
            OnOK?.Invoke();
            
        });

        AddUIEvent(GetButton((int)Buttons.Cancle).gameObject, (data) => {
            ClosePopupUI();
            OnCancle?.Invoke();
            
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
    }

    private void OnDestroy()
    {
        OnOK = null;
        OnCancle = null;
    }
}
