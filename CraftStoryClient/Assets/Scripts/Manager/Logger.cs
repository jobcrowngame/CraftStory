using System;
using UnityEngine;

public class Logger : Single<Logger>
{
#if UNITY_EDITOR
    private static LogLV level = LogLV.Log;
#else
    private static LogLV level = LogLV.Error;
#endif

    public static void Log(string format, params object[] args)
    {
        if (level < LogLV.Warning)
        {
            Debug.LogFormat("[LOG]" + format, args);
            DebugLG.E.Add(string.Format("[LOG]" + format, args));
        }
    }
    public static void Log(Vector2 v2)
    {
        Log("[LOG]" + v2);
    }
    public static void Log(object v2)
    {
        Log("[LOG]" + v2);
    }
    public static void Warning(string format, params object[] args)
    {
        if (level < LogLV.Error)
        {
            Debug.LogWarningFormat("[WARNING]" + format, args);
            DebugLG.E.Add(string.Format("[WARNING]" + format, args));
        }
    }
    public static void Error(string format, params object[] args)
    {
        if (level < LogLV.Off)
        {
            string msg = string.Format("[ERROR][ACC:" + DataMng.E.UserData.Account + "]" + format, args);
            Debug.LogErrorFormat(msg);
            DebugLG.E.Add(msg);
            NWMng.E.ShowClientLog(msg);
        }
    }
    public static void Error(Exception e)
    {
        Error("[ERROR]" + e.Message);
    }

    enum LogLV
    {
        Log,
        Warning,
        Error,
        Off,
    }
}
