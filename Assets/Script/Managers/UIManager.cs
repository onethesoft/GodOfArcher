using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public enum Category
    {
        UI_Stat,
        UI_ShopPopup,
        UI_RunePopup,
        UI_PetPopup,
        UI_Item,
        UI_Artifact
    }

    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    Dictionary<Category, System.Type> _dictCategory = new Dictionary<Category, Type>();

    public void Init()
    {
        foreach(Category v in Enum.GetValues(typeof(Category)))
        {
            _dictCategory.Add(v, Type.GetType(v.ToString()));

        }
    }


    public GameObject UI_Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }

            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.overrideSorting = true;
        //canvas.worldCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
       

        if (sort)
        {
            canvas.sortingOrder = (_order);
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }

    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);

        go.transform.SetParent(UI_Root.transform);


        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(UI_Root.transform);

        return popup;
    }
    public void ShowPopupUI(Category cat)
    {
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{cat}");
        Component _get = Util.GetOrAddComponent(go, _dictCategory[cat]);
        _popupStack.Push((UI_Popup)Convert.ChangeType(_get, _dictCategory[cat]));
        go.transform.SetParent(UI_Root.transform);

    }
    
    
    public bool ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return true;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return false;
        }

        ClosePopupUI();
        return true;
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
    }

}
