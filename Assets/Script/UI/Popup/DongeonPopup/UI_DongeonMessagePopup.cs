using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DongeonMessagePopup : UI_Popup
{
    enum Buttons
    {
        OkButton,
        CancelButton
    }

    enum Texts
    {
        Content,
        TitleText
    }
    public System.Action OnOk;
    public System.Action OnCancel;
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        AddUIEvent(GetButton((int)Buttons.OkButton).gameObject, (data) => {
            OnOk?.Invoke();
        });

        AddUIEvent(GetButton((int)Buttons.CancelButton).gameObject, (data) => {
            OnCancel?.Invoke();
            ClosePopupUI();
        });
    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
        OnOk = null;
        OnCancel = null;
    }
}
