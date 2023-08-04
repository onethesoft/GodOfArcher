using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager 
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (prefab.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(prefab, parent).gameObject;

        return Object.Instantiate(prefab, parent);
    }

    public GameObject Instantiate(GameObject obj, Transform parent = null)
    {
        if (obj == null)
        {
            Debug.Log($"Failed to Instantiate GameObject");
            return null;
        }

        if (obj.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(obj, parent).gameObject;

        return Object.Instantiate(obj, parent);

    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }

}
