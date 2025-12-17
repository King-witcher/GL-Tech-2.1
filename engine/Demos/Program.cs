//using GLTech.World;
//using System;

//namespace GLTech.Demos
//{
//    unsafe static internal partial class Program
//    {
//        static void Main()
//        {
//            Logger logger = new Logger("Program");
//            Debug.OpenConsole();

//            // Renderer settings
//            Engine.FullScreen = true;
//            Engine.CustomWidth = 1920;
//            Engine.CustomHeight = 1080;
//            Engine.FieldOfView = 110f;
//            Engine.CaptureMouse = true;
//            Engine.ParallelRendering = true;

//            while (true)
//            {
//                Console.Write(
//                    "Choose a map to test:\n" +
//                    "1 - Empty Scene\n" +
//                    "2 - Floor Stress Test\n" +
//                    "3 - Rotating Pillars\n" +
//                    "4 - Wolfenstein 3D\n" +
//                    "5 - Simple BlockMap\n" +
//                    "6 - Super BlockMap\n" +
//                    "7 - Simple Benchmark\n" +
//                    "0 - Exit\n]");
//                var option = Console.ReadKey();
//                Console.WriteLine();

//                if (option.KeyChar == '0') break;

//                using Scene? scene = option.KeyChar switch
//                {
//                    '1' => new Scene(),
//                    '2' => new FloorStressTest.Map(),
//                    '3' => new RotatingPillars.Map(),
//                    '5' => new SampleBlockMap.Map(),
//                    '6' => new SuperBlockMap.Map(),
//                    '7' => new SimpleBenchmark.Map(),
//                    _ => null
//                };

//                if (scene == null)
//                {
//                    Debug.Log("Invalid option", "Program", Debug.Options.Error);
//                    continue;
//                }

//                Engine.Run(scene);
//            }
//        }
//    }
//}
