using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VisualPlaneData
    {
        internal Vector geom_start;
        internal Vector geom_direction;
        internal Texture texture;               // Yes, by value.
        internal VisualPlaneData* link_next;    // Visual planes are stored in scenes a linked list.

        internal static VisualPlaneData* Create(Vector start, Vector end, Texture texture)
        {
            VisualPlaneData* result = (VisualPlaneData*)Marshal.AllocHGlobal(sizeof(VisualPlaneData));
            result->texture = texture;
            result->geom_direction = end - start;
            result->geom_start = start;
            result->link_next = null;
            return result;
        }

        internal static VisualPlaneData* Create(Vector start, float angle, float length, Texture texture)
        {
            VisualPlaneData* result = (VisualPlaneData*)Marshal.AllocHGlobal(sizeof(VisualPlaneData));
            Vector dir = new Vector(angle) * length;
            result->texture = texture;
            result->geom_direction = dir;
            result->geom_start = start;
            result->link_next = null;
            return result;
        }

        internal static void Delete(VisualPlaneData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
