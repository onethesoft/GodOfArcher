using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_AnimatedCurrency : UI_Base
{
    public enum Animation
    {
        Fade,
        Move,
    }

    
    public string Text
    {
        set
        {
            if(GetComponent<Text>() != null)
                GetComponent<Text>().text = value;
        }
    }
    public void SetTextColor(Color textColor)
    {
        GetComponent<Text>().color = textColor;
    }
    public override void Init()
    {
       
    }
    private void OnEnable()
    {
        GetComponent<DOTweenAnimation>().DORestartById(Animation.Move.ToString());
        GetComponent<DOTweenAnimation>().DORestartById(Animation.Fade.ToString());
    }

    public void OnCompleted()
    {
        Managers.Resource.Destroy(gameObject);
    }

    
}
