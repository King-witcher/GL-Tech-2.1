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
            // Renderer.FullScreen = true;

            Renderer renderer = new Renderer();

            renderer.CustomHeight = 600;
            renderer.CustomWidth = 800;
            renderer.FieldOfView = 110f;
            renderer.SynchronizeThreads = true;
            renderer.CaptureMouse = true;

            // Mapa pequeno para exemplificar o funcionamento do BlockMap
            FloorStressTest.Map floorStressTest = new();
            renderer.Play(floorStressTest);
            floorStressTest.Dispose();

            // Mapa pequeno para exemplificar o funcionamento do BlockMap
            SampleBlockMap.Map sampleBlockMap = new();
            renderer.Play(sampleBlockMap);
            sampleBlockMap.Dispose();

            // Mapa que demonstra o funcionamento do sistema de parenting e do background
            RotatingPillars.Map pillarsMap = new();
            renderer.CaptureMouse = true;
            renderer.Play(pillarsMap);
            pillarsMap.Dispose();

            // Primeiro mapa do jogo Wolfenstein 3D - clássico da computação gráfica por usar a técnica de RayCasting
            Wolfenstein.Map wolfenstein = new();
            renderer.Play(wolfenstein);
            wolfenstein.Dispose();

            // Teste de estresse com um blockmap composto por muitas paredes com tratamento de colisão
            SuperBlockMap.Map superBlockMap = new();
            renderer.Play(superBlockMap);
            superBlockMap.Dispose();
        }
    }
}
