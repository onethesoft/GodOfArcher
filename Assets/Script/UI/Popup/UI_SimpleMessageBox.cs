using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SimpleMessageBox : UI_Popup
{
   
    public enum Mode
    {
        OK,
        OKCancel
    }
    enum Texts
    {
        Text
    }
    enum Buttons
    {
        OK,
        Cancel
    }
    

    public string Text = string.Empty;
    public int TextSize = 40;
   
    public System.Action OK;
    public System.Action Cancel;

    public Mode mode = Mode.OKCancel;

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

      
     
        GetText((int)Texts.Text).text = Text;
        GetText((int)Texts.Text).fontSize = TextSize;
      

        if (mode == Mode.OK)
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
