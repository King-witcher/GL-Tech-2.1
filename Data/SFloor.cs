using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SFloor
    {
        internal Vector oa;
        private Vector ab;
        private Vector ac;
        private float det;
        internal SFloor* list_next;

        public Texture texture;

        internal static SFloor* Create(Vector a, Vector b, Vector c, Texture texture)
        {
            SFloor* result = (SFloor*)Marshal.AllocHGlobal(sizeof(SFloor));
            result->oa = a;
            result->ab = b - a;
            result->ac = c - a;
            result->det = result->ab.x * result->ac.y - result->ab.y * result->ac.x;
            result->list_next = null;
            result->texture = texture;
            return result;
        }

        public bool Contains(Vector point)
        {
            float a = (point.x * ac.y - point.y * ac.x) / det;
            float b = (point.y * ab.x - point.x * ab.y) / det;

            if (0 <= a + b && a + b < 1)
                if (0 <= a)
                    if (0 <= b)
                        return true;

            return false;
        }

        public Color MapTexture(Vector coordinates)
        {
            float xratio = (coordinates.x) % 1f;
            float yratio = (coordinates.y) % 1f;
            return texture.MapPixel(xratio, yratio);
        }
    }
}
