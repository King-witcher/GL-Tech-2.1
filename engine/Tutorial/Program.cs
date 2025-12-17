
using Engine.Imaging;
using Engine.Scripting;
using Engine.Scripting.Physics;
using Engine.Scripting.Prefab;
using Engine.World;
using Engine.World.Composed;

namespace Engine.Tutorial
{
    class Map : Scene
    {
        Image golden = new(Resources.Golden);
        Image universe = new(Resources.Universe);

        protected override void Delete()
        {
            golden.Dispose();
            universe.Dispose();
        }

        public Map()
        {
            // Plane
            {
                var tex = new Texture(golden);
                var plane = new Wall(
                    start: (-1f, 2f),
                    end: (1f, 3f),
                    texture: tex
                );
                Add(plane);
            }

            // Plane 2
            {
                var tex = new Texture(universe);
                var plane = new Plane(
                    start: (-1f, 4f),
                    end: (1f, 4f),
                    texture: tex
                );
                Add(plane);
            }

            // Camera
            {
                KinematicBody body = new PointCollider();
                Script movement = new Q1Movement(body);

                Camera.AddScript<MouseLook>();
                Camera.AddScripts(body, movement);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Scene scene = new Map();
            Renderer.CaptureMouse = true;
            Renderer.FullScreen = true;
            Renderer.Run(scene);
        }
    }
}
