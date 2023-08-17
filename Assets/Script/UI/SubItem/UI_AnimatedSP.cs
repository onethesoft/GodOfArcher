using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AnimatedSP : UI_Base
{
    [SerializeField]
    Define.CurrencyID _currency;

    [SerializeField]
    Image image;

    [SerializeField]
    Text text;

    [HideInInspector]
    DOTweenAnimation _imageTweener;

    [HideInInspector]
    DOTweenAnimation _textTweener;
    public override void Init()
    {

        Managers.Game.OnCurrencyChanged -= UpdateCurrency;
        Managers.Game.OnCurrencyChanged += UpdateCurrency;

        _imageTweener = image.GetComponent<DOTweenAnimation>();
        _textTweener = text.GetComponent<DOTweenAnimation>();

        // image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        // text.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        if (_imageTweener != null)
        {
            _imageTweener.DORestart();
        }


        if (_textTweener != null)
        {
            _textTweener.DORestart();
        }

    }

    public void UpdateCurrency(Define.CurrencyID id, string amount)
    {
        if (id == _currency)
        {

            if (gameObject.activeSelf)
            {
                _imageTweener.DORestart();
                _textTweener.DORestart();
            }
            else
                gameObject.SetActive(true);

        }

    }
    private void OnDestroy()
    {
        Managers.Game.OnCurrencyChanged -= UpdateCurrency;
    }



}
