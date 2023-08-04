using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogTest : UI_Popup
{
    enum Buttons
    {
        Exit
    }
    enum Texts
    {
        LogText
    }
    string _text;
    public void Setup(string text)
    {
        _text = text;
    }
    public void AddLog(string logText)
    {
        _text += System.Environment.NewLine + logText;
        
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.LogText).text = _text;

        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            Application.Quit();
        });
    }
}
