using Engine;
using Engine.World;
using Engine.World.Composed;
using Engine.Imaging;
using Engine.Scripting;
using Engine.Scripting.Prefab;
using System.Runtime.InteropServices;
using System.Diagnostics;

using System;

namespace Engine.Demos
{
    unsafe struct StructTest
    {
        float a;
        float b;
        float c;
        float d;
        float e;
        float f;
        float g;
        float h;
        float i;
        StructTest* next;

        public static StructTest* Create(StructTest* next)
        {
            StructTest* result = (StructTest*) Marshal.AllocHGlobal(sizeof(StructTest));
            *result = default;
            result->next = next;
            return result;
        }

    }

    class ClassTest
    {
        float a;
        float b;
        float c;
        float d;
        float e;
        float f;
        float g;
        float h;
        float i;

        public ClassTest(ClassTest next)
        {
            this.next = next;
        }

        ClassTest next;
    }

    unsafe static internal partial class Program
    {
        static void Main()
        {
            Renderer.ParallelRendering = false;
            Renderer.SynchronizeThreads = true;
            Debug.OpenConsole();

            RenderDemos();
        }

        static void TestFloor()
        {
            Renderer.CustomHeight = 100;
            Renderer.CustomWidth = 100;
            Renderer.FieldOfView = 90;

            Scene sc = new Scene(Texture.FromColor(Color.White, out _));
            sc.Camera.WorldPosition = (0.5f, 0.5f);
            sc.Camera.WorldDirection = Vector.Forward;

            {
                Plane a = new((0, 1), (1, 1), Texture.FromColor(Color.Blue, out _));
                Script move = new Move(-0.005f * Vector.Forward);
                sc.Camera.AddScript(move);
                sc.Add(a);
            }

            sc.AddOnFrame(() => { System.Console.WriteLine(sc.Camera.WorldPosition); });

            Renderer.Run(sc);
        }

        static void RenderDemos()
        {
            // Renderer customization
            Renderer.FullScreen = true;
            //Renderer.CustomHeight = 600;
            //Renderer.CustomWidth = 800;
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
            Engine.Renderer.CaptureMouse = true;
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
