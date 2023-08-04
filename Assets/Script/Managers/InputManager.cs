using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager 
{
    public Action<Define.MouseEvent> MouseAction = null;
    bool _isPressed = false;

    UI_SimpleMessageBox _closePopup = null;

    public void OnUpdate()
    {

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape))
        {
            if (_closePopup == null && Managers.Scene.CurrentScene.SceneType == Define.Scene.Main)
            {
                _closePopup = Managers.UI.ShowPopupUI<UI_SimpleMessageBox>();
                _closePopup.Text = "게임을 정말 종료하시곘습니까?";
                _closePopup.Cancel += () => { _closePopup = null; };
                _closePopup.OK += () => {
                    UnityEngine.Object.FindObjectOfType<GameDataUpdater>().ManualUpdateUser();
                    Application.Quit();
                };
            }
        }

        if (MouseAction != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

           
            if (Input.GetMouseButton(0))
            {
                MouseAction.Invoke(Define.MouseEvent.Pressed);
                _isPressed = true;
            }
            else
            {
                if (_isPressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.Click);
                    _isPressed = false;
                }
            }
            //MouseAction.Invoke();
        }


#else
       if(Input.GetKey(KeyCode.Escape))
        {
            if(_closePopup == null && Managers.Scene.CurrentScene.SceneType == Define.Scene.Main)
            {
                _closePopup = Managers.UI.ShowPopupUI<UI_SimpleMessageBox>();
                _closePopup.Text = "게임을 정말 종료하시곘습니까?";
                _closePopup.Cancel += () => { _closePopup = null; };
                _closePopup.OK += () => {
                    UnityEngine.Object.FindObjectOfType<GameDataUpdater>().ManualUpdateUser();
                    Application.Quit();
                };
            }
        }

        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
        }
        else
            return;

        


        if (MouseAction != null)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButton(0))
            {
                MouseAction.Invoke(Define.MouseEvent.Pressed);
                _isPressed = true;
            }
            else
            {
                if (_isPressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.Click);
                    _isPressed = false;
                }
            }
            
        }
#endif
    }

    public void Clear()
    {
        //MouseAction = null;
    }
}
