using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct WallData
    {
        internal Vector geom_direction;
        internal Vector geom_start;
        internal Texture texture; // Yes, by value.

        internal static WallData* Create(Vector start, Vector end, Texture texture)
        {
            WallData* result = (WallData*)Marshal.AllocHGlobal(sizeof(WallData));
            result->texture = texture;
            result->geom_direction = end - start;
            result->geom_start = start;
            return result;
        }

        internal static WallData* Create(Vector start, float angle, float length, Texture texture)
        {
            WallData* result = (WallData*)Marshal.AllocHGlobal(sizeof(WallData));
            Vector dir = new Vector(angle) * length;
            result->texture = texture;
            result->geom_direction = dir;
            result->geom_start = start;
            return result;
        }

        internal static void Delete(WallData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
