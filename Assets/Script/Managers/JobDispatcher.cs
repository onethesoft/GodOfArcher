using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;


public class DuplicateKeyComparer<T> : IComparer<T> where T : IComparable
{
    public int Compare(T x, T y)
    {
        int ret = x.CompareTo(y);
        if (ret == 0)
            return 1;
        else
            return ret;
    }
}

public class JobDispatcher : MonoBehaviour
{
    private object _lock = new object();
    private SortedList<DateTime, Action> _queue = new SortedList<DateTime, Action>(new DuplicateKeyComparer<DateTime>());
    private List<Action> _items = new List<Action>();


    public class JobItem
    {
        DateTime _excuteTime;
        Func<object[], bool> _callback;
        public JobItem(params object[] _params)
        {
            _callback?.Invoke(_params);
        }
    }
    


    public void Reserve(DateTime executetime, Action<object[]> job, params object [] args)
    {
        lock (_lock)
            _queue.Add(executetime, ()=> { StartCoroutine(JobHandlerWrapper(job, args)); });
    }

    public void Reserve(DateTime executetime, Action job)
    {
        lock (_lock)
            _queue.Add(executetime, () => { StartCoroutine(JobHandlerWrapper(job)); });
    }



    public void Distribute(DateTime now)
    {
        if (_queue.Count == 0)
            return;

        lock (_lock)
        {
            while (_queue.Count != 0)
            {
                if (now < _queue.First().Key)
                    break;

                _items.Add(_queue.First().Value);
                _queue.RemoveAt(0);
            }

            foreach (Action item in _items)
                item?.Invoke();

            _items.Clear();

        }
    }

    IEnumerator JobHandlerWrapper(Action<object[]> job , params object [] args)
    {
        job(args);
        yield return null;
    }
    IEnumerator JobHandlerWrapper(Action job)
    {
        job();
        yield return null;
    }

    private void Update()
    {
        Distribute(GlobalTime.Now);
    }
    private void OnDestroy()
    {
        _items.Clear();
        _queue.Clear();
    }


}
