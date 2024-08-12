using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Engine
{
    public static class Debug
    {

        private static bool enabled = false;
        public static bool Enabled => enabled;

        public enum Options { Normal, Success, Warning, Error }

        public static void Log()
        {
            if (Enabled)
                Console.WriteLine();
        }

        public static void Log(object message, Options debugOption = Options.Normal)
        {
            if (Enabled)
            {
                ConsoleColor prev = Console.ForegroundColor;

                Console.ForegroundColor = debugOption switch
                {
                    Options.Normal => ConsoleColor.Gray,
                    Options.Success => ConsoleColor.Green,
                    Options.Warning => ConsoleColor.DarkYellow,
                    Options.Error => ConsoleColor.DarkRed,
                    _ => ConsoleColor.White,
                };

                string pre = debugOption switch
                {
                    Options.Normal => "",
                    Options.Success => "[Success]: ",
                    Options.Warning => "[WARNING]: ",
                    Options.Error => "[ERROR]: ",
                    _ => "",
                };

                Console.WriteLine(pre + message.ToString());

                Console.ForegroundColor = prev;
            }
        }

        public static string Read()
        {
            if (enabled)
            {
                DebuggerMessage("Waiting for input...");
                string retn = Console.ReadLine();
                Console.WriteLine();
                return retn;
            }
            else
                return string.Empty;
        }

        public static void Pause()
        {
            if (enabled)
            {
                DebuggerMessage("Waiting for a key...");
                Console.ReadKey();
                Console.Write("\b \b\n");
            }
        }

        public static void Clear()
        {
            if (enabled)
                Console.Clear();
        }

        internal static void OpenConsole()
        {
            if (!enabled)
            {
                kernel32_AllocConsole();

                IntPtr stdinh = kernel32_GetStdHandle(-10);
                IntPtr stdouth = kernel32_GetStdHandle(-11);
                IntPtr stderrh = kernel32_GetStdHandle(-12);

                var safein = new SafeFileHandle(stdinh, true);
                var safeout = new SafeFileHandle(stdouth, true);
                var safeerr = new SafeFileHandle(stderrh, true);

                var fsin = new FileStream(safein, FileAccess.Read);
                var fsout = new FileStream(safeout, FileAccess.Write);
                var fserr = new FileStream(safeerr, FileAccess.Write);

                var srin = new StreamReader(fsin, Console.InputEncoding);
                var srout = new StreamWriter(fsout, Console.OutputEncoding);

                var srerr = new StreamWriter(fserr, Console.OutputEncoding);

                srout.AutoFlush = srerr.AutoFlush = true;

                Console.SetIn(srin);
                Console.SetOut(srout);
                Console.SetError(srerr);

                Console.Title = "GL Tech 2.1 - Debugger";

                enabled = true;
            }
            #region kernel32.dll
            [DllImport("kernel32.dll", EntryPoint = "GetStdHandle")]
            static extern IntPtr kernel32_GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll", EntryPoint = "AllocConsole")]
            static extern bool kernel32_AllocConsole();
            #endregion
        }

        internal static void CloseConsole()
        {
            if (enabled)
            {
                enabled = false;

                kernel32_FreeConsole();

                // Those lines were responsible for making the entire .NET crash in the past, so I'll keep those standard devices as they are.
                // Console.SetIn(TextReader.Null);
                // Console.SetOut(TextWriter.Null);
                // Console.SetError(TextWriter.Null);
            }
            #region kernel32.dll
            [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool kernel32_FreeConsole();
            #endregion
        }

        private static void DebuggerMessage(string message)
        {
            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        internal static void InternalLog(object message, Options debugOption = Options.Normal)
        {
            DebuggerMessage("GL Tech 2.1 says:");
            Log(message.ToString() + "\n", debugOption);
        }
    }
}

public class Joke
{

}


public class Meme
{
    public Joke Joke
    {
        set; private get;
    }
}