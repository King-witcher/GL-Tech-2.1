using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct HorizontalList : IDisposable
    {
        private Node* first;

        public HorizontalList()
        {
            first = null;
        }

        public void Add(Node* node)
        {
            node->next = first;
            first = node;
        }

        public void Add(HorizontalStruct* horizontal)
        {
            Node* node = Node.Create(horizontal);
            Add(node);
        }

        void Raise(Node* prev)
        {
            var cur = prev->next;
            prev->next = cur->next;
            cur->next = first;
            first = cur;
        }

        public HorizontalList GetIntersections(Vector a, Vector b)
        {
            var cur = first;
            var list = new HorizontalList();

            while(cur != null)
            {
                if (cur->data->Cull((a, b)))
                {
                    list.Add(Node.Create(cur->data));
                }
                cur = cur->next;
            }

            return list;
        }

        public HorizontalStruct* Locate(Vector point)
        {
            Node* prev = null;
            Node* cur = first;

            while (cur != null)
            {
                HorizontalStruct* data = cur->data;
                if (data->Contains(point))
                {
                    if (prev != null)
                        Raise(prev);
                    return cur->data;
                }
                prev = cur;
                cur = cur->next;
            }
            return null;
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

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct Node
        {
            public HorizontalStruct* data;
            public Node* next;

            public static Node* Create(HorizontalStruct* sfloor)
            {
                Node* result = (Node*)Marshal.AllocHGlobal(sizeof(Node));
                result->data = sfloor;
                return result;
            }

            public static void Delete(Node* node)
            {
                Marshal.FreeHGlobal((IntPtr) node);
            }
        }
    }


    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct HorizontalStruct
    {
        internal Vector tl;
        internal Vector br;

        public Texture texture;

        internal static HorizontalStruct* Create(Vector topLeft, Vector bottomRight, Texture texture)
        {
            HorizontalStruct* result = (HorizontalStruct*)Marshal.AllocHGlobal(sizeof(HorizontalStruct));
            result->tl = topLeft;
            result->br = bottomRight;
            result->texture = texture;
            return result;
        }

        internal bool Cull(Segment segment)
        {
            Vector center = (tl + br) / 2f;
            float radius = (tl - br).Module / 2f;

            float distance = Math.Abs((segment.start.y - center.y) * segment.direction.x - (segment.start.x - center.x) * segment.direction.y) / segment.direction.Module;

            return distance < radius;
        }

        public bool Contains(Vector point)
        {
            if (point.x > tl.x && point.x < br.x && point.y < tl.y && point.y > br.y)
                return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color MapTexture(Vector coordinates)
        {
            float xratio = (coordinates.x) % 1f;
            float yratio = (coordinates.y) % 1f;
            return texture.MapPixel(xratio, yratio);
        }
    }
}
