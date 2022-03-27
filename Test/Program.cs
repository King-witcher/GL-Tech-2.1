using Engine;

namespace Test
{
    static internal partial class Program
    {
        static void Main()
        {
            Debug.OpenConsole();

            PillarsMap();
            GridExample();
            E1M1();
            ExtremeGrid eg = new();
            Renderer.Run(eg);

            Debug.Log("Releasing resources...");
        }
    }
}
