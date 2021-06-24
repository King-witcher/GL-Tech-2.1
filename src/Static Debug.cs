using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GLTech2
{
    /// <summary>
    ///     Provides an interface for printing text on the screen.
    /// </summary>
    public static class Debug
    {
        static bool consoleEnabled = false;

        /// <summary>
        /// Enables a console that can be used to output and input text.
        /// </summary>
        public static void EnableConsole()
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool AllocConsole();

            AllocConsole();
            consoleEnabled = true;
        }

        /// <summary>
        /// Disables the console.
        /// </summary>
        public static void DisableConsole()
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool FreeConsole();

            FreeConsole();
            consoleEnabled = false;
        }

        /// <summary>
        ///     Prints a message on the screen given an option.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="debugOption">Option</param>
        public static void Log(string message = "", Options debugOption = Options.Message)
        {
            if (!consoleEnabled)
                return;

            ConsoleColor prev = Console.ForegroundColor;

            Console.ForegroundColor = debugOption switch
            {
                Options.Message => ConsoleColor.White,
                Options.Info => ConsoleColor.Cyan,
                Options.Warning => ConsoleColor.DarkYellow,
                Options.Error => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };

            Console.WriteLine(message);

            Console.ForegroundColor = prev;
        }

        /// <summary>
        /// Reads text from the debug console, if enabled. Otherwise, will always return an empty string.
        /// </summary>
        /// <returns></returns>
        public static string Read()
        {
            if (!consoleEnabled)
                return string.Empty;

            return Console.ReadLine();
        }

        static internal void InternalLog(string origin, string message, Options debugOption = Options.Message)
        {
            if (!consoleEnabled)
                return;

            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(origin + ": ");
            Log(message, debugOption);
            Console.ForegroundColor = prev;
        }

        /// <summary>
        /// Specifies constants that define the details of how a message should be printed.
        /// </summary>
        public enum Options
        {
            Message,
            Info,
            Warning,
            Error,
        }
    }
}
