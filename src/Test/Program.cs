using GLTech2;

namespace Test
{
    static internal partial class Program
    {
        static void Main()
        {
            Debug.ConsoleEnabled = true;

            Debug.Pause("Press any key to start.");
            Debug.Clear();
            E1M1();

            Debug.Pause("Press any key to start.");
            Debug.Clear();
            AnimatedExample();

            Debug.Pause("Press any key to start.");
            Debug.Clear();
            ExtremeGrid();

            Debug.Pause("Press any key to close.");
            Debug.Log("Releasing resources...");
        }
    }
}
