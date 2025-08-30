using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Composed;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Scripting.Debugging;

namespace Engine.Demos.FloorStressTest
{
    // Um pequeno mapa usando BlockMap
    public class Map : Scene
    {
        Image floors_buffer = new(Resources.golden);

        protected override void Delete()
        {
            floors_buffer.Dispose();
        }

        public Map()
        {
            Background = Texture.FromColor(Color.Black, out _);

            const int size = 100;

            // Floors
            {
                var texture = new Texture(floors_buffer);
                for (float i = 0; i < size; i++)
                {
                    for (float j = 0; j < size; j++)
                    {
                        Floor f = new Floor((i, j), (i + 1, j + 1), texture);
                        Ceiling c = new Ceiling((i, j), (i + 1, j + 1), texture);
                        Add(f, c);
                    }
                }
            }

            // Wall
            {
                var texture = new Texture(floors_buffer, 0f, size, 0f, 1f);
                Add(
                    new Wall((0f, 0f), (0f, size), texture),
                    new Wall((0f, size), (size, size), texture),
                    new Wall((size, size), (size, 0f), texture),
                    new Wall((size, 0f), (0f, 0f), texture)
                );
            }

            // Camera
            {
                Camera.AddScript<DebugPerformance>();
                Camera.WorldPosition = (0.5f, 0.5f);
                Camera.WorldDirection = (1f, 1f);

                PointCollider collider = new PointCollider();
                Q1Movement movement = new Q1Movement(collider);
                MouseLook mouseLook = new MouseLook(2.2f);

                Camera.AddScript(collider);
                Camera.AddScript(movement);
                Camera.AddScript(mouseLook);
            }
        }
    }
}