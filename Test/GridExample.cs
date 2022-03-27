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
            using Image floors_buffer = new Image(DemoTextures.Floors);
            using Image golden_buffer = new Image(DemoTextures.Golden);
            using Image v_buffer = new Image(DemoTextures.V2);
            using Image background_buffer = new Image(DemoTextures.Universe);

            Texture background = new Texture(background_buffer);

            using Scene scene = new Scene(background);

            // BlockMap
            {
                using Image grid = new Image(DemoTextures.MapGrid);
                BlockMap.TextureMapper mapper = new BlockMap.TextureMapper();
                {
                    Texture floors = new Texture(
                        source: floors_buffer,
                        hrepeat: 0.25f,
                        vrepeat: 0.25f);
                    Texture golden = new Texture(
                        source: golden_buffer,
                        hrepeat: 1f);
                    Texture v = new Texture(
                        source: v_buffer,
                        hrepeat: 1f);

                    mapper[(255, 255, 255)] = floors;
                    mapper[(0, 192, 0)] = golden;
                    mapper[(128, 0, 255)] = v;
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