using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Prefab;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;

namespace Test
{
    partial class Program
    {
        // Esse é um mapa com uma quantidade extrema de planos para testes de gargalo.
        static void ExtremeGrid()
        {
            using Image BlockMapbuf = new Image(DemoTextures.Plant);
            using Image texturebuf = new Image(DemoTextures.GrayHexagons);
            using Image backgroundbuf = new Image(DemoTextures.HellSky);

            Texture background = new Texture(backgroundbuf);
            Scene scene = new Scene(background);

            // BlockMap
            {
                Texture tex = new Texture(BlockMapbuf, 0f, 1f);
                BlockMap.TextureMapper tb = new BlockMap.TextureMapper();
                tb[(0, 0, 0)] = tex;
                BlockMap gm = new BlockMap(BlockMapbuf, tb);
                gm.WorldScale = 0.04f;
                scene.Add(gm);
            }

            // Camera
            {
                Camera camera = scene.Camera;
                camera.AddBehaviour(new MouseLook(2.2f));
                camera.AddBehaviour<DebugPerformanceStats>();
                camera.AddBehaviour<DebugSceneInfo>();

                KinematicBody kinematicBody = new PointCollider();
                SoftMovement softMovement = new SoftMovement(kinematicBody);
                camera.AddBehaviour(kinematicBody);
                camera.AddBehaviour(softMovement);

                scene.Add(camera);
            }

            Engine.Renderer.CaptureMouse = true;
            Engine.Renderer.FullScreen = true;
            Engine.Renderer.DoubleBuffer = true;
            Engine.Renderer.Run(scene);
        }
    }
}
