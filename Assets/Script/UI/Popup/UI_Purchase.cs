using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Purchase : UI_Popup
{
    public enum ButtonMode
    {
        OkCancel,
        Cancel
    }
    enum Images
    {
        Currency
    }
    enum Texts
    {
        CurrencyAmount,
        TitleText,
        DescriptionText
    }

    enum Buttons
    {
        OK,
        Cancel
    }

    public Define.CurrencyID Currency;
    public int Amount;
    public string Title;
    public string Description;

    public Action OnPurchase;
    public Action OnCancel;
    public ButtonMode mode = ButtonMode.OkCancel;
    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetImage((int)Images.Currency).sprite = Managers.Game.PlyaerDataBase.CurrencyDict[Currency.ToString()].Icon;
        GetText((int)Texts.CurrencyAmount).text = $"X {Amount}";


        GetText((int)Texts.TitleText).text = Title;

        if(string.IsNullOrEmpty(Description) == false)
            GetText((int)Texts.DescriptionText).text = Description;

        if (mode == ButtonMode.Cancel)
        {
            GetButton((int)Buttons.OK).gameObject.SetActive(false);
            GetButton((int)Buttons.Cancel).gameObject.SetActive(true);
        }
        else
        {
            GetButton((int)Buttons.OK).gameObject.SetActive(true);
            GetButton((int)Buttons.Cancel).gameObject.SetActive(true);
        }
        AddUIEvent(GetButton((int)Buttons.OK).gameObject, (data) => {
            OnPurchase?.Invoke();
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Cancel).gameObject, (data) => {
            OnCancel?.Invoke();
            ClosePopupUI();
        });

        


    }

    public override void ClosePopupUI()
    {
        OnPurchase = null;
        OnCancel = null;
        base.ClosePopupUI();

    }
}
