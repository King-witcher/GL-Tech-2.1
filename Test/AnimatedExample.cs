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
        static void AnimatedExample()
        {
            // Firstly, load buffers
            using Image background_buffer = (Image)DemoTextures.HellSky;
            using Image carvedWall_buffer = (Image)DemoTextures.CarvedWall;
            using Image bricks_buffer = (Image)DemoTextures.Bricks;
            using Image wood_buffer = (Image)DemoTextures.Wood;
            using Image grayHexagons_buffer = (Image)DemoTextures.GrayHexagons;

            // Scene
            Texture background = new Texture(
                buffer: background_buffer,
                hoffset: 0f,
                hrepeat: 1f);

            using Scene scene = new Scene(background);

            // Pivot
            {
                Empty pivot = new Empty(x: 0, y: 0.2868f);
                pivot.AddScript(new Rotate { AngularSpeed = -20f });

                // Square
                {
                    Texture tex = new Texture(
                        buffer: wood_buffer,
                        hoffset: 0f,
                        hrepeat: 2f);

                    Entity e = new RegularPolygon(
                        position: (-0.5f, 0f),
                        vertices: 4,
                        radius: .354f,
                        texture: tex);

                    e.AddScript(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                // Cylinder
                {
                    Texture tex = new Texture(
                        buffer: bricks_buffer,
                        hoffset: 0f,
                        hrepeat: 4f);

                    Entity e = new RegularPolygon(
                        position: (0.5f, 0f),
                        vertices: 100,
                        radius: .318f,
                        texture: tex);

                    e.AddScript(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                // Triangle
                {
                    Texture tex = new Texture(
                        buffer: carvedWall_buffer,
                        hoffset: 0f,
                        hrepeat: 1f);

                    Entity e = new RegularPolygon(
                        position: (0f, 0.866f),
                        vertices: 3,
                        radius: -.385f,
                        texture: tex);

                    e.AddScript(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                scene.Add(pivot);
            }

            // Big triangle
            {
                Texture tex = new Texture(
                    buffer: grayHexagons_buffer,
                    hoffset: 0f,
                    hrepeat: 32f);

                Entity e = new RegularPolygon(
                    position: Vector.Zero,
                    vertices: 4,
                    radius: -2f,
                    texture: tex);

                scene.Add(e);
            }

            // Camera
            {
                scene.Camera.AddScript<DebugPerformanceStats>();

                scene.Camera.AddScript<MouseLook>();

                NoclipMode nm = new NoclipMode();
                scene.Camera.AddScript(nm);
                SoftMovement movement = new SoftMovement(nm);
                scene.Camera.AddScript(movement);
            }

            // Setup Renderer
            Engine.Renderer.FullScreen = true;
            Engine.Renderer.FieldOfView = 110f;
            Engine.Renderer.ParallelRendering = true;
            Engine.Renderer.DoubleBuffer = false;
            Engine.Renderer.CaptureMouse = true;

            // Run!
            Engine.Renderer.Run(scene);
        }
    }
}
