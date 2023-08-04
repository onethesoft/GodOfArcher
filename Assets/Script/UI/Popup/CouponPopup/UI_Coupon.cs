using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Coupon : UI_Popup
{
    enum InputFields
    {
        InputField
    }
    enum Texts
    {
        TitleText,
        Text,
        Placeholder,
        ButtonText,
        Description
    }
    enum Buttons
    {
        Button,
        Close
    }
    string _inputNickname;
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<InputField>(typeof(InputFields));

        Get<InputField>((int)InputFields.InputField).onValueChanged.AddListener(
           (text) => {
               _inputNickname = text;
           }
       );

        GetText((int)Texts.Description).text = "쿠폰 번호를 입력해 주세요";

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            if (string.IsNullOrEmpty(_inputNickname))
                return;

            Define.CouponResponse _res = Managers.Item.ConsumeCoupon(_inputNickname , FindObjectOfType<GameData>());
            if(_res == Define.CouponResponse.Success)
            {
                GetText((int)Texts.Description).text = "쿠폰이 발급되었습니다. 우편함에서 확인해주세요";
            }
            else if(_res == Define.CouponResponse.NotExist)
            {
                GetText((int)Texts.Description).text = "잘못된 쿠폰번호입니다.";
            }
            else if(_res == Define.CouponResponse.AlreadyIssue)
            {
                GetText((int)Texts.Description).text = "이미 발급된 쿠폰입니다.";
            }
            else if(_res == Define.CouponResponse.Expired)
            {
                GetText((int)Texts.Description).text = "기한이 만료된 쿠폰입니다.";
            }
           
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });

    }
}
