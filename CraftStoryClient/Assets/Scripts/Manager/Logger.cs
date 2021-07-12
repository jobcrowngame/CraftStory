using System;
using UnityEngine;

public class Logger : Single<Logger>
{
#if UNITY_EDITOR
    private static LogLV level = LogLV.Warning;
#else
    private static LogLV level = LogLV.Error;
#endif

    public static void Log(string format, params object[] args)
    {
        if (level < LogLV.Warning)
            Debug.LogFormat("[LOG]" + format, args);
    }
    public static void Log(Vector2 v2)
    {
        if (level < LogLV.Warning)
            Debug.LogFormat("[LOG]" + v2);
    }
    public static void Log(Vector2Int v2)
    {
        if (level < LogLV.Warning)
            Debug.LogFormat("[LOG]" + v2);
    }
    public static void Log(object v2)
    {
        if (level < LogLV.Warning)
            Debug.LogFormat("[LOG]" + v2);
    }
    public static void Warning(string format, params object[] args)
    {
        if (level < LogLV.Error)
            Debug.LogWarningFormat("[WARNING]" + format, args);
    }
    public static void Error(string format, params object[] args)
    {
        if (level < LogLV.Off)
            Debug.LogErrorFormat("[ERROR]" + format, args);
    }
    public static void Error(Exception e)
    {
        if (level < LogLV.Off)
            Debug.LogErrorFormat("[ERROR]" + e.Message);
    }

    enum LogLV
    {
        Log,
        Warning,
        Error,
        Off,
    }
}
