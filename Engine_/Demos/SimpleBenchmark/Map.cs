using GLTech.World;

namespace GLTech.Demos.SimpleBenchmark
{
    internal class Map : Scene
    {
        Image bmp = Utils.GetImageFromBitmap(Resources.Bmp);

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
