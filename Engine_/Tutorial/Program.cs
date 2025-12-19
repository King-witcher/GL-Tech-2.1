
using GLTech.Scripting;
using GLTech.Scripting.Physics;
using GLTech.Scripting.Prefab;
using GLTech.World;
using GLTech.World.Composed;

namespace GLTech.Tutorial
{
    class Map : Scene
    {
        Image golden = Utils.GetImageFromBitmap(Resources.Golden);
        Image universe = Utils.GetImageFromBitmap(Resources.Universe);

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
            //Engine.CaptureMouse = true;
            //Engine.FullScreen = true;
            //Engine.Run(scene);
        }
    }
}
