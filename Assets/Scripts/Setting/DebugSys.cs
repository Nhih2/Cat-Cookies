using UnityEngine;
using System.Runtime.CompilerServices;

public static class DebugSys
{
    public static void Log(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
        Debug.Log($"[{className}.{memberName}:{lineNumber}] {message}");
    }

    public static void LogWarning(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
        Debug.LogWarning($"[{className}.{memberName}:{lineNumber}] {message}");
    }

    public static void LogError(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
        Debug.LogError($"[{className}.{memberName}:{lineNumber}] {message}");
    }
}
