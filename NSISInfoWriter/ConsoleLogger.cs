using System;

namespace NSISInfoWriter
{
    public enum LogLevel { Info, Warning, Error }

    /// <summary>
    /// Simplest console logger ever
    /// </summary>
    public static class ConsoleLogger
    {
        public static bool IsEnabled { get; set; } = false;

        public static void Log(string message, LogLevel level) {
            if (!IsEnabled) {
                return;
            }
            ConsoleColor color = Console.ForegroundColor;
            switch (level) {
                case LogLevel.Info:
                    color = ConsoleColor.Cyan;
                    break;
                case LogLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;
            }
            var time = DateTime.Now.ToString("g");
            var fullMessage = $"[ {level,-7} ] [{time}] {message}";
            Helpers.ShowColor(fullMessage, color);

        }

        public static void LogInfo(string msg) => Log(msg, LogLevel.Info);
        public static void LogWarn(string msg) => Log(msg, LogLevel.Warning);
        public static void LogError(string msg) => Log(msg, LogLevel.Error);
    }
}
