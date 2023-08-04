using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_DebugView : UI_Base 
{

    class DebugTop : MonoBehaviour, IDragHandler , IBeginDragHandler , IEndDragHandler
    {
        bool isDrag = false;
        GameObject parent;
        Vector2 _pressPos;
        Vector3 _objectPos;
        public void setParent(GameObject parent)
        {
            this.parent = parent;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (isDrag)
            {
                Vector2 diff = (eventData.position - _pressPos);
                parent.transform.position = _objectPos + (Vector3)diff;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDrag)
            {
                isDrag = true;
                _pressPos = eventData.position;
                _objectPos = parent.transform.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isDrag)
            {
                isDrag = false;
            }
        }
    }

    int Limit = 200;
 
    
    
    enum GameObjects
    {
        Content,
        View,
        DebugLogText
    }
    enum Buttons
    {
        Top ,
        Button,
        AutoLogButton
    }
   
    Queue<TextMeshProUGUI> _logTexts;
    Coroutine _logCoroutine = null;
    bool _isAuto = false;
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _logTexts = new Queue<TextMeshProUGUI>();

        GetButton((int)Buttons.Top).gameObject.AddComponent<DebugTop>().setParent(gameObject);
        Application.logMessageReceived += HandleLog;

        _logCoroutine = StartCoroutine(ClearLog());

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            if (Get<GameObject>((int)GameObjects.View).gameObject.activeSelf)
                Get<GameObject>((int)GameObjects.View).gameObject.SetActive(false);
            else
                Get<GameObject>((int)GameObjects.View).gameObject.SetActive(true);
        });

      
        AddUIEvent(GetButton((int)Buttons.AutoLogButton).gameObject, (data) => {
            _isAuto = !_isAuto;
            GetButton((int)Buttons.AutoLogButton).GetComponentInChildren<Text>().text = $"자동 로그 지우기 {(_isAuto ? "on" : "off")}";


        });


    }
    IEnumerator ClearLog()
    {
        while (true)
        {
            yield return new WaitForSeconds(120);
            if (_isAuto)
            {
                Get<GameObject>((int)GameObjects.DebugLogText).GetComponent<TextMeshProUGUI>().text = "";
                LayoutRebuilder.ForceRebuildLayoutImmediate(Get<GameObject>((int)GameObjects.Content).GetComponent<RectTransform>());
            }
        }
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string colorCode;
        if(type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
            colorCode = ColorUtility.ToHtmlStringRGBA(Color.red);
        else if( type == LogType.Warning)
            colorCode = ColorUtility.ToHtmlStringRGBA(Color.yellow);
        else 
            colorCode = ColorUtility.ToHtmlStringRGBA(Color.white);



        if ((type == LogType.Error || type == LogType.Assert) && Get<GameObject>((int)GameObjects.View).gameObject.activeSelf == false)
        {
            Get<GameObject>((int)GameObjects.View).gameObject.SetActive(true);
        }


        Get<GameObject>((int)GameObjects.DebugLogText).GetComponent<TextMeshProUGUI>().text += $"<color=#{colorCode}>[{DateTime.Now.ToString()}] {logString}</color>" + System.Environment.NewLine;
        Get<GameObject>((int)GameObjects.DebugLogText).GetComponent<TextMeshProUGUI>().text += $"<color=#{colorCode}>{stackTrace}</color>" + System.Environment.NewLine;
       
      
        

        LayoutRebuilder.ForceRebuildLayoutImmediate(Get<GameObject>((int)GameObjects.Content).GetComponent<RectTransform>());

        
    }
    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
        StopAllCoroutines();
    }


}
