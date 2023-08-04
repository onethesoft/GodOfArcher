using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_ButtonBlocker : MonoBehaviour
{

    [SerializeField]
    Button _button;

    [SerializeField]
    float _blockSecond = 1.0f;

    float duration;
    bool isStarted = false;
    private void Start()
    {
        gameObject.SetActive(false);
        duration =  Time.deltaTime / _blockSecond;
      
    }

    public void StartBlocker()
    {
        GetComponent<Image>().fillAmount = 1;
        gameObject.SetActive(true);
        isStarted = true;
    }

    private void Update()
    {
        if(isStarted)
        {
            GetComponent<Image>().fillAmount -= duration;
            if(GetComponent<Image>().fillAmount <= 0)
            {
                isStarted = false;
                GetComponent<Image>().fillAmount = 0;
                gameObject.SetActive(false);
            }
        }
    }






}
