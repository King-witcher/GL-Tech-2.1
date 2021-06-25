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

            Debug.Clear();
            Debug.Pause("Press any key to start.");
            AnimatedExample();

            Debug.Pause("Press any key to close.");
            Debug.Log("Releasing resources...");
        }
    }
}
