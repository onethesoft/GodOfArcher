using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UI_DamageText : UI_Base
{
    Dictionary<Define.DamageType, Color32> _fontColors;
    Define.DamageType _type;

    DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _moveTween = null;
    private void Awake()
    {
        
        _fontColors = new Dictionary<Define.DamageType, Color32>();
        _fontColors.Add(Define.DamageType.Normal, new Color32(255, 255, 255, 255));
        _fontColors.Add(Define.DamageType.Critical, new Color32(255, 0, 0, 255));
        _fontColors.Add(Define.DamageType.Skill, new Color32(255, 255, 0, 255));

        //Get<DOTweenAnimation>().recycle
    }
    public override void Init()
    {
        //GetComponent<Text>().color = _fontColors[_type];
        GetComponent<TextMeshPro>().color = _fontColors[_type];
        //GetComponent<TextMeshProUGUI>().color = _fontColors[_type];
    }
    public void SetDamage(string damage, Define.DamageType type = Define.DamageType.Normal)
    {
        GetComponent<TextMeshPro>().color = _fontColors[_type];
        //GetComponent<Text>().color = _fontColors[_type];
        //GetComponent<Text>().text = damage;
         GetComponent<TextMeshPro>().text = damage;
        _type = type;

        if (_type != Define.DamageType.Skill)
            GetComponent<TextMeshPro>().fontSize = 6;

    }
   
    public void Play()
    {
        GetComponent<TextMeshPro>().color = _fontColors[_type];
        //GetComponent<Text>().color = _fontColors[_type];

        if (_moveTween == null)
            _moveTween = transform.DOMoveY(transform.position.y + 1.0f, 1.5f).SetRecyclable(true).OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject);
            }).SetAutoKill(false).SetEase(Ease.OutQuad);
        else
        {
            _moveTween.startValue = gameObject.transform.position;
            _moveTween.endValue = gameObject.transform.position + 3*Vector3.up;
            _moveTween.Restart();
        }

        GetComponent<DOTweenAnimation>().DORestartById("Fade");


    }
    void onComplete()
    {

    }

    public void OnCompleteMove()
    {
        
       
        
    }
    private void OnDestroy()
    {

        _fontColors.Clear();
        GetComponent<DOTweenAnimation>().DOKillById("Fade");
     
    }
}
