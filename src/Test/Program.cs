using GLTech2;

namespace Test
{
    static internal partial class Program
    {
        static void Main()
        {
            Debug.OpenConsole();

            GLTech2.Imaging.Pixel a = 0x_ff_80_40_20;
            System.Console.WriteLine(a.ToString());
            System.Console.ReadKey();

            E1M1();

            Debug.Log("Releasing resources...");
        }
    }
}
