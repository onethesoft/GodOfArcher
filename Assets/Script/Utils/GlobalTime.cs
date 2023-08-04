using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTime : MonoBehaviour
{
    private static AndroidJavaClass callClass = null;
    private static DateTime? gTime;
    public static DateTime Now 
    { 
        get
        {
            return gTime.HasValue == true ? gTime.Value + TimeSpan.FromMilliseconds(ElapsedRealTime()) : DateTime.Now;
        }
    }

    
    private static long ElapsedRealTime()
    {
#if UNITY_EDITOR
        return (long)TimeSpan.FromSeconds((double)Time.realtimeSinceStartup).TotalMilliseconds;
#elif UNITY_ANDROID
        if (callClass == null)
                callClass = new AndroidJavaClass("android.os.SystemClock");
            return callClass.CallStatic<long>("elapsedRealtime");
#endif
        
    }

    public static void SetSyncTime(DateTime time)
    {
        gTime = DateTime.SpecifyKind(time , DateTimeKind.Utc);
        gTime -= TimeSpan.FromMilliseconds(ElapsedRealTime());
      
    }
}
