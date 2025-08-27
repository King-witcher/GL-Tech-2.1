
using Engine.World;
using SDL2;
using System;
using Windows.Storage.Streams;
using static SDL2.SDL;

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
                    "0 - Exit\n]");
                var option = Console.ReadKey();
                Console.WriteLine();

                if (option.KeyChar == '0') break;

                using Scene? scene = option.KeyChar switch
                {
                    '1' => new FloorStressTest.Map(),
                    '2' => new RotatingPillars.Map(),
                    '3' => new Wolfenstein.Map(),
                    '4' => new SampleBlockMap.Map(),
                    '5' => new SuperBlockMap.Map(),
                    _ => null
                };

                if (scene == null)
                {
                    Debug.Log("Invalid option", "Program", Debug.Options.Error);
                    continue;
                }

                Renderer.Run(scene);
            }
        }
    }
}
