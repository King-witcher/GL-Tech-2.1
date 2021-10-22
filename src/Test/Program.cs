using GLTech2;

namespace Test
{
    static internal partial class Program
    {
        static void Main()
        {
            Debug.Read();

            Debug.Pause();
            E1M1();
            Debug.InternalLog("Log interno testando coisaaaaasdf ahskhw lorem ipsum dolorium set at amet amium sit amagarium loenzum tudus mundus tudusn di la unum", Debug.Options.Error);
            Debug.Read();

            Debug.Pause();
            AnimatedExample();

            Debug.Pause();
            ExtremeGrid();

            Debug.Pause();
            Debug.Log("Releasing resources...");
        }
    }
}
