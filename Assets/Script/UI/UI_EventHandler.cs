using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler 
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnPressHandler = null;
    public Action<PointerEventData> OnReleaseHandler = null;




    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnPressHandler != null)
            OnPressHandler.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnReleaseHandler != null)
            OnReleaseHandler.Invoke(eventData);
    }
}
