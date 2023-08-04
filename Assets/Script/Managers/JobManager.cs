using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;

public class JobManager 
{
    const string ObjectName = "JobDispatcher";
    JobDispatcher _dispatcher = null;

    public void Init()
    {
        if(GameObject.Find(ObjectName) == null)
        {
            GameObject _job = new GameObject { name = ObjectName };
            _dispatcher = Util.GetOrAddComponent<JobDispatcher>(_job);
            UnityEngine.Object.DontDestroyOnLoad(_job);
        }

        
    }
    
    public void ReserveJob(TimeSpan tickCount , Action<object []> job , params object [] obj)
    {
        
        Debug.Assert(_dispatcher != null, "_dispatcher is not exist");
        
        _dispatcher.Reserve(GlobalTime.Now + tickCount, job , obj);


    }
    public void ReserveJob(TimeSpan tickCount, Action job)
    {

        Debug.Assert(_dispatcher != null, "_dispatcher is not exist");

        _dispatcher.Reserve(GlobalTime.Now + tickCount, job);


    }



}
