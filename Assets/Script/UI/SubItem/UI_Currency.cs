using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Currency : UI_Base
{
    [SerializeField]
    Define.CurrencyID _currencyId;


    enum Texts
    {
        AmountText
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.AmountText).text = Util.GetBigIntegerUnit(Managers.Game.GetCurrency(_currencyId.ToString()));

        Managers.Game.OnCurrencyChanged -= AnimateGainCurrency;
        Managers.Game.OnCurrencyChanged += AnimateGainCurrency;

        Managers.Game.OnCurrencyUpdated -= UpdateCurrency;
        Managers.Game.OnCurrencyUpdated += UpdateCurrency;

    }
    void AnimateGainCurrency(Define.CurrencyID id , string amount)
    {
        if (id == _currencyId)
        {
            GetText((int)Texts.AmountText).text = Util.GetBigIntegerUnit(Managers.Game.GetCurrency(_currencyId.ToString()));

            System.Numerics.BigInteger _amount = System.Numerics.BigInteger.Parse(amount);
            string _amountText;
            if(_amount > 0)
            {
                _amountText = "+" + Util.GetBigIntegerUnit(_amount);
            }
            else
            {
                _amount = System.Numerics.BigInteger.Abs(_amount);
                _amountText = "-" + Util.GetBigIntegerUnit(_amount);

            }
            
            UI_AnimatedCurrency _animationText =  Util.GetOrAddComponent<UI_AnimatedCurrency>(Managers.Resource.Instantiate("UI/SubItem/UI_AnimatedCurrency" , gameObject.transform));
            
            _animationText.Text = _amountText;
            _animationText.gameObject.transform.position = gameObject.transform.position;

            if (_currencyId == Define.CurrencyID.Gold)
                _animationText.SetTextColor(Color.yellow);
            else if (_currencyId == Define.CurrencyID.Ruby)
                _animationText.SetTextColor(Color.red);
            
        }
    }

    void UpdateCurrency(Define.CurrencyID id)
    {
        if (id == _currencyId)
        {
            GetText((int)Texts.AmountText).text = Util.GetBigIntegerUnit(Managers.Game.GetCurrency(_currencyId.ToString()));

          
        }
    }
    /* private void Update()
     {
         GetText((int)Texts.AmountText).text = Util.GetBigIntegerUnit(Managers.Game.GetCurrency(_currencyId.ToString()));
     }
    */

    private void OnDestroy()
    {
        Managers.Game.OnCurrencyChanged -= AnimateGainCurrency;
        Managers.Game.OnCurrencyUpdated -= UpdateCurrency;

    }

    
}
