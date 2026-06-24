using GLTech;
using GLTech.Scripting;
using GLTech.Scripting.Debugging;
using GLTech.Scripting.Physics;
using GLTech.Scripting.Prefab;
using GLTech.World;
using GLTech.World.Composed;

namespace GLTech.Demos.RotatingPillars
{
    public class Map : Scene
    {
        // Firstly, load buffers
        Image background_buffer = Utils.GetImageFromBitmap(Resources.HellSky);
        Image carvedWall_buffer = Utils.GetImageFromBitmap(Resources.CarvedWall);
        Image bricks_buffer = Utils.GetImageFromBitmap(Resources.Bricks);
        Image wood_buffer = Utils.GetImageFromBitmap(Resources.Wood);
        Image grayHexagons_buffer = Utils.GetImageFromBitmap(Resources.GrayHexagons);

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
                        rotate.AngularSpeed = Scripting.Random.GetFloat(-90f, 90f);

                        pillar.Parent = pillars;
                        pillar.AddScript(rotate);
                    }
                }

                Add(pillars);
            }

            // Camera
            {
                Camera.AddScript<SwitchBackgroundScript>();
                Camera.AddScript<DebugPerformance>();
                Camera.AddScript<MouseLook>();
                Noclip nm = new();
                Camera.AddScript(nm);
                Q3Movement movement = new(nm);
                Camera.AddScript(movement);
            }
        }
    }
}

class SwitchBackgroundScript : Script
{
    Texture background;
    static Logger logger = new("RotatingPillars");

    void Start()
    {
        logger.Log("Press Q to enable background, E to disable it.");
        background = Scene.Background;
    }
}
