using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct TriangleStruct
    {
        public Vector a;
        public Vector b;
        public Vector c;

        // Inward edge normals and offsets, precomputed at creation. A point p is
        // inside iff dot(n_i, p) >= d_i for the three edges. The renderer's span
        // clipper consumes these directly, so Translate must keep them in sync.
        public Vector n0;
        public Vector n1;
        public Vector n2;
        public float d0;
        public float d1;
        public float d2;

        public Texture texture;

        public static TriangleStruct* Create(Vector a, Vector b, Vector c, Texture texture)
        {
            // Winding is normalized to counter-clockwise so the left normal of each
            // directed edge always points inward.
            if (Vector.CrossProduct(b - a, c - a) < 0f)
                (b, c) = (c, b);

            TriangleStruct* result = (TriangleStruct*)Marshal.AllocHGlobal(sizeof(TriangleStruct));
            result->a = a;
            result->b = b;
            result->c = c;
            result->texture = texture;
            result->n0 = LeftNormal(b - a);
            result->n1 = LeftNormal(c - b);
            result->n2 = LeftNormal(a - c);
            result->d0 = Vector.DotProduct(result->n0, a);
            result->d1 = Vector.DotProduct(result->n1, b);
            result->d2 = Vector.DotProduct(result->n2, c);
            return result;
        }

        public static void Delete(TriangleStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }

        static Vector LeftNormal(Vector v) => new Vector(-v.y, v.x);

        public void Translate(Vector delta)
        {
            a += delta;
            b += delta;
            c += delta;
            d0 += Vector.DotProduct(n0, delta);
            d1 += Vector.DotProduct(n1, delta);
            d2 += Vector.DotProduct(n2, delta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vector point)
        {
            return Vector.DotProduct(n0, point) >= d0
                && Vector.DotProduct(n1, point) >= d1
                && Vector.DotProduct(n2, point) >= d2;
        }
    }

    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct TriangleList : IDisposable
    {
        public Node* first;
        public int count;

        public void Add(TriangleStruct* triangle)
        {
            Node* node = (Node*)Marshal.AllocHGlobal(sizeof(Node));
            node->data = triangle;
            node->next = first;
            first = node;
            count++;
        }

        public TriangleStruct* Find(Vector point)
        {
            for (Node* cur = first; cur != null; cur = cur->next)
                if (cur->data->Contains(point))
                    return cur->data;
            return null;
        }

        // Frees the nodes only; the triangles are owned by their entities.
        public void Dispose()
        {
            Node* current = first;
            while (current != null)
            {
                Node* next = current->next;
                Marshal.FreeHGlobal((IntPtr)current);
                current = next;
            }
            first = null;
            count = 0;
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {
            public TriangleStruct* data;
            public Node* next;
        }
    }
}
