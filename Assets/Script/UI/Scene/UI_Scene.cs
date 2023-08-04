using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, false);
        //Util.GetOrAddComponent<UI_DebugView>(Managers.Resource.Instantiate("UI/SubItem/UI_DebugView",gameObject.transform));
    }
    private void Start()
    {
        Init();
    }
}
