using GLTech2;
using GLTech2.Behaviours;
using GLTech2.Elements;

namespace Test
{
    partial class Program
    {
        static void GridExample()
        {
            // Buffers used
            using PixelBuffer bricks_buffer = new PixelBuffer(DemoTextures.Bricks);
            using PixelBuffer wood_buffer = new PixelBuffer(DemoTextures.Wood);
            using PixelBuffer hexagons_buffer = new PixelBuffer(DemoTextures.GrayHexagons);
            using PixelBuffer background_buffer = new PixelBuffer(DemoTextures.HellSky);

            Texture background = new Texture(background_buffer);

            using Scene scene = new Scene(background);

            // GridMap
            {
                using PixelBuffer grid = new PixelBuffer(DemoTextures.MapGrid);
                GridMap.TextureBindings binds = new GridMap.TextureBindings();
                {
                    Texture bricks = new Texture(
                        buffer: bricks_buffer,
                        hrepeat: 2f);
                    Texture wood = new Texture(
                        buffer: wood_buffer,
                        hrepeat: 1f);
                    Texture hexagons = new Texture(
                        buffer: hexagons_buffer,
                        hrepeat: 2f);

                    binds[(255, 255, 255)] = bricks;
                    binds[(0, 192, 0)] = wood;
                    binds[(128, 0, 255)] = hexagons;
                }

                GridMap gridMap = new GridMap(map: grid, textureBindings: binds);
                scene.AddElement(gridMap);
            }

            // Observer
            {
                Observer pov = new Observer((5, 5), 180);

                pov.AddBehaviour<DebugPerformanceStats>();
                // pov.AddBehaviour<DebugComponents>();
                pov.AddBehaviour<DebugSceneInfo>();
                pov.AddBehaviour<NoclipController>();
                pov.AddBehaviour(new MouseLook(2.2f));

                scene.AddElement(pov);
            }

            // Renderer customization
            Renderer.FullScreen = false;
            Renderer.CustomWidth = 1600;
            Renderer.CustomHeight = 900;
            Renderer.FieldOfView = 110f;
            Renderer.ParallelRendering = true;
            Renderer.DoubleBuffering = true;
            Renderer.CaptureMouse = true;

            // Run!
            Renderer.Run(scene);
        }
    }
}