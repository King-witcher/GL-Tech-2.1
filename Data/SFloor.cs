using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal struct STriFloor
    {
        private Vector oa;
        private Vector ab;
        private Vector ac;
        private float det;

        public Texture texture;

        public STriFloor(Vector a, Vector b, Vector c, Texture texture = default)
        {
            oa = a;
            ab = b - a;
            ac = c - a;
            det = ab.x * ac.y - ab.y * ac.x;

            System.Console.WriteLine(det);

            this.texture = texture;
        }


        public bool Contains(Vector point)
        {
            float a = (point.x * ac.y - point.y * ac.x) / det;
            float b = (point.y * ab.x - point.x * ab.y) / det;

            if (0 <= a && a < 1 && 0 <= b && b < 1)
                return true;

            return false;
        }

        public Color MapTexture(Vector cordinates)
        {
            return default;
        }
    }
}
