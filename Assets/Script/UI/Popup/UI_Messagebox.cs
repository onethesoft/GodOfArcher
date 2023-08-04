using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Messagebox : UI_Popup
{
    public enum Mode
    {
        OK , 
        OKCancel
    }
    enum Texts
    {
        TitleText,
        MainText,
        Text
    }
    enum Buttons
    {
        Close,
        OK,
        Cancel
    }
    public string Title = string.Empty;

    public string Text = string.Empty;
    public int TextSize = 50;
    public string SubText = string.Empty;
    public int SubTextSize = 40;

    public System.Action OK;
    public System.Action Cancel;

    public Mode mode = Mode.OKCancel;

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        if (string.IsNullOrEmpty(Title) == false)
            GetText((int)Texts.TitleText).text = Title;
        else
            GetText((int)Texts.TitleText).gameObject.SetActive(false);

        if (string.IsNullOrEmpty(Text) == false)
        {
            GetText((int)Texts.MainText).text = Text;
            GetText((int)Texts.MainText).fontSize = TextSize;
        }
        else
            GetText((int)Texts.MainText).gameObject.SetActive(false);

        if (string.IsNullOrEmpty(SubText) == false)
        {
            GetText((int)Texts.Text).text = SubText;
            GetText((int)Texts.Text).fontSize = SubTextSize;
        }
        else
            GetText((int)Texts.Text).gameObject.SetActive(false);

        if(mode == Mode.OK)
        {
            GetButton((int)Buttons.Cancel).gameObject.SetActive(false);
        }
        

        AddUIEvent(GetButton((int)Buttons.OK).gameObject, (data) => {
            ClosePopupUI();
            OK?.Invoke();

        });
        AddUIEvent(GetButton((int)Buttons.Cancel).gameObject, (data) => {
            ClosePopupUI();
            Cancel?.Invoke();
        });



    }

    private void OnDestroy()
    {
        OK = null;
        Cancel = null;
    }



}
