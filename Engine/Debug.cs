using System.Runtime.InteropServices;
using System.Text;

namespace GLTech
{
    internal struct LogEntry
    {
        public string message;
        public Color color;
    }

    public static class Debug
    {
        #region public
        public enum Options { Normal, Success, Warning, Error }
        public static int VisibleWalls { get; internal set; } = 0;
        /// <summary>
        /// This field is delayed by one frame when running scripts.
        /// </summary>
        public static float ScriptTime { get; internal set; } = 0f;
        public static float WallCullTime { get; internal set; } = 0f;
        public static float WallRenderTime { get; internal set; } = 0f;
        public static float PresentTime { get; internal set; } = 0f;
        public static float RenderTime { get; internal set; } = 0f;

        public static void Log(string message, Color color) =>
            LogsList.Add(new LogEntry { message = message, color = color });

        public static void Log(string message = "") =>
            Log(message, Color.White);

        public static void LogTerminal()
        {
            if (terminalEnabled) Console.WriteLine();
        }

        public static void LogTerminal(object message, string? context = null, Options debugOption = Options.Normal)
        {
            if (!terminalEnabled) return;

            ConsoleColor prev = Console.ForegroundColor;

            Console.ForegroundColor = debugOption switch
            {
                Options.Normal => ConsoleColor.Gray,
                Options.Success => ConsoleColor.Green,
                Options.Warning => ConsoleColor.DarkYellow,
                Options.Error => ConsoleColor.DarkRed,
                _ => ConsoleColor.White,
            };

            var logBuilder = new StringBuilder(40);
            logBuilder.Append(debugOption switch
            {
                Options.Normal => "       ",
                Options.Success => "[SUCC] ",
                Options.Warning => "[WARN] ",
                Options.Error => "[ERRO] ",
                _ => "",
            });

            if (context != null && context.Length > 0)
            {
                logBuilder.Append(context);
                logBuilder.Append(new String(' ', Math.Max(1, 20 - context.Length)));
            }

            logBuilder.Append(message);

            Console.WriteLine(logBuilder.ToString());
            Console.ForegroundColor = prev;
        }
        #endregion

        #region internal
        internal static Span<LogEntry> LogEntries => CollectionsMarshal.AsSpan(LogsList);

        /// <summary>
        /// Clears all log entries after being rendered.
        /// Must always be called after rendering.
        /// </summary>
        internal static void ClearLogs()
        {
            LogsList.Clear();
        }
        #endregion

        #region private
        private static List<LogEntry> LogsList = new();

        private static bool terminalEnabled = false;
        #endregion
    }
}