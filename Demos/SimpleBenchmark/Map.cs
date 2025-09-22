using Engine.Imaging;
using Engine.World;

namespace Engine.Demos.SimpleBenchmark
{
    internal class Map : Scene
    {
        Image bmp = new(Resources.Bmp);

        protected override void Delete()
        {
            bmp.Dispose();
        }

        public Map()
        {
            Texture tex = new(bmp);
            Plane plane = new((-1f, 0.5f), (1f, 0.5f), tex);
            Add(plane);
        }
    }
}
