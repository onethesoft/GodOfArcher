using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayfabExecuter : MonoBehaviour
{
    Queue<PlayfabSender> _pooling;
    List<PlayfabSender> _execute;
    List<PlayfabSender> _waitUntilExecute;

    int MaxCallsAPICountFor = 5;

    void Start()
    {
        _pooling = new Queue<PlayfabSender>();
        _execute = new List<PlayfabSender>();
        _waitUntilExecute = new List<PlayfabSender>();
    }
    public void Execute(PlayfabSender sender)
    {
        _waitUntilExecute.Add(sender);
    }

    public PlayfabSender Pop()
    {
        if (_pooling.Count > 0)
            return _pooling.Dequeue();

        return new PlayfabSender();
    }

    public void Push(PlayfabSender sender)
    {
        _pooling.Enqueue(sender);
    }
    public int GetWaitCommandQueueCount()
    {
        return _waitUntilExecute.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(_waitUntilExecute.Count > 0 && _execute.Count < MaxCallsAPICountFor)
        {
            PlayfabSender _sender = _waitUntilExecute.First();

            _execute.Add(_sender);
            _waitUntilExecute.Remove(_sender);

            _sender.Execute();
        }

        if(_execute.Any(x=>x.status == PlayfabSender.Status.Exit))
        {
            List<PlayfabSender> _exitList = _execute.Where(x => x.status == PlayfabSender.Status.Exit).ToList();
            foreach(PlayfabSender sender in _exitList)
            {
                Push(sender);
                _execute.Remove(sender);
            }
            
        }
    }
}
