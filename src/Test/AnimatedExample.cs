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
        static void AnimatedExample()
        {
            // Firstly, load buffers
            using ImageData background_buffer = (ImageData)DemoTextures.HellSky;
            using ImageData carvedWall_buffer = (ImageData)DemoTextures.CarvedWall;
            using ImageData bricks_buffer = (ImageData)DemoTextures.Bricks;
            using ImageData wood_buffer = (ImageData)DemoTextures.Wood;
            using ImageData grayHexagons_buffer = (ImageData)DemoTextures.GrayHexagons;

            // Scene
            Texture background = new Texture(
                buffer: background_buffer,
                hoffset: 0f,
                hrepeat: 1f);

            using Scene scene = new Scene(background);

            // Pivot
            {
                Empty pivot = new Empty(x: 0, y: 0.2868f);
                pivot.AddBehaviour(new Rotate { AngularSpeed = -20f });

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

                    e.AddBehaviour(new Rotate { AngularSpeed = 180f });
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

                    e.AddBehaviour(new Rotate { AngularSpeed = 180f });
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

                    e.AddBehaviour(new Rotate { AngularSpeed = 180f });
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
                scene.Camera.AddBehaviour<DebugPerformanceStats>();

                scene.Camera.AddBehaviour<MouseLook>();

                NoclipMode nm = new NoclipMode();
                scene.Camera.AddBehaviour(nm);
                SoftMovement movement = new SoftMovement(nm);
                scene.Camera.AddBehaviour(movement);
            }

            // Setup Renderer
            Engine.FullScreen = true;
            Engine.FieldOfView = 110f;
            Engine.ParallelRendering = true;
            Engine.DoubleBuffer = false;
            Engine.CaptureMouse = true;

            // Run!
            Engine.Run(scene);
        }
    }
}
