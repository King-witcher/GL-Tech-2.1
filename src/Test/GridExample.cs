using GLTech2;
using GLTech2.Behaviours;
using GLTech2.Drawing;
using GLTech2.Entities;
using GLTech2.Entities.StandardEntites;

namespace Test
{
    partial class Program
    {
        // Um pequeno mapa usando MapGrid
        static void GridExample()
        {
            // Buffers used
            using PixelBuffer bricks_buffer = new PixelBuffer(DemoTextures.Bricks);
            using PixelBuffer wood_buffer = new PixelBuffer(DemoTextures.Wood);
            using PixelBuffer hexagons_buffer = new PixelBuffer(DemoTextures.GrayHexagons);
            using PixelBuffer background_buffer = new PixelBuffer(DemoTextures.HellSky);

            Texture background = new Texture(background_buffer);

            using Scene scene = new Scene(background);

            // BlockMap
            {
                using PixelBuffer grid = new PixelBuffer(DemoTextures.MapGrid);
                BlockMap.TextureMapper mapper = new BlockMap.TextureMapper();
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

                    mapper[(255, 255, 255)] = bricks;
                    mapper[(0, 192, 0)] = wood;
                    mapper[(128, 0, 255)] = hexagons;
                }

                BlockMap map = new BlockMap(map: grid, textureBindings: mapper);
                scene.Add(map);
            }

            // Camera
            {
                Camera camera = scene.Camera;
                camera.AddBehaviour<DebugPerformanceStats>();
                camera.AddBehaviour<DebugSceneInfo>();

                PointCollider collider = new PointCollider();
                SoftMovement movement = new SoftMovement(collider);
                MouseRotation mouseLook = new MouseRotation(2.2f);

                camera.AddBehaviour(collider);
                camera.AddBehaviour(movement);
                camera.AddBehaviour(mouseLook);

                scene.Add(camera);
            }

            // Renderer customization
            Renderer.FullScreen = false;
            Renderer.CustomWidth = 1600;
            Renderer.CustomHeight = 900;
            Renderer.FieldOfView = 110f;
            Renderer.ParallelRendering = true;
            Renderer.DoubleBuffer = false;
            Renderer.CaptureMouse = true;

            // Run!
            Renderer.Start(scene);
        }
    }
}