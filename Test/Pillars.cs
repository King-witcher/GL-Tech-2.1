using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Prefab;
using Engine.Scripting;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Test; // Fiz isso na pura preguiça de ficar renomeando o DemoTextures bugado do VS

namespace Test
{
    partial class Program
    {
        static void PillarsMap()
        {
            // Firstly, load buffers
            using Image background_buffer = new(DemoTextures.HellSky);
            using Image carvedWall_buffer = new(DemoTextures.CarvedWall);
            using Image bricks_buffer = new(DemoTextures.Bricks);
            using Image wood_buffer = new(DemoTextures.Wood);
            using Image grayHexagons_buffer = new(DemoTextures.GrayHexagons);

            // Scene
            Texture background = new Texture(
                source: background_buffer,
                hoffset: 0f,
                hrepeat: 1f);

            using Scene scene = new Scene(background);

            // Pivot
            {
                Empty pivot = new(x: 0, y: 0.2868f);
                pivot.AddScript(new Rotate { AngularSpeed = -20f });

                // Square
                {
                    Texture tex = new(
                        source: wood_buffer,
                        hoffset: 0f,
                        hrepeat: 2f);

                    Entity e = new RegularPolygon(
                        position: (-1f, 0f),
                        vertices: 4,
                        radius: .354f,
                        texture: tex);

                    e.AddScript(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                // Cylinder
                {
                    Texture tex = new Texture(
                        source: bricks_buffer,
                        hoffset: 0f,
                        hrepeat: 4f);

                    Entity e = new RegularPolygon(
                        position: (1f, 0f),
                        vertices: 100,
                        radius: .318f,
                        texture: tex);

                    e.AddScript(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                // Triangle
                {
                    Texture tex = new Texture(
                        source: carvedWall_buffer,
                        hoffset: 0f,
                        hrepeat: 1f);

                    Entity e = new RegularPolygon(
                        position: (0f, 1.732f),
                        vertices: 3,
                        radius: -.385f,
                        texture: tex);

                    e.AddScript(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                scene.Add(pivot);
            }

            // Pillars
            {
                Empty pillars = new(Vector.Zero);

                Texture tex = new Texture(
                    source: wood_buffer,
                    hoffset: 0f,
                    hrepeat: 1f);

                for (int i = -20; i < 20; i++)
                {
                    for (int j = -20; j < 20; j++)
                    {
                        Entity pillar = new RegularPolygon(
                            position: (i, j),
                            vertices: 4,
                            radius: 0.2f,
                            texture: tex);

                        Rotate rotate = new();
                        rotate.AngularSpeed = Random.GetFloat(-90f, 90f);

                        pillar.Parent = pillars;
                        pillar.AddScript(rotate);
                    }
                }

                scene.Add(pillars);
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
            Engine.Renderer.SynchronizeThreads = true;
            Engine.Renderer.CaptureMouse = true;

            // Run!
            Engine.Renderer.Run(scene);
        }
    }
}
