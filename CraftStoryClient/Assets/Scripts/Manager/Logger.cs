using UnityEngine;

public class Logger : Single<Logger>
{
    public void Log(string format, params object[] args)
    {
        Debug.LogFormat("[LOG]" + format, args);
    }
    public void Warning(string format, params object[] args)
    {
        Debug.LogWarningFormat("[WARNING]" + format, args);
    }
    public void Error(string format, params object[] args)
    {
        Debug.LogErrorFormat("[ERROR]" + format, args);
    }
}
