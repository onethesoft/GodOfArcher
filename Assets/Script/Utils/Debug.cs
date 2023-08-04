using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class Debug 
{

    public static ILogger unityLogger = UnityEngine.Debug.unityLogger;
    static UI_DebugView _view = null;


    public static bool isDebugBuild 
    { 
        
        get 
        {
#if ENABLE_LOG
            return true;
#else
            return false;
#endif
        } 
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void CreateLogView(GameObject parent)
    {
        _view = Util.GetOrAddComponent<UI_DebugView>(Managers.Resource.Instantiate("UI/SubItem/UI_DebugView", parent.transform));
        //if(File.Exists())

    }
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Clear()
    {
        _view = null;

    }
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
        
        UnityEngine.Debug.Log(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message, Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);

    }
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarningFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);

    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityEngine.Debug.DrawRay(start, dir, color);
    }


    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void ClearDeveloperConsole()
    {
        UnityEngine.Debug.ClearDeveloperConsole();

    }
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogException(System.Exception ex)
    {
        UnityEngine.Debug.LogException(ex);

    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);

    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Assert(bool condition, string? message)
    {
        UnityEngine.Debug.Assert(condition,message);

    }

    static void WriteLog(string content)
    {

    }
}
