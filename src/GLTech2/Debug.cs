using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    /// Provides an interface for printing text on the screen.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Clears the console.
        /// </summary>
        public static void Clear()
        {
            if (enabled)
                Console.Clear();
        }

        /// <summary>
        /// Specifies constants that define the details of how a message should be printed.
        /// </summary>
        public enum Options { Normal, Success, Warning, Error }

        /// <summary>
        ///     Prints a message on the screen given an option.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="debugOption">Option</param>
        public static void Log(string message = "", Options debugOption = Options.Normal)
        {
            if (!enabled)
                return;

            ConsoleColor prev = Console.ForegroundColor;

            Console.ForegroundColor = debugOption switch
            {
                Options.Normal => ConsoleColor.Gray,
                Options.Success => ConsoleColor.Green,
                Options.Warning => ConsoleColor.DarkYellow,
                Options.Error => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };

            string pre = debugOption switch
            {
                Options.Normal => "",
                Options.Success => "",
                Options.Warning => "[WARNING]: ",
                Options.Error => "[ERROR]: "
            };

            Console.WriteLine(pre + message);

            Console.ForegroundColor = prev;
        }

        /// <summary>
        /// Pauses the execution of the engine until the user presses a key on the console, if enabled.
        /// </summary>
        public static void Pause()
        {
            if (!enabled)
                return;

            Console.Write("Press any key to continue...");

            Console.ReadKey();
            Console.Write("\b \b");
        }

        /// <summary>
        /// Pauses the execution of engine until the user inserts a text on the console, if enabled, and then return it; if disabled, will always return an empty string.
        /// </summary>
        /// <returns>The string typed by the user on the console, if enabled; otherwise, string.Empty</returns>
        public static string Read()
        {
            if (!enabled)
                return string.Empty;

            return Console.ReadLine();
        }

        static internal void InternalLog(string message, Options debugOption = Options.Normal)
        {
            if (!enabled)
                return;

            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"Internal Log:");
            Log(message, debugOption);
            Console.ForegroundColor = prev;
        }

        private static bool enabled = false;

        internal static void Enable()
        {
            AllocConsole();
            enabled = true;

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool AllocConsole();
        }

        internal static void Disable()
        {
            FreeConsole();
            enabled = false;

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool FreeConsole();
        }

    }
}
