using Engine;

namespace Test
{
    static internal partial class Program
    {
        static void Main()
        {
            Debug.OpenConsole();

            // Mapa que demonstra o funcionamento do sistema de parenting e do background
            // PillarsMap pillarsMap = new();
            // Renderer.Run(pillarsMap);
            // pillarsMap.Dispose();

            // Mapa pequeno para exemplificar o funcionamento do BlockMap
            SampleBlockMap sampleBlockMap = new();
            Renderer.Run(sampleBlockMap);
            sampleBlockMap.Dispose();

            // Primeiro mapa do jogo Wolfenstein 3D - clássico da computação gráfica por usar a técnica de RayCasting
            // Wolfenstein wolfenstein = new();
            // Renderer.Run(wolfenstein);
            // wolfenstein.Dispose();

            // Teste de estresse com um blockmap composto por muitas paredes com tratamento de colisão
            // SuperBlockMap superBlockMap = new();
            // Renderer.Run(superBlockMap);
            // superBlockMap.Dispose();
        }
    }
}
