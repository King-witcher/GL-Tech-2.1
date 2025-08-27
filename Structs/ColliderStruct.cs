using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.World;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    internal unsafe struct ColliderStruct
    {
        internal Vector start;
        internal Vector direction;
        internal ColliderStruct* list_next;
        static internal int count;

        internal static ColliderStruct* Create(Vector start, Vector end)
        {
            count++;
            ColliderStruct* result = (ColliderStruct*)Marshal.AllocHGlobal(sizeof(ColliderStruct));
            result->start = start;
            result->direction = end - start;
            result->list_next = null;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Test(Segment ray, out float distance)
        {
            // Culling
            if (direction.x * ray.direction.y - direction.y * ray.direction.x <= 0)
            {
                distance = float.PositiveInfinity;
                return false;
            }

            float
                drx = direction.x,
                dry = direction.y;

            float det = ray.direction.x * dry - ray.direction.y * drx;

            if (det == 0)
            {
                distance = float.PositiveInfinity;
                return false;
            }

            float spldet = ray.direction.x * (ray.start.y - start.y) - ray.direction.y * (ray.start.x - start.x);
            float dstdet = drx * (ray.start.y - start.y) - dry * (ray.start.x - start.x);
            float spltmp = spldet / det;
            float dsttmp = dstdet / det;
            if (spltmp < 0 || spltmp >= 1 || dsttmp <= 0) // dsttmp = 0 means column height = x/0.
            {
                distance = float.PositiveInfinity;
                return false;
            }
            distance = dsttmp;
            return true;
        }

        internal static void Delete(ColliderStruct* item)
        {
            count--;
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
