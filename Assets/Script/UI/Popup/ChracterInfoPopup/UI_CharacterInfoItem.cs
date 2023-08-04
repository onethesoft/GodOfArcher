using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterInfoItem : UI_Base
{
    string _headText;
    string _contentText;
    public string HeadText 
    { 
        get
        {
            return _headText;
        }
        set
        {
            _headText = value;
            if (GetText((int)Texts.Head) != null)
            {
                GetText((int)Texts.Head).text = value;
            }
        }
    }
    public string ContentText 
    {  
        get
        {
            return _contentText;
        }
        set
        {
            _contentText = value;
            if (GetText((int)Texts.Content) != null) GetText((int)Texts.Head).text = value;
        }
    }

    enum Texts
    {
        Head,
        Content
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.Head).text = HeadText;
        GetText((int)Texts.Content).text = ContentText;

    }

    
}
