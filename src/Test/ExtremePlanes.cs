using GLTech2;
using GLTech2.Behaviours;
using GLTech2.Elements;

namespace Test
{
    partial class Program
    {
        static void ExtremePlanes()
        {
            using PixelBuffer BlockMapbuf = new PixelBuffer(DemoTextures.Plant);
            using PixelBuffer texturebuf = new PixelBuffer(DemoTextures.GrayHexagons);
            using PixelBuffer backgroundbuf = new PixelBuffer(DemoTextures.HellSky);

            Texture background = new Texture(backgroundbuf);
            Scene scene = new Scene(background);

            // BlockMap
            {
                Texture tex = new Texture(texturebuf, 0f, 0.03f);
                BlockMap.TextureMapper tb = new BlockMap.TextureMapper();
                tb[(0, 0, 0)] = tex;
                BlockMap gm = new BlockMap(BlockMapbuf, tb);
                gm.WorldScale = 0.04f;
                scene.AddElement(gm);
            }

            // Observer
            Camera observer = new Camera((0, 0));
            {
                observer.AddBehaviour<MouseLook>();
                observer.AddBehaviour<DebugPerformanceStats>();
                observer.AddBehaviour<DebugSceneInfo>();

                KinematicBody kinematicBody = new PointCollider();
                SoftMovement softMovement = new SoftMovement(kinematicBody);
                observer.AddBehaviour(kinematicBody);
                observer.AddBehaviour(softMovement);

                scene.AddElement(observer);
            }

            Renderer.CaptureMouse = true;
            Renderer.FullScreen = true;
            Renderer.DoubleBuffer = true;
            Renderer.Start(observer);
        }
    }
}
