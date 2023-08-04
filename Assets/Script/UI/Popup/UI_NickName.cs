using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_NickName : UI_Popup
{
    enum InputFields
    {
        InputField
    }
    enum Buttons
    {
        Button
    }
    enum Texts
    {
        Description
    }
    string _inputNickname;
    string[] fworld_list;

   
    public override void Init()
    {
        base.Init();

        Bind<InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        fworld_list = Managers.Data.LoadProhibitedText();
        Get<InputField>((int)InputFields.InputField).onValueChanged.AddListener(
            (text) => {
                _inputNickname = text;
            }
        );
        //fworld_list = Managers.Data.TextData(Define.DataFileList.fword_list.ToString()).

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            if (string.IsNullOrEmpty(_inputNickname))
            {
                GetText((int)Texts.Description).text = "아이디를 입력해 주세요";
                return;
            }
            else if(_inputNickname.Length < 3)
            {
                GetText((int)Texts.Description).text = "3자 이상 입력해 주세요";
                return;
            }

            if(IsProhibitNickName(_inputNickname))
            {
                GetText((int)Texts.Description).text = "비속어는 사용할 수 없습니다";
                return;
            }

            ClosePopupUI();

            Managers.Network.UpdateNickName(_inputNickname);
            Managers.UI.ShowPopupUI<UI_Loading>(); 


        });


    }
    bool IsProhibitNickName(string nickname)
    {
        if (fworld_list.ToList().Any(x => nickname.Contains(x)))
            return true;
        return false;
      
        
    }
    
}
