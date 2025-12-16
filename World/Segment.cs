using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Segment
    {
        internal Vector start;
        internal Vector direction;

        public Vector Start => start;

        public Vector Direction => direction;

        public Segment(Vector start, Vector direction)
        {
            this.start = start;
            this.direction = direction;
        }

        public Segment(Vector start, float angle)
        {
            this.start = start;
            direction = new Vector(angle);
        }

        public static Vector GetIntersectionRatio(Segment ab, Segment cd)
        {
            float cross_product = Vector.CrossProduct(ab.direction, cd.direction);

            // Parallel lines
            if (cross_product == 0)
                return Vector.Infinity;

            Vector ac = cd.start - ab.start;

            float x = Vector.CrossProduct(ac, cd.direction) / cross_product;
            float y = Vector.CrossProduct(ac, ab.direction) / cross_product;

            return new(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector GetRS(Segment other)
        {
            // Medium performance impact.
            float
                drx = direction.x,
                dry = direction.y;

            float det = dry * other.direction.x - drx * other.direction.y; // Caching can only be used here

            if (det == 0) // Parallel
            {
                return new Vector(float.PositiveInfinity, float.PositiveInfinity);
            }

            float idet = 1f / det;
            var delta = other.start - start;

            float r = idet * (other.direction.x * delta.y - other.direction.y * delta.x);
            float s = idet * (drx * delta.y - dry * delta.x);

            return new Vector(r, s);
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

        public override string ToString()
        {
            return $"<{start} -> {start + direction}>";
        }

        public static implicit operator Segment((Vector start, Vector end) tuple) =>
            new(tuple.start, tuple.end - tuple.start);
    }
}
