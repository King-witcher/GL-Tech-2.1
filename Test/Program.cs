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

            // Teste de estresse com um blockmap composto por muitas paredes com tratamento de colisão
            SuperBlockMap superBlockMap = new();
            Renderer.Run(superBlockMap);
            superBlockMap.Dispose();
        }
    }
}
