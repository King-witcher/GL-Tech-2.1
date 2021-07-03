using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    ///     Provides an interface for printing text on the screen.
    /// </summary>
    public static class Debug
    {
        static bool consoleEnabled = false;

        /// <summary>
        /// Gets and sets whether the console should be enabled or not.
        /// </summary>
        public static bool ConsoleEnabled
        {
            get => consoleEnabled;
            set
            {
                if (value)
                    EnableConsole();
                else
                    DisableConsole();
            }
        }

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
        /// Clears the console.
        /// </summary>
        public static void Clear() =>
            Console.Clear();

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
        /// Pauses the execution of the engine until the user presses a key on the console, if enabled.
        /// </summary>
        public static void Pause()
        {
            if (!consoleEnabled)
                return;

            Console.ReadKey();
            Console.Write("\b \b");
        }

        /// <summary>
        /// Prints a message and pauses the execution of the engine until the user presses a key on the console, if enabled.
        /// </summary>
        /// <param name="message">Pause message</param>
        public static void Pause(string message)
        {
            if (!consoleEnabled)
                return;

            Console.Write(message);
            Pause();
            Console.WriteLine();
        }

        /// <summary>
        /// Pauses the execution of engine until the user inserts a text on the console, if enabled, and then return it; if disabled, will always return an empty string.
        /// </summary>
        /// <returns>The string typed by the user on the console, if enabled; otherwise, string.Empty</returns>
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
