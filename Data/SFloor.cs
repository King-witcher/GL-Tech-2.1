using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal struct SFloor
    {
        public Vector up_left;
        public Vector down_right;
        public Texture texture;

        public bool Contains(Vector point)
        {
            if (up_left.x <= point.x && up_left.y <= point.y)
                if (down_right.x > point.x && down_right.y > point.y)
                    return true;
            return false;
        }
    }
}
