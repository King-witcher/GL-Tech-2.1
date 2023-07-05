using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Composed;
using Engine.Scripting;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Demos; // Fiz isso na pura preguiça de ficar renomeando o DemoTextures bugado do VS
using Engine.Input;

namespace Engine.Demos.RotatingPillars
{
    public class Map : Scene
    {
        // Firstly, load buffers
        Image background_buffer = new(Resources.HellSky);
        Image carvedWall_buffer = new(Resources.CarvedWall);
        Image bricks_buffer = new(Resources.Bricks);
        Image wood_buffer = new(Resources.Wood);
        Image grayHexagons_buffer = new(Resources.GrayHexagons);

        protected override void Delete()
        {
            background_buffer.Dispose();
            carvedWall_buffer.Dispose();
            bricks_buffer.Dispose();
            wood_buffer.Dispose();
            grayHexagons_buffer.Dispose();
        }

        // Scene
        public Map()
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
                Noclip nm = new Noclip();
                Camera.AddScript(nm);
                SoftMovement movement = new SoftMovement(nm);
                Camera.AddScript(movement);
            }
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

    void OnKeyDown(ScanCode key)
    {
        switch (key)
        {
            case ScanCode.Q:
                if (Scene.Background.source.Buffer == System.IntPtr.Zero)
                    Debug.Log("Scene background enabled.");
                Scene.Background = background;
                break;

            case ScanCode.E:
                if (Scene.Background.source.Buffer != System.IntPtr.Zero)
                    Debug.Log("Scene background disabled.");
                Scene.Background = Texture.NullTexture;
                break;
        }
    }
}
