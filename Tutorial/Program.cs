using Engine;
using Engine.Imaging;
using Engine.Scripting;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.World;
using Engine.World.Composed;

namespace Engine.Tutorial
{
    class Mundo : Scene
    {
        Image buffer = new(Resources.Golden);
        Image background_buffer = new(Resources.Universe);

        public Mundo()
        {
            // Background
            {
                Texture tex = new(background_buffer);
                Background = tex;
            }

            // Plano
            {
                Texture tex = new(buffer);

                Plane plane = new(
                    start: (-1f, 3f),
                    end: (1f, 2f),
                    texture: tex);

                Add(plane);
            }

            // Polígono
            {
                Texture tex = new(buffer, hrepeat: 5f);
                RegularPolygon polygon = new(Vector.Forward, 5, 1f, tex);
                polygon.AddScript<Rotate>();

                Add(polygon);
            }

            // Camera
            {
                Camera.WorldPosition = (0f, -1f);

                PointCollider collider = new();
                SoftMovement movement = new(collider);

                Camera.AddScripts(collider, movement);
                Camera.AddScript<MouseLook>();
            }
        }

        protected override void Delete()
        {
            buffer.Dispose();
            background_buffer.Dispose();
        }
    }

    class Program
    {
        static void Main()
        {
            using Mundo mundo = new();
        }
    }
}
