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
        // Um pequeno mapa usando MapGrid
        static void GridExample()
        {
            // Buffers used
            using Image bricks_buffer = new Image(DemoTextures.Bricks);
            using Image wood_buffer = new Image(DemoTextures.Wood);
            using Image hexagons_buffer = new Image(DemoTextures.GrayHexagons);
            using Image background_buffer = new Image(DemoTextures.Universe);

            Texture background = new Texture(background_buffer);

            using Scene scene = new Scene(background);

            // BlockMap
            {
                using Image grid = new Image(DemoTextures.MapGrid);
                BlockMap.TextureMapper mapper = new BlockMap.TextureMapper();
                {
                    Texture bricks = new Texture(
                        source: bricks_buffer,
                        hrepeat: 2f);
                    Texture wood = new Texture(
                        source: wood_buffer,
                        hrepeat: 1f);
                    Texture hexagons = new Texture(
                        source: hexagons_buffer,
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
                camera.AddScript<DebugPerformanceStats>();
                camera.AddScript<DebugSceneInfo>();

                PointCollider collider = new PointCollider();
                SoftMovement movement = new SoftMovement(collider);
                MouseLook mouseLook = new MouseLook(2.2f);

                camera.AddScript(collider);
                camera.AddScript(movement);
                camera.AddScript(mouseLook);

                scene.Add(camera);
            }

            // Renderer customization
            Renderer.FullScreen = false;
            Renderer.CustomHeight = 600;
            Renderer.CustomWidth = 800;
            Renderer.FieldOfView = 110f;
            Renderer.ParallelRendering = true;
            Renderer.SynchronizeThreads = false;
            Renderer.CaptureMouse = true;

            // Run!
            Engine.Renderer.Run(scene);
        }
    }
}