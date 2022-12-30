using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;
using Engine.World;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PlaneList
    {
        private Node* first;

        public PlaneList()
        {
            first = null;
        }

        public void Add(Node* node)
        {
            node->next = first;
            first = node;
        }

        public void Add(PlaneStruct* plane)
        {
            Node* node = Node.Create(plane);
            Add(node);
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

            public static Node* Create(PlaneStruct* plane)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Test(Segment ray, out float cur_dist, out float cur_split)
        {
            segment.TestAgainstRay(ray, out cur_dist, out cur_split);
        }

        internal static void Delete(PlaneStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
