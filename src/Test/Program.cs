using GLTech2;

namespace Test
{
    static internal partial class Program
    {
        static void Main()
        {
            Debug.Pause();
            Debug.Clear();
            E1M1();

            Debug.Pause();
            Debug.Clear();
            AnimatedExample();

            Debug.Pause();
            Debug.Clear();
            ExtremeGrid();

            Debug.Pause();
            Debug.Log("Releasing resources...");
        }
    }
}
