using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UI_ScrollingSlider : UI_Base
{
    [SerializeField]
    RawImage rawImage;

    [SerializeField]
    Image _border;

    [SerializeField]
    Image _edge;

    [SerializeField]
    Image _edge_1;

    [SerializeField]
    Image _edge_2;

    [SerializeField]
    TextMeshProUGUI _counterText;

    float _count = 3.0f;
    System.Action _callback;

    bool _isPlay = false;
    public void SetDuration(float duration)
    {
        if (duration < 0.0f)
            return;

        _count = duration;
    }
    public void OnComplete(System.Action callback)
    {
        _callback = callback;
    }
    public void Play()
    {
        _edge_1.GetComponent<DOTweenAnimation>().DORestartById("Color");
        _edge_1.GetComponent<DOTweenAnimation>().DORestartById("Scale");

        _edge_2.GetComponent<DOTweenAnimation>().DORestartById("Color");
        _edge_2.GetComponent<DOTweenAnimation>().DORestartById("Scale");

        Vector2 _dff = _border.GetComponent<RectTransform>().sizeDelta;
        _dff.x = 0;
        _border.GetComponent<RectTransform>().DOSizeDelta(_dff, _count).OnComplete(() => {
            _callback?.Invoke();
        });



        _edge.GetComponent<RectTransform>().DOAnchorPosX(0.0f, _count).OnComplete(() =>
        {
            _edge.gameObject.SetActive(false);
        }).OnStart(() => {
            _edge.gameObject.SetActive(true);
        });

        _counterText.DOCounter((int)_count, 0, _count, false);
        _isPlay = true;
    }
    public override void Init()
    {

        _edge.gameObject.SetActive(false);
        _isPlay = false;

        _counterText.text = _count.ToString("F1");

    }

    private void Update()
    {

        Rect _rect = rawImage.uvRect;
        _rect.x -= (Time.deltaTime * 0.5f) ;
        rawImage.uvRect = _rect;


        if (_isPlay == true)
        {
            if (_count > 0)
                _count -= Time.deltaTime;
            _counterText.text = _count.ToString("F1");
        }

        //Vector2 _d_2 = _edge.GetComponent<RectTransform>().anchoredPosition;
        //_d_2.x = _d.x;
        //_edge.GetComponent<RectTransform>().anchoredPosition = _d_2;

    }
}
