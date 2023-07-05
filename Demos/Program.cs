
namespace Engine.Demos
{
    unsafe static internal partial class Program
    {
        static void Main()
        {
            RenderDemos();
        }

        static void RenderDemos()
        {
            Debug.OpenConsole();

            // Renderer customization
            Renderer.FullScreen = true;
            Renderer.CustomWidth = 1920;
            Renderer.CustomHeight = 1080;
            Renderer.FieldOfView = 110f;
            Renderer.SynchronizeThreads = true;
            Renderer.CaptureMouse = true;

            // Mapa pequeno para exemplificar o funcionamento do BlockMap
            FloorStressTest.Map floorStressTest = new();
            Renderer.Run(floorStressTest);
            floorStressTest.Dispose();

            // Mapa pequeno para exemplificar o funcionamento do BlockMap
            SampleBlockMap.Map sampleBlockMap = new();
            Renderer.Run(sampleBlockMap);
            sampleBlockMap.Dispose();

            // Mapa que demonstra o funcionamento do sistema de parenting e do background
            RotatingPillars.Map pillarsMap = new();
            Renderer.CaptureMouse = true;
            Renderer.Run(pillarsMap);
            pillarsMap.Dispose();

            // Primeiro mapa do jogo Wolfenstein 3D - clássico da computação gráfica por usar a técnica de RayCasting
            Wolfenstein.Map wolfenstein = new();
            Renderer.Run(wolfenstein);
            wolfenstein.Dispose();

            // Teste de estresse com um blockmap composto por muitas paredes com tratamento de colisão
            SuperBlockMap.Map superBlockMap = new();
            Renderer.Run(superBlockMap);
            superBlockMap.Dispose();
        }
    }
}
