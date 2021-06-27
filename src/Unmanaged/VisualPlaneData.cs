using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VisualPlaneData
    {
        internal Vector geom_direction;
        internal Vector geom_start;
        internal Texture texture; // Yes, by value.

        internal static VisualPlaneData* Create(Vector start, Vector end, Texture texture)
        {
            VisualPlaneData* result = (VisualPlaneData*)Marshal.AllocHGlobal(sizeof(VisualPlaneData));
            result->texture = texture;
            result->geom_direction = end - start;
            result->geom_start = start;
            return result;
        }

        internal static VisualPlaneData* Create(Vector start, float angle, float length, Texture texture)
        {
            VisualPlaneData* result = (VisualPlaneData*)Marshal.AllocHGlobal(sizeof(VisualPlaneData));
            Vector dir = new Vector(angle) * length;
            result->texture = texture;
            result->geom_direction = dir;
            result->geom_start = start;
            return result;
        }

        internal static void Delete(VisualPlaneData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
