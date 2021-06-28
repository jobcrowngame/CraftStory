using System;
using UnityEngine;

public class Logger : Single<Logger>
{
    public static void Log(string format, params object[] args)
    {
        Debug.LogFormat("[LOG]" + format, args);
    }
    public static void Log(Vector2 v2)
    {
        Debug.LogFormat("[LOG]" + v2);
    }
    public static void Log(Vector2Int v2)
    {
        Debug.LogFormat("[LOG]" + v2);
    }
    public static void Log(object v2)
    {
        Debug.LogFormat("[LOG]" + v2);
    }
    public static void Warning(string format, params object[] args)
    {
        Debug.LogWarningFormat("[WARNING]" + format, args);
    }
    public static void Error(string format, params object[] args)
    {
        Debug.LogErrorFormat("[ERROR]" + format, args);
    }
    public static void Error(Exception e)
    {
        Debug.LogErrorFormat("[ERROR]" + e.Message);
    }
}
