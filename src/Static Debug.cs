using System;
using System.Text.RegularExpressions;

namespace GLTech2
{
    /// <summary>
    ///     Provides an interface for printing text on the screen.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        ///     Gets and sets if the Debug should print system warnings. Obsolete.
        /// </summary>
        [Obsolete]
        public static bool DebugWarnings { get; set; } = true;

        [Obsolete]
        internal static void LogWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (DebugWarnings)
                System.Console.WriteLine(warning);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
        }

        /// <summary>
        ///     Prints a message on the screen.
        /// </summary>
        /// <param name="message">Message</param>
        public static void Log(string message)
        {
            System.Console.WriteLine(message);
            Console.WriteLine();
        }

        /// <summary>
        ///     Prints a message on the screen given an option.
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="option">Option</param>
        public static void Log(string text, Options option)
        {
            ConsoleColor prev = Console.ForegroundColor;

            switch(option)
            {
                case Options.Message:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(text);
                    break;

                case Options.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(text);
                    break;

                case Options.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(text);
                    break;

                case Options.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(text);
                    break;
            }
            Console.WriteLine();
            Console.ForegroundColor = prev;
        }

        static internal void InternalLog(string source, string message, Options options)
        {
            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(source + ": ");
            Log(message, options);
            Console.ForegroundColor = prev;
        }

#pragma warning disable
        // Suboptimal and not well coded.
        static void idPrint(string str)
        {
            Regex rgx = new Regex(@"\^[0-7]");
            ConsoleColor[] colorMap =
            {
                ConsoleColor.Black,
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.DarkYellow,
                ConsoleColor.Blue,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.White
            };

            string[] segments = rgx.Split(str);
            ConsoleColor[] colors = new ConsoleColor[segments.Length];

            {
                colors[0] = ConsoleColor.White;
                int i = 1;
                foreach (Match match in rgx.Matches(str))
                {
                    string tag = match.Value;
                    int index = int.Parse(tag.ToCharArray()[1].ToString());
                    colors[i++] = colorMap[index];
                }
            }

            for (int i = 0; i < segments.Length; i++)
            {
                Console.ForegroundColor = colors[i];
                Console.Write(segments[i]);
            }
        }
#pragma warning enable

        /// <summary>
        ///     Specifies constants that define the details of how a message should be printed.
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
