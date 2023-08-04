using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeButton : UI_Base
{
    public string StatText
    {
        set
        {
            if (GetText((int)Texts.StatText) != null)
                GetText((int)Texts.StatText).text = value;
        }
    }
    public string CountText
    {
        set
        {
            if (GetText((int)Texts.CountText) != null)
                GetText((int)Texts.CountText).text = value;
        }
    }
    enum Texts
    {
        StatText,
        CountText
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));


    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    
}
