using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Prefab;
using Engine.Scripting;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Test; // Fiz isso na pura preguiça de ficar renomeando o DemoTextures bugado do VS
using Engine.Input;

namespace Test
{
    partial class Program
    {
        public class PillarsMap : Scene
        {
            // Firstly, load buffers
            Image background_buffer = new(DemoTextures.HellSky);
            Image carvedWall_buffer = new(DemoTextures.CarvedWall);
            Image bricks_buffer = new(DemoTextures.Bricks);
            Image wood_buffer = new(DemoTextures.Wood);
            Image grayHexagons_buffer = new(DemoTextures.GrayHexagons);

            protected override void Delete()
            {
                background_buffer.Dispose();
                carvedWall_buffer.Dispose();
                bricks_buffer.Dispose();
                wood_buffer.Dispose();
                grayHexagons_buffer.Dispose();
            }

            // Scene
            public PillarsMap()
            {
                Background = new Texture(background_buffer);

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

                    Add(pivot);
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

                    Add(pillars);
                }

                // Camera
                {
                    Camera.AddScript<SwitchBackgroundScript>();
                    Camera.AddScript<DebugPerformanceStats>();
                    Camera.AddScript<MouseLook>();
                    NoclipMode nm = new NoclipMode();
                    Camera.AddScript(nm);
                    SoftMovement movement = new SoftMovement(nm);
                    Camera.AddScript(movement);
                }

                // Setup Renderer
                Engine.Renderer.FullScreen = true;
                Engine.Renderer.FieldOfView = 110f;
                Engine.Renderer.ParallelRendering = true;
                Engine.Renderer.SynchronizeThreads = true;
                Engine.Renderer.CaptureMouse = true;
            }
        }
    }

    class SwitchBackgroundScript : Script
    {
        Texture background;

        void Start()
        {
            background = Scene.Background;
        }

        // PAREI AQUI, TAVA DANDO ALGUM ERRO MUITO ESTRANHO
        void OnKeyDown(InputKey key)
        {
            if (key == InputKey.E)
            {
                if (Scene.Background.source.Buffer == System.IntPtr.Zero)
                    Debug.Log("Scene background enabled.");
                Scene.Background = background;
            }
            else if (key == InputKey.Q)
            {
                if (Scene.Background.source.Buffer != System.IntPtr.Zero)
                    Debug.Log("Scene background disabled.");
                Scene.Background = Texture.NullTexture;
            }
        }

        /*void OnFrame()
        {
            if (Keyboard.IsKeyDown(InputKey.Q))
            {
                if (Scene.Background == Texture.NullTexture)
                    Debug.Log("Scene background enabled.");
                Scene.Background = background;
            }
            if (Keyboard.IsKeyDown(InputKey.E))
            {
                if (Scene.Background != Texture.NullTexture)
                    Debug.Log("Scene background disabled.");
                Scene.Background = Texture.NullTexture;
            }
        }*/
    }
}
