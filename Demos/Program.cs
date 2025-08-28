
using Engine.Structs;
using Engine.World;
using System;

namespace Engine.Demos
{
    unsafe static internal partial class Program
    {
        static void Main()
        {
            Logger logger = new Logger("Program");
            Debug.OpenConsole();

            // Renderer settings
            Renderer.FullScreen = true;
            Renderer.CustomWidth = 1920;
            Renderer.CustomHeight = 1080;
            Renderer.FieldOfView = 110f;
            Renderer.CaptureMouse = true;

            while (true)
            {
#if true
                logger.Log($"Cameras count: {CameraStruct.count}");
                logger.Log($"Planes count: {PlaneStruct.count}");
                logger.Log($"Colliders count: {ColliderStruct.count}");
                logger.Log($"Horizontals count: {HorizontalStruct.count}");
                logger.Log($"HorizontalNodes count: {HorizontalList.Node.count}");
#endif

                Console.Write(
                    "Choose a map to test:\n" +
                    "1 - Empty Scene\n" +
                    "2 - Floor Stress Test\n" +
                    "3 - Rotating Pillars\n" +
                    "4 - Wolfenstein 3D\n" +
                    "5 - Simple BlockMap\n" +
                    "6 - Super BlockMap\n" +
                    "0 - Exit\n]");
                var option = Console.ReadKey();
                Console.WriteLine();

                if (option.KeyChar == '0') break;

                using Scene? scene = option.KeyChar switch
                {
                    '1' => new Scene(),
                    '2' => new FloorStressTest.Map(),
                    '3' => new RotatingPillars.Map(),
                    '4' => new Wolfenstein.Map(),
                    '5' => new SampleBlockMap.Map(),
                    '6' => new SuperBlockMap.Map(),
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
