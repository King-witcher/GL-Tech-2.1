using GLTech2;
using GLTech2.Behaviours;
using GLTech2.Elements;

namespace Test
{
    partial class Program
    {
        static void ExtremePlanes()
        {
            using PixelBuffer gridmapbuf = new PixelBuffer(DemoTextures.Plant);
            using PixelBuffer texturebuf = new PixelBuffer(DemoTextures.GrayHexagons);
            using PixelBuffer backgroundbuf = new PixelBuffer(DemoTextures.HellSky);

            Texture background = new Texture(backgroundbuf);
            Scene scene = new Scene(background);

            // Gridmap
            {
                Texture tex = new Texture(texturebuf, 0f, 0.03f);
                GridMap.TextureBindings tb = new GridMap.TextureBindings();
                tb[(0, 0, 0)] = tex;
                GridMap gm = new GridMap(gridmapbuf, tb);
                gm.Scale = 0.04f;
                scene.AddElement(gm);
            }

            // Observer
            Camera observer = new Camera((0, 0));
            {
                observer.AddBehaviour<MouseLook>();
                observer.AddBehaviour<FlatMovement>();
                observer.AddBehaviour<DebugPerformanceStats>();
                scene.AddElement(observer);
            }

            Renderer.CaptureMouse = true;
            Renderer.FullScreen = true;
            Renderer.Start(observer);
        }
    }
}
