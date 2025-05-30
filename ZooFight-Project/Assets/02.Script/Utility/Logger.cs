using System.Diagnostics;

public static class Logger
{
    [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
    public static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
    public static void LogWarning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
}
