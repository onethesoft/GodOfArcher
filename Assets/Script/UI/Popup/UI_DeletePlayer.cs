using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DeletePlayer : UI_Popup
{
    enum Buttons
    {
        OKButton,
        CancelButton
    }

    enum InputFields
    {
        InputField
    }
    enum Texts
    {
        AgreeText,
        InfoText
    }
    enum GameObjects
    {
        VerticalLayout
    }

    string _inputText = string.Empty;

    string agreeText;

    string _infoText_0 = $"회원탈퇴를 진행합니다.{System.Environment.NewLine}{System.Environment.NewLine}회원탈퇴가 완료되면 모든 정보가 삭제되며, 복구가 불가능합니다.{System.Environment.NewLine}{System.Environment.NewLine}정말로 회원탈퇴를 신청하시려면 아래 문장을 입력해 주세요.";
    string _infoText_1 = $"회원탈퇴를 진행하시겠습니까?{System.Environment.NewLine}{System.Environment.NewLine}진행하시려면 확인 버튼을 눌러주세요.";



    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        Get<InputField>((int)InputFields.InputField).onValueChanged.AddListener(
            (text) => {
                _inputText = text;
            }
        );

        Get<InputField>((int)InputFields.InputField).gameObject.SetActive(false);
        GetText((int)Texts.AgreeText).gameObject.SetActive(false);
        GetText((int)Texts.InfoText).text = _infoText_1;


        AddUIEvent(GetButton((int)Buttons.OKButton).gameObject, (data) => {

            if (Get<InputField>((int)InputFields.InputField).gameObject.activeSelf)
            {
                if (_inputText != GetText((int)Texts.AgreeText).text)
                    return;
            }
            else
            {
                ShowInputField();
                return;
            }
            

            Managers.UI.ShowPopupUI<UI_LoadingBlock>();


            Managers.Network.DeletePlayerFromTitle(Managers.Game.PlayerId);
        });

        AddUIEvent(GetButton((int)Buttons.CancelButton).gameObject, (data) => {
            ClosePopupUI();
        });



    }

    private void ShowInputField()
    {
        Get<InputField>((int)InputFields.InputField).gameObject.SetActive(true);
        GetText((int)Texts.AgreeText).gameObject.SetActive(true);
        GetText((int)Texts.InfoText).text = _infoText_0;

        LayoutRebuilder.ForceRebuildLayoutImmediate(Get<GameObject>((int)GameObjects.VerticalLayout).GetComponent<RectTransform>());
    }
    public void OnInputFieldChanged(string value)
    {
        _inputText = value;
    }
}
