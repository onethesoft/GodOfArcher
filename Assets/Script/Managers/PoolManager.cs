
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour 
{
    Transform _root = null;
    Dictionary<string, Pool> _pool;
    Dictionary<System.Type, Stack<object>> _objectPool;

    

    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int Count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < Count; i++)
            {

            }
        }
        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return Util.GetOrAddComponent<Poolable>(go);
        }
        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.gameObject.SetActive(false);
            poolable.transform.SetParent(Root);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            if (parent == null)
                poolable.transform.SetParent(Managers.Scene.CurrentScene.transform);
            else
                poolable.transform.SetParent(parent);

            poolable.gameObject.SetActive(true);

            

            poolable.IsUsing = true;

            return poolable;

        }

    }


    public void Init()
    {
        _pool = new Dictionary<string, Pool>();
        _objectPool = new Dictionary<System.Type, Stack<object>>();
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }
    public void CreatePool(GameObject Original, int Count = 5)
    {
        Pool pool = new Pool();
        pool.Init(Original, Count);
        pool.Root.parent = _root;

        _pool.Add(Original.name, pool);


    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (!_pool.ContainsKey(name))
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }
    public void Push(object obj)
    {
        System.Type type = obj.GetType();
        if(!_objectPool.ContainsKey(type))
        {
            _objectPool[type] = new Stack<object>();
        }
        _objectPool[type].Push(obj);
    }
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (!_pool.ContainsKey(original.name))
        {
            CreatePool(original);
        }

        return _pool[original.name].Pop(parent);
    }

    public T Pop<T>() where T : new ()
    {
        System.Type type = typeof(T);
        Stack<object> stack;

        if(_objectPool.TryGetValue(type , out stack) && stack.Count > 0)
        {
            return (T)stack.Pop();
        }
        else
        {
            return new T();
        }
            
    }
    

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }


    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
