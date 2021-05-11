using System;
using UnityEngine;

public class MLog : Single<MLog>
{
    public void Init()
    {

    }

    public static void Log(string msg)
    {
        Debug.Log(msg);
    }

    public static void Warning(string msg)
    {
        Debug.LogWarning(msg);
    }

    public static void Error(string msg)
    {
        Debug.LogError(msg);
    }
}