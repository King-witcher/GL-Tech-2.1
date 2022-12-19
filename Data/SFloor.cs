using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SFloor
    {
        internal Vector tl;
        internal Vector br;
        internal SFloor* list_next;

        public Texture texture;

        internal static SFloor* Create(Vector topLeft, Vector bottomRight, Texture texture)
        {
            SFloor* result = (SFloor*)Marshal.AllocHGlobal(sizeof(SFloor));
            result->tl = topLeft;
            result->br = bottomRight;
            result->list_next = null;
            result->texture = texture;
            return result;
        }

        public bool Contains(Vector point)
        {
            if (point.x > tl.x && point.x < br.x && point.y > tl.y && point.y < br.y)
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
