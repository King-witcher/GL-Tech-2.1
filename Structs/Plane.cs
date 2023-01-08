using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;
using Engine.World;

namespace Engine.Structs
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

        public void Add(Plane* plane)
        {
            Node* node = Node.Create(plane);
            Add(node);
            count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Plane* NearestPlane(Segment ray, out float distance, out float splitRatio)
        {
            unchecked
            {
                Plane* nearest = null;
                distance = float.PositiveInfinity;
                splitRatio = 2f;
                Node* cur = first;

                while (cur != null)
                {
                    cur->data->Test(ray, out float cur_dist, out float cur_ratio);
                    if (cur_dist < distance)
                    {
                        splitRatio = cur_ratio;
                        distance = cur_dist;
                        nearest = cur->data;
                    }
                    cur = cur->next;
                }

                return nearest;
            }
        }

        internal PlaneList CullByView(View view)
        {
            var planeList = new PlaneList();

            var current = first;

            while (current != null)
            {
                if(view.Contains(current->data->segment))
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
                Plane.Delete(current->data);
                Node.Delete(current);
                current = next;
            }
        }

        public struct Node
        {
            public Plane* data;
            public Node* next;

            public static Node* Create(Plane* plane)
            {
                Node* result = (Node*)Marshal.AllocHGlobal(sizeof(Node));
                result->data = plane;
                return result;
            }

            public static void Delete(Node* node)
            {
                Marshal.FreeHGlobal((IntPtr)node);
            }
        }
    }


    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Plane
    {
        internal Segment segment;
        internal Texture texture;               // Yes, by value.
        internal Plane* list_next;    // Planes are stored in scenes a linked list.

        internal static Plane* Create(Vector start, Vector end, Texture texture)
        {
            Plane* result = (Plane*)Marshal.AllocHGlobal(sizeof(Plane));
            result->texture = texture;
            result->segment = new(start, end - start);
            result->list_next = null;
            return result;
        }

        internal static Plane* Create(Vector start, float angle, float length, Texture texture)
        {
            Plane* result = (Plane*)Marshal.AllocHGlobal(sizeof(Plane));
            Vector direction = new Vector(angle) * length;
            result->texture = texture;
            result->segment = new(start, direction);
            result->list_next = null;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Test(Segment ray, out float cur_dist, out float cur_split)
        {
            segment.TestAgainstRay(ray, out cur_dist, out cur_split);
        }

        internal static void Delete(Plane* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
