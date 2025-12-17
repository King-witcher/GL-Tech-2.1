using GLTech.Imaging;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PlaneList : IDisposable
    {
        public Node* first;
        public int count;

        public PlaneList()
        {
            first = null;
            count = 0;
        }

        public void Add(Node* node)
        {
            node->next = first;
            first = node;
            count++;
        }

        public void Add(PlaneStruct* plane)
        {
            Node* node = Node.Create(plane);
            Add(node);
            count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PlaneStruct* NearestPlane(Segment ray, out Vector rs)
        {
            unchecked
            {
                rs = Vector.Infinity;
                PlaneStruct* nearest = null;
                Node* cur = first;

                while (cur != null)
                {
                    Vector cur_rs = ray.GetRS(cur->data->segment);

                    // If intersects before the ray starts or outside the plane bounds, skip.
                    if (cur_rs.x < 0f || cur_rs.y < 0f || cur_rs.y >= 1f)
                    {
                        cur = cur->next;
                        continue;
                    }

                    if (cur_rs.x < rs.x)
                    {
                        rs = cur_rs;
                        nearest = cur->data;
                    }
                    cur = cur->next;
                }

                return nearest;
            }
        }

        internal PlaneList CullBySurface(Vector position)
        {
            var planeList = new PlaneList();
            var current = first;

            while (current != null)
            {
                var left = current->data->segment.start - position;
                var right = current->data->segment.direction + left;

                if (Vector.CrossProduct(left, right) < 0)
                    planeList.Add(current->data);
                current = current->next;
            }

            return planeList;
        }

        internal PlaneList CullByFrustum(View view)
        {
            var planeList = new PlaneList();

            var current = first;

            while (current != null)
            {
                if (view.Contains(current->data->segment))
                    planeList.Add(current->data);
                current = current->next;
            }

            return planeList;
        }

        void Raise(Node* prev)
        {
            var cur = prev->next;
            prev->next = cur->next;
            cur->next = first;
            first = cur;
        }

        public void Dispose()
        {
            var current = first;

            while (current != null)
            {
                var next = current->next;
                Node.Delete(current);
                current = next;
            }
        }

        public void DeletePlanes()
        {
            var current = first;

            while (current != null)
            {
                var next = current->next;
                PlaneStruct.Delete(current->data);
                Node.Delete(current);
                current = next;
            }
        }

        public struct Node
        {
            public PlaneStruct* data;
            public Node* next;
            internal static int count;

            public static Node* Create(PlaneStruct* plane)
            {
                count++;
                Node* result = (Node*)Marshal.AllocHGlobal(sizeof(Node));
                result->data = plane;
                return result;
            }

            public static void Delete(Node* node)
            {
                count--;
                Marshal.FreeHGlobal((IntPtr)node);
            }
        }
    }


    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PlaneStruct
    {
        internal Segment segment;
        internal Texture texture;               // Yes, by value.
        internal PlaneStruct* list_next;    // Planes are stored in scenes a linked list.

        internal static PlaneStruct* Create(Vector start, Vector end, Texture texture)
        {
            PlaneStruct* result = (PlaneStruct*)Marshal.AllocHGlobal(sizeof(PlaneStruct));
            result->texture = texture;
            result->segment = new(start, end - start);
            result->list_next = null;
            return result;
        }

        internal static PlaneStruct* Create(Vector start, float angle, float length, Texture texture)
        {
            PlaneStruct* result = (PlaneStruct*)Marshal.AllocHGlobal(sizeof(PlaneStruct));
            Vector direction = new Vector(angle) * length;
            result->texture = texture;
            result->segment = new(start, direction);
            result->list_next = null;
            return result;
        }

        internal static void Delete(PlaneStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
