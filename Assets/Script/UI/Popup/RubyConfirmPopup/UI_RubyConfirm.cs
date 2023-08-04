using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RubyConfirm : UI_Popup
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
        AmountText,
        TitleText
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

    int _amount;
    public int Amount
    {
        get
        {
            return _amount;
        }
        set
        {
            _amount = value;

        }
    }

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetText((int)Texts.TitleText).text = _titleText;
        GetText((int)Texts.AmountText).text = $"X{_amount}";

        AddUIEvent(GetButton((int)Buttons.OK).gameObject, (data) => {
            OnOK?.Invoke();
            ClosePopupUI();
        });

        AddUIEvent(GetButton((int)Buttons.Cancle).gameObject, (data) => {
            OnCancle?.Invoke();
            ClosePopupUI();
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });



    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
        OnOK = null;
        OnCancle = null;
    }

}
