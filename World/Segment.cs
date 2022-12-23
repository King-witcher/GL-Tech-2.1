using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.World
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Segment
    {
        internal Vector start;
        internal Vector direction;

        public Vector A => start;

        public Vector B => direction;

        public Segment(Vector a, Vector b)
        {
            this.start = a;
            this.direction = b;
        }

        public Segment(Vector a, float angle)
        {
            this.start = a;
            direction = new Vector(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TestAgainstRay(Segment ray, out float cur_dist, out float cur_split)
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

        public bool TestIntersect(Segment segment, out Vector intersection)
        {
            if (direction.x * segment.direction.y - direction.y * segment.direction.x <= 0)
            {
                intersection = Vector.Zero;
                return false;
            }

            // Medium performance impact.
            float
                drx = direction.x,
                dry = direction.y;

            float det = segment.direction.x * dry - segment.direction.y * drx; // Caching can only be used here

            if (det == 0) // Parallel
            {
                intersection = Vector.Zero;
                return false;
            }

            float spldet = segment.direction.x * (segment.start.y - start.y) - segment.direction.y * (segment.start.x - start.x);
            float dstdet = drx * (segment.start.y - start.y) - dry * (segment.start.x - start.x);
            float spltmp = spldet / det;
            float dsttmp = dstdet / det;
            if (spltmp < 0f || spltmp >= 1f || dsttmp <= 0f || dsttmp > 1f) // dsttmp = 0 means column height = x/0.
            {
                intersection = Vector.Zero;
                return false;
            }

            intersection = segment.start + spltmp * (segment.direction - segment.start);
            return true;
        }
    }
}
