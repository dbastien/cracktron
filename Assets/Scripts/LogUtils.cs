using UnityEngine;

public static class LogUtils
{
    public static void Log(LogType logType, object message)
    {
        Debug.unityLogger.Log(logType, message);
    }

    public static void Log(LogType logType, object message, Object context)
    {
        Debug.unityLogger.Log(logType, message, context);
    }

    public static void LogFormat(LogType logType, string format, params object[] args)
    {
        Debug.unityLogger.LogFormat(logType, format, args);
    }

    public static void LogFormat(LogType logType, Object context, string format, params object[] args)
    {
        Debug.unityLogger.LogFormat(logType, context, format, args);
    }}