using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;
using Engine.World;

namespace Engine.Data
{

    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SPlane
    {
        internal Vector start;
        internal Vector direction;
        internal Texture texture;               // Yes, by value.
        internal SPlane* list_next;    // Planes are stored in scenes a linked list.

        internal static SPlane* Create(Vector start, Vector end, Texture texture)
        {
            SPlane* result = (SPlane*)Marshal.AllocHGlobal(sizeof(SPlane));
            result->texture = texture;
            result->direction = end - start;
            result->start = start;
            result->list_next = null;
            return result;
        }

        internal static SPlane* Create(Vector start, float angle, float length, Texture texture)
        {
            SPlane* result = (SPlane*)Marshal.AllocHGlobal(sizeof(SPlane));
            Vector dir = new Vector(angle) * length;
            result->texture = texture;
            result->direction = dir;
            result->start = start;
            result->list_next = null;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Test(Ray ray, out float cur_dist, out float cur_split)
        {
            if (direction.x * ray.direction.y - direction.y * ray.direction.x <= 0)
            {
                cur_dist = float.PositiveInfinity;
                cur_split = 2f;
                return;
            }

            // Medium performance impact.
            float
                drx = direction.x,
                dry = direction.y;

            float det = ray.direction.x * dry - ray.direction.y * drx; // Caching can only be used here

            if (det == 0) // Parallel
            {
                cur_dist = float.PositiveInfinity;
                cur_split = 2f;
                return;
            }

            float spldet = ray.direction.x * (ray.start.y - start.y) - ray.direction.y * (ray.start.x - start.x);
            float dstdet = drx * (ray.start.y - start.y) - dry * (ray.start.x - start.x);
            float spltmp = spldet / det;
            float dsttmp = dstdet / det;
            if (spltmp < 0 || spltmp >= 1 || dsttmp <= 0) // dsttmp = 0 means column height = x/0.
            {
                cur_dist = float.PositiveInfinity;
                cur_split = 2f;
                return;
            }
            cur_split = spltmp;
            cur_dist = dsttmp;
            return;
        }

        internal static void Delete(SPlane* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
