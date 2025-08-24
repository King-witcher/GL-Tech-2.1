
using Engine.World;
using System;

namespace Engine.Demos
{
    unsafe static internal partial class Program
    {
        static void Main()
        {
            Debug.OpenConsole();

            // Renderer settings
            Renderer.FullScreen = true;
            Renderer.CustomWidth = 1920;
            Renderer.CustomHeight = 1080;
            Renderer.FieldOfView = 110f;
            Renderer.SynchronizeThreads = true;
            Renderer.CaptureMouse = true;

            while (true)
            {
                Console.Write(
                    "Choose a map to test:\n" +
                    "1 - Floor Stress Test\n" +
                    "2 - Rotating Pillars\n" +
                    "3 - Wolfenstein 3D\n" +
                    "4 - Simple BlockMap\n" +
                    "5 - Super BlockMap\n" +
                    "0 - Exit\n> ");

                var option = Console.ReadLine();

                using Scene scene = option switch
                {
                    "1" => new FloorStressTest.Map(),
                    "2" => new RotatingPillars.Map(),
                    "3" => new Wolfenstein.Map(),
                    "4" => new SampleBlockMap.Map(),
                    "5" => new SuperBlockMap.Map(),
                    _ => null
                };
                Renderer.Run(scene);
            }
            RenderDemos();
        }

        static void RenderDemos()
        {
            Debug.OpenConsole();

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
