using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Agreement : UI_Popup
{
    enum Buttons
    {
        TermsOfUse_CheckBox,
        Privacypolicy_CheckBox,
        AgreeButton
    }

    const string CheckBoxOff = "Sprites/TitleScene/toggle_checkbox_off";
    const string CheckBoxOn = "Sprites/TitleScene/toggle_checkbox_on";

    bool IsTermsOfUseAgree = false;
    bool IsPrivacypolicy = false;
    public System.Action OnExit = null;
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        AddUIEvent(GetButton((int)Buttons.TermsOfUse_CheckBox).gameObject, (data) => {
            IsTermsOfUseAgree = !IsTermsOfUseAgree;
            GetButton((int)Buttons.TermsOfUse_CheckBox).gameObject.GetComponent<Image>().sprite = IsTermsOfUseAgree ?  Managers.Resource.Load<Sprite>(CheckBoxOn) : Managers.Resource.Load<Sprite>(CheckBoxOff);

           
        });
        AddUIEvent(GetButton((int)Buttons.Privacypolicy_CheckBox).gameObject, (data) => {
            IsPrivacypolicy = !IsPrivacypolicy;
            GetButton((int)Buttons.Privacypolicy_CheckBox).gameObject.GetComponent<Image>().sprite = IsPrivacypolicy ? Managers.Resource.Load<Sprite>(CheckBoxOn) : Managers.Resource.Load<Sprite>(CheckBoxOff);

           
        });

        AddUIEvent(GetButton((int)Buttons.AgreeButton).gameObject, (data) => {
            CheckAgreeState();
        });
    }
    void CheckAgreeState()
    {
        if(IsTermsOfUseAgree && IsPrivacypolicy)
        {
            Managers.UI.ClosePopupUI();
            OnExit?.Invoke();
            OnExit = null;
            

        }
    }
}
