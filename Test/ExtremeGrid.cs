using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Prefab;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Test;

namespace Test
{
    partial class Program
    {
        // Esse é um mapa com uma quantidade extrema de planos para testes de gargalo.
        static void ExtremeGrid()
        {
            using Image BlockMapbuf = new(DemoTextures.Plant);
            using Image Test = new(DemoTextures.Test);
            using Image backgroundbuf = new(DemoTextures.CrazyBackground);

            Texture background = new Texture(backgroundbuf, 0, 3);
            Scene scene = new Scene(background);

            // BlockMap
            {
                Texture tex = new(Test, 0f, 1f, 0f, 25f);
                BlockMap.TextureMapper tb = new();
                tb[(0, 0, 0)] = tex;
                BlockMap gm = new(BlockMapbuf, tb);
                gm.WorldScale = 0.04f;
                scene.Add(gm);
            }

            // Camera
            {
                Camera camera = scene.Camera;
                camera.AddScript(new MouseLook(2.2f));
                camera.AddScript<DebugPerformanceStats>();
                camera.AddScript<DebugSceneInfo>();

                KinematicBody kinematicBody = new PointCollider();
                SoftMovement softMovement = new SoftMovement(kinematicBody);
                camera.AddScript(kinematicBody);
                camera.AddScript(softMovement);

                scene.Add(camera);
            }

            Engine.Renderer.CaptureMouse = true;
            Engine.Renderer.FullScreen = true;
            Engine.Renderer.SynchronizeThreads = true;
            Engine.Renderer.Run(scene);
        }
    }
}
