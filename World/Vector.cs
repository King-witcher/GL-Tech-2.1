using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vector : IEquatable<Vector>
    {
        private const float TORAD = (float)(Math.PI / 180.0);
        private const float TODEGREE = (float)(180.0 / Math.PI);

        internal readonly float x;
        internal readonly float y;

        public float X { get => x; }
        public float Y { get => y; }
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector(float angle) // Not optimized by fast sin and fast cos
        {
            x = (float)Math.Sin(angle * TORAD);
            y = (float)Math.Cos(angle * TORAD);
        }

        public static Vector FromAngle(float angle, float module)
        {
            float x = (float)Math.Sin(angle * Math.PI / 180);
            float y = (float)Math.Cos(angle * Math.PI / 180);
            return new Vector(module * x, module * y);
        }

        public static Vector GetRandom(float maxModule)
        {
            Random rnd = new Random();

            return new Vector((float)rnd.NextDouble() * 360f)
                * ((float)rnd.NextDouble() * maxModule);
        }

        public float Angle      // Not optimized
        {
            get
            {
                if (Module == 0)
                    return 0;

                float temp = 180f * (float)Math.Asin(x / Module) / (float)Math.PI;
                if (y > 0)
                    return temp;
                else
                    return 180 - temp;
            }
            set
            {
                float delta = value - this.Angle;
                this *= new Vector(delta);
            }
        }

        public float Module
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt(x * x + y * y);
            set
            {
                float delta = value / Module;
                this *= delta;
            }
        }

        public static Vector Zero { get => new Vector(0, 0); }
        public static Vector Forward { get => new Vector(0, 1); }
        public static Vector Right { get => new Vector(1, 0); }
        public static Vector Backward { get => new Vector(0, -1); }
        public static Vector Left { get => new Vector(-1, 0); }

        public static Vector[] GetRegularPolygon(Vector center, float radius, int edges)
        {
            if (edges == 0)
                throw new ArgumentException("\"edges\" cannot be 0.", "edges");

            Vector[] result = new Vector[edges];
            for (int i = 0; i < edges; i++)
                result[i] = center + radius * new Vector(-i * 360 / edges);
            return result;
        }

        public float GetDistance(Vector vector)
        {
            float dx = vector.x - x;
            float dy = vector.y - y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public Vector Projection(Vector origin, Vector normal)
        {
            return (this - origin) / normal;
        }

        public Vector Disprojection(Vector origin, Vector normal)
        {
            return this * normal + origin;
        }

        public override string ToString()
        {
            return $"<{x}, {y}>";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }
            bool xeq = x.Equals(((Vector)obj).x);
            bool yeq = y.Equals(((Vector)obj).y);
            if (xeq && yeq)
                return true;
            else
                return false;
        }

        public static float DotProduct(Vector left, Vector right) =>
            left.x * right.x + left.y * right.y;

        public bool Equals(Vector other)
        {
            return x == other.x &&
                   y == other.y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public static Vector operator -(Vector vector) =>
            new Vector(
                -vector.x,
                -vector.y);

        public static Vector operator +(Vector left, Vector right) =>
            new Vector(
                left.x + right.x,
                left.y + right.y);

        public static Vector operator -(Vector left, Vector right) =>
            new Vector(
                left.x - right.x,
                left.y - right.y);

        public static Vector operator *(Vector left, Vector right) =>
            new Vector(
                left.y * right.x + left.x * right.y,
                left.y * right.y - left.x * right.x);

        public static Vector operator /(Vector dividend, Vector divider)
        {
            float delta = divider.x * divider.x + divider.y * divider.y;
            if (delta == 0)
                throw new DivideByZeroException("Divider vector cannot be <0, 0>.");

            float x = (dividend.x * divider.y - dividend.y * divider.x) / delta;
            float y = (dividend.y * divider.y + dividend.x * divider.x) / delta;
            return new Vector(x, y);
        }

        public static Vector operator *(float scalar, Vector vector) =>
            new Vector(
                vector.x * scalar,
                vector.y * scalar);

        public static Vector operator *(Vector vector, float scalar) =>
            scalar * vector;

        public static Vector operator /(Vector vector, float scalar) =>
            new Vector(
                vector.x / scalar,
                vector.y / scalar);

        public static bool operator ==(Vector left, Vector right)
        {
            bool xeq = left.x == right.x;
            bool yeq = left.y == right.y;
            return xeq && yeq;
        }

        public static bool operator !=(Vector left, Vector right)
        {
            bool xdif = left.x != right.x;
            bool ydif = left.y != right.y;
            return xdif || ydif;
        }

        public static implicit operator Vector((float x, float y) components)
        {
            return new Vector(components.x, components.y);
        }
    }
}
