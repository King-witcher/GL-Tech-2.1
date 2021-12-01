using GLTech2;
using GLTech2.Imaging;
using GLTech2.Entities;
using GLTech2.Entities.StandardEntites;
using GLTech2.Scripting.Debugging;
using GLTech2.Scripting.StandardScripts;
using GLTech2.Scripting.Physics;

namespace Test
{
    partial class Program
    {
        // Um pequeno mapa usando MapGrid
        static void GridExample()
        {
            // Buffers used
            using ImageData bricks_buffer = new ImageData(DemoTextures.Bricks);
            using ImageData wood_buffer = new ImageData(DemoTextures.Wood);
            using ImageData hexagons_buffer = new ImageData(DemoTextures.GrayHexagons);
            using ImageData background_buffer = new ImageData(DemoTextures.HellSky);

            Texture background = new Texture(background_buffer);

            using Scene scene = new Scene(background);

            // BlockMap
            {
                using ImageData grid = new ImageData(DemoTextures.MapGrid);
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
                MouseLook mouseLook = new MouseLook(2.2f);

                camera.AddBehaviour(collider);
                camera.AddBehaviour(movement);
                camera.AddBehaviour(mouseLook);

                scene.Add(camera);
            }

            // Renderer customization
            Engine.FullScreen = false;
            Engine.CustomWidth = 1600;
            Engine.CustomHeight = 900;
            Engine.FieldOfView = 110f;
            Engine.ParallelRendering = true;
            Engine.DoubleBuffer = false;
            Engine.CaptureMouse = true;

            // Run!
            Engine.Run(scene);
        }
    }
}