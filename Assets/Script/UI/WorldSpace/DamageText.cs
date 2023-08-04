using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Numerics;
using TMPro;

public class DamageText : UI_Base
{
    [SerializeField]
    GameObject Text;
    
    public void SetDamage(string damage , Define.DamageType type = Define.DamageType.Normal)
    {
        
        Text.GetComponent<TextMeshProUGUI>().text = damage;

        
        
        if (type == Define.DamageType.Normal)
            Text.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        else if (type == Define.DamageType.Critical)
            Text.GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0,255);
        else
            Text.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 0, 255);

    



    }
    
    public void PlayeAnimation()
    {
      
        GetComponent<DOTweenAnimation>().DORestart();

        Text.GetComponent<DOTweenAnimation>().DORestartById("Fade");
        Text.GetComponent<DOTweenAnimation>().DORestartById("Punch");
    }

    public void OnCompleteMove()
    {
        Managers.Resource.Destroy(gameObject);
    }

    public override void Init()
    {
        return;
    }
    private void OnDestroy()
    {
        GetComponent<DOTweenAnimation>().DOKill();

        Text.GetComponent<DOTweenAnimation>().DOKillById("Fade");
        Text.GetComponent<DOTweenAnimation>().DOKillById("Punch");
    }
}
