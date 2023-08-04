using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectButton : UI_Base
{
    public delegate void SelectHandler(UI_SelectButton item);
    public event SelectHandler onSelect = null;
    enum Images
    {
        Select
    }

    bool _select = false;
    public bool Select 
    {
        get { return _select; }
        set 
        { 
            _select = value;
            if(GetImage((int)Images.Select) != null)
            {
                if (_select)
                    GetImage((int)Images.Select).gameObject.SetActive(true);
                else
                    GetImage((int)Images.Select).gameObject.SetActive(false);
            }
        }
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));

        if (_select)
            GetImage((int)Images.Select).gameObject.SetActive(true);
        else
            GetImage((int)Images.Select).gameObject.SetActive(false);

        AddUIEvent(gameObject, (data) => {
            Select = true;
            onSelect?.Invoke(this);

        });

    }
    private void Start()
    {
        Init();
    }


}
