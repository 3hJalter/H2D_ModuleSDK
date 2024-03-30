using System.Collections.Generic;
using UnityEngine;

namespace HoangHH
{
    public enum LogType
    {
        None = -1,
        Todo = 0,
        Test = 1,
    }
    
    public static class HDebug
    {
        private static readonly Dictionary<LogType, string> LOGColors = new()
        {
            {LogType.None, "#FF0000"},
            {LogType.Todo, "#00C3FF"},
            {LogType.Test, "#FF00FF"},
        }; // Add Color for LogType here


        /// <summary>
        /// Log with type and log content.
        /// NOTE: Log will build show in Unity Editor only in PROTOTYPE build.
        /// When build to RELEASE and you want to hide all logs, remove PROTOTYPE define in Player Settings.
        /// </summary>
        public static void Log(LogType type, string content)
        {
#if PROTOTYPE
            Debug.Log($"<color={LOGColors[type]}>[{type}] {content}</color>");
#endif
        }
    }
}
