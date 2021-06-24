using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal static partial class ProgramMain
    {
        static void Main()
        {
            Debug.ConsoleEnabled = true;

            Debug.Log("Press any key to start.");
            Console.ReadKey();

            GridExample();

            Debug.Log("Press any key to start.");
            Console.ReadKey();

            AnimatedExample();

            Debug.Log("Press any key to close.");
            Console.ReadKey();

            Debug.Log("Releasing resources...");
        }
    }
}
