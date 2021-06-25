using GLTech2;

namespace Test
{
    static partial class Program
    {
        static void Main()
        {
            Debug.ConsoleEnabled = true;

            Debug.Pause("Press any key to start.");
            GridExample();

            Debug.Pause("Press any key to start.");
            AnimatedExample();

            Debug.Pause("Press any key to start.");
            Debug.Log("Releasing resources...");
        }
    }
}
