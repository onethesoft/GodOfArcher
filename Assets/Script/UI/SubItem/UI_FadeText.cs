using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FadeText : UI_Base
{
    public string text { get; set; }
    enum Texts
    {
        Text
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        gameObject.transform.position = gameObject.transform.parent.transform.position;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 0, y = 100 };
        gameObject.GetComponent<RectTransform>().localScale = new Vector3 { x = 1.0f, y = 1.0f, z = 1.0f };

        

        GetText((int)Texts.Text).text = text;

        GetText((int)Texts.Text).gameObject.GetComponent<DOTweenAnimation>().DORestart();
        GetComponent<DOTweenAnimation>().DOPlay();
    }

    private void OnEnable()
    {

       
        if (gameObject.transform.parent != null)
        {

            gameObject.transform.position = gameObject.transform.parent.transform.position;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 0, y = 100 };
            gameObject.GetComponent<RectTransform>().localScale = new Vector3 { x = 1.0f, y = 1.0f, z = 1.0f };
        }

        if (GetText((int)Texts.Text) != null)
        {
            GetText((int)Texts.Text).text = text;
            GetText((int)Texts.Text).gameObject.GetComponent<DOTweenAnimation>().DORestart();
        }
        GetComponent<DOTweenAnimation>().DORestart();

    }
    public void RePlay()
    {
        GetText((int)Texts.Text).gameObject.GetComponent<DOTweenAnimation>().DORestart();
        GetComponent<DOTweenAnimation>().DORestart();
    }
    public void Destroy()
    {
        Managers.Resource.Destroy(gameObject);
    }


}
