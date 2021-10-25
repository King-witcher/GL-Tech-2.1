using GLTech2;
using GLTech2.Behaviours;
using GLTech2.Drawing;
using GLTech2.Entities;
using GLTech2.Entities.StandardEntites;

namespace Test
{
    partial class Program
    {
        // Esse é um mapa com uma quantidade extrema de planos para testes de gargalo.
        static void ExtremeGrid()
        {
            using PixelBuffer BlockMapbuf = new PixelBuffer(DemoTextures.Plant);
            using PixelBuffer texturebuf = new PixelBuffer(DemoTextures.GrayHexagons);
            using PixelBuffer backgroundbuf = new PixelBuffer(DemoTextures.HellSky);

            Texture background = new Texture(backgroundbuf);
            Scene scene = new Scene(background);

            // BlockMap
            {
                Texture tex = new Texture(BlockMapbuf, 0f, 1f);
                BlockMap.TextureMapper tb = new BlockMap.TextureMapper();
                tb[(0, 0, 0)] = tex;
                BlockMap gm = new BlockMap(BlockMapbuf, tb);
                gm.WorldScale = 0.04f;
                scene.AddElement(gm);
            }

            // Camera
            {
                Camera camera = scene.Camera;
                camera.AddBehaviour(new MouseRotation(2.2f));
                camera.AddBehaviour<DebugPerformanceStats>();
                camera.AddBehaviour<DebugSceneInfo>();

                KinematicBody kinematicBody = new PointCollider();
                SoftMovement softMovement = new SoftMovement(kinematicBody);
                camera.AddBehaviour(kinematicBody);
                camera.AddBehaviour(softMovement);

                scene.AddElement(camera);
            }

            Renderer.CaptureMouse = true;
            Renderer.FullScreen = true;
            Renderer.DoubleBuffer = true;
            Renderer.Start(scene);
        }
    }
}
