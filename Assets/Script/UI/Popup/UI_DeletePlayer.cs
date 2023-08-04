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

    string _infoText_0 = $"ȸ��Ż�� �����մϴ�.{System.Environment.NewLine}{System.Environment.NewLine}ȸ��Ż�� �Ϸ�Ǹ� ��� ������ �����Ǹ�, ������ �Ұ����մϴ�.{System.Environment.NewLine}{System.Environment.NewLine}������ ȸ��Ż�� ��û�Ͻ÷��� �Ʒ� ������ �Է��� �ּ���.";
    string _infoText_1 = $"ȸ��Ż�� �����Ͻðڽ��ϱ�?{System.Environment.NewLine}{System.Environment.NewLine}�����Ͻ÷��� Ȯ�� ��ư�� �����ּ���.";



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
