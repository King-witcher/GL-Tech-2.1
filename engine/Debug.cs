using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine
{
    public class Debug
    {
        private static bool enabled = false;
        public static bool Enabled => enabled;

        public enum Options { Normal, Success, Warning, Error }

        public static void Log()
        {
            if (enabled) Console.WriteLine();
        }

        public static void Log(object message, string? context = null, Options debugOption = Options.Normal)
        {
            if (!enabled) return;

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

        public static string Read()
        {
            if (!enabled) return string.Empty;

            string retn = Console.ReadLine();
            Console.WriteLine();
            return retn;
        }

        public static void Pause()
        {
            if (!enabled) return;

            Console.ReadKey();
            Console.Write("\b \b\n");
        }

        public static void Clear()
        {
            if (enabled) Console.Clear();
        }

        #region kernel32.dll
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle")]
        static extern IntPtr kernel32_GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole")]
        static extern bool kernel32_AllocConsole();

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool kernel32_FreeConsole();
        #endregion

        internal static void OpenConsole()
        {
            if (enabled) return;

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

        internal static void CloseConsole()
        {
            if (!enabled) return;
            enabled = false;

            kernel32_FreeConsole();
        }
    }
}