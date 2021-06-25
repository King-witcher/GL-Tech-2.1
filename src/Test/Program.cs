using System;   // Vou tirar isso jaja
using GLTech2;

namespace Test
{
    internal static partial class Program
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
